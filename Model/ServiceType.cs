using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ServiceType : BaseModel
    {
        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }

        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<ServiceTypeTag> ServiceTypeTags { get; set; }
    }


    public class ServiceTypeConfiguration : EntityTypeConfiguration<ServiceType>
    {
        public ServiceTypeConfiguration()
        {
            this.HasKey(s => s.ServiceTypeId);
            this.Property(s => s.ServiceTypeName).IsRequired().HasMaxLength(6);
        }
    }
}
