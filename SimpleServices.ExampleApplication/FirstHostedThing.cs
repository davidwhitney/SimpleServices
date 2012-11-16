namespace SimpleServices.ExampleApplication
{
    public class FirstHostedThing : IWindowsService
    {
        public ApplicationContext AppContext { get; set; }

        public void Start(string[] args)
        {
            AppContext.Log("FirstHostedThing Starting up");
        }

        public void Stop()
        {
            AppContext.Log("FirstHostedThing Stopping up");
        }

    }
}