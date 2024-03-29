﻿using NetCommen.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NetCommen
{
    public class Packet
    {
        public bool EncryptFlag { get; set; }
        public int PacketId { get; private set; }

        private List<byte> buffer;
        private byte[] readableBuffer;

        public int readPos { get; private set; }

        /// <summary>Creates a new empty packet (without an ID).</summary>
        public Packet()
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0
        }

        /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
        /// <param name="_id">The packet ID.</param>
        public Packet(int _id)
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0

            Write(_id); // Write packet id to the buffer
        }

        /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
        /// <param name="_data">The bytes to add to the packet.</param>
        public Packet(byte[] _data)
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0

            SetBytes(_data);
        }

        #region Functions
        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="_data">The bytes to add to the packet.</param>
        public void SetBytes(byte[] _data)
        {
            Write(_data);
            readableBuffer = buffer.ToArray();
        }

        /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
        public Packet WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(EncryptFlag));
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the packet at the very beginning
            return this;
        }

        /// <summary>Inserts the given int at the start of the buffer.</summary>
        /// <param name="_value">The int to insert.</param>
        public void InsertInt(int _value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(_value)); // Insert the int at the start of the buffer
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count; // Return the length of buffer
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - readPos; // Return the remaining length (unread)
        }
        public Packet WriteError(bool hasError, string message = "")
        {
            Write(hasError);
            if (hasError)
            {
                Write(message);
            }
            return this;
        }
        public void ReadError()
        {
            bool hasError = ReadBool();
            if (hasError)
            {
                throw new Exception(ReadString());
            }
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="_shouldReset">Whether or not to reset the packet.</param>
        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear(); // Clear buffer
                readableBuffer = null;
                readPos = 0; // Reset readPos
            }
            else
            {
                readPos -= 4; // "Unread" the last read int
            }
        }
        #endregion

        #region Write Data fullName
        public Packet WriteByte(object _value) => Write((byte)_value);
        public Packet WriteBytes(object _value) => Write((byte[])_value);
        public Packet WriteInt16(object _value) => Write((short)_value);
        public Packet WriteInt32(object _value) => Write((int)_value);
        public Packet WriteInt64(object _value) => Write((long)_value);
        public Packet WriteSingle(object _value) => Write((float)_value);
        public Packet WriteBoolean(object _value) => Write((bool)_value);
        public Packet WriteString(object _value) => Write((string)_value);
        #endregion

        #region Read Data fullName
        public short ReadInt16(bool _moveReadPos = true) => ReadShort(_moveReadPos);
        public int ReadInt32(bool _moveReadPos = true) => ReadInt(_moveReadPos);
        public long ReadInt64(bool _moveReadPos = true) => ReadLong(_moveReadPos);
        public float ReadSingle(bool _moveReadPos = true) => ReadFloat(_moveReadPos);
        public bool ReadBoolean(bool _moveReadPos = true) => ReadBool(_moveReadPos);
        #endregion


        #region Write Data
        /// <summary>Adds a byte to the packet.</summary>
        /// <param name="_value">The byte to add.</param>
        public Packet Write(byte _value)
        {
            buffer.Add(_value);
            return this;
        }
        /// <summary>Adds an array of bytes to the packet.</summary>
        /// <param name="_value">The byte array to add.</param>
        public Packet Write(byte[] _value)
        {
            buffer.AddRange(_value);
            return this;
        }
        /// <summary>Adds a short to the packet.</summary>
        /// <param name="_value">The short to add.</param>
        public Packet Write(short _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
            return this;
        }
        /// <summary>Adds an int to the packet.</summary>
        /// <param name="_value">The int to add.</param>
        public Packet Write(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
            return this;
        }
        /// <summary>Adds a long to the packet.</summary>
        /// <param name="_value">The long to add.</param>
        public Packet Write(long _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
            return this;
        }
        /// <summary>Adds a float to the packet.</summary>
        /// <param name="_value">The float to add.</param>
        public Packet Write(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
            return this;
        }
        /// <summary>Adds a bool to the packet.</summary>
        /// <param name="_value">The bool to add.</param>
        public Packet Write(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
            return this;
        }
        public Packet Write(object _value)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, _value);
            byte[] obj = ms.ToArray();
            Write(obj.Length);
            buffer.AddRange(obj);
            return this;
        }
        /// <summary>Adds a string to the packet.</summary>
        /// <param name="_value">The string to add.</param>
        public Packet Write(string _value)
        {
            if (_value == null)
            {
                Write(0);
                return this;
            }
            Write(_value.Length); // Add the length of the string to the packet
            buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // Add the string itself
            return this;
        }
        #endregion

        #region Read Data
        /// <summary>Reads a byte from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte ReadByte(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                byte _value = readableBuffer[readPos]; // Get the byte at readPos' position
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Increase readPos by 1
                }
                return _value; // Return the byte
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        /// <summary>Reads an array of bytes from the packet.</summary>
        /// <param name="_length">The length of the byte array.</param>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                byte[] _value = buffer.GetRange(readPos, _length).ToArray(); // Get the bytes at readPos' position with a range of _length
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += _length; // Increase readPos by _length
                }
                return _value; // Return the bytes
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        /// <summary>Reads a short from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public short ReadShort(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                short _value = BitConverter.ToInt16(readableBuffer, readPos); // Convert the bytes to a short
                if (_moveReadPos)
                {
                    // If _moveReadPos is true and there are unread bytes
                    readPos += 2; // Increase readPos by 2
                }
                return _value; // Return the short
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public int ReadInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                int _value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Increase readPos by 4
                }
                return _value; // Return the int
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        /// <summary>Reads a long from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public long ReadLong(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                long _value = BitConverter.ToInt64(readableBuffer, readPos); // Convert the bytes to a long
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 8; // Increase readPos by 8
                }
                return _value; // Return the long
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        /// <summary>Reads a float from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public float ReadFloat(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                float _value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Increase readPos by 4
                }
                return _value; // Return the float
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        /// <summary>Reads a bool from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public bool ReadBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Increase readPos by 1
                }
                return _value; // Return the bool
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        /// <summary>Reads a string from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt(); // Get the length of the string
                string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Convert the bytes to a string
                if (_moveReadPos && _value.Length > 0)
                {
                    // If _moveReadPos is true string is not empty
                    readPos += _length; // Increase readPos by the length of the string
                }
                return _value; // Return the string
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        public object ReadObject(bool _moveReadPos = true)
        {
            int len = ReadInt();
            byte[] arrBytes = ReadBytes(len);
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            object obj = (object)binForm.Deserialize(memStream);

            if (_moveReadPos && arrBytes.Length > 0)
            {
                // If _moveReadPos is true string is not empty
                readPos += len; // Increase readPos by the length of the string
            }
            return obj;
        }
        #endregion

        #region Operators
        public static Packet operator +(Packet packt, int value)
        {
            return packt.Write(value);
        }
        public static Packet operator +(Packet packt, float value)
        {
            return packt.Write(value);
        }
        public static Packet operator +(Packet packt, string value)
        {
            return packt.Write(value);
        }
        public static Packet operator +(Packet packt, byte value)
        {
            return packt.Write(value);
        }
        public static Packet operator +(Packet packt, byte[] value)
        {
            return packt.Write(value);
        }
        public static Packet operator +(Packet packt, bool value)
        {
            return packt.Write(value);
        }
        public static Packet operator +(Packet packt, short value)
        {
            return packt.Write(value);
        }
        public static Packet operator +(Packet packt, long value)
        {
            return packt.Write(value);
        }
        public static Packet operator +(Packet packt, Guid value)
        {
            return packt.Write(value.ToString());
        }
        public static Packet operator +(Packet packt, object value)
        {
            return packt.Write(value);
        }
        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            string result = "00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15\n";
            int a = (int)Math.Ceiling(buffer.Count / 16d);
            for (int i = 0; i < a; i++)
            {
                int to = 16 + i * 16;
                for (int j = i*16; j < Math.Min(to, buffer.Count); j++)
                {
                    if (j < buffer.Count)
                        result += BitConverter.ToString(new byte[] { buffer[j] }) + " ";
                    else
                        result += "   ";
                }
                result += "\n";
            }

            return result;
        }
    }
}
