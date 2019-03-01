using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RHOnline.Library.Globalization;

namespace RHOnline.Models
{
    public class Holerite
    {
        public long Id { get; set; }
        public Usuario Usuario { get; set; }
        public int Cbo { get; set; }
        public string Cargo { get; set; }
        public string Tipo { get; set; }
        public int CompetenciaMes { get; set; }
        public int CompetenciaAno { get; set; }
        public double TotalVencimentos { get; set; }
        public double TotalDescontos { get; set; }
        public double TotalLiquido { get; set; }
        public double SalarioBase { get; set; }
        public double SalarioINSS { get; set; }
        public double BaseCalculoFGTS { get; set; }
        public double FGTSdoMes { get; set; }
        public double BaseCalculoIRRF { get; set; }
        public string FaixaIRRF { get; set; }
        public string Mensagem { get; set; }
        public string Autenticacao { get; set; }

        public void GerarId()
        {
            DateTime agora = Globalization.HoraAtualBR();

            int data = agora.Day + agora.Month + agora.Year + agora.Hour + agora.Minute + agora.Second + agora.Millisecond;

            string id = Usuario.Id + CompetenciaMes.ToString("00") + CompetenciaAno.ToString("0000") + data.ToString();
            
            int digitoVerificador = 0;

            for (int i = 0; i < id.Length; i++)
            {
                digitoVerificador += int.Parse(id.Substring(i, 1));

            }

            id += digitoVerificador.ToString().Substring(0,1);

            this.Id = long.Parse(id);

            //Converte o Id para Hexadecimal e aloca na Autenticação
            this.Autenticacao = this.Id.ToString("X");

            

        }


    }
}
