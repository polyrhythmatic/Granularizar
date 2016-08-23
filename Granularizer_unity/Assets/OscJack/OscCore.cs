//
// OSC Jack - OSC Input Plugin for Unity
//
// Copyright (C) 2015, 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Object = System.Object;

namespace OscJack
{
	// OSC message storage struct
	public struct OscMessage
	{
		public string address;
		public object[] data;

		public OscMessage(string address, object[] data)
		{
			this.address = address;
			this.data = data;
		}

		public override string ToString ()
		{
			var temp = address + ":";
			if (data.Length > 0)
			{
				for (var i = 0; i < data.Length - 1; i++)
					temp += data[i] + ",";
				temp += data[data.Length - 1];
			}
			return temp;
		}
	}

	public class OscMessageSend
	{
		public string address;
		public ArrayList values;

		public OscMessageSend()
		{
			values = new ArrayList();
		}

		public static OscMessageSend StringToOscMessage (string message)
		{
			OscMessageSend oM = new OscMessageSend ();
			//			Debug.Log(oM.values.Count);
			// Console.WriteLine("Splitting " + message);
			string[] ss = message.Split (new char[] { ' ' });
			IEnumerator sE = ss.GetEnumerator ();
			if (sE.MoveNext ())
				oM.address = (string)sE.Current;
			while (sE.MoveNext ()) {
				string s = (string)sE.Current;
				// Console.WriteLine("  <" + s + ">");
				if (s.StartsWith ("\"")) {
					StringBuilder quoted = new StringBuilder ();
					bool looped = false;
					if (s.Length > 1)
						quoted.Append (s.Substring (1));
					else
						looped = true;
					while (sE.MoveNext ()) {
						string a = (string)sE.Current;
						// Console.WriteLine("    q:<" + a + ">");
						if (looped)
							quoted.Append (" ");
						if (a.EndsWith ("\"")) {
							quoted.Append (a.Substring (0, a.Length - 1));
							break;
						} else {
							if (a.Length == 0)
								quoted.Append (" ");
							else
								quoted.Append (a);
						}
						looped = true;
					}
					oM.values.Add (quoted.ToString ());
				} else {
					if (s.Length > 0) {
						try {
							int i = int.Parse (s);
							Console.WriteLine ("  i:" + i);
							oM.values.Add (i);
						} catch {
							try {
								float f = float.Parse (s);
								Console.WriteLine ("  f:" + f);
								oM.values.Add (f);
							} catch {
								Console.WriteLine ("  s:" + s);
								oM.values.Add (s);
							}
						}
					}
				}
			}
			return oM;
		}

		public static int OscMessageToPacket (OscMessageSend oscM, byte[] packet, int length)
		{
			return OscMessageToPacket (oscM, packet, 0, length);
		}


		// Creates an array of bytes from a single OscMessage.  Used internally.
		private static int OscMessageToPacket (OscMessageSend oscM, byte[] packet, int start, int length)
		{
			int index = start;
			index = InsertString (oscM.address, packet, index, length);
			if (oscM.values.Count > 0) {
				StringBuilder tag = new StringBuilder ();
				tag.Append (",");
				int tagIndex = index;
				index += PadSize (2 + oscM.values.Count);

				foreach (object o in oscM.values) {
					if (o is int) {
						int i = (int)o;
						tag.Append ("i");
						packet [index++] = (byte)((i >> 24) & 0xFF);
						packet [index++] = (byte)((i >> 16) & 0xFF);
						packet [index++] = (byte)((i >> 8) & 0xFF);
						packet [index++] = (byte)((i) & 0xFF);
					} else {
						if (o is float) {
							float f = (float)o;
							tag.Append ("f");
							byte[] buffer = new byte[4];
							MemoryStream ms = new MemoryStream (buffer);
							BinaryWriter bw = new BinaryWriter (ms);
							bw.Write (f);
							packet [index++] = buffer [3];
							packet [index++] = buffer [2];
							packet [index++] = buffer [1];
							packet [index++] = buffer [0];
						} else {
							if (o is string) {
								tag.Append ("s");
								index = InsertString (o.ToString (), packet, index, length);
							} else {
								tag.Append ("?");
							}
						}
					}
				}
				InsertString (tag.ToString (), packet, tagIndex, length);
			}
			return index;
		}
		// Inserts a string, correctly padded into a packet.  Used internally.
		private static int InsertString (string s, byte[] packet, int start, int length)
		{
			int index = start;
			foreach (char c in s) {
				packet [index++] = (byte)c;
				if (index == length)
					return index;
			}
			packet [index++] = 0;
			int pad = (s.Length + 1) % 4;
			if (pad != 0) {
				pad = 4 - pad;
				while (pad-- > 0)
					packet [index++] = 0;
			}
			return index;
		}

		// Takes a length and returns what it would be if padded to the nearest 4 bytes.
		private static int PadSize (int rawSize)
		{
			int pad = rawSize % 4;
			if (pad == 0)
				return rawSize;
			else
				return rawSize + (4 - pad);
		}
	}

	// OSC packet parser
	public class OscParser
	{
		#region Public Methods And Properties

		public int MessageCount {
			get { return _messageQueue.Count; }
		}

		public OscParser ()
		{
			_messageQueue = new Queue<OscMessage> ();
		}

		public OscMessage PopMessage ()
		{
			return _messageQueue.Dequeue ();
		}

		public void FeedData (Byte[] data)
		{
			_readBuffer = data;
			_readPoint = 0;

			ReadMessage ();

			_readBuffer = null;
		}

		#endregion

		#region Private Implementation

		Queue<OscMessage> _messageQueue;
		Byte[] _readBuffer;
		int _readPoint;

		void ReadMessage ()
		{
			var address = ReadString ();

			if (address == "#bundle") {
				ReadInt64 ();

				while (true) {
					if (_readPoint >= _readBuffer.Length)
						return;

					var peek = _readBuffer [_readPoint];
					if (peek == '/' || peek == '#') {
						ReadMessage ();
						return;
					}

					var bundleEnd = _readPoint + ReadInt32 ();
					while (_readPoint < bundleEnd)
						ReadMessage ();
				}
			}

			var types = ReadString ();
			var temp = new OscMessage (address, new object[types.Length - 1]);

			for (var i = 0; i < types.Length - 1; i++) {
				switch (types [i + 1]) {
				case 'f':
					temp.data [i] = ReadFloat32 ();
					break;
				case 'i':
					temp.data [i] = ReadInt32 ();
					break;
				case 's':
					temp.data [i] = ReadString ();
					break;
				case 'b':
					temp.data [i] = ReadBlob ();
					break;
				}
			}

			_messageQueue.Enqueue (temp);
		}

		float ReadFloat32 ()
		{
			Byte[] temp = {
				_readBuffer [_readPoint + 3],
				_readBuffer [_readPoint + 2],
				_readBuffer [_readPoint + 1],
				_readBuffer [_readPoint]
			};
			_readPoint += 4;
			return BitConverter.ToSingle (temp, 0);
		}

		int ReadInt32 ()
		{
			int temp =
				(_readBuffer [_readPoint + 0] << 24) +
				(_readBuffer [_readPoint + 1] << 16) +
				(_readBuffer [_readPoint + 2] << 8) +
				(_readBuffer [_readPoint + 3]);
			_readPoint += 4;
			return temp;
		}

		long ReadInt64 ()
		{
			long temp =
				((long)_readBuffer [_readPoint + 0] << 56) +
				((long)_readBuffer [_readPoint + 1] << 48) +
				((long)_readBuffer [_readPoint + 2] << 40) +
				((long)_readBuffer [_readPoint + 3] << 32) +
				((long)_readBuffer [_readPoint + 4] << 24) +
				((long)_readBuffer [_readPoint + 5] << 16) +
				((long)_readBuffer [_readPoint + 6] << 8) +
				((long)_readBuffer [_readPoint + 7]);
			_readPoint += 8;
			return temp;
		}

		string ReadString ()
		{
			var offset = 0;
			while (_readBuffer [_readPoint + offset] != 0)
				offset++;
			var s = System.Text.Encoding.UTF8.GetString (_readBuffer, _readPoint, offset);
			_readPoint += (offset + 4) & ~3;
			return s;
		}

		Byte[] ReadBlob ()
		{
			var length = ReadInt32 ();
			var temp = new Byte[length];
			Array.Copy (_readBuffer, _readPoint, temp, 0, length);
			_readPoint += (length + 3) & ~3;
			return temp;
		}

		#endregion
	}
}
