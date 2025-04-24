using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CloudSalon.Model.DTO;
using Newtonsoft.Json;
using CloudSalon.Common;
using System.Security.Principal;
using CloudSalon.DAL;
using CloudSalon.Model;
using Ninject;
using CloudSalon.Model.Enum;
using System.Text.RegularExpressions;
using System.Web;

namespace CloudSalon.API
{
    public class AuthController : BaseApiController
    {
        [Inject]
        public EmployeeDAL eeDAL { get; set; }
        [Inject]
        public SalonDAL salonDAL { get; set; }
        [Inject]
        public UserDAL userDAL { get; set; }

        Salon salonEntity = null;
        User userEntity = null;
        List<Employee> employeeEntities = null;

        #region APIs
        [HttpGet]
        [Route("auth/ee/{mobile}/{password}")]
        public List<AuthDTO> SalonEEAuth(string mobile, string password)
        {
            base.Validator<string, string>(ValidSalonEEAuth);

            List<AuthDTO> dto = new List<AuthDTO>();
            foreach (Employee employeeEntity in employeeEntities)
            {
                int hours = 0;
                if (employeeEntity.UserTypeId == (int)UserTypeEnum.Beautician)
                    hours = 12;
                else if (employeeEntity.UserTypeId == (int)UserTypeEnum.SalonAdmin || employeeEntity.UserTypeId == (int)UserTypeEnum.SalonOwner)
                    hours = 4;

                JWT jwt = new JWT()
                {
                    UserId = employeeEntity.EmployeeId,
                    UserTypeId = employeeEntity.UserTypeId,
                    SalonId = employeeEntity.SalonId,
                    IsBeautician = employeeEntity.IsBeautician,
                    IdentityCode = employeeEntity.Salon.IdentityCode,
                    Expire = DateTime.Now.AddHours(hours).Ticks
                };

                string claim = JsonConvert.SerializeObject(jwt);
                claim = EncodingHelper.EncodeBase64(claim);
                dto.Add(new AuthDTO()
                {
                    Jwt = claim + "." + EncodingHelper.HMACMD5(claim, Constant.JWTKey),
                    SalonName = employeeEntity.Salon.SalonName,
                    SmallPicture = employeeEntity.Salon.Picture == null ? null : Constant.SALON_IMAGE_FOLDER_URL + "s" + employeeEntity.Salon.Picture
                });
            }
            return dto;
        }

        [HttpGet]
        [Route("auth/validcode/{mobile}/{identityCode}")]
        public void SendValidCode(string mobile, string identityCode)
        {
            base.Validator<string, string>(ValidSendValidCode);

            if (userEntity == null)
            {
                userEntity = new CloudSalon.Model.User()
                {
                    Mobile = mobile,
                    CreatedDate = DateTime.Now
                };
                salonEntity.Users.Add(userEntity);
            }

            string validCode = new Random().Next(10000).ToString("D4");
            Message.SendMoblieMessage(mobile, Constant.USER_LOGIN_VALIDCODE_TEMPLATE_ID, new string[] { validCode, Constant.USER_LOGIN_VALIDCODE_EXPIRATION.ToString(), salonEntity.SalonName }, HttpContext.Current.IsDebuggingEnabled);
            userEntity.LoginValidCodes.Add(new LoginValidCode()
            {
                Code = validCode,
                CreatedDate = DateTime.Now,
                IP = System.Web.HttpContext.Current.Request.UserHostAddress
            });

            salonDAL.Update(salonEntity);
        }

        [HttpGet]
        [Route("auth/user/{mobile}/{code}/{identityCode}")]
        public string UserAuth(string mobile, string code, string identityCode)
        {
            base.Validator<string, string, string>(ValidUserAuth);

            userEntity.IsActive = true;
            userDAL.Update(userEntity);

            JWT jwt = new JWT()
            {
                UserId = userEntity.UserId,
                UserTypeId = (int)UserTypeEnum.User,
                SalonId = salonEntity.SalonId,
                Expire = DateTime.Now.AddMonths(2).Ticks
            };

            string claim = JsonConvert.SerializeObject(jwt);
            claim = EncodingHelper.EncodeBase64(claim);
            return claim + "." + EncodingHelper.HMACMD5(claim, Constant.JWTKey);
        }
        #endregion


        #region Validations
        [NonAction]
        public void ValidSendValidCode(string mobile, string identityCode)
        {
            //生成规则:
            //每分钟一条
            //一个号码一天最多5条
            //如何屏蔽恶意发送手机验证码
            //发短信接口有约束，详见http://www.yuntongxun.com/doc/rest/sms/3_2_2_1.html

            salonEntity = salonDAL.GetSalonByIdentityCode(identityCode);
            if (salonEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }

            /*网上查到的验证手机，号码范围以及正则表达式，目前只验证11位数字
             * 移动号码段:139、138、137、136、135、134、150、151、152、157、158、159、182、183、187、188、147
             * 联通号码段:130、131、132、136、185、186、145
             * 电信号码段:133、153、180、189
             * string regex = "^((13[0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\\d{8}$";
             */


            if (!Regex.IsMatch(mobile, @"^\d{11}$"))
            {
                this.InvalidMessages.Add("请输入正确的手机号");
                return;
            }

            userEntity = userDAL.GetUser(mobile, salonEntity.SalonId, true);
            if (userEntity != null)
            {
                if (userEntity.LoginValidCodes.Count > 0)
                {
                    DateTime lastOne = userEntity.LoginValidCodes.OrderByDescending(c => c.CreatedDate).First().CreatedDate;
                    if (lastOne.AddMinutes(1) > DateTime.Now)
                    {
                        this.InvalidMessages.Add("每分钟只能发送一条验证码，请您稍后再试");
                        return;
                    }

                    int count = userEntity.LoginValidCodes.Where(l => l.CreatedDate >= DateTime.Now.Date).Count();
                    if (count >= 5)
                    {
                        this.InvalidMessages.Add("一天最多发送5条验证码，请使用您最后收到的验证码");
                        return;
                    }
                }
            }
        }
        
        [NonAction]
        public void ValidUserAuth(string mobile, string code, string identityCode)
        {
            /*
            1. 找不到identityCode，“无效请求”
            2. 找不到手机号码，“未注册的手机号”
            3. 2小时内生成的codes中没有code,"无效验证码"
            4. code不是最后一条LoginValidCode，“验证码已失效”
            5. 验证码过期15分钟，“验证码已过期”
            */

            salonEntity = salonDAL.GetSalonByIdentityCode(identityCode);
            if (salonEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }

            userEntity = userDAL.GetUser(mobile, salonEntity.SalonId, true);
            if (userEntity == null)
            {
                this.InvalidMessages.Add("请先点击获取验证码");
                return;
            }


            List<LoginValidCode> codes = userEntity.LoginValidCodes.Where(l => l.CreatedDate >= DateTime.Now.AddHours(-2)).OrderByDescending(l => l.CreatedDate).ToList();
            if (codes.Where(l => l.Code == code).Count() == 0)
            {
                this.InvalidMessages.Add("无效验证码");
                return;
            }

            if (codes[0].Code != code)
            {
                this.InvalidMessages.Add("验证码已失效");
                return;
            }

            if (codes[0].CreatedDate.AddMinutes(Constant.USER_LOGIN_VALIDCODE_EXPIRATION) < DateTime.Now)
            {
                this.InvalidMessages.Add("验证码已过期");
                return;
            }
        }

        [NonAction]
        public void ValidSalonEEAuth(string mobile, string password)
        {
            employeeEntities = eeDAL.AuthEmployee(mobile, EncodingHelper.MD5(password), true);
            if (employeeEntities.Count == 0)
            {
                this.InvalidMessages.Add("手机号或密码错误，请重新输入");
                return;
            }
        }
        #endregion
    }
}