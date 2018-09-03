using EfScmDataAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EfScmDalTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            using (var context = new EfScmContext())
            {
                var partName = $"Sample {DateTime.Now.ToString("HHmmss")}";
                var part = new PartType()
                {
                    Name = partName
                };

                await context.Parts.AddAsync(part);
                await context.SaveChangesAsync();

                var dbPart = context.Parts.Single(p => p.Name == partName);

                Assert.Equal(dbPart.Name, part.Name);

                context.Parts.Remove(dbPart);
                await context.SaveChangesAsync();
                
                dbPart = context.Parts.FirstOrDefault(p => p.Name == partName);

                Assert.Null(dbPart);
            }
        }
    }
}
