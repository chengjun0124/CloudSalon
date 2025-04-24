using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ConsumedServiceDetail : BaseModel
    {
        public int ConsumedServiceId { get; set; }
        public int PurchasedServiceId { get; set; }
        public byte Time { get; set; }

        public virtual ConsumedService ConsumedService { get; set; }
        public virtual PurchasedService PurchasedService { get; set; }

    }

    public class ConsumedServiceDetailConfiguration : EntityTypeConfiguration<ConsumedServiceDetail>
    {
        public ConsumedServiceDetailConfiguration()
        {
            this.HasKey(csd => new { csd.ConsumedServiceId, csd.PurchasedServiceId });


            this.HasRequired(s => s.ConsumedService)
                .WithMany(s => s.ConsumedServiceDetails)
                .HasForeignKey(s => s.ConsumedServiceId);

            this.HasRequired(s => s.PurchasedService)
                .WithMany(s => s.ConsumedServiceDetails)
                .HasForeignKey(s => s.PurchasedServiceId);
        }
    }
}
