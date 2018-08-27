using System;
using System.Linq;
using System.Threading.Tasks;
using ScmDataAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SqliteScmTest
{
    public class UnitTest1 : IClassFixture<SampleScmDataFixture>
    {
        private readonly SampleScmDataFixture _fixture;
        private readonly IScmContext _context;

        public UnitTest1(SampleScmDataFixture fixture) => 
            (_fixture, _context) = (fixture, fixture.Services.GetRequiredService<IScmContext>());

        [Fact]
        public async Task Test1()
        {
            var orders = await _context.GetOrders();

            Assert.Equal(0, orders.Count());

            var supplier = _context.Suppliers.First();
            var part = _context.Parts.First();
            var order = new Order()
            {
                SupplierId = supplier.Id,
                Supplier = supplier,
                PartTypeId = part.Id,
                Part = part,
                PartCount = 10,
                PlacedDate = DateTime.Now
            };

            await _context.CreateOrder(order);

            Assert.NotEqual(0, order.Id);

            orders = await _context.GetOrders();

            Assert.Single(orders);
        }
    }
}
