using AutoMapper;
using CloudSalon.Common;
using CloudSalon.DAL;
using CloudSalon.Model;
using CloudSalon.Model.DTO;
using CloudSalon.Model.Enum;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CloudSalon.API.Controllers
{
    //[EnableCors(origins: "http://192.168.0.104", headers: "authorization,content-type", methods: "PUT")]
    public class ServiceController : BaseApiController
    {
        [Inject]
        public ServiceDAL serviceDAL { get; set; }
        [Inject]
        public SalonDAL salonDAL { get; set; }
        [Inject]
        public ServiceEffectImageDAL serviceEffectImageDAL { get; set; }
        [Inject]
        public UserDAL userDAL { get; set; }
        [Inject]
        public ServiceFunctionalityTagDAL serviceFunctionalityTagDAL { get; set; }
        


        Salon salonEntity = null;
        public Service ServiceEntity { get; set; }
        public User UserEntity { get; set; }

        #region APIs
        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [UserValidator(ParamName = "userId", PropertyName = "UserEntity")]
        [Route("service/avai/user/{userId}")]
        public List<AvaiServiceDTO_D> getAvaiServices(int userId)
        {
            List<AvaiServiceDTO_D> list = new List<AvaiServiceDTO_D>();

            UserEntity.PurchasedServices.GroupBy(ps => ps.ServiceSnapShot.ServiceId).ToList().ForEach(s =>
            {
                if (s.Where(ps => ps.Time.HasValue == false).Count() > 0
                    ||
                    s.Sum(ps => ps.Time) - s.Sum(ps => ps.ConsumedServiceDetails.Where(csd => csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Completed || csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Confirmed).Sum(csd => csd.Time)) > 0)
                {
                    AvaiServiceDTO_D avaiServiceDTO = new AvaiServiceDTO_D();
                    avaiServiceDTO.ServiceId = s.Key;
                    avaiServiceDTO.ServiceName = s.First().ServiceSnapShot.Service.ServiceName;
                    avaiServiceDTO.Interval = s.First().ServiceSnapShot.Service.TreatmentInterval;
                    avaiServiceDTO.SmallSubjectImage = s.First().ServiceSnapShot.Service.SubjectImage == null ? null : Constant.SERVICE_SUBJECT_IMAGE_FOLDER_URL + "s" + s.First().ServiceSnapShot.Service.SubjectImage;
                    if (s.Where(ps => ps.Time.HasValue == false).Count() > 0)
                        avaiServiceDTO.Time = null;
                    else
                        avaiServiceDTO.Time = s.Sum(ps => ps.Time);

                    avaiServiceDTO.ConsumedServices = new List<ConsumedServiceDetailDTO_D>();

                    List<ConsumedServiceDetail> csd1 = new List<ConsumedServiceDetail>();
                    s.ToList().ForEach(ps => csd1.AddRange(ps.ConsumedServiceDetails.Where(csd => csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Completed || csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Confirmed)));
                    csd1.OrderByDescending(csd => csd.ConsumedService.ConsumedServiceId).ToList().ForEach(csd =>
                    {
                        avaiServiceDTO.ConsumedServices.Add(Mapper.Map<ConsumedServiceDetailDTO_D>(csd));
                    });

                    list.Add(avaiServiceDTO);
                }

            });
            return list;
        }

        [HttpGet]
        [Route("service/servicetypes")]
        public List<ServiceTypeDTO> GetServiceTypes()
        {
            List<ServiceTypeDTO> list = new List<ServiceTypeDTO>();
            serviceDAL.GetServiceTypes().ForEach(st => list.Add(Mapper.Map<ServiceTypeDTO>(st)));

            return list;
        }

        [HttpGet]
        [Route("service/hasserviceservicetypes/{identityCode}")]
        public List<ServiceTypeDTO> GetHasServiceServiceTypes(string identityCode)
        {
            base.Validator<string>(ValidGetHasServiceServiceTypes);

            List<ServiceTypeDTO> list = new List<ServiceTypeDTO>();
            serviceDAL.GetServiceTypesHasService(salonEntity.SalonId).ForEach(st => list.Add(Mapper.Map<ServiceTypeDTO>(st)));

            return list;

        }

        [HttpPost]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public int CreateService(ServiceDTO dto)
        {
            base.Validator<ServiceDTO>(ValidateCreateService);

            var service = Mapper.Map<Service>(dto);
            service.CreatedDate = DateTime.Now;
            service.SalonId = this.Identity.SalonId;
            service.EditBy = this.Identity.UserId;
            service.Detail = EncodingHelper.FilterXSS(dto.Detail);
            
            if (dto.SubjectImage != null)
            {
                service.SubjectImage = FileHelper.SaveImageWithThumbnail(Constant.SERVICE_SUBJECT_IMAGE_FOLDER_Absolute, dto.SubjectImage, Constant.SERVICE_SUBJECT_IMAGE_WIDTH_THUMBNAIL, Constant.SERVICE_SUBJECT_IMAGE_HEIGHT_THUMBNAIL);
            }

            if (dto.EffectImages != null)
            {
                for (int i = 0; i < dto.EffectImages.Count; i++)
                {
                    service.EffectImages.Add(new ServiceEffectImage() 
                    {
                        FileName = FileHelper.SaveImageWithThumbnail(Constant.SERVICE_EFFECT_IMAGE_FOLDER_Absolute, dto.EffectImages[i].Image, Constant.SERVICE_EFFECT_IMAGE_WIDTH_THUMBNAIL,Constant.SERVICE_EFFECT_IMAGE_HEIGHT_THUMBNAIL),
                        Seq = i + 1
                    });
                }
            }

            //insert data into ServiceFunctionalityTags
            for (int i = 0; i < dto.FunctionalityTags.Count; i++)
            {
                service.FunctionalityTags.Add(new ServiceFunctionalityTag()
                {
                    TagName = dto.FunctionalityTags[i],
                    Seq = i + 1
                });
            }

            //insert data into ServiceSnapShots
            ServiceSnapShot serviceSnapShot = new ServiceSnapShot();
            serviceSnapShot.ServiceName =service.ServiceName;
            serviceSnapShot.ServiceTypeId =service.ServiceTypeId;
            serviceSnapShot.Pain =service.Pain;
            serviceSnapShot.Duration =service.Duration;
            serviceSnapShot.OncePrice =service.OncePrice;
            serviceSnapShot.TreatmentPrice =service.TreatmentPrice;
            serviceSnapShot.TreatmentTime =service.TreatmentTime;
            serviceSnapShot.TreatmentInterval =service.TreatmentInterval;
            serviceSnapShot.OncePriceOnSale = service.OncePriceOnSale;
            serviceSnapShot.TreatmentPriceOnSale = service.TreatmentPriceOnSale;
            serviceSnapShot.TreatmentTimeOnSale = service.TreatmentTimeOnSale;
            //serviceSnapShot.Functionality =service.Functionality;
            serviceSnapShot.SuitablePeople =service.SuitablePeople;
            serviceSnapShot.Detail = service.Detail;
            serviceSnapShot.CreatedDate =service.CreatedDate;
            serviceSnapShot.SubjectImage =service.SubjectImage;
            serviceSnapShot.EditBy =this.Identity.UserId;

            //insert data into ServiceEffectImageSnapShots
            for (int i = 0; i < service.EffectImages.Count; i++)
			{
                serviceSnapShot.EffectImages.Add(new ServiceEffectImageSnapShot() 
                {
                    FileName = service.EffectImages.ToList()[i].FileName,
                    Seq = service.EffectImages.ToList()[i].Seq
                });
			}

            //insert data into ServiceFunctionalityTagsSnapShots
            for (int i = 0; i < dto.FunctionalityTags.Count; i++)
            {
                serviceSnapShot.FunctionalityTags.Add(new ServiceFunctionalityTagSnapShot()
                {
                    TagName = dto.FunctionalityTags[i],
                    Seq = i + 1
                });   
            }

            service.ServiceSnapShots.Add(serviceSnapShot);
            serviceDAL.Insert(service);

            return service.ServiceId;
        }

        [HttpPut]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void UpdateService(ServiceDTO dto)
        {
            base.Validator<ServiceDTO>(ValidateUpdateService);


            int oldEffectImageCount = ServiceEntity.EffectImages.Count;
            int newEffectImageCount = dto.EffectImages == null ? 0 : dto.EffectImages.Count;
            int oldTagCount = ServiceEntity.FunctionalityTags.Count;
            int newTagCount = dto.FunctionalityTags.Count;

            //check old and new data, is there any property changed?
            bool bolIsChanged = ServiceEntity.ServiceName != dto.ServiceName
                || ServiceEntity.ServiceTypeId != (int)dto.ServiceTypeId
                || ServiceEntity.Pain != dto.Pain
                || ServiceEntity.Duration != dto.Duration
                || ServiceEntity.OncePrice != dto.OncePrice
                || ServiceEntity.TreatmentPrice != dto.TreatmentPrice
                || ServiceEntity.TreatmentTime != dto.TreatmentTime
                || ServiceEntity.OncePriceOnSale != dto.OncePriceOnSale
                || ServiceEntity.TreatmentPriceOnSale != dto.TreatmentPriceOnSale
                || ServiceEntity.TreatmentTimeOnSale != dto.TreatmentTimeOnSale
                || ServiceEntity.TreatmentInterval != dto.TreatmentInterval
                //|| serviceEntity.Functionality != dto.Functionality
                || ServiceEntity.SuitablePeople != dto.SuitablePeople
                || ServiceEntity.Detail != dto.Detail
                || ServiceEntity.Seq != dto.Seq
                || dto.SubjectImage != null
                || oldEffectImageCount != newEffectImageCount
                || oldTagCount != newTagCount;

            //check EffectImages changed or not
            if (!bolIsChanged)
            {
                if (dto.EffectImages != null)
                {
                    List<ServiceEffectImage> oldImages = ServiceEntity.EffectImages.OrderBy(s => s.Seq).ToList();
                    for (int i = 0; i < dto.EffectImages.Count; i++)
                    {
                        if (dto.EffectImages[i].Image != oldImages[i].FileName)
                        {
                            bolIsChanged = true;
                            break;
                        }
                    }
                }
            }

            //check tags changed or not
            if (!bolIsChanged)
            {
                bolIsChanged = !dto.FunctionalityTags.SequenceEqual(ServiceEntity.FunctionalityTags.OrderBy(t => t.Seq).Select(t => t.TagName));
            }

            //only save when property changed
            if (bolIsChanged)
            {
                ServiceEntity.ServiceName = dto.ServiceName;
                ServiceEntity.ServiceTypeId = (int)dto.ServiceTypeId;
                ServiceEntity.Pain = dto.Pain;
                ServiceEntity.Duration = dto.Duration;
                ServiceEntity.OncePrice = dto.OncePrice;
                ServiceEntity.TreatmentPrice = dto.TreatmentPrice;
                ServiceEntity.TreatmentTime = dto.TreatmentTime;
                ServiceEntity.OncePriceOnSale = dto.OncePriceOnSale;
                ServiceEntity.TreatmentPriceOnSale = dto.TreatmentPriceOnSale;
                ServiceEntity.TreatmentTimeOnSale = dto.TreatmentTimeOnSale;
                ServiceEntity.TreatmentInterval = dto.TreatmentInterval;
                //serviceEntity.Functionality = dto.Functionality;
                ServiceEntity.SuitablePeople = dto.SuitablePeople;
                ServiceEntity.Detail = EncodingHelper.FilterXSS(dto.Detail);
                ServiceEntity.UpdatedDate = DateTime.Now;
                ServiceEntity.Seq = dto.Seq;
                ServiceEntity.EditBy = this.Identity.UserId;


                if (dto.SubjectImage != null)
                    ServiceEntity.SubjectImage = FileHelper.SaveImageWithThumbnail(Constant.SERVICE_SUBJECT_IMAGE_FOLDER_Absolute, dto.SubjectImage, Constant.SERVICE_SUBJECT_IMAGE_WIDTH_THUMBNAIL, Constant.SERVICE_SUBJECT_IMAGE_HEIGHT_THUMBNAIL);

                List<ServiceEffectImage> effectImages = effectImages = new List<ServiceEffectImage>();
                if (dto.EffectImages != null)
                {
                    int seq = 1;
                    for (int i = 0; i < dto.EffectImages.Count; i++)
                    {
                        if (ServiceEntity.EffectImages.Where(img => img.FileName == dto.EffectImages[i].Image).Count() == 1)
                        {
                            effectImages.Add(new ServiceEffectImage()
                            {
                                FileName = dto.EffectImages[i].Image,
                                Seq = seq++
                            });
                        }
                        else
                        {
                            effectImages.Add(new ServiceEffectImage()
                            {
                                FileName = FileHelper.SaveImageWithThumbnail(Constant.SERVICE_EFFECT_IMAGE_FOLDER_Absolute, dto.EffectImages[i].Image, Constant.SERVICE_EFFECT_IMAGE_WIDTH_THUMBNAIL, Constant.SERVICE_EFFECT_IMAGE_HEIGHT_THUMBNAIL),
                                Seq = seq++
                            });
                        }
                    }
                }
                serviceEffectImageDAL.Delete(ServiceEntity.EffectImages);
                ServiceEntity.EffectImages = effectImages;

                //kill and fill ServiceFunctionalityTags
                serviceFunctionalityTagDAL.Delete(ServiceEntity.FunctionalityTags);
                for (int i = 0; i < dto.FunctionalityTags.Count; i++)
                {
                    ServiceEntity.FunctionalityTags.Add(new ServiceFunctionalityTag()
                    {
                        TagName = dto.FunctionalityTags[i],
                        Seq = i + 1
                    });
                }


                //insert data into ServiceSnapShots
                ServiceSnapShot serviceSnapShot = new ServiceSnapShot();
                serviceSnapShot.ServiceName = ServiceEntity.ServiceName;
                serviceSnapShot.ServiceTypeId = ServiceEntity.ServiceTypeId;
                serviceSnapShot.Pain = ServiceEntity.Pain;
                serviceSnapShot.Duration = ServiceEntity.Duration;
                serviceSnapShot.OncePrice = ServiceEntity.OncePrice;
                serviceSnapShot.TreatmentPrice = ServiceEntity.TreatmentPrice;
                serviceSnapShot.TreatmentTime = ServiceEntity.TreatmentTime;
                serviceSnapShot.OncePriceOnSale = ServiceEntity.OncePriceOnSale;
                serviceSnapShot.TreatmentPriceOnSale = ServiceEntity.TreatmentPriceOnSale;
                serviceSnapShot.TreatmentTimeOnSale = ServiceEntity.TreatmentTimeOnSale;
                serviceSnapShot.TreatmentInterval = ServiceEntity.TreatmentInterval;
                //serviceSnapShot.Functionality = serviceEntity.Functionality;
                serviceSnapShot.SuitablePeople = ServiceEntity.SuitablePeople;
                serviceSnapShot.Detail = ServiceEntity.Detail;
                serviceSnapShot.CreatedDate = ServiceEntity.UpdatedDate.Value;
                serviceSnapShot.SubjectImage = ServiceEntity.SubjectImage;
                serviceSnapShot.EditBy = this.Identity.UserId;

                //insert data into ServiceEffectImageSnapShots
                for (int i = 0; i < ServiceEntity.EffectImages.Count; i++)
                {
                    serviceSnapShot.EffectImages.Add(new ServiceEffectImageSnapShot()
                    {
                        FileName = ServiceEntity.EffectImages.ToList()[i].FileName,
                        Seq = ServiceEntity.EffectImages.ToList()[i].Seq
                    });
                }


                //insert data into ServiceFunctionalityTagSnapShots
                for (int i = 0; i < dto.FunctionalityTags.Count; i++)
                {
                    serviceSnapShot.FunctionalityTags.Add(new ServiceFunctionalityTagSnapShot()
                    {
                        TagName = dto.FunctionalityTags[i],
                        Seq = i + 1
                    });
                }
                

                ServiceEntity.ServiceSnapShots.Add(serviceSnapShot);

                serviceDAL.Update(ServiceEntity);
            }
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.Beautician, UserTypeEnum.SalonOwner)]
        [Route("services/{pageNumber}/{pageSize}")]
        public List<ServiceDTO> GetServices(int pageNumber, int pageSize)
        {
            var service = serviceDAL.GetServices(this.Identity.SalonId, pageNumber, pageSize, null, true, true, true, true);

            List<ServiceDTO> serviceDTOs = new List<ServiceDTO>();
            service.ForEach(ee => serviceDTOs.Add(Mapper.Map<ServiceDTO>(ee)));

            return serviceDTOs;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.User)]
        [Route("service/hot/{top}")]
        public List<ServiceDTO> GetHotServices(int top)
        {
            List<Service> services = serviceDAL.GetHotServices(top, this.Identity.SalonId);

            List<ServiceDTO> serviceDTOs = new List<ServiceDTO>();
            services.ForEach(s => serviceDTOs.Add(Mapper.Map<ServiceDTO>(s)));

            return serviceDTOs;
        }
        
        [HttpGet]
        [Route("service/{pageNumber}/{pageSize}/{identityCode}/{serviceTypeId?}")]
        public List<ServiceDTO> GetServicesByIdentityCode(int pageNumber, int pageSize, string identityCode, int? serviceTypeId = null)
        {
            base.Validator<int, int, string, int?>(ValidGetServicesByIdentityCode);

            List<Service> service = serviceDAL.GetServices(salonEntity.SalonId, pageNumber, pageSize, serviceTypeId, true, true, true, true);

            List<ServiceDTO> serviceDTOs = new List<ServiceDTO>();
            service.ForEach(s => serviceDTOs.Add(Mapper.Map<ServiceDTO>(s)));

            return serviceDTOs;
        }


        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.Beautician, UserTypeEnum.SalonOwner)]
        [ServiceValidator(ParamName = "id", PropertyName = "ServiceEntity", isGetTags = true, isGetServiceType = true, isGetSalon = true)]
        public ServiceDTO GetService(int id)
        {
            ServiceDTO serviceDTO = Mapper.Map<ServiceDTO>(ServiceEntity);
            return serviceDTO;
        }

        [HttpGet]
        [Route("service/{serviceId}/{identityCode}")]
        public ServiceDTO GetServiceByIdentityCode(int serviceId, string identityCode)
        {
            base.Validator<int, string>(ValidGetServiceByIdentityCode);

            return Mapper.Map<ServiceDTO>(ServiceEntity);
        }

        [HttpDelete]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void DeleteService(int id)
        {
            base.Validator<int>(ValidateDeleteService);

            ServiceEntity.IsDeleted = true;
            ServiceEntity.UpdatedDate = DateTime.Now;
            serviceDAL.Update(ServiceEntity);
        }
        

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.Beautician, UserTypeEnum.SalonOwner)]
        [Route("service/count")]
        public int GetServiceCount()
        {
            return serviceDAL.GetServiceCount(this.Identity.SalonId);
        }
        #endregion

        #region validation methods
        [NonAction]
        public void ValidGetServicesByIdentityCode(int pageNumber, int pageSize, string identityCode, int? serviceTypeId = null)
        {
            salonEntity = salonDAL.GetSalonByIdentityCode(identityCode);
            if (salonEntity == null)
                this.IsIllegalParameter = true;
        }

        [NonAction]
        public void ValidGetHasServiceServiceTypes(string identityCode)
        {
            salonEntity = salonDAL.GetSalonByIdentityCode(identityCode);
            if (salonEntity == null)
                this.IsIllegalParameter = true;
        }

        [NonAction]
        public void ValidGetServiceByIdentityCode(int serviceId, string identityCode)
        {
            salonEntity = salonDAL.GetSalonByIdentityCode(identityCode);
            if (salonEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }

            ServiceEntity = serviceDAL.GetService(serviceId, salonEntity.SalonId, false, false, false);
            if (ServiceEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }
        }

        private void ValidateService(ServiceDTO dto, bool IsCreate)
        {
            this.ValidatorContainer.SetValue(dto.SubjectImage)
                .IsImage(Constant.SERVICE_SUBJECT_IMAGE_WIDTH, Constant.SERVICE_SUBJECT_IMAGE_HEIGHT);

            this.ValidatorContainer.SetValue("服务名称", dto.ServiceName)
                .IsRequired()
                .Length(null, 16);

            this.ValidatorContainer.SetValue("服务类别", dto.ServiceTypeId)
                .IsInList(ServiceTypeEnum.Acne, ServiceTypeEnum.Beauty, ServiceTypeEnum.Moisturize, ServiceTypeEnum.Tighten, ServiceTypeEnum.Toxin, ServiceTypeEnum.Unhairing, ServiceTypeEnum.Weight, ServiceTypeEnum.Tattoo, ServiceTypeEnum.Allergy);

            this.ValidatorContainer.SetValue("疼痛指数", dto.Pain)
                .InRange((byte)0, (byte)5, null);

            this.ValidatorContainer.SetValue("服务时长", dto.Duration)
                .InRange(1, 150, null);


            this.ValidatorContainer.SetValue("单次价格和疗程价格", new object[] { dto.OncePrice, dto.TreatmentPrice })
                .IsOneRequired();

            this.ValidatorContainer.SetValue("单次价格", dto.OncePrice)
                .InRange(1m, 9999999.99m, null)
                .DecimalLength(2);

            this.ValidatorContainer.SetValue("疗程价格", dto.TreatmentPrice)
                .InRange(1m, 9999999.99m, null)
                .DecimalLength(2);

            this.ValidatorContainer.SetValue("疗程间隔", dto.TreatmentInterval);
            if (dto.TreatmentPrice.HasValue || dto.TreatmentPriceOnSale.HasValue)
                this.ValidatorContainer.IsRequired();

            this.ValidatorContainer
                .InRange((byte)1, (byte)200, null);

            this.ValidatorContainer.SetValue("疗程次数", dto.TreatmentTime)
                .InRange((byte)1, (byte)100, null);


            this.ValidatorContainer.SetValue("优惠单价", dto.OncePriceOnSale)
                .InRange(1m, 9999999.99m, null)
                .DecimalLength(2);

            this.ValidatorContainer.SetValue("优惠疗程价", dto.TreatmentPriceOnSale)
                .InRange(1m, 9999999.99m, null)
                .DecimalLength(2);

            this.ValidatorContainer.SetValue("优惠疗程次数", dto.TreatmentTimeOnSale)
                .InRange((byte)1, (byte)100, null);

            if (dto.FunctionalityTags != null)
            {
                //remove white space
                for (int i = 0; i < dto.FunctionalityTags.Count; i++)
                {
                    if (dto.FunctionalityTags[i] != null)
                        dto.FunctionalityTags[i] = Regex.Replace(dto.FunctionalityTags[i], @"\s", "");
                }
                //remove empty tag
                dto.FunctionalityTags.RemoveAll(t => string.IsNullOrEmpty(t));

                //remove duplicated tag
                dto.FunctionalityTags = dto.FunctionalityTags.Distinct().ToList();
            }
            this.ValidatorContainer.SetValue("美容功效", dto.FunctionalityTags)
                .IsRequired()
                .Custom(() => {
                    return dto.FunctionalityTags.Count <= 5;
                },"最多放置5个标签")
                .Custom(() =>
                {
                    return dto.FunctionalityTags.FindIndex(t => t.Length > 5) == -1;
                }, "单个标签不能超过5个字符");



            this.ValidatorContainer.SetValue("适用对象", dto.SuitablePeople)
                .Length(null, 200);

            this.ValidatorContainer.SetValue("服务详情", dto.Detail)
                .IsRequired();


            if (dto.EffectImages != null)
            {
                for (int i = 0; i < dto.EffectImages.Count; i++)
                {
                    //新建的service的所有EffectImages都要验证
                    //或者不是新建的service，但EffectImage在数据库中不存在的，说明这张EffectImage是新增的，需要验证
                    if (IsCreate || ServiceEntity.EffectImages.Where(img => img.FileName == dto.EffectImages[i].Image).Count() == 0)
                    {
                        this.ValidatorContainer.SetValue("效果图", dto.EffectImages[i].Image)
                            .IsImage(Constant.SERVICE_EFFECT_IMAGE_WIDTH, Constant.SERVICE_EFFECT_IMAGE_HEIGHT);
                    }
                }
            }
        }

        [NonAction]
        public void ValidateUpdateService(ServiceDTO dto)
        {
            ServiceEntity = serviceDAL.GetService(dto.ServiceId, this.Identity.SalonId, false, false, false);
            if (ServiceEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }

            ValidateService(dto, false);
        }

        [NonAction]
        public void ValidateCreateService(ServiceDTO dto)
        {
            ValidateService(dto, true);
        }

        [NonAction]
        public void ValidateDeleteService(int id)
        {
            ServiceEntity = serviceDAL.GetService(id, this.Identity.SalonId, false, false, false);
            if (ServiceEntity == null)
                this.IsIllegalParameter = true;
        }
        #endregion
    }
}
