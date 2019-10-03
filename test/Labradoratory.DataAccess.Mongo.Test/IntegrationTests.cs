using Xunit;

namespace Labradoratory.DataAccess.Mongo.Test
{
    public class IntegrationTests
    {
        public IntegrationTests()
        {
            // Configure mongo db.            
        }

        [Fact]
        public void IntegrationTest()
        {
            CreateDatabase();
            CreateCollection();
            AddEntityToCollection();
        }

        private void CreateDatabase()
        {

        }

        private void CreateCollection()
        {

        }

        private void AddEntityToCollection()
        {

        }
    }
}
