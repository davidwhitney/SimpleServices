using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;

namespace SimpleServices
{
	public class ApplicationContext
	{
		public Action<string> Log { get; set; }
        public Action<System.ServiceProcess.ServiceInstaller, ServiceProcessInstaller> ConfigureInstall { get; set; }
        public AppCache Cache { get; private set; }
	    public IList<IWindowsService> Services { get; set; }
        public Func<IIocContainer> Container { get; set; }

	    public ApplicationContext()
		{
		    Log = Environment.UserInteractive ? Console.WriteLine : new Action<string>(s1 => Debug.WriteLine(s1));
		    ConfigureInstall = (installer, processInstaller) => { };
	        Container = () => { throw new NotImplementedException(); };
            Cache = new AppCache();
	        Services = new List<IWindowsService>();
		}

        public class AppCache : ConcurrentDictionary<string, object>
        {
            public T Get<T>(string key) where T : class
            {
                return this[key] as T;
            }
        }
	}
}