using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics
{
    abstract class Examplar
    {
        private List<IExample> Examples { get; } = new List<IExample>();
        public Examplar Add<T>() where T : IExample, new()
        {
            Examples.Add(new T());
            return this;
        }

        public void Test()
        {
            foreach (var item in Examples)
            {
                item.Setup();
                item.Test();
            }
        }
    }
}
