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
    public class AppointmentController : BaseApiController
    {
        [Inject]
        public EmployeeDAL eeDAL { get; set; }
        [Inject]
        public SalonDAL salonDAL { get; set; }
        [Inject]
        public ServiceDAL serviceDAL { get; set; }
        [Inject]
        public AppointmentDAL appointmentDAL { get; set; }
        [Inject]
        public UserDAL userDAL { get; set; }
        [Inject]
        public PurchasedServiceDAL purchasedServiceDAL { get; set; }

        Appointment appointmentEntity = null;
        Salon salonEntity = null;
        Service serviceEntity = null;
        AppointmentFlow appointmentFlowEntity = null;
        Employee employeeEntity = null;
        User userEntity = null;

        #region APIs
        [HttpPost]
        [ApiAuthorize(UserTypeEnum.User)]
        public void CreateAppointment(AppointmentDTO dto)
        {
            base.Validator<AppointmentDTO>(ValidateCreateAppointment);

            var entity = Mapper.Map<Appointment>(dto);

            entity.UserId = this.Identity.UserId;
            entity.ServiceSnapShotId = serviceEntity.ServiceSnapShots.OrderByDescending(sss => sss.ServiceSnapShotId).First().ServiceSnapShotId;

            entity.AppointmentFlows.Add(new AppointmentFlow()
            {
                AppointmentStatusId = (int)AppointmentStatusEnum.Pending,
                CreatedDate = DateTime.Now
            });

            appointmentDAL.Insert(entity);

            if ((dto.AppointmentDate - DateTime.Now).TotalDays > 7)
                AppNotification.SendAppNotification(dto.EmployeeId.Value, "新预约", "新预约", "您有新预约，请及时处理", "新预约", "http://salonadmin.jiyunorg.com/#!/appointment/" + entity.AppointmentId, DateTime.Now.AddDays(7));
            else
                AppNotification.SendAppNotification(dto.EmployeeId.Value, "新预约", "新预约", "您有新预约，请及时处理", "新预约", "http://salonadmin.jiyunorg.com/#!/appointment/" + entity.AppointmentId, dto.AppointmentDate);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("appointment/avai/user/{userId}")]
        public List<AppointmentDTO> GetAvaiAppointments(int userId)
        {
            base.Validator<int>(ValidateGetAvaiAppointments);

            List<AppointmentDTO> list = new List<AppointmentDTO>();
            appointmentDAL.GetAppointmentsByUserId(userId, DateTime.Now.Date, null, AppointmentStatusEnum.Pending, AppointmentStatusEnum.Confirmed).ForEach(a =>
            {
                list.Add(Mapper.Map<AppointmentDTO>(a));
            });
            return list;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.User)]
        public AvaiAppointmentDTO GetAvaiAppointmentTimes(int id)
        {
            base.Validator<int>(ValidateGetAvaiAppointmentTimes);

            var employees = eeDAL.GetBeauticians(this.Identity.SalonId);
            if (employees.Count == 0)
                return null;

            DateTime startDate = DateTime.Now.Date;
            DateTime theDate = startDate;

            AvaiAppointmentDTO avaiAppointmentDTO = new AvaiAppointmentDTO();
            //{
            //    OpenTime = salonEntity.OpenTime,
            //    CloseTime = salonEntity.CloseTime,
            //    Interval = Constant.TIME_INTERVAL
            //};

            employees.ForEach(ee =>
            {
                var avaiEmployee = Mapper.Map<Beautician>(ee);

                theDate = startDate;
                for (int i = 0; i < 6; i++)
                {
                    theDate = startDate.AddDays(i);

                    var AvaiDate = new AvaiDate()
                    {
                        Date = theDate,
                        //case 1: 美容院不营业的日期，不能预约
                        IsSalonClose = IsSalonClose(salonEntity.SalonCloses, theDate),
                        //case 2: 技师休息，不能预约
                        IsDayoff = IsEmployeeDayoff(ee, theDate),
                    };

                    //3. 翻牌的技师选择翻牌时间   xx:00 至 xx:00 当天有效， 可随时翻回
                    //4. 被预约的技师不能预约
                    //5. 小于当前日期的不能预约
                    AvaiDate.AvaiTimes = GetAvaiTimes(salonEntity, theDate, serviceEntity.Duration, ee);

                    avaiEmployee.AvaiDates.Add(AvaiDate);
                }

                avaiAppointmentDTO.Beauticians.Add(avaiEmployee);
            });
            return avaiAppointmentDTO;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.User)]
        [Route("appointment/user/{date?}")]
        public List<AppointmentDTO> GetUserAppointments(DateTime? date = null)
        {
            List<Appointment> appointments =null;
            if (!date.HasValue)
                appointments = appointmentDAL.GetAppointmentsByUserId(this.Identity.UserId, DateTime.Now, null, AppointmentStatusEnum.Pending, AppointmentStatusEnum.Confirmed, AppointmentStatusEnum.Rejected);
            else
            {
                DateTime startDate = date.Value.Date;
                DateTime endDate = date.Value.AddMonths(1);

                startDate = startDate.AddDays(-(startDate.Day - 1));
                endDate = endDate.AddDays(-endDate.Day).AddDays(1);

                appointments = appointmentDAL.GetAppointmentsByUserId(this.Identity.UserId, startDate, endDate);
            }

            List<AppointmentDTO> appointmentDTOs = new List<AppointmentDTO>();
            appointments.ForEach(a =>
            {
                appointmentDTOs.Add(Mapper.Map<AppointmentDTO>(a));
            });

            return appointmentDTOs;
        }
        
        [HttpGet]
        [ApiAuthorize(UserTypeEnum.User, UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("appointment/detail/{appointmentId}")]
        public AppointmentDTO GetAppointment(int appointmentId)
        {
            base.Validator<int>(ValidateGetAppointment);

            return Mapper.Map<AppointmentDTO>(appointmentEntity);
        }

        [HttpPut]
        [ApiAuthorize(UserTypeEnum.User, UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void ChangeAppointStatus(AppointmentStatusDTO status)
        {
            base.Validator<AppointmentStatusDTO>(ValidateChangeAppointStatus);

            AppointmentFlow entify = new AppointmentFlow();
            entify.AppointmentId = status.AppointmentId;
            entify.AppointmentStatusId = (int)status.ApponintmentStatus;
            entify.CreatedDate = DateTime.Now;

            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                entify.EmployeeId = this.Identity.UserId;

            //部分状态需要发手机短信，短信接口未认证，无法添加短信模板

            appointmentDAL.CreateAppointFlow(entify);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("appointment/{pageNumber}/{pageSize}")]
        public List<AppointmentDTO> GetAppointments(int pageNumber, int pageSize)
        {
            List<Appointment> appointments = null;
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                appointments = appointmentDAL.GetAppointments(this.Identity.SalonId, null, null, pageNumber, pageSize, AppointmentStatusEnum.Pending, AppointmentStatusEnum.Confirmed);
            else
                appointments = appointmentDAL.GetAppointments(null, this.Identity.UserId, null, pageNumber, pageSize, AppointmentStatusEnum.Pending, AppointmentStatusEnum.Confirmed);

            List<AppointmentDTO> appointmentDTOs = new List<AppointmentDTO>();
            appointments.ForEach(a => 
            {
                appointmentDTOs.Add(Mapper.Map<AppointmentDTO>(a));
            });

            return appointmentDTOs;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("appointment/{date}/{pageNumber}/{pageSize}")]
        public List<AppointmentDTO> GetCompletedAppointments(DateTime date, int pageNumber, int pageSize)
        {
            List<Appointment> appointments = null;
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                appointments = appointmentDAL.GetAppointments(this.Identity.SalonId, null, date.Date, pageNumber, pageSize, AppointmentStatusEnum.Rejected, AppointmentStatusEnum.Completed, AppointmentStatusEnum.UserCanceled, AppointmentStatusEnum.EmployeeCanceled);
            else
                appointments = appointmentDAL.GetAppointments(null, this.Identity.UserId, date.Date, pageNumber, pageSize, AppointmentStatusEnum.Rejected, AppointmentStatusEnum.Completed, AppointmentStatusEnum.UserCanceled, AppointmentStatusEnum.EmployeeCanceled);

            List<AppointmentDTO> appointmentDTOs = new List<AppointmentDTO>();
            appointments.ForEach(a =>
            {
                appointmentDTOs.Add(Mapper.Map<AppointmentDTO>(a));
            });

            return appointmentDTOs;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("appointment/count/{start}")]
        public List<DateAndCount> GetCompletedAppointmentCount(DateTime start)
        {
            List<int> counts;
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                counts = appointmentDAL.GetAppointmentCount(start.Date, start.AddDays(6).Date, this.Identity.SalonId, null, AppointmentStatusEnum.Rejected, AppointmentStatusEnum.Completed, AppointmentStatusEnum.UserCanceled, AppointmentStatusEnum.EmployeeCanceled);
            else
                counts = appointmentDAL.GetAppointmentCount(start.Date, start.AddDays(6).Date, null, this.Identity.UserId, AppointmentStatusEnum.Rejected, AppointmentStatusEnum.Completed, AppointmentStatusEnum.UserCanceled, AppointmentStatusEnum.EmployeeCanceled);

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

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("appointment/todaycount")]
        public int GetTodayAppointmentCount()
        {
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner)
                return appointmentDAL.GetSalonAppointmentCountByDate(DateTime.Now, this.Identity.SalonId);
            else
                return appointmentDAL.GetEmployeeAppointmentCountByDate(DateTime.Now, this.Identity.UserId);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.Beautician)]
        [Route("appointment/recent")]
        public AppointmentDTO GetRecentAppointment()
        {
            Appointment a = appointmentDAL.GetRecentAppointment(this.Identity.UserId);
            return Mapper.Map<AppointmentDTO>(a);
 
        }
        #endregion

        #region validation methods
        [NonAction]
        public void ValidateGetAvaiAppointments(int userId)
        {
            userEntity = userDAL.GetUser(userId, this.Identity.SalonId, false, false);
            if (userEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }
        }

        [NonAction]
        public void ValidateChangeAppointStatus(AppointmentStatusDTO status)
        {
            if (this.Identity.UserType == UserTypeEnum.User || this.Identity.UserType == UserTypeEnum.Beautician)
                appointmentEntity = appointmentDAL.GetAppointment(status.AppointmentId, this.Identity.UserType, this.Identity.UserId, true, false, false, false, false, false);
            else
                appointmentEntity = appointmentDAL.GetAppointment(status.AppointmentId, this.Identity.UserType, this.Identity.SalonId, true, false, false, false, false, false);

            if (appointmentEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }


            appointmentFlowEntity = appointmentEntity.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).First();

            if (this.Identity.UserType == UserTypeEnum.User)
            {
                //用户只能修改Pending
                if (appointmentFlowEntity.AppointmentStatusId != (int)AppointmentStatusEnum.Pending)
                {
                    this.IsIllegalParameter = true;
                    return;
                }

                //用户只能把状态修改为UserCanceled
                if (status.ApponintmentStatus != AppointmentStatusEnum.UserCanceled)
                {
                    this.IsIllegalParameter = true;
                    return;
                }
            }
            else
            {
                if (appointmentFlowEntity.AppointmentStatusId == (int)AppointmentStatusEnum.Pending)
                {
                    //当前状态为用户预约，技师和美管可以改至2个状态Confirmed和Rejected
                    if (status.ApponintmentStatus != AppointmentStatusEnum.Confirmed && status.ApponintmentStatus != AppointmentStatusEnum.Rejected)
                    {
                        this.IsIllegalParameter = true;
                        return;
                    }
                }
                else if (appointmentFlowEntity.AppointmentStatusId == (int)AppointmentStatusEnum.Confirmed)
                {
                    //当前状态为技师确认，技师和美管可以改至EmployeeCanceled
                    if (status.ApponintmentStatus != AppointmentStatusEnum.EmployeeCanceled)
                    {
                        this.IsIllegalParameter = true;
                        return;
                    }
                }
                else
                {
                    this.IsIllegalParameter = true;
                    return;
                }
            }
            
        }

        [NonAction]
        public void ValidateCreateAppointment(AppointmentDTO dto)
        {
            if (!dto.EmployeeId.HasValue)
            {
                this.IsIllegalParameter = true;
                return;
            }

            serviceEntity = serviceDAL.GetService(dto.ServiceId, this.Identity.SalonId, false, false, false);
            if (serviceEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }

            employeeEntity = eeDAL.GetBeautician(dto.EmployeeId.Value, this.Identity.SalonId);
            if (employeeEntity == null)
            {
                this.InvalidMessages.Add("您选择的美容师已不存在，请返回首页重新选择");
                return;
            }

            salonEntity = salonDAL.Get(this.Identity.SalonId);
            //case 1: 美容院不营业的日期，不能预约            
            bool close = IsSalonClose(salonEntity.SalonCloses, dto.AppointmentDate);
            if (close)
            {
                this.InvalidMessages.Add("该日期美容院不营业，请返回首页重新选择");
                return;
            }

            //case 2: 技师休息，不能预约
            bool dayoff = IsEmployeeDayoff(employeeEntity, dto.AppointmentDate);
            if (dayoff)
            {
                this.InvalidMessages.Add("该日期美容师休息，请返回首页重新选择");
                return;
            }


            //3. 翻牌的技师选择翻牌时间   xx:00 至 xx:00 当天有效， 可随时翻回
            //4. 被预约的技师不能预约
            //5. 小于当前日期的不能预约
            List<AvaiTime> avaiTimes = GetAvaiTimes(salonEntity, dto.AppointmentDate.Date, serviceEntity.Duration, employeeEntity);
            if (!avaiTimes.Exists(a => a.IsAvailable == true && a.Time == dto.AppointmentDate.TimeOfDay))
            {
                this.InvalidMessages.Add("该时间不可预约，请返回首页重新选择");
                return;
            }
        }

        [NonAction]
        public void ValidateGetAppointment(int appointmentId)
        {
            int id;
            if (this.Identity.UserType == UserTypeEnum.User || this.Identity.UserType == UserTypeEnum.Beautician)
                id = this.Identity.UserId;
            else
                id = this.Identity.SalonId;
            appointmentEntity = appointmentDAL.GetAppointment(appointmentId, this.Identity.UserType, id, true, true, true, true, false, true);
            if (appointmentEntity == null)
                this.IsIllegalParameter = true;
        }

        [NonAction]
        public void ValidateGetAvaiAppointmentTimes(int id)
        {
            salonEntity = salonDAL.Get(this.Identity.SalonId);
            serviceEntity = serviceDAL.GetService(id, this.Identity.SalonId, false, false, false);
            if (serviceEntity == null)
                this.IsIllegalParameter = true;
        }
        #endregion

        private bool IsAvaiTime(TimeSpan appointmentTime, int serviceDuring, TimeSpan unavaiStart, TimeSpan unavaiEnd)
        {
            if (appointmentTime + TimeSpan.FromMinutes(serviceDuring) > unavaiStart - Constant.SERVICE_BUFFER
                &&
                appointmentTime < unavaiEnd + Constant.SERVICE_BUFFER)
                return false;

            return true;
        }

        private bool IsAvaiTime(DateTime appointmentDate, int serviceDuring, Employee ee)
        {
            if (appointmentDate < DateTime.Now)
                return false;
            var unavaiTime = ee.UnavaiTimes.Where(un => un.UnavaiDate == appointmentDate.Date).FirstOrDefault();
            if (unavaiTime != null)
            {
                if (!IsAvaiTime(appointmentDate.TimeOfDay, serviceDuring, unavaiTime.StartTime, unavaiTime.EndTime))
                    return false;
            }
            

            var appointments = ee.Appointments.Where(a =>
                        (
                            a.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).First().AppointmentStatusId == (int)AppointmentStatusEnum.Pending
                            || 
                            a.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).First().AppointmentStatusId == (int)AppointmentStatusEnum.Confirmed
                        )
                        &&
                        a.AppointmentDate.Date == appointmentDate.Date
                    ).ToList();

            foreach (var a in appointments)
            {
                if (!IsAvaiTime(appointmentDate.TimeOfDay, serviceDuring, a.AppointmentDate.TimeOfDay, a.AppointmentDate.TimeOfDay + TimeSpan.FromMinutes(a.ServiceSnapShot.Duration)))
                    return false;   
            }

            return true;
        }

        private List<AvaiTime> GetAvaiTimes(Salon salon, DateTime appointmentDate, int serviceDuration, Employee ee)
        {
            List<AvaiTime> AvaiTimes = new List<AvaiTime>();
            TimeSpan salonOpenTime = salon.OpenTime;
            TimeSpan salonCloseTime = salon.CloseTime;
            while (salonOpenTime < salonCloseTime)
            {
                //if (salonOpenTime > DateTime.Now.TimeOfDay)
                //{
                    AvaiTimes.Add(new AvaiTime()
                    {
                        Time = salonOpenTime,
                        IsAvailable = IsAvaiTime(appointmentDate + salonOpenTime, serviceDuration, ee)
                    });
                //}
                salonOpenTime = salonOpenTime + Constant.TIME_INTERVAL;
            }

            
            return AvaiTimes;
        }

        private bool IsSalonClose(ICollection<SalonClose> salonCloses, DateTime date)
        {
            date = date.Date;
            if (salonCloses.Where(sc => date >= sc.StartDate && date <= sc.EndDate).Count() > 0)
                return true;
            else
                return false;
        }

        private bool IsEmployeeDayoff(Employee ee, DateTime date)
        {
            bool isEEDayoff = false;
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (ee.IsDayoffMon)
                        isEEDayoff = true;
                    break;
                case DayOfWeek.Tuesday:
                    if (ee.IsDayoffTue)
                        isEEDayoff = true;
                    break;
                case DayOfWeek.Wednesday:
                    if (ee.IsDayoffWeb)
                        isEEDayoff = true;
                    break;
                case DayOfWeek.Thursday:
                    if (ee.IsDayoffThu)
                        isEEDayoff = true;
                    break;
                case DayOfWeek.Friday:
                    if (ee.IsDayoffFri)
                        isEEDayoff = true;
                    break;
                case DayOfWeek.Saturday:
                    if (ee.IsDayoffSat)
                        isEEDayoff = true;
                    break;
                case DayOfWeek.Sunday:
                    if (ee.IsDayoffSun)
                        isEEDayoff = true;
                    break;
            }
            return isEEDayoff;
        }
    }
    
}