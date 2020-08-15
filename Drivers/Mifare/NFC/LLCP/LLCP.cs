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
        //byte sequenceCounter = 0;
        //byte GetSequenceCounter(bool reset)
        //{
        //    byte result;
        //    if (reset)
        //        sequenceCounter = 0;
        //    result = sequenceCounter;
        //    sequenceCounter++;
        //    if (sequenceCounter == 4)
        //        sequenceCounter = 0;
        //    return result;
        //}
        //public int StartNFC()
        //{
        //    int result = 0;
        //    //should be generated
        //    byte[] NfcId3T_TX = new byte[] {
        //        0x10,
        //        0x11,
        //        0x12,
        //        0x13,
        //        0x14,
        //        0x15,
        //        0x16,
        //        0x17,
        //        0x18,
        //        0x19,
        //    };

        //    byte[] LLCP_MAGIC_HEADER = new byte[] {
        //    	//> LLCP magic number
        //  0x46,   //F
        //        0x66,   //f
        //        0x6D,   //m
        //  //TLVs
        //  //> version
        //  0x01,
        //        0x01,
        //        0x11,
        //  //> Well known service list
        //  0x03,
        //        0x02,
        //        0x00,
        //        0x13,
        //  // link time out
        //  0x04,
        //        0x01,
        //        0x96
        //    };

        //    byte[] LLCP_CONNECT_SNEP = new byte[] {
        //    	//> LLCP Connect	0000 01      01 00      10 0010
        //  //					DSAP		 PTYPE		SSAP
        //  0x05,
        //        0x20,		//example on the internet shows 21
        //  //TLV RWS receive window size
        //  0x05,
        //        0x01,
        //        0x04,
        //        //TLV MIUX
        //        0x02,
        //        0x02,
        //        0x07,
        //        0x80,
        //  // service name urn:nfc:sn:snep
        //  0x06,
        //        0x0F,
        //        0x75,
        //        0x72,
        //        0x6e,
        //        0x3a,
        //        0x6e,
        //        0x66,
        //        0x63,
        //        0x3a,
        //        0x73,
        //        0x6e,
        //        0x3a,
        //        0x73,
        //        0x6e,
        //        0x65,
        //        0x70
        //    };

        //    byte[] LLCP_CONNECT_NPP = new byte[] {
        //    	//> LLCP Connect	0000 01      01 00      10 0010
        //  //					DSAP		 PTYPE		SSAP
        //  0x05,
        //        0x22,		//example on the internet shows 21
        //  //TLV RWS receive window size
        //  0x05,
        //        0x01,
        //        0x04,
        //        //TLV MIUX
        //        0x02,
        //        0x02,
        //        0x07,
        //        0x80,
        //  // service name urn:nfc:sn:snep
        //  0x06,
        //        0x0F,
        //        0x63,
        //        0x6F,
        //        0x6D,
        //        0x2E,
        //        0x61,
        //        0x6E,
        //        0x64,
        //        0x72,
        //        0x6F,
        //        0x69,
        //        0x64,
        //        0x2E,
        //        0x6E,
        //        0x70,
        //        0x70
        //    };

        //    byte[] LLCP_PAYLOAD1 = new byte[] {
        //  //> LLCP PAYLOAD	0100 00      11 00      10 0010
        //  //					DSAP		 PTYPE		SSAP
        //  0x43,
        //        0x22,
        //  // sequence
        //  0x00,
        //  // data
        //  //NPP HEADER
        //  0x01,	//version 0000 0001
        //  0x00,	// nmbr of NDEF entries byte 3
        //  0x00,	// nmbr of NDEF entries byte 2
        //  0x00,	// nmbr of NDEF entries byte 1
        //  0x01,	// nmbr of NDEF entries byte 0
        //  //NPP NDEF ENTRY
        //  0x01,	//Action code
        //  0x00,	// ndef length byte 3
        //  0x00,	// ndef length byte 2
        //  0x00,	// ndef length byte 1
        //  0x14,	// ndef length byte 0
        //  //NPP PAYLOAD (NDEF MESSAGE)
        //  //NDEF HEADER
        //  0xD1,	// complexe header.. zie doc (geen id length en field)
        //  0x01,	// type length
        //  0x10,	// payload length
        //  0x54,	// Type "T"=text
        //  //NDEF PAYLOAD
        //  0x02,	// 	status
        //  0x65,	//	language
        //  0x6E,	//	language
        //  //text "KooleControls"
        //  0x4B,
        //        0x6F,
        //        0x6F,
        //        0x6C,
        //        0x65,
        //        0x43,
        //        0x6F,
        //        0x6E,
        //        0x74,
        //        0x72,
        //        0x6F,
        //        0x6C,
        //        0x73
        //    };

        //    byte[] LLCP_DISCONNECT = new byte[] {
        //    	//> LLCP Connect	0000 01      01 01      10 0010
        //  //					DSAP		 PTYPE		SSAP
        //  0x05,
        //        0x62
        //    };

        //    byte[] LLCP_SYMM = new byte[] {
        //  		//> LLCP Connect	0000 01      01 01      10 0010
        //     //					DSAP		 PTYPE		SSAP
        //     0x00,
        //        0x00
        //    };
        //    int initStep = 0;
        //    bool ret = false;
        //    int numBytes = 0;
        //    byte[] status = new byte[4];
        //    //atr request function

        //    byte[] NfcId3T_RX = new byte[10]; //UID of the phone
        //    LogLevel debugLevel = LogLevel.None;


        //    ////********************************************
        //    ////****** RATS
        //    ////********************************************
        //    //SpiWriteRegister(Command.WRITE_REGISTER, Register.IRQ_CLEAR, new byte[] { 0xFF, 0xFF, 0x0F, 0x00 });
        //    //byte[] rats = new byte[] { 0xE0, 0x80 };  // The PN512 can has a buffer of 64 bytes and we set the cid to 0
        //    //ret = SendDataToCard(rats);
        //    //LogInfo.Log($"RATS: {BitConverter.ToString(rats)}", debugLevel);
        //    //SpiReadRegister(Register.IRQ_STATUS, status);
        //    //LogInfo.Log($"RATS IRQ status: {BitConverter.ToString(status)}", debugLevel);
        //    //(numBytes, _) = GetNumberOfBytesReceivedAndValidBits();
        //    //LogInfo.Log($"RATS numBytes: {numBytes}", debugLevel);
        //    //if (numBytes > 0)
        //    //{
        //    //    byte[] ats = new byte[numBytes];
        //    //    ReadDataFromCard(ats, ats.Length);
        //    //    LogInfo.Log($"RATS Reply: {BitConverter.ToString(ats)}", debugLevel);
        //    //    if (ats[0] == ats.Length)
        //    //    {
        //    //        if ((ats[1] & 0xF0) > 0) //can we choose the data rates?
        //    //        {
        //    //            //PPS request
        //    //            byte[] pps = new byte[] { 0xD0, 0x11, 0x00 };
        //    //            ret = SendDataToCard(pps);
        //    //            LogInfo.Log($"PPS: {BitConverter.ToString(pps)}", debugLevel);
        //    //            SpiReadRegister(Register.IRQ_STATUS, status);
        //    //            LogInfo.Log($"PPS IRQ status: {BitConverter.ToString(status)}", debugLevel);
        //    //            (numBytes, _) = GetNumberOfBytesReceivedAndValidBits();
        //    //            LogInfo.Log($"PPS numBytes: {numBytes}", debugLevel);
        //    //            if (numBytes > 0)
        //    //            {
        //    //                byte[] psr = new byte[numBytes];
        //    //                ReadDataFromCard(psr, psr.Length);
        //    //                LogInfo.Log($"PPS Reply: {BitConverter.ToString(psr)}", debugLevel);
        //    //            }
        //    //        }
        //    //    }

        //    //}
        //    //else
        //    //{
        //    //    LogInfo.Log($"RATS: Not replied", debugLevel);
        //    //}

        //    //********************************************
        //    //****** LLCP Bind Send LLCP MAGIC HEADER and receive the magic header
        //    //********************************************
        //    byte targetNumber = 0;
        //    byte[] atr_req = Nfcip1.AtrReq(NfcId3T_TX, LLCP_MAGIC_HEADER);
        //    Thread.Sleep(1);
        //    Span<byte> atr_res; //its devided by 100 for delay
        //    LogInfo.Log($"ATR_REQ: {BitConverter.ToString(atr_req)}", debugLevel);
        //    if (Tranceive(targetNumber, atr_req, out atr_res, 500) >= 0)
        //    {
        //        LogInfo.Log($"ATR_REQ Response: {BitConverter.ToString(atr_res.ToArray())}", debugLevel);

        //        byte[] reply = atr_res.ToArray();
        //        if ((reply[2] == (byte)Nfcip1.Types.RES) && (reply[3] == (byte)Nfcip1.Commands.ATR_RES)) //hebben we een reply ontvangen
        //        {
        //            Array.Copy(reply, 4, NfcId3T_RX, 0, 10);
        //            double TimeOut = (reply[17] * 309.8);
        //            LogInfo.Log($"  Time-out used: {TimeOut}", debugLevel);
        //            byte ppt = reply[18];
        //            LogInfo.Log($"  Max buffersize: {((ppt >> 4) + 1) * 64}", debugLevel);
        //            if ((ppt & 0x02) > 0)
        //            {
        //                byte[] tGenBytes = new byte[reply.Length - 19];
        //                Array.Copy(reply, 19, tGenBytes, 0, tGenBytes.Length);
        //                LogInfo.Log($"  GenBytes available: {BitConverter.ToString(tGenBytes)}", debugLevel);
        //                if ((reply[19] == 0x46) && (reply[20] == 0x66) && (reply[21] == 0x6D))
        //                {
        //                    LogInfo.Log($"  LLCP magic header found!", debugLevel);
        //                    //int cnt = 0;
        //                    //int stop = 0;
        //                    //for (i = 21; i < replyCount; i++)
        //                    //{
        //                    //    if (cnt == 0)
        //                    //    {
        //                    //        snprintf(test, BUFFERSIZE, "%s\t\tType: %d ", test, reply[i]);
        //                    //        cnt++;
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        if (cnt == 1)
        //                    //        {
        //                    //            snprintf(test, BUFFERSIZE, "%slength: %d value: ", test, reply[i]);
        //                    //            stop = reply[i];
        //                    //            cnt++;
        //                    //        }
        //                    //        else
        //                    //        {
        //                    //            snprintf(test, BUFFERSIZE, "%s%.2x ", test, reply[i]);
        //                    //            cnt++;
        //                    //            if ((stop + 2) == cnt)
        //                    //            {
        //                    //                cnt = 0;
        //                    //                snprintf(test, BUFFERSIZE, "%s\r\n", test);
        //                    //            }
        //                    //        }
        //                    //    }
        //                    //}
        //                    initStep = 2;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        LogInfo.Log($"ATR_REQ No response", debugLevel);
        //    }


        //    if (initStep == 2)
        //    {
        //        Thread.Sleep(20);
        //        LogInfo.Log($"LLCP SYMM", debugLevel);
        //        //********************************************
        //        //****** LLCP SYMM
        //        //********************************************
        //        byte[] dep_req = Nfcip1.DepReq(0, new byte[] { 0, 0 });
        //        Span<byte> dep_res; //its devided by 100 for delay
        //        LogInfo.Log($"DEP_REQ: {BitConverter.ToString(dep_req)}", debugLevel);
        //        if (Tranceive(targetNumber, dep_req, out dep_res, 500) >= 0)
        //        {
        //            byte[] reply = dep_res.ToArray();
        //            LogInfo.Log($"DEP_REQ Response: {BitConverter.ToString(reply)}", debugLevel);
        //            initStep++;
        //        }
        //        else
        //        {
        //            LogInfo.Log($"DEP_REQ No response", debugLevel);
        //        }
        //    }

        //    if (initStep == 3)
        //    {
        //        Thread.Sleep(20);
        //        LogInfo.Log($"LLCP SYMM", debugLevel);
        //        //********************************************
        //        //****** LLCP SYMM
        //        //********************************************
        //        byte[] dep_req = Nfcip1.DepReq(1, new byte[] { 0, 0 });
        //        Span<byte> dep_res; //its devided by 100 for delay
        //        LogInfo.Log($"DEP_REQ: {BitConverter.ToString(dep_req)}", debugLevel);
        //        if (Tranceive(targetNumber, dep_req, out dep_res, 500) >= 0)
        //        {
        //            byte[] reply = dep_res.ToArray();
        //            LogInfo.Log($"DEP_REQ Response: {BitConverter.ToString(reply)}", debugLevel);
        //            initStep++;
        //        }
        //        else
        //        {
        //            LogInfo.Log($"DEP_REQ No response", debugLevel);
        //        }
        //    }

        //    if (initStep == 4)
        //    {
        //        Thread.Sleep(20);
        //        LogInfo.Log($"LLCP CONNECT", debugLevel);
        //        // ********************************************
        //        // ******LLCP CONNECT
        //        // ********************************************
        //        byte[] dep_req = Nfcip1.DepReq(2, LLCP_CONNECT_SNEP);
        //        Span<byte> dep_res; //its devided by 100 for delay
        //        LogInfo.Log($"DEP_REQ: {BitConverter.ToString(dep_req)}", debugLevel);
        //        if (Tranceive(targetNumber, dep_req, out dep_res, 500) >= 0)
        //        {
        //            byte[] reply = dep_res.ToArray();
        //            LogInfo.Log($"DEP_REQ Response: {BitConverter.ToString(reply)}", debugLevel);
        //            initStep++;
        //        }
        //        else
        //        {
        //            LogInfo.Log($"DEP_REQ No response", debugLevel);
        //        }
        //    }

        //    if (initStep == 5)
        //    {
        //        Thread.Sleep(20);
        //        // ********************************************
        //        // ****** SNEP MODE (message NFC powered by Stollmann)
        //        // ********************************************
        //        LogInfo.Log($"Send NDEF SNEP", debugLevel);
        //        NdefLibrary.Ndef.NdefMessage message = new NdefLibrary.Ndef.NdefMessage();

        //        NdefLibrary.Ndef.NdefUriRecord uriRecord = new NdefLibrary.Ndef.NdefUriRecord();
        //        uriRecord.Uri = "http://www.koolecontrols.nl";

        //        NdefLibrary.Ndef.NdefTextRecord textRecord = new NdefLibrary.Ndef.NdefTextRecord();
        //        textRecord.LanguageCode = "en";
        //        textRecord.Text = "Wat een geweldige test is dit";

        //        message.Add(uriRecord);
        //        byte[] ndef = message.ToByteArray();
        //        byte[] snep = SNEP.Request(ndef);
        //        byte[] llcp = LLCP.GetFrame(0x04, LLCP.PTYPES.I, 0x20, 0, snep);
        //        byte[] dep_req = Nfcip1.DepReq(3, llcp);

        //        Span<byte> dep_res; //its devided by 100 for delay
        //        LogInfo.Log($"DEP_REQ: {BitConverter.ToString(dep_req)}", debugLevel);
        //        if (Tranceive(targetNumber, dep_req, out dep_res, 500) >= 0)
        //        {
        //            byte[] reply = dep_res.ToArray();
        //            LogInfo.Log($"DEP_REQ Response: {BitConverter.ToString(reply)}", debugLevel);
        //            initStep++;
        //        }
        //        else
        //        {
        //            LogInfo.Log($"DEP_REQ No response", debugLevel);
        //        }
        //    }

        //    //if (initStep == 4)
        //    //{
        //    //    // ********************************************
        //    //    // ****** LLCP SYMM
        //    //    // ********************************************
        //    //    byte[] dep_req = Nfcip1.DepReq(GetSequenceCounter(false), LLCP_SYMM);
        //    //    SpiWriteRegister(Command.WRITE_REGISTER, Register.IRQ_CLEAR, new byte[] { 0xFF, 0xFF, 0x0F, 0x00 });
        //    //    ret = SendDataToCard(dep_req);
        //    //    LogInfo.Log($"DEP_REQ: {BitConverter.ToString(dep_req)}", debugLevel);
        //    //    SpiReadRegister(Register.IRQ_STATUS, status);
        //    //    int count = 0;
        //    //    while ((status[0] & 0x03) != 0x03)
        //    //    {
        //    //        Thread.Sleep(1);
        //    //        SpiReadRegister(Register.IRQ_STATUS, status);
        //    //        LogInfo.Log($"  wait:  {BitConverter.ToString(status)} {count}", debugLevel);
        //    //        count++;
        //    //    }
        //    //    (numBytes, _) = GetNumberOfBytesReceivedAndValidBits();
        //    //    result = numBytes;
        //    //    LogInfo.Log($"DEP_REQ numBytes: {numBytes}", debugLevel);
        //    //    if (numBytes > 0)
        //    //    {
        //    //        byte[] reply = new byte[numBytes];
        //    //        ReadDataFromCard(reply, reply.Length);
        //    //        LogInfo.Log($"DEP_REQ Response: {BitConverter.ToString(reply)}", debugLevel);
        //    //        initStep++;
        //    //    }
        //    //    else
        //    //    {
        //    //        LogInfo.Log($"DEP_REQ No response", debugLevel);
        //    //    }
        //    //}
        //    return result;
        //}

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

        public LLCP(INfcTranceiver chip, Data106kbpsTypeA iso14443Device)
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
