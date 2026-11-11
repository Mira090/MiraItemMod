using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiraItemMod.Utilities
{
    public static class SpecialStatCategories
    {
        static SpecialStatCategories()
        {
            var type = typeof(SpecialStatCategories);
            var pros = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(string));
            foreach (var pro in pros)
            {
                pro.SetValue(type, pro.Name);
                //Core.Logger("Categories: " + pro.Name);
            }
        }
        /// <summary>
        /// ダメージ
        /// </summary>
        public static string Damage { get; private set; }
        /// <summary>
        /// 武器
        /// </summary>
        public static string Weapon { get; private set; }
        /// <summary>
        /// クロスボウ
        /// </summary>
        public static string Crossbow { get; private set; }
        /// <summary>
        /// 魔導書
        /// </summary>
        public static string Grimoire { get; private set; }
        /// <summary>
        /// 吸収
        /// </summary>
        public static string Steal { get; private set; }
        /// <summary>
        /// デバフ
        /// </summary>
        public static string Debuff { get; private set; }
        /// <summary>
        /// 雷雲
        /// </summary>
        /// <remarks>無効化されている</remarks>
        public static string DarkCloud { get; private set; }
        /// <summary>
        /// 火足
        /// </summary>
        /// <remarks>無効化されている</remarks>
        public static string FlameGround { get; private set; }
    }
}
