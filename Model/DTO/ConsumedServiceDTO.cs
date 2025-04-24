using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CloudSalon.Model.Enum;


namespace CloudSalon.Model.DTO
{
    //尝试合并ConsumedServiceDTO_D和ConsumedServiceDetailDTO_D
    public class ConsumedServiceDetailDTO_D : BaseDTO
    {
        public DateTime CreatedDate { get; set; }
        public string EmployeeNickName { get; set; }
        public int Duration { get; set; }
        public bool IsAppoint { get; set; }
        public ConsumeModeEnum Mode { get; set; }
        public ConsumedServiceStatusEnum ConsumedServiceStatusId { get; set; }
        public byte Time{ get; set; }
    }

    public class CheckBeauticianScanDTO_P : BaseDTO
    {
        public string ConsumeCode { get; set; }
        public int AppointmentId { get; set; }
    }

    public class ChangeConsumedService_P : BaseDTO
    {
        public int ConsumedServiceId { get; set; }
        public int ServiceId { get; set; }
        public byte Time { get; set; }
        public int? AppointmentId { get; set; }
        public int EmployeeId { get; set; }
        public string ChangeTimeReason { get; set; }
    }
    

    public class CheckAOScanDTO_P : BaseDTO
    {
        public string ConsumeCode { get; set; }
        public int ServiceId { get; set; }
        public int EmployeeId { get; set; }
        public byte Time { get; set; }
        public int? AppointmentId { get; set; }
        public string ChangeTimeReason { get; set; }
    }

    public class CheckAOManualDTO_P : BaseDTO
    {
        public int PurchasedServiceId { get; set; }
        public int EmployeeId { get; set; }
        public byte Time { get; set; }
        public int? AppointmentId { get; set; }
        public string ChangeTimeReason { get; set; }
    }

    public class ConsumedServiceDTO_D : BaseDTO
    {
        public int ConsumedServiceId { get; set; }
        public string EmployeeNickName { get; set; }
        public string EmployeePicture { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ConsumedDate { get; set; }
        public byte Time { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public AppointmentStatusEnum? AppointmentStatusId { get; set; }
        public int? AppointmentId { get; set; }        
        public string ChangeTimeReason { get; set; }
        public string UserPicture { get; set; }
        public string UserName { get; set; }
        public bool IsAnonym { get; set; }
        public ConsumedServiceStatusEnum ConsumedServiceStatusId { get; set; }
        public ConsumeModeEnum Mode { get; set; }
        public int OperatorId { get; set; }
        public int? UserId { get; set; }
    }

    public class CheckAnonymDTO_P : BaseDTO
    {
        public int ServiceId { get; set; }
        public int EmployeeId { get; set; }
        public decimal Payment { get; set; }
        public byte Time { get; set; }
        public string ChangeTimeReason { get; set; }
    }

    public class ChangeConsumedServiceStatus_P : BaseDTO
    {
        public int ConsumedServiceId { get; set; }
        public ConsumedServiceStatusEnum ConsumedServiceStatusId { get; set; }
    }
}
