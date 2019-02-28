using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RHOnline.Models.ViewModels
{
    public class HoleriteVM : Holerite
    {

        public List<Evento> Eventos { get; set; }

        public HoleriteVM()
        {
            this.Eventos = new List<Evento>();
            
        }
        
    }
}
