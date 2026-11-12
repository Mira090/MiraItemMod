using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Utilities
{
    public enum EPassivePerkLv
    {
        lv5,
        lv10,
        lv20
    }
    public enum EAssetType
    {
        Item,
        ItemCategory,
        Miracle,
        EffectHUD,
        Misc,
        Projectile,
        UI,
        Weapon,
        Passive,
        Keyword,
        Unit
    }
    public static class Enums
    {
        public static string ToPath(this EAssetType type)
        {
            return type.ToString() + "/";
        }
        public static string ToPath(this EAssetType type, string name)
        {
            return type.ToString() + "/" + name;
        }
        public static string ToPath(this EAssetType type, string path, params string[] paths)
        {
            if (paths.Length == 0)
                return type.ToPath(path);
            return type.ToString() + "/" + path + "/" + string.Join("/", paths);
        }
        public static string ToUpperString(this EPassivePerkLv lv)
        {
            return lv.ToString().ToUpperInvariant();
        }
    }
}
