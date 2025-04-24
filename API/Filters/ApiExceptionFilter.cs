using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Filters;
using System.Net.Http;
using Elmah;

namespace CloudSalon.API
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var ex = actionExecutedContext.Exception;

            if (ex is InvalidException)
            {
                InvalidException invalid = (InvalidException)ex;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, invalid.Messages);
            }
            else if (ex is IllegalParameterException)
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                //Other all exceptions would be logged here                
                byte[] b = HttpContext.Current.Request.BinaryRead(HttpContext.Current.Request.ContentLength);
                Error elmahError = new Error(ex, System.Web.HttpContext.Current);
                elmahError.Form.Add("requestBody", HttpContext.Current.Request.ContentEncoding.GetString(b));
                Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(elmahError);


                if (!HttpContext.Current.IsDebuggingEnabled)
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
                

            //var result = new ApiResult();
            //Error elmahError = null;
            //string errorID = null;
            //if (actionExecutedContext.Exception is FaultException)
            //{
            //    FaultException serviceException = (FaultException)actionExecutedContext.Exception;

            //    if (serviceException.Code.Name == "001")//invalid in services, send the details to API caller
            //    {
            //        result.Data = ((FaultException<InvalidMessageCollection>)serviceException).Detail;
            //        result.Status = HttpStatusCode.BadRequest;
            //        result.Message = serviceException.Message;
            //        actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(result.Status, result, "application/json");
            //        return;
            //    }
            //    else if (serviceException.Code.Name == "003")
            //    {
            //        string[] message = serviceException.Message.Split(new char[] { '|' });
            //        result.Message = message[0];
            //        result.Data = message[1].Length == 0 ? null : message[1];
            //        result.Status = HttpStatusCode.BadRequest;
            //        actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(result.Status, result, "application/json");
            //        return;
            //    }
            //    else if (serviceException.Code.Name == "002")  //error in services, services have logged and handled it, API caller should NOT know the details
            //    {
            //        elmahError = new Error(serviceException, System.Web.HttpContext.Current);
            //        elmahError.Form.Add("APIBody", actionExecutedContext.Request.Properties["json"].ToString());
            //        errorID = Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(elmahError);

            //        result.Data = null;
            //        result.Status = HttpStatusCode.InternalServerError;
            //        result.Message = "service error";
            //        result.ErrorID = errorID;

            //        actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(result.Status, result, "application/json");
            //        return;
            //    }
            //}

            //elmahError = new Error(actionExecutedContext.Exception, System.Web.HttpContext.Current);
            //elmahError.Form.Add("APIBody", actionExecutedContext.Request.Properties["json"].ToString());
            //errorID = Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(elmahError);

            ////unhandled error in API, API should log and handle it, API caller should NOT know the details
            //result.Status = HttpStatusCode.InternalServerError;
            //result.Message = "API error";
            //result.Data = null;
            //result.ErrorID = errorID;
            //actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(result.Status, result, "application/json");

        }
    }
}