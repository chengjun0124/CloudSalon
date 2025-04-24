using CloudSalon.Common;
using CloudSalon.Model.DTO;
using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace CloudSalon.API
{
    public class ApiAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        UserTypeEnum[] _userTypes;
        public ApiAuthorizeAttribute(params UserTypeEnum[] userTypes) 
        {
            this._userTypes = userTypes;
        }
        

        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (!actionContext.RequestContext.Principal.Identity.IsAuthenticated)
                return false;
            Identity identity = (Identity)actionContext.RequestContext.Principal.Identity;


            if (this._userTypes.Contains(identity.UserType))
                return true;
            else//SalonAdmin and SalonOwner可以被设为美容师，所以需要以下逻辑
                return this._userTypes.Contains(UserTypeEnum.Beautician) && identity.IsBeautician.HasValue && identity.IsBeautician.Value;
            
        }
    }
    public class Identity : IIdentity
    {
        public string AuthenticationType
        {
            get { return ""; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return ""; }
        }

        public int UserId { get; set; }
        public UserTypeEnum UserType { get; set; }
        public int SalonId { get; set; }
        public bool? IsBeautician { get; set; }

    }

    public class AuthenticationFilter : IAuthenticationFilter
    {
        public async Task AuthenticateAsync(HttpAuthenticationContext context, System.Threading.CancellationToken cancellationToken)
        {
            // 1. Look for credentials in the request.
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;
            
            // 2. If there are no credentials, do nothing.
            if (authorization != null)
            {
                string claim = authorization.Parameter.Split('.')[0];
                string sign = authorization.Parameter.Split('.')[1];

                if (EncodingHelper.HMACMD5(claim, Constant.JWTKey) == sign)
                {
                    JWT jwt = Newtonsoft.Json.JsonConvert.DeserializeObject<JWT>(EncodingHelper.DecodeBase64(claim));

                    if (DateTime.Now.Ticks < jwt.Expire)
                    {
                        IIdentity i = new Identity() 
                        {
                            UserId =jwt.UserId,
                            UserType = (UserTypeEnum)jwt.UserTypeId,
                            SalonId = jwt.SalonId,
                            IsBeautician = jwt.IsBeautician
                        };
                        IPrincipal p = new System.Security.Principal.GenericPrincipal(i, null);
                        context.Principal = p;
                    }
                }
            }
            

            //

            //// 3. If there are credentials but the filter does not recognize the 
            ////    authentication scheme, do nothing.
            //if (authorization.Scheme != "Basic")
            //{
            //    return;
            //}

            //// 4. If there are credentials that the filter understands, try to validate them.
            //// 5. If the credentials are bad, set the error result.
            //if (String.IsNullOrEmpty(authorization.Parameter))
            //{
            //    context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
            //    return;
            //}

            //Tuple<string, string> userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);
            //if (userNameAndPasword == null)
            //{
            //    context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
            //}

            //string userName = userNameAndPasword.Item1;
            //string password = userNameAndPasword.Item2;

            //IPrincipal principal = await AuthenticateAsync(userName, password, cancellationToken);
            //if (principal == null)
            //{
            //    context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
            //}

            //// 6. If the credentials are valid, set principal.
            //else
            //{
            //    context.Principal = principal;
            //}
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, System.Threading.CancellationToken cancellationToken)
        {
            return;
        }

        public bool AllowMultiple
        {
            get { throw new NotImplementedException(); }
        }
    }
}