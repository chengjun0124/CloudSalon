using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace CloudSalon.Model.DTO
{

    public class AuthDTO : BaseDTO
    {
        public string Jwt;
        public string SalonName;
        public string SmallPicture;
    }
}
