using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http;
using System.Reflection;
using Ninject;

namespace CloudSalon.API
{
    public class ApiActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            #region Attribute验证器，用于验证非法参数，非法参数将返回404
            var ivalidators = actionContext.ActionDescriptor.GetCustomAttributes<IllegalParamValidatorAttribute>();
            foreach (var v in ivalidators)
            {
                object paramValue = null;
                //从参数列表里找到参数的值，传给验证器
                int index = actionContext.ActionArguments.Keys.ToList().IndexOf(v.ParamName.Split('.')[0]);
                paramValue = actionContext.ActionArguments.Values.ElementAt(index);

                if(v.ParamName.Split('.').Length>1)
                    paramValue = paramValue.GetType().GetProperty(v.ParamName.Split('.')[1]).GetValue(paramValue);
                

                //验证器需要Identity，这样验证器可以读取SalonId，UserId等信息
                v.Controller = (BaseApiController)actionContext.ControllerContext.Controller;

                //验证器里如果有[Inject],在这里注入所需要的对象
                v.GetType().GetProperties().ToList().ForEach(p => 
                {
                    if (p.GetCustomAttributes(typeof(InjectAttribute)).Count() > 0)
                    {
                        p.SetValue(v, NinjectRegister.Kernel.Get(p.PropertyType));
                    }
                });

                //参数值为空，不做验证
                if (paramValue != null && !v.Validate(paramValue))
                    throw new IllegalParameterException();
            }
            #endregion
        }
    }
}