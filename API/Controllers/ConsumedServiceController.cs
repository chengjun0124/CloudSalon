
using AutoMapper;
using CloudSalon.DAL;
using CloudSalon.Model;
using CloudSalon.Model.DTO;
using CloudSalon.Model.Enum;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CloudSalon.API.Controllers
{
    public class ConsumedServiceController : BaseApiController
    {
        [Inject]
        public AppointmentDAL appointmentDAL { get; set; }
        [Inject]
        public UserDAL userDAL { get; set; }
        [Inject]
        public ConsumedServiceDAL consumedServiceDAL { get; set; }
        [Inject]
        public ServiceDAL serviceDAL { get; set; }
        [Inject]
        public EmployeeDAL employeeDAL { get; set; }
        [Inject]
        public PurchasedServiceDAL purchasedServiceDAL { get; set; }


        public Appointment AppointmentEntity { get; set; }
        public PurchasedService PurchasedServiceEntity { get; set; }
        public List<PurchasedService> PurchasedServiceEntities { get; set; }
        public Service ServiceEntity { get; set; }
        public Employee EmployeeEntity { get; set; }
        public ConsumedService ConsumedServiceEntity { get; set; }
        public User UserEntity { get; set; }

        #region APIs
        [HttpPost]
        [ApiAuthorize(UserTypeEnum.Beautician)]
        [Route("consumedservice/beauticianscan")]
        public int BeauticianScan(CheckBeauticianScanDTO_P dto)
        {
            base.Validator<CheckBeauticianScanDTO_P>(ValidateBeauticianScan);

            ConsumedService cs = new ConsumedService()
            {
                UserId = UserEntity.UserId,
                EmployeeId = this.Identity.UserId,
                CreatedDate = DateTime.Now,
                Mode = (int)ConsumeModeEnum.BeauticianScan,
                ConsumedServiceStatusId = (int)ConsumedServiceStatusEnum.Completed
            };

            ConsumedServiceDetail csd = new ConsumedServiceDetail() 
            {
                Time = 1
            };

            cs.ConsumedServiceDetails.Add(csd);
            PurchasedServiceEntities[0].ConsumedServiceDetails.Add(csd);
            AppointmentEntity.ConsumedServices.Add(cs);

            AppointmentEntity.AppointmentFlows.Add(new AppointmentFlow()
            {
                AppointmentStatusId = (int)AppointmentStatusEnum.Completed,
                CreatedDate = DateTime.Now
            });

            appointmentDAL.Update(AppointmentEntity);

            return PurchasedServiceEntities[0].PurchasedServiceId;
        }

        [HttpPost]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [PurchasedServiceValidator(ParamName = "dto.PurchasedServiceId", PropertyName = "PurchasedServiceEntity")]
        [BeauticianValidator(ParamName = "dto.EmployeeId")]
        [AppointmentValidator(AppointmentStatusEnum.Pending,
            AppointmentStatusEnum.Confirmed,
            ParamName = "dto.AppointmentId",
            PropertyName = "AppointmentEntity",
            IsGetAppointmentFlow = true)]
        [Route("consumedservice/aocharge")]
        public void AOCharge(CheckAOManualDTO_P dto)
        {
            base.Validator<CheckAOManualDTO_P>(ValidateAOCharge);

            ConsumedService cs = new ConsumedService()
            {
                UserId = PurchasedServiceEntity.UserId,
                EmployeeId = dto.EmployeeId,
                CreatedDate = DateTime.Now,
                OperatorId = this.Identity.UserId,
                Mode = (int)ConsumeModeEnum.AOManual,
                ConsumedServiceStatusId = (int)ConsumedServiceStatusEnum.NeedConfirm,
                ChangeTimeReason = dto.ChangeTimeReason
            };

            cs.ConsumedServiceDetails.Add(new ConsumedServiceDetail()
            {
                Time = dto.Time,
                PurchasedServiceId = dto.PurchasedServiceId
            });

            /*
             bugid 108之前,有关联的预约，就把预约状态设置成完成
             bugid 108之后，逻辑变化了，创建消费单后不修改预约状态，因为消费单是可以修改，也可以撤销的，所以要等到用户确认后，预约状态才修改成已完成
             */
            if (AppointmentEntity != null)
            {
                AppointmentEntity.ConsumedServices.Add(cs);
                appointmentDAL.Update(AppointmentEntity);
            }
            else//无预约的，直接插入ConsumedService
                consumedServiceDAL.Insert(cs);
        }

        [HttpPost]
        [ApiAuthorize(UserTypeEnum.SalonAdmin,UserTypeEnum.SalonOwner)]
        [ServiceValidator(ParamName = "dto.ServiceId", PropertyName = "ServiceEntity")]
        [BeauticianValidator(ParamName = "dto.EmployeeId", PropertyName = "EmployeeEntity")]
        [AppointmentValidator(AppointmentStatusEnum.Pending, 
            AppointmentStatusEnum.Confirmed, 
            ParamName = "dto.AppointmentId", 
            PropertyName = "AppointmentEntity",
            IsGetAppointmentFlow = true)]
        [Route("consumedservice/aoscan")]
        public void AOScan(CheckAOScanDTO_P dto)
        {
            base.Validator<CheckAOScanDTO_P>(ValidateAOScan);

            //创建消费单
            ConsumedService cs = new ConsumedService()
            {
                UserId = UserEntity.UserId,
                EmployeeId = dto.EmployeeId,
                CreatedDate = DateTime.Now,
                Mode = (int)ConsumeModeEnum.AOScan,
                OperatorId = this.Identity.UserId,
                ConsumedServiceStatusId = (int)ConsumedServiceStatusEnum.NeedConfirm,
                ChangeTimeReason = dto.ChangeTimeReason
            };

            AddConsumedServiceDetails(cs, dto.Time);

            /*
             bugid 108之前,有关联预约的，就把预约状态设置成完成
             bugid 108之后，逻辑变化了，创建消费单后不修改预约状态，因为消费单是可以修改，也可以撤销，所以要等到用户确认后，预约状态才修改成已完成
             */
            if (AppointmentEntity != null)
            {
                AppointmentEntity.ConsumedServices.Add(cs);
                appointmentDAL.Update(AppointmentEntity);
            }
            else//无预约的，直接插入ConsumedService
                consumedServiceDAL.Insert(cs);
            
        }

        [HttpPost]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [ServiceValidator(ParamName = "dto.ServiceId", PropertyName = "ServiceEntity")]
        [BeauticianValidator(ParamName = "dto.EmployeeId")]
        [Route("consumedservice/checkanonym")]
        public void CheckAnonym(CheckAnonymDTO_P dto)
        {
            base.Validator<CheckAnonymDTO_P>(ValidateCheckAnonym);

            PurchasedService ps = new PurchasedService();
            ps.ServiceSnapShotId = ServiceEntity.ServiceSnapShots.OrderByDescending(sss => sss.ServiceSnapShotId).First().ServiceSnapShotId;
            ps.Time = dto.Time;
            ps.Payment = dto.Payment;
            ps.Mode = (int)PurchaseModeEnum.Single;
            ps.CreatedDate = DateTime.Now;
            ps.OperatorId = this.Identity.UserId;

            ConsumedService cs = new ConsumedService();
            cs.EmployeeId = dto.EmployeeId;
            cs.CreatedDate = ps.CreatedDate;
            cs.OperatorId = this.Identity.UserId;
            cs.ConsumedServiceStatusId = (int)ConsumedServiceStatusEnum.Completed;
            cs.Mode = (int)ConsumeModeEnum.Anonym;
            cs.ChangeTimeReason = dto.ChangeTimeReason;

            ConsumedServiceDetail csd = new ConsumedServiceDetail();
            csd.Time = dto.Time;

            csd.ConsumedService = cs;
            ps.ConsumedServiceDetails.Add(csd);


            purchasedServiceDAL.Insert(ps);
        }

        [HttpPut]
        [ApiAuthorize(UserTypeEnum.User)]
        [ConsumedServiceValidator(ConsumedServiceStatusEnum.NeedConfirm, ParamName = "dto.ConsumedServiceId", PropertyName = "ConsumedServiceEntity",
            IsGetAppointmentFlows=true)]
        [Route("consumedservice/changeconsumedservicestatus")]
        public void ChangeConsumedServiceStatus(ChangeConsumedServiceStatus_P dto)
        {
            base.Validator<ChangeConsumedServiceStatus_P>(ValidChangeConsumedServiceStatus);

            ConsumedServiceEntity.ConsumedServiceStatusId = (int)dto.ConsumedServiceStatusId;
            if (dto.ConsumedServiceStatusId == ConsumedServiceStatusEnum.Confirmed)
            {
                ConsumedServiceEntity.UserConfirmedDate = DateTime.Now;
                //用户确认服务后，如果关联预约，并且预约状态不是已完成，需要修改预约状态至已完成
                if (ConsumedServiceEntity.Appointment != null)
                {
                    if (ConsumedServiceEntity.Appointment.AppointmentFlows.Where(af => af.AppointmentStatusId == (int)AppointmentStatusEnum.Completed).Count() == 0)
                    {
                        ConsumedServiceEntity.Appointment.AppointmentFlows.Add(new AppointmentFlow()
                        {
                            AppointmentStatusId = (int)AppointmentStatusEnum.Completed,
                            CreatedDate = ConsumedServiceEntity.UserConfirmedDate.Value
                        });
                    }
                }
            }
            consumedServiceDAL.Update(ConsumedServiceEntity);
        }

        [HttpPut]
        [ConsumedServiceValidator(ConsumedServiceStatusEnum.Rejected, ParamName = "dto.ConsumedServiceId", PropertyName = "ConsumedServiceEntity")]
        [ServiceValidator(ParamName = "dto.ServiceId", PropertyName = "ServiceEntity")]
        [BeauticianValidator(ParamName = "dto.EmployeeId")]
        [AppointmentValidator(AppointmentStatusEnum.Pending,
            AppointmentStatusEnum.Confirmed,
            ParamName = "dto.AppointmentId",
            PropertyName = "AppointmentEntity")]
        [Route("consumedservice/changeconsumedservice")]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void ChangeConsumedService(ChangeConsumedService_P dto)
        {
            base.Validator<ChangeConsumedService_P>(ValidateChangeConsumedService);

            consumedServiceDAL.Delete(ConsumedServiceEntity.ConsumedServiceDetails);

            ConsumedServiceEntity.ConsumedServiceStatusId = (int)ConsumedServiceStatusEnum.NeedConfirm;
            ConsumedServiceEntity.ChangeTimeReason = dto.ChangeTimeReason;
            ConsumedServiceEntity.EmployeeId = dto.EmployeeId;
            ConsumedServiceEntity.AppointmentId = dto.AppointmentId;

            AddConsumedServiceDetails(ConsumedServiceEntity, dto.Time);
            consumedServiceDAL.Update(ConsumedServiceEntity);
        }

        [HttpDelete]
        [ConsumedServiceValidator(ConsumedServiceStatusEnum.Rejected, ConsumedServiceStatusEnum.NeedConfirm, ParamName = "id", PropertyName = "ConsumedServiceEntity")]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void DeleteConsumedService(int id)
        {
            base.Validator<int>(ValidDeleteConsumedService);

            consumedServiceDAL.Delete(ConsumedServiceEntity);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.User)]
        public List<ConsumedServiceDTO_D> GetUnConfirmedConsumedServices()
        {
            List<ConsumedServiceDTO_D> list = new List<ConsumedServiceDTO_D>();
            consumedServiceDAL.GetUnConfirmedConsumedServices(this.Identity.UserId, true, true, true, true).ForEach(cs => 
            {
                list.Add(Mapper.Map<ConsumedServiceDTO_D>(cs));
            });

            return list;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.User, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [ConsumedServiceValidator(ParamName = "id", 
            PropertyName = "ConsumedServiceEntity", 
            IsGetConsumedServiceDetails = true, 
            IsGetEmployee = true, 
            IsGetService = true, 
            IsGetAppointment = true,
            IsGetUser=true,
            IsGetAppointmentFlows=true)]
        public ConsumedServiceDTO_D GetConsumedService(int id)
        {
            return Mapper.Map<ConsumedServiceDTO_D>(ConsumedServiceEntity);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("consumedservice/{date}/{pageNumber}/{pageSize}")]
        public List<ConsumedServiceDTO_D> GetConsumedServices(DateTime date, int pageNumber, int pageSize)
        {
            List<ConsumedService> consumedServices = null;
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                consumedServices = consumedServiceDAL.GetConsumedServices(this.Identity.SalonId, null, date.Date, pageNumber, pageSize);
            else
                consumedServices = consumedServiceDAL.GetConsumedServices(null, this.Identity.UserId, date.Date, pageNumber, pageSize);

            List<ConsumedServiceDTO_D> consumedServiceDTOs = new List<ConsumedServiceDTO_D>();
            consumedServices.ForEach(cs =>
            {
                consumedServiceDTOs.Add(Mapper.Map<ConsumedServiceDTO_D>(cs));
            });

            return consumedServiceDTOs;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("consumedservice/count/{start}")]
        public List<DateAndCount> GetConsumedServiceCount(DateTime start)
        {
            List<int> counts;
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                counts = consumedServiceDAL.GetConsumedServiceCount(start.Date, start.AddDays(6).Date, this.Identity.SalonId, null);
            else
                counts = consumedServiceDAL.GetConsumedServiceCount(start.Date, start.AddDays(6).Date, null, this.Identity.UserId);

            List<DateAndCount> list = new List<DateAndCount>();
            for (int i = 0; i < 7; i++)
            {
                list.Add(new DateAndCount()
                {
                    Date = start.Date.AddDays(i),
                    Count = counts[i]
                }
                );
            }
            return list;
        }
        #endregion

        #region validations
        private void ValidateChangeConsumedService(ChangeConsumedService_P dto)
        {
            if (ConsumedServiceEntity.OperatorId != this.Identity.UserId)
            {
                this.IsIllegalParameter = true;
                return;
            }

            if (dto.AppointmentId.HasValue)
            {
                if (AppointmentEntity.UserId != ConsumedServiceEntity.UserId)
                {
                    this.IsIllegalParameter = true;
                    return;
                }
            }

            this.ValidatorContainer.SetValue("消费次数", dto.Time)
                .InRange((byte)1, (byte)100, null);

            if (dto.Time > 1)
            {
                this.ValidatorContainer.SetValue("修改原因", dto.ChangeTimeReason)
                    .IsRequired();
            }

            this.ValidatorContainer.SetValue("修改原因", dto.ChangeTimeReason)
                .Length(null, 200);

            PurchasedServiceEntities = ConsumedServiceEntity.User.PurchasedServices.Where(ps => ps.ServiceSnapShot.ServiceId == dto.ServiceId).ToList();
            ValidateConsumedTimes(PurchasedServiceEntities, dto.Time, dto.ConsumedServiceId);
        }

        private void ValidDeleteConsumedService(int id)
        {
            if (ConsumedServiceEntity.OperatorId != this.Identity.UserId)
                this.IsIllegalParameter = true;
        }

        private void ValidChangeConsumedServiceStatus(ChangeConsumedServiceStatus_P dto)
        {
            if (dto.ConsumedServiceStatusId != ConsumedServiceStatusEnum.Confirmed && dto.ConsumedServiceStatusId != ConsumedServiceStatusEnum.Rejected)
                this.IsIllegalParameter = true;
        }

        private void ValidateCheckAnonym(CheckAnonymDTO_P dto)
        {
            this.ValidatorContainer.SetValue("实付金额", dto.Payment)
            .InRange(1m, 9999999.99m, null)
            .DecimalLength(2);

            this.ValidatorContainer.SetValue("服务次数", dto.Time)
                .InRange((byte)1, (byte)100, null);

            if (dto.Time > 1)
            {
                this.ValidatorContainer.SetValue("修改原因", dto.ChangeTimeReason)
                    .IsRequired();
            }

            this.ValidatorContainer.SetValue("修改原因", dto.ChangeTimeReason)
                .Length(null, 200);
            
        }

        private void ValidateAOCharge(CheckAOManualDTO_P dto)
        {
            /* 
             * 发起预约的用户是dto里的UserId
             * Time必须大于0
             * 已购服务剩余次数大于等于Time
             */
            if (dto.AppointmentId.HasValue)
            {
                if (PurchasedServiceEntity.UserId != AppointmentEntity.UserId)
                {
                    //该会员无此预约
                    this.IsIllegalParameter = true;
                    //this.InvalidMessages.Add("该会员无此预约");
                    return;
                }
            }

            if (dto.Time <= 0 || dto.Time > 100)
            {
                this.InvalidMessages.Add("消费次数必须在1到100之间");
                return;
            }

            if (dto.Time > 1 && string.IsNullOrWhiteSpace(dto.ChangeTimeReason))
            {
                this.InvalidMessages.Add("您已修改消费次数，请输入修改原因");
                return;
            }

            if (dto.ChangeTimeReason != null && dto.ChangeTimeReason.Length > 200)
            {
                this.InvalidMessages.Add("修改原因不能超过200个字符");
                return;
            }

            ValidateConsumedTimes(PurchasedServiceEntity, dto.Time, 0);
        }

        private void ValidateAOScan(CheckAOScanDTO_P dto)
        {
            /*
             * 发起预约的用户是此消费二维码的用户
             * 消费二维码存在，并且在有效期             
             * Time必须大于0
             * 用户已购买的此服务，且总的剩余次数大于等于Time             
             */

            UserEntity = userDAL.GetUserByConsumeCode(dto.ConsumeCode, this.Identity.SalonId, true, true, false, false);
            if (UserEntity == null)
            {
                this.InvalidMessages.Add("无效消费码");
                return;
            }
            else if (!UserEntity.ConsumeCodeExpiredDate.HasValue || UserEntity.ConsumeCodeExpiredDate < DateTime.Now)
            {
                this.InvalidMessages.Add("消费码已过期，请您的会员刷新消费码并重新扫码");
                return;
            }

            if (dto.AppointmentId.HasValue)
            {
                if (UserEntity.UserId != AppointmentEntity.UserId)
                {
                    this.InvalidMessages.Add("该会员无此预约，请扫描正确的会员消费码");
                    return;
                }
            }

            if (dto.Time <= 0 || dto.Time > 100)
            {
                this.InvalidMessages.Add("消费次数必须在1到100之间");
                return;
            }

            if (dto.Time > 1 && string.IsNullOrWhiteSpace(dto.ChangeTimeReason))
            {
                this.InvalidMessages.Add("您已修改消费次数，请输入修改原因");
                return;
            }

            if (dto.ChangeTimeReason != null && dto.ChangeTimeReason.Length > 200)
            {
                this.InvalidMessages.Add("修改原因不能超过200个字符");
                return;
            }

            PurchasedServiceEntities = UserEntity.PurchasedServices.Where(ps =>
                ps.ServiceSnapShot.ServiceId == dto.ServiceId).ToList();

            ValidateConsumedTimes(PurchasedServiceEntities, dto.Time, 0);
        }

        private void ValidateBeauticianScan(CheckBeauticianScanDTO_P dto)
        {
            /*
             * 验证逻辑：
             * 存在的预约，并且预约状态是Confirmed
             * 接受预约的美容师是当前登录用户
             * 消费二维码存在，并且在有效期
             * 发起预约的用户是此消费二维码的用户
             * 用户已购买预约对应的服务，并且有剩余次数             
             */
            AppointmentEntity = appointmentDAL.GetAppointment(dto.AppointmentId, UserTypeEnum.Beautician, this.Identity.UserId, true, false, true, false, true, false);
            if (AppointmentEntity == null)
            {
                //不存在的预约id
                this.IsIllegalParameter = true;
                return;
            }

            AppointmentFlow af = AppointmentEntity.AppointmentFlows.OrderByDescending(a => a.AppointmentFlowId).First();
            if (af.AppointmentStatusId != (int)AppointmentStatusEnum.Confirmed)
            {
                //只能扫码预约成功的预约
                this.IsIllegalParameter = true;
                return;
            }

            UserEntity = userDAL.GetUserByConsumeCode(dto.ConsumeCode, this.Identity.SalonId, true, true,false,false);
            if (UserEntity == null)
            {
                this.InvalidMessages.Add("无效消费码");
                return;
            }
            else if (!UserEntity.ConsumeCodeExpiredDate.HasValue || UserEntity.ConsumeCodeExpiredDate < DateTime.Now)
            {
                this.InvalidMessages.Add("消费码已过期，请您的会员刷新消费码并重新扫码");
                return;
            }

            if (UserEntity.UserId != AppointmentEntity.UserId)
            {
                this.InvalidMessages.Add("该会员无此预约，请扫描正确的会员消费码");
                return;
            }

            List<PurchasedService> psList = UserEntity.PurchasedServices.Where(ps =>
                ps.ServiceSnapShot.ServiceId == AppointmentEntity.ServiceSnapShot.ServiceId).ToList();

            ValidateConsumedTimes(psList, 1, 0);
        }

        private void ValidateConsumedTimes(PurchasedService ps, int times, int consumedServiceId)
        {
            List<PurchasedService> psList = new List<PurchasedService>();
            psList.Add(ps);
            ValidateConsumedTimes(psList, times, consumedServiceId);
        }

        private void ValidateConsumedTimes(List<PurchasedService> psList, int times, int consumedServiceId)
        {
            if (psList.Count == 0)
            {
                this.InvalidMessages.Add("该会员尚未购买此服务");
                return;
            }

            PurchasedServiceEntities = psList.Where(ps =>
                ps.Time.HasValue == false
                || ps.Time - ps.ConsumedServiceDetails.Where(csd => csd.ConsumedServiceId != consumedServiceId
                    &&
                    (csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Completed
                    ||
                    csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Confirmed)
                    ).Sum(csd => csd.Time) > 0
                ).OrderBy(ps => ps.PurchasedServiceId).ToList();

            if (PurchasedServiceEntities.Where(ps => ps.Time.HasValue == false).Count() == 0)
            {
                if (PurchasedServiceEntities.Sum(ps => ps.Time - ps.ConsumedServiceDetails.Where(csd => csd.ConsumedServiceId != consumedServiceId
                    &&
                    (csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Completed
                    ||
                    csd.ConsumedService.ConsumedServiceStatusId == (int)ConsumedServiceStatusEnum.Confirmed))
                    .Sum(cs => cs.Time)) < times)
                {
                    this.InvalidMessages.Add("该会员购买的此服务剩余次数不足" + times + "次");
                    return;
                }
            }


            PurchasedServiceEntities = psList.Where(ps =>
                ps.Time.HasValue == false
                || ps.Time - ps.ConsumedServiceDetails.Where(csd => csd.ConsumedServiceId != consumedServiceId).Sum(csd => csd.Time) > 0
                ).OrderBy(ps => ps.PurchasedServiceId).ToList();

            if (PurchasedServiceEntities.Where(ps => ps.Time.HasValue == false).Count() == 0)
            {
                if (PurchasedServiceEntities.Sum(ps => ps.Time - ps.ConsumedServiceDetails.Where(csd => csd.ConsumedServiceId != consumedServiceId).Sum(cs => cs.Time)) < times)
                {
                    this.InvalidMessages.Add("该会员有未处理的消费单，请先处理完毕");
                    return;
                }
            }
        }
        #endregion

        private void AddConsumedServiceDetails(ConsumedService cs, byte time)
        {
            //如果某个已购服务的余额不足，就会创建多条ConsumedServiceDetail，某个已购服务余额不足时，从下一个已购服务扣除
            //优先从较早的购买的服务里扣除
            PurchasedServiceEntities.ForEach(ps =>
            {
                if (time > 0)
                {
                    byte remainTime;
                    byte consumedTime;
                    if (ps.Time.HasValue)
                    {
                        remainTime = (byte)(ps.Time - ps.ConsumedServiceDetails.Sum(csd1 => csd1.Time));
                        consumedTime = remainTime - time < 0 ? remainTime : time;
                    }
                    else//无次数限制的服务
                        consumedTime = time;

                    ConsumedServiceDetail csd = new ConsumedServiceDetail()
                    {
                        Time = consumedTime,
                        PurchasedService = ps
                    };
                    cs.ConsumedServiceDetails.Add(csd);

                    time = (byte)(time - consumedTime);
                }
            });
        }

        
    }
    
}