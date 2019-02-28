using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RHOnline.Models
{
    public class Arquivo
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Path { get; set; }
        public byte Processado { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime DataProcessamento { get; set; }
        public byte Tipo { get; set; }

    }
}
