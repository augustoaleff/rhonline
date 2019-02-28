using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RHOnline.Models;
using RHOnline.Models.Logs;

namespace RHOnline.Database
{
    public class DatabaseContext : DbContext
    {
        
        public DbSet<Usuario> Int_RH_Usuarios { get; set;}
        public DbSet<Log> Int_RH_Logs { get; set; }
        public DbSet<Loja> Int_RH_Lojas { get; set; }
        public DbSet<Arquivo> Int_RH_Arquivos { get; set; }
        //public DbSet<Holerite> Int_RH_Holerites { get; set; }
        //public DbSet<Evento> Int_RH_Eventos { get; set; }


        //public DbSet<Loja> Int_DP_Lojas { get; set; }
        //public DbSet<Setor> Int_DP_Setores { get; set; }
        public DbSet<ValidacaoSenha> Int_DP_ValidSenhas { get; set; }
        public DbSet<LogTipos> Int_DP_Logs_Tipos { get; set; }
        public DbSet<Imagem> Int_DP_Banner { get; set; }
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
