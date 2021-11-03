using API_Proyecto.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Proyecto.Repositories
{
    public class UserCollection : IUserCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();

        private IMongoCollection<Users> Collection;

        public UserCollection()
        {
            Collection = _repository.db.GetCollection<Users>("Users");
        }
        public async Task AddUser(Users user)
        {
 
            await Collection.InsertOneAsync(user); 
        }

        public async Task<List<Users>> GetAllUsers()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async  Task<Users> GetUserById(string id)
        {
            return await Collection.FindAsync(new BsonDocument { { "_id", new ObjectId(id) } }).Result.FirstAsync();
        }

        public async Task UpdateUser(Users user)
        {
            var filter = Builders<Users>
                .Filter
                .Eq(s => s.id, user.id);
            await Collection.ReplaceOneAsync(filter, user);
        }
    }
}
