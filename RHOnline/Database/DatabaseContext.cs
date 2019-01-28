using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RHOnline.Models;

namespace RHOnline.Database
{
    public class DatabaseContext : DbContext
    {

        public DbSet<Usuario> Int_RH_Usuarios { get; set;}
        public DbSet<Loja> Int_DP_Lojas { get; set; }
        public DbSet<Setor> Int_DP_Setores { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
