using System;
using Labradoratory.DataAccess.ChangeTracking;
using Labradoratory.DataAccess.Mongo.Extensions;
using Mongo2Go;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Xunit;

namespace Labradoratory.DataAccess.Mongo.Test
{
    public class IntegrationTests
    {
        public IntegrationTests()
        {
            // Configure mongo db.   
            Runner = MongoDbRunner.Start();
            Client = new MongoClient(Runner.ConnectionString);
            Database = Client.GetDatabase("TestDatabase");
        }

        private MongoDbRunner Runner { get; }
        private MongoClient Client { get; }
        private IMongoDatabase Database { get; }

        [Fact]
        public void TestUpdateObject()
        {
            var test = new TestObject
            {
                Id = Guid.NewGuid().ToString(),
                IntValue = 123,
                StringValue = "Original"
            };
            test.CommitChanges();
            Assert.False(test.HasChanges);

            var collection = Database.GetCollection<TestObject>(nameof(TestObject));
            collection.InsertOne(test);

            test.StringValue = "Updated";
            test.IntValue = 321;

            var changes = test.GetChangeSet();
            var update = changes.CreateUpdateDefinition<TestObject>();
            var filter = Builders<TestObject>.Filter.Eq(to => to.Id, test.Id);
            collection.UpdateOne(filter, update);

            var fromDb = collection.FindSync(filter).FirstOrDefault();

            Assert.NotNull(fromDb);
            Assert.Equal(test.StringValue, fromDb.StringValue);
            Assert.Equal(test.IntValue, fromDb.IntValue);
        }

        [Fact]
        public void TestUpdateNestedObject()
        {
            Assert.True(false);
        }

        [Fact]
        public void TestUpdateCollection()
        {
            Assert.True(false);
        }

        [Fact]
        public void TestUpdateDictionary()
        {
            Assert.True(false);
        }

        public class TestObject : Entity
        {
            public override object[] GetKeys()
            {
                return ToKeys(Id);
            }

            [BsonId]
            public string Id { get; set; }

            public string StringValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }

            public int IntValue
            {
                get => GetValue<int>();
                set => SetValue(value);
            }
        }
    }
}
