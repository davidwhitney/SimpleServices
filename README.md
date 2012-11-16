SimpleServices
==============

A base class for self-installing Windows Services with debugging hooks to run as console apps without requiring InstallUtil
This allows you to build Windows services that also can be F5 run / debugged straight from Visual Studio with minimal effort.

SimpleServices allows you to manually control service installation from code, rather than awkward designer files that do code-gen.

- Support for IoC Containers
- Support for manipulation of installation in code
- Creates auto-installing services (no SVCUTIL required!)
- Support for logging frameworks
- Built in App cache concurrent dictionary to share state between isolated hosted IWindowsServices.

You need to implement the interface "IWindowsService":

    public interface IWindowsService
    {
        void Start(string[] args);
        void Stop();
        ApplicationContext AppContext { get; set; }
    }
    
And you're good to go.

You application entry point will look something like this (taken from the provided executing example):

    [RunInstaller(true)]
    public class Program : ServiceInstaller
    {
        private static void Main(string[] args)
        {
            new Service(args,
                        new List<IWindowsService>
                        {
                            new MyService(),
                        }.ToArray,
                        installationSettings: (serviceInstaller, serviceProcessInstaller) =>
                        {
                            serviceInstaller.ServiceName = "SimpleServices.ExampleApplication";
                            serviceInstaller.StartType = ServiceStartMode.Manual;
                            serviceProcessInstaller.Account = ServiceAccount.LocalService;
                        },
                        configureContext: x => { x.Log = Console.WriteLine; })
                .Host();
        }
    
        class MyService  : IWindowsService
        {
            public ApplicationContext AppContext { get; set; }
            
            public void Start(string[] args)
            {
                AppContext.Log("Hello world!");
            }
    
            public void Stop()
            {
                AppContext.Log("Goodbye world!");
            }
    
        }
    }

Available on NuGet.