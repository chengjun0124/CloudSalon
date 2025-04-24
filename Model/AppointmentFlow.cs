using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class AppointmentFlow : BaseModel
    {
        public int AppointmentFlowId { get; set; }
        public int AppointmentId { get; set; }
        public int AppointmentStatusId { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? EmployeeId { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual AppointmentStatus AppointmentStatus { get; set; }
        public virtual Employee Employee { get; set; }
    }


    public class AppointmentFlowConfiguration : EntityTypeConfiguration<AppointmentFlow>
    {
        public AppointmentFlowConfiguration()
        {
            this.HasKey(a => a.AppointmentFlowId);
            this.Property(a => a.Comments).HasMaxLength(200);

            this.HasRequired(a => a.Appointment)
               .WithMany(u => u.AppointmentFlows)
               .HasForeignKey(a => a.AppointmentId);

            this.HasRequired(a => a.AppointmentStatus)
               .WithMany(u => u.AppointmentFlows)
               .HasForeignKey(a => a.AppointmentStatusId);

            this.HasOptional(a => a.Employee)
               .WithMany(u => u.AppointmentFlows)
               .HasForeignKey(a => a.EmployeeId);
        }
    }
}
