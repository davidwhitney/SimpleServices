namespace SimpleServices
{
    public interface IWindowsService
    {
        ApplicationContext AppContext { get; set; }
        void Start(string[] args);
        void Stop();
    }
}