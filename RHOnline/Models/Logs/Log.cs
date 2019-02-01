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

        public void EsqueciMinhaSenha_Envio(int usuario, string codigo)
        {
            this.Usuario = usuario;
            this.Tipo = 6; //Enviado link de redefinição de senha com sucesso
            this.Obs = "Código de Validação: " + codigo;
        }

        public void EsqueciMinhaSenha_Envio_Erro(int usuario, string codigo, Exception exp)
        {
            this.Usuario = usuario;
            this.Tipo = 5; //Enviado link de redefinição de senha com erro
            this.Obs = "ERRO: " + exp.Message + " / Código de Validação: " + codigo;
        }

        public void EsqueciMinhaSenha_Troca(int usuario)
        {
            this.Usuario = usuario;
            this.Tipo = 8; //Redefinição de senha por link com sucesso
        }

        public void EsqueciMinhaSenha_Troca_Erro(int usuario, Exception exp)
        {
            this.Usuario = usuario;
            this.Tipo = 7; //Redefinição de senha por link com erro
            this.Obs = "ERRO: " + exp.Message;
        }

        public void AlterarMinhaSenha(int usuario)
        {
            this.Usuario = usuario;
            this.Tipo = 10; //Alteração de senha pelo usuário com sucesso
        }

        public void AlterarMinhaSenha_Erro(int usuario, Exception exp)
        {
            this.Usuario = usuario;
            this.Tipo = 9; //Alteração de senha pelo usuário com erro
            this.Obs = "ERRO: " + exp.Message;
        }

    }
}
