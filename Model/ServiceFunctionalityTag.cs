using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ServiceFunctionalityTag : BaseModel
    {
       public string TagName{get;set;}
       public int ServiceId { get; set; }
       public int Seq { get; set; }

       public virtual Service Service { get; set; }
        
    }


    public class ServiceFunctionalityTagConfiguration : EntityTypeConfiguration<ServiceFunctionalityTag>
    {
        public ServiceFunctionalityTagConfiguration()
        {
            this.HasKey(t => new { t.TagName, t.ServiceId });

            this.HasRequired(t => t.Service)
               .WithMany(t => t.FunctionalityTags)
               .HasForeignKey(t => t.ServiceId);
        }
    }
}
