using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class UnavaiAppointment : BaseModel
    {
        public int UnavaiId { get; set; }
        public DateTime UnavaiDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
    }

    public class UnavaiAppointmentConfiguration : EntityTypeConfiguration<UnavaiAppointment>
    {
        public UnavaiAppointmentConfiguration()
        {
            this.HasKey(u => u.UnavaiId);


            this.HasRequired(u => u.Employee)
                .WithMany(e => e.UnavaiAppointments)
                .HasForeignKey(u => u.EmployeeId);
        }
    }
}
