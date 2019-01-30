using RHOnline.Library.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RHOnline.Models.Logs
{
    public class Log
    {
        public long Id { get; set; }
        public int Usuario { get; set; }
        public DateTime Data { get; set; }
        public int Tipo { get; set; }
        //public long Ocorrencia { get; set; }
        //public int UsuarioAlterado { get; set; }
        //public long Mensagem { get; set; }
        public string Obs { get; set; }
        //public long Funcionario { get; set; }
        public string Sistema_Operacional { get; set; }
        
        public Log()
        {
            this.Sistema_Operacional = Environment.OSVersion.VersionString;
            this.Data = Globalization.HoraAtualBR();
        }

        public void LogIn(int usuario)
        {
            this.Usuario = usuario;
            this.Tipo = 2; //Login efetuado com Sucesso 
        }

        public void LogIn_Erro(int usuario, Exception e)
        {
            this.Usuario = usuario;
            this.Obs = "ERRO: " + e.Message;
            this.Tipo = 1; //Login efetuado com Erro 
        }

        public void LogOut(int usuario)
        {
            this.Usuario = usuario;
            this.Tipo = 4; //Logout efetuado com Sucesso
        }

        public void LogOut_Erro(int usuario, Exception e)
        {
            this.Usuario = usuario;
            this.Tipo = 3; //Logout efetuado com Sucesso
            this.Obs = "ERRO: " + e.Message;
        }

    }
}
