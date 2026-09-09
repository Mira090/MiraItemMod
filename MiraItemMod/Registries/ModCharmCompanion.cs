using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModCharmCompanion : ModCharm
    {
        public static ModCharmCompanion Create(string name, int maxLevel, Func<ModUnit> unit)
            => Create<Charm_SummonUnit>(name, maxLevel, unit);
        public static ModCharmCompanion Create<T>(string name, int maxLevel, Func<ModUnit> unit) where T : Charm_SummonUnit
            => new ModCharmCompanion().SetCharmCompanion<T>(name, maxLevel, unit);
        internal ModCharmCompanion SetCharmCompanion<T>(string name, int maxLevel, Func<ModUnit> unit) where T : Charm_SummonUnit
        {
            SetCharm<T>(name, maxLevel, true);
            UnitName = new LocalizedString("Companion_" + Name);
            GetUnit = unit;
            return this.SetDamageId().SetEffects("Charm_SummonCompanion_Effect");
        }

        public int[] DamageByLevel { get; internal set; } = new int[5] { 8, 15, 25, 34, 44 };
        public int[] HpByLevel { get; internal set; } = new int[5] { 10, 20, 30, 40, 50 };
        public LocalizedString UnitName { get; internal set; }
        public Func<ModUnit> GetUnit { get; internal set; }
        public override GameObject CreateResourcePrefab()
        {
            var gameObject = base.CreateResourcePrefab();
            if(gameObject.TryGetComponent<Charm_SummonUnit>(out var summon))
            {
                summon.damageByLevel = DamageByLevel;
                summon.hpByLevel = HpByLevel;
                summon.unitName = UnitName;
                if (DamageId != null)
                    summon.followerDamageId = DamageId.Id;
            }
            return gameObject;
        }
        public virtual void InitUnit()
        {
            if (ResourcePrefab == null)
                return;
            if (ResourcePrefab.TryGetComponent<Charm_SummonUnit>(out var summon))
            {
                var unit = GetUnit?.Invoke();
                if(unit == null)
                    return;
                if (unit.Prefab == null)
                    unit.InitPrefab();
                summon.unitPrefab = unit.Prefab;
            }
        }
    }
}
