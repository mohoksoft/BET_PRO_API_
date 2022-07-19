using System.Web.Http;
using WebActivatorEx;
using BET_PRO_API;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace BET_PRO_API
{
    ///<Summary>
    /// XML Komentar
    ///</Summary>
    public class SwaggerConfig
    {
        private static string GetXmlCommentsPath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory + @"\bin\BET_PRO_API.xml";
        }

        ///<Summary>
        /// XML Komentar
        ///</Summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                       
                        c.SingleApiVersion("v1", "BET_PRO_API");

                       
                        c.IncludeXmlComments(GetXmlCommentsPath());

                        
                    })
                .EnableSwaggerUi(c =>
                    {
                        
                    });
        }
    }
}
