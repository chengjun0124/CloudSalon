using CloudSalon.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CloudSalon.Model.Enum;

namespace CloudSalon.DAL
{
    public class AppointmentDAL : BaseDAL<Appointment>
    {
        public AppointmentDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }


        public List<Appointment> GetAppointments(int? salonId, int? employeeId, DateTime? date, int pageNumber, int pageSize, params AppointmentStatusEnum[] status)
        {
            IQueryable<Appointment> list = dbContext.Query<Appointment>();
            
            list=list.Include(a => a.User);
            list = list.Include(a => a.ServiceSnapShot);
            list = list.Include(a => a.Employee);

            if (date.HasValue)
            {
                DateTime end=date.Value.AddDays(1);
                list = list.Where(a => a.AppointmentDate >= date && a.AppointmentDate < end);
            }
            if (status.Length > 0)
            {
                list = list.Include(a => a.AppointmentFlows);
                list = list.Where(a => status.Contains((AppointmentStatusEnum)a.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).FirstOrDefault().AppointmentStatusId));
            }

            if (salonId.HasValue)
                return list.Where(a => a.Employee.SalonId == salonId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize).ToList();
            else
                return list.Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.AppointmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
        }

        public List<Appointment> GetAppointmentsByUserId(int userId, DateTime? startDate, DateTime? endDate, params AppointmentStatusEnum[] status)
        {
            IQueryable<Appointment> list = dbContext.Query<Appointment>();

            list = list.Include(a => a.User);
            list = list.Include(a => a.ServiceSnapShot);
            list = list.Include(a => a.Employee);
            list = list.Include(a => a.AppointmentFlows);

            list = list.Where(a => a.UserId == userId);

            if (startDate.HasValue)
                list = list.Where(a => a.AppointmentDate >= startDate);

            if(endDate.HasValue)
                list = list.Where(a => a.AppointmentDate < endDate);

            if (status != null && status.Length > 0)
                list = list.Where(a => status.Contains((AppointmentStatusEnum)a.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).FirstOrDefault().AppointmentStatusId));
            
            return list.OrderBy(a => a.AppointmentDate).ToList();
        }

        public Appointment GetAppointment(int appointmentId, UserTypeEnum userType, int id, bool isGetAppointmentFlow, bool isGetEmployee, bool isGetServiceSnapShot, bool isGetUser, bool isGetConsumedService, bool isGetSalon)
        {
            IQueryable<Appointment> list = dbContext.Query<Appointment>();

            if (isGetAppointmentFlow)
                list = list.Include(a => a.AppointmentFlows);

            if (isGetEmployee)
                list = list.Include(a => a.Employee);

            if (isGetServiceSnapShot)
                list = list.Include(a => a.ServiceSnapShot);

            if(isGetUser)
                list = list.Include(a => a.User);

            if (isGetConsumedService)
                list = list.Include(a => a.ConsumedServices);

            if(isGetSalon)
                list = list.Include("Employee.Salon");

            if (userType == UserTypeEnum.User)
                return list.Where(a => a.AppointmentId == appointmentId && a.UserId == id).FirstOrDefault();
            else if (userType == UserTypeEnum.Beautician)
                return list.Where(a => a.AppointmentId == appointmentId && a.EmployeeId == id).FirstOrDefault();
            else
                return list.Where(a => a.AppointmentId == appointmentId && a.Employee.SalonId == id).FirstOrDefault();
        }

        public AppointmentFlow CreateAppointFlow(AppointmentFlow entity)
        {
            dbContext.Insert<AppointmentFlow>(entity);
            return entity;
        }

        public int GetSalonAppointmentCountByDate(DateTime date, int salonId)
        {
            date = date.Date;
            DateTime endDate = date.AddDays(1);

            return dbContext.Query<Appointment>().Where(a =>
                a.Employee.SalonId == salonId &&
                a.AppointmentDate >= date && a.AppointmentDate < endDate && 
                a.AppointmentFlows.Where(af => af.AppointmentStatusId == (int)AppointmentStatusEnum.Confirmed).Count() == 1).Count();
        }

        public int GetEmployeeAppointmentCountByDate(DateTime date, int employeeId)
        {
            date = date.Date;
            DateTime endDate = date.AddDays(1);

            return dbContext.Query<Appointment>().Where(a =>
                a.EmployeeId == employeeId &&
                a.AppointmentDate >= date && a.AppointmentDate < endDate &&
                a.AppointmentFlows.Where(af => af.AppointmentStatusId == (int)AppointmentStatusEnum.Confirmed).Count() == 1).Count();
        }

        public Appointment GetRecentAppointment(int employeeId)
        {
            DateTime date = DateTime.Now.AddMinutes(1);
            DateTime endDate = date.Date.AddDays(1);
            return dbContext.Query<Appointment>().Where(a =>
                a.EmployeeId == employeeId &&
                a.AppointmentFlows.Where(af => af.AppointmentStatusId == (int)AppointmentStatusEnum.Confirmed).Count() == 1 &&
                a.AppointmentDate > date && a.AppointmentDate < endDate
                ).OrderBy(a=>a.AppointmentDate).FirstOrDefault();
        }

        public List<int> GetAppointmentCount(DateTime start, DateTime end, int? salonId, int? employeeId, params AppointmentStatusEnum[] status)
        {
            end = end.AddDays(1);

            IQueryable<Appointment> list=this.dbContext.Query<Appointment>()
                .Include(a => a.AppointmentFlows)
                .Where(a => a.AppointmentDate >= start
                && a.AppointmentDate < end);

            if (salonId.HasValue)
                list = list.Where(a => a.Employee.SalonId == salonId);
            else
                list = list.Where(a => a.EmployeeId == employeeId);

            if(status.Length>0)
                list = list.Where(a => status.Contains((AppointmentStatusEnum)a.AppointmentFlows.OrderByDescending(af => af.AppointmentFlowId).FirstOrDefault().AppointmentStatusId));

            List<IGrouping<DateTime, Appointment>> group = list.ToList().GroupBy(a => a.AppointmentDate.Date).ToList();

            DateTime date = start;
            IGrouping<DateTime, Appointment> g = null;
            List<int> count = new List<int>();
            while (date < end)
            {
                g = group.Find(a => a.Key == date);
                count.Add(g == null ? 0 : g.Count());

                date = date.AddDays(1);
            }

            return count;
        }
    }
}
