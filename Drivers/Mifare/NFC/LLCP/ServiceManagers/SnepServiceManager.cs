using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.ServiceManagers
{
    public class SnepServiceManager : ServiceManager
    {
        private ServiceManagerConnection Connection { get; set; }

        public SnepServiceManager(ILinkManager linkmanager) : base(linkmanager)
        {
            this.SSAP = (byte)WelKnownServiceAccessPoints.SimpleNdefExchangeProtocolService;
            Params.SetMiux(2048);
            Params.SetServiceName("urn:nfc:sn:snep");
            Params.SetReceiveWindowSize(1); //in an example it was 4
            Connection = new ServiceManagerConnection();
            Connection.LinkManager = linkmanager;
        }

        public bool SendNdefMessage(byte[] ndef)
        {
            bool result = false;
            if (!Connection.Connected)
            {
                Connection.Connect(0x20, Params);
            }
            if (Connection.Connected)
            {
                byte[] message = SNEP.Put(ndef);
                byte[] response = new byte[0];
                Connection.SendMessage(message, out response);
            }
            return result;
        }

    }
}
