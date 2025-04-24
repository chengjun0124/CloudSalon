using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class UserType : BaseModel
    {
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }

    public class UserTypeConfiguration : EntityTypeConfiguration<UserType>
    {
        public UserTypeConfiguration()
        {
            this.HasKey(s => s.UserTypeId);
            this.Property(s => s.UserTypeName).HasMaxLength(10).IsRequired();
        }
    }
}
