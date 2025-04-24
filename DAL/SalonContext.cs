using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CloudSalon.Model;
using System.Configuration;

namespace CloudSalon.DAL
{
    public class SalonContext : BaseContext
    {
        public SalonContext() : base(ConfigurationManager.AppSettings["DBConnection"]) { }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new SalonConfiguration());
            modelBuilder.Configurations.Add(new UserTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeConfiguration());
            modelBuilder.Configurations.Add(new ServiceTypeConfiguration());
            modelBuilder.Configurations.Add(new ServiceConfiguration());
            modelBuilder.Configurations.Add(new ServiceEffectImageSnapShotConfiguration());            
            modelBuilder.Configurations.Add(new ServiceSnapShotConfiguration());
            modelBuilder.Configurations.Add(new ServiceEffectImageConfiguration());
            modelBuilder.Configurations.Add(new LoginValidCodeConfiguration());
            modelBuilder.Configurations.Add(new SalonCloseConfiguration());
            modelBuilder.Configurations.Add(new UnavaiTimeConfiguration());
            modelBuilder.Configurations.Add(new AppointmentConfiguration());
            modelBuilder.Configurations.Add(new AppointmentFlowConfiguration());
            modelBuilder.Configurations.Add(new AppointmentStatusConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new PurchasedServiceConfiguration());
            modelBuilder.Configurations.Add(new ConsumedServiceConfiguration());
            modelBuilder.Configurations.Add(new ConsumedServiceDetailConfiguration());
            modelBuilder.Configurations.Add(new ServiceTypeTagConfiguration());
            modelBuilder.Configurations.Add(new ServiceFunctionalityTagConfiguration());
            modelBuilder.Configurations.Add(new ServiceFunctionalityTagSnapShotConfiguration());
        }
    }
}
