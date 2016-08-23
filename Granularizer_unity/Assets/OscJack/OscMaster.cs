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

using UnityEngine;
using Object = System.Object;

namespace OscJack
{
    // OSC master directory class
    // Provides the interface for the OSC master directory.
    public static class OscMaster
    {
		// UDP listening port number list
        static int[] listenPortList = { 9000 };

        // Determines whether any data has arrived to a given address.
        public static bool HasData(string address)
        {
            return _directory.HasData(address);
        }

        // Returns a data set which was sent to a given address.
        public static Object[] GetData(string address)
        {
            return _directory.GetData(address);
        }

        // Clears data at a given address.
        public static void ClearData(string address)
        {
            _directory.ClearData(address);
        }
        
        //Sends Osc message
        public static void SendData(string data)
        {
			OscMessageSend oscMessage = OscMessageSend.StringToOscMessage(data);

			byte[] packet = new byte[1000];
			int length = OscMessageSend.OscMessageToPacket( oscMessage, packet, 1000 );
			//Debug.Log(packet);
//			/Debug.Log(length);
			client.SendPacket( packet, length);
        }

        // Returns a reference to the master directory instance.
        public static OscDirectory MasterDirectory {
            get { return _directory; }
        }

		public static OscClient MasterClient {
			get { return client; }
		}
			
        static OscDirectory _directory = new OscDirectory(listenPortList);
		static OscClient client = new OscClient("127.0.0.1", 8000);
    }
}
