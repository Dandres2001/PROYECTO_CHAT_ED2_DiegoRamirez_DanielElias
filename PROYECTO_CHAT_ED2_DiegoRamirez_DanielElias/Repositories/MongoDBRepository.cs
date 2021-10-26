using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Repositories
{
    public class MongoDBRepository
    {

        public MongoClient clientMongo;

        public IMongoDatabase database;

        public MongoDBRepository()
        {
            clientMongo = new MongoClient("mongodb://localhost:27017");

            database = clientMongo.GetDatabase("BaseDeDatos");
        }
    }
}
