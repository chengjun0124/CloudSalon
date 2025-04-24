using AutoMapper;
using CloudSalon.Common;
using CloudSalon.DAL;
using CloudSalon.Model;
using CloudSalon.Model.DTO;
using CloudSalon.Model.Enum;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudSalon.API.Controllers
{
    public class PurchasedServiceController : BaseApiController
    {
        [Inject]
        public PurchasedServiceDAL purchasedServiceDAL { get; set; }
        [Inject]
        public ServiceDAL serviceDAL { get; set; }
        [Inject]
        public UserDAL userDAL { get; set; }

        public PurchasedService PurchasedServiceEntity { get; set; }
        Service serviceEntity = null;
        User userEntity = null;

        #region APIs
        [HttpPost]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public int CreatePurchasedService(PurchasedServiceDTO dto)
        {
            base.Validator<PurchasedServiceDTO>(ValidateCreatePurchasedService);

            PurchasedServiceEntity = Mapper.Map<PurchasedService>(dto);
            PurchasedServiceEntity.ServiceSnapShotId = serviceEntity.ServiceSnapShots.OrderByDescending(sss => sss.ServiceSnapShotId).First().ServiceSnapShotId;
            PurchasedServiceEntity.CreatedDate = DateTime.Now;
            PurchasedServiceEntity.OperatorId = this.Identity.UserId;

            purchasedServiceDAL.Insert(PurchasedServiceEntity);
            return PurchasedServiceEntity.PurchasedServiceId;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner, UserTypeEnum.User)]
        [PurchasedServiceValidator(ParamName = "id", PropertyName = "PurchasedServiceEntity")]
        public PurchasedServiceDTO_D GetPurchasedService(int id)
        {
            PurchasedServiceEntity.ConsumedServiceDetails = PurchasedServiceEntity.ConsumedServiceDetails
                .Where(csd => csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Completed || csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Confirmed)
                .OrderByDescending(csd => csd.ConsumedService.CreatedDate).ToList();
            return Mapper.Map<PurchasedServiceDTO_D>(PurchasedServiceEntity);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner, UserTypeEnum.User)]
        [Route("purchasedservice/{pageNumber}/{pageSize}/{isAvai}/{userId?}")]
        public List<PurchasedServiceDTO_D> GetPurchasedServices(int pageNumber, int pageSize, bool? isAvai, int? userId = null)
        {
            base.Validator<int, int, bool?, int?>(ValidateGetPurchasedServices);

            List<PurchasedService> list = null;
            int id;

            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                id = userId.Value;
            else
                id = this.Identity.UserId;


            list = purchasedServiceDAL.GetPurchasedServicesByUserId(id, pageNumber, pageSize, isAvai);

            List<PurchasedServiceDTO_D> dto = new List<PurchasedServiceDTO_D>();
            list.ForEach(ps =>
            {
                ps.ConsumedServiceDetails = ps.ConsumedServiceDetails.Where(csd=>
                    csd.ConsumedService.ConsumedServiceStatusId==(int)ConsumedServiceStatusEnum.Confirmed
                    ||csd.ConsumedService.ConsumedServiceStatusId==(int)ConsumedServiceStatusEnum.Completed
                    ).OrderByDescending(csd => csd.ConsumedService.CreatedDate).ToList();
                dto.Add(Mapper.Map<PurchasedServiceDTO_D>(ps));
            });

            return dto;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner, UserTypeEnum.User)]
        [Route("purchasedservice/count/{userId?}")]
        public int GetPurchasedServiceCount(int? userId = null)
        {
            base.Validator<int?>(ValidateGetPurchasedServiceCount);

            int id;

            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                id = userId.Value;
            else
                id = this.Identity.UserId;

            return purchasedServiceDAL.GetPurchasedServiceCount(id);
        }
        #endregion

        #region validation methods
        [NonAction]
        public void ValidateGetPurchasedServiceCount(int? userId = null)
        {
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
            {
                if (!userId.HasValue)
                {
                    this.IsIllegalParameter = true;
                    return;
                }
                userEntity = userDAL.GetUser(userId.Value, this.Identity.SalonId, false, false);
                if (userEntity == null)
                    this.IsIllegalParameter = true;
            }
        }

        [NonAction]
        public void ValidateGetPurchasedServices(int pageNumber, int pageSize, bool? isAvai, int? userId = null)
        {
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
            {
                if (!userId.HasValue)
                {
                    this.IsIllegalParameter = true;
                    return;
                }
                userEntity = userDAL.GetUser(userId.Value, this.Identity.SalonId, false, false);
                if (userEntity == null)
                    this.IsIllegalParameter = true;
            }
        }

        [NonAction]
        public void ValidateCreatePurchasedService(PurchasedServiceDTO dto)
        {
            serviceEntity = serviceDAL.GetService(dto.ServiceId, this.Identity.SalonId, false, false, false);
            if (serviceEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }
            userEntity = userDAL.GetUser(dto.UserId, this.Identity.SalonId, false, false);
            if (userEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }

            if (userEntity.PurchasedServices.Where(ps => ps.ServiceSnapShot.ServiceId == dto.ServiceId && ps.Time.HasValue == false).Count() > 0)
            {
                this.InvalidMessages.Add("该用户已购买过此服务，并且服务次数为无限制");
            }


            this.ValidatorContainer.SetValue("服务次数", dto.Time)
                .InRange((byte)1, (byte)100, null);

            this.ValidatorContainer.SetValue("模式", dto.Mode)
                .IsInList(PurchaseModeEnum.Single, PurchaseModeEnum.Treatment);

            this.ValidatorContainer.SetValue("实付金额", dto.Payment)
            .InRange(1m, 9999999.99m, null)
            .DecimalLength(2);    
        }
        #endregion
        
    }
    
}