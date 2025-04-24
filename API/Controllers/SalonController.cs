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
    public class SalonController : BaseApiController
    {
        [Inject]
        public SalonDAL salonDAL { get; set; }


        Salon salonEntity;

        #region APIs
        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public SalonDTO GetSalon()
        {
            var salon = salonDAL.Get(this.Identity.SalonId, true, true);
            return Mapper.Map<SalonDTO>(salon);
        }

        //[HttpGet]
        //[ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner,UserTypeEnum.Beautician)]
        //[Route("salon/identitycode")]
        //public string GetSalonIdentityCode()
        //{
        //    return salonDAL.GetSalonIdentityCodeBySalonId(this.Identity.SalonId).IdentityCode;
        //}

        [HttpGet]
        [Route("salon/{identityCode}")]
        public SalonDTO GetSalon(string identityCode)
        {
            base.Validator<string>(ValidGetSalon);

            return Mapper.Map<SalonDTO>(salonEntity);
        }


        [HttpPut]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void UpdateSalon(SalonDTO dto)
        {
            base.Validator<SalonDTO>(ValidateUpdateSalon);

            var salon = salonDAL.Get(this.Identity.SalonId, false, false);

            salon.OpenTime = dto.OpenTime;
            salon.CloseTime = dto.CloseTime;
            salon.SalonName = dto.SalonName;
            salon.SalonAddress = dto.SalonAddress;
            salon.Phone = dto.Phone;
            salon.Contact = dto.Contact;
            salon.Description = dto.Description;

            string toBeDeleted = null;
            string toBeDeletedQRCodePicture = null;
            if (dto.Picture != null)
            {
                if (salon.Picture != null)
                    toBeDeleted = Constant.SALON_IMAGE_FOLDER_Absolute + salon.Picture;
                salon.Picture = FileHelper.SaveImageWithThumbnail(Constant.SALON_IMAGE_FOLDER_Absolute, dto.Picture, Constant.SALON_IMAGE_WIDTH_THUMBNAIL, Constant.SALON_IMAGE_HEIGHT_THUMBNAIL);
            }

            if (dto.QrCodePicture != null)
            {
                if (salon.QRCodePicture != null)
                    toBeDeletedQRCodePicture = Constant.SALON_QRCODEIMAGE_FOLDER_Absolute + salon.QRCodePicture;
                salon.QRCodePicture = FileHelper.SaveImage(Constant.SALON_QRCODEIMAGE_FOLDER_Absolute, dto.QrCodePicture);
            }

            salonDAL.Update(salon);

            if (toBeDeleted != null)
                FileHelper.DeleteImage(toBeDeleted, true);
            if (toBeDeletedQRCodePicture != null)
                FileHelper.DeleteImage(toBeDeletedQRCodePicture, false);
        }

        [HttpPost]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public int CreateSalonClose(SalonCloseDTO dto)
        {
            var salonClose = new SalonClose() 
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                SalonId = this.Identity.SalonId
            };

            salonDAL.CreateSalonClose(salonClose);
            return salonClose.Id;
        }

        [HttpDelete]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void DeleteSalonClose(int id)
        {
            var salonClose = salonDAL.GetSalonClose(id, this.Identity.SalonId);

            salonDAL.DeleteSalonClose(salonClose);
        }
        #endregion


        [NonAction]
        public void ValidateUpdateSalon(SalonDTO dto)
        {
            this.ValidatorContainer.SetValue(dto.Picture)
                .IsImage(Constant.SALON_IMAGE_WIDTH, Constant.SALON_IMAGE_HEIGHT);

            this.ValidatorContainer.SetValue("二维码",dto.QrCodePicture)
                .IsImage(Constant.SALON_QRCODEIMAGE_WIDTH, Constant.SALON_QRCODEIMAGE_HEIGHT);

            this.ValidatorContainer.SetValue("店铺名称", dto.SalonName).IsRequired().Length(null, 10);
            this.ValidatorContainer.SetValue("联系人", dto.Contact).IsRequired().Length(null, 6);
            this.ValidatorContainer.SetValue("联系电话", dto.Phone).IsRequired().Length(null, 12).IsPhoneNumer();
            this.ValidatorContainer.SetValue("店铺地址", dto.SalonAddress).IsRequired().Length(null, 50);
            this.ValidatorContainer.SetValue("开门时间", dto.OpenTime)
                .IsInScale(30)
                .InRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:30"), @"hh\:mm");

            this.ValidatorContainer.SetValue("关门时间", dto.CloseTime)
                .IsInScale(30)
                .InRange(TimeSpan.Parse("00:00"), TimeSpan.Parse("23:30"), @"hh\:mm")
                .Compare("开门时间", dto.OpenTime, CompareOperation.Greater);

            this.ValidatorContainer.SetValue("店铺介绍", dto.Description)
                .IsRequired()
                .Length(null, 200);
        }

        [NonAction]
        public void ValidGetSalon(string identityCode)
        {
            salonEntity = salonDAL.GetSalonByIdentityCode(identityCode);
            if (salonEntity == null)
                this.IsIllegalParameter = true;
        }
    }
}
