using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items
{
    public class Charm_MagitechFrostRelic : Charm_StatusInstance
    {
        public static string Status = "MagitechFrostRelic".ToUpperInvariant();
        public static int BuffDuration = 22;

        public int[] percent = new int[] { 30, 40, 50, 60, 70, 80 };



        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("BUFF", BuffDuration.ToString())
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.flag.Contains("FROSTRELIC"))
            {
                if (!percent.SafeRandomAccess(CurrentLevelToIdx()).Percent())
                    return;
                foreach (var debuff in avatar.Debuffs)
                {
                    if (debuff is CharacterDebuff_Plasma plasma)
                    {
                        float damageRatio = plasma.BuffDuration / plasma.defaultDuration;
                        plasma.InvokeAttack(plasma.NetworkTarget, plasma.CurrentStack, damageRatio);
                    }
                    else if (debuff is CharacterDebuff_Electric electric)
                    {
                        float damageRatio = electric.BuffDuration / electric.defaultDuration;
                        electric.InvokeAttack(electric.NetworkTarget, electric.CurrentStack, damageRatio);
                    }
                }
            }
            else if(damage.id == "Debuff_Electric" || damage.id == "Debuff_Plasma")
            {
                NetworkAvatar.ApplyBuff(Data.MagitechFrostRelicBuff, 1, NetworkAvatar, true);
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
