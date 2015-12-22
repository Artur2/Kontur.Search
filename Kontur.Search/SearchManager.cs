using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kontur.Search.Providers;
using Kontur.Search.Providers.InMemory;

namespace Kontur.Search
{

    /// <summary>
    /// Главный объект по поиску
    /// </summary>
    /// <remarks>
    /// Сама по себе задача имеет довольно далекие от real world требования
    /// Поэтому довольно сложно представить себе гибкую архитектуру
    /// Понравилось как написан Microsoft.AspNet.Identity (https://github.com/aspnet/Identity)
    /// Этот класс написан после изучения этого кода 
    /// </remarks>
    public class SearchManager
    {
        [NotNull]
        private ISearchProvider _searchProvider;

        public SearchManager([NotNull]ISearchProvider searchProvider)
        {
            _searchProvider = searchProvider;
        }

        public virtual ISearchProvider Provider
        {
            [NotNull]
            get
            {
                return _searchProvider;
            }
        }

        [NotNull]
        public static ISearchProvider CreateDefaultProvider()
        {
            return new TrieSearchProvider();
        }

        public virtual bool SupportsPriorityNodes
        {
            get
            {
                ThrowIfProviderNull();

                return _searchProvider is IPrioritySearchProvider;
            }
        }

        public virtual void AddEntry([NotNull]string entry)
        {
            ThrowIfProviderNull();

            Provider.AddEntry(entry);
        }

        public virtual void AddEntry([NotNull]string entry, int priority)
        {
            if (!SupportsPriorityNodes)
                throw new NotSupportedException();

            // ReSharper disable once PossibleNullReferenceException
            (Provider as IPrioritySearchProvider).AddEntry(entry, priority);
        }

        public virtual IEnumerable<string> Search([NotNull]string query, int maxResults)
        {
            ThrowIfProviderNull();

            return Provider.Search(query, maxResults);
        }

        private void ThrowIfProviderNull()
        {
            if (Provider == null)
                throw new NullReferenceException("Provider is null");
        }
    }
}
