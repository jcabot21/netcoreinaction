using Xunit;
using WidgetScmDataAccess;
using System.Linq;

namespace SqliteScmTest
{
    public class TableCreationTest : IClassFixture<SampleScmDataFixture>
    {
        private readonly SampleScmDataFixture _fixture;
        private readonly ScmContext _context;

        public TableCreationTest(SampleScmDataFixture fixture)
        {
            _fixture = fixture;
            _context = new ScmContext(fixture.Connection);
        }

        [Fact]
        public void DatabaseCreationTest()
        {
            Assert.True(true);
        }

        [Fact]
        public void ReadDataTest()
        {
            Assert.Single(_context.Parts);
            Assert.Equal("8289 L-shaped plate", _context.Parts.First().Name);
        }
    }
}