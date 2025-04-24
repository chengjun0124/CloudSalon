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
    public class EmployeeController : BaseApiController
    {
        [Inject]
        public EmployeeDAL eeDAL { get; set; }
        [Inject]
        public SalonDAL salonDAL { get; set; }


        Employee employee = null;

        #region APIs
        [HttpPost]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public int CreateEmployee(EmployeeDTO dto)
        {
            base.Validator<EmployeeDTO>(ValidateCreateEmployee);

            Employee ee = Mapper.Map<Employee>(dto);
            ee.Password = EncodingHelper.MD5(dto.PassWord);
            ee.CreatedDate = DateTime.Now;
            ee.SalonId = this.Identity.SalonId;
            //美管只能新建美容师
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin)
                ee.UserTypeId = (int)UserTypeEnum.Beautician;

            //新建的员工是美容师，IsBeautician必须是NULL,不是美容师，IsBeautician必须有值,
            if (ee.UserTypeId == (int)UserTypeEnum.Beautician)
                ee.IsBeautician = null;
            else
                ee.IsBeautician = dto.IsBeautician.Value;

            if (dto.Picture != null)
            {
                ee.Picture = FileHelper.SaveImageWithThumbnail(Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_Absolute, dto.Picture, Constant.EMPLOYEE_PORTRAIT_IMAGE_WIDTH_THUMBNAIL, Constant.EMPLOYEE_PORTRAIT_IMAGE_HEIGHT_THUMBNAIL);
            }

            eeDAL.Insert(ee);
            return ee.EmployeeId;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("employee/{pageNumber}/{pageSize}")]
        public List<EmployeeDTO> GetEmployees(int pageNumber, int pageSize)
        {
            List<UserTypeEnum> userTypes = new List<UserTypeEnum>();
            userTypes.Add(UserTypeEnum.Beautician);

            if (this.Identity.UserType == UserTypeEnum.SalonOwner)
                userTypes.Add(UserTypeEnum.SalonAdmin);

            var ees = eeDAL.GetEmployees(this.Identity.SalonId, pageNumber, pageSize, true, userTypes);
            List<EmployeeDTO> eeDTOs = new List<EmployeeDTO>();
            ees.ForEach(ee => eeDTOs.Add(Mapper.Map<EmployeeDTO>(ee)));

            return eeDTOs;
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("employee/count")]
        public int GetEmployeeCount()
        {
            List<UserTypeEnum> userTypes = new List<UserTypeEnum>();
            userTypes.Add(UserTypeEnum.Beautician);

            if (this.Identity.UserType == UserTypeEnum.SalonOwner)
                userTypes.Add(UserTypeEnum.SalonAdmin);

            return eeDAL.GetEmployeeCount(this.Identity.SalonId, userTypes);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.Beautician, UserTypeEnum.SalonOwner)]
        public EmployeeDTO GetEmployee(int? id=null)
        {
            base.Validator<int?>(ValidateGetEmployee);

            if (!id.HasValue)
                employee = eeDAL.GetEmployee(this.Identity.UserId, this.Identity.SalonId);

            return Mapper.Map<EmployeeDTO>(employee);
        }

        [HttpGet]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        [Route("employee/beautician")]
        public List<EmployeeDTO> GetBeauticians()
        {
            var list = new List<EmployeeDTO>();
            eeDAL.GetBeauticians(this.Identity.SalonId).ForEach(e => 
            {
                list.Add(Mapper.Map<EmployeeDTO>(e));
            });
            return list;
        }

        
        [HttpPut]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void UpdateEmployee(EmployeeDTO dto)
        {
            base.Validator<EmployeeDTO>(ValidateUpdateEmployee);

            if (dto.PassWord != null && dto.PassWord.Length > 0)
                employee.Password = EncodingHelper.MD5(dto.PassWord);
            
            employee.NickName = dto.NickName;
            employee.IsDayoffMon = dto.IsDayoffMon;
            employee.IsDayoffTue = dto.IsDayoffTue;
            employee.IsDayoffWeb = dto.IsDayoffWeb;
            employee.IsDayoffThu = dto.IsDayoffThu;
            employee.IsDayoffFri = dto.IsDayoffFri;
            employee.IsDayoffSat = dto.IsDayoffSat;
            employee.IsDayoffSun = dto.IsDayoffSun;
            employee.Mobile = dto.Mobile;

            //只有SalonOwner可以修改员工的UserTypeId属性
            if (this.Identity.UserType == UserTypeEnum.SalonOwner)
            {
                //dto.UserTypeId原本是int,后来将其改成枚举UserTypeEnum，因为枚举在写表达式时看起来更直观，下次尝试把Model里的UserTypeId也改成枚举
                employee.UserTypeId = (int)dto.UserTypeId;
            }
            employee.Name = dto.Name;
            employee.Description = dto.Description;

            //更新的员工是美容师，IsBeautician必须是NULL,不是美容师，IsBeautician必须有值,
            if (employee.UserTypeId == (int)UserTypeEnum.Beautician)
                employee.IsBeautician = null;
            else
                employee.IsBeautician = dto.IsBeautician.Value;

            string toBeDeleted = null;
            if (dto.Picture != null)
            {
                if (employee.Picture != null)
                    toBeDeleted = Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_Absolute + employee.Picture;
                employee.Picture = FileHelper.SaveImageWithThumbnail(Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_Absolute, dto.Picture, Constant.EMPLOYEE_PORTRAIT_IMAGE_WIDTH_THUMBNAIL, Constant.EMPLOYEE_PORTRAIT_IMAGE_HEIGHT_THUMBNAIL);
            }

            eeDAL.Update(employee);

            if (toBeDeleted != null)
                FileHelper.DeleteImage(toBeDeleted, true);
        }

        [HttpPut]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner, UserTypeEnum.Beautician)]
        [Route("employee/profile")]
        public void UpdateProfile(EmployeeDTO dto)
        {
            base.Validator<EmployeeDTO>(ValidateUpdateProfile);

            if (dto.PassWord != null && dto.PassWord.Length > 0)
                employee.Password = EncodingHelper.MD5(dto.PassWord);

            employee.NickName = dto.NickName;
            employee.IsDayoffMon = dto.IsDayoffMon;
            employee.IsDayoffTue = dto.IsDayoffTue;
            employee.IsDayoffWeb = dto.IsDayoffWeb;
            employee.IsDayoffThu = dto.IsDayoffThu;
            employee.IsDayoffFri = dto.IsDayoffFri;
            employee.IsDayoffSat = dto.IsDayoffSat;
            employee.IsDayoffSun = dto.IsDayoffSun;
            employee.Name = dto.Name;
            employee.Description = dto.Description;

            //更新的员工不是美容师，IsBeautician必须有值。注意：此处不能使用this.Identity.UserType判断当前登录人的身份，因为如果老板在此员工登录后把他从美管修改成美容师
            //this.Identity.UserType任然读到美管，那么employee.IsBeautician被更新成true或者false，数据库就会存在是美容师但IsBeautician字段又有值这样的错误数据。
            if (employee.UserTypeId != (int)UserTypeEnum.Beautician)
                employee.IsBeautician = dto.IsBeautician.Value;

            string toBeDeleted = null;
            if (dto.Picture != null)
            {
                if (employee.Picture != null)
                    toBeDeleted = Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_Absolute + employee.Picture;
                employee.Picture = FileHelper.SaveImageWithThumbnail(Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_Absolute, dto.Picture, Constant.EMPLOYEE_PORTRAIT_IMAGE_WIDTH_THUMBNAIL, Constant.EMPLOYEE_PORTRAIT_IMAGE_HEIGHT_THUMBNAIL);
            }

            eeDAL.Update(employee);

            if (toBeDeleted != null)
                FileHelper.DeleteImage(toBeDeleted, true);
        }

        [HttpDelete]
        [ApiAuthorize(UserTypeEnum.SalonAdmin, UserTypeEnum.SalonOwner)]
        public void DeleteEmployee(int id)
        {
            base.Validator<int>(ValidateDeleteEmployee);

            employee.IsDeleted = true;
            eeDAL.Update(employee);
        }
        #endregion

        #region Validations
        [NonAction]
        public void ValidateUpdateProfile(EmployeeDTO dto)
        {
            employee = eeDAL.GetEmployee(this.Identity.UserId, this.Identity.SalonId);
            if (employee == null)
            {
                this.IsIllegalParameter = true;
                return;
            }

            this.ValidatorContainer.SetValue(dto.Picture)
                .IsImage(Constant.EMPLOYEE_PORTRAIT_IMAGE_WIDTH, Constant.EMPLOYEE_PORTRAIT_IMAGE_HEIGHT);

            this.ValidatorContainer.SetValue("姓名", dto.Name)
                .IsRequired()
                .Length(null, 6);

            
            this.ValidatorContainer.SetValue("密码", dto.PassWord)
                .Length(8, 16)
                .Pattern("[a-zA-Z]", "至少包含一个字母")
                .Pattern("[0-9]", "至少包含一个数字");


            this.ValidatorContainer.SetValue("昵称", dto.NickName)
                .IsRequired()
                .Length(null, 15)
                .Custom(() => !eeDAL.IsExistNickName(dto.NickName, employee.EmployeeId, this.Identity.SalonId), "已存在");

            //更新的员工不是美容师，IsBeautician必须有值
            if (employee.UserTypeId != (int)UserTypeEnum.Beautician)
                this.ValidatorContainer.SetValue("设为美容师", dto.IsBeautician).IsRequired();            

            this.ValidatorContainer.SetValue("简介", dto.Description)
                .Length(null, 200);
        }

        [NonAction]
        public void ValidateDeleteEmployee(int id)
        {
            employee = eeDAL.GetEmployee(id, this.Identity.SalonId);

            if (employee == null)
                this.IsIllegalParameter = true;
            else if (this.Identity.UserType == UserTypeEnum.SalonAdmin && employee.UserTypeId != (int)UserTypeEnum.Beautician)
                this.IsIllegalParameter = true;
            else if (employee.UserTypeId == (int)UserTypeEnum.SalonOwner)
                this.IsIllegalParameter = true;
        }

        [NonAction]
        public void ValidateGetEmployee(int? id = null)
        {
            if (this.Identity.UserType == UserTypeEnum.Beautician && id.HasValue)
            {
                this.IsIllegalParameter = true;
            }
            else if ((this.Identity.UserType == UserTypeEnum.SalonAdmin || this.Identity.UserType == UserTypeEnum.SalonOwner) && id.HasValue)
            {
                employee = eeDAL.GetEmployee(id.Value, this.Identity.SalonId);
                if (employee == null)
                    this.IsIllegalParameter = true;
                else if (this.Identity.UserType == UserTypeEnum.SalonAdmin && employee.UserTypeId != (int)UserTypeEnum.Beautician)
                    this.IsIllegalParameter = true;
            }
        }

        private void ValidateCreateUpdateEmployee(EmployeeDTO dto, bool isCreate)
        {
            if (this.Identity.UserType == UserTypeEnum.SalonAdmin)
            {
                //更新员工，美管只能更新美容师
                if (!isCreate)
                {
                    if (employee.UserTypeId != (int)UserTypeEnum.Beautician)
                    {
                        this.IsIllegalParameter = true;
                        return;
                    }
                }
            }


            this.ValidatorContainer.SetValue(dto.Picture)
                .IsImage(Constant.EMPLOYEE_PORTRAIT_IMAGE_WIDTH, Constant.EMPLOYEE_PORTRAIT_IMAGE_HEIGHT);

            this.ValidatorContainer.SetValue("姓名", dto.Name)
                .IsRequired()
                .Length(null, 6);

            this.ValidatorContainer.SetValue("手机", dto.Mobile)
                .IsRequired()
                .IsMobile()
                .Custom(() => !eeDAL.IsExistMobile(dto.Mobile, isCreate ? 0 : dto.EmployeeId, this.Identity.SalonId), "号码已存在");

            this.ValidatorContainer.SetValue("密码", dto.PassWord);
            if(isCreate)
                this.ValidatorContainer.IsRequired();
            this.ValidatorContainer
                .Length(8, 16)
                .Pattern("[a-zA-Z]", "至少包含一个字母")
                .Pattern("[0-9]", "至少包含一个数字");

            //SalonOwner可以创建或修改美容师和美管，美管创建的只能是美容师
            if (this.Identity.UserType == UserTypeEnum.SalonOwner)
            {
                this.ValidatorContainer.SetValue("员工类型", dto.UserTypeId)
                    .IsInList(UserTypeEnum.Beautician, UserTypeEnum.SalonAdmin);

                //只有SalonOwner才可能编辑美管，那么编辑的员工如果不是美容师，IsBeautician必填。
                //当登录人为美管，不需要验证IsBeautician，因为美管只能编辑美容师，而美容师的IsBeautician不从DTO里读，硬编码为null。
                if (dto.UserTypeId != UserTypeEnum.Beautician)
                    this.ValidatorContainer.SetValue("设为美容师", dto.IsBeautician).IsRequired();
            }

            this.ValidatorContainer.SetValue("昵称", dto.NickName)
                .IsRequired()
                .Length(null, 15)
                .Custom(() => !eeDAL.IsExistNickName(dto.NickName, isCreate ? 0 : dto.EmployeeId, this.Identity.SalonId), "已存在");

            this.ValidatorContainer.SetValue("简介", dto.Description)
                .Length(null, 200);
        }

        [NonAction]
        public void ValidateCreateEmployee(EmployeeDTO dto)
        {
            ValidateCreateUpdateEmployee(dto, true);
        }

        [NonAction]
        public void ValidateUpdateEmployee(EmployeeDTO dto)
        {
            employee = eeDAL.GetEmployee(dto.EmployeeId, this.Identity.SalonId);
            if (employee == null)
            {
                this.IsIllegalParameter = true;
                return;
            }
            ValidateCreateUpdateEmployee(dto, false);
        }
        #endregion
    }
}
