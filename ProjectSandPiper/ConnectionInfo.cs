using System.Net.Sockets;

namespace ThrushSoftware.SandPiper
{
    public class ConnectionInfo : Serializible
    {
        public TcpClient Client;
        public NetworkStream Stream;
        
        public ConnectionInfo()
        {

        }
        public ConnectionInfo(TcpClient client)
        {
            this.Client = client;
            this.Stream = client.GetStream();
        }
    }
}
