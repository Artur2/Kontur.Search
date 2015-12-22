using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontur.Search.Providers.InMemory;

namespace Kontur.Search.Tests.Fixtures
{
    public class TrieSearchProviderFixture
    {
        public TrieSearchProvider CreateProvider()
        {
            return new TrieSearchProvider();
        }
    }
}
