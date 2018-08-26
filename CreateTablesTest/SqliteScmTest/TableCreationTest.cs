using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Threading.Tasks;
using WidgetScmDataAccess;
using Xunit;

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

            Assert.Equal(part.Id, inventory.PartTypeId);
            Assert.Equal(100, inventory.Count);
            Assert.Equal(10, inventory.OrderThreshold);
        }

        [Fact]
        public async Task TestPartCommands()
        {
            var item = _context.Inventory.First();
            var startCount = item.Count;

            await _context.CreatePartCommand(new PartCommand()
            {
                PartTypeId = item.PartTypeId,
                PartCount = 10,
                Command = PartCountOperation.Add
            });

            await _context.CreatePartCommand(new PartCommand()
            {
                PartTypeId = item.PartTypeId,
                PartCount = 5,
                Command = PartCountOperation.Remove
            });

            var inventory = new Inventory(_context);

            await inventory.UpdateInventory();

            Assert.Equal(startCount + 5, item.Count);
        }

        [Fact]
        public async Task TestCreateOrderTransaction()
        {
            var placedDate = DateTime.Now;
            var supplier = _context.Suppliers.First();
            var order = new Order()
            {
                PartTypeId = supplier.PartTypeId,
                SupplierId = supplier.Id,
                PartCount = 10,
                PlacedDate = placedDate
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await _context.CreateOrder(order));

            using (var command = new SqliteCommand(
                @"SELECT Count(*) FROM [Order] WHERE
                SupplierId=@supplierId AND
                PartTypeId=@partTypeId AND
                PlacedDate=@placedDate AND
                PartCount=10 AND
                FulfilledDate IS NULL", _fixture.Connection
            ))
            {
                // TODO: Add Parameters
                Assert.Equal(0, (long) await command.ExecuteScalarAsync());
            }
        }

        [Fact]
        public async Task TestUpdateInventory()
        {
            var item = _context.Inventory.First();
            var totalCount = item.Count;

            await _context.CreatePartCommand(new PartCommand()
            {
                PartTypeId = item.PartTypeId,
                PartCount = totalCount,
                Command = PartCountOperation.Remove
            });

            var inventory = new Inventory(_context);

            await inventory.UpdateInventory();

            var order = (await _context.GetOrders()).FirstOrDefault(
                o => o.PartTypeId == item.PartTypeId && !o.FulfilledDate.HasValue
            );

            Assert.NotNull(order);

            await _context.CreatePartCommand(new PartCommand()
            {
                PartTypeId = item.PartTypeId,
                PartCount = totalCount,
                Command = PartCountOperation.Add
            });

            await inventory.UpdateInventory();

            Assert.Equal(totalCount, item.Count);
        }
    }
}