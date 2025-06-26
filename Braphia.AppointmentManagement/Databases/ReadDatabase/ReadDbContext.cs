using Braphia.AppointmentManagement.Databases.ReadDatabase.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Braphia.AppointmentManagement.Databases.WriteDatabase
{
    public class ReadDbContext
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;

        public ReadDbContext(string connectionString, string databaseName)
        {
            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(databaseName);
            Map();
        }

        internal IMongoCollection<AppointmentViewQueryModel> AppointmentViews 
        {
            get
            {
                return _database.GetCollection<AppointmentViewQueryModel>("AppointmentViews");
            }
        }

        //Map the AppointmentViewQueryModel to the MongoDB collection
        private void Map()
        {
            BsonClassMap.RegisterClassMap<AppointmentViewQueryModel>(cm =>
            {
                cm.AutoMap();
               
            });
        }
    }
}
