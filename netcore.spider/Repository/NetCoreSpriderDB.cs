using Microsoft.EntityFrameworkCore;
using NetCore.Spider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Spider.Repository
{
    public class NetCoreSpriderDBContext : DbContext
    {
        //public NetCoreSpriderDBContext(DbContextOptions<NetCoreSpriderDBContext> options)
        //: base(options)
        //{
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=NetCoreSprider;User ID=sa;Password=password1", b => b.UseRowNumberForPaging());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Link> Link { get; set; }

        public virtual DbSet<PageContext> PageContext { get; set; }

        public virtual DbSet<CrawHistory> CrawHistory { get; set; }
    }
}
