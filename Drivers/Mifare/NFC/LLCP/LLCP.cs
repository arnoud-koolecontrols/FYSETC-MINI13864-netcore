using Iot.Device.NFC.LLCP.Parameters;
using Iot.Device.NFC.LLCP.ServiceManagers;
using Iot.Device.NFC.LLCP.ServiceManagers.NFCIP1;
using Iot.Device.NFC.LLCP.ServiceManagers.SNEP;
using Iot.Device.Rfid;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Iot.Device.NFC.LLCP
{
    public class LLCP
    {
        public enum PTYPES
        {
            SYMM = 0,
            PAX,
            AGF,
            UI,
            CONNECT,
            DISC,
            CC,
            DM,
            FRMR,
            I = 12,
            RR,
            RNR,
        }
        
        /// <summary>
        /// LLCP protocol version
        /// </summary>
        public static Version Version { get; } = new Version(1, 1);
        public static byte[] LLCPMagicNumber { get; } = new byte[] { 0x46, 0x66, 0x6D };
        public int MIUX { get; set; } = 2048;
        public LinkServiceClass LSC { get; private set; } = LinkServiceClass.Class3;
        private Dictionary<int, ServiceManager> ServiceManagers { get; set; } = new Dictionary<int, ServiceManager>();
        private ConcurrentQueue<byte[]> Buffer { get; } = new ConcurrentQueue<byte[]>();
        private SequenceGenerator SequenceGenerator { get; } = new SequenceGenerator();

        public event EventHandler<LLCPLinkActivatedEventArgs> ConnectionChanged;

        public SnepServiceManager SnepServiceManager { 
            get 
            { 
                if (ServiceManagers.ContainsKey((byte)WelKnownServiceAccessPoints.SimpleNdefExchangeProtocolService))
                {
                    ServiceManager serviceManager = ServiceManagers[(byte)WelKnownServiceAccessPoints.SimpleNdefExchangeProtocolService];
                    if (serviceManager is SnepServiceManager)
                    {
                        return (SnepServiceManager)serviceManager;
                    }
                }
                return null;
            }
        }

        public bool AddServiceManager(ServiceManager manager)
        {
            bool result = false;
            if (! ServiceManagers.ContainsKey(manager.SSAP))
            {
                try
                {
                    ServiceManagers.Add(manager.SSAP, manager);
                    result = true;
                }
                catch
                {

                }
            }
            return result;
        } 

        public LLCP(INfcTransceiver chip, Data106kbpsTypeA iso14443Device)
        {
            IsoIec18092LinkServiceManager linkManager = new IsoIec18092LinkServiceManager();
            linkManager.Chip = chip;
            linkManager.TargetNumber = iso14443Device.TargetNumber;
            ServiceManagers.Add(linkManager.SSAP, linkManager);
            SnepServiceManager snepServiceManager = new SnepServiceManager(linkManager);
            ServiceManagers.Add(snepServiceManager.SSAP, snepServiceManager);
        }

        private CancellationTokenSource wtoken;
        private Task task;

        private void StopWork()
        {
            wtoken.Cancel();

            try
            {
                task.Wait();
            }
            catch (AggregateException) { }
        }

        private void StartWork()
        {
            wtoken = new CancellationTokenSource();
            int symmRetry = 0;
            task = Task.Factory.StartNew(action =>
            {
                ILinkManager manager = (ILinkManager)ServiceManagers[0];

                LLCPParameters paramsOut = new LLCPParameters(Version, MIUX, GetSupportedWelKnownServiceList(), manager.LinkTimeOut, LSC);
                LLCPParameters paramsIn;

                if (manager.LinkActivation(paramsOut, out paramsIn))
                {
                    ConnectionChanged?.Invoke(this, new LLCPLinkActivatedEventArgs()
                    {
                        Connected = true
                    });
                    while (true)
                    {
                        if (wtoken.IsCancellationRequested) //stop is requeste break the loop and send close connection
                        {
                            Console.WriteLine("Closing LLCP connection");
                            break;
                        }
                        if (! manager.Symm())
                        {
                            symmRetry++;
                            if (symmRetry > 5)
                            {
                                break; //stop LLCP
                            }
                        } else
                        {
                            symmRetry = 0;
                        }
                    }

                    manager.LinkDeActivation();
                    ConnectionChanged?.Invoke(this, new LLCPLinkActivatedEventArgs()
                    {
                        Connected = false
                    });
                } else
                {
                    Console.WriteLine("Link activation failed");
                }
 
            }, wtoken, TaskCreationOptions.LongRunning);
        }

        public void Start()
        {
            if (ServiceManagers.ContainsKey(0))
            {
                if (ServiceManagers[0] is ILinkManager)
                {
                    StartWork();
                    return;
                }
            }
            throw new Exception("No linkmanager configured");
        }

        public void Stop()
        {
            StopWork();
        }

        private int GetSupportedWelKnownServiceList()
        {
            int wks = 0;
            foreach (KeyValuePair<int, ServiceManager> pair in ServiceManagers)
            {
                if (pair.Key < 0x10)
                {
                    wks |= 1 << pair.Key;
                }
            }
            return wks;
        }

        public static byte[] GetFrame(byte dsap, PTYPES pType, byte ssap, byte seq, byte[] payload)
        {
            int length = payload.Length + 2;
            if ((pType == PTYPES.I) || (pType == PTYPES.RR) || (pType == PTYPES.RNR))
            {
                length++;
            }

            byte[] result = new byte[length];
            byte ptype = (byte)pType;
            int index = 0;
            result[index++] = (byte)(((dsap & 0x3F) << 2) | ((ptype >> 2) & 0x03));
            result[index++] = (byte)((ssap & 0x3F) | ((ptype & 0x03) << 6));
            if ((pType == PTYPES.I) || (pType == PTYPES.RR) || (pType == PTYPES.RNR))
            {
                result[index++] = seq;
            }

            payload.CopyTo(result, index);
            return result;
        }
    }
}
