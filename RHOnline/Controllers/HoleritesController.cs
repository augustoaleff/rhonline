using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RHOnline.Database;
using RHOnline.Library;
using RHOnline.Library.Filters;
using RHOnline.Library.Globalization;
using RHOnline.Models;
using RHOnline.Models.ViewModels;
using Rotativa.AspNetCore;

namespace RHOnline.Controllers
{
    [Login]
    public class HoleritesController : Controller
    {

        private DatabaseContext _db;

        public HoleritesController(DatabaseContext db)
        {
            _db = db;
        }

        

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Holerite()
        {
            //LerArquivo();
            HoleriteVM holerite = CriarHolerite();

            string data = Globalization.DataRelatorioPdfBR();

            ViewAsPdf relatorioPDF = new ViewAsPdf
            {
                WkhtmlPath = "~/RHOnline/wwwroot/Rotativa",
                ViewName = "Holerite",
                IsGrayScale = false,
                Model = holerite,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--page-offset 0 --footer-left " + data + " --footer-right [page]/[toPage] --footer-font-size 8",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(0,0,0,0)
            };

            return relatorioPDF;

        }

        [Admin]
        [HttpGet]
        public ActionResult EnviarArquivo()
        {

            List<Arquivo> arquivos = _db.Int_RH_Arquivos.OrderByDescending(o => o.DataEnvio).Take(20).ToList();

            return View(arquivos);
        }

        //TEMPORÁRIO
        public HoleriteVM CriarHolerite()
        {

            HoleriteVM holerite = new HoleriteVM();

            int id_user = HttpContext.Session.GetInt32("ID") ?? 0;

            holerite.Usuario = _db.Int_RH_Usuarios.Find(id_user);

            int loja = _db.Int_RH_Usuarios.Where(a => a.Id == id_user).Select(s => s.Loja.Id).FirstOrDefault();
            
            holerite.Usuario.Loja = _db.Int_RH_Lojas.Find(loja);

            Evento evento1 = new Evento()
            {
                Codigo = 1,
                Descricao = "SALARIO",
                Referencia = 50.0,
                Vencimento = 1000.0,
                Desconto = 0.0
            };

            Evento evento2 = new Evento()
            {
                Codigo = 34,
                Descricao = "TESTE",
                Referencia = 50.0,
                Vencimento = 1000.0,
                Desconto = 0.0
            };

            Evento evento3 = new Evento()
            {
                Codigo = 28,
                Descricao = "TESTE2",
                Referencia = 50.0,
                Vencimento = 0.0,
                Desconto = 250.36
            };

            Evento evento4 = new Evento()
            {
                Codigo = 654,
                Descricao = "TESTE3 TESTE 3 TESTE 3 TESTE 3 TESTE 3",
                Referencia = 6.38,
                Vencimento = 0.0,
                Desconto = 542.65
            };

            Evento evento5 = new Evento()
            {
                Codigo = 8,
                Descricao = "TESTE 4",
                Referencia = 985.36,
                Vencimento = 225412.00,
                Desconto = 52145.25
            };

            Evento evento6 = new Evento()
            {
                Codigo = 43,
                Descricao = "TESTE 5",
                Referencia = 50.0,
                Vencimento = 0,
                Desconto = 520.00
            };


            holerite.Eventos.Add(evento1);
            holerite.Eventos.Add(evento2);
            holerite.Eventos.Add(evento3);
            holerite.Eventos.Add(evento4);
            holerite.Eventos.Add(evento5);
            holerite.Eventos.Add(evento6);

            holerite.Cbo = 1234;
            holerite.BaseCalculoFGTS = 1;
            holerite.BaseCalculoIRRF = 1;
            holerite.FaixaIRRF = "11";
            holerite.TotalVencimentos = 1000;
            holerite.TotalDescontos = 200;
            holerite.TotalLiquido = 800;
            holerite.SalarioINSS = 100;
            holerite.SalarioBase = 1000;
            holerite.Mensagem = "mensgaem teste TESTE TESTE MESNAGEM TESTE TESTE TESTE MENSGAME TESTE TESTE TESTE MENSAGEM TESTE mensgaem teste TESTE TESTE MESNAGEM TESTE TESTE TESTE MENSGAME TESTE TESTE TESTE MENSAGEM TESTE";
            holerite.Cargo = "programador";

            holerite.CompetenciaAno = 2019;
            holerite.CompetenciaMes = 03;
            holerite.Tipo = "Mensal";

            holerite.GerarId();

            return holerite;
        }

        [Admin]
        [HttpPost]
        public ActionResult EnviarArquivo(List<IFormFile> funcionarios, List<IFormFile> holerites, List<IFormFile> eventos)
        {
            long tamanho = funcionarios.Sum(f => f.Length) + holerites.Sum(f => f.Length) + eventos.Sum(f => f.Length);

            try
            {
                EnviarArquivoTxt(funcionarios, 1);
                EnviarArquivoTxt(holerites, 2);
                EnviarArquivoTxt(eventos, 3);

                HoleriteVM holerite = new HoleriteVM();
                
                TempData["EnviarArquivoOK"] = "=)  Arquivos enviados com sucesso, prontos para serem processados!";

            }
            catch (Exception exp)
            {
                TempData["EnviarArquivoErro"] = "=(  Ocorreu um erro ao tentar enviar o arquivo, por favor, tente novamente!";

            }

            return RedirectToAction("EnviarArquivo");
        }

        public void EnviarArquivoTxt(List<IFormFile> files, byte codigo)
        {

            DateTime agora = Globalization.HoraAtualBR();

            if (files.Count > 0)
            {

                foreach (var file in files)
                {

                    string path = Path.Combine(
                                    Directory.GetCurrentDirectory(), "wwwroot\\textfiles",
                                    string.Concat(codigo.ToString(), "_", file.FileName));


                    using (FileStream fs = System.IO.File.Create(path))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }

                    Arquivo arquivo = new Arquivo()
                    {
                        Nome = file.FileName,
                        Processado = 0,
                        Path = path,
                        DataProcessamento = agora,
                        DataEnvio = agora,
                        Tipo = codigo
                    };

                    _db.Int_RH_Arquivos.Add(arquivo);
                    _db.SaveChanges();
                }
            }
        }

        [Login]
        [Admin]
        public ActionResult ExcluirArquivo(long key)
        {

            Arquivo arquivo = _db.Int_RH_Arquivos.Find(key);

            if (arquivo != null)
            {

                _db.Int_RH_Arquivos.Remove(arquivo);

                _db.SaveChanges();

            }
            else
            {
                TempData["EnviarArquivoErro"] = "O Arquivo não foi encontrado!";
            }


            return RedirectToAction("EnviarArquivo");

        }

        [Login]
        [Admin]
        public ActionResult ProcessarArquivo(long key)
        {
            Arquivo arquivo = _db.Int_RH_Arquivos.Find(key);

            if (arquivo != null)
            {
                if (arquivo.Processado == 0)
                {
                    switch (arquivo.Tipo)
                    {
                        case 1: //Funcionários

                            ProcessarArquivo_Funcionarios(arquivo);

                            break;
                        case 2: //Holerites

                            ProcessarArquivo_Holerites(arquivo);

                            break;
                        case 3: //Evento

                            ProcessarArquivo_Eventos(arquivo);

                            break;
                        default:

                            TempData["EnviarArquivoErro"] = "O Arquivo selcionado está com erro, favor enviá-lo novamente!";

                            break;
                    }


                }
                else
                {
                    TempData["EnviarArquivoErro"] = "O Arquivo selcionado já foi processado!";
                }

            }
            else
            {
                TempData["EnviarArquivoErro"] = "O Arquivo não foi encontrado!";
            }


            return RedirectToAction("EnviarArquivo");

        }


        public void ProcessarArquivo_Holerites(Arquivo arquivo)
        {
            string path = Path.Combine(
                          Directory.GetCurrentDirectory(), "wwwroot/textfiles", string.Concat("1_", arquivo.Nome));

            List<Holerite> holerites = new List<Holerite>();

            int contador = 0; //Informa o número da linha

            if (System.IO.File.Exists(path))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        string linha;

                        List<Loja> lojas = _db.Int_RH_Lojas.OrderBy(o => o.CNPJ).ToList();


                        while ((linha = sr.ReadLine()) != null)
                        {
                            contador += 1;

                            int usuario = int.Parse(linha.Substring(0, 7).Trim());

                            Usuario user = _db.Int_RH_Usuarios.Find(usuario);

                            Holerite holerite = new Holerite
                            {
                                Usuario = user,
                                Tipo = linha.Substring(22, 20).Trim(),
                                Cbo = int.Parse(linha.Substring(45, 7).Trim()),
                                Cargo = linha.Substring(55, 50).Trim(),
                                CompetenciaMes = int.Parse(linha.Substring(10,2)),
                                CompetenciaAno = int.Parse(linha.Substring(15,4)),
                                TotalVencimentos = ConverterDouble(linha, 108, 10),
                                TotalDescontos = ConverterDouble(linha, 121, 10),
                                TotalLiquido = ConverterDouble(linha, 134, 10),
                                SalarioBase = ConverterDouble(linha, 147, 10),
                                SalarioINSS = ConverterDouble(linha, 160, 10),
                                BaseCalculoFGTS = ConverterDouble(linha, 173, 10),
                                FGTSdoMes = ConverterDouble(linha, 186, 10),
                                BaseCalculoIRRF = ConverterDouble(linha, 199, 10),
                                FaixaIRRF = linha.Substring(212, 20).Trim()
                            };

                            holerite.GerarId();

                            holerites.Add(holerite);

                        }
                    }

                    if (holerites.Count > 0)
                    {
                        foreach (Holerite holerite in holerites)
                        {
                            //_db.Int_RH_Usuarios.Add(holerite);
                        }

                        _db.SaveChanges();
                    }

                    RegistrarProcessamento(arquivo.Id);

                }
                catch (Exception exp)
                {
                    TempData["EnviarArquivoErro"] = "ERRO: Holerite: linha:" + contador + "Mensagem: " + exp.Message;
                    _db.Dispose();
                }

            }

        }

        public double ConverterDouble(string linha, int inicio, int tamanho)
        {
            if (linha.Substring(inicio, tamanho).Trim() != "")
            {
                return double.Parse(linha.Substring(inicio, tamanho).Trim());
            }
            else
            {
                return 0.0;
            }

        }

        public void ProcessarArquivo_Eventos(Arquivo arquivo)
        {
            string path = Path.Combine(
                          Directory.GetCurrentDirectory(), "wwwroot/textfiles", string.Concat("1_", arquivo.Nome));

            List<Evento> eventos = new List<Evento>();

            int contador = 0; //Informa o número da linha

            if (System.IO.File.Exists(path))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        string linha;

                        List<Loja> lojas = _db.Int_RH_Lojas.OrderBy(o => o.CNPJ).ToList();

                        Usuario user = new Usuario();
                        int mes, ano, id_user;
                        Holerite holerite = new Holerite();
                        string tipo;

                        mes = 0;
                        ano = 0;
                        tipo = "";

                        while ((linha = sr.ReadLine()) != null)
                        {

                            contador += 1;

                            if (linha.EndsWith("#"))
                            {
                                id_user = int.Parse(linha.Substring(0, 7).Trim());
                                mes = int.Parse(linha.Substring(10, 2).Trim());
                                ano = int.Parse(linha.Substring(15, 4).Trim());
                                tipo = linha.Substring(22, 20).Trim();
                                user = _db.Int_RH_Usuarios.Find(id_user);

                                //holerite = _db.Int_RH_Holerites.Where(a => a.Usuario.Id == id_user && a.CompetenciaMes == mes &&
                                //a.CompetenciaAno == ano && a.Tipo == tipo).FirstOrDefault();

                                



                            }
                            else
                            {
                                Evento evento = new Evento()
                                {
                                    User = user,
                                    Competencia_Mes = mes,
                                    Competencia_Ano = ano,
                                    Tipo = tipo,
                                    Codigo = int.Parse(linha.Substring(47, 5).Trim()),
                                    Descricao = linha.Substring(55, 50).Trim(),
                                    Referencia = ConverterDouble(linha, 108, 10),
                                    Vencimento = ConverterDouble(linha, 121, 10),
                                    Desconto = ConverterDouble(linha, 134, 10),
                                    Holerite = holerite
                                };
                                
                                eventos.Add(evento);
                            }

                        }
                    }

                    if (eventos.Count > 0)
                    {
                        foreach (Evento evento in eventos)
                        {
                            try
                            {
                               // _db.Int_RH_Eventos.Add(evento);
                                _db.SaveChanges();
                                eventos.Remove(evento);

                            }catch(Exception exp)
                            {
                                

                            }

                            
                        }

                        _db.SaveChanges();
                    }

                    RegistrarProcessamento(arquivo.Id);

                }
                catch (Exception exp)
                {
                    TempData["EnviarArquivoErro"] = "ERRO: Funcionario: linha:" + contador + "Mensagem: " + exp.Message;
                    _db.Dispose();
                }

            }
        }

        public void ProcessarArquivo_Funcionarios(Arquivo arquivo)
        {

            string path = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot/textfiles", string.Concat("1_", arquivo.Nome));

            List<Usuario> usuarios = new List<Usuario>();

            int contador = 0; //Informa o número da linha

            if (System.IO.File.Exists(path))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        string linha;

                        List<Loja> lojas = _db.Int_RH_Lojas.OrderBy(o => o.CNPJ).ToList();

                        Usuario func = new Usuario();

                        while ((linha = sr.ReadLine()) != null)
                        {
                            contador += 1;

                            Usuario usuario = new Usuario
                            {
                                Id = int.Parse(linha.Substring(31, 7).Trim()),
                                Loja = lojas.Where(a => a.CNPJ == linha.Substring(0, 14)).First(),
                                CPF = linha.Substring(17, 11).Trim(),
                                CodigoAtivacao = int.Parse(linha.Substring(31, 7).Trim()),
                                Nome = linha.Substring(41, 70).Trim(),
                                DataAdmissao = DateTime.Parse(linha.Substring(114, 10).Trim()),
                                Cadastrado = 0,
                                Ativo = 1,
                                Nivel = 2,
                                Verificado = 0
                            };

                            usuarios.Add(usuario);

                        }
                    }

                    if (usuarios.Count > 0)
                    {
                        foreach (Usuario usuario in usuarios)
                        {
                            Usuario user = _db.Int_RH_Usuarios.Find(usuario.Id);

                            if (user == null)
                            {
                                _db.Int_RH_Usuarios.Add(usuario);
                            }
                        }

                        _db.SaveChanges();

                    }

                    RegistrarProcessamento(arquivo.Id);

                }
                catch (Exception exp)
                {
                    TempData["EnviarArquivoErro"] = "ERRO: Funcionario: linha:" + contador + "Mensagem: " + exp.Message;
                    _db.Dispose();
                }

            }

        }

        public void RegistrarProcessamento(long id)
        {
            Arquivo file = _db.Int_RH_Arquivos.Find(id);
            file.DataProcessamento = Globalization.HoraAtualBR();
            file.Processado = 1;
            _db.SaveChanges();

            //
            ///INCLUIR CODIGO *INSERT* LOG AQUI///
            //

        }


        public void LerArquivo(string nome, byte tipo)
        {
            //Funcionarios
            /*string path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/textfiles/1FOLHAFUNC012019.txt");*/

            //Eventos
            string path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/textfiles/1FOLHAFUNC0120192.txt");

            List<Usuario> usuarios = new List<Usuario>();
            List<Evento> eventos = new List<Evento>();

            int contador = 0; //Informa o número da linha

            if (System.IO.File.Exists(path))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        string linha;

                        int codigo_func;
                        Usuario func = new Usuario();

                        while ((linha = sr.ReadLine()) != null)
                        {
                            contador += 1;

                            if (linha.StartsWith("#"))
                            {
                                codigo_func = int.Parse(linha.Substring(1, 7).Trim());
                                func = _db.Int_RH_Usuarios.Find(codigo_func);

                            }
                            else if (linha.EndsWith("@"))
                            {

                            }
                            else
                            {

                                Evento evento = new Evento()
                                {
                                    User = func,
                                    Id = int.Parse(linha.Substring(1, 7)),
                                    Descricao = linha.Substring(10, 50).Trim(),
                                    Referencia = double.Parse(linha.Substring(65, 10))
                                };


                                //Se a parte do Vencimento tiver algo
                                if (linha.Substring(76, 10).Trim() != "")
                                {
                                    evento.Vencimento = double.Parse(linha.Substring(76, 10));
                                }

                                //Se a parte do Desconto tiver algo
                                if (linha.Substring(88, 10).Trim() != "")
                                {
                                    evento.Vencimento = double.Parse(linha.Substring(88, 10));
                                }

                                eventos.Add(evento);

                            }

                            Console.WriteLine(linha);

                        }
                    }

                }
                catch (Exception exp)
                {
                    Console.WriteLine("linha: " + contador + ", " + exp.Message);
                }
            }
        }
    }
}