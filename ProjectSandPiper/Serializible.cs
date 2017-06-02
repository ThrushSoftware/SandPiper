using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ThrushSoftware.SandPiper
{
    [Serializable]
    public abstract class Serializible
    {
        public byte[] GetBytes()
        {
            BinaryFormatter f = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                f.Serialize(stream, this);
                stream.Position = 0;
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }
    }
}
