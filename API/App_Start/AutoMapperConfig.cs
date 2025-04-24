using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AutoMapper;
using CloudSalon.Model.DTO;
using CloudSalon.Model;
using CloudSalon.Common;
using CloudSalon.Model.Enum;

namespace CloudSalon.API
{
    public class AutoMapperServiceConfig
    {
        public static void Configure()
        {
            Mapper.Reset();



            CreateMapsModelToDto();
            CreateMapsDtoToModel();
        }


        private static void CreateMapsModelToDto()
        {
            Mapper.CreateMap<Employee, EmployeeDTO>()
                .ForMember(dto => dto.PassWord, opt => opt.Ignore())
                .ForMember(dto => dto.Picture, opt => opt.MapFrom(e => e.Picture == null ? e.Picture : Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_URL + e.Picture))
                .ForMember(dto => dto.SmallPicture, opt => opt.MapFrom(e => e.Picture == null ? e.Picture : Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_URL + "s" + e.Picture))
                .ForMember(dto => dto.TodayAppointmentCount, opt => opt.MapFrom(e => e.Appointments.Where(a => a.AppointmentDate.Date == DateTime.Now.Date).Count()))
                .ForMember(dto => dto.MonthCompletedAppointmentCount, opt => opt.MapFrom(e => e.Appointments.Where(a => a.AppointmentDate.Year == DateTime.Now.Year && a.AppointmentDate.Month == DateTime.Now.Month && a.AppointmentFlows.Where(af => af.AppointmentStatusId == (int)AppointmentStatusEnum.Completed).Count() == 1).Count()));

            Mapper.CreateMap<Employee, Beautician>()
                .ForMember(dto => dto.Picture, opt => opt.MapFrom(s => s.Picture == null ? null : Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_URL + s.Picture))
                .ForMember(dto => dto.ServedCount, opt => opt.MapFrom(s => s.Appointments.Where(a => a.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).First().AppointmentStatusId == (int)AppointmentStatusEnum.Completed).Count()));
            

            Mapper.CreateMap<Salon, SalonDTO>()
                .ForMember(dto => dto.BeauticianCount, opt => opt.MapFrom(s => s.Employees.Where(e=>e.IsDeleted==false && e.UserTypeId==(int)UserTypeEnum.Beautician).Count()))
                .ForMember(dto => dto.Picture, opt => opt.MapFrom(e => e.Picture == null ? e.Picture : Constant.SALON_IMAGE_FOLDER_URL + e.Picture))
                .ForMember(dto => dto.SmallPicture, opt => opt.MapFrom(e => e.Picture == null ? e.Picture : Constant.SALON_IMAGE_FOLDER_URL + "s" + e.Picture))
                .ForMember(dto => dto.QrCodePicture, opt => opt.MapFrom(e => e.QRCodePicture == null ? e.QRCodePicture : Constant.SALON_QRCODEIMAGE_FOLDER_URL + e.QRCodePicture));

            Mapper.CreateMap<Service, ServiceDTO>()
                .ForMember(dto => dto.ServiceTypeName, opt => opt.MapFrom(s => s.ServiceType.ServiceTypeName))
                .ForMember(dto => dto.SubjectImage, opt => opt.MapFrom(s => s.SubjectImage == null ? s.SubjectImage : Constant.SERVICE_SUBJECT_IMAGE_FOLDER_URL + s.SubjectImage))
                .ForMember(dto => dto.SmallSubjectImage, opt => opt.MapFrom(s => s.SubjectImage == null ? s.SubjectImage : Constant.SERVICE_SUBJECT_IMAGE_FOLDER_URL + "s" + s.SubjectImage))
                .ForMember(dto => dto.QrCodePicture, opt => opt.MapFrom(s => s.Salon.QRCodePicture == null ? s.Salon.QRCodePicture : Constant.SALON_QRCODEIMAGE_FOLDER_URL + s.Salon.QRCodePicture))
                .ForMember(dto => dto.FunctionalityTags, opt => opt.MapFrom(s => s.FunctionalityTags.OrderBy(t=>t.Seq).Select(t=>t.TagName)));

            Mapper.CreateMap<ServiceEffectImage, ServiceEffectImageDTO>()
                .ForMember(dto => dto.Image, opt => opt.MapFrom(s => Constant.SERVICE_EFFECT_IMAGE_FOLDER_URL + s.FileName))
                .ForMember(dto => dto.SmallImage, opt => opt.MapFrom(s => Constant.SERVICE_EFFECT_IMAGE_FOLDER_URL + "s" + s.FileName));
            
            Mapper.CreateMap<User, UserDTO>()
                .ForMember(dto => dto.Picture, opt => opt.MapFrom(s => s.Picture == null ? null : Constant.USER_PORTRAIT_IMAGE_FOLDER_URL + s.Picture))
                .ForMember(dto => dto.PurchasedServiceCount, opt => opt.MapFrom(u => u.PurchasedServices.Count))
                .ForMember(dto => dto.RemainPurchasedServiceCount, opt => opt.MapFrom(u => u.PurchasedServices.Where(ps => ps.Time - ps.ConsumedServiceDetails.Sum(csd => csd.Time) > 0).Count()))
                .ForMember(dto => dto.RemainTime, opt => opt.MapFrom(u => u.PurchasedServices.Sum(ps=>ps.Time-ps.ConsumedServiceDetails.Sum(csd=>csd.Time))))
                .ForMember(dto => dto.LastConsumedDate, opt => opt.MapFrom(u => u.ConsumedServices.OrderByDescending(c => c.ConsumedServiceId).Count() == 0 ? (DateTime?)null : u.ConsumedServices.OrderByDescending(c => c.ConsumedServiceId).First().CreatedDate));



            Mapper.CreateMap<SalonClose, SalonCloseDTO>();
            Mapper.CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dto => dto.ServiceName, opt => opt.MapFrom(a => a.ServiceSnapShot.ServiceName))
                .ForMember(dto => dto.NickName, opt => opt.MapFrom(a => a.Employee.NickName))
                .ForMember(dto => dto.Mobile, opt => opt.MapFrom(a => a.User.Mobile))
                .ForMember(dto => dto.AppointmentStatusId, opt => opt.MapFrom(a => a.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).First().AppointmentStatusId))
                .ForMember(dto => dto.SalonPhone, opt => opt.MapFrom(a => a.Employee.Salon.Phone))
                .ForMember(dto => dto.UserPicture, opt => opt.MapFrom(a => a.User.Picture == null ? a.User.Picture : Constant.USER_PORTRAIT_IMAGE_FOLDER_URL + a.User.Picture))
                .ForMember(dto => dto.UserNickName, opt => opt.MapFrom(a => a.User.NickName))
                .ForMember(dto => dto.CreatedDate, opt => opt.MapFrom(a => a.AppointmentFlows.OrderBy(af => af.AppointmentFlowId).First().CreatedDate))
                .ForMember(dto => dto.EmployeePicture, opt => opt.MapFrom(a => a.Employee.Picture == null ? a.Employee.Picture : Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_URL + "s" + a.Employee.Picture))
                .ForMember(dto => dto.ServiceId, opt => opt.MapFrom(a => a.ServiceSnapShot.ServiceId));

            Mapper.CreateMap<UnavaiTime, UnavaiTimeDTO>();
            Mapper.CreateMap<ServiceType, ServiceTypeDTO>();

            Mapper.CreateMap<PurchasedService, PurchasedServiceDTO_D>()
                .ForMember(dto => dto.ServiceName, opt => opt.MapFrom(ps => ps.ServiceSnapShot.ServiceName))
                .ForMember(dto => dto.Interval, opt => opt.MapFrom(ps => ps.ServiceSnapShot.TreatmentInterval))
                .ForMember(dto => dto.ConsumedServices, opt => opt.MapFrom(ps => ps.ConsumedServiceDetails))
                .ForMember(dto => dto.SmallSubjectImage, opt => opt.MapFrom(ps => ps.ServiceSnapShot.SubjectImage == null ? null : Constant.SERVICE_SUBJECT_IMAGE_FOLDER_URL + "s" + ps.ServiceSnapShot.SubjectImage))
                .ForMember(dto => dto.ServiceId, opt => opt.MapFrom(ps => ps.ServiceSnapShot.ServiceId));

            Mapper.CreateMap<ConsumedServiceDetail, ConsumedServiceDetailDTO_D>()
                .ForMember(dto => dto.CreatedDate, opt => opt.MapFrom(csd => csd.ConsumedService.CreatedDate))
                .ForMember(dto => dto.EmployeeNickName, opt => opt.MapFrom(csd => csd.ConsumedService.Employee.NickName))
                .ForMember(dto => dto.Duration, opt => opt.MapFrom(csd => csd.PurchasedService.ServiceSnapShot.Duration*csd.Time))
                .ForMember(dto => dto.IsAppoint, opt => opt.MapFrom(cs => cs.ConsumedService.AppointmentId.HasValue))
                .ForMember(dto => dto.Mode, opt => opt.MapFrom(cs => cs.ConsumedService.Mode))
                .ForMember(dto => dto.ConsumedServiceStatusId, opt => opt.MapFrom(cs => cs.ConsumedService.ConsumedServiceStatusId));


            Mapper.CreateMap<ConsumedService, ConsumedServiceDTO_D>()
                .ForMember(dto => dto.EmployeeNickName, opt => opt.MapFrom(cs => cs.Employee.NickName))
                .ForMember(dto => dto.ConsumedDate, opt => opt.MapFrom(cs => cs.CreatedDate))
                .ForMember(dto => dto.Time, opt => opt.MapFrom(cs => cs.ConsumedServiceDetails.Sum(csd => csd.Time)))
                .ForMember(dto => dto.ServiceId, opt => opt.MapFrom(cs => cs.ConsumedServiceDetails.ToList()[0].PurchasedService.ServiceSnapShot.ServiceId))
                .ForMember(dto => dto.ServiceName, opt => opt.MapFrom(cs => cs.ConsumedServiceDetails.ToList()[0].PurchasedService.ServiceSnapShot.Service.ServiceName))
                .ForMember(dto => dto.EmployeePicture, opt => opt.MapFrom(cs => cs.Employee.Picture == null ? null : Constant.EMPLOYEE_PORTRAIT_IMAGE_FOLDER_URL + "s" + cs.Employee.Picture))
                .ForMember(dto => dto.AppointmentDate, opt => opt.MapFrom(cs => cs.AppointmentId.HasValue ? cs.Appointment.AppointmentDate : (DateTime?)null))
                .ForMember(dto => dto.UserPicture, opt => opt.MapFrom(cs => cs.User == null ? null : cs.User.Picture == null ? null : Constant.USER_PORTRAIT_IMAGE_FOLDER_URL + cs.User.Picture))
                .ForMember(dto => dto.IsAnonym, opt => opt.MapFrom(cs => cs.User == null ? true : false))
                .ForMember(dto => dto.AppointmentStatusId, opt => opt.MapFrom(cs => cs.AppointmentId.HasValue ? (AppointmentStatusEnum)cs.Appointment.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).First().AppointmentStatusId : (AppointmentStatusEnum?)null));
        }

        private static void CreateMapsDtoToModel()
        {
            Mapper.CreateMap<EmployeeDTO, Employee>();
            Mapper.CreateMap<ServiceDTO, Service>()
                .ForMember(m => m.EffectImages, opt => opt.Ignore())
                .ForMember(m => m.FunctionalityTags, opt => opt.Ignore());
            Mapper.CreateMap<SalonCloseDTO, SalonDTO>();
            Mapper.CreateMap<UnavaiTimeDTO, UnavaiTime>();
            Mapper.CreateMap<AppointmentDTO, Appointment>();
            Mapper.CreateMap<PurchasedServiceDTO, PurchasedService>();
        }
    }
}