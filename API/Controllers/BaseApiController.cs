using CloudSalon.Common;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CloudSalon.API
{
    public class BaseApiController : ApiController
    {
        public BaseApiController()
        {
            this.InvalidMessages = new List<string>();
            this.ValidatorContainer = new ValidatorContainer(this.InvalidMessages);
        }


        public List<string> InvalidMessages { get; set; }
        public bool IsIllegalParameter{ get; set; }
        public ValidatorContainer ValidatorContainer { get; set; }


        protected Identity Identity
        {
            get
            {
                return (Identity)this.User.Identity;
            }
        }

        #region 调用自定义验证器,具体验证方法定义在各个Controller里。这个验证器用于验证数据有效性，如果数据无效，应该返回400，并给出原因
        protected void Validator<T>(Action<T> func)
        {
            T p1 = (T)this.ActionContext.ActionArguments.ToList()[0].Value;
            func(p1);

            if (this.IsIllegalParameter)
                throw new IllegalParameterException();

            if (this.InvalidMessages.Count > 0)
                throw new InvalidException() { Messages = this.InvalidMessages };
        }

        protected void Validator<T1,T2>(Action<T1,T2> func)
        {
            T1 p1 = (T1)this.ActionContext.ActionArguments.ToList()[0].Value;
            T2 p2 = (T2)this.ActionContext.ActionArguments.ToList()[1].Value;
            func(p1, p2);

            if (this.IsIllegalParameter)
                throw new IllegalParameterException();

            if (this.InvalidMessages.Count > 0)
                throw new InvalidException() { Messages = this.InvalidMessages };
        }

        protected void Validator<T1, T2,T3>(Action<T1, T2,T3> func)
        {
            T1 p1 = (T1)this.ActionContext.ActionArguments.ToList()[0].Value;
            T2 p2 = (T2)this.ActionContext.ActionArguments.ToList()[1].Value;
            T3 p3 = (T3)this.ActionContext.ActionArguments.ToList()[2].Value;
            func(p1, p2, p3);

            if (this.IsIllegalParameter)
                throw new IllegalParameterException();

            if (this.InvalidMessages.Count > 0)
                throw new InvalidException() { Messages = this.InvalidMessages };
        }

        protected void Validator<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func)
        {
            T1 p1 = (T1)this.ActionContext.ActionArguments.ToList()[0].Value;
            T2 p2 = (T2)this.ActionContext.ActionArguments.ToList()[1].Value;
            T3 p3 = (T3)this.ActionContext.ActionArguments.ToList()[2].Value;
            T4 p4 = (T4)this.ActionContext.ActionArguments.ToList()[3].Value;
            func(p1, p2, p3, p4);

            if (this.IsIllegalParameter)
                throw new IllegalParameterException();

            if (this.InvalidMessages.Count > 0)
                throw new InvalidException() { Messages = this.InvalidMessages };
        }
        #endregion
    }
}