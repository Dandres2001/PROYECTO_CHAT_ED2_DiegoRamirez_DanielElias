using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Models;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Repositories
{
    public class UserCollection : IUserCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();

        private IMongoCollection<Users> Collection;

        public UserCollection()
        {
            Collection = _repository.database.GetCollection<Users>("Users");
        }

        public void createUser(Users user)
        {
            Collection.InsertOneAsync(user);
        }

        public List<Users> GetAllUsers()
        {
            var query = Collection.Find(new BsonDocument()).ToListAsync();

            return query.Result;
        }
    }
}
