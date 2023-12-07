using ElpisOpcServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows;
using ElpisOpcServer.SunPowerGen;

namespace ElpisOpcServer.SocketService.Net.Formatters
{
    #region Using Memory stream
    public class CharWriter : ByteWriter
    {
        static byte GetHexValue(int i)
        {
            return i < 10 ? (byte)(i + 48) : (byte)(i + 87);
        }

        public void WriteByte(byte value)
        {

            var span = Memory.Span;                                                             ////// here we changed Memory ===> to Memory.Span
            GrowIfNeeded(2);
            if (value < 10)
            {
                span[BytesPending++] = 48;
                span[BytesPending++] = (byte)(value + 48);
                return;
            }
            span[BytesPending++] = GetHexValue(value / 16);
            span[BytesPending++] = GetHexValue(value % 16);
        }
        public void WriteUInt16(ushort value)
        {
            var span = Memory.Span;
            GrowIfNeeded(4);
            if (value < 10)
            {
                span[BytesPending++] = 48;
                span[BytesPending++] = 48;
                span[BytesPending++] = 48;
                span[BytesPending++] = (byte)(value + 48);
                return;
            }
            if (value < 256)
            {
                span[BytesPending++] = 48;
                span[BytesPending++] = 48;
                span[BytesPending++] = GetHexValue(value / 16);
                span[BytesPending++] = GetHexValue(value % 16);
                return;
            }
            span[BytesPending++] = GetHexValue(value / 4096);
            span[BytesPending++] = GetHexValue(value / 256 % 16);
            span[BytesPending++] = GetHexValue(value / 16 % 16);
            span[BytesPending++] = GetHexValue(value % 16);
        }
        public void Write(byte[] value)
        {
            GrowIfNeeded(value.Length);
            value.AsMemory().CopyTo(Memory.Slice(BytesPending));
            BytesPending += value.Length;
            // .net core
            // value.AsMemory().CopyTo(Memory.Slice(BytesPending));
            // .net framework 4.5.2                                                             //////// Here we changed
            //Buffer.BlockCopy(value, 0, Memory, BytesPending, value.Length);
            //BytesPending += value.Length;
        }
        public override byte[] Encode(MessagePacket value)
        {
            ReadOnlyMemory<byte> message = GetMessage(value.Message);
            ushort length = (ushort)message.Length;
            Clear();
            WriteByte((byte)value.Type);
            WriteUInt16(length);
            WriteByte((byte)((int)value.Quality | ((value.Retain ? 1 : 0) << 4)));
            WriteUInt16(value.Id);
            while (Memory.Length < BytesPending + length)
            {
                GrowIfNeeded(length);
            }
            message.CopyTo(Memory.Slice(BytesPending));                                     ////// here also we changed two lines
            BytesPending += length;
            // .net core
            // message.CopyTo(Memory.Slice(BytesPending));
            //.net framework 4.5.2
            //message.Span.CopyTo(new Span<byte>(Memory, BytesPending, length));
            //BytesPending += length;
            //Writer.Write(Memory, 0, BytesPending);
            return Memory.ToArray();

        }

    }
    #endregion Using Memory stream




    #region Using list<byte> library

    //public class CharWriter : ByteWriter
    //{
    //    static byte GetHexValue(int i)
    //    {
    //        return i < 10 ? (byte)(i + 48) : (byte)(i + 87);
    //    }

    //    public void WriteByte(byte value)
    //    {
    //        GrowIfNeeded(2);
    //        if (value < 10)
    //        {
    //            Writer.Add(48);
    //            Writer.Add((byte)(value + 48));
    //            return;
    //        }
    //        Writer.Add(GetHexValue(value / 16));
    //        Writer.Add(GetHexValue(value % 16));
    //    }

    //    public void WriteUInt16(ushort value)
    //    {
    //        GrowIfNeeded(4);
    //        if (value < 10)
    //        {
    //            Writer.Add(48);
    //            Writer.Add(48);
    //            Writer.Add(48);
    //            Writer.Add((byte)(value + 48));
    //            return;
    //        }
    //        if (value < 256)
    //        {
    //            Writer.Add(48);
    //            Writer.Add(48);
    //            Writer.Add(GetHexValue(value / 16));
    //            Writer.Add(GetHexValue(value % 16));
    //            return;
    //        }
    //        Writer.Add(GetHexValue(value / 4096));
    //        Writer.Add(GetHexValue(value / 256 % 16));
    //        Writer.Add(GetHexValue(value / 16 % 16));
    //        Writer.Add(GetHexValue(value % 16));
    //    }

    //    public void Write(byte[] value)
    //    {
    //        Writer.AddRange(value);
    //    }

    //    public override byte[] Encode(MessagePacket value)
    //    {
    //        try
    //        {
    //            ReadOnlyMemory<byte> message = GetMessage(value.Message);
    //            ushort length = (ushort)message.Length;
    //            Clear();
    //            WriteByte((byte)value.Type);
    //            WriteUInt16(length);
    //            WriteByte((byte)((int)value.Quality | ((value.Retain ? 1 : 0) << 4)));
    //            WriteUInt16(value.Id);
    //            while (Writer.Count < BytesPending + length)
    //            {
    //                GrowIfNeeded(length);
    //            }
    //            // message.Span.CopyTo(Writer.GetSpan(BytesPending, length));
    //            message.Span.CopyTo(Writer.GetRange(BytesPending, length).ToArray());
    //            BytesPending += length;
    //            return GetBytes();
    //        }
    //        catch (Exception ex)
    //        {

    //            MessageBox.Show(ex.Message);
    //            return null;
    //        }
    //    }
    //}
    #endregion Using list<byte> library



    #region using byte[] library
    //public class CharWriter : ByteWriter
    //{
    //   // public int len;
    //    //public CharWriter(ByteWriter bw)
    //    //{
    //    //    this.len = bw.lent;
    //    //}
    //    static byte GetHexValue(int i)
    //    {
    //        return i < 10 ? (byte)(i + 48) : (byte)(i + 87);
    //    }

    //    public void WriteByte(byte value)
    //    {
    //        GrowIfNeeded(2);
    //        if (value < 10)
    //        {
    //            Writer[BytesPending++] = 48;
    //            Writer[BytesPending++] = (byte)(value + 48);
    //            return;
    //        }
    //        Writer[BytesPending++] = GetHexValue(value / 16);
    //        Writer[BytesPending++] = GetHexValue(value % 16);
    //    }

    //    public void WriteUInt16(ushort value)
    //    {
    //        GrowIfNeeded(4);
    //        if (value < 10)
    //        {
    //            Writer[BytesPending++] = 48;
    //            Writer[BytesPending++] = 48;
    //            Writer[BytesPending++] = 48;
    //            Writer[BytesPending++] = (byte)(value + 48);
    //            return;
    //        }
    //        if (value < 256)
    //        {
    //            Writer[BytesPending++] = 48;
    //            Writer[BytesPending++] = 48;
    //            Writer[BytesPending++] = GetHexValue(value / 16);
    //            Writer[BytesPending++] = GetHexValue(value % 16);
    //            return;
    //        }
    //        Writer[BytesPending++] = GetHexValue(value / 4096);
    //        Writer[BytesPending++] = GetHexValue(value / 256 % 16);
    //        Writer[BytesPending++] = GetHexValue(value / 16 % 16);
    //        Writer[BytesPending++] = GetHexValue(value % 16);
    //    }

    //    public void Write(byte[] value)
    //    {
    //        GrowIfNeeded(value.Length);
    //        value.CopyTo(Writer, BytesPending);
    //        BytesPending += value.Length;
    //    }

    //    public override byte[] Encode(MessagePacket value)
    //    {
    //        try
    //        {
    //            ReadOnlyMemory<byte> message = GetMessage(value.Message);
    //            ushort length = (ushort)message.Length;
    //            Clear();
    //            WriteByte((byte)value.Type);
    //            WriteUInt16(length);
    //            WriteByte((byte)((int)value.Quality | ((value.Retain ? 1 : 0) << 4)));
    //            WriteUInt16(value.Id);
    //            while (Writer.Length < BytesPending + length)
    //            {
    //                GrowIfNeeded(length);
    //            }
    //            message.Span.CopyTo(Writer.AsSpan(BytesPending));
    //            BytesPending += length;

    //            return GetBytes();
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show(ex.Message);
    //            
    //            return null;
    //        }
    //    }
    //}

    #endregion using byte[] library
}
