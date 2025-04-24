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
    public class UnavaiTimeController : BaseApiController
    {
        [Inject]
        public EmployeeDAL eeDAL { get; set; }

        UnavaiTime unavaiTime = null;

        #region APIs
        [HttpPost]
        [ApiAuthorize(UserTypeEnum.Beautician)]
        public int CreateUnavaiTime(UnavaiTimeDTO dto)
        {
            base.Validator<UnavaiTimeDTO>(ValidateCreateUnavaiTime);

            var entity = Mapper.Map<UnavaiTime>(dto);

            entity.EmployeeId = this.Identity.UserId;
            entity.UnavaiDate = DateTime.Now;

            eeDAL.CreateUnavaiAppointment(entity);
            return entity.UnavaiId;
        }

        [HttpDelete]
        [ApiAuthorize(UserTypeEnum.Beautician)]
        public void DeleteUnavaiTime(int id)
        {
            base.Validator<int>(ValidateDeleteUnavaiTime);

            eeDAL.DeleteUnavaiTime(unavaiTime);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.Beautician)]
        public List<UnavaiTimeDTO> GetUnavaiTimes()
        {
            var entity = eeDAL.GetUnavaiTimes(this.Identity.UserId);
            List<UnavaiTimeDTO> list = new List<UnavaiTimeDTO>();
            entity.ForEach(u => list.Add(Mapper.Map<UnavaiTimeDTO>(u)));

            return list;
        }
        #endregion

        #region Validations
        [NonAction]
        public void ValidateCreateUnavaiTime(UnavaiTimeDTO dto)
        {
            this.ValidatorContainer.SetValue("开始时间", dto.StartTime)
                .InRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:30"), @"hh\:mm")
                .IsInScale(30)
                .Compare("结束时间", dto.EndTime, CompareOperation.Less);
            

            this.ValidatorContainer.SetValue("结束时间", dto.EndTime)
                .InRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:30"), @"hh\:mm")
                .IsInScale(30);
        }

        [NonAction]
        public void ValidateDeleteUnavaiTime(int id) 
        {
            unavaiTime = eeDAL.GetUnavaiTime(id, this.Identity.UserId);
            if (unavaiTime == null)
                this.IsIllegalParameter = true;
        }
        #endregion
    }
}
