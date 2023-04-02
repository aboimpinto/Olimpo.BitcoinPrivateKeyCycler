using MongoDB.Bson.Serialization.Attributes;

namespace BitcoinPrivateKeyCycler.Service
{
    public class PrivateKeyAddress
    {
        [BsonElement("PrivateKeyBytes")]
        public byte[] PrivateKeyBytes { get; set; }
    }
}