using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class Employee : BaseModel
    {
        public int EmployeeId { get; set; }
        //public string UserName { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }
        public int UserTypeId { get; set; }
        public string Picture { get; set; }
        public int SalonId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDayoffMon { get; set; }
        public bool IsDayoffTue { get; set; }
        public bool IsDayoffWeb { get; set; }
        public bool IsDayoffThu { get; set; }
        public bool IsDayoffFri { get; set; }
        public bool IsDayoffSat { get; set; }
        public bool IsDayoffSun { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsBeautician { get; set; }


        public virtual UserType UserType { get; set; }
        public virtual Salon Salon { get; set; }
        public virtual ICollection<UnavaiTime> UnavaiTimes { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<AppointmentFlow> AppointmentFlows { get; set; }
        public virtual ICollection<ConsumedService> ConsumedServices { get; set; }
        
        
    }

    public class EmployeeConfiguration : EntityTypeConfiguration<Employee>
    {
        public EmployeeConfiguration()
        {
            this.HasKey(e => e.EmployeeId);
            //this.Property(e => e.UserName).IsRequired().HasMaxLength(16).IsUnicode(false);
            this.Property(e => e.Password).IsRequired().IsUnicode(false).IsFixedLength().HasMaxLength(32);
            this.Property(e => e.NickName).IsRequired().HasMaxLength(15);

            this.Property(e => e.UserTypeId).IsRequired();
            this.Property(e => e.Picture).HasMaxLength(40).IsFixedLength().IsUnicode(false);
            this.Property(e => e.SalonId).IsRequired();
            this.Property(e => e.CreatedDate).IsRequired();
            this.Property(e => e.IsDeleted).IsRequired();
            this.Property(e => e.Mobile).IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(11);

            this.HasRequired(e => e.UserType)
                .WithMany(et => et.Employees)
                .HasForeignKey(e => e.UserTypeId);

            this.HasRequired(e => e.Salon)
                .WithMany(s => s.Employees)
                .HasForeignKey(e => e.SalonId);
        }
    }
}
