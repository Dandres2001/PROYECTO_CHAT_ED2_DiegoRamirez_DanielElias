using API_Proyecto.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Proyecto.Repositories
{
    public class ChatRoomCollection : IChatRoomCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();

        private IMongoCollection<ChatRoom> Collection;

        public ChatRoomCollection()
        {
            Collection = _repository.db.GetCollection<ChatRoom>("ChatRooms");
        }
 
        public async Task<List<ChatRoom>> GetAllChatRooms()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<ChatRoom> GetChatRoomById(string id)
        {
            return await Collection.FindAsync(new BsonDocument { { "_id", new ObjectId(id) } }).Result.FirstAsync();
        }

        public async Task NewChatroom(ChatRoom chat)
        {
            await Collection.InsertOneAsync(chat);
        }

        public async Task EditChat(ChatRoom chat)
        {
            var filter = Builders<ChatRoom>
              .Filter
              .Eq(s => s.id, chat.id);
            await Collection.ReplaceOneAsync(filter, chat);
        }

    }
}
