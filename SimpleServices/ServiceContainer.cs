using System;
using System.ServiceProcess;

namespace SimpleServices
{
    public class ServiceContainer : ServiceBase, IWindowsService
    {
        public ApplicationContext AppContext { get; set; }
        private readonly IWindowsService[] _unitOfCode;
        private readonly System.ComponentModel.IContainer _components;

		public ServiceContainer(IWindowsService[] unitOfCode)
        {
			if (unitOfCode == null) throw new ArgumentNullException("unitOfCode");

            _unitOfCode = unitOfCode; 
            _components = new System.ComponentModel.Container();
        }

    	public void Start(string[] args)
    	{
			OnStart(args);
        }


        protected override void OnStart(string[] args)
		{
			foreach(var unit in _unitOfCode)
			{
                AppContext.Log("Starting: " + unit);
				unit.Start(args);
			}
		}

        protected override void OnStop()
		{
			foreach (var unit in _unitOfCode)
			{
                AppContext.Log("Stopping: " + unit);
				unit.Stop();
			}
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}