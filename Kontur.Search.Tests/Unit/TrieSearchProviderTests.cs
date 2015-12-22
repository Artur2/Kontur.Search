using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontur.Search.Tests.Fixtures;
using Xunit;

namespace Kontur.Search.Tests.Unit
{
    public class TrieSearchProviderTests : IClassFixture<TrieSearchProviderFixture>
    {
        private TrieSearchProviderFixture _fixture;

        public TrieSearchProviderTests(TrieSearchProviderFixture fixture)
        {
            _fixture = fixture;
        }

        public TrieSearchProviderFixture Fixture
        {
            get
            {
                return _fixture;
            }
        }

        [Fact(DisplayName = "Добавление нулл и пустой строки должно вызывать ошибку")]
        public void Add_Empty_And_Null_Entry_Should_Throw_Exception()
        {
            var provider = Fixture.CreateProvider();
            Assert.Throws<ArgumentNullException>(() => provider.AddEntry(string.Empty));
            Assert.Throws<ArgumentNullException>(() => provider.AddEntry(null));
        }

        [Fact(DisplayName = "Проверяем добавление")]
        public void Add_Entry_Should_Work_Clearly()
        {
            var provider = Fixture.CreateProvider();
            provider.AddEntry("0xDEAD");
        }

        [Fact(DisplayName = "Добавляем и ищем")]
        public void Add_And_Search_Must_Return_Added_Value()
        {
            var provider = Fixture.CreateProvider();
            var entry = new { Text = "ASDF", Priority = 1 };

            provider.AddEntry(entry.Text, entry.Priority);

            var searchResults = provider.Search("A", 10);

            Assert.True(searchResults.Any());
        }

        [Fact(DisplayName = "Добавление двух строк и проверка сортировки")]
        public void Add_Entries_And_Return_Values_Must_Have_Correct_Order()
        {
            var provider = Fixture.CreateProvider();

            var entries = new Dictionary<int, string>()
            {
                {1,"0xDEADBEEF" },
                {2, "0xDEAD" }
            };

            foreach (var entry in entries)
            {
                provider.AddEntry(entry.Value, entry.Key);
            }

            var searchResults = provider.Search("0x", 10);

            Assert.True(searchResults.FirstOrDefault() == entries.ElementAt(1).Value);
        }

        [Theory(DisplayName = "Добавляем 100 000 записей и ищем с кол-вом 50 000 результатов")]
        [InlineData("0xDEAD")]
        [InlineData("Diz Nutz")]
        [InlineData("Okay!!!")]
        public void Add_One_Hungred_Thousands_Entries_Then_Search(string prefix)
        {
            var provider = Fixture.CreateProvider();
            var random = new Random();

            var dictionary = new Dictionary<int, string>();

            for (int i = 0; i <= 100000; i++)
            {
                var priority = random.Next(1, int.MaxValue);

                while (dictionary.ContainsKey(priority))
                {
                    priority = random.Next(1, int.MaxValue);
                }

                dictionary.Add(priority, $"{prefix}{priority}");
                provider.AddEntry($"{prefix}{priority}", priority);
            }

            Assert.All(provider.Search($"{prefix}1", 50000), (result) =>
            {
                Assert.True(dictionary.ContainsValue(result));
            });
        }

    }
}
