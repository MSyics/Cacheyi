using MSyics.Traceyi;
using System;
using System.Threading.Tasks;

namespace MSyics.Cacheyi
{
    class Program : Examplar
    {
        static void Main(string[] args)
        {
            new Program()
                .Add<SetupExample>()

                .Add<_Example>()

                .Test();
        }
    }
}
