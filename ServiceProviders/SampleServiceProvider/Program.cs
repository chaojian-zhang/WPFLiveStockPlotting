namespace SampleServiceProvider
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var server = new SampleServiceDataProvider();
            server.Initialize();
            server.Start();
        }
    }
}
