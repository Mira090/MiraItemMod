using MiraItemMod.Compats;
using MiraItemMod.Registries;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonCommon_DebuffByDefense :WeaponAddon
    {
        public List<CharacterDebuff> Debuffs
        {
            get
            {
                var list = new List<CharacterDebuff> { SephiriaPrefabs.Poison, SephiriaPrefabs.Burn, SephiriaPrefabs.Frostbite, SephiriaPrefabs.Electric };
                if (ModCompat.GetHasLoaded<StarsSephiriaModCompat>())
                {
                    list.Add(SephiriaPrefabs.WindErosion);
                }
                return list;
            }
        }
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack || avatar.IsDead)
                return;
            var defense = Mathf.Abs(parent.Networkowner.unitAvatar.GetCustomStat(ECustomStat.DamageReduction));
            if (defense > 0)
            {
                for (int q = 0; q < (defense / 10); q++)
                {
                    if(30.Percent())
                        avatar.ApplyDebuff(Debuffs.SafeRandomAccess(q, ArrayExtensions.ERandomAccessType.Repeat), parent.Networkowner.unitAvatar);
                }
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
        }

        public override Loc.KeywordValue[] BuildKeywords()
        {
            var separator = new LocalizedString("WeaponAddon_Staff_Sickle_T3_Debuff_Separator").ToString().Replace("-", string.Empty);
            var sb = new StringBuilder();
            sb.Append("<tag=Debuff_Poison>");
            sb.Append(separator);
            sb.Append("<tag=Burn>");
            sb.Append(separator);
            sb.Append("<tag=Frostbite>");
            sb.Append(separator);
            sb.Append("<tag=Electric>");
            if (ModCompat.GetHasLoaded<StarsSephiriaModCompat>())
            {
                sb.Append(separator);
                sb.Append("<tag=WindErosion>");
            }
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("DEBUFFS", sb.ToString()),
            };
        }
    }
}
