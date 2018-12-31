using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Security
{
    internal class ValueSerializerFactory
    {
        private static readonly ValueSerializerFactory factory;

        private IList<IBinarySerializer> serializers;

        static ValueSerializerFactory()
        {
            List<IBinarySerializer> serializers = new List<IBinarySerializer>()
            {
                new NumberSerializer(),
                new StringSerializer(),
                new GuidSerializer()
            };
            factory = new ValueSerializerFactory(serializers);
        }

        private ValueSerializerFactory(IList<IBinarySerializer> serializers)
        {
            this.serializers = serializers;
        }

        public static ValueSerializerFactory GetInstance()
        {
            return factory;
        }

        public IBinarySerializer GetSerializer(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException(nameof(valueType));

            foreach (IBinarySerializer handler in serializers)
            {
                if (handler.CanHandle(valueType))
                    return handler;
            }
            throw new NotSupportedException($"could not find a suitable binary serializer for type \"{valueType.FullName}\".");
        }
    }
}
