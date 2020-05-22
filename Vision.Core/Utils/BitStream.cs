//*****************************************************************************
// Copyright � 2005, Bill Koukoutsis
//
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice, this
// list of conditions and the following disclaimer.
//
// Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation
// and/or other materials provided with the distribution.
//
// Neither the name of the ORGANIZATION nor the names of its contributors may
// be used to endorse or promote products derived from this software without
// specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//*****************************************************************************

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Resources;
using System.Security.Cryptography;
using System.Text;

namespace Vision.Core.Utils
{
    /// <summary>
    ///		Creates a stream for reading and writing variable-length data.
    /// </summary>
    /// <remarks>
    ///		<b><font color="red">Notes to Callers:</font></b> Make sure to
    ///		include the "BitStream.resx" resource file in projects using the
    ///		<see cref="BitStream"/> class.<br></br>
    ///		<br></br>
    ///		[20051201]: Fixed problem with <c>public virtual void Write(ulong bits, int bitIndex, int count)</c>
    ///		and <c>public virtual int Read(out ulong bits, int bitIndex, int count)</c> methods.<br></br>
    ///		<br></br>
    ///		[20051127]: Added <c>public virtual void WriteTo(Stream bits);</c> to write
    ///		the contents of the current <b>bit</b> stream to another stream.<br></br>
    ///		<br></br>
    ///		[20051125]: Added the following implicit operators to allow type casting
    ///		instances of the <see cref="BitStream"/> class to and from other types
    ///		of <see cref="Stream"/> objects:<br></br>
    ///		<br></br>
    ///		<c>public static implicit operator BitStream(MemoryStream bits);</c><br></br>
    ///		<c>public static implicit operator MemoryStream(BitStream bits);</c><br></br>
    ///		<c>public static implicit operator BitStream(FileStream bits);</c><br></br>
    ///		<c>public static implicit operator BitStream(BufferedStream bits);</c><br></br>
    ///		<c>public static implicit operator BufferedStream(BitStream bits);</c><br></br>
    ///		<c>public static implicit operator BitStream(NetworkStream bits);</c><br></br>
    ///		<c>public static implicit operator BitStream(CryptoStream bits);</c><br></br>
    ///		<br></br>
    ///		[20051124]: Added <c>public virtual <see cref="byte"/> [] ToByteArray();</c> method.<br></br>
    ///		<br></br>
    ///		[20051124]: The <c>public override <see cref="int"/> ReadByte();</c> and
    ///		<c>public override void WriteByte(<see cref="byte"/> value)</c> method are now
    ///		supported by the <see cref="BitStream"/> class.<br></br>
    ///		<br></br>
    ///		[20051123]: Added <c>public BitStream(<see cref="Stream"/> bits);</c> contructor.<br></br>
    ///		<br></br>
    /// </remarks>
    /// <seealso cref="BitStream"/>
    /// <seealso cref="Stream"/>
    /// <seealso cref="int"/>
    /// <seealso cref="byte"/>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class BitStream : Stream
    {
        #region Nested Classes [20051116]

        #region private sealed class BitStreamResources [20051116]

        /// <summary>
        ///		Manages reading resources on behalf of the <see cref="BitStream"/>
        ///		class.
        /// </summary>
        /// <remarks>
        ///		<b><font color="red">Notes to Callers:</font></b> Make sure to
        ///		include the "BitStream.resx" resource file in projects using the
        ///		<see cref="BitStream"/> class.
        /// </remarks>
        private sealed class BitStreamResources
        {
            #region Fields [20051116]

            /// <summary>
            ///		The <see cref="ResourceManager"/> object.
            /// </summary>
            /// <remarks>
            ///		.
            /// </remarks>
            private static ResourceManager _resourceManager;

            /// <summary>
            ///		An <see cref="object"/> used to lock access to
            ///		<see cref="BitStream"/> resources while the current
            ///		<see cref="ResourceManager"/> is busy.
            /// </summary>
            /// <remarks>
            ///		.
            /// </remarks>
            private static object _oResManLock;

            /// <summary>
            ///		A <see cref="bool"/> value specifying whether a resource is
            ///		currently being loaded.
            /// </summary>
            /// <remarks>
            ///		.
            /// </remarks>
            /// <seealso cref="bool"/>
            private static bool _blnLoadingResource;

            #endregion Fields [20051116]

            #region Methods [20051116]

            /// <summary>
            ///		Initialises the resource manager.
            /// </summary>
            /// <remarks>
            ///		.
            /// </remarks>
            private static void InitialiseResourceManager()
            {
                if (_resourceManager != null) return;
                lock (typeof(BitStreamResources))
                {
                    if (_resourceManager != null) return;
                    _oResManLock = new object();
                    _resourceManager = new ResourceManager("BKSystem.IO.BitStream", typeof(BitStream).Assembly);
                }
            }

            /// <summary>
            ///		Gets the specified string resource.
            /// </summary>
            /// <remarks>
            ///		.
            /// </remarks>
            /// <param name="name">
            ///		A <see cref="string"/> representing the specified resource.
            /// </param>
            /// <returns>
            ///		A <see cref="string"/> representing the contents of the specified
            ///		resource.
            /// </returns>
            /// <seealso cref="string"/>
            public static string GetString(string name)
            {
                string str;
                if (_resourceManager == null)
                    InitialiseResourceManager();

                lock (_oResManLock)
                {
                    if (_blnLoadingResource)
                        return ("The resource manager was unable to load the resource: " + name);

                    _blnLoadingResource = true;
                    str = _resourceManager?.GetString(name, null);
                    _blnLoadingResource = false;
                }
                return str;
            }

            #endregion Methods [20051116]
        }

        #endregion private sealed class BitStreamResources [20051116]

        #endregion Nested Classes [20051116]

        #region Constants [20051116]

        /// <summary>
        ///		An <see cref="int"/> value defining the number of bits
        ///		in a <see cref="byte"/> value type.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="int"/>
        /// <seealso cref="byte"/>
        private const int SizeOfByte = 8;

        /// <summary>
        ///		An <see cref="int"/> value defining the number of bits
        ///		in a <see cref="char"/> value type.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="int"/>
        /// <seealso cref="char"/>
        private const int SizeOfChar = 128;

        /// <summary>
        ///		An <see cref="int"/> value defining the number of bits
        ///		in a <see cref="ushort"/> value type.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="int"/>
        /// <seealso cref="ushort"/>
        private const int SizeOfUInt16 = 16;

        /// <summary>
        ///		An <see cref="int"/> value defining the number of bits
        ///		in a <see cref="uint"/> value type.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="int"/>
        /// <seealso cref="uint"/>
        private const int SizeOfUInt32 = 32;

        /// <summary>
        ///		An <see cref="int"/> value defining the number of bits
        ///		in a <see cref="float"/> value type.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="int"/>
        /// <seealso cref="float"/>
        private const int SizeOfSingle = 32;

        /// <summary>
        ///		An <see cref="int"/> value defining the number of bits
        ///		in a <see cref="ulong"/> value type.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="int"/>
        /// <seealso cref="ulong"/>
        private const int SizeOfUInt64 = 64;

        /// <summary>
        ///		An <see cref="int"/> value defining the number of bits
        ///		in a <see cref="double"/> value type.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="int"/>
        /// <seealso cref="double"/>
        private const int SizeOfDouble = 64;

        /// <summary>
        ///		An <see cref="uint"/> value defining the number of bits
        ///		per element in the internal buffer.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="uint"/>
        private const uint BitBuffer_SizeOfElement = SizeOfUInt32;

        /// <summary>
        ///		An <see cref="int"/> value defining the number of bit
        ///		shifts equivalent to the number of bits per element in the
        ///		internal buffer.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="int"/>
        private const int BitBuffer_SizeOfElement_Shift = 5;

        /// <summary>
        ///		An <see cref="uint"/> value defining the equivalent of
        ///		a divisor in bitwise <b>AND</b> operations emulating
        ///		modulo calculations.
        /// </summary>
        /// <remarks>
        ///		This field is constant.
        /// </remarks>
        /// <seealso cref="uint"/>
        private const uint BitBuffer_SizeOfElement_Mod = 31;

        /// <summary>
        ///		An <see cref="uint"/> array defining a series of values
        ///		useful in generating bit masks in read and write operations.
        /// </summary>
        /// <remarks>
        ///		This field is static.
        /// </remarks>
        private static readonly uint[] BitMaskHelperLUT = {
            0x00000000,
            0x00000001, 0x00000003, 0x00000007, 0x0000000F,
            0x0000001F, 0x0000003F, 0x0000007F, 0x000000FF,
            0x000001FF, 0x000003FF, 0x000007FF, 0x00000FFF,
            0x00001FFF, 0x00003FFF, 0x00007FFF, 0x0000FFFF,
            0x0001FFFF, 0x0003FFFF, 0x0007FFFF, 0x000FFFFF,
            0x001FFFFF, 0x003FFFFF, 0x007FFFFF, 0x00FFFFFF,
            0x01FFFFFF, 0x03FFFFFF, 0x07FFFFFF, 0x0FFFFFFF,
            0x1FFFFFFF, 0x3FFFFFFF, 0x7FFFFFFF, 0xFFFFFFFF,
        };

        #endregion Constants [20051116]

        #region Fields [20051114]

        /// <summary>
        ///		A <see cref="bool"/> value specifying whether the current
        ///		stream is able to process data.
        /// </summary>
        /// <remarks>
        ///		This field is set to <b>true</b> by default.
        /// </remarks>
        /// <seealso cref="bool"/>
        private bool _blnIsOpen = true;

        /// <summary>
        ///		An array of <see cref="uint"/> values specifying the internal
        ///		bit buffer for the current stream.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <seealso cref="uint"/>
        private uint[] _auiBitBuffer;

        /// <summary>
        ///		An <see cref="uint"/> value specifying the current length of the
        ///		internal bit buffer for the current stream.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <seealso cref="uint"/>
        private uint _uiBitBuffer_Length;

        /// <summary>
        ///		An <see cref="uint"/> value specifying the current elemental index
        ///		of the internal bit buffer for the current stream.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <seealso cref="uint"/>
        private uint _uiBitBuffer_Index;

        /// <summary>
        ///		An <see cref="uint"/> value specifying the current bit index for the
        ///		current element of the internal bit buffer for the current stream.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <seealso cref="uint"/>
        private uint _uiBitBuffer_BitIndex;

        /// <summary>
        ///		An <see cref="IFormatProvider"/> object specifying the format specifier
        ///		for the current stream.
        /// </summary>
        /// <remarks>
        ///		This field is set to <b><see cref="CultureInfo.InvariantCulture"/></b>
        ///		by default.
        /// </remarks>
        /// <see cref="IFormatProvider"/>
        /// <see cref="CultureInfo.InvariantCulture"/>
        private static readonly IFormatProvider _ifp = CultureInfo.InvariantCulture;

        #endregion Fields [20051114]

        #region Properties [20051116]

        /// <summary>
        ///		Gets the length of this stream in <b>bits</b>.
        /// </summary>
        ///	<exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        ///	</exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <value>
        ///		An <see cref="long"/> value specifying the length of this stream in
        ///		<b>bits</b>.
        /// </value>
        /// <seealso cref="long"/>
        public override long Length
        {
            get
            {
                if (!_blnIsOpen)
                    throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

                return _uiBitBuffer_Length;
            }
        }

        /// <summary>
        ///		Gets the maximum number of <b>8-bit</b> values required to store this
        ///		stream.
        /// </summary>
        ///	<exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        ///	</exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <example>
        ///		<font face="Courier New">
        ///		<font color="green">
        ///		// This property can be used in conjunction with the <see cref="Read(byte [], int, int)"/> method<br></br>
        ///		// to read the entire stream into a <see cref="byte"/> array.<br></br>
        ///		</font>
        ///		:<br></br>
        ///		BitStream bstrm = <font color="blue">new</font> BitStream();<br></br>
        ///		:<br></br>
        ///		:<br></br>
        ///		<font color="blue">byte</font> [] abytBuffer = <font color="blue">new byte</font> [bstrm.Length8];<br></br>
        ///		<font color="blue">int</font> iBitsRead = Read(abytBuffer, 0, (<font color="blue">int</font>)bstrm.Length8);<br></br>
        ///		:<br></br>
        ///		</font>
        /// </example>
        /// <value>
        ///		An <see cref="long"/> value specifying the maximum number of
        ///		<b>8-bit</b> values required to store this stream.
        /// </value>
        /// <seealso cref="Read(byte [], int, int)"/>
        /// <seealso cref="byte"/>
        /// <seealso cref="long"/>
        public virtual long Length8
        {
            get
            {
                if (!_blnIsOpen)
                    throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

                return (_uiBitBuffer_Length >> 3) + ((_uiBitBuffer_Length & 7) > 0 ? 1 : 0);
            }
        }

        /// <summary>
        ///		Gets the maximum number of <b>16-bit</b> values required to store this
        ///		stream.
        /// </summary>
        ///	<exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        ///	</exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <example>
        ///		<font face="Courier New">
        ///		<font color="green">
        ///		// This property can be used in conjunction with the <see cref="Read(short [], int, int)"/> method<br></br>
        ///		// to read the entire stream into an <see cref="short"/> array.<br></br>
        ///		</font>
        ///		:<br></br>
        ///		BitStream bstrm = <font color="blue">new</font> BitStream();<br></br>
        ///		:<br></br>
        ///		:<br></br>
        ///		<font color="blue">short</font> [] asBuffer = <font color="blue">new short</font> [bstrm.Length16];<br></br>
        ///		<font color="blue">int</font> iBitsRead = Read(asBuffer, 0, (<font color="blue">int</font>)bstrm.Length16);<br></br>
        ///		:<br></br>
        ///		</font>
        /// </example>
        /// <value>
        ///		An <see cref="long"/> value specifying the maximum number of
        ///		<b>16-bit</b> values required to store this stream.
        /// </value>
        /// <seealso cref="Read(short [], int, int)"/>
        /// <seealso cref="short"/>
        /// <seealso cref="long"/>
        public virtual long Length16
        {
            get
            {
                if (!_blnIsOpen)
                    throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

                return (_uiBitBuffer_Length >> 4) + ((_uiBitBuffer_Length & 15) > 0 ? 1 : 0);
            }
        }

        /// <summary>
        ///		Gets the maximum number of <b>32-bit</b> values required to store this
        ///		stream.
        /// </summary>
        ///	<exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        ///	</exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <example>
        ///		<font face="Courier New">
        ///		<font color="green">
        ///		// This property can be used in conjunction with the <see cref="Read(int [], int, int)"/> method<br></br>
        ///		// to read the entire stream into an <see cref="int"/> array.<br></br>
        ///		</font>
        ///		:<br></br>
        ///		BitStream bstrm = <font color="blue">new</font> BitStream();<br></br>
        ///		:<br></br>
        ///		:<br></br>
        ///		<font color="blue">int</font> [] aiBuffer = <font color="blue">new int</font> [bstrm.Length32];<br></br>
        ///		<font color="blue">int</font> iBitsRead = Read(aiBuffer, 0, (<font color="blue">int</font>)bstrm.Length32);<br></br>
        ///		:<br></br>
        ///		</font>
        /// </example>
        /// <value>
        ///		An <see cref="long"/> value specifying the maximum number of
        ///		<b>32-bit</b> values required to store this stream.
        /// </value>
        /// <seealso cref="Read(int [], int, int)"/>
        /// <seealso cref="int"/>
        /// <seealso cref="long"/>
        public virtual long Length32
        {
            get
            {
                if (!_blnIsOpen)
                    throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

                return (_uiBitBuffer_Length >> 5) + ((_uiBitBuffer_Length & 31) > 0 ? 1 : 0);
            }
        }

        /// <summary>
        ///		Gets the maximum number of <b>64-bit</b> values required to store this
        ///		stream.
        /// </summary>
        ///	<exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        ///	</exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <example>
        ///		<font face="Courier New">
        ///		<font color="green">
        ///		// This property can be used in conjunction with the <see cref="Read(long [], int, int)"/> method<br></br>
        ///		// to read the entire stream into an <see cref="long"/> array.<br></br>
        ///		</font>
        ///		:<br></br>
        ///		BitStream bstrm = <font color="blue">new</font> BitStream();<br></br>
        ///		:<br></br>
        ///		:<br></br>
        ///		<font color="blue">long</font> [] alBuffer = <font color="blue">new long</font> [bstrm.Length64];<br></br>
        ///		<font color="blue">int</font> iBitsRead = Read(alBuffer, 0, (<font color="blue">int</font>)bstrm.Length64);<br></br>
        ///		:<br></br>
        ///		</font>
        /// </example>
        /// <value>
        ///		An <see cref="long"/> value specifying the maximum number of
        ///		<b>64-bit</b> values required to store this stream.
        /// </value>
        /// <seealso cref="Read(long [], int, int)"/>
        /// <seealso cref="long"/>
        public virtual long Length64
        {
            get
            {
                if (!_blnIsOpen)
                    throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

                return (_uiBitBuffer_Length >> 6) + ((_uiBitBuffer_Length & 63) > 0 ? 1 : 0);
            }
        }

        /// <summary>
        ///		Gets the number of <b>bits</b> allocated to this stream.
        /// </summary>
        ///	<exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        ///	</exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <value>
        ///		An <see cref="long"/> value specifying the number of <b>bits</b>
        ///		allocated to this stream.
        /// </value>
        public virtual long Capacity
        {
            get
            {
                if (!_blnIsOpen)
                    throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

                return ((long)_auiBitBuffer.Length) << BitBuffer_SizeOfElement_Shift;
            }
        }

        /// <summary>
        ///		Gets or sets the current <b>bit</b> position within this stream.
        /// </summary>
        ///	<exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        ///	</exception>
        ///	<exception cref="ArgumentOutOfRangeException">
        ///		The position is set to a negative value or position + 1 is geater than
        ///		<see cref="Length"/>.
        ///	</exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <value>
        ///		An <see cref="long"/> value specifying the current <b>position</b>
        ///		within this stream.
        /// </value>
        /// <seealso cref="Length"/>
        /// <seealso cref="long"/>
        public override long Position
        {
            get
            {
                if (!_blnIsOpen)
                    throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

                var uiPosition = (_uiBitBuffer_Index << BitBuffer_SizeOfElement_Shift) + _uiBitBuffer_BitIndex;
                return uiPosition;
            }
            set
            {
                if (!_blnIsOpen)
                    throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), BitStreamResources.GetString("ArgumentOutOfRange_NegativePosition"));

                var uiRequestedPosition = (uint)value;

                if (_uiBitBuffer_Length < uiRequestedPosition + 1)
                    throw new ArgumentOutOfRangeException(nameof(value), BitStreamResources.GetString("ArgumentOutOfRange_InvalidPosition"));

                _uiBitBuffer_Index = uiRequestedPosition >> BitBuffer_SizeOfElement_Shift;
                if ((uiRequestedPosition & BitBuffer_SizeOfElement_Mod) > 0)
                    _uiBitBuffer_BitIndex = (uiRequestedPosition & BitBuffer_SizeOfElement_Mod);
                else
                    _uiBitBuffer_BitIndex = 0;
            }
        }

        /// <summary>
        ///		Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <value>
        ///		A <see cref="bool"/> value indicating whether the current stream
        ///		supports reading.
        /// </value>
        /// <seealso cref="bool"/>
        public override bool CanRead => _blnIsOpen;

        /// <summary>
        ///		Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <remarks>
        ///		This method always returns <b>false</b>. To set the position within
        ///		the current stream use the <see cref="Position"/> property instead.
        /// </remarks>
        /// <value>
        ///		A <see cref="bool"/> value indicating whether the current stream
        ///		supports seeking.
        /// </value>
        /// <seealso cref="Position"/>
        /// <seealso cref="bool"/>
        public override bool CanSeek => false;

        /// <summary>
        ///		Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <value>
        ///		A <see cref="bool"/> value indicating whether the current stream
        ///		supports writing.
        /// </value>
        /// <seealso cref="bool"/>
        public override bool CanWrite => _blnIsOpen;

        /// <summary>
        ///		Gets a value indicating whether the current stream supports setting
        ///		its length.
        /// </summary>
        /// <remarks>
        ///		This field always returns <b>false</b>. All write operations at the
        ///		end of the <b>BitStream</b> expand the <b>BitStream</b> automatically.
        /// </remarks>
        /// <value>
        ///		A <see cref="bool"/> value indicating whether the current stream
        ///		supports setting its length.
        /// </value>
        /// <see cref="bool"/>
        public static bool CanSetLength => false;

        /// <summary>
        ///		Gets a value indicating whether the current stream supports the flush
        ///		operation.
        /// </summary>
        /// <remarks>
        ///		This field always returns <b>false</b>. Since any data written to a
        ///		<b>BitStream</b> is written into RAM, flush operations become
        ///		redundant.
        /// </remarks>
        /// <value>
        ///		A <see cref="bool"/> value indicating whether the current stream
        ///		supports the flush operation.
        /// </value>
        /// <seealso cref="bool"/>
        public static bool CanFlush => false;

        #endregion Properties [20051116]

        #region ctors/dtors [20051123]

        /// <summary>
        ///		Initialises a new instance of the <see cref="BitStream"/> class
        ///		with an expandable capacity initialised to one.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <seealso cref="BitStream"/>
        public BitStream()
        {
            // Initialise the bit buffer with 1 UInt32
            _auiBitBuffer = new uint[1];
        }

        /// <summary>
        ///		Initialises a new instance of the <see cref="BitStream"/> class
        ///		with an expandable capacity initialised to the specified capacity in
        ///		<b>bits</b>.
        /// </summary>
        ///	<exception cref="ArgumentOutOfRangeException">
        ///		<i>capacity</i> is negative or zero.
        ///	</exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="capacity">
        ///		An <see cref="long"/> specifying the initial size of the internal
        ///		bit buffer in <b>bits</b>.
        /// </param>
        /// <seealso cref="BitStream"/>
        public BitStream(long capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(BitStreamResources.GetString("ArgumentOutOfRange_NegativeOrZeroCapacity"));

            _auiBitBuffer = new uint[(capacity >> BitBuffer_SizeOfElement_Shift) + ((capacity & BitBuffer_SizeOfElement_Mod) > 0 ? 1 : 0)];
        }

        /// <summary>
        ///		Initialises a new instance of the <see cref="BitStream"/> class
        ///		with the <b>bits</b> provided by the specified <see cref="Stream"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		Added [20051122].
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="Stream"/> object containing the specified <b>bits</b>.
        /// </param>
        /// <seealso cref="BitStream"/>
        /// <seealso cref="Stream"/>
        public BitStream(Stream bits) : this()
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            // Write the stream to the internal buffer using a temporary byte buffer
            var l_bits = new byte[bits.Length];

            var lCurrentPos = bits.Position;
            bits.Position = 0;

            bits.Read(l_bits, 0, (int)bits.Length);

            bits.Position = lCurrentPos;

            Write(l_bits, 0, (int)bits.Length);
        }

        #endregion ctors/dtors [20051123]

        #region Methods [20051201]

        #region Write [20051201]

        #region Generic Writes [20051115]

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="uint"/> value to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="uint"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="uint"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="uint"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="uint"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="uint"/>
        private void Write(ref uint bits, ref uint bitIndex, ref uint count)
        {
            // Calculate the current position
            var uiBitBuffer_Position = (_uiBitBuffer_Index << BitBuffer_SizeOfElement_Shift) + _uiBitBuffer_BitIndex;
            // Detemine the last element in the bit buffer
            var uiBitBuffer_LastElementIndex = (_uiBitBuffer_Length >> BitBuffer_SizeOfElement_Shift);
            // Clalculate this values end index
            var uiValue_EndIndex = bitIndex + count;

            // Clear out unwanted bits in value
            var iValue_BitsToShift = (int)bitIndex;
            var uiValue_BitMask = (BitMaskHelperLUT[count] << iValue_BitsToShift);
            bits &= uiValue_BitMask;

            // Position the bits in value
            var uiBitBuffer_FreeBits = BitBuffer_SizeOfElement - _uiBitBuffer_BitIndex;
            iValue_BitsToShift = (int)(uiBitBuffer_FreeBits - uiValue_EndIndex);
            uint uiValue_Indexed;
            if (iValue_BitsToShift < 0)
                uiValue_Indexed = bits >> Math.Abs(iValue_BitsToShift);
            else
                uiValue_Indexed = bits << iValue_BitsToShift;

            // Clear current bits in bit buffer that are at same indices
            // (only if overwriting)
            if (_uiBitBuffer_Length >= (uiBitBuffer_Position + 1))
            {
                var iBitBuffer_BitsToShift = (int)(uiBitBuffer_FreeBits - count);
                uint uiBitBuffer_BitMask;
                if (iBitBuffer_BitsToShift < 0)
                    uiBitBuffer_BitMask = uint.MaxValue ^ (BitMaskHelperLUT[count] >> Math.Abs(iBitBuffer_BitsToShift));
                else
                    uiBitBuffer_BitMask = uint.MaxValue ^ (BitMaskHelperLUT[count] << iBitBuffer_BitsToShift);
                _auiBitBuffer[_uiBitBuffer_Index] &= uiBitBuffer_BitMask;

                // Is this the last element of the bit buffer?
                if (uiBitBuffer_LastElementIndex == _uiBitBuffer_Index)
                {
                    uint uiBitBuffer_NewLength;
                    if (uiBitBuffer_FreeBits >= count)
                        uiBitBuffer_NewLength = uiBitBuffer_Position + count;
                    else
                        uiBitBuffer_NewLength = uiBitBuffer_Position + uiBitBuffer_FreeBits;
                    if (uiBitBuffer_NewLength > _uiBitBuffer_Length)
                    {
                        var uiBitBuffer_ExtraBits = uiBitBuffer_NewLength - _uiBitBuffer_Length;
                        UpdateLengthForWrite(uiBitBuffer_ExtraBits);
                    }
                }
            }
            else // Not overwrinting any bits: _uiBitBuffer_Length < (uiBitBuffer_Position + 1)
            {
                UpdateLengthForWrite(uiBitBuffer_FreeBits >= count ? count : uiBitBuffer_FreeBits);
            }

            // Write value
            _auiBitBuffer[_uiBitBuffer_Index] |= uiValue_Indexed;

            if (uiBitBuffer_FreeBits >= count)
                UpdateIndicesForWrite(count);
            else // Some bits in value did not fit
                 // in current bit buffer element
            {
                UpdateIndicesForWrite(uiBitBuffer_FreeBits);

                var uiValue_RemainingBits = count - uiBitBuffer_FreeBits;
                var uiValue_StartIndex = bitIndex;
                Write(ref bits, ref uiValue_StartIndex, ref uiValue_RemainingBits);
            }
        }

        #endregion Generic Writes [20051115]

        #region 1-Bit Writes [20051116]

        /// <summary>
        ///		Writes the <b>bit</b> represented by a <see cref="bool"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bit">
        ///		A <see cref="bool"/> value representing the <b>bit</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="bool"/>
        public virtual void Write(bool bit)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

            // Convert the bool to UInt32
            var uiBit = (uint)(bit ? 1 : 0);
            uint uiBitIndex = 0;
            uint uiCount = 1;

            Write(ref uiBit, ref uiBitIndex, ref uiCount);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="bool"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="bool"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="bool"/>
        public virtual void Write(bool[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="bool"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="bool"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="bool"/>
        ///		offset to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="bool"/> values to write.
        /// </param>
        /// <seealso cref="bool"/>
        /// <seealso cref="int"/>
        public virtual void Write(bool[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            for (var iBitCounter = offset; iBitCounter < iEndIndex; iBitCounter++)
                Write(bits[iBitCounter]);
        }

        #endregion 1-Bit Writes [20051116]

        #region 8-Bit Writes [20051124]

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="byte"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="byte"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="byte"/>
        public virtual void Write(byte bits)
        {
            Write(bits, 0, SizeOfByte);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="byte"/> value to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="byte"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="byte"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="byte"/>
        /// <seealso cref="int"/>
        public virtual void Write(byte bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfByte - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_Byte"));

            uint uiBits = bits;
            var uiBitIndex = (uint)bitIndex;
            var uiCount = (uint)count;

            Write(ref uiBits, ref uiBitIndex, ref uiCount);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="byte"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="byte"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="byte"/>
        public virtual void Write(byte[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="byte"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="byte"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="byte"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="byte"/> values to write.
        /// </param>
        /// <seealso cref="byte"/>
        /// <seealso cref="int"/>
        public sealed override void Write(byte[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            for (var iByteCounter = offset; iByteCounter < iEndIndex; iByteCounter++)
                Write(bits[iByteCounter]);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="sbyte"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="sbyte"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="sbyte"/>

        public virtual void Write(sbyte bits)
        {
            Write(bits, 0, SizeOfByte);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="sbyte"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="sbyte"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="sbyte"/>
        /// <seealso cref="int"/>

        public virtual void Write(sbyte bits, int bitIndex, int count)
        {
            // Convert the value to a byte
            var bytBits = (byte)bits;

            Write(bytBits, bitIndex, count);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="sbyte"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="sbyte"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="sbyte"/>

        public virtual void Write(sbyte[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="sbyte"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="sbyte"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="sbyte"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="sbyte"/> values to write.
        /// </param>
        /// <seealso cref="sbyte"/>
        /// <seealso cref="int"/>

        public virtual void Write(sbyte[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var l_bits = new byte[count];
            Buffer.BlockCopy(bits, offset, l_bits, 0, count);

            Write(l_bits, 0, count);
        }

        /// <summary>
        ///		Writes a byte to the current stream at the current position.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.<br></br>
        ///		<br></br>
        ///		Modified [20051124]
        /// </remarks>
        /// <param name="value">
        ///		The byte to write.
        /// </param>
        public override void WriteByte(byte value)
        {
            Write(value);
        }

        #endregion 8-Bit Writes [20051124]

        #region 16-Bit Writes [20051115]

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="char"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="char"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="char"/>

        public virtual void Write(char bits)
        {
            Write(bits, 0, SizeOfChar);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="char"/> value to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="char"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="char"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="char"/>
        /// <seealso cref="int"/>

        public virtual void Write(char bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfChar - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_Char"));

            uint uiBits = bits;
            var uiBitIndex = (uint)bitIndex;
            var uiCount = (uint)count;

            Write(ref uiBits, ref uiBitIndex, ref uiCount);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="char"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="char"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="char"/>

        public virtual void Write(char[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="char"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="char"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="char"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="char"/> values to write.
        /// </param>
        /// <seealso cref="char"/>
        /// <seealso cref="int"/>

        public virtual void Write(char[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            for (var iCharCounter = offset; iCharCounter < iEndIndex; iCharCounter++)
                Write(bits[iCharCounter]);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ushort"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="ushort"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="ushort"/>

        public virtual void Write(ushort bits)
        {
            Write(bits, 0, SizeOfUInt16);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ushort"/> value to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="ushort"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="ushort"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="ushort"/>
        /// <seealso cref="int"/>

        public virtual void Write(ushort bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfUInt16 - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_UInt16"));

            uint uiBits = bits;
            var uiBitIndex = (uint)bitIndex;
            var uiCount = (uint)count;

            Write(ref uiBits, ref uiBitIndex, ref uiCount);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ushort"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="ushort"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="ushort"/>

        public virtual void Write(ushort[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ushort"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="ushort"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="ushort"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="ushort"/> values to write.
        /// </param>
        /// <seealso cref="ushort"/>
        /// <seealso cref="int"/>

        public virtual void Write(ushort[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            for (var iUInt16Counter = offset; iUInt16Counter < iEndIndex; iUInt16Counter++)
                Write(bits[iUInt16Counter]);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="short"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="short"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="short"/>
        public virtual void Write(short bits)
        {
            Write(bits, 0, SizeOfUInt16);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="short"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="short"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="short"/>
        /// <seealso cref="int"/>
        public virtual void Write(short bits, int bitIndex, int count)
        {
            // Convert the value to an UInt16
            var usBits = (ushort)bits;

            Write(usBits, bitIndex, count);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="short"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="short"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="short"/>
        public virtual void Write(short[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="short"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="short"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="short"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="short"/> values to write.
        /// </param>
        /// <seealso cref="short"/>
        /// <seealso cref="int"/>
        public virtual void Write(short[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var ausBits = new ushort[count];
            Buffer.BlockCopy(bits, offset << 1, ausBits, 0, count << 1);

            Write(ausBits, 0, count);
        }

        #endregion 16-Bit Writes [20051115]

        #region 32-Bit Writes [20051115]

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="uint"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="uint"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="uint"/>

        public virtual void Write(uint bits)
        {
            Write(bits, 0, SizeOfUInt32);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="uint"/> value to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="uint"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="uint"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="uint"/>
        /// <seealso cref="int"/>

        public virtual void Write(uint bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfUInt32 - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_UInt32"));

            var uiBitIndex = (uint)bitIndex;
            var uiCount = (uint)count;

            Write(ref bits, ref uiBitIndex, ref uiCount);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="uint"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="uint"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="uint"/>

        public virtual void Write(uint[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="uint"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="uint"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="uint"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="uint"/> values to write.
        /// </param>
        /// <seealso cref="uint"/>
        /// <seealso cref="int"/>

        public virtual void Write(uint[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            for (var iUInt32Counter = offset; iUInt32Counter < iEndIndex; iUInt32Counter++)
                Write(bits[iUInt32Counter]);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="int"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="int"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="int"/>
        public virtual void Write(int bits)
        {
            Write(bits, 0, SizeOfUInt32);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="int"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="int"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="int"/>
        public virtual void Write(int bits, int bitIndex, int count)
        {
            // Convert the value to an UInt32
            var uiBits = (uint)bits;

            Write(uiBits, bitIndex, count);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="int"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="int"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="int"/>
        public virtual void Write(int[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="int"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="int"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="int"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="int"/> values to write.
        /// </param>
        /// <seealso cref="int"/>
        public virtual void Write(int[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var auiBits = new uint[count];
            Buffer.BlockCopy(bits, offset << 2, auiBits, 0, count << 2);

            Write(auiBits, 0, count);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="float"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="float"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="float"/>
        public virtual void Write(float bits)
        {
            Write(bits, 0, SizeOfSingle);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="float"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="float"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="float"/>
        /// <seealso cref="int"/>
        public virtual void Write(float bits, int bitIndex, int count)
        {
            var l_bits = BitConverter.GetBytes(bits);
            var uiBits = l_bits[0] | ((uint)l_bits[1]) << 8 | ((uint)l_bits[2]) << 16 | ((uint)l_bits[3]) << 24;
            Write(uiBits, bitIndex, count);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="float"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="float"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="float"/>
        public virtual void Write(float[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="float"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="float"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="float"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="float"/> values to write.
        /// </param>
        /// <seealso cref="float"/>
        /// <seealso cref="int"/>
        public virtual void Write(float[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            for (var iSingleCounter = offset; iSingleCounter < iEndIndex; iSingleCounter++)
                Write(bits[iSingleCounter]);
        }

        #endregion 32-Bit Writes [20051115]

        #region 64-Bit Writes [20051201]

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ulong"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="ulong"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="ulong"/>

        public virtual void Write(ulong bits)
        {
            Write(bits, 0, SizeOfUInt64);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ulong"/> value to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="ulong"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.<br></br>
        ///		<br></br>
        ///		Fixed [20051201].
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="ulong"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="ulong"/>
        /// <seealso cref="int"/>

        public virtual void Write(ulong bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfUInt64 - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_UInt64"));

            var iBitIndex1 = (bitIndex >> 5) < 1 ? bitIndex : -1;
            var iBitIndex2 = (bitIndex + count) > 32 ? (iBitIndex1 < 0 ? bitIndex - 32 : 0) : -1;
            var iCount1 = iBitIndex1 > -1 ? (iBitIndex1 + count > 32 ? 32 - iBitIndex1 : count) : 0;
            var iCount2 = iBitIndex2 > -1 ? (iCount1 == 0 ? count : count - iCount1) : 0;

            if (iCount1 > 0)
            {
                var uiBits1 = (uint)bits;
                var uiBitIndex1 = (uint)iBitIndex1;
                var uiCount1 = (uint)iCount1;
                Write(ref uiBits1, ref uiBitIndex1, ref uiCount1);
            }
            if (iCount2 > 0)
            {
                var uiBits2 = (uint)(bits >> 32);
                var uiBitIndex2 = (uint)iBitIndex2;
                var uiCount2 = (uint)iCount2;
                Write(ref uiBits2, ref uiBitIndex2, ref uiCount2);
            }
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ulong"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="ulong"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="ulong"/>

        public virtual void Write(ulong[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ulong"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="ulong"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="ulong"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="ulong"/> values to write.
        /// </param>
        /// <seealso cref="ulong"/>
        /// <seealso cref="int"/>

        public virtual void Write(ulong[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            for (var iUInt64Counter = offset; iUInt64Counter < iEndIndex; iUInt64Counter++)
                Write(bits[iUInt64Counter]);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="long"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="long"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="long"/>
        public virtual void Write(long bits)
        {
            Write(bits, 0, SizeOfUInt64);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="ushort"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="long"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="long"/>
        /// <seealso cref="int"/>
        public virtual void Write(long bits, int bitIndex, int count)
        {
            // Convert the value to an UInt64
            var ulBits = (ulong)bits;

            Write(ulBits, bitIndex, count);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="long"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="long"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="long"/>
        public virtual void Write(long[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="long"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="long"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="long"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="long"/> values to write.
        /// </param>
        /// <seealso cref="long"/>
        /// <seealso cref="int"/>
        public virtual void Write(long[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var aulBits = new ulong[count];
            Buffer.BlockCopy(bits, offset << 4, aulBits, 0, count << 4);

            Write(aulBits, 0, count);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="double"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="double"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <seealso cref="double"/>
        public virtual void Write(double bits)
        {
            Write(bits, 0, SizeOfDouble);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in an <see cref="double"/> value to
        ///		the current stream.
        /// </summary>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="double"/> value specifying the <b>bits</b> to write data
        ///		from.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the little-endian <b>bit</b>
        ///		index to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<b>bits</b> to write.
        /// </param>
        /// <seealso cref="double"/>
        /// <seealso cref="int"/>
        public virtual void Write(double bits, int bitIndex, int count)
        {
            var l_bits = BitConverter.GetBytes(bits);
            var ulBits = l_bits[0] | ((ulong)l_bits[1]) << 8 | ((ulong)l_bits[2]) << 16 | ((ulong)l_bits[3]) << 24 |
                         ((ulong)l_bits[4]) << 32 | ((ulong)l_bits[5]) << 40 | ((ulong)l_bits[6]) << 48 | ((ulong)l_bits[7]) << 56;

            Write(ulBits, bitIndex, count);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="double"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="double"/> array specifying the buffer to write data from.
        /// </param>
        /// <seealso cref="double"/>
        public virtual void Write(double[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            Write(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Writes the <b>bits</b> contained in a <see cref="double"/> buffer to
        ///		the current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="double"/> array specifying the buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="double"/> offset
        ///		to begin writing from.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="double"/> values to write.
        /// </param>
        public virtual void Write(double[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            for (var iDoubleCounter = offset; iDoubleCounter < iEndIndex; iDoubleCounter++)
                Write(bits[iDoubleCounter]);
        }

        #endregion 64-Bit Writes [20051201]

        #region Miscellaneous

        /// <summary>
        ///		Writes the entire contents of this <b>bit</b> stream to another stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		Added [20051127]<br></br>
        ///		<br></br>
        ///		All write operations at the end of the <b>BitStream</b> expand the
        ///		<b>BitStream</b>.
        ///	</remarks>
        /// <param name="bits">
        ///		A <see cref="Stream"/> object specifying the stream to write this
        ///		<b>bit</b> stream to.
        /// </param>
        /// <seealso cref="Stream"/>
        public virtual void WriteTo(Stream bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_Stream"));

            var l_bits = ToByteArray();

            bits.Write(l_bits, 0, l_bits.Length);
        }

        #endregion Miscellaneous

        #endregion Write [20051201]

        #region Read [20051201]

        #region Generic Reads [20051115]

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="uint"/> value.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="uint"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="uint"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="uint"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="uint"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="uint"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="uint"/>
        private uint Read(ref uint bits, ref uint bitIndex, ref uint count)
        {
            if (bits <= 0) throw new ArgumentOutOfRangeException(nameof(bits));
            // Calculate the current position
            var uiBitBuffer_Position = (_uiBitBuffer_Index << BitBuffer_SizeOfElement_Shift) + _uiBitBuffer_BitIndex;

            // Determine if there are enough bits available to be read from the buffer
            var uiActualCount = count;
            if (_uiBitBuffer_Length < (uiBitBuffer_Position + uiActualCount))
                uiActualCount = _uiBitBuffer_Length - uiBitBuffer_Position;

            // Get current bit buffer element value
            var uiValue = _auiBitBuffer[_uiBitBuffer_Index];
            var iValue_BitsToShift = (int)(BitBuffer_SizeOfElement - (_uiBitBuffer_BitIndex + uiActualCount));

            if (iValue_BitsToShift < 0)
            {
                // Clear out unwanted bits in value
                iValue_BitsToShift = Math.Abs(iValue_BitsToShift);
                var uiValue_BitMask = (BitMaskHelperLUT[uiActualCount] >> iValue_BitsToShift);
                uiValue &= uiValue_BitMask;
                uiValue <<= iValue_BitsToShift;

                var uiRemainingCount = (uint)iValue_BitsToShift;
                uint uiBitIndex = 0;
                uint uiValueToAppend = 0;

                UpdateIndicesForRead(uiActualCount - uiRemainingCount);

                Read(ref uiValueToAppend, ref uiBitIndex, ref uiRemainingCount);

                uiValue |= uiValueToAppend;
            }
            else
            {
                // Clear out unwanted bits in value
                var uiValue_BitMask = (BitMaskHelperLUT[uiActualCount] << iValue_BitsToShift);
                uiValue &= uiValue_BitMask;
                uiValue >>= iValue_BitsToShift;

                UpdateIndicesForRead(uiActualCount);
            }

            bits = uiValue << (int)bitIndex;

            return uiActualCount;
        }

        #endregion Generic Reads [20051115]

        #region 1-Bit Reads [20051116]

        /// <summary>
        ///		Reads the <b>bit</b> contained in the current stream to a
        ///		<see cref="bool"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bit">
        ///		When this method returns, contains the specified <see cref="bool"/>
        ///		value with the <b>bit</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bit</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be 1 or zero if the end of the
        ///		current stream is reached before the <b>bit</b> is read.
        /// </returns>
        /// <seealso cref="bool"/>
        /// <seealso cref="int"/>
        public virtual int Read(out bool bit)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

            uint uiBitIndex = 0;
            uint uiCount = 1;
            uint uiBit = 0;
            var uiBitsRead = Read(ref uiBit, ref uiBitIndex, ref uiCount);

            bit = Convert.ToBoolean(uiBit);

            return (int)uiBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="bool"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="bool"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="bool"/>
        /// <seealso cref="int"/>
        public virtual int Read(bool[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="bool"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="bool"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="bool"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="bool"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="bool"/>
        /// <seealso cref="int"/>
        public virtual int Read(bool[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iBitCounter = offset; iBitCounter < iEndIndex; iBitCounter++)
                iBitsRead += Read(out bits[iBitCounter]);

            return iBitsRead;
        }

        #endregion 1-Bit Reads [20051116]

        #region 8-Bit Reads [20051124]

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="byte"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="byte"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="byte"/>
        /// <seealso cref="int"/>
        public virtual int Read(out byte bits)
        {
            return Read(out bits, 0, SizeOfByte);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="byte"/> value.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="byte"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="byte"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="byte"/>
        /// <seealso cref="int"/>
        public virtual int Read(out byte bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfByte - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_Byte"));

            var uiBitIndex = (uint)bitIndex;
            var uiCount = (uint)count;
            uint uiBits = 0;
            var uiBitsRead = Read(ref uiBits, ref uiBitIndex, ref uiCount);

            bits = (byte)uiBits;

            return (int)uiBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="byte"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="byte"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="byte"/>
        /// <seealso cref="int"/>
        public virtual int Read(byte[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="byte"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="byte"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="byte"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="byte"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="byte"/>
        /// <seealso cref="int"/>
        public override int Read(byte[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iByteCounter = offset; iByteCounter < iEndIndex; iByteCounter++)
                iBitsRead += Read(out bits[iByteCounter]);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="sbyte"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="sbyte"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="sbyte"/>
        /// <seealso cref="int"/>

        public virtual int Read(out sbyte bits)
        {
            return Read(out bits, 0, SizeOfByte);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="sbyte"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="sbyte"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="sbyte"/>
        /// <seealso cref="int"/>

        public virtual int Read(out sbyte bits, int bitIndex, int count)
        {
            var iBitsRead = Read(out byte bytBits, bitIndex, count);
            bits = (sbyte)bytBits;
            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="sbyte"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="sbyte"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="sbyte"/>
        /// <seealso cref="int"/>

        public virtual int Read(sbyte[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="sbyte"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="sbyte"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="sbyte"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="sbyte"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="sbyte"/>
        /// <seealso cref="int"/>

        public virtual int Read(sbyte[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iSByteCounter = offset; iSByteCounter < iEndIndex; iSByteCounter++)
                iBitsRead += Read(out bits[iSByteCounter]);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads a byte from the stream and advances the position within the
        ///		stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <remarks>
        ///		Modified [20051124]
        /// </remarks>
        /// <returns>
        ///		The unsigned byte cast to an <b>Int32</b>, or -1 if at the end of the
        ///		stream.
        /// </returns>
        public override int ReadByte()
        {
            var iBitsRead = Read(out byte bytBits);

            if (iBitsRead == 0)
                return -1;
            else
                return bytBits;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///		Added [20051124].
        /// </remarks>
        /// <returns>
        /// </returns>
        public virtual byte[] ToByteArray()
        {
            // Write this stream's internal buffer to a new byte buffer
            var lCurrentPos = Position;
            Position = 0;

            var l_bits = new byte[Length8];
            Read(l_bits, 0, (int)Length8);

            if (Position != lCurrentPos)
                Position = lCurrentPos;

            return l_bits;
        }

        #endregion 8-Bit Reads [20051124]

        #region 16-Bits Reads [20051115]

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="char"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="char"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="char"/>
        /// <seealso cref="int"/>
        public virtual int Read(out char bits)
        {
            return Read(out bits, 0, SizeOfChar);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="char"/> value.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="char"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="char"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="char"/>
        /// <seealso cref="int"/>
        public virtual int Read(out char bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfChar - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_Char"));

            var uiBitIndex = (uint)bitIndex;
            var uiCount = (uint)count;
            uint uiBits = 0;
            var uiBitsRead = Read(ref uiBits, ref uiBitIndex, ref uiCount);

            bits = (char)uiBits;

            return (int)uiBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="char"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="char"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="char"/>
        /// <seealso cref="int"/>
        public virtual int Read(char[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="char"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="char"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="char"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="char"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="char"/>
        /// <seealso cref="int"/>
        public virtual int Read(char[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iCharCounter = offset; iCharCounter < iEndIndex; iCharCounter++)
                iBitsRead += Read(out bits[iCharCounter]);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="ushort"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="ushort"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="ushort"/>
        /// <seealso cref="int"/>

        public virtual int Read(out ushort bits)
        {
            return Read(out bits, 0, SizeOfUInt16);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="ushort"/> value.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="ushort"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="ushort"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="ushort"/>
        /// <seealso cref="int"/>

        public virtual int Read(out ushort bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfUInt16 - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_UInt16"));

            var uiBitIndex = (uint)bitIndex;
            var uiCount = (uint)count;
            uint uiBits = 0;
            var uiBitsRead = Read(ref uiBits, ref uiBitIndex, ref uiCount);

            bits = (ushort)uiBits;

            return (int)uiBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="ushort"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="ushort"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="ushort"/>
        /// <seealso cref="int"/>

        public virtual int Read(ushort[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="ushort"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="ushort"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="ushort"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="ushort"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="ushort"/>
        /// <seealso cref="int"/>

        public virtual int Read(ushort[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iUInt16Counter = offset; iUInt16Counter < iEndIndex; iUInt16Counter++)
                iBitsRead += Read(out bits[iUInt16Counter]);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="short"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="short"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="short"/>
        /// <seealso cref="int"/>
        public virtual int Read(out short bits)
        {
            return Read(out bits, 0, SizeOfUInt16);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="short"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="short"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="short"/>
        /// <seealso cref="int"/>
        public virtual int Read(out short bits, int bitIndex, int count)
        {
            var iBitsRead = Read(out ushort usBits, bitIndex, count);

            bits = (short)usBits;

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="short"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="short"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="short"/>
        /// <seealso cref="int"/>
        public virtual int Read(short[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="short"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="short"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="short"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="short"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="short"/>
        /// <seealso cref="int"/>
        public virtual int Read(short[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iShortCounter = offset; iShortCounter < iEndIndex; iShortCounter++)
                iBitsRead += Read(out bits[iShortCounter]);

            return iBitsRead;
        }

        #endregion 16-Bits Reads [20051115]

        #region 32-Bit Reads [20051115]

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="uint"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="uint"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="uint"/>
        /// <seealso cref="int"/>

        public virtual int Read(out uint bits)
        {
            return Read(out bits, 0, SizeOfUInt32);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="uint"/> value.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="uint"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="uint"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="uint"/>
        /// <seealso cref="int"/>

        public virtual int Read(out uint bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfUInt32 - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_UInt32"));

            var uiBitIndex = (uint)bitIndex;
            var uiCount = (uint)count;
            uint uiBits = 0;
            var uiBitsRead = Read(ref uiBits, ref uiBitIndex, ref uiCount);

            bits = uiBits;

            return (int)uiBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="uint"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="uint"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="uint"/>
        /// <seealso cref="int"/>

        public virtual int Read(uint[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="uint"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="uint"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="uint"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="uint"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="uint"/>
        /// <seealso cref="int"/>

        public virtual int Read(uint[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iUInt32Counter = offset; iUInt32Counter < iEndIndex; iUInt32Counter++)
                iBitsRead += Read(out bits[iUInt32Counter]);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="int"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="int"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="int"/>
        public virtual int Read(out int bits)
        {
            return Read(out bits, 0, SizeOfUInt32);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="int"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="int"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="int"/>
        public virtual int Read(out int bits, int bitIndex, int count)
        {
            var iBitsRead = Read(out uint uiBits, bitIndex, count);

            bits = (int)uiBits;

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="int"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="int"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="int"/>
        public virtual int Read(int[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="int"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="int"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="int"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="int"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="int"/>
        public virtual int Read(int[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iInt32Counter = offset; iInt32Counter < iEndIndex; iInt32Counter++)
                iBitsRead += Read(out bits[iInt32Counter]);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="float"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="float"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="float"/>
        /// <seealso cref="int"/>
        public virtual int Read(out float bits)
        {
            return Read(out bits, 0, SizeOfSingle);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="float"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="float"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="float"/>
        /// <seealso cref="int"/>
        public virtual int Read(out float bits, int bitIndex, int count)
        {
            var uiBitsRead = Read(out int uiBits, bitIndex, count);

            bits = BitConverter.ToSingle(BitConverter.GetBytes(uiBits), 0);

            return uiBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="float"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="float"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="float"/>
        /// <seealso cref="int"/>
        public virtual int Read(float[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="float"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="float"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="float"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="float"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="float"/>
        /// <seealso cref="int"/>
        public virtual int Read(float[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iSingleCounter = offset; iSingleCounter < iEndIndex; iSingleCounter++)
                iBitsRead += Read(out bits[iSingleCounter]);

            return iBitsRead;
        }

        #endregion 32-Bit Reads [20051115]

        #region 64-bit Reads [20051201]

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="ulong"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.<br></br>
        ///		<br></br>
        ///		Fixed [20051201].
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="ulong"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="ulong"/>
        /// <seealso cref="int"/>

        public virtual int Read(out ulong bits)
        {
            return Read(out bits, 0, SizeOfUInt64);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="ulong"/> value.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>bitIndex</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>bitIndex</i> subtracted from the number of <b>bits</b> in a
        ///		<see cref="ulong"/> is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="ulong"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="ulong"/>
        /// <seealso cref="int"/>

        public virtual int Read(out ulong bits, int bitIndex, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bitIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (SizeOfUInt64 - bitIndex))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrBitIndex_UInt64"));

            var iBitIndex1 = (bitIndex >> 5) < 1 ? bitIndex : -1;
            var iBitIndex2 = (bitIndex + count) > 32 ? (iBitIndex1 < 0 ? bitIndex - 32 : 0) : -1;
            var iCount1 = iBitIndex1 > -1 ? (iBitIndex1 + count > 32 ? 32 - iBitIndex1 : count) : 0;
            var iCount2 = iBitIndex2 > -1 ? (iCount1 == 0 ? count : count - iCount1) : 0;

            uint uiBitsRead = 0;
            uint uiBits1 = 0;
            uint uiBits2 = 0;
            if (iCount1 > 0)
            {
                var uiBitIndex1 = (uint)iBitIndex1;
                var uiCount1 = (uint)iCount1;
                uiBitsRead = Read(ref uiBits1, ref uiBitIndex1, ref uiCount1);
            }
            if (iCount2 > 0)
            {
                var uiBitIndex2 = (uint)iBitIndex2;
                var uiCount2 = (uint)iCount2;
                uiBitsRead += Read(ref uiBits2, ref uiBitIndex2, ref uiCount2);
            }

            bits = ((ulong)uiBits2 << 32) | uiBits1;

            return (int)uiBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="ulong"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="ulong"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="ulong"/>
        /// <seealso cref="int"/>

        public virtual int Read(ulong[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="ulong"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="ulong"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="ulong"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="ulong"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="ulong"/>
        /// <seealso cref="int"/>

        public virtual int Read(ulong[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iUInt64Counter = offset; iUInt64Counter < iEndIndex; iUInt64Counter++)
                iBitsRead += Read(out bits[iUInt64Counter]);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="long"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="long"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="long"/>
        /// <seealso cref="int"/>
        public virtual int Read(out long bits)
        {
            return Read(out bits, 0, SizeOfUInt64);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="long"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="long"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="long"/>
        /// <seealso cref="int"/>
        public virtual int Read(out long bits, int bitIndex, int count)
        {
            var iBitsRead = Read(out ulong ulBits, bitIndex, count);

            bits = (long)ulBits;

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="long"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="long"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="long"/>
        /// <seealso cref="int"/>
        public virtual int Read(long[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to an
        ///		<see cref="long"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="long"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="long"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="long"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="long"/>
        /// <seealso cref="int"/>
        public virtual int Read(long[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iInt64Counter = offset; iInt64Counter < iEndIndex; iInt64Counter++)
                iBitsRead += Read(out bits[iInt64Counter]);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="double"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="double"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="double"/>
        /// <seealso cref="int"/>
        public virtual int Read(out double bits)
        {
            return Read(out bits, 0, SizeOfDouble);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="double"/> value.
        /// </summary>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="double"/>
        ///		value with the <b>bits</b> between bitIndex and (bitIndex + count - 1)
        ///		replaced by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="bitIndex">
        ///		An <see cref="int"/> value specifying the <b>bit</b> index at which to
        ///		begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of <b>bits</b>
        ///		to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the value. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="double"/>
        /// <seealso cref="int"/>
        public virtual int Read(out double bits, int bitIndex, int count)
        {
            var iBitsRead = Read(out ulong ulBits, bitIndex, count);

            bits = BitConverter.ToDouble(BitConverter.GetBytes(ulBits), 0);

            return iBitsRead;
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="double"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="double"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="double"/>
        /// <seealso cref="int"/>
        public virtual int Read(double[] bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));

            return Read(bits, 0, bits.Length);
        }

        /// <summary>
        ///		Reads the <b>bits</b> contained in the current stream to a
        ///		<see cref="double"/> buffer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<i>offset</i> or <i>count</i> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<i>offset</i> subtracted from the buffer length is less than <i>count</i>.
        /// </exception>
        /// <remarks>
        ///		The <b>Read</b> method returns zero if the end of the current stream
        ///		is reached. In all other cases, <b>Read</b> always reads at least one
        ///		<b>bit</b> from the current stream before returning.
        /// </remarks>
        /// <param name="bits">
        ///		When this method returns, contains the specified <see cref="double"/>
        ///		array with the values between offset and (offset + count - 1) replaced
        ///		by the <b>bits</b> read from the current stream.
        /// </param>
        /// <param name="offset">
        ///		An <see cref="int"/> value specifying the <see cref="double"/> offset
        ///		at which to begin reading.
        /// </param>
        /// <param name="count">
        ///		An <see cref="int"/> value specifying the maximum number of
        ///		<see cref="double"/> values to read.
        /// </param>
        /// <returns>
        ///		An <see cref="int"/> value specifying the number of <b>bits</b>
        ///		written into the buffer. This can be less than the number of <b>bits</b>
        ///		requested if that number of <b>bits</b> are not currently available,
        ///		or zero if the end of the current stream is reached before any
        ///		<b>bits</b> are read.
        /// </returns>
        /// <seealso cref="double"/>
        /// <seealso cref="int"/>
        public virtual int Read(double[] bits, int offset, int count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitBuffer"));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), BitStreamResources.GetString("ArgumentOutOfRange_NegativeParameter"));
            if (count > (bits.Length - offset))
                throw new ArgumentException(BitStreamResources.GetString("Argument_InvalidCountOrOffset"));

            var iEndIndex = offset + count;
            var iBitsRead = 0;
            for (var iDoubleCounter = offset; iDoubleCounter < iEndIndex; iDoubleCounter++)
                iBitsRead += Read(out bits[iDoubleCounter]);

            return iBitsRead;
        }

        #endregion 64-bit Reads [20051201]

        #endregion Read [20051201]

        #region Logical Operations [20051115]

        /// <summary>
        ///		Performs a bitwise <b>AND</b> operation on the <b>bits</b> in the
        ///		current stream against the corresponding <b>bits</b> in the specified
        ///		stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		The stream specified by the <i>bits</i> parameter and the current
        ///		stream do not have the same number of <b>bits</b>.
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="BitStream"/> object with which to perform the bitwise
        ///		<b>AND</b> operation.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object containing the result of the bitwise
        ///		<b>AND</b> operation on the <b>bits</b> in the current stream against
        ///		the corresponding <b>bits</b> in the specified stream.
        /// </returns>
        /// <seealso cref="BitStream"/>
        public virtual BitStream And(BitStream bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitStream"));
            if (bits.Length != _uiBitBuffer_Length)
                throw new ArgumentException(BitStreamResources.GetString("Argument_DifferentBitStreamLengths"));

            // Create the new BitStream
            var newBitStream = new BitStream(_uiBitBuffer_Length);

            var uiWholeUInt32Lengths = _uiBitBuffer_Length >> BitBuffer_SizeOfElement_Shift;
            uint uiCounter;

            for (uiCounter = 0; uiCounter < uiWholeUInt32Lengths; uiCounter++)
                newBitStream._auiBitBuffer[uiCounter] = _auiBitBuffer[uiCounter] & bits._auiBitBuffer[uiCounter];

            // Are there any further bits in the buffer?
            if ((_uiBitBuffer_Length & BitBuffer_SizeOfElement_Mod) <= 0) return newBitStream;
            var uiBitMask = uint.MaxValue << (int)(BitBuffer_SizeOfElement - (_uiBitBuffer_Length & BitBuffer_SizeOfElement_Mod));
            newBitStream._auiBitBuffer[uiCounter] = _auiBitBuffer[uiCounter] & bits._auiBitBuffer[uiCounter] & uiBitMask;

            return newBitStream;
        }

        /// <summary>
        ///		Performs a bitwise <b>OR</b> operation on the <b>bits</b> in the
        ///		current stream against the corresponding <b>bits</b> in the specified
        ///		stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		The stream specified by the <i>bits</i> parameter and the current
        ///		stream do not have the same number of <b>bits</b>.
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="BitStream"/> object with which to perform the bitwise
        ///		<b>OR</b> operation.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object containing the result of the bitwise
        ///		<b>OR</b> operation on the <b>bits</b> in the current stream against
        ///		the corresponding <b>bits</b> in the specified stream.
        /// </returns>
        /// <seealso cref="BitStream"/>
        public virtual BitStream Or(BitStream bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitStream"));
            if (bits.Length != _uiBitBuffer_Length)
                throw new ArgumentException(BitStreamResources.GetString("Argument_DifferentBitStreamLengths"));

            // Create the new BitStream
            var newBitStream = new BitStream(_uiBitBuffer_Length);

            var uiWholeUInt32Lengths = _uiBitBuffer_Length >> BitBuffer_SizeOfElement_Shift;
            uint uiCounter;

            for (uiCounter = 0; uiCounter < uiWholeUInt32Lengths; uiCounter++)
                newBitStream._auiBitBuffer[uiCounter] = _auiBitBuffer[uiCounter] | bits._auiBitBuffer[uiCounter];

            // Are there any further bits in the buffer?
            if ((_uiBitBuffer_Length & BitBuffer_SizeOfElement_Mod) <= 0) return newBitStream;
            var uiBitMask = uint.MaxValue << (int)(BitBuffer_SizeOfElement - (_uiBitBuffer_Length & BitBuffer_SizeOfElement_Mod));
            newBitStream._auiBitBuffer[uiCounter] = _auiBitBuffer[uiCounter] | bits._auiBitBuffer[uiCounter] & uiBitMask;

            return newBitStream;
        }

        /// <summary>
        ///		Performs a bitwise e<b>X</b>clusive <b>OR</b> operation on the
        ///		<b>bits</b> in the current stream against the corresponding <b>bits</b>
        ///		in the specified stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		The stream specified by the <i>bits</i> parameter and the current
        ///		stream do not have the same number of <b>bits</b>.
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="BitStream"/> object with which to perform the bitwise
        ///		e<b>X</b>clusive <b>OR</b> operation.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object containing the result of the bitwise
        ///		e<b>X</b>clusive <b>OR</b> operation on the <b>bits</b> in the current
        ///		stream against the corresponding <b>bits</b> in the specified stream.
        /// </returns>
        /// <seealso cref="BitStream"/>
        public virtual BitStream Xor(BitStream bits)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitStream"));
            if (bits.Length != _uiBitBuffer_Length)
                throw new ArgumentException(BitStreamResources.GetString("Argument_DifferentBitStreamLengths"));

            // Create the new BitStream
            var newBitStream = new BitStream(_uiBitBuffer_Length);

            var uiWholeUInt32Lengths = _uiBitBuffer_Length >> BitBuffer_SizeOfElement_Shift;
            uint uiCounter;

            for (uiCounter = 0; uiCounter < uiWholeUInt32Lengths; uiCounter++)
                newBitStream._auiBitBuffer[uiCounter] = _auiBitBuffer[uiCounter] ^ bits._auiBitBuffer[uiCounter];

            // Are there any further bits in the buffer?
            if ((_uiBitBuffer_Length & BitBuffer_SizeOfElement_Mod) <= 0) return newBitStream;
            var uiBitMask = uint.MaxValue << (int)(BitBuffer_SizeOfElement - (_uiBitBuffer_Length & BitBuffer_SizeOfElement_Mod));
            newBitStream._auiBitBuffer[uiCounter] = _auiBitBuffer[uiCounter] ^ bits._auiBitBuffer[uiCounter] & uiBitMask;

            return newBitStream;
        }

        /// <summary>
        ///		Performs a bitwise <b>NOT</b> operation on the <b>bits</b> in the
        ///		current stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <returns>
        ///		A <see cref="BitStream"/> object containing the result of the bitwise
        ///		<b>NOT</b> operation on the <b>bits</b> in the current stream.
        /// </returns>
        /// <seealso cref="BitStream"/>
        public virtual BitStream Not()
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

            // Create the new BitStream
            var newBitStream = new BitStream(_uiBitBuffer_Length);

            var uiWholeUInt32Lengths = _uiBitBuffer_Length >> BitBuffer_SizeOfElement_Shift;
            uint uiCounter;

            for (uiCounter = 0; uiCounter < uiWholeUInt32Lengths; uiCounter++)
                newBitStream._auiBitBuffer[uiCounter] = ~_auiBitBuffer[uiCounter];

            // Are there any further bits in the buffer?
            if ((_uiBitBuffer_Length & BitBuffer_SizeOfElement_Mod) <= 0) return newBitStream;
            var uiBitMask = uint.MaxValue << (int)(BitBuffer_SizeOfElement - (_uiBitBuffer_Length & BitBuffer_SizeOfElement_Mod));
            newBitStream._auiBitBuffer[uiCounter] = ~_auiBitBuffer[uiCounter] & uiBitMask;

            return newBitStream;
        }

        #endregion Logical Operations [20051115]

        #region Bit Shifting [20051116]

        /// <summary>
        ///		Moves the <b>bits</b> of the current stream to the left
        ///		by the specified number places.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="count">
        ///		An <see cref="ulong"/> that represents the specified number of places.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object containing the result of left shift
        ///		operation on the <b>bits</b> in the current stream.
        /// </returns>
        public virtual BitStream ShiftLeft(long count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

            // Create a copy of the current stream
            var newBitStream = Copy();

            var uiCount = (uint)count;
            var uiLength = (uint)newBitStream.Length;

            if (uiCount >= uiLength)
            {
                // Clear out all bits
                newBitStream.Position = 0;

                for (uint uiBitCounter = 0; uiBitCounter < uiLength; uiBitCounter++)
                    newBitStream.Write(false);
            }
            else // count < Length
            {
                for (uint uiBitCounter = 0; uiBitCounter < uiLength - uiCount; uiBitCounter++)
                {
                    newBitStream.Position = uiCount + uiBitCounter;
                    newBitStream.Read(out bool blnBit);
                    newBitStream.Position = uiBitCounter;
                    newBitStream.Write(blnBit);
                }

                // Clear out the last count bits
                for (var uiBitCounter = uiLength - uiCount; uiBitCounter < uiLength; uiBitCounter++)
                    newBitStream.Write(false);
            }

            newBitStream.Position = 0;

            return newBitStream;
        }

        /// <summary>
        ///		Moves the <b>bits</b> of the current stream to the right
        ///		by the specified number places.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        ///		The current stream is closed.
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="count">
        ///		An <see cref="ulong"/> that represents the specified number of places.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object containing the result of right shift
        ///		operation on the <b>bits</b> in the current stream.
        /// </returns>
        public virtual BitStream ShiftRight(long count)
        {
            if (!_blnIsOpen)
                throw new ObjectDisposedException(BitStreamResources.GetString("ObjectDisposed_BitStreamClosed"));

            // Create a copy of the current stream
            var newBitStream = Copy();

            var uiCount = (uint)count;
            var uiLength = (uint)newBitStream.Length;

            if (uiCount >= uiLength)
            {
                // Clear out all bits
                newBitStream.Position = 0;

                for (uint uiBitCounter = 0; uiBitCounter < uiLength; uiBitCounter++)
                    newBitStream.Write(false);
            }
            else // count < Length
            {
                for (uint uiBitCounter = 0; uiBitCounter < uiLength - uiCount; uiBitCounter++)
                {
                    newBitStream.Position = uiBitCounter;
                    newBitStream.Read(out bool blnBit);
                    newBitStream.Position = uiBitCounter + uiCount;
                    newBitStream.Write(blnBit);
                }

                // Clear out the first count bits
                newBitStream.Position = 0;
                for (uint uiBitCounter = 0; uiBitCounter < uiCount; uiBitCounter++)
                    newBitStream.Write(false);
            }

            newBitStream.Position = 0;

            return newBitStream;
        }

        #endregion Bit Shifting [20051116]

        #region ToString [2001116]

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the current stream
        ///		in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <returns>
        ///		A <see cref="string"/> object representing the current stream
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        public override string ToString()
        {
            var uiWholeUInt32Lengths = _uiBitBuffer_Length >> 5;
            uint uiCounter;
            int iBitCounter;
            const uint uintOne = 1;

            var sb = new StringBuilder((int)_uiBitBuffer_Length);

            for (uiCounter = 0; uiCounter < uiWholeUInt32Lengths; uiCounter++)
            {
                sb.Append("[" + uiCounter.ToString(_ifp) + "]:{");
                for (iBitCounter = 31; iBitCounter >= 0; iBitCounter--)
                {
                    var uiBitMask = uintOne << iBitCounter;

                    sb.Append((_auiBitBuffer[uiCounter] & uiBitMask) == uiBitMask ? '1' : '0');
                }
                sb.Append("}\r\n");
            }

            // Are there any further bits in the buffer?
            if ((_uiBitBuffer_Length & 31) <= 0) return sb.ToString();
            {
                sb.Append("[" + uiCounter.ToString(_ifp) + "]:{");
                var iBitCounterMin = (int)(32 - (_uiBitBuffer_Length & 31));

                for (iBitCounter = 31; iBitCounter >= iBitCounterMin; iBitCounter--)
                {
                    var uiBitMask = uintOne << iBitCounter;

                    sb.Append((_auiBitBuffer[uiCounter] & uiBitMask) == uiBitMask ? '1' : '0');
                }

                for (iBitCounter = iBitCounterMin - 1; iBitCounter >= 0; iBitCounter--)
                    sb.Append('.');

                sb.Append("}\r\n");
            }

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bit">
        ///		A <see cref="bool"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="bool"/>
        public static string ToString(bool bit)
        {
            var str = "bool{" + (bit ? 1 : 0) + "}";
            return str;
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="byte"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="byte"/>
        public static string ToString(byte bits)
        {
            var sb = new StringBuilder(8);
            const uint uintOne = 1;

            sb.Append("Byte{");
            for (var iBitCounter = 7; iBitCounter >= 0; iBitCounter--)
            {
                var uiBitMask = uintOne << iBitCounter;

                sb.Append((bits & uiBitMask) == uiBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="sbyte"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="sbyte"/>

        public static string ToString(sbyte bits)
        {
            var bytBits = (byte)bits;

            var sb = new StringBuilder(8);
            const uint uintOne = 1;

            sb.Append("SByte{");
            for (var iBitCounter = 7; iBitCounter >= 0; iBitCounter--)
            {
                var uiBitMask = uintOne << iBitCounter;

                sb.Append((bytBits & uiBitMask) == uiBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="char"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="char"/>
        public static string ToString(char bits)
        {
            var sb = new StringBuilder(16);
            const uint uintOne = 1;

            sb.Append("Char{");
            for (var iBitCounter = 15; iBitCounter >= 0; iBitCounter--)
            {
                var uiBitMask = uintOne << iBitCounter;

                sb.Append((bits & uiBitMask) == uiBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="ushort"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="ushort"/>

        public static string ToString(ushort bits)
        {
            var sBits = (short)bits;

            var sb = new StringBuilder(16);
            const uint uintOne = 1;

            sb.Append("UInt16{");
            for (var iBitCounter = 15; iBitCounter >= 0; iBitCounter--)
            {
                var uiBitMask = uintOne << iBitCounter;

                sb.Append((sBits & uiBitMask) == uiBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="short"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="short"/>
        public static string ToString(short bits)
        {
            var sb = new StringBuilder(16);
            const uint uintOne = 1;

            sb.Append("Int16{");
            for (var iBitCounter = 15; iBitCounter >= 0; iBitCounter--)
            {
                var uiBitMask = uintOne << iBitCounter;

                sb.Append((bits & uiBitMask) == uiBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="uint"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="uint"/>

        public static string ToString(uint bits)
        {
            var sb = new StringBuilder(32);
            const uint uintOne = 1;

            sb.Append("UInt32{");
            for (var iBitCounter = 31; iBitCounter >= 0; iBitCounter--)
            {
                var uiBitMask = uintOne << iBitCounter;

                sb.Append((bits & uiBitMask) == uiBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="int"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="int"/>
        public static string ToString(int bits)
        {
            var uiBits = (uint)bits;

            var sb = new StringBuilder(32);
            const uint uintOne = 1;

            sb.Append("Int32{");
            for (var iBitCounter = 31; iBitCounter >= 0; iBitCounter--)
            {
                var uiBitMask = uintOne << iBitCounter;

                sb.Append((uiBits & uiBitMask) == uiBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="ulong"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="ulong"/>

        public static string ToString(ulong bits)
        {
            var sb = new StringBuilder(64);
            const ulong ulongOne = 1;

            sb.Append("UInt64{");
            for (var iBitCounter = 63; iBitCounter >= 0; iBitCounter--)
            {
                var ulBitMask = ulongOne << iBitCounter;

                sb.Append((bits & ulBitMask) == ulBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="long"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="long"/>
        public static string ToString(long bits)
        {
            var ulBits = (ulong)bits;

            var sb = new StringBuilder(64);
            const ulong ulongOne = 1;

            sb.Append("Int64{");
            for (var iBitCounter = 63; iBitCounter >= 0; iBitCounter--)
            {
                var ulBitMask = ulongOne << iBitCounter;

                sb.Append((ulBits & ulBitMask) == ulBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="float"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="float"/>
        public static string ToString(float bits)
        {
            var l_bits = BitConverter.GetBytes(bits);
            var uiBits = l_bits[0] | ((uint)l_bits[1]) << 8 | ((uint)l_bits[2]) << 16 | ((uint)l_bits[3]) << 24;

            var sb = new StringBuilder(32);
            const uint uintOne = 1;

            sb.Append("Single{");
            for (var iBitCounter = 31; iBitCounter >= 0; iBitCounter--)
            {
                var uiBitMask = uintOne << iBitCounter;

                sb.Append((uiBits & uiBitMask) == uiBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        ///		Returns a <see cref="string"/> that represents the specified
        ///		value in binary notation.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="double"/> value representing the specified value.
        /// </param>
        /// <returns>
        ///		A <see cref="string"/> object representing the specified value
        ///		in binary notation.
        /// </returns>
        /// <seealso cref="string"/>
        /// <seealso cref="double"/>
        public static string ToString(double bits)
        {
            var l_bits = BitConverter.GetBytes(bits);
            var ulBits = l_bits[0] | ((ulong)l_bits[1]) << 8 | ((ulong)l_bits[2]) << 16 | ((ulong)l_bits[3]) << 24 |
                         ((ulong)l_bits[4]) << 32 | ((ulong)l_bits[5]) << 40 | ((ulong)l_bits[6]) << 48 | ((ulong)l_bits[7]) << 56;

            var sb = new StringBuilder(64);
            const ulong ul1 = 1;

            sb.Append("Double{");
            for (var iBitCounter = 63; iBitCounter >= 0; iBitCounter--)
            {
                var ulBitMask = ul1 << iBitCounter;

                sb.Append((ulBits & ulBitMask) == ulBitMask ? '1' : '0');
            }
            sb.Append("}");

            return sb.ToString();
        }

        #endregion ToString [2001116]

        #region General [20051116]

        #region Private [20051116]

        /// <summary>
        ///		Updates the length of the internal buffer after wrinting to the
        ///		current stream by the specified number of <b>bits</b>.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="uint"/> value defining the specified
        ///		number of <b>bits</b>.
        /// </param>
        /// <seealso cref="uint"/>
        private void UpdateLengthForWrite(uint bits)
        {
            // Increment _uiBitBuffer_Length
            _uiBitBuffer_Length += bits;
        }

        /// <summary>
        ///		Updates the internal buffer's <b>bit</b> indices after writing to the
        ///		current stream by the specified number of <b>bits</b>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///		The internal buffer's <b>bit</b> index is greater than 32.
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="uint"/> value defining the specified
        ///		number of <b>bits</b>.
        /// </param>
        /// <seealso cref="uint"/>
        private void UpdateIndicesForWrite(uint bits)
        {
            // Increment _uiBitBuffer_BitIndex
            _uiBitBuffer_BitIndex += bits;

            if (_uiBitBuffer_BitIndex == BitBuffer_SizeOfElement)
            {
                // Increment _uiBitBuffer_Index
                _uiBitBuffer_Index++;

                // Reset the bit index
                _uiBitBuffer_BitIndex = 0;

                // Redimension the bit buffer if necessary
                if (_auiBitBuffer.Length == (_uiBitBuffer_Length >> BitBuffer_SizeOfElement_Shift))
                    _auiBitBuffer = ReDimPreserve(_auiBitBuffer, (uint)_auiBitBuffer.Length << 1);
            }
            else if (_uiBitBuffer_BitIndex > BitBuffer_SizeOfElement)
                throw new InvalidOperationException(BitStreamResources.GetString("InvalidOperation_BitIndexGreaterThan32"));
        }

        /// <summary>
        ///		Updates the internal buffer's <b>bit</b> indices after reading to the
        ///		current stream by the specified number of <b>bits</b>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///		The internal buffer's <b>bit</b> index is greater than 32.
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		An <see cref="uint"/> value defining the specified
        ///		number of <b>bits</b>.
        /// </param>
        /// <seealso cref="uint"/>
        private void UpdateIndicesForRead(uint bits)
        {
            // Increment _uiBitBuffer_BitIndex
            _uiBitBuffer_BitIndex += bits;
            if (_uiBitBuffer_BitIndex == BitBuffer_SizeOfElement)
            {
                // Increment _uiBitBuffer_Index
                _uiBitBuffer_Index++;

                // Reset the bit index
                _uiBitBuffer_BitIndex = 0;
            }
            else if (_uiBitBuffer_BitIndex > BitBuffer_SizeOfElement)
                throw new InvalidOperationException(BitStreamResources.GetString("InvalidOperation_BitIndexGreaterThan32"));
        }

        /// <summary>
        ///		Re-dimensions and preserves the contents of a buffer by the specified
        ///		amount.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="buffer">
        ///		An <see cref="uint"/> array specifying the buffer to re-dimension.
        /// </param>
        /// <param name="newLength">
        ///		An <see cref="uint"/> value specifying the new length of the buffer.
        /// </param>
        /// <returns>
        ///		An <see cref="uint"/> array secifying the re-dimensioned array.
        /// </returns>
        /// <seealso cref="uint"/>
        private static uint[] ReDimPreserve(uint[] buffer, uint newLength)
        {
            var auiNewBuffer = new uint[newLength];

            var uiBufferLength = (uint)buffer.Length;

            if (uiBufferLength < newLength)
                Buffer.BlockCopy(buffer, 0, auiNewBuffer, 0, (int)(uiBufferLength << 2));
            else // buffer.Length >= newLength
                Buffer.BlockCopy(buffer, 0, auiNewBuffer, 0, (int)(newLength << 2));

            // Free the previously allocated buffer
            // buffer = null;

            return auiNewBuffer;
        }

        #endregion Private [20051116]

        #region Public [20051116]

        /// <summary>
        ///		Closes the current stream for reading and writing.
        /// </summary>
        /// <remarks>
        ///		.
        /// </remarks>
        public override void Close()
        {
            _blnIsOpen = false;
            // Reset indices
            _uiBitBuffer_Index = 0;
            _uiBitBuffer_BitIndex = 0;
        }

        /// <summary>
        ///		Returns the array of unsigned integers from which this stream was
        ///		created.
        /// </summary>
        /// <remarks>
        ///		This method works even when the current stream is closed.
        /// </remarks>
        /// <returns>
        ///		The integer array from which this stream was created.
        /// </returns>

        public virtual uint[] GetBuffer()
        {
            return _auiBitBuffer;
        }

        /// <summary>
        ///		Creates a copy of the current stream.
        /// </summary>
        /// <remarks>
        ///		This method works even when the current stream is closed.
        /// </remarks>
        /// <returns>
        ///		A <see cref="BitStream"/> object representing the copy of the current
        ///		stream.
        /// </returns>
        /// <seealso cref="BitStream"/>
        public virtual BitStream Copy()
        {
            var bitStreamCopy = new BitStream(Length);

            Buffer.BlockCopy(_auiBitBuffer, 0, bitStreamCopy._auiBitBuffer, 0, bitStreamCopy._auiBitBuffer.Length << 2);

            bitStreamCopy._uiBitBuffer_Length = _uiBitBuffer_Length;

            return bitStreamCopy;
        }

        #endregion Public [20051116]

        #endregion General [20051116]

        #region Not Supported [20051116]

        /// <summary>
        ///		Begins an asynchronous read operation.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///		This method is not supported.
        /// </exception>
        /// <remarks>
        ///		<b><font color="red">Notes to Callers:</font></b> This method is not
        ///		supported, and cannot be used. Asynchronous operations are not
        ///		supported by the <see cref="BitStream"/> class.
        /// </remarks>
        /// <param name="buffer">
        ///		The buffer to read the data into.
        /// </param>
        /// <param name="offset">
        ///		The byte offset in <i>buffer</i> at which to begin writing data read
        ///		from the stream.
        /// </param>
        /// <param name="count">
        ///		The maximum number of bytes to read.
        /// </param>
        /// <param name="callback">
        ///		An optional asynchronous callback, to be called when the read is
        ///		complete.
        /// </param>
        /// <param name="state">
        ///		A user-provided object that distinguishes this particular asynchronous
        ///		read request from other requests.
        /// </param>
        /// <returns>
        ///		An <see cref="IAsyncResult"/> that represents the asynchronous read,
        ///		which could still be pending.
        /// </returns>
        /// <seealso cref="BitStream"/>
        /// <seealso cref="IAsyncResult"/>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException(BitStreamResources.GetString("NotSupported_AsyncOps"));
        }

        /// <summary>
        ///		Begins an asynchronous write operation.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///		This method is not supported.
        /// </exception>
        /// <remarks>
        ///		<b><font color="red">Notes to Callers:</font></b> This method is not
        ///		supported, and cannot be used. Asynchronous operations are not
        ///		supported by the <see cref="BitStream"/> class.
        /// </remarks>
        /// <param name="buffer">
        ///		The buffer to write data from.
        /// </param>
        /// <param name="offset">
        ///		The byte offset in <i>buffer</i> from which to begin writing.
        /// </param>
        /// <param name="count">
        ///		The maximum number of bytes to write.
        /// </param>
        /// <param name="callback">
        ///		An optional asynchronous callback, to be called when the write is
        ///		complete.
        /// </param>
        /// <param name="state">
        ///		A user-provided object that distinguishes this particular asynchronous
        ///		write request from other requests.
        /// </param>
        /// <returns>
        ///		An <see cref="IAsyncResult"/> that represents the asynchronous write,
        ///		which could still be pending.
        /// </returns>
        /// <seealso cref="BitStream"/>
        /// <seealso cref="IAsyncResult"/>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException(BitStreamResources.GetString("NotSupported_AsyncOps"));
        }

        /// <summary>
        ///		Waits for the pending asynchronous read to complete.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///		This method is not supported.
        /// </exception>
        /// <remarks>
        ///		<b><font color="red">Notes to Callers:</font></b> This method is not
        ///		supported, and cannot be used. Asynchronous operations are not
        ///		supported by the <see cref="BitStream"/> class.
        /// </remarks>
        /// <param name="asyncResult">
        ///		The reference to the pending asynchronous request to finish.
        /// </param>
        /// <returns>
        ///		The number of bytes read from the stream, between zero (0) and the
        ///		number of bytes you requested. Streams only return zero (0) at the
        ///		end of the stream, otherwise, they should block until at least one
        ///		byte is available.
        /// </returns>
        ///	<seealso cref="BitStream"/>
        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotSupportedException(BitStreamResources.GetString("NotSupported_AsyncOps"));
        }

        /// <summary>
        ///		Ends an asynchronous write operation.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///		This method is not supported.
        /// </exception>
        /// <remarks>
        ///		<b><font color="red">Notes to Callers:</font></b> This method is not
        ///		supported, and cannot be used. Asynchronous operations are not
        ///		supported by the <see cref="BitStream"/> class.
        /// </remarks>
        /// <param name="asyncResult">
        ///		A reference to the outstanding asynchronous I/O request.
        /// </param>
        /// <seealso cref="BitStream"/>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException(BitStreamResources.GetString("NotSupported_AsyncOps"));
        }

        /// <summary>
        ///		When overridden in a derived class, sets the position within the
        ///		current stream.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///		This method is not supported.
        /// </exception>
        /// <remarks>
        ///		<b><font color="red">Notes to Callers:</font></b> This method is not
        ///		supported, and cannot be used. To set the position within the current
        ///		stream use the <see cref="Position"/> property instead.
        /// </remarks>
        /// <param name="offset">
        ///		A byte offset relative to the origin parameter.
        /// </param>
        /// <param name="origin">
        ///		A value of type <see cref="SeekOrigin"/> indicating the reference point
        ///		used to obtain the new position.
        /// </param>
        /// <returns>
        ///		The new position within the current stream.
        /// </returns>
        /// <seealso cref="Position"/>
        /// <seealso cref="SeekOrigin"/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(BitStreamResources.GetString("NotSupported_Seek"));
        }

        /// <summary>
        ///		When overridden in a derived class, sets the length of the current
        ///		stream.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///		This method is not supported.
        /// </exception>
        /// <remarks>
        ///		<b><font color="red">Notes to Callers:</font></b> This method is not
        ///		supported, and cannot be used. All write operations at the end of the
        ///		<b>BitStream</b> expand the <b>BitStream</b> automatically.
        /// </remarks>
        /// <param name="value">
        ///		The desired length of the current stream in bytes.
        /// </param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException(BitStreamResources.GetString("NotSupported_SetLength"));
        }

        /// <summary>
        ///		When overridden in a derived class, clears all buffers for this stream
        ///		and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///		This method is not supported. Since any data written to a
        ///		<see cref="BitStream"/> is written into RAM, this method is
        ///		redundant.
        /// </exception>
        /// <remarks>
        ///		<b><font color="red">Notes to Callers:</font></b> This method is not
        ///		supported, and cannot be used.
        /// </remarks>
        public override void Flush()
        {
            throw new NotSupportedException(BitStreamResources.GetString("NotSupported_Flush"));
        }

        #endregion Not Supported [20051116]

        #endregion Methods [20051201]

        #region Implicit Operators [20051125]

        /// <summary>
        ///		Converts a <see cref="MemoryStream"/> object to a new instance of the
        ///		<see cref="BitStream"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="MemoryStream"/> object to convert.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object representing the new instance of the
        ///		<see cref="BitStream"/> class.
        /// </returns>
        /// <seealso cref="MemoryStream"/>
        /// <seealso cref="BitStream"/>
        public static implicit operator BitStream(MemoryStream bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_MemoryStream"));

            return new BitStream(bits);
        }

        /// <summary>
        ///		Converts a <see cref="BitStream"/> object to a new instance of the
        ///		<see cref="MemoryStream"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="BitStream"/> object to convert.
        /// </param>
        /// <returns>
        ///		A <see cref="MemoryStream"/> object representing the new instance of
        ///		the <see cref="MemoryStream"/> class.
        /// </returns>
        /// <seealso cref="BitStream"/>
        /// <seealso cref="MemoryStream"/>
        public static implicit operator MemoryStream(BitStream bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitStream"));

            return new MemoryStream(bits.ToByteArray());
        }

        /// <summary>
        ///		Converts a <see cref="FileStream"/> object to a new instance of the
        ///		<see cref="BitStream"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		This operator allows implicit casting from an instance of a
        ///		<see cref="FileStream"/> object to a new instance of a
        ///		<see cref="BitStream"/> object. No equivalent operator has been made
        ///		available that allows implicit casting from an instance of a
        ///		<see cref="BitStream"/> object to a new instance of a
        ///		<see cref="FileStream"/> object.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="FileStream"/> object to convert.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object representing the new instance of the
        ///		<see cref="BitStream"/> class.
        /// </returns>
        /// <seealso cref="FileStream"/>
        /// <seealso cref="BitStream"/>
        public static implicit operator BitStream(FileStream bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_FileStream"));

            return new BitStream(bits);
        }

        /// <summary>
        ///		Converts a <see cref="BufferedStream"/> object to a new instance of the
        ///		<see cref="BitStream"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="BufferedStream"/> object to convert.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object representing the new instance of the
        ///		<see cref="BitStream"/> class.
        /// </returns>
        /// <seealso cref="BufferedStream"/>
        /// <seealso cref="BitStream"/>
        public static implicit operator BitStream(BufferedStream bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BufferedStream"));

            return new BitStream(bits);
        }

        /// <summary>
        ///		Converts a <see cref="BitStream"/> object to a new instance of the
        ///		<see cref="BufferedStream"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="BitStream"/> object to convert.
        /// </param>
        /// <returns>
        ///		A <see cref="MemoryStream"/> object representing the new instance of
        ///		the <see cref="BufferedStream"/> class.
        /// </returns>
        /// <seealso cref="BitStream"/>
        /// <seealso cref="BufferedStream"/>
        public static implicit operator BufferedStream(BitStream bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_BitStream"));

            return new BufferedStream((MemoryStream)bits);
        }

        /// <summary>
        ///		Converts a <see cref="NetworkStream"/> object to a new instance of the
        ///		<see cref="BitStream"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		This operator allows implicit casting from an instance of a
        ///		<see cref="NetworkStream"/> object to a new instance of a
        ///		<see cref="BitStream"/> object. No equivalent operator has been made
        ///		available that allows implicit casting from an instance of a
        ///		<see cref="BitStream"/> object to a new instance of a
        ///		<see cref="NetworkStream"/> object.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="NetworkStream"/> object to convert.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object representing the new instance of the
        ///		<see cref="BitStream"/> class.
        /// </returns>
        /// <seealso cref="NetworkStream"/>
        /// <seealso cref="BitStream"/>
        public static implicit operator BitStream(NetworkStream bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_NetworkStream"));

            return new BitStream(bits);
        }

        /// <summary>
        ///		Converts a <see cref="CryptoStream"/> object to a new instance of the
        ///		<see cref="BitStream"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///		<i>bits</i> is a null reference (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <remarks>
        ///		This operator allows implicit casting from an instance of a
        ///		<see cref="CryptoStream"/> object to a new instance of a
        ///		<see cref="BitStream"/> object. No equivalent operator has been made
        ///		available that allows implicit casting from an instance of a
        ///		<see cref="BitStream"/> object to a new instance of a
        ///		<see cref="CryptoStream"/> object.
        /// </remarks>
        /// <param name="bits">
        ///		A <see cref="CryptoStream"/> object to convert.
        /// </param>
        /// <returns>
        ///		A <see cref="BitStream"/> object representing the new instance of the
        ///		<see cref="BitStream"/> class.
        /// </returns>
        /// <seealso cref="CryptoStream"/>
        /// <seealso cref="BitStream"/>
        public static implicit operator BitStream(CryptoStream bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits), BitStreamResources.GetString("ArgumentNull_CryptoStream"));

            return new BitStream(bits);
        }

        #endregion Implicit Operators [20051125]
    }
}