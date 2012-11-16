namespace SimpleServices.ExampleApplication
{
    public class SecondHostedThing : IWindowsService
    {
        public ApplicationContext AppContext { get; set; }

        public void Start(string[] args)
        {
            AppContext.Log("SecondHostedThing Starting up");
        }

        public void Stop()
        {
            AppContext.Log("SecondHostedThing Stopping up");
        }
    }
}