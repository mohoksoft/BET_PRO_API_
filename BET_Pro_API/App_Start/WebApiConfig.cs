using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Timers;
using System.Web.Http;

namespace BET_PRO_API
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public static class WebApiConfig
    {
        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            //config.EnableCors();

            config.Routes.MapHttpRoute(
                            name: "DefaultApi",
                            routeTemplate: "api/{controller}/{id}",
                            defaults: new { id = RouteParameter.Optional }
                        );

            //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "BetProLog.txt"), true))
            //    outputFile.WriteLine("Start: " + DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
