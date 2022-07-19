using System.Web;
using System.Web.Http;
using static UpdateTimer;

namespace BET_PRO_API
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class WebApiApplication : HttpApplication
    {
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        protected void Application_Start()
        {
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //ListTimer.StartDataTimer(60000);
        }
    }
}
