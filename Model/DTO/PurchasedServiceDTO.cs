using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CloudSalon.Model.Enum;


namespace CloudSalon.Model.DTO
{
    public class PurchasedServiceDTO : BaseDTO
    {
        public int PurchasedServiceId { get; set; }
        public int ServiceId { get; set; }
        public int UserId { get; set; }
        public byte? Time { get; set; }
        public decimal Payment { get; set; }
        public PurchaseModeEnum Mode { get; set; }
    }

    public class PurchasedServiceDTO_D : BaseDTO
    {
        public int PurchasedServiceId { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public byte? Interval { get; set; }
        public decimal Payment { get; set; }
        public byte? Time { get; set; }
        public string SmallSubjectImage { get; set; }
        public PurchaseModeEnum Mode { get; set; }

        public List<ConsumedServiceDetailDTO_D> ConsumedServices { get; set; }
    }
}
