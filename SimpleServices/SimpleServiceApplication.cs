using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;

// ReSharper disable ClassNeverInstantiated.Global
namespace SimpleServices
{
    /// <summary>
    /// Instantiated by the framework as part of installation.
    /// Weird looking implementation is required to work around the way the framework
    /// service installation class works (it finds a class that inherits Installer and has a RunInstaller(true) attrib
    /// creates an instance of it and executes it. To enable configuration, we need to punch an Action shaped hole in the side
    /// that can be invoked by the zero param ctor)
    /// </summary>
    [RunInstaller(true)]
    public class SimpleServiceApplication : Installer
    {
        private static Action<SimpleServiceApplication> _configureAction = configuration => { };

        public static void PerformAnyRequestedInstallations(string[] args, ApplicationContext context, string assemblyLocation = null)
        {
            if(assemblyLocation == null)
            {
                assemblyLocation = Assembly.GetEntryAssembly().Location;
            }

            var behavior = GetInstallBehavior(args);

            var parameter = string.Concat(args);
            _configureAction = x => x.ConfigureServiceInstall(context);

            switch (behavior)
            {
                case InstallBehavior.Install:
                    EnsureElevated(args);
                    InstallAssemblyAsService(assemblyLocation);
                    break;
                case InstallBehavior.Uninstall:
                    EnsureElevated(args);
                    UninstallService(assemblyLocation);
                    break;
                case InstallBehavior.TryInstall:
                    EnsureElevated(args);
                    TryInstallAsService(assemblyLocation);
                    break;
            }
        }

        private static InstallBehavior GetInstallBehavior(string[] args) {
            
            var installPatterns = new[] { "-install", "/install", "-i", "/i" };
            if (installPatterns.Any(pattern => args.Contains(pattern))) {
                return InstallBehavior.Install;
            }

            var uninstallPatterns = new[] { "-uninstall", "/uninstall", "-u", "/u" };
            if (uninstallPatterns.Any(pattern => args.Contains(pattern)))
            {
                return InstallBehavior.Uninstall;
            }

            var tryInstallPatterns = new[] { "-tryinstall", "/tryinstall", "-ti", "/ti" };
            if (tryInstallPatterns.Any(pattern => args.Contains(pattern)))
            {
                return InstallBehavior.TryInstall;
            }

            return InstallBehavior.Undefined;
        }

        private static void TryInstallAsService(string assemblyLocation)
        {
            try
            {
                InstallAssemblyAsService(assemblyLocation);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errored while trying to install service. If the service is already installed, this is fine. Error was: " + ex.Message);
                Environment.Exit(0);
            }
        }

        private static void InstallAssemblyAsService(string assemblyLocation)
        {
            ManagedInstallerClass.InstallHelper(new[] { assemblyLocation });
            Console.WriteLine("Installed");
            Environment.Exit(0);
        }

        private static void UninstallService(string assemblyLocation)
        {
            ManagedInstallerClass.InstallHelper(new[] { "/u", assemblyLocation});
            Console.WriteLine("Uninstalled");
            Environment.Exit(0);
        }

        private static void EnsureElevated(string[] args)
        {
            var pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            var hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (hasAdministrativeRight)
            {
                return;
            }

            Console.WriteLine("Requesting elevated rights to perform operation...");

            try
            {
                var processInfo = new ProcessStartInfo
                    {
                        Verb = "runas",
                        FileName = Assembly.GetEntryAssembly().Location,
                        UseShellExecute = true,
                        Arguments = string.Join(" ", args)
                    };

                var p = Process.Start(processInfo);
                p.WaitForExit();
                
                Environment.Exit(0);
            }
            catch (Win32Exception)
            {
                Console.WriteLine("Could not execute installation as administrator. Ensure you have the correct permissions.");
                Environment.Exit(0);
            }
        }

        // Below = class instantiated by the framework installation classes

        private readonly ServiceProcessInstaller _serviceProcessInstaller = new ServiceProcessInstaller();
        private readonly System.ServiceProcess.ServiceInstaller _serviceInstaller = new System.ServiceProcess.ServiceInstaller();

        public SimpleServiceApplication()
        {
            _configureAction(this);
        }

        private void ConfigureServiceInstall(ApplicationContext context)
        {
            _serviceInstaller.ServiceName = Guid.NewGuid().ToString();
            _serviceInstaller.StartType = ServiceStartMode.Manual;
            _serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            context.ConfigureInstall(_serviceInstaller, _serviceProcessInstaller);

            var eventLogInstaller = new EventLogInstaller {Source = _serviceInstaller.ServiceName};

            Installers.AddRange(new Installer[] { _serviceProcessInstaller, _serviceInstaller, eventLogInstaller });
        }

        public static bool IsServiceInstalled(string serviceName)
        {
            var services = ServiceController.GetServices();
            return services.Any(service => service.ServiceName == serviceName);
        }
    }
}
// ReSharper restore ClassNeverInstantiated.Global