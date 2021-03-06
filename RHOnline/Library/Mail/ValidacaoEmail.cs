﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace RHOnline.Library.Mail
{
    public class ValidacaoEmail
    {

        public static void EnviarEmailValidacao(string email, string codigo, string nome)
        {
            string saudacao, conteudo, rodape;
            DateTime agora = Globalization.Globalization.HoraAtualBR();

            nome = Shared.PegarPrimeiroNome(nome);

            if (agora.Hour >= 3 && agora.Hour <= 11) //Entre 3h e meio-dia ==> Bom Dia
            {
                saudacao = "Bom Dia " + nome + ",<br />";
            }
            else if (agora.Hour >= 12 && agora.Hour <= 17) //Entre meio-dia e 18h ==> Boa Tarde
            {
                saudacao = "Boa Tarde " + nome + ",<br />";
            }
            else //Entre 18h e 3h ==> Boa Noite
            {
                saudacao = "Boa Noite " + nome + ",<br />";
            }
            
            conteudo = "<br />  <a href='http://www.eletroleste.com.br/RHOnline/Home/ValidarEmail?key=" + codigo + "'>Clique Aqui </a> para confirmar o seu e-mail <br />" +
                " <br /><br />Favor desconsiderar caso tenha recebido por erro";

            rodape = "<br /><br /><font size='1'>Mensagem Automática, favor não responder. Enviada: " + Globalization.Globalization.DataAtualExtensoBR() + "</font>";

            SmtpClient smtp = new SmtpClient(Constants.ServidorSMTP, Constants.PortaSMTP)
            {
                EnableSsl = true,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(Constants.Usuario, Constants.Senha)
            };

            MailMessage mensagem = new MailMessage
            {
                From = new MailAddress("no-reply@eletroleste.com.br", "RHOnline"),
                Subject = "Confirmação de Email",
                IsBodyHtml = true,
                Body = saudacao +
                conteudo +
                rodape
            };
            mensagem.Bcc.Add(email);
            smtp.Send(mensagem);

        }

    }
}
