
// Add the following usings:
using Owin;
using System.Web.Http;
using Ninject;
using System.Reflection;
using Ninject.Web.WebApi.OwinHost;
using Ninject.Web.Common.OwinHost;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MinimalOwinWebApiSelfHost
{
    public class Startup
    {
        public static ObservableCollection<string> JobsLogCollection { get; set; }

        /// <summary>
        /// The startup class takes a job log collection instance
        /// </summary>
        /// <param name="jobCollection"></param>
        public Startup(ObservableCollection<string> jobsLogCollection)
        {
            JobsLogCollection = jobsLogCollection;
        }
        
        // This method is required by Katana:
        public void Configuration(IAppBuilder app)
        {
            var webApiConfiguration = ConfigureWebApi();

            app.UseNinjectMiddleware(CreateKernel).UseNinjectWebApi(webApiConfiguration);

            // Use the extension method provided by the WebApi.Owin library:
            app.UseWebApi(webApiConfiguration);
        }

        private static StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            kernel.Bind<INotifyCollectionChanged>().ToConstant(JobsLogCollection);

            return kernel;
        }

        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
              "MyRoute",
              "api/{controller}/{action}/{seconds}",
              new {});

            return config;
        }
    }
}
