using Parsely;
using Parsely.Test.Extractables;
using Xunit;

namespace Parsely.Test
{
    public class ExtensionTests
    {
        [Fact]
        public void CanCheckForNullProperties()
        {
            var obj = new {
                Name = "Mike",
            };
            
            var car = new Car()
            {
                // Make = "Acura",
                Name = "TL",
                Year = 2012
            }; 

            Assert.False(obj.HasNullProperties());
            Assert.True(car.HasNullProperties());
        }
    }
}