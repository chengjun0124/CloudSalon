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
    public class TagController : BaseApiController
    {
        [Inject]
        public ServiceTypeTagDAL serviceTypeTagDAL { get; set; }

        [HttpGet]
        [Route("tag/servicetype/{serviceTypeId}")]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public List<string> GetServiceTypeTags(int serviceTypeId)
        {
            return serviceTypeTagDAL.GetServiceTypeTags(serviceTypeId).Select(t => t.TagName).ToList();
        }
    }
}
