using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public class ModMiracleStatusDice : ModMiracleStatus
    {
        public static ModMiracleStatusDice Create(string name, int dice, params Miracle_StatusInstance.StatInfo[] stats)
            => new ModMiracleStatusDice().SetMiracleStatus<Miracle_Gambler>(name, dice, stats);
        public static ModMiracleStatusDice Create<T>(string name, int dice, params Miracle_StatusInstance.StatInfo[] stats) where T : Miracle_Gambler
            => new ModMiracleStatusDice().SetMiracleStatus<T>(name, dice, stats);
        internal ModMiracleStatusDice SetMiracleStatus<T>(string name, int dice, params Miracle_StatusInstance.StatInfo[] stats) where T : Miracle_Gambler
        {
            SetItem(name);

            MiracleType = typeof(T);
            Stats = stats;
            Dice = dice;

            return this;
        }
        public int Dice { get; internal set; }
        public override GameObject CreateResourcePrefab()
        {
            var o = new GameObject("TMiracle_" + Name);
            var miracle = o.AddComponent(MiracleType) as Miracle_Gambler;
            //Core.Logger($"CreateMiracleStatusDice");
            o.AddComponent<LogComponent>();
            o.hideFlags = HideFlags.HideAndDontSave;
            miracle.id = Id;
            miracle.aName = LocalizedName;
            miracle.giveItem = GiveItem;
            miracle.categories = Categories;
            miracle.miracleImage = MiracleImage ?? AssetLoader.LoadSprite(MiracleImageFileName);
            miracle.tier = Tier;
            miracle.effects = Effects;
            miracle.statInfo = Stats;
            miracle.autoGenerateEffectString = AutoGenerateEffectString;
            miracle.dice = Dice;
            //miracle.enabled = false;
            return o;
        }
    }
}
