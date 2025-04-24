using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ConsumedService : BaseModel
    {
        public ConsumedService()
        {
            this.ConsumedServiceDetails = new List<ConsumedServiceDetail>();
        }

        public int ConsumedServiceId { get; set; }
        public int? UserId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? AppointmentId { get; set; }
        public int? OperatorId { get; set; } //美容师扫码，此字段为null；美管扫码或手动记账或匿名买单，此字段为操作人ID
        public byte Mode { get; set; }//消费方式：0=美容师扫码,1美管或老板扫码,2美管或老板手动扣除,3匿名买单
        public int ConsumedServiceStatusId { get; set; }//美容师扫码或匿名买单，此字段为3；美管扫码或手动记账，此字段为ConsumedServiceStatusEnum之一
        public DateTime? UserConfirmedDate { get; set; }//美容师扫码，此字段为null；美管扫码或手动记账后，当用户在消费单里点确认后，此字段保存当时的时间，ConsumedServiceStatusId设置成ConsumedServiceStatusEnum.Confirmed
        public string ChangeTimeReason { get; set; }//操作人修改了消费次数，必须输入修改理由


        public virtual Employee Employee { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual ICollection<ConsumedServiceDetail> ConsumedServiceDetails { get; set; }
        public virtual User User { get; set; }
    }

    public class ConsumedServiceConfiguration : EntityTypeConfiguration<ConsumedService>
    {
        public ConsumedServiceConfiguration()
        {
            this.HasKey(s => s.ConsumedServiceId);

            

            this.HasRequired(s => s.Employee)
                .WithMany(s => s.ConsumedServices)
                .HasForeignKey(s => s.EmployeeId);

            this.HasOptional(s => s.Appointment)
                .WithMany(s => s.ConsumedServices)
                .HasForeignKey(s => s.AppointmentId);

            this.HasOptional(s => s.User)
                .WithMany(s => s.ConsumedServices)
                .HasForeignKey(s => s.UserId);
        }
    }
}
