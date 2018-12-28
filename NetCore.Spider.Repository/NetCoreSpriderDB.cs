using Microsoft.EntityFrameworkCore;
using NetCore.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Spider.Repository
{
    public class NetCoreSpriderDB : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=NetCoreSprider1;User ID=sa;Password=password1", b => b.UseRowNumberForPaging());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<SourceClassify> SourceClassify { get; set; }

        public virtual DbSet<CrawHistory> CrawHistory { get; set; }

        public virtual DbSet<Link> Link { get; set; }

        public virtual DbSet<PageContext> PageContext { get; set; }
    }
}
