using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class SalonClose : BaseModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SalonId { get; set; }

        public virtual Salon Salon { get; set; }
    }


    public class SalonCloseConfiguration : EntityTypeConfiguration<SalonClose>
    {
        public SalonCloseConfiguration()
        {
            this.HasKey(s => s.Id);

            this.HasRequired(s => s.Salon)
                .WithMany(s => s.SalonCloses)
                .HasForeignKey(s => s.SalonId);
        }
    }
}

