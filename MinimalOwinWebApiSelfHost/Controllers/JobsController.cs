using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.Http;
using MinimalOwinWebApiSelfHost.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;


namespace MinimalOwinWebApiSelfHost.Controllers
{
    public class JobsController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Mock a data store:
        private static IList<Job> jobsStore = new List<Job>
            {
                new Job { Id = 1, Name = "Analyze" },
                new Job { Id = 2, Name = "Filter" },
                new Job { Id = 3, Name = "Migrate" }
            };

        private readonly ObservableCollection<string> jobQueueLog;

        public JobsController(ObservableCollection<string> jobQueueLog)
        {
            this.jobQueueLog = jobQueueLog;
        }

        public JobsController(INotifyCollectionChanged jobQueue)
        {
            this.jobQueueLog = jobQueue as ObservableCollection<string>;

            if(this.jobQueueLog == null)
            {
                throw new InvalidOperationException("Invalid job collection!");
            }
        }

        public IEnumerable<Job> Get()
        {
            return jobsStore;
        }

        /// <summary>
        /// Specifies the number a seconds for which the running thread will be blocked
        /// </summary>
        /// <param name="seconds">Assume seconds is a positive number</param>
        [HttpGet]
        public void StartLongRunningJob(int seconds)
        {
            Thread.Sleep(seconds * 1000);

            log.InfoFormat("Controller entry point Thread {0}, Managed Thread Pool Thread: {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
            
            //Should raise the CollectionChanged event on the observable collection 
            this.jobQueueLog.Add("Long running job...");
        }

        public Job Get(int id)
        {

            log.InfoFormat(string.Format("Controller entry point Thread {0}, Managed Thread Pool Thread: {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread));

            //Should raise the CollectionChanged event on the observable collection 
            this.jobQueueLog.Add(String.Format("job added: {0}", id));

            var job = jobsStore.FirstOrDefault(c => c.Id == id);
            if (job == null)
            {
                throw new HttpResponseException(
                    System.Net.HttpStatusCode.NotFound);
            }
            return job;
        }
    }
}
