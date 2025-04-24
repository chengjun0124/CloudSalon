using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class LoginValidCode : BaseModel
    {
        public int CodeId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public string IP { get; set; }

        public virtual User User { get; set; }

    }

    public class LoginValidCodeConfiguration : EntityTypeConfiguration<LoginValidCode>
    {
        public LoginValidCodeConfiguration()
        {
            this.HasKey(l => l.CodeId);
            this.Property(l => l.Code).IsRequired().IsFixedLength().HasMaxLength(4).IsUnicode(false);
            this.Property(l => l.CreatedDate).IsRequired();
            this.Property(l => l.UserId).IsRequired();
            this.Property(l => l.IP).IsRequired().IsUnicode(false).HasMaxLength(15);

            this.HasRequired(u => u.User)
                .WithMany(l => l.LoginValidCodes)
                .HasForeignKey(l => l.UserId);
        }
    }
}
