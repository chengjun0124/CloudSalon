using CloudSalon.Common;
using CloudSalon.DAL;
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
    public class PasswordController : BaseApiController
    {
        [Inject]
        public EmployeeDAL eeDAL { get; set; }

        [HttpPut]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.Beautician, UserTypeEnum.SalonOwner)]
        public void UpdateEEPassword(PasswordDTO dto)
        {
            var ee = eeDAL.Get(this.Identity.UserId);

            ee.Password = EncodingHelper.MD5(dto.NewPassword);

            eeDAL.Update(ee);
        }
    }
}
