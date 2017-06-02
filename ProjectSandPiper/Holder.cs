using System;

namespace ThrushSoftware.SandPiper
{
    [Serializable]
    public class Holder<T> : Serializible
    {
        public T Item;

        public Holder()
        {

        }
        public Holder(T item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            return Item.ToString();
        }
    }
}
