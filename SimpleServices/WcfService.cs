using System;
using System.ServiceModel;

namespace SimpleServices
{
    public class WcfService<TService> : IWindowsService
    {
        public ApplicationContext AppContext { get; set; }
        private readonly TService _service;
        private readonly Uri _baseAddress;
        private ServiceHost _serviceHost;

    	public WcfService(TService service, Uri baseAddress)
        {
            _service = service;
            _baseAddress = baseAddress;
        }

    	public void Start(string[] args)
    	{
            AppContext.Log("Opening Wcf Host for " + typeof(TService));

			_serviceHost = new ServiceHost(_service, _baseAddress);
            _serviceHost.Open();

            foreach (var se in _serviceHost.Description.Endpoints)
            {
                AppContext.Log(string.Format("Address: {0}, Binding: {1}, ServiceName: {2}", se.Address, se.Binding.Name, se.Contract.Name));
            }
        }

        public void Stop()
        {
            if ((_serviceHost.State == CommunicationState.Closed &&
                 _serviceHost.State == CommunicationState.Closing))
            {
                return;
            }

            try
            {
                _serviceHost.Close(new TimeSpan(0, 0, 2, 30));
            }
            catch
            {
                _serviceHost.Abort();
            }
        }
    }
}