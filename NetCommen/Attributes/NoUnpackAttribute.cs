﻿using System;

namespace NetCommen.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NoUnpackAttribute : Attribute { }
}
