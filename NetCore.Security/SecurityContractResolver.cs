using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace NetCore.Security
{
    public class SecurityContractResolver : DefaultContractResolver
    {
        public SecurityContractResolver()
        : base()
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
            if (jsonProperty.Ignored)
                return jsonProperty;

            if (member.IsDefined(typeof(SecurityFieldAttribute), true))
            {
                Type memberType = null;
                if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo property = member as PropertyInfo;
                    memberType = property.PropertyType;
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    FieldInfo field = member as FieldInfo;
                    memberType = field.FieldType;
                }

                if (memberType != null)
                {
                    ValueSerializerFactory instance = ValueSerializerFactory.GetInstance();
                    IBinarySerializer serializer = instance.GetSerializer(memberType);
                    SecurityFieldConverter converter = new SecurityFieldConverter(serializer);
                    jsonProperty.Converter = converter;
                }
            }
            return jsonProperty;
        }
    }
}
