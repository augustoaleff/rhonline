using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RHOnline.Models
{
    public class Evento
    {
        [Key]
        public long Id { get; set; }
        
        public Usuario User { get; set; }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public double Referencia { get; set; }
        public double Vencimento { get; set; }
        public double Desconto { get; set; }

        //Referencia o Holerites
        public Holerite Holerite { get; set; }

        public int Competencia_Mes { get; set; }
        public int Competencia_Ano { get; set; }
        public string Tipo { get; set; }

        
    }
}
