using MSyics.Traceyi;

namespace MSyics.Cacheyi.Example
{
    class Program : Examplar
    {
        static void Main(string[] args)
        {
            Traceable.Add("logging.json");

            new Program()
                .Add<SetupExample>()

                .Test();

            return;
        }
    }
}
