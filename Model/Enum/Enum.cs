using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Model.Enum
{
    public enum UserTypeEnum
    {
        Admin = 1,
        Beautician = 2,
        User = 3,
        SalonAdmin = 4,
        SalonOwner = 5
    }

    public enum AppointmentStatusEnum
    {
        Pending = 1,
        Rejected = 2,
        Confirmed = 3,
        Completed = 4,
        UserCanceled = 5,
        EmployeeCanceled = 6
    }

    public enum ValidImageStatus
    {
        OK = 0,
        IllegalImage = 1,
        InvalidImageFormat = 2,
        InvalidImageSize = 3
    }

    public enum ServiceTypeEnum
    {
        /// <summary>
        /// 补水保湿
        /// </summary>
        Moisturize = 1,

        /// <summary>
        /// 美容祛斑
        /// </summary>
        Beauty = 2,

        /// <summary>
        /// 祛痘祛印
        /// </summary>
        Acne = 3,

        /// <summary>
        /// 紧致抗衰
        /// </summary>
        Tighten = 4,

        /// <summary>
        /// 塑身减肥
        /// </summary>
        Weight = 5,

        /// <summary>
        /// 身体脱毛
        /// </summary>
        Unhairing = 6,

        /// <summary>
        /// 理疗排毒
        /// </summary>
        Toxin = 7,

        /// <summary>
        /// 纹绣美甲
        /// </summary>
        Tattoo = 8,

        /// <summary>
        /// 抗敏脱敏
        /// </summary>
        Allergy = 9
    }


    public enum PurchaseModeEnum : byte
    {
        Single = 0,
        Treatment = 1
    }

    public enum ConsumeModeEnum : byte
    {
        //0=美容师扫码,1美管或老板扫码,2美管或老板手动扣除,3匿名买单
        BeauticianScan = 0,
        AOScan = 1,
        AOManual = 2,
        Anonym=3
    }

    public enum ConsumedServiceStatusEnum
    {
        //1:表示已消费，等待用户确认
        //2:用户已确认
        //3:完成消费，匿名买单和美容师扫码都不需要用户确认，所以匿名买单和美容师扫码后使用这个状态
        //4:用户拒绝
        NeedConfirm = 1,
        Confirmed = 2,
        Completed = 3,
        Rejected = 4
    }
}
