using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NetCore.Security
{
    internal class NumberSerializer : IBinarySerializer
    {
        public bool CanHandle(Type valueType)
        {
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                case TypeCode.Decimal:
                default:
                    return false;
            }
        }

        public object Read(byte[] bytes)
        {
            TypeCode typeCode = (TypeCode)bytes[0];
            long num = BitConverter.ToInt64(bytes, 1);
            //valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            //long num = BitConverter.ToInt64(bytes, 0);
            switch (typeCode)
            {
                //unbox a value type must be same type when it was boxed
                case TypeCode.SByte:
                    return Convert.ToSByte(num);
                case TypeCode.Byte:
                    return Convert.ToByte(num);
                case TypeCode.Int16:
                    return Convert.ToInt16(num);
                case TypeCode.UInt16:
                    return Convert.ToUInt16(num);
                case TypeCode.Int32:
                    return Convert.ToInt32(num);
                case TypeCode.UInt32:
                    return Convert.ToUInt32(num);
                case TypeCode.Int64:
                    return num;
                case TypeCode.UInt64:
                    return unchecked((ulong)num);
                case TypeCode.Single:
                    double d = BitConverter.Int64BitsToDouble(num);
                    return Convert.ToSingle(d);
                case TypeCode.Double:
                    return BitConverter.Int64BitsToDouble(num);
                default:
                    throw new NotSupportedException();
            }
        }

        public byte[] Write(object value)
        {
            Type type = value.GetType();
            type = (Nullable.GetUnderlyingType(type) ?? type);
            TypeCode typeCode = Type.GetTypeCode(type);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.WriteByte((byte)typeCode);
            byte[] array = null;
            long value2 = 0L;
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                    {
                        value2 = Convert.ToInt64(value);
                        //return BitConverter.GetBytes(num);
                        break;
                    }
                case TypeCode.UInt64:
                    {
                        ulong num = Convert.ToUInt64(value);
                        value2 = (long)num;
                        break;
                    }
                case TypeCode.Single:
                    {
                        float value4 = Convert.ToSingle(value);
                        value2 = BitConverter.SingleToInt32Bits(value4);
                        break;
                    }
                case TypeCode.Double:
                    {
                        double value3 = Convert.ToDouble(value);
                        value2 = BitConverter.DoubleToInt64Bits(value3);
                        break;
                    }
            }
            array = BitConverter.GetBytes(value2);
            memoryStream.Write(array, 0, array.Length);
            return memoryStream.ToArray();
        }
    }

}
