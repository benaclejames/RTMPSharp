using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace RTMP
{
    public class RTMPClient
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly PacketHandler _packetHandler;
        private readonly Queue<RTMPSerializeable> _queue = new Queue<RTMPSerializeable>();

        public void Log(string message)
        {
            Console.Write("[");
            Console.Write(GetClientIdentity());
            Console.Write("] ");
            Console.WriteLine(message);
        }
        
        public RTMPClient(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            _packetHandler = new PacketHandler(this);

            var c0 = new byte[CS0.Length];
            _stream.Read(c0, 0, c0.Length);
            var version = new CS0(c0);
            Log("Client requested RTMP Spec version: " + version.RTMPVersion);

            var c1 = new byte[CS1.Length];
            _stream.Read(c1, 0, c1.Length);
            new CS1(c1);

            _stream.Write(c0, 0, c0.Length); // s0
            _stream.Write(c1, 0, c1.Length); // s1

            var c2 = new byte[CS2.Length];
            _stream.Read(c2, 0, c2.Length); // C2
            new CS2(c2);

            // Send the same back
            _stream.Write(c1, 0, c1.Length); // S2

            Log("Hands Shook. Connection Established!");

            // Start a new thread to call the recv method
            new Thread(() => { ReceiveLoop(client); }).Start();
        }

        private void ReceiveLoop(TcpClient client)
        {
            while (client.Connected)
            {
                FlushStream();
                _packetHandler.Parse(_stream);
            }
        }

        public string GetClientIdentity() => _client.Client.RemoteEndPoint.ToString();
        
        private void FlushStream()
        {
            foreach (var bytes in _queue.Select(packet => packet.Serialize()))
                _stream.Write(bytes, 0, bytes.Length);

            _queue.Clear();
        }

        public void EnqueueSend(RTMPSerializeable msg) => _queue.Enqueue(msg);
    }
}