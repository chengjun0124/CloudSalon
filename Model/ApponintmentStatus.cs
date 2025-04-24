using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class AppointmentStatus : BaseModel
    {
        public int AppointmentStatusId { get; set; }
        public string Status { get; set; }

        public virtual ICollection<AppointmentFlow> AppointmentFlows { get; set; }

    }


    public class AppointmentStatusConfiguration : EntityTypeConfiguration<AppointmentStatus>
    {
        public AppointmentStatusConfiguration()
        {
            this.HasKey(a => a.AppointmentStatusId);
            this.Property(a => a.Status).HasMaxLength(50).IsUnicode(false);
        }
    }
}
