using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kontur.Search.Providers
{
    public interface ISearchProvider
    {
        void AddEntry([NotNull]string entry);

        [NotNull]
        IEnumerable<string> Search([NotNull]string query, int maxResults);
    }
}
