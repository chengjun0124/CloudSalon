using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.DTO
{
    public class ServiceDTO : BaseDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public ServiceTypeEnum ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
        public byte Pain { get; set; }
        public int Duration { get; set; }
        public decimal? OncePrice { get; set; }
        public decimal? TreatmentPrice { get; set; }
        public byte? TreatmentTime { get; set; }
        public byte? TreatmentInterval { get; set; }
        
        public string SuitablePeople { get; set; }
        public string Detail { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string SubjectImage { get; set; }
        public string SmallSubjectImage { get; set; }
        public string QrCodePicture { get; set; }
        public int? Seq { get; set; }
        public decimal? OncePriceOnSale { get; set; }
        public decimal? TreatmentPriceOnSale { get; set; }
        public byte? TreatmentTimeOnSale { get; set; }

        public List<ServiceEffectImageDTO> EffectImages { get; set; }
        public List<string> FunctionalityTags { get; set; }
    }

    public class ServiceEffectImageDTO : BaseDTO
    {
        public string Image{ get; set; }
        public string SmallImage { get; set; }
    }

    public class AvaiServiceDTO_D : BaseDTO
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public byte? Interval { get; set; }
        public int? Time { get; set; }
        public string SmallSubjectImage { get; set; }

        public List<ConsumedServiceDetailDTO_D> ConsumedServices { get; set; }
    }
}
