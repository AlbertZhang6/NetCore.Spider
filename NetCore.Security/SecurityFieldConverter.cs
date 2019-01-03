using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace NetCore.Security
{
    internal class SecurityFieldConverter : JsonConverter
    {
        private class ModelStateError
        {
        }

        private IBinarySerializer binarySerializer;

        public SecurityFieldConverter(IBinarySerializer binarySerializer)
            : base()
        {
            this.binarySerializer = binarySerializer;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            byte[] payload = binarySerializer.Write(value);
            ISecurityEncryptor encryptor = SecurityEncryptorFactory.GetEncryptor();
            string text = encryptor.Encrypt(payload);
            if (text == null)
            {
                writer.WriteNull();
                return;
            }
            JValue val = JValue.CreateString(text);
            val.WriteTo(writer, Array.Empty<JsonConverter>());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            else if (reader.TokenType != JsonToken.String)
                return new ModelStateError();

            JToken val = JToken.Load(reader);
            string text = val.ToObject<string>(serializer);
            if (string.Empty.Equals(text))
            {
                return string.Empty;
            }
            ISecurityEncryptor encryptor = SecurityEncryptorFactory.GetEncryptor();
            byte[] payload = encryptor.Decrypt(text);
            if (payload == null)
            {
                return new ModelStateError();
            }
            return binarySerializer.Read(payload);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }

}
