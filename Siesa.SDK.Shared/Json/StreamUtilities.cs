using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using System.Text;

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
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            byte[] compressedBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream compressedStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    serializer.WriteObject(compressedStream, setting);
                }

                compressedBytes = memoryStream.ToArray();
            }

            return compressedBytes;
        }

        public static T Decompress<T>(Stream compressedStream)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            T restoredData;
            using (GZipStream decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress, true))
            {
                restoredData = (T)serializer.ReadObject(decompressedStream);
            }

            return restoredData;
        }
    }
}