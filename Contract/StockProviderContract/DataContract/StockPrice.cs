namespace StockProviderContract.DataContract
{
    public record StockPrice(string StockName, DateTime Time, double Price)
    {
        public byte[] Serialize()
        {
            using MemoryStream memoryStream = new();
            using BinaryWriter writer = new BinaryWriter(memoryStream);
            writer.Write(StockName);
            writer.Write(Time.Ticks);
            writer.Write(Price);
            return memoryStream.ToArray();
        }
        public static StockPrice Deserialize(byte[] bytes)
        {
            using MemoryStream memoryStream = new MemoryStream(bytes);
            using BinaryReader reader = new BinaryReader(memoryStream);
            return new StockPrice(reader.ReadString(), new DateTime(reader.ReadInt64()), reader.ReadDouble());
        }
    }
}
