using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using OrderForm2016.Models;

namespace OrderForm2016.Models
{
    public class SIAdminData : DbContext
    {
        public SIAdminData()
            : base("name=siAdmin")
        {
            Database.SetInitializer<SIAdminData>(null);
        }

        public DbSet<policy_plan> policy_plan { get; set; }
        public DbSet<USStateList> USStateList { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Enrollment_Payment> Enrollment_Payment { get; set; }
        public DbSet<Enrollment_Premium> Enrollment_Premium { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}