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
            return View();
        }


        [Login] 
        public ActionResult Inicio()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(string cpf, string senha)
        {
            cpf = cpf.Replace(".", "").Replace("-", "").Replace(",", "").Replace("/", "").Replace("\\", "").Trim().ToLower();

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
                        try
                        {
                            int loja = _db.Int_RH_Usuarios.Where(a => a.Id == user.Id).Select(s => s.Loja.Id).FirstOrDefault();
                            user.Loja = _db.Int_DP_Lojas.Find(loja);

                            int setor = _db.Int_RH_Usuarios.Where(a => a.Id == user.Id).Select(s => s.Setor.Id).FirstOrDefault();
                            user.Setor = _db.Int_DP_Setores.Find(setor);

                            HttpContext.Session.SetInt32("ID", user.Id);
                            HttpContext.Session.SetString("CPF", user.CPF);
                            HttpContext.Session.SetInt32("Nivel", user.Nivel);
                            HttpContext.Session.SetInt32("Loja", user.Loja.Id);
                            HttpContext.Session.SetInt32("Setor", user.Setor.Id);
                            HttpContext.Session.SetString("UltimoAcesso", Globalization.DataHoraExtensoBR(user.UltimoAcesso));

                            user.UltimoAcesso = Globalization.HoraAtualBR();

                            _db.SaveChanges();

                            return RedirectToAction("Inicio");

                        }
                        catch (Exception exp)
                        {
                            string mensagem = exp.Message;

                            TempData["LoginErro"] = "Ocorreu um erro ao tentar efetuar o login";
                            return RedirectToAction("Index");
                        }
                        
                    }
                    else
                    {
                        TempData["LoginErro"] = "Senha Incorreta!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["LoginErro"] = "Usuário não está ativo";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["LoginErro"] = "Usuário não encontrado!";
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
                        user_cad.Celular = user.Celular;
                        user_cad.Telefone = user.Telefone;
                        user_cad.Nivel = 2;
                        user_cad.Senha = Criptografia.GetMd5Hash(user.Senha);
                        user_cad.Cadastrado = 1;
                        user_cad.Ativo = 1;

                        _db.SaveChanges();

                        TempData["CadastroOK"] = "Cadastro efetuado com Sucesso!";

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




        public ActionResult EsqueciMinhaSenha()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidarCadastro(string cpf, int? codigo)
        {

            int code = codigo ?? 0;

            cpf = cpf.Replace(",", "").Replace(".", "").Replace("-", "").Replace("/", "").Replace("\\", "");

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
                        TempData["CadastreSeNotOK"] = "Código não conferem com este CPF!";
                    }
                }
                else
                {
                    TempData["CadastreSeNotOK"] = "Já exixte um cadastro com este CPF!";
                }
            }
            else
            {
                TempData["CadastreSeNotOK"] = "CPF não encontrado!";
            }


            return RedirectToAction("Index", "Home");

        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}