using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class QuoteEngineData : DbContext
    {
        public QuoteEngineData()
            : base("name=QuoteEngine")
        {
            Database.SetInitializer<QuoteEngineData>(null);
        }

        public DbSet<country> country { get; set; }
        public DbSet<ProductOrderFields> ProductOrderFields { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductOrderFieldOptions> ProductOrderFieldOptions { get; set; }
        public DbSet<QuoteRequest> QuoteRequest { get; set; }
        public DbSet<Certificate> Certificate { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}