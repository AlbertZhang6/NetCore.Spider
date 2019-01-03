using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace NetCore.Security
{
    public class SecurityParameterBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                //request doesn't include this name's model parameter or the model parameter's value is FromBody
                return Task.CompletedTask;
            }
            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            Type type = bindingContext.ModelMetadata.UnderlyingOrModelType;
            ValueSerializerFactory factory = ValueSerializerFactory.GetInstance();
            //ensure the model type is supported
            IBinarySerializer binarySerializer = factory.GetSerializer(type);

            string s = valueProviderResult.FirstValue;
            object value = null;
            if (string.IsNullOrWhiteSpace(s))
            {
                if (type == typeof(string))
                {
                    if (!bindingContext.ModelMetadata.ConvertEmptyStringToNull)
                        value = s;
                }
                else if (!bindingContext.ModelMetadata.IsReferenceOrNullableType)
                {
                    bindingContext.ModelState.TryAddModelError(modelName, "The value should not be empty.");
                    return Task.CompletedTask;
                }
            }
            else
            {
                ISecurityEncryptor service = ServiceProviderServiceExtensions.GetService<ISecurityEncryptor>(bindingContext.HttpContext.RequestServices);
                byte[] payload = service.Decrypt(s);
                if (payload == null)
                {
                    bindingContext.ModelState.TryAddModelError(modelName, "Security parameter's value is invalid.");
                    return Task.CompletedTask;
                }
                value = binarySerializer.Read(payload);
            }

            bindingContext.Result = ModelBindingResult.Success(value);
            return Task.CompletedTask;
        }
    }
}
