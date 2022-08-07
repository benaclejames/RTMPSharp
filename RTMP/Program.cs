using System.Net;
using System.Net.Sockets;

namespace RTMP
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Recieve on new tcp socket
            var tcp = new TcpListener(IPAddress.Any, 1935);
            tcp.Start();
            while (true)
            {
                var client = tcp.AcceptTcpClient();
                new RTMPClient(client);

                // Send the same back


                // Recv
                /*while (true)
                {
                    var s2 = new byte[client.ReceiveBufferSize];
                    read = stream.Read(s2, 0, client.ReceiveBufferSize);
                    //parse whole s2 as utf 8
                    var packet = new Packet(s2);
                    if (packet.Chunks.Any(chunk =>
                        chunk.messageHeader.TypeId == 20 && ((AMFMessage) chunk.data).RPC == "connect"))
                    {
                        Console.WriteLine("Connecting");
                    }
                }*/
            }
        }
    }
}