using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ServiceFunctionalityTagSnapShot : BaseModel
    {
        public string TagName{get;set;}
        public int ServiceSnapShotId { get; set; }
        public int Seq { get; set; }

        public virtual ServiceSnapShot ServiceSnapShot { get; set; }
    }


    public class ServiceFunctionalityTagSnapShotConfiguration : EntityTypeConfiguration<ServiceFunctionalityTagSnapShot>
    {
        public ServiceFunctionalityTagSnapShotConfiguration()
        {
            this.HasKey(t => new { t.TagName ,t.ServiceSnapShotId});

            this.HasRequired(t => t.ServiceSnapShot)
               .WithMany(t => t.FunctionalityTags)
               .HasForeignKey(t => t.ServiceSnapShotId);
        }
    }
}
