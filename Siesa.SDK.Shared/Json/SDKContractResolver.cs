using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Siesa.SDK.Shared.Json
{
    public class SDKContractResolver : DefaultContractResolver
{
    public new static readonly SDKContractResolver Instance = new SDKContractResolver();

    protected override JsonContract CreateContract(Type objectType)
    {
        JsonContract contract = base.CreateContract(objectType);

        // this will only be called once and then cached
        if (objectType == typeof(DateOnly) || objectType == typeof(DateOnly?))
        {
            contract.Converter = new DateOnlyJsonConverter();
        }

        return contract;
    }
}

}
