using System;
using System.Collections.Generic;
using System.IO;
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

        private Random rand = new Random();
        
        // Function to generate random bytes of set size
        private byte[] GenerateRandomData(int size)
        {
            byte[] ret = new byte[size];
            rand.NextBytes(ret);
            return ret;
        }
        
        public RTMPClient(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            _packetHandler = new PacketHandler(this);
            
            Log(_client.Available.ToString());

            var c0 = new byte[CS0.Length];
            var readBytes = _stream.Read(c0, 0, c0.Length);
            var version = new CS0(c0);
            Log("Client requested RTMP Spec version: " + version.RTMPVersion);

            var c1 = new byte[CS1.Length];
            readBytes = _stream.Read(c1, 0, c1.Length);
            new CS1(c1);
            Log("Read c1");
            
            Log(_client.Available.ToString());

            List<byte> resp = new List<byte>();
            resp.Add(3);
            resp.AddRange(new byte[]{0,0,0,0});
            resp.AddRange(new byte[]{0,0,0,0});
            resp.AddRange(GenerateRandomData(1536-8));
            resp.AddRange(new byte[]{0,0,0,0});
            resp.AddRange(new byte[]{0,0,0,0});
            resp.AddRange(GenerateRandomData(1536-8));
            _stream.Write(resp.ToArray(), 0, resp.Count);

            var c2 = new byte[CS2.Length];
            _stream.Read(c2, 0, c2.Length); // C2
            new CS2(c2);
            Log("Read c2");

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