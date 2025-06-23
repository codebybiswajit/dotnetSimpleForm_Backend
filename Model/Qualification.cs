using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Model
{
    public class Qualification1
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        

        [BsonElement("userId")]
        public string UserId { get; set; } = String.Empty;
    }
}
