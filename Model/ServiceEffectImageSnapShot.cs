using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ServiceEffectImageSnapShot : BaseModel
    {
        public int ServiceSnapShotId { get; set; }
        public string FileName { get; set; }        
        public int Seq { get; set; }

        public virtual ServiceSnapShot ServiceSnapShot { get; set; }
    }

    public class ServiceEffectImageSnapShotConfiguration : EntityTypeConfiguration<ServiceEffectImageSnapShot>
    {
        public ServiceEffectImageSnapShotConfiguration()
        {
            this.Property(si => si.ServiceSnapShotId).IsRequired();
            this.HasKey(si => new { si.ServiceSnapShotId, si.Seq });
            this.Property(si => si.FileName).HasMaxLength(40).IsFixedLength().IsUnicode(false);
            this.Property(si => si.Seq).IsRequired();


            this.HasRequired(si => si.ServiceSnapShot)
                .WithMany(s => s.EffectImages)
                .HasForeignKey(si => si.ServiceSnapShotId);
        }
    }
}
