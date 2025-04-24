using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class Appointment : BaseModel
    {
        public Appointment()
        {
            this.AppointmentFlows = new List<AppointmentFlow>();
        }

        public int AppointmentId { get; set; }
        public int ServiceSnapShotId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int UserId { get; set; }        

        
        public virtual ServiceSnapShot ServiceSnapShot { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<AppointmentFlow> AppointmentFlows { get; set; }
        public virtual ICollection<ConsumedService> ConsumedServices { get; set; }
        
    }


    public class AppointmentConfiguration : EntityTypeConfiguration<Appointment>
    {
        public AppointmentConfiguration()
        {
            this.HasKey(a => a.AppointmentId);

            this.Property(a => a.EmployeeId).IsRequired();
            this.Property(a => a.AppointmentDate).IsRequired();
            this.Property(a => a.UserId).IsRequired();


            this.HasRequired(a => a.Employee)
               .WithMany(e => e.Appointments)
               .HasForeignKey(a => a.EmployeeId);

            this.HasRequired(a => a.User)
               .WithMany(u => u.Appointments)
               .HasForeignKey(a => a.UserId);

            this.HasRequired(a => a.ServiceSnapShot)
               .WithMany(u => u.Appointments)
               .HasForeignKey(a => a.ServiceSnapShotId);
        }
    }
}
