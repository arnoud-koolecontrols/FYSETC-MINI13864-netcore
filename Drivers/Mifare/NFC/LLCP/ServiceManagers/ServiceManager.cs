using myApp.Drivers.Mifare.NFC.LLCP.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.ServiceManagers
{
    public class ServiceManager
    {
        public class ServiceManagerConnection
        {
            public ILinkManager LinkManager { get; set; } = null;
            public byte DSAP { get; private set; } = 0;
            public byte SSAP { get; private set; } = 0;
            public bool Connected { get; private set; } = false;
            public bool Connect(byte ssap, LLCPParameters parameters)
            {
                bool result = false;
                if (LinkManager != null)
                {
                    this.SSAP = ssap;
                    byte[] payload = parameters.GetParams();
                    byte[] connectRequest = LLCP.GetFrame((byte)WelKnownServiceAccessPoints.ServiceDiscoveryProtocolService, LLCP.PTYPES.CONNECT, ssap, 0, payload);
                    byte[] response = new byte[0];
                    if (LinkManager.Tranceive(connectRequest, out response))
                    {
                        if (response.Length > 2)
                        {
                            this.DSAP = (byte)(response[2] & 0x3F);             
                            // check response
                            Connected = true;
                            
                        }       
                    }
                }
                return result;
            }

            public bool Disconnect()
            {
                bool result = false;          
                if (LinkManager != null)
                {
                    byte[] disconnectRequest = LLCP.GetFrame(DSAP, LLCP.PTYPES.DISC, SSAP, 0, new byte[0]);
                    byte[] response = new byte[0];
                    if (LinkManager.Tranceive(disconnectRequest, out response))
                    {
                        // check response
                        result = true;
                    }
                }
                return result;
            }

            public bool SendMessage(byte[] message, out byte[] response)
            {
                bool result = false;
                response = new byte[0];
                if (LinkManager != null)
                {
                    byte[] llcp = LLCP.GetFrame(DSAP, LLCP.PTYPES.I, SSAP, 0, message);
                    if (LinkManager.Tranceive(llcp, out response))
                    {
                        //check response
                        result = true;
                    }
                }
                return result;
            }
        }

        protected ILinkManager LinkManager { get; private set; } = null;
        /// <summary>
        /// Address Field SSAP: Source Service Access Point
        /// 00h – 0Fh   Identifies the Well-Known Service Access Points
        /// 10h – 1Fh   Identifies Services in the local service environment and are advertised by local SDP
        /// 20h – 3Fh   Identifies Services in the local service environment and are NOT advertised by local SDP
        /// </summary>
        public byte SSAP { get; set; } = 0;

        public LLCPParameters Params { get; } = new LLCPParameters();

        public ServiceManager(ILinkManager linkmanager)
        {
            LinkManager = linkmanager;
        }
    }

}
