using ElpisOpcServer.Models;
using System;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using ElpisOpcServer.SunPowerGen;

namespace ElpisOpcServer.SocketService.Net.Formatters
{
    #region Using Memory stream
    public class ByteWriter : IPacketWriter
    {


        protected readonly MemoryStream Writer;
        private readonly MemoryStream JsonBuffer;
        // private readonly JsonTextWriter JsonWriter;
        private readonly Utf8JsonWriter JsonWriter;
        protected Memory<byte> Memory;
        //protected byte[] Memory;
        protected int BytesPending;

        public ByteWriter()
        {
            Writer = new MemoryStream();
            // Writer = new byte[1024]; // Initialize the buffer with a suitable size
            JsonBuffer = new MemoryStream();
            JsonWriter = new Utf8JsonWriter(JsonBuffer);
            Memory = new Memory<byte>();
        }

        public int Length => BytesPending;
        public void Reset(int size)
        {
            BytesPending = 0;
            //Writer.SetLength(size);
           // Writer.Seek(0, SeekOrigin.Begin);
            Memory = new byte[size];
        }
        public void Clear()
        {
            BytesPending = 0;
            //Writer.SetLength(0);
            Writer.Seek(0, SeekOrigin.Begin);
        }
        public byte[] GetBytes()
        {
            return Writer.ToArray();
        }
        protected void GrowIfNeeded(int length)
        {

            if (Memory.Length - BytesPending <length)
            {
                
                Writer.SetLength(length * 2);
                var newMemory = Writer.GetBuffer();
                Memory.CopyTo(newMemory);
                Memory = newMemory;
            }
            //if (Memory.Length - BytesPending < length)
            //{
            //    var newMemory = new byte[Memory.Length * 2];
            //    Array.Copy(Memory, newMemory, BytesPending);
            //    Memory = newMemory;
            //}
        }
        public virtual byte[] Encode(MessagePacket value)
        {
            ReadOnlyMemory<byte> message = GetMessage(value.Message);
            int length = message.Length;
            var id = value.Id;
            Clear();
            GrowIfNeeded(length + 6);
            var span = Memory.Span;
            span[BytesPending++] = (byte)value.Type;
            span[BytesPending++] = (byte)(length & 0xFF);
            span[BytesPending++] = (byte)(length >> 8);
            span[BytesPending++] = (byte)((int)value.Quality | ((value.Retain ? 1 : 0) << 4));
            span[BytesPending++] = (byte)(id & 0xFF);
            span[BytesPending++] = (byte)(id >> 8);
            //message.Span.CopyTo(new Span<byte>(Memory, BytesPending, length));
            message.CopyTo(Memory.Slice(BytesPending));
            BytesPending += length;
            ////// Here also i need to adda line compe the device api
            return Writer.ToArray();

            //Memory[BytesPending++] = (byte)value.Type;
            //Memory[BytesPending++] = (byte)(length & 0xFF);
            //Memory[BytesPending++] = (byte)(length >> 8);
            //Memory[BytesPending++] = (byte)((int)value.Quality | ((value.Retain ? 1 : 0) << 4));
            //Memory[BytesPending++] = (byte)(id & 0xFF);
            //Memory[BytesPending++] = (byte)(id >> 8);
            //message.Span.CopyTo(new Span<byte>(Memory, BytesPending, length));
            //BytesPending += length;
            //Writer.Write(Memory, 0, BytesPending);
            //return Writer.ToArray();
        }

        protected ReadOnlyMemory<byte> GetMessage(JsonElement element)
        {
            // int len = element.GetArrayLength();
            JsonBuffer.SetLength(Pump_Test.packetlength);
            JsonBuffer.Capacity = Pump_Test.packetlength;
            JsonBuffer.Seek(0, SeekOrigin.Begin);
            JsonWriter.Reset();
            element.WriteTo(JsonWriter);
            JsonWriter.Flush();
            return JsonBuffer.GetBuffer();
            //return new ReadOnlyMemory<byte>(byteArray, 0, leangth); ;//JsonBuffer.GetBuffer();
            //return GetReadOnlyMemory(JsonBuffer);//GetBytesFromJsonBuffer(JsonBuffer);
        }
        private ReadOnlyMemory<byte> GetReadOnlyMemory(MemoryStream memoryStream)
        {
            byte[] byteArray = memoryStream.GetBuffer(); // Get the underlying byte array
            int length = (int)memoryStream.Length; // Get the actual length of the data in the MemoryStream
            return new ReadOnlyMemory<byte>(byteArray, 0, length);
        }
        private ReadOnlyMemory<byte> GetBytesFromJsonBuffer(StringWriter jsonBuffer)
        {
            string jsonString = jsonBuffer.ToString();
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new ReadOnlyMemory<byte>(jsonBytes);
        }

    }
    #endregion Using Memory stream

    #region Using List<byte> library
    //public class ByteWriter : IPacketWriter
    //{
    //    protected readonly List<byte> Writer;
    //    private readonly List<byte> JsonBuffer;
    //    private readonly Utf8JsonWriter JsonWriter;
    //    protected int BytesPending;
    //   // protected readonly byte[] writte1;
    //    public ByteWriter()
    //    {
    //        //writte1 = new byte[2048];
    //        Writer = new List<byte>();
    //        JsonBuffer = new List<byte>();
    //        JsonWriter = new Utf8JsonWriter(new MemoryStream(JsonBuffer.ToArray()));
    //    }

    //    public int Length => BytesPending;

    //    public void Reset(int size)
    //    {

    //        BytesPending = 0;
    //        //for (int i = 0; i < Writer.Count; i++)
    //        //{
    //        //    Writer[i] = 0;
    //        //}
    //        Writer.Clear();
    //        Writer.Capacity = size;
    //    }

    //    public void Clear()
    //    {
    //        BytesPending = 0;
    //        Writer.Clear();
    //    }

    //    public byte[] GetBytes()
    //    {

    //        return Writer.ToArray();
    //    }

    //    //protected void GrowIfNeeded(int length)
    //    //{
    //    //    if (Writer.Capacity - Writer.Count < length)
    //    //    {
    //    //        Writer.Capacity = Writer.Count + length;
    //    //    }
    //    //}
    //    protected void GrowIfNeeded(int length)
    //    {
    //        if (Writer.Count - BytesPending < length)
    //        {
    //            /// old code working
    //            var newmemory = Writer.Count * 2;
    //            Writer.Add((byte)newmemory);

    //            /// new code memory increment
    //            //var newmemory = length * 2;
    //            //Writer.Add((byte)newmemory);
    //        }

    //    }
    //    public virtual byte[] Encode(MessagePacket value)
    //    {
    //        ReadOnlyMemory<byte> message = GetMessage(value.Message);
    //        int length = message.Length;
    //        var id = value.Id;
    //        Clear();
    //        GrowIfNeeded(length + 6);
    //        Writer.Add((byte)value.Type);
    //        Writer.Add((byte)(length & 0xFF));
    //        Writer.Add((byte)(length >> 8));
    //        Writer.Add((byte)((int)value.Quality | ((value.Retain ? 1 : 0) << 4)));
    //        Writer.Add((byte)(id & 0xFF));
    //        Writer.Add((byte)(id >> 8));
    //        while (Writer.Count < BytesPending + length)
    //        {
    //            GrowIfNeeded(length + 2);
    //        }
    //        // message.Span.CopyTo(Writer.ToArray(), BytesPending);
    //        message.Span.CopyTo(Writer.GetRange(BytesPending, length).ToArray());
    //        BytesPending += length;
    //        return GetBytes();
    //    }

    //    protected ReadOnlyMemory<byte> GetMessage(JsonElement element)
    //    {
    //        JsonBuffer.Clear();
    //        JsonWriter.Reset();
    //       // element.WriteTo(JsonWriter);
    //        JsonBuffer.AddRange(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(element));
    //        JsonWriter.Flush();
    //        return JsonBuffer.ToArray();
    //    }
    //}

    #endregion Using List<byte> library



    #region Using byte[] library
    //public class ByteWriter : IPacketWriter
    //{

    //    protected static byte[] Writer;
    //    private static byte[] JsonBuffer;
    //    private static Utf8JsonWriter JsonWriter;
    //    protected int BytesPending;
    //    //Pump_Test Packetlength = new Pump_Test();
    //    //public int lent;
    //    //public ByteWriter()
    //    //{

    //    //}
    //    public ByteWriter()
    //    {
    //        //Writer = new byte[268]; // Set the initial size as per your requirement
    //        //JsonBuffer = new byte[268];
    //        Writer = new byte[Pump_Test.packetlength]; // Set the initial size as per your requirement
    //        JsonBuffer = new byte[Pump_Test.packetlength];
    //        //Writer = new byte[length]; // Set the initial size as per your requirement
    //        //JsonBuffer = new byte[length];
    //        JsonWriter = new Utf8JsonWriter(new MemoryStream(JsonBuffer));
    //    }

    //    public int Length => BytesPending;

    //    public void Reset(int size)
    //    {
    //        BytesPending = 0;
    //        Array.Clear(Writer, 0, Writer.Length);
    //    }

    //    public void Clear()
    //    {
    //        BytesPending = 0;
    //        Array.Clear(Writer, 0, Writer.Length);
    //    }

    //    public byte[] GetBytes()
    //    {
    //        byte[] result = new byte[BytesPending];
    //        Array.Copy(Writer, result, BytesPending);
    //        return result;
    //    }

    //    protected void GrowIfNeeded(int length)
    //    {
    //        if (Writer.Length - BytesPending < length)
    //        {
    //            int newCapacity = Math.Max(Writer.Length * 2, BytesPending + length);
    //            Array.Resize(ref Writer, newCapacity);
    //        }
    //    }

    //    public virtual byte[] Encode(MessagePacket value)
    //    {
    //        ReadOnlyMemory<byte> message = GetMessage(value.Message);
    //        int length = message.Length;
    //        var id = value.Id;
    //        Clear();
    //        GrowIfNeeded(length + 6);
    //        Writer[BytesPending++] = (byte)value.Type;
    //        Writer[BytesPending++] = (byte)(length & 0xFF);
    //        Writer[BytesPending++] = (byte)(length >> 8);
    //        Writer[BytesPending++] = (byte)((int)value.Quality | ((value.Retain ? 1 : 0) << 4));
    //        Writer[BytesPending++] = (byte)(id & 0xFF);
    //        Writer[BytesPending++] = (byte)(id >> 8);
    //        while (Writer.Length < BytesPending + length/*BytesPending < length*/)
    //        {
    //            GrowIfNeeded(length + 2);
    //        }
    //        message.Span.CopyTo(Writer.AsSpan(BytesPending));
    //        BytesPending += length;
    //        return GetBytes();
    //    }

    //    protected ReadOnlyMemory<byte> GetMessage(JsonElement element)
    //    {

    //        try
    //        {

    //            Array.Clear(Writer, 0, Writer.Length);
    //            JsonWriter.Reset();
    //           // JsonBuffer = new byte[Writer.Length];
    //           // JsonWriter = new Utf8JsonWriter(new MemoryStream(Writer.Length));
    //            element.WriteTo(JsonWriter);
    //            JsonWriter.Flush();
    //            return JsonBuffer;

    //        }
    //        catch (Exception ex)
    //        {
    //           
    //            throw;
    //        }

    //        //using (MemoryStream stream = new MemoryStream(JsonBuffer))
    //        //{
    //        //    JsonWriter.Reset(stream);
    //        //    //JsonWriter.WriteJsonElement(element);
    //        //    element.WriteTo(JsonWriter);
    //        //}
    //        //return new ReadOnlyMemory<byte>(JsonBuffer, 0, (int)JsonWriter.BytesCommitted);
    //    }
    //}
    #endregion Using byte[] library
}
