using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Utilities
{
    public enum EPassivePerkLv
    {
        lv5,
        lv10,
        lv20
    }
    public static class Enums
    {
        public static string ToUpperString(this EPassivePerkLv lv)
        {
            return lv.ToString().ToUpperInvariant();
        }
    }
}
