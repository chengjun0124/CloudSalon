using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model
{
    public class ServiceSnapShot : BaseModel
    {
        public ServiceSnapShot()
        {
            this.EffectImages = new List<ServiceEffectImageSnapShot>();
            this.FunctionalityTags = new List<ServiceFunctionalityTagSnapShot>();
        }

        public int ServiceSnapShotId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int ServiceTypeId { get; set; }
        public byte Pain { get; set; }
        //public TimeSpan Duration { get; set; }//服务时长，单位是分钟
        public int Duration { get; set; }//服务时长，单位是分钟。原本改字段类型在数据库是time(7), 在c#中是TimeSpan，后来发现TimeSpan发送到客户端后使用不方便，每次在客户端要做从TimeSpan到int的转换，比如在美管后台的编辑服务页面，保存时要转换int到TimeSpan，如果API验证表单失败，页面还需要把TimeSpan转换成int，不然用户就会在页面上看到xx:xx:xx，然而这个字段在页面上的效果应是一个数字字段，单位是分钟
        public decimal? OncePrice { get; set; }
        public decimal? TreatmentPrice { get; set; }
        public byte? TreatmentTime { get; set; }//疗程次数，单位是次
        public byte? TreatmentInterval { get; set; }//间隔时间，单位是天
        //public string Functionality { get; set; }
        public string SuitablePeople { get; set; }//适用对象
        public string Detail { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SubjectImage { get; set; }
        public int EditBy { get; set; }
        public decimal? OncePriceOnSale { get; set; }
        public decimal? TreatmentPriceOnSale { get; set; }
        public byte? TreatmentTimeOnSale { get; set; }

        public virtual Service Service { get; set; }
        public virtual ICollection<ServiceEffectImageSnapShot> EffectImages { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<PurchasedService> PurchasedServices { get; set; }
        public virtual ICollection<ServiceFunctionalityTagSnapShot> FunctionalityTags { get; set; }
    }


    public class ServiceSnapShotConfiguration : EntityTypeConfiguration<ServiceSnapShot>
    {
        public ServiceSnapShotConfiguration()
        {
            this.HasKey(s => s.ServiceSnapShotId);
            this.Property(s => s.ServiceName).IsRequired().HasMaxLength(16);
            this.Property(s => s.ServiceTypeId).IsRequired();
            this.Property(s => s.Pain).IsRequired();
            this.Property(s => s.Duration).IsRequired();
            this.Property(s => s.OncePrice).HasColumnType("money").IsOptional();
            this.Property(s => s.TreatmentPrice).HasColumnType("money").IsOptional();
            this.Property(s => s.TreatmentTime).IsOptional();
            this.Property(s => s.TreatmentInterval).IsOptional();
            //this.Property(s => s.Functionality).HasMaxLength(200);
            this.Property(s => s.SuitablePeople).HasMaxLength(200);
            this.Property(s => s.SubjectImage).HasMaxLength(40).IsFixedLength().IsUnicode(false);
            this.Property(s => s.CreatedDate).IsRequired();


            this.HasRequired(s => s.Service)
                .WithMany(s => s.ServiceSnapShots)
                .HasForeignKey(s => s.ServiceId);
        }
    }
}
