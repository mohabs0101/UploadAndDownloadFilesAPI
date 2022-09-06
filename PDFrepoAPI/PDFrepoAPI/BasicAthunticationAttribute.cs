using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;

 
using System.Web.Http.Filters;
 

namespace PDFrepoAPI
{
    public class BasicAthunticationAttribute : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)//action context parameter provid us access to requers and response objrct
        {
            //in basic authontication client sends cradintials using header 
            //the requerst objct has headers and we check for authorazation header 
            if (actionContext.Request.Headers.Authorization == null)//client did not send cridentials
            {
                //send unauthraized response
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else//if header is avilable 
            {
                //retrive username and password 
                string authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                //authenticationToken will be based64 encoded in this format username:password
                //decode base64 
                string decodedauthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                //splite username:password
                string[] UsernamePasswordArray = decodedauthenticationToken.Split(':');//will return string array ,first username second password 
                string username = UsernamePasswordArray[0];
                string password = UsernamePasswordArray[1];
                //set current princeable of excuting thread to this username 
                //send username to archivesecurity class to check it in db 
                if (PDFrepoSecurity.login(username, password)) //if the method return true(username and passwordmatched)
                {
                    //set princable to username ,set current principle ffor the thread by ccreating genericidentity
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);

                }
                else
                {
                    //send unauthraized response
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }

    }
}