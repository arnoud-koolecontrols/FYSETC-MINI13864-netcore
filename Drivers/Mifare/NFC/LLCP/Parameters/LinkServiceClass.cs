using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.NFC.LLCP.Parameters
{
    /// <summary>
    /// 
    /// </summary>
    public enum LinkServiceClass
    {
        Unknown = 0,
        /// <summary>
        /// Devices offering only the Connectionless transport service
        /// </summary>
        Class1,
        /// <summary>
        /// Devices offering only the Connection-oriented transport service
        /// </summary>
        Class2,
        /// <summary>
        /// Devices offering both Connectionless and Connection-oriented transport services
        /// </summary>
        Class3
    }
}
