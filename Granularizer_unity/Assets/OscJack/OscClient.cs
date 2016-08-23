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
using System.Net;
using System.Net.Sockets;
using System.Threading;

using UnityEngine;
using Object = System.Object;

namespace OscJack
{
    // OSC over UDP client class
    public class OscClient
    {
        UdpClient _udpSender;
        IPEndPoint _sendPoint;

		public OscClient(string sendAddress, int sendPort)
        {   
			Debug.Log(sendPort);
			_sendPoint = new IPEndPoint(IPAddress.Parse(sendAddress), sendPort);
			_udpSender = new UdpClient();
			_udpSender.Connect(_sendPoint);
        }

        public void Close()
        {
            _udpSender.Close();
            _udpSender = null;
        }
		public void SendPacket(byte[] packet, int length)
		{
			_udpSender.Send(packet, length);
		}
    }
}
