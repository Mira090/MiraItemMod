using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiraItemMod.Utilities
{
    public class ItemCategories
    {
        static ItemCategories()
        {
            var type = typeof(ItemCategories);
            var pros = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(string));
            foreach( var pro in pros)
            {
                pro.SetValue(type, pro.Name.ToSephiriaUpperId());
                //Core.Logger("Categories: " + pro.Name.ToSephiriaUpperId());
            }
        }
        /// <summary>
        /// 仲間
        /// </summary>
        public static string Companion { get; private set; }
        /// <summary>
        /// 雷雲
        /// </summary>
        public static string DarkCloud { get; private set; }
        /// <summary>
        /// 燠火
        /// </summary>
        public static string Ember { get; private set; }
        /// <summary>
        /// 太陽剣
        /// </summary>
        public static string FlameSword { get; private set; }
        /// <summary>
        /// 氷の武具
        /// </summary>
        public static string Frost { get; private set; }
        /// <summary>
        /// 氷河
        /// </summary>
        public static string Glacier { get; private set; }
        /// <summary>
        /// 守護
        /// </summary>
        public static string Guardian { get; private set; }
        /// <summary>
        /// 魔法工学
        /// </summary>
        public static string Magitech { get; private set; }
        /// <summary>
        /// 神秘
        /// </summary>
        public static string Mystic { get; private set; }
        /// <summary>
        /// 惑星
        /// </summary>
        public static string Planet { get; private set; }
        /// <summary>
        /// 精密
        /// </summary>
        public static string Precision { get; private set; }
        /// <summary>
        /// 影
        /// </summary>
        public static string Shadow { get; private set; }
        /// <summary>
        /// 堅固
        /// </summary>
        public static string Sturdy { get; private set; }
        /// <summary>
        /// 風の歌
        /// </summary>
        public static string WindSong { get; private set; }
        /// <summary>
        /// アカデミー
        /// </summary>
        public static string Academy { get; private set; }
        /// <summary>
        /// 湖
        /// </summary>
        public static string Lake { get; private set; }
        /// <summary>
        /// 呪い
        /// </summary>
        public static string Curse { get; private set; }
        /// <summary>
        /// 交渉
        /// </summary>
        public static string Savvy { get; private set; }
        /// <summary>
        /// 元素
        /// </summary>
        public static string Elemental { get; private set; }
        /// <summary>
        /// 錬金術
        /// </summary>
        public static string Alchemy { get; private set; }
        /// <summary>
        /// 鍛冶場
        /// </summary>
        public static string Weapon { get; private set; }
        //public static string Purgatory { get; private set; }
        /// <summary>
        /// 生命
        /// </summary>
        public static string Vitality { get; private set; }
        /// <summary>
        /// 夜空
        /// </summary>
        public static string Stargaze { get; private set; }
        /// <summary>
        /// 空の歌
        /// </summary>
        public static string SkySong { get; private set; }
        /// <summary>
        /// 酩酊
        /// </summary>
        public static string Drunk { get; private set; }
        /// <summary>
        /// 運命
        /// </summary>
        public static string Fortune { get; private set; }
        /// <summary>
        /// 魔導書
        /// </summary>
        public static string Grimoire { get; private set; }
    }
}
