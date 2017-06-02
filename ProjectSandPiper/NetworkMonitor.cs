using System.Collections.Generic;
using System.Threading;

namespace ThrushSoftware.SandPiper
{
    /// <summary>
    /// A class to be used to easily monitor communications to the computer via TCPClient
    /// It will call the Callbacks that ach header has in the registered header list
    /// </summary>
    public class NetworkMonitor
    {
        /// <summary>
        /// How long to wait between checks, low values increase performance hit
        /// High values are better
        /// </summary>
        public static int TimeOut = 1000;
        /// <summary>
        /// The raw data collected from the network stream
        /// </summary>
        public string RawBuffer { get; private set; }
        /// <summary>
        /// The default readsize of the bytes buffer when reading data
        /// </summary>
        private const int DefBufferSize = 2048;

        /// <summary>
        /// The full list of Headers recognized by the class
        /// </summary>
        public List<Header> RegisteredHeaders = new List<Header>();

        private bool isRunning = false;
        private ConnectionInfo info;
        private Thread listener;

        public NetworkMonitor(ConnectionInfo info)
        {
            this.info = info;

            this.listener = new Thread(MonitorLoop);
        }

        /// <summary>
        /// Starts the network listener
        /// </summary>
        public void Start()
        {
            isRunning = true;
            this.listener.Start();
        }

        public void Stop()
        {
            isRunning = false;
        }

        public void RegisterHeader(Header h)
        {
            this.RegisteredHeaders.Add(h);
        }

        private void MonitorLoop()
        {
            int bufferSize = DefBufferSize;
            while (isRunning)
            {
                if(info.Stream.DataAvailable)
                {
                    if(bufferSize == DefBufferSize)
                        System.Threading.Thread.Sleep(100);
                    byte[] buffer = new byte[bufferSize];
                    int amount = info.Stream.Read(buffer, 0, buffer.Length);

                    for(int i = 0; i < amount; i++)
                    {
                        RawBuffer += (char)buffer[i];
                    }
                    
                    if (amount == bufferSize)
                    {
                        bufferSize *= 2;
                    }
                }
                else
                {
                    //Check if it came out of recieving loop
                    if(bufferSize != DefBufferSize)
                    {
                        int end;
                        while((end = RawBuffer.IndexOf(Header.END)) != -1)
                        {
                            string partialHeader = RawBuffer.Substring(0, end + Header.END.Length);
                            RawBuffer = RawBuffer.Substring(end + Header.END.Length);

                            foreach (Header h in RegisteredHeaders)
                                if (h.Compatible(partialHeader))
                                {
                                    h.CallBack(h.DeserializeHeader(partialHeader).ToArray());
                                    break;
                                }

                            bufferSize = DefBufferSize;
                        }
                        
                    }
                    System.Threading.Thread.Sleep(TimeOut);
                }

            }
        }
    }
}
