using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace OrderForm2016.Models
{
    public class OrderForm2016Data : DbContext
    {
        public OrderForm2016Data()
            : base("name=OrderForm2016")
        {
            Database.SetInitializer<OrderForm2016Data>(null);
        }

        public DbSet<BaseForm> BaseForm { get; set; }
        public DbSet<ccPartial> ccPartial { get; set; }
        public DbSet<ChildAges> ChildAges { get; set; }
        public DbSet<TravelerAges> TravelerAges { get; set; }
        public DbSet<MissionaryOptions> MissionaryOptions { get; set; }
        public DbSet<Options360> Options360 { get; set; }
        public DbSet<QuoteResults> QuoteResults { get; set; }
        public DbSet<RepatOptions> RepatOptions { get; set; }
        public DbSet<TravelOptions> TravelOptions { get; set; }
        public DbSet<TripCanOptions> TripCanOptions { get; set; }
        public DbSet<NationwideOptions> NationwideOptions { get; set; }
        public DbSet<StudyAbroadPartial> StudyAbroadPartial { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<Error> Error { get; set; }
        public DbSet<xlHead> xlHead { get; set; }
        public DbSet<xlTravelers> xlTravelers { get; set; }
        //public DbSet<VisitorMatrix> VisitorMatrices { get; set; }
        //public DbSet<RenewMemberLogin> RenewMemberLogins { get; set; }
        //public DbSet<RenewEnrollment> RenewEnrollments { get; set; }
        public DbSet<QuoteForm> QuoteForm { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}