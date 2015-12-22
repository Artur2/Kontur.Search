using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontur.Search.Providers;
using Moq;

namespace Kontur.Search.Tests.Fixtures
{
    public class SearchManagerFixture
    {
        public SearchManager CreateNew(ISearchProvider provider)
        {
            return new SearchManager(provider);
        }

        public ISearchProvider CreateSearchProviderMoq()
        {
            var searchProviderMoq = new Mock<ISearchProvider>();

            return searchProviderMoq.Object;
        }
    }
}
