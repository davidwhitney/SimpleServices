using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceProcess;

namespace SimpleServices.ExampleApplication
{
    [RunInstaller(true)]
    public class Program : SimpleServiceApplication
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
