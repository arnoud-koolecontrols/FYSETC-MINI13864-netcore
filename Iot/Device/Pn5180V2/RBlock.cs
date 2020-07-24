﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Device.Pn5180V2
{
    internal enum RBlock
    {
        Acknoledge = 0b0001_0000,
        NAcknoledge = 0b0000_0000
    }
}