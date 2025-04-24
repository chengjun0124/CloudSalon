using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class Salon : BaseModel
    {
        public int SalonId { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public string IdentityCode { get; set; }
        public string SalonName { get; set; }
        public string SalonAddress { get; set; }
        public string Phone { get; set; }
        public string Contact { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public string QRCodePicture { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Service> Services { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<SalonClose> SalonCloses { get; set; }        
    }

    public class SalonConfiguration : EntityTypeConfiguration<Salon>
    {
        public SalonConfiguration()
        {
            this.HasKey(s => s.SalonId);

            this.Property(s => s.OpenTime).IsRequired();
            this.Property(s => s.CloseTime).IsRequired();
            this.Property(s => s.IdentityCode).IsRequired().IsUnicode(false).IsFixedLength();
            this.Property(s => s.SalonName).IsRequired().IsUnicode(true).HasMaxLength(10);
            this.Property(s => s.SalonAddress).IsRequired().IsUnicode(true).HasMaxLength(50);
            this.Property(s => s.Phone).IsRequired().IsUnicode(false).HasMaxLength(20);
        }
    }
}
