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
    public class UserController : BaseApiController
    {
        [Inject]
        public UserDAL userDAL { get; set; }

        User userEntity = null;

        #region APIs
        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("user/{pageNumber}/{pageSize}/{keyword?}")]
        public List<UserDTO> GetUsers(int pageNumber, int pageSize, string keyword = null)
        {
            //System.Threading.Thread.Sleep(1000);
            var users = userDAL.GetUsers(this.Identity.SalonId, pageNumber, pageSize, keyword, true, true,true);

            List<UserDTO> userDTOs = new List<UserDTO>();
            users.ForEach(ee => userDTOs.Add(Mapper.Map<UserDTO>(ee)));

            return userDTOs;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.User, UserTypeEnum.SalonOwner)]
        public UserDTO GetUser(int? id = null)
        {
            base.Validator<int?>(ValidGetUser);

            return Mapper.Map<UserDTO>(userEntity);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.User)]
        [Route("user/consumecode")]
        public string GenerateConsumeCode()
        {
            userEntity = userDAL.Get(this.Identity.UserId);
            userEntity.ConsumeCode = Guid.NewGuid().ToString();
            userEntity.ConsumeCodeExpiredDate = DateTime.Now.AddMinutes(5);

            userDAL.Update(userEntity);

            return userEntity.ConsumeCode;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("user/consumecode/{consumeCode}")]
        public int GetUserId(string consumeCode)
        {
            base.Validator<string>(ValidateGetUserId);

            return userEntity.UserId;
        }

        [HttpPut]
        [ApiAuthorize(UserTypeEnum.User)]
        public void UpdateUser(UserDTO dto)
        {
            base.Validator<UserDTO>(ValidUpdateUser);

            var user = userDAL.GetUser(this.Identity.UserId, this.Identity.SalonId, false, false);

            user.NickName = dto.NickName;
            user.Name = dto.Name;

            string toBeDeletePicture = null;
            if (dto.Picture != null)
            {
                if (user.Picture != null)
                    toBeDeletePicture = Constant.USER_PORTRAIT_IMAGE_FOLDER_Absolute + user.Picture;
                user.Picture = FileHelper.SaveImage(Constant.USER_PORTRAIT_IMAGE_FOLDER_Absolute, dto.Picture);
            }

            userDAL.Update(user);

            if (toBeDeletePicture != null)
                FileHelper.DeleteImage(toBeDeletePicture, false);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.Beautician, UserTypeEnum.SalonOwner)]
        [Route("user/count")]
        public int GetUserCount()
        {
            return userDAL.GetUserCount(this.Identity.SalonId);
        }

        [HttpPut]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("user/memo")]
        public void UpdateMemo(UserDTO dto)
        {
            base.Validator<UserDTO>(ValidateUpdateMemo);

            userEntity.Memo = dto.Memo;
            userDAL.Update(userEntity);
        }

        [HttpPost]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public int CreateUser(UserDTO dto)
        {
            base.Validator<UserDTO>(ValidCreateUser);

            CloudSalon.Model.User user = new User()
            {
                Name = dto.Name,
                Mobile = dto.Mobile,
                Memo = dto.Memo,
                SalonId = this.Identity.SalonId,
                CreatedDate = DateTime.Now,
                IsActive = false
            };
            userDAL.Insert(user);
            return user.UserId;
        }
        #endregion

        #region validation methods
        [NonAction]
        public void ValidateGetUserId(string consumeCode)
        {
            userEntity = userDAL.GetUserByConsumeCode(consumeCode, this.Identity.SalonId, false, false, false, false);
            if (userEntity == null)
            {
                this.InvalidMessages.Add("无效消费码");
                return;
            }
            else if (!userEntity.ConsumeCodeExpiredDate.HasValue || userEntity.ConsumeCodeExpiredDate < DateTime.Now)
            {
                this.InvalidMessages.Add("消费码已过期，请您的会员刷新消费码并重新扫码");
                return;
            }
        }

        [NonAction]
        public void ValidateUpdateMemo(UserDTO dto)
        {
            userEntity = userDAL.GetUser(dto.UserId, this.Identity.SalonId, false, false);
            if (userEntity == null)
            {
                this.IsIllegalParameter = true;
                return;
            }

            this.ValidatorContainer.SetValue("备注", dto.Memo)
                .Length(null, 500);
        }

        [NonAction]
        public void ValidCreateUser(UserDTO dto)
        {
            this.ValidatorContainer.SetValue("姓名", dto.Name)
                .Length(null, 6);

            this.ValidatorContainer.SetValue("手机", dto.Mobile)
                .IsRequired()
                .IsMobile()
                .Custom(() => !userDAL.IsExistedUser(dto.Mobile, this.Identity.SalonId), "号码已存在");

            this.ValidatorContainer.SetValue("备注", dto.Memo)
                .Length(null, 500);
        }

        [NonAction]
        public void ValidGetUser(int? id)
        {
            if (this.Identity.UserType != UserTypeEnum.User)
            {
                if (!id.HasValue)
                {
                    this.IsIllegalParameter = true;
                    return;
                }
                userEntity = userDAL.GetUser(id.Value, this.Identity.SalonId, false, false);
            }
            else
                userEntity = userDAL.GetUser(this.Identity.UserId, this.Identity.SalonId, false, false);


            if (userEntity == null)
                this.IsIllegalParameter = true;
        }

        [NonAction]
        public void ValidUpdateUser(UserDTO dto)
        {
            this.ValidatorContainer.SetValue("昵称", dto.NickName)
                .Length(null, 15);
            
            this.ValidatorContainer.SetValue("姓名", dto.Name)
                .Length(null, 6);

            this.ValidatorContainer.SetValue(dto.Picture)
                .IsImage(Constant.USER_PORTRAIT_IMAGE_WIDTH, Constant.USER_PORTRAIT_IMAGE_HEIGHT);
        }
        #endregion
    }
}
