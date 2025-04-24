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
    public class ServiceSnapShotController : BaseApiController
    {
        [Inject]
        public ServiceSnapShotDAL serviceSnapShotDAL { get; set; }
        ServiceSnapShot serviceSnapShotEntity;

        #region APIs
        
        #endregion


    }
}
