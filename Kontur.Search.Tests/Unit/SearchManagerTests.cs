using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kontur.Search.Tests.Fixtures;
using Xunit;

namespace Kontur.Search.Tests.Unit
{
    public class SearchManagerTests : IClassFixture<SearchManagerFixture>
    {
        private SearchManagerFixture fixture;

        public SearchManagerTests(SearchManagerFixture fixture)
        {
            this.fixture = fixture;
        }

        public SearchManagerFixture Fixture
        {
            get
            {
                return fixture;
            }
        }

        [Fact(DisplayName = "Пытамся добавить строку с приоритетом для нерасчитанного провайдера")]
        public void Trying_To_Add_Entry_With_Priority_Without_Support_Should_Throw_Exception()
        {
            var provider = Fixture.CreateSearchProviderMoq();
            var searchManager = Fixture.CreateNew(provider);

            Assert.Throws<NotSupportedException>(() => searchManager.AddEntry("TEST", 1));
        }

        [Fact(DisplayName = "Пытаемся искать с пустым провайдером")]
        public void Create_Manager_With_Null_Provider_Should_Throw_Exception()
        {
            var searchManager = Fixture.CreateNew(null);

            Assert.Throws<NullReferenceException>(() => searchManager.Search("TEST", 1));
        }
    }
}
