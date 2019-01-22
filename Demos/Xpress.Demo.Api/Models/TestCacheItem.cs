using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xpress.Demo.Api.Models
{
    [Serializable]
    public class TestCacheItem
    {
        public string Name { get; private set; }


        public TestCacheItem(string name)
        {
            Name = name;
        }
    }
}
