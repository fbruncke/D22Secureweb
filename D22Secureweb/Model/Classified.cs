using MongoDB.Bson.Serialization.Attributes;

namespace D22Secureweb.Model
{
    public class Classified
    {
        [BsonId]
        public int Id { get; set; }

        public int ClassifiedID { get; set; }

        public string? SecretData { get; set; }

        public int Version { get; set; }
    }
}
