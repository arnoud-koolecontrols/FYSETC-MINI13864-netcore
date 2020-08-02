using Iot.Device.Rfid;
using myApp.Drivers.Mifare.NFC.LLCP.Parameters;
using myApp.Drivers.Mifare.NFC.LLCP.ServiceManagers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace myApp.Drivers.Mifare.NFC.LLCP
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

        public enum WelKnownServiceAccessPoints
        {
            LLCLinkManagementService = 0,               // LLC-LM
            ServiceDiscoveryProtocolService = 1,        // SDP
            SimpleNdefExchangeProtocolService = 4,      // SNEP
        }

        public Data106kbpsTypeA Iso14443Device { get; private set; } = null;
        public ITranceiver Chip { get; private set; } = null;
        /// <summary>
        /// The LTO parameter SHALL specify the maximum time interval between the last received bit of an 
        ///   LLC PDU transmission from the remote to the local LLC and the first bit of the subsequent LLC 
        ///   PDU transmission from the local to the remote LLC
        /// the default link timeout value is 100 milliseconds
        /// </summary>
        public int LinkTimeOut { get; set; } = 100;
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
       

        public LLCP(ITranceiver chip, Data106kbpsTypeA iso14443Device)
        {
            Chip = chip;
            Iso14443Device = iso14443Device;
            IsoIec18092LinkServiceManager linkManager = new IsoIec18092LinkServiceManager();
            ServiceManagers.Add(linkManager.SSAP, linkManager);
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

        private void SendSymmReset()
        {
            if (LinkTimeOut <= 100)
            {
                LastSymmSentAt = DateTime.Now.AddMilliseconds(LinkTimeOut - 10); //Perhaps not needed but Send the SYMM 1 unit before timing out
            }
            else
            {
                LastSymmSentAt = DateTime.Now.AddMilliseconds(90);
            }
        }

        DateTime LastSymmSentAt { get; set; } = DateTime.MinValue;
        private void StartWork()
        {
            wtoken = new CancellationTokenSource();
            int symmRetry = 0;
            task = Task.Factory.StartNew(action =>
            {
                ILinkManager manager = (ILinkManager)ServiceManagers[0];

                LLCPParameters paramsOut = new LLCPParameters(Version, MIUX, GetSupportedWelKnownServiceList(), LinkTimeOut, LSC);
                LLCPParameters paramsIn;

                if (manager.LinkActivation(this.Chip, Iso14443Device.TargetNumber, paramsOut, out paramsIn))
                {
                    SendSymmReset();
                    Console.WriteLine("LinkActivation successful");
                    while (true)
                    {
                        if (wtoken.IsCancellationRequested) //stop is requeste break the loop and send close connection
                        {
                            Console.WriteLine("Closing LLCP connection");
                            break;
                        }
                       
                        byte[] res;
                        if (Buffer.TryDequeue(out res))
                        {

                            //todo handle PMU's and route the to the right services
                            //should there be a delay after tranceiving data?
                        }
                        else
                        {
                            if (LastSymmSentAt < DateTime.Now)
                            {
                                if (manager.Symm(Chip, Iso14443Device.TargetNumber))
                                {
                                    symmRetry = 0;
                                    Console.WriteLine("Symm");
                                } else
                                {
                                    symmRetry++;
                                    if (symmRetry > 5)
                                    {
                                        Console.WriteLine("Closing LLCP");
                                        break;
                                    }
                                    Console.WriteLine("Symm failed");
                                }
                                SendSymmReset();
                            }
                        }
                    }

                    if (manager.LinkDeActivation(this.Chip, Iso14443Device.TargetNumber))
                    {
                        Console.WriteLine("Link deactivation successful");
                    } else
                    {
                        Console.WriteLine("Link deactivation failed");
                    }
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
