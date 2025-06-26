using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Model
{
    // public enum UserRole
    // {
    //     admin,
    //     user
    // }
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public string Name { get; set; } = String.Empty;

        [BsonElement("phno")]
        public string PhoneNumber { get; set; } = String.Empty;


        [BsonElement("user_name")]
        public string UserName { get; set; } = String.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = String.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = String.Empty;
        [BsonElement("confirmpassword")]
        public string ConfirmPassword { get; set; } = String.Empty;

        [BsonElement("dob")]
        public string Dob { get; set; } = String.Empty;
        [BsonIgnoreIfNull]
        public List<Qualification> Qualified { get; set; } = new List<Qualification>();
        [BsonIgnoreExtraElements]
        public class Qualification
        {
            [BsonElement("clgName"), BsonIgnoreIfNull]
            public string CollegeName { get; set; } = String.Empty;

            [BsonElement("Degree")]
            public string Degree { get; set; } = String.Empty;

            [BsonElement("year")]
            public int Year { get; set; } = 2005;

            [BsonElement("percentage")]
            public string Percentage { get; set; } = String.Empty;
        }
        [BsonElement("role")]
        // [BsonRepresentation(BsonType.String)]
        // public UserRole Role { get; set; } = UserRole.user;
        public string Role { get; set; } = "user";
        [BsonElement("createdAt")]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString();

    }
}
