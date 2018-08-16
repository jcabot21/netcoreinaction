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
        public void ReadPartDataTest()
        {
            Assert.Single(_context.Parts);

            var part = _context.Parts.First();

            Assert.Equal("8289 L-shaped plate", part.Name);
            Assert.Equal(1, part.Id);
        }

        [Fact]
        public void ReadInventoryDataTest()
        {
            Assert.Single(_context.Inventory);
            
            var part = _context.Parts.First();
            var inventory = _context.Inventory.First();

            Assert.Equal(part.Id, inventory.ParTypeId);
            Assert.Equal(100, inventory.Count);
            Assert.Equal(10, inventory.OrderThreshold);
        }
    }
}