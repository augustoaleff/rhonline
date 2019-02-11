using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using RHOnline.Library;
using RHOnline.Database;
using Microsoft.AspNetCore.Http;
using RHOnline.Library.Filters;
using RHOnline.Models;
using RHOnline.Library.Globalization;
using System.Xml.Linq;
using System.IO;
using RHOnline.Models.Logs;

namespace RHOnline.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext _db;

        public HomeController(DatabaseContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {

            //Verifica se já há alguma sessão ativa
            if(HttpContext.Session.GetInt32("ID") != null)
            {
                return RedirectToAction("Inicio");
            }
            else
            {
                return View();
            }
            
        }
        
        [Login]
        public IActionResult Inicio()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string cpf, string senha)
        {
            cpf = Shared.RetirarCaracteres(cpf);

            Usuario user = _db.Int_RH_Usuarios.Where(a => a.CPF.Equals(cpf)).FirstOrDefault();

            if (user != null)
            {
                ViewBag.User = user;

                if (user.Ativo == 1)
                {
                    senha = senha.Replace(",", "").Replace(".", "").Replace("'", "").Replace(".", "").Trim();

                    bool senha_comparacao = Criptografia
                        .VerifyMd5Hash(senha.Replace(",", "").Replace(".", "").Replace("'", "").Trim(), user.Senha);

                    if (senha_comparacao)
                    {
                        Log log = new Log();

                        try
                        {
                            int loja = _db.Int_RH_Usuarios.Where(a => a.Id == user.Id).Select(s => s.Loja.Id).FirstOrDefault();
                            user.Loja = _db.Int_DP_Lojas.Find(loja);

                            int setor = _db.Int_RH_Usuarios.Where(a => a.Id == user.Id).Select(s => s.Setor.Id).FirstOrDefault();
                            user.Setor = _db.Int_DP_Setores.Find(setor);

                            HttpContext.Session.SetInt32("ID", user.Id);
                            HttpContext.Session.SetString("Nome", user.Nome);
                            HttpContext.Session.SetString("CPF", user.CPF);
                            HttpContext.Session.SetInt32("Nivel", user.Nivel);
                            HttpContext.Session.SetInt32("Loja", user.Loja.Id);
                            HttpContext.Session.SetInt32("Setor", user.Setor.Id);
                            HttpContext.Session.SetString("UltimoAcesso", Globalization.DataHoraExtensoBR(user.UltimoAcesso));

                            user.UltimoAcesso = Globalization.HoraAtualBR();

                            _db.SaveChanges();

                            log.LogIn(user.Id);
                            
                            return RedirectToAction("Inicio");

                        }
                        catch (Exception exp)
                        {
                            string mensagem = exp.Message;

                            TempData["MensagemErroIndex"] = "Ocorreu um erro ao tentar efetuar o login";

                            log.LogIn_Erro(user.Id, exp);

                            return RedirectToAction("Index");
                        }
                        finally
                        {
                            _db.Int_RH_Logs.Add(log);
                            _db.SaveChanges();
                        }

                    }
                    else
                    {
                        TempData["MensagemErroIndex"] = "Senha Incorreta!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["MensagemErroIndex"] = "Usuário não está ativo";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["MensagemErroIndex"] = "Usuário não encontrado!";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Cadastrar([FromForm]Usuario user, string confirmasenha)
        {
            int loja = _db.Int_RH_Usuarios.Where(a => a.Id == user.Id).Select(s => s.Loja.Id).FirstOrDefault();
            user.Loja = _db.Int_DP_Lojas.Find(loja);

            int setor = _db.Int_RH_Usuarios.Where(a => a.Id == user.Id).Select(s => s.Setor.Id).FirstOrDefault();
            user.Setor = _db.Int_DP_Setores.Find(setor);

            ViewBag.User = user;

            if (ValidarSenha(user.Senha))
            {
                if (user.Senha.Equals(confirmasenha))
                {
                    try
                    {
                        user.Senha = user.Senha.Replace(",", "").Replace(".", "").Replace("'", "").Trim();

                        Usuario user_cad = _db.Int_RH_Usuarios.Find(user.Id);

                        user_cad.DataCadastro = Globalization.HoraAtualBR();
                        user_cad.Email = user.Email;
                        user_cad.Celular = Shared.RetirarCaracteres(user.Celular);
                        user_cad.Telefone = Shared.RetirarCaracteres(user.Telefone);
                        
                        user_cad.Nivel = 2; //Usuário Comum
                        user_cad.Senha = Criptografia.GetMd5Hash(user.Senha);
                        user_cad.Cadastrado = 1;
                        user_cad.Ativo = 1;

                        _db.SaveChanges();

                        TempData["MensagemSucessoIndex"] = "Cadastro efetuado com Sucesso!";

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception)
                    {
                        TempData["CadastroNotOK"] = "Ocorreu um erro ao tentar cadastrar, por favor, tente novamente";
                    }
                }
                else
                {
                    TempData["SenhaNaoValidada"] = "Senhas não conferem!";
                }
            }

            return View();
        }

        public bool ValidarSenha(string senha)
        {
            if (senha.Length >= 8)
            {
                if (senha.Length <= 20)
                {
                    if (senha.ToLower() != senha)
                    {
                        if (senha.ToUpper() != senha)
                        {
                            if (senha.Contains("0") || senha.Contains("1") || senha.Contains("2") ||
                                senha.Contains("3") || senha.Contains("4") || senha.Contains("5") ||
                                senha.Contains("6") || senha.Contains("7") || senha.Contains("8") || senha.Contains("9"))
                            {
                                return true;
                            }
                            else
                            {
                                TempData["SenhaNaoValidada"] = "A Senha deve conter pelo menos um número";
                                return false;
                            }
                        }
                        else
                        {
                            TempData["SenhaNaoValidada"] = "A Senha deve conter pelo menos uma letra minúscula";
                            return false;
                        }
                    }
                    else
                    {
                        TempData["SenhaNaoValidada"] = "A Senha deve conter pelo menos uma letra maiúscula";
                        return false;
                    }
                }
                else
                {
                    TempData["SenhaNaoValidada"] = "A Senha deve conter menos de 20 caracteres";
                    return false;
                }
            }
            else
            {
                TempData["SenhaNaoValidada"] = "A Senha deve conter mais de 8 caracteres";
                return false;
            }
        }


        //Trocar Senha por envio e link no email
        public ActionResult TrocarSenha()
        {
            return View();
        }


        //Trocar Senha por envio e link no email
        [HttpGet]
        public ActionResult TrocarSenha(long? key)
        {
            long key2 = key ?? 1;

            DateTime agora = Globalization.HoraAtualBR();
            var vKey = _db.Int_DP_ValidSenhas.Find(key2);

            if (vKey != null)
            {

                if (vKey.DataExpiracao >= agora)
                {
                    if (vKey.Utilizado == 0)
                    {
                        Usuario usuario = _db.Int_RH_Usuarios.Find(vKey.Usuario);

                        vKey.Utilizado = 1;
                        _db.SaveChanges();

                        ViewBag.Senha = "";
                        ViewBag.Nome = usuario.Nome;
                        ViewBag.Email = usuario.Email;
                        ViewBag.IdSenha = usuario.Id;
                        


                        ViewBag.TrocarSenha = vKey;

                        return View();
                    }
                    else
                    {
                        //Quando o link já foi utilizado
                        TempData["MensagemErroIndex"] = "Link já Utilizado";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["MensagemErroIndex"] = "Link Expirado";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["MensagemErroIndex"] = "Link Inválido";
                return RedirectToAction("Index");

            }
        }
        
        [Login]
        public ActionResult AlterarMinhaSenha()
        {

            int id_notnull = HttpContext.Session.GetInt32("ID") ?? 0;

            Usuario usuario = _db.Int_RH_Usuarios.Find(id_notnull);
            
            return View();
        }

        [Login]
        [HttpPost]
        public ActionResult AlterarMinhaSenha([FromForm]int? id, string senha, string confirmasenha, string senhaAnterior)
        {

            ViewBag.Senha = senha;
            ViewBag.SenhaAnterior = senhaAnterior;
            
            int id_notnull = id ?? 0;

            Usuario usuario = _db.Int_RH_Usuarios.Find(id_notnull);

            if (senha == confirmasenha)
            {
                if (ValidarSenha(senha))
                {
                    if (Criptografia.VerifyMd5Hash(senhaAnterior, usuario.Senha))
                    {
                        Log log = new Log();

                        try
                        {
                            usuario.Senha = Criptografia.GetMd5Hash(senha);
                            _db.SaveChanges();

                            log.AlterarMinhaSenha(id_notnull);
                            TempData["AlterarMinhaSenhaOK"] = "Senha alterada com sucesso!";
                            ViewBag.Senha = "";

                            return RedirectToAction("Inicio");

                        }
                        catch (Exception exp)
                        {
                            log.AlterarMinhaSenha_Erro(id_notnull, exp);
                            TempData["AlterarMinhaSenhaNotOK"] = "Ocorreu um Erro ao tentar alterar a senha, por favor, tente novamente mais tarde!";
                        }
                        finally
                        {
                            _db.Int_RH_Logs.Add(log);
                            _db.SaveChanges();
                        }

                    }
                    else
                    {
                        TempData["SenhaAnteriorNotOK"] = "Senha Anterior Incorreta!";
                    }
                }
            }
            else
            {
                TempData["SenhaNaoValidada"] = "Senhas não conferem!";
            }

            return View();

        }
        
        //Trocar Senha por envio e link no email
        [HttpPost]
        public ActionResult TrocarSenha([FromForm]int? id, string nome, string email, string cpf, string senha, string confirmasenha)
        {
            int id_notnull = id ?? 0;

            ViewBag.Senha = senha;
            ViewBag.Nome = nome;
            ViewBag.Email = email;
            ViewBag.IdSenha = id;
            ViewBag.CPF = string.Format("{0:###.###.###-##}", cpf); 
            
            cpf = Shared.RetirarCaracteres(cpf);
            
            Usuario vUsuario = _db.Int_RH_Usuarios.Find(id);

            if (cpf == vUsuario.CPF)
            {
                if (senha == confirmasenha)
                {
                    //Log log = new Log();

                    if (ValidarSenha(senha))
                    {
                        Log log = new Log();

                        try
                        {
                            vUsuario.Senha = Criptografia.GetMd5Hash(senha);

                            _db.SaveChanges();

                            log.EsqueciMinhaSenha_Troca(vUsuario.Id);

                            TempData["MensagemSucessoIndex"] = "Senha Alterada com sucesso";
                            return RedirectToAction("Index");
                        }
                        catch (Exception exp)
                        {
                            log.EsqueciMinhaSenha_Troca_Erro(id_notnull, exp);

                            TempData["TrocaSenhaNotOK"] = "Ocorreu um Erro ao tentar redefinir a senha, por favor, tente novamente mais tarde!";
                            
                            return View();
                        }
                        finally
                        {
                            _db.Int_RH_Logs.Add(log);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    TempData["SenhaNaoValidada"] = "Senhas não conferem!";
                    return View();
                }
            }
            else
            {
                TempData["CPFErro"] = "CPF não confere!";
                return View();
            }
        }


        public ActionResult EnviarLinkSenha(string cpf)
        {

            string codigo;
            DateTime agora = Globalization.HoraAtualBR();
            
            cpf = Shared.RetirarCaracteres(cpf);

            Usuario user = _db.Int_RH_Usuarios.Where(a => a.CPF.Equals(cpf)).FirstOrDefault();

            if (user != null)
            {
                if (user.Email != null && user.Email != "")
                {
                    codigo = "99" + user.Id.ToString() + agora.Minute.ToString() + agora.Month.ToString() + agora.Day.ToString() +
                         agora.Year.ToString().Substring(2, 2) + user.CPF.Substring(3, 3) + agora.Hour.ToString();

                    ValidacaoSenha valid = new ValidacaoSenha
                    {
                        Id = long.Parse(codigo),
                        Data = agora,
                        DataExpiracao = agora.AddHours(5),
                        Utilizado = 0,
                        Usuario = user.Id
                    };

                    _db.Int_DP_ValidSenhas.Add(valid);
                    _db.SaveChanges();

                    Log log = new Log();

                    try
                    {
                        string nome = Shared.PegarPrimeiroNome(user.Nome);

                        Library.Mail.EnviarLinkSenha.EnviarLinkTrocarSenha(user.Email, codigo, nome);

                        //log.EsqueciMinhaSenha_Envio(vEmail.Id, codigo);

                        log.EsqueciMinhaSenha_Envio(user.Id, codigo);
                        
                        TempData["MensagemSucessoIndex"] = "Um link foi enviado ao seu e-mail com instruções para trocar a senha";

                        return RedirectToAction("Index");
                    }
                    catch (Exception exp)
                    {
                        //log.EsqueciMinhaSenha_Envio_Erro(vEmail.Id, codigo, exp);

                        TempData["MensagemErroIndex"] = "Ocorreu um erro ao tentar enviar o link, por favor tente novamente";

                        log.EsqueciMinhaSenha_Envio_Erro(user.Id, codigo, exp);

                        return RedirectToAction("Index");
                    }
                    finally
                    {
                        _db.Int_RH_Logs.Add(log);
                        _db.SaveChanges();
                    }

                }
                else
                {
                    TempData["MensagemErroIndex"] = "Não há e-mail cadastrado para esse CPF, entre em contato com o TI!";
                }
            }
            else
            {
                TempData["MensagemErroIndex"] = "CPF não encontrado!";
            }

            return RedirectToAction("Index");
        }

        [Login]
        public ActionResult AtualizarCadastro()
        {
            int id = HttpContext.Session.GetInt32("ID") ?? 0;

            Usuario user = _db.Int_RH_Usuarios.Find(id);

            ViewBag.User = user;

            return View();
        }
        
        [Login]
        [HttpPost]
        public ActionResult AtualizarCadastro(int? id, string email, string telefone, string celular)
        {
            int id_notnull = id ?? 0;

            Usuario usuario = _db.Int_RH_Usuarios.Find(id_notnull);

            ViewBag.User = usuario;

            telefone = Shared.RetirarCaracteres(telefone);

            celular = Shared.RetirarCaracteres(celular);

            email = email.ToLower();

            try
            {
                usuario.Telefone = telefone;
                usuario.Celular = celular;
                usuario.Email = email;

                _db.SaveChanges();

                TempData["MensagemSucessoInicio"] = "Cadastro atualizado com sucesso!";

                return RedirectToAction("Inicio");

            }
            catch (Exception)
            {

                TempData["AtualizarCadastroNotOk"] = "Ocorreu um erro ao tentar atualizar o cadastro";

                return View();

            } 
        }
        
        [HttpPost]
        public ActionResult ValidarCadastro(string cpf, int? codigo)
        {
            int code = codigo ?? 0;
            
            cpf = Shared.RetirarCaracteres(cpf);

            Usuario user = _db.Int_RH_Usuarios.Where(a => a.CPF.Equals(cpf)).FirstOrDefault();

            int loja = _db.Int_RH_Usuarios.Where(a => a.Id == user.Id).Select(s => s.Loja.Id).FirstOrDefault();
            user.Loja = _db.Int_DP_Lojas.Find(loja);

            int setor = _db.Int_RH_Usuarios.Where(a => a.Id == user.Id).Select(s => s.Setor.Id).FirstOrDefault();
            user.Setor = _db.Int_DP_Setores.Find(setor);

            if (user != null)
            {
                if (user.Cadastrado == 0)
                {
                    if (user.CodigoAtivacao == code && code != 0)
                    {
                        ViewBag.User = user;
                        return View("Cadastrar");
                    }
                    else
                    {
                        TempData["MensagemErroIndex"] = "Código não conferem com este CPF!";
                    }
                }
                else
                {
                    TempData["MensagemErroIndex"] = "Já exixte um cadastro com este CPF!";
                }
            }
            else
            {
                TempData["MensagemErroIndex"] = "CPF não encontrado!";
            }
            
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Logout()
        {

            Log log = new Log();

            int user_notnull = HttpContext.Session.GetInt32("ID") ?? 0;

            try
            {
                HttpContext.Session.Clear();
                log.LogOut(user_notnull);

            }
            catch (Exception exp)
            {
                log.LogOut_Erro(user_notnull, exp);
            }
            finally
            {
                _db.Int_RH_Logs.Add(log);
                _db.SaveChanges();
            }


            return RedirectToAction("Index");
        }
    }
}