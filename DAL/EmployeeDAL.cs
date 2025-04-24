using CloudSalon.Model;
using CloudSalon.Model.Enum;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CloudSalon.DAL
{
    public class EmployeeDAL : BaseDAL<Employee>
    {
        public EmployeeDAL(SalonContext dbContext)
        {
            base.dbContext = dbContext;
        }

        public List<Employee> AuthEmployee(string mobile, string password, bool isGetSalon)
        {
            IQueryable<Employee> list = dbContext.Query<Employee>();

            if (isGetSalon)
                list = list.Include(e => e.Salon);

            return list.Where(
                e => e.Mobile == mobile
                && e.Password == password
                && e.IsDeleted == false).ToList();
        }

        public List<Employee> GetEmployees(int salonId, int pageNumber, int pageSize, bool isGetAppointment, List<UserTypeEnum> userTypes)
        {
            IQueryable<Employee> list = dbContext.Query<Employee>();

            if (isGetAppointment)
                list = list.Include("Appointments.AppointmentFlows");            

            return list
                .Where(ee => ee.SalonId == salonId && ee.IsDeleted == false && userTypes.Contains((UserTypeEnum)ee.UserTypeId))
                .OrderByDescending(ee => ee.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
        }

        public int GetEmployeeCount(int salonId, List<UserTypeEnum> userTypes)
        {
            return dbContext.Query<Employee>()
                .Where(e => e.SalonId == salonId && e.IsDeleted == false && userTypes.Contains((UserTypeEnum)e.UserTypeId)).Count();
        }

        public List<Employee> GetBeauticians(int salonId)
        {
            return dbContext.Query<Employee>()
                .Include(ee=>ee.Appointments)
                .Include(ee=>ee.UnavaiTimes)
                .Where(ee => ee.SalonId == salonId && ee.IsDeleted == false
                    && (ee.UserTypeId == (int)UserTypeEnum.Beautician || ee.IsBeautician == true))
                .ToList();
        }

        public Employee GetBeautician(int employeeId, int salonId)
        {
            return dbContext.Query<Employee>()
                .Where(ee => ee.EmployeeId == employeeId && ee.SalonId == salonId && ee.IsDeleted == false
                    && (ee.UserTypeId == (int)UserTypeEnum.Beautician || ee.IsBeautician == true))
                    .FirstOrDefault();
        }


        public Employee GetEmployee(int employeeId, int salonId)
        {
            return dbContext.Query<Employee>()
                .Where(ee => ee.EmployeeId == employeeId && ee.SalonId == salonId && ee.IsDeleted == false).FirstOrDefault();
        }

        public bool IsExistMobile(string mobile, int employeeId, int salonId)
        {
            return dbContext.Query<Employee>()
                .Where(ee => ee.Mobile == mobile
                && ee.SalonId == salonId
                && ee.IsDeleted == false
                && ee.EmployeeId != employeeId
                ).Count() > 0;
        }

        public bool IsExistNickName(string nickName, int employeeId, int salonId)
        {
            return dbContext.Query<Employee>()
                .Where(ee => ee.NickName == nickName
                && ee.SalonId == salonId
                && ee.IsDeleted == false
                && ee.EmployeeId != employeeId
                ).Count() > 0;
        }

        public int CreateUnavaiAppointment(UnavaiTime entity)
        {
            dbContext.Insert<UnavaiTime>(entity);
            return entity.UnavaiId;
        }

        public List<UnavaiTime> GetUnavaiTimes(int employeeId)
        {
            DateTime today = DateTime.Now.Date;
            return dbContext.Query<UnavaiTime>().Where(u => u.EmployeeId == employeeId && u.UnavaiDate == today)
                .OrderBy(u=>u.StartTime).ToList();
        }

        public UnavaiTime GetUnavaiTime(int id,int employeeId)
        {
            return dbContext.Query<UnavaiTime>().Where(u => u.UnavaiId == id && u.EmployeeId == employeeId).FirstOrDefault();
        }

        public void DeleteUnavaiTime(UnavaiTime entity)
        {
            dbContext.Delete<UnavaiTime>(entity);
        }
    }
}
