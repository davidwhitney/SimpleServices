SimpleServices
==============

[![Build status](https://ci.appveyor.com/api/projects/status?id=sro2wbgp7naykoet)](https://ci.appveyor.com/project/simpleservices)

# Breaking changes

* V.2 - Clients must rename their Program.cs inheritance of ServiceInstaller to SimpleServiceApplication.

#Intro

A base class for self-installing Windows Services with debugging hooks to run as console apps without requiring InstallUtil
This allows you to build Windows services that also can be F5 run / debugged straight from Visual Studio with minimal effort.

SimpleServices allows you to manually control service installation from code, rather than awkward designer files that do code-gen.

- Support for IoC Containers
- Support for manipulation of installation in code
- Creates auto-installing services (no SVCUTIL required!)
- Support for logging frameworks
- Built in App cache concurrent dictionary to share state between isolated hosted IWindowsServices.

# Getting started

Simple services requires you do two things:

 - Implement the interface IWindowsService on the entry point of your application logic class
 - Use it's bootstrapping code to initilise your app.

# Example

If you had a console app that looked like this:
    
    public static void Main_Original(string[] args)
    {
        Console.WriteLine("Hello world!");
        Console.ReadLine();
        Console.WriteLine("Goodbye world!");
    }

You would first need to extract out your login into a class that implemented IWindowsService.
The interface looks like this:

    public interface IWindowsService
    {       
        ApplicationContext AppContext { get; set; }
        void Start(string[] args);
        void Stop();
    }
    
Once you did that, you'd have a class that looked like this:

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

Which is now ready to run as either a console app or a windows service.
Now, you'd want to replace your application startup code in Main with this:

    [RunInstaller(true)]
    public class Program : SimpleServiceApplication
    {
        private static void Main(string[] args)
        {
            new Service(args,
                        new List<IWindowsService> { new MyService() }.ToArray,
                        installationSettings: (serviceInstaller, serviceProcessInstaller) =>
                        {
                            serviceInstaller.ServiceName = "SimpleServices.ExampleApplication";
                            serviceInstaller.StartType = ServiceStartMode.Manual;
                            serviceProcessInstaller.Account = ServiceAccount.LocalService;
                        },
                        configureContext: x => { x.Log = Console.WriteLine; })
                .Host();
        }
    }

And when you press F5, your application will behave just as it did before.


# But what do I get that's extra?

Now that you've started using SimpleServices, you can install your application as a Windows service by calling:

 - MyExeName.exe /i or MyExeName.exe /install

and you can uninstall by calling: 

 - MyExeName.exe /u or MyExeName.exe /uninstall

# Get it!

- Pull / build this repository
- From NuGe => PM> Install-Package simpleservices
