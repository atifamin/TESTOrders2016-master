using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace OrderForm2016.Models
{
    public class GBGEDIData:DbContext

    {
        public GBGEDIData()
            : base("name=GBGEDI") { }
        public DbSet<GBGRequest> GBGRequest { get; set; }
        public DbSet<GBGResponse> GBGResponse { get; set; }
        public DbSet<GBGRequestComplete> GBGRequestComplete { get; set; }
        public DbSet<GBGResponseComplete> GBGResponseComplete { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}