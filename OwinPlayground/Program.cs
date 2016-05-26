using MinimalOwinWebApiSelfHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using System.Net;
using System.Threading.Tasks;

namespace OwinPlayground
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      
        static void Main(string[] args)
        {
            log.InfoFormat("Main entry point Thread{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            //Setup the observable collection
            //Not thread safe!
            ObservableCollection<string> jobsCollection = new ObservableCollection<string>();
            jobsCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChangedHandler);

            var modules = new List<ISyncModule>();
            modules.Add(new JobAcceptModule() { JobsCollection = jobsCollection, BaseUri = "http://localhost:8081" });
            modules.Add(new JobAcceptModule() { JobsCollection = jobsCollection, BaseUri = "http://localhost:8080" });
            modules.Add(new MyPollingJobAcceptModule());

            log.InfoFormat("Processor count {0}", Environment.ProcessorCount);

            //RegisterModulesWithPallarelInvoke(modules);
            
            RegisterModules(modules);

        }

        /// <summary>
        /// Registers the modules via TPL Pallarel.Invoke
        /// </summary>
        /// <param name="modules"></param>
        private static void RegisterModulesWithParallelInvoke(IList<ISyncModule> modules)
        {
            ParallelOptions parOpts = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 1
            };
            var actions = new List<Action>();
            foreach (var mm in modules)
            {
                actions.Add(() => mm.Run());
            }
            Parallel.Invoke(actions.ToArray());
        }

        /// <summary>
        /// Registers the modules via TPL Task
        /// </summary>
        /// <param name="modules"></param>
        private static void RegisterModules(IList<ISyncModule> modules)
        {
            var tasks = new List<Task>();
            foreach (var m in modules)
            {
                //Will start a new thread outside the thread pool
                var task = Task.Factory.StartNew(
                        () => {
                          
                            m.Run();
                            
                        },
                        TaskCreationOptions.LongRunning
                    );

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Collection changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Handle the Collection Add event
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                log.InfoFormat("Thread {0}, CollectionChangedMethod added job: " + e.NewItems[0].ToString(), System.Threading.Thread.CurrentThread.ManagedThreadId);
                using (WebClient client = new WebClient())
                {
                    //client.DownloadDataAsync(new Uri("http://localhost:8081/api/Jobs/42"));
                    //client.DownloadData("http://localhost:8081/api/Jobs/2");
                }

            }
        }
    }
}
