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

        [HttpPost]
        [ApiAuthorize(UserTypeEnum.Beautician)]
        public int CreateUnavaiAppointment(UnavaiAppointmentDTO dto)
        {
            var entity = Mapper.Map<UnavaiAppointment>(dto);

            entity.EmployeeId = this.Identity.UserId;
            entity.UnavaiDate = DateTime.Now;

            eeDAL.CreateUnavaiAppointment(entity);
            return entity.UnavaiId;
        }


        [HttpDelete]
        [ApiAuthorize(UserTypeEnum.Beautician)]
        public void DeleteUnavaiAppointment(int id)
        {
            var entity = eeDAL.GetUnavaiAppointment(id, this.Identity.UserId);
            eeDAL.DeleteUnavaiAppointment(entity);

        }

        [HttpGet]
        public AvaiAppointmentDTO GetAppointment(int id)
        {
            var salon = salonDAL.Get(this.Identity.SalonId);
            var service = serviceDAL.GetService(id, this.Identity.SalonId);
            var employees = eeDAL.GetEmployees(this.Identity.SalonId);
            DateTime firstDate = DateTime.Now.Date;
            DateTime theDate;
            TimeSpan time = salon.OpenTime;
            TimeSpan interval = TimeSpan.FromMinutes(Constant.TIME_INTERVAL);
            TimeSpan serviceEndTime;
            TimeSpan comeBackTime;

            AvaiAppointmentDTO avaiAppointmentDTO = new AvaiAppointmentDTO()
            {
                OpenTime = salon.OpenTime,
                CloseTime = salon.CloseTime,
                Interval = Constant.TIME_INTERVAL
            };

            employees.ForEach(ee =>
            {
                var avaiEmployee = new AvaiEmployee() 
                {
                    EmployeeId=ee.EmployeeId,
                    Pictute = ee.Pictute
                };
                avaiAppointmentDTO.Employees.Add(avaiEmployee);
                
                
                for (int i = 0; i < 6; i++)
                {
                    theDate = firstDate.AddDays(i);

                    var unAvaiDate =new UnAvaiDate() 
                    {
                        Date = theDate,
                    };

                    avaiEmployee.UnAvaiDates.Add(unAvaiDate);

                    //case 1: 不营业中的日期，统统不能预约
                    if (salon.SalonCloses.Where(sc => theDate >= sc.StartDate && theDate <= sc.EndDate).Count() > 0)
                        unAvaiDate.IsSalonClose = true;

                    //case 2: 休息的技师，不能预约
                    bool isEEDayoff = false;
                    switch (theDate.DayOfWeek)
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
                    if (isEEDayoff)
                        unAvaiDate.IsDayoff = true;

                    if (unAvaiDate.IsSalonClose || unAvaiDate.IsDayoff)
                        continue;

                    //3. 翻牌的技师选择翻牌时间   xx:00 至 xx:00 当天有效， 可随便翻回
                    var unavaiAppointment = ee.UnavaiAppointments.Where(un => un.UnavaiDate == theDate).FirstOrDefault();
                    if (unavaiAppointment != null)
                    {
                        while (time < salon.CloseTime)
                        {
                            serviceEndTime = time.Add(TimeSpan.FromMinutes(service.Duration + Constant.SERVICE_BUFFER_MINUTES));
                            comeBackTime = unavaiAppointment.EndTime.Add(TimeSpan.FromMinutes(Constant.SERVICE_BUFFER_MINUTES));
                            if (serviceEndTime > unavaiAppointment.StartTime && time < comeBackTime)
                                unAvaiDate.UnAvaiTimes.Add(time);

                            time = time.Add(interval);
                        }
                    }



                    //4. 被预约的技师不能预约
                    
                }

                
            });
            return avaiAppointmentDTO;
        }


    }

    public class AvaiAppointmentDTO
    {
        public AvaiAppointmentDTO()
        {
            this.Employees = new List<AvaiEmployee>();
        }

        public TimeSpan OpenTime;
        public TimeSpan CloseTime;
        public int Interval;

        public List<AvaiEmployee> Employees;
    }

    public class AvaiEmployee
    {
        public AvaiEmployee()
        {
            this.UnAvaiDates = new List<UnAvaiDate>();
        }
        public int EmployeeId;
        public string Pictute;
        public List<UnAvaiDate> UnAvaiDates;

        
    }

    public class UnAvaiDate
    {
        public UnAvaiDate()
        {
            this.UnAvaiTimes = new List<TimeSpan>();
        }
        public DateTime Date;
        public List<TimeSpan> UnAvaiTimes;
        public bool IsDayoff;
        public bool IsSalonClose;
    }
}