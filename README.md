SimpleServices
==============

A base class collection / pattern for simply building Windows services that run as services and console apps.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceProcess;

namespace SimpleServices.ExampleApplication
{
    [RunInstaller(true)]
    public class Program : ServiceInstaller
    {
        private static void Main(string[] args)
        {
            var container = new FakeIoCContainer();

            new Service(args,
                        new List<IWindowsService>
                            {
                                container.GetType<FirstHostedThing>(),
                                container.GetType<SecondHostedThing>()
                            }.ToArray,
                        installationSettings: (serviceInstaller, serviceProcessInstaller) =>
                            {
                                serviceInstaller.ServiceName = "SimpleServices.ExampleApplication";
                                serviceInstaller.StartType = ServiceStartMode.Manual;
                                serviceProcessInstaller.Account = ServiceAccount.LocalService;
                            },
                        registerContainer: () => new FakeIoCContainer(),
                        configureContext: x => { x.Log = Console.WriteLine; })
                .Host();
        }
    }
}

Available on NuGet.