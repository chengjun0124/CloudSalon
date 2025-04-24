using CloudSalon.DAL;
using CloudSalon.Model;
using CloudSalon.Model.Enum;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CloudSalon.API
{
    public abstract class IllegalParamValidatorAttribute : Attribute
    {
        public string ParamName { get; set; }
        public abstract bool Validate(object o);
        public string PropertyName { get; set; }
        public BaseApiController Controller { get; set; }

        protected Identity Identity
        {
            get
            {
                return (Identity)this.Controller.User.Identity;
            }
        }

        protected void PassEntityToController(BaseModel Entity)
        {
            //传Entity给Controller，这样Controller的API里无需再次从数据库里获取这个实体
            if (this.PropertyName != null)
                this.Controller.GetType().GetProperty(this.PropertyName).SetValue(this.Controller, Entity);
        }
    }

    public class ServiceValidatorAttribute : IllegalParamValidatorAttribute
    {
        [Inject]
        public ServiceDAL serviceDAL { get; set; }

        public bool isGetTags { get; set; }
        public bool isGetServiceType { get; set; }
        public bool isGetSalon { get; set; }

        public override bool Validate(object serviceId)
        {
            Service serviceEntity = serviceDAL.GetService((int)serviceId, Identity.SalonId, isGetTags, isGetServiceType, isGetSalon);

            this.PassEntityToController(serviceEntity);

            if (serviceEntity== null)
                return false;
            else
                return true;
        }
    }

    public class BeauticianValidatorAttribute : IllegalParamValidatorAttribute
    {
        [Inject]
        public EmployeeDAL employeeDAL { get; set; }

        public override bool Validate(object employeeId)
        {
            Employee ee = employeeDAL.GetBeautician((int)employeeId, this.Identity.SalonId);

            this.PassEntityToController(ee);

            if (ee == null)
                return false;
            else
                return true;
        }
    }

    public class PurchasedServiceValidatorAttribute : IllegalParamValidatorAttribute
    {
        [Inject]
        public PurchasedServiceDAL psDAL { get; set; }

        public override bool Validate(object purchasedServiceId)
        {
            PurchasedService ps = null;

            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                ps = psDAL.GetPurchasedService((int)purchasedServiceId, this.Identity.SalonId, null);
            else
                ps = psDAL.GetPurchasedService((int)purchasedServiceId, null, this.Identity.UserId);

            if (ps == null)
                return false;
            else
            {
                this.PassEntityToController(ps);
                return true;
            }
        }
    }

    public class AppointmentValidatorAttribute : IllegalParamValidatorAttribute
    {
        AppointmentStatusEnum[] _status;

        [Inject]
        public AppointmentDAL aDAL { get; set; }

        public bool IsGetAppointmentFlow { get; set; }
        public bool IsGetEmployee { get; set; }
        public bool IsGetServiceSnapShot { get; set; }
        public bool IsGetUser { get; set; }
        public bool IsGetConsumedService { get; set; }
        public bool IsGetSalon { get; set; }


        public AppointmentValidatorAttribute(params AppointmentStatusEnum[] status)
        {
            this._status = status;
        }

        public override bool Validate(object appointmentId)
        {
            int id;

            if (this.Identity.UserType == UserTypeEnum.User || this.Identity.UserType == UserTypeEnum.Beautician)
                id = this.Identity.UserId;
            else
                id = this.Identity.SalonId;

            var a = aDAL.GetAppointment((int)appointmentId, this.Identity.UserType, id, IsGetAppointmentFlow, IsGetEmployee, IsGetServiceSnapShot, IsGetUser, IsGetConsumedService, IsGetSalon);


            if (a == null)
                return false;

            if (this._status.Length > 0)
            {
                if (!this._status.Contains((AppointmentStatusEnum)a.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).First().AppointmentStatusId))
                    return false;
            }


            this.PassEntityToController(a);
            return true;
            
        }
    }

    public class ConsumedServiceValidatorAttribute : IllegalParamValidatorAttribute
    {
        [Inject]
        public ConsumedServiceDAL csDAL { get; set; }

        ConsumedServiceStatusEnum[] _status;

        public bool IsGetConsumedServiceDetails { get; set; }
        public bool IsGetEmployee { get; set; }
        public bool IsGetService { get; set; }
        public bool IsGetAppointment { get; set; }
        public bool IsGetUser { get; set; }
        public bool IsGetAppointmentFlows { get; set; }

        public ConsumedServiceValidatorAttribute(params ConsumedServiceStatusEnum[] status)
        {
            this._status = status;
        }

        public override bool Validate(object consumedServiceId)
        {
            ConsumedService cd;

            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                cd = csDAL.GetConsumedService((int)consumedServiceId, this.Identity.UserType, this.Identity.SalonId, IsGetConsumedServiceDetails, IsGetEmployee, IsGetService, IsGetAppointment, IsGetUser, IsGetAppointmentFlows);
            else
                cd = csDAL.GetConsumedService((int)consumedServiceId, this.Identity.UserType, this.Identity.UserId, IsGetConsumedServiceDetails, IsGetEmployee, IsGetService, IsGetAppointment, IsGetUser, IsGetAppointmentFlows);

            if (cd == null)
                return false;

            if (this._status.Length > 0)
            {
                if (!this._status.Contains((ConsumedServiceStatusEnum)cd.ConsumedServiceStatusId))
                    return false;
            }

            this.PassEntityToController(cd);
            return true;
            
        }
    }

    public class UserValidatorAttribute : IllegalParamValidatorAttribute
    {
        [Inject]
        public UserDAL userDAL { get; set; }

        public override bool Validate(object userId)
        {
            User u;

            u = userDAL.GetUser((int)userId, this.Identity.SalonId, false, false);

            if (u == null)
                return false;

            this.PassEntityToController(u);
            return true;

        }
    }
}