using myApp.Drivers.Mifare.NFC.LLCP.Parameters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP.Parameters
{
    public class LLCPParameters
    {
        public LLCParameterVersion Version { get; private set; } = null;
        public LLCParameterMiux MIUX { get; private set; } = null;
        public LLCParameterWellKnownServiceList WellKnownServiceList { get; private set; } = null;
        public LLCParameterLinkTimeOut LinkTimeOut { get; private set; } = null;
        public LLCParameterReceiveWindowSize ReceiveWindowSize { get; private set; } = null;
        public LLCParameterServiceName ServiceName { get; private set; } = null;
        public LLCParameterOption Option { get; private set; } = null;
        public LLCParameterServiceDiscoveryRequest ServiceDiscoveryRequest { get; private set; } = null;
        public LLCParameterServiceDiscoveryResponse ServiceDiscoveryResponse { get; private set; } = null;

        public void SetMiux(int miux)
        {
            MIUX = new LLCParameterMiux(miux);
        }
        public void SetVersion(Version version)
        {
            Version = new LLCParameterVersion(version);
        }

        public void SetWellKnownServiceList(int wellKnownServiceList)
        {
            WellKnownServiceList = new LLCParameterWellKnownServiceList(wellKnownServiceList);
        }

        public void SetLinkTimeOut(int linkTimeOut)
        {
            LinkTimeOut = new LLCParameterLinkTimeOut(linkTimeOut);
        }

        public void SetOption(LinkServiceClass linkServiceClass)
        {
            Option = new LLCParameterOption(linkServiceClass);
        }

        public void SetServiceName(string serviceName)
        {
            ServiceName = new LLCParameterServiceName(serviceName);
        }

        public void SetReceiveWindowSize(int receiveWindow)
        {
            ReceiveWindowSize = new LLCParameterReceiveWindowSize(receiveWindow);
        }

        public LLCPParameters()
        {

        }

        public LLCPParameters(Version version, int miux, int wellKnownServiceList, int linkTimeOut, LinkServiceClass linkServiceClass)
        {
            SetVersion(version);
            SetMiux(miux);
            SetWellKnownServiceList(wellKnownServiceList);
            SetLinkTimeOut(linkTimeOut);
            SetOption(linkServiceClass);
        }

        private byte[] AddParameterToArray(byte[] part1, LLCPParameter parameter)
        {
            byte[] parData = parameter.Data;
            byte[] result = new byte[part1.Length + parData.Length];
            part1.CopyTo(result, 0);
            parData.CopyTo(result, part1.Length);
            return result;
        }

        public byte[] GetParams()
        {
            byte[] result = new byte[0];
            if (Version != null)
            {
                result = AddParameterToArray(result, Version);
            }

            if (ReceiveWindowSize != null)
            {
                result = AddParameterToArray(result, ReceiveWindowSize);
            }

            if (MIUX != null)
            {
                result = AddParameterToArray(result, MIUX);
            }

            if (WellKnownServiceList != null)
            {
                result = AddParameterToArray(result, WellKnownServiceList);
            }

            if (LinkTimeOut != null)
            {
                result = AddParameterToArray(result, LinkTimeOut);
            }

            if (Option != null)
            {
                result = AddParameterToArray(result, Option);
            }

            if (ServiceName != null)
            {
                result = AddParameterToArray(result, ServiceName);
            }
            return result;
        }

        public byte[] GetMagicHeader()
        {
            // All PAX fields should be send in the ATR_REQ payload 
            byte[] result = LLCP.LLCPMagicNumber;
            // version must be send 
            if (Version != null)
            {
                result = AddParameterToArray(result, Version);
            } else
            {
                throw new Exception("Magic header must contain version");
            }
            // miux may be send
            if (MIUX != null)
            {
                result = AddParameterToArray(result, MIUX);
            }

            // well know service-List should be send
            if (WellKnownServiceList != null)
            {
                result = AddParameterToArray(result, WellKnownServiceList);
            }

            // Link timeout may be send
            if (LinkTimeOut != null)
            {
                result = AddParameterToArray(result, LinkTimeOut);
            }

            // option may be send
            if (Option != null)
            {
                result = AddParameterToArray(result, Option);
            }
            return result;
        }

        public LLCPParameters(byte[] rawData)
        {
            int index = 0;
            while (index < rawData.Length)
            {
                LLCPParameter parameter = new LLCPParameter(rawData, index);
                switch (parameter.Type)
                {
                    case LLCParameterType.Version:
                        Version = new LLCParameterVersion(parameter);
                        break;

                    case LLCParameterType.MIUX:
                        MIUX = new LLCParameterMiux(parameter);
                        break;

                    case LLCParameterType.WellKnownServiceList:
                        WellKnownServiceList = new LLCParameterWellKnownServiceList(parameter);
                        break;

                    case LLCParameterType.LinkTimeOut:
                        LinkTimeOut = new LLCParameterLinkTimeOut(parameter);
                        break;

                    case LLCParameterType.ReceiveWindowSize:
                        ReceiveWindowSize = new LLCParameterReceiveWindowSize(parameter);
                        break;

                    case LLCParameterType.ServiceName:
                        ServiceName = new LLCParameterServiceName(parameter);
                        break;

                    case LLCParameterType.Option:
                        Option = new LLCParameterOption(parameter);
                        break;

                    case LLCParameterType.ServiceDiscoveryRequest:
                        ServiceDiscoveryRequest = new LLCParameterServiceDiscoveryRequest(parameter);
                        break;

                    case LLCParameterType.ServiceDiscoveryResponse:
                        ServiceDiscoveryResponse = new LLCParameterServiceDiscoveryResponse(parameter);
                        break;
                }
                index += (parameter.Length + 2);
            }
        }
    }
}
