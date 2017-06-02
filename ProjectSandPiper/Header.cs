using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.IO;

namespace ThrushSoftware.SandPiper
{
    public delegate void HeaderRecievedCallBack(params Serializible[] args);

    public class Header
    {
        public const string END = "END_STREAM";
        public const string SEP = "SEPERATOR";
        public static Header ID { get { return new Header("ID", typeof(Holder<int>)); } }
        public static Header PICTURE { get { return new Header("PICTURE", typeof(Holder<Bitmap>)); } }
        public HeaderRecievedCallBack CallBack;

        public string Head;
        public List<Type> Format = new List<Type>();

        public Header()
        {

        }
        public Header(string Head, params Type[] Format)
        {
            this.Head = Head;
            this.Format.AddRange(Format);
        }

        //Make more efficient? to lazy to do it now
        public byte[] Fill(params Serializible[] items)
        {
            string[] strings = new string[items.Length];
            string data = Head;

            //Get data for each ITEM in items
            for (int i = 0; i < items.Length - 1; i++)
            {
                string s = "";
                foreach (byte b in items[i].GetBytes())
                    s += (char)b;
                data += s + SEP;
            }
            string x = "";
            foreach (byte b in items[items.Length - 1].GetBytes())
                x += (char)b;
            data += x + END;

            byte[] bytes = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                bytes[i] = (byte)data[i];
            return bytes;
        }
        public bool Compatible(string s)
        {
            return s.StartsWith(Head);
        }

        public List<Serializible> DeserializeHeader(string s)
        {
            string current = s.Substring(this.Head.Length);

            current = current.Substring(0, current.IndexOf(END));
            string[] objects = Regex.Split(current, SEP);

            List<Serializible> rets = new List<Serializible>();

            for(int j = 0; j < objects.Length; j++)
            {
                byte[] bytes = new byte[objects[j].Length];

                for (int i = 0; i < objects[j].Length; i++)
                {
                    bytes[i] = (byte)objects[j][i];                    
                }

                BinaryFormatter f = new BinaryFormatter();
                using(MemoryStream stream = new MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    //Must reset the position to zero other wise it will fail!
                    stream.Position = 0;
                    rets.Add((Serializible)f.Deserialize(stream));
                }
            }
            return rets;
        }
    }
}
