using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Entities
{
    [Serializable]
    public class LocalizedFlavorText : LocalizedString
    {
        public static readonly string ModName = "Mira's Item Mod";
        public LocalizedFlavorText() : base()
        {

        }

        public LocalizedFlavorText(string key) : base(key)
        {

        }
        public override string ToString()
        {
            var loc = base.ToString();
            if (string.IsNullOrEmpty(loc))
                return loc;
            return loc + "\r\n#" + ModName;
        }
    }
}
