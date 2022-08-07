using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace RTMP
{
    public class RTMPClient
    {
        public static NetworkStream stream;
        private static Queue<RTMPSerializeable> queue = new Queue<RTMPSerializeable>();
        
        // Handshake
        private readonly CS0 _version;
        public CS1 C1;
        public CS2 C2;
        
        public RTMPClient(TcpClient client)
        {
            stream = client.GetStream();
            
            var c0 = new byte[CS0.Length];
            stream.Read(c0, 0, c0.Length);
            _version = new CS0(c0);
            Console.WriteLine("Client requested version {0} RTMP Spec", _version.RTMPVersion);
                
            var c1 = new byte[CS1.Length];
            stream.Read(c1, 0, c1.Length);
            C1 = new CS1(c1);
            
            stream.Write(c0, 0, c0.Length); // s0
            stream.Write(c1, 0, c1.Length); // s1
            
            var c2 = new byte[CS2.Length];
            stream.Read(c2, 0, c2.Length);  // C2
            C2 = new CS2(c2);
            
            // Send the same back
            stream.Write(c1, 0, c1.Length); // S2
                
            Console.WriteLine("Hands Shook. Connection Established!");
            
            // Start a new thread to call the recv method
            new Thread(() =>
            {
                Recv(client);
            }).Start();
        }

        public void Recv(TcpClient client)
        {
            while (true)
            {
                FlushStream();
                var s2 = new byte[client.ReceiveBufferSize];
                var len = stream.Read(s2, 0, client.ReceiveBufferSize);
                //parse whole s2 as utf 8
                Packet.Parse(s2, len);
            }
        }

        private void FlushStream()
        {
            foreach (var packet in queue)
            {
                byte[] bytes = packet.Serialize();
                stream.Write(bytes, 0, bytes.Length);
            }
            queue.Clear();
        }
        
        public static void EnqueueSend(RTMPSerializeable msg) => queue.Enqueue(msg);
    }
}