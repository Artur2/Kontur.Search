using JetBrains.Annotations;

namespace Kontur.Search.Providers
{
    public interface IPrioritySearchProvider : ISearchProvider
    {
        void AddEntry([NotNull]string entry, int priority);
    }
}
