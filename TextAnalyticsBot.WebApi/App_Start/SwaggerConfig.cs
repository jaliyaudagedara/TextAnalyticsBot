using System.Web.Http;
using WebActivatorEx;
using TextAnalyticsBot.WebApi;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]
namespace TextAnalyticsBot.WebApi
{
    /// <summary>
    /// Class SwaggerConfig.
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Registers this instance.
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "TextAnalyticsBot.WebApi");
                    c.IncludeXmlComments(string.Format(@"{0}\bin\TextAnalyticsBot.WebApi.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                })
                .EnableSwaggerUi();
        }
    }
}
