using System;

// Add reference to:
using Microsoft.Owin.Hosting;
using System.Collections.ObjectModel;
using System.Threading;
using log4net;

namespace MinimalOwinWebApiSelfHost
{
    public class MyPollingJobAcceptModule : ISyncModule
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void Run()
        {
            log.InfoFormat("I will do my thing and finish.");
        }
    }

    public class JobAcceptModule : ISyncModule
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Observable JobsLogCollection
        /// </summary>
        public ObservableCollection<string> JobsCollection { get; set; }

        /// <summary>
        /// Instance specific configuration property
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// Does the actual work
        /// </summary>
        public void Run()
        {

            log.InfoFormat("Thread:{0}, Managed ThreadPool:{1}; Starting web Server...", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);

            var server = WebApp.Start(BaseUri, (appBuilder) =>
            {
                var startup = new Startup(JobsCollection);

                startup.Configuration(appBuilder);
            });

            log.InfoFormat("Server running at {0} - press Enter to quit. ", BaseUri);


            Console.ReadLine();
        }
    }
}
