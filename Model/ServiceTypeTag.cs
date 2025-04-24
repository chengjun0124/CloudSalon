using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ServiceTypeTag : BaseModel
    {
        public string TagName { get; set; }
        public int ServiceTypeId { get; set; }

        public virtual ServiceType ServiceType { get; set; }
    }


    public class ServiceTypeTagConfiguration : EntityTypeConfiguration<ServiceTypeTag>
    {
        public ServiceTypeTagConfiguration()
        {
            this.HasKey(t => new { t.TagName, t.ServiceTypeId });



            this.HasRequired(t => t.ServiceType)
               .WithMany(t => t.ServiceTypeTags)
               .HasForeignKey(t => t.ServiceTypeId);
        }
    }
}
