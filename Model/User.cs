using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class User : BaseModel
    {
        public User()
        {
            this.LoginValidCodes = new List<LoginValidCode>();
        }


        public int UserId { get; set; }
        public int SalonId { get; set; }
        public string Mobile { get; set; }
        public string NickName { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ConsumeCode { get; set; }
        public DateTime? ConsumeCodeExpiredDate { get; set; }
        public string Memo { get; set; }
        public bool IsActive { get; set; }

        public virtual Salon Salon { get; set; }
        public virtual ICollection<LoginValidCode> LoginValidCodes { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<PurchasedService> PurchasedServices { get; set; }
        public virtual ICollection<ConsumedService> ConsumedServices { get; set; }             
    }

    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            this.HasKey(u => u.UserId);
            this.Property(u => u.SalonId).IsRequired();
            this.Property(u => u.Mobile).IsFixedLength().HasMaxLength(11).IsRequired().IsUnicode(false);
            this.Property(u => u.NickName).IsOptional().HasMaxLength(15);
            this.Property(u => u.Picture).HasMaxLength(40).IsFixedLength().IsUnicode(false).IsOptional();
            this.Property(u => u.Name).HasMaxLength(6).IsUnicode(true).IsOptional();
            this.Property(u => u.CreatedDate).IsRequired();

            this.HasRequired(u => u.Salon)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SalonId);

        }
    }
}
