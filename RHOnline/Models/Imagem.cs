﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RHOnline.Models
{
    public class Imagem
    {
        [Key]
        public int Ordem { get; set; }
        public string Path { get; set; } //Nome da Imagem no caminho RHOnline/images/
        public string Nome { get; set; } //Nome Alternativo para a Imagem

    }
}
