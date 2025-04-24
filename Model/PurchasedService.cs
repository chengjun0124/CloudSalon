using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class PurchasedService : BaseModel
    {
        public PurchasedService()
        {
            this.ConsumedServiceDetails = new List<ConsumedServiceDetail>();
        }


        public int PurchasedServiceId { get; set; }
        public int? UserId { get; set; }
        public int ServiceSnapShotId { get; set; }
        public byte? Time { get; set; }
        public decimal Payment { get; set; }
        public byte Mode { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OperatorId { get; set; }

        public virtual ServiceSnapShot ServiceSnapShot { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ConsumedServiceDetail> ConsumedServiceDetails { get; set; }
    }

    public class PurchasedServiceConfiguration : EntityTypeConfiguration<PurchasedService>
    {
        public PurchasedServiceConfiguration()
        {
            this.HasKey(s => s.PurchasedServiceId);

            this.HasRequired(s => s.ServiceSnapShot)
                .WithMany(s => s.PurchasedServices)
                .HasForeignKey(s => s.ServiceSnapShotId);

            this.HasOptional(s => s.User)
                .WithMany(s => s.PurchasedServices)
                .HasForeignKey(s => s.UserId);
        }
    }
}
