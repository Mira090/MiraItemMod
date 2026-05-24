using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Combos
{
    public class ComboEffect_Stargaze : ComboEffectBase
    {
        private string damageId = "Ability_Stargaze";
        public int debuffActivateComboCount = 2;

        public LocalizedString debuffActivateEffectNameString = new LocalizedString("ComboEffect_StargazeActivateName");

        public LocalizedString debuffActivateEffectString = new LocalizedString("ComboEffect_StargazeActivate");

        public override Loc.KeywordValue[] BuildDefaultEffectKeyword()
        {
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("EFFECTNAME", debuffActivateEffectNameString.ToString())
            };
        }
        protected override void OnRequestComboData(UnitAvatar avatar, List<ComboEffectElement> elements)
        {
            base.OnRequestComboData(avatar, elements);
            Loc.KeywordValue[] keywordValues = new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("EFFECTNAME", debuffActivateEffectNameString.ToString()),
            };
            elements.Add(new ComboEffectElement
            {
                comboCount = debuffActivateComboCount,
                effectName = KeywordDatabase.Convert(Loc.Convert(debuffActivateEffectString.ToString(), keywordValues), useColor: false)
            });
        }
        protected override int OnEnableEffect(int comboCount, int oldComboCount)
        {
            int result = base.OnEnableEffect(comboCount, oldComboCount);
            if (comboCount >= debuffActivateComboCount && (bool)Networkavatar)
            {
                result = debuffActivateComboCount;
                Networkavatar.OnAttackUnit += OnAttackUnit;
            }
            else if (comboCount < debuffActivateComboCount && oldComboCount >= debuffActivateComboCount && (bool)Networkavatar)
            {
                Networkavatar.OnAttackUnit -= OnAttackUnit;
            }
            return result;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (!Networkavatar.IsDead && IsSpecialAttack(damage))
            {
                float customStatUnsafe = Networkavatar.Inventory.charms.Values.Sum(charm => charm.CurrentLevelToIdx());
                if (customStatUnsafe > 0 && avatar != null)
                {
                    DamageInstance d = DamageInstance.GetDamage(Networkavatar, damageId, avatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                    d.SetCustomColor(true, new Color(0.5f, 0, 0));
                    avatar.ApplyDamage(d);
                }
            }
        }
        private bool IsSpecialAttack(DamageInstance damage)
        {
            if(Networkavatar.GetCustomStatUnsafe("PlanetStargaze".ToUpperInvariant()) > 0)
                return ModUtil.PlanetDamageIds.Contains(damage.id);
            return damage.id == "Weapon_SpecialAttack" || damage.id == "Weapon_SpecialAttack_Amethyst";
        }

        protected override void OnDisableEffect()
        {
            base.OnDisableEffect();
            if ((bool)Networkavatar)
            {
                Networkavatar.OnAttackUnit -= OnAttackUnit;
            }
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
