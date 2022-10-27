using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

namespace Siesa.SDK.Shared.Json
{
    public class StreamUtilities
    {
        public static Stream StringToStream(string json)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            return new MemoryStream(byteArray);
        }
        public static byte[] Compress<T>(T setting)
        {
            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new SDKContractResolver()
            });
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            byte[] compressedBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream compressedStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    using (StreamWriter streamWriter = new StreamWriter(compressedStream))
                    {
                        using (JsonWriter writer = new JsonTextWriter(streamWriter))
                        {
                            serializer.Serialize(writer, setting);
                        }
                    }
                    //serializer.Serialize(compressedStream, setting);
                }

                compressedBytes = memoryStream.ToArray();
            }

            return compressedBytes;
        }

        public static T Decompress<T>(Stream compressedStream)
        {
            JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new SDKContractResolver()
            });
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            T restoredData;
            using (GZipStream decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress, true))
            {
                using (StreamReader streamReader = new StreamReader(decompressedStream))
                {
                    using (JsonReader reader = new JsonTextReader(streamReader))
                    {
                        restoredData = serializer.Deserialize<T>(reader);
                    }
                }
                //restoredData = (T)serializer.ReadObject(decompressedStream);
            }

            return restoredData;
        }
    }
}