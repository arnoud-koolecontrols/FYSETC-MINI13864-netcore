using myApp.Drivers.Mifare.NFC.LLCP.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace myApp.Drivers.Mifare.NFC.LLCP
{
    public interface ILinkManager
    {
        bool LinkActivation(ITranceiver chip, byte targetNumber, LLCPParameters paramsOut, out LLCPParameters paramsIn);
        bool LinkDeActivation(ITranceiver chip, byte targetNumber);
        bool Symm(ITranceiver chip, byte targetNumber);

    }
}
