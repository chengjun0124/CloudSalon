using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ServiceEffectImage : BaseModel
    {
        public int ServiceId { get; set; }
        public string FileName { get; set; }        
        public int Seq { get; set; }

        public virtual Service Service { get; set; }
    }

    public class ServiceEffectImageConfiguration : EntityTypeConfiguration<ServiceEffectImage>
    {
        public ServiceEffectImageConfiguration()
        {
            this.Property(si => si.ServiceId).IsRequired();
            this.HasKey(si => new { si.ServiceId, si.Seq });
            this.Property(si => si.FileName).HasMaxLength(40).IsFixedLength().IsUnicode(false);
            this.Property(si => si.Seq).IsRequired();


            this.HasRequired(si => si.Service)
                .WithMany(s => s.EffectImages)
                .HasForeignKey(si => si.ServiceId);
        }
    }
}
