using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_MaxMPPower : Charm_StatusInstance
    {
        private int mp = 0;
        private int require = 3;
        private int requireConsume = 4;
        private int[] percent = new int[] { 5, 10, 20, 40 };
        private int[] damagePercent = new int[] { 20, 30, 40, 80, 120 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? GetDamage(avatar, 0) + "→" + GetDamage(avatar, maxLevel) : GetDamage(avatar, level).ToString();
            string value2 = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value3 = showAllLevel ? damagePercent.SafeRandomAccess(0) + "→" + damagePercent.SafeRandomAccess(maxLevel) : damagePercent.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("PERCENT", value2 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", value3 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("MP", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        private int GetDamage(UnitAvatar avatar, int level)
        {
            if (avatar.GetCustomStatUnsafe("INFINITYMP") > 0)
                return 0;
            if (avatar.MP >= avatar.MaxMp * (percent.SafeRandomAccess(LevelToIdx(level)) / 100f))
            {
                var d = avatar.MaxMp * (percent.SafeRandomAccess(LevelToIdx(level)) / 100f);
                return Mathf.RoundToInt((avatar.MaxMp - (avatar.MP - d)) * damagePercent.SafeRandomAccess(LevelToIdx(level)) / 100f);
            }
            else
            {
                var d = avatar.MP;
                var a = d / (avatar.MaxMp * (percent.SafeRandomAccess(LevelToIdx(level)) / 100f));
                return Mathf.RoundToInt(a * (avatar.MaxMp - (avatar.MP - d)) * damagePercent.SafeRandomAccess(LevelToIdx(level)) / 100f);
            }
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (idx == 0 && level < require)
                return null;
            if (idx == 1 && level < requireConsume)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
            if (CurrentLevelToIdx() >= require)
            {
                mp = NetworkAvatar.GetCustomStat(ECustomStat.MPRegen);
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, -mp);
                NetworkAvatar.AddCustomStat(ECustomStat.FinalMP, mp);
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
            if (mp != 0)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, mp);
                NetworkAvatar.AddCustomStat(ECustomStat.FinalMP, -mp);
                mp = 0;
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            if (!IsEffectEnabled)
                return;
            if(newLevel >= require && oldLevel < require)
            {
                mp = NetworkAvatar.GetCustomStat(ECustomStat.MPRegen);
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, -mp);
                NetworkAvatar.AddCustomStat(ECustomStat.FinalMP, mp);
            }
            else if (newLevel < require && oldLevel >= require)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, mp);
                NetworkAvatar.AddCustomStat(ECustomStat.FinalMP, -mp);
                mp = 0;
            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (NetworkAvatar.GetCustomStat(ECustomStat.MPRegen) == 0 || !IsEffectEnabled)
                return;
            if (CurrentLevelToIdx() >= require)
            {
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, mp);
                NetworkAvatar.AddCustomStat(ECustomStat.FinalMP, -mp);
                mp = NetworkAvatar.GetCustomStat(ECustomStat.MPRegen);
                NetworkAvatar.AddCustomStat(ECustomStat.MPRegen, -mp);
                NetworkAvatar.AddCustomStat(ECustomStat.FinalMP, mp);
            }
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar unit, DamageInstance damage)
        {
            if (CurrentLevelToIdx() < requireConsume)
                return;
            if (damage.id == "Weapon_SpecialAttack" || damage.id == "Weapon_SpecialAttack_Amethyst")
            {
                if(NetworkAvatar.GetCustomStatUnsafe("INFINITYMP") > 0)
                {
                    var d = NetworkAvatar.MaxMp * (percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
                    NetworkAvatar.UseMp(Mathf.RoundToInt(d));
                    damage.SetCustomColor(false, new Color(0.5f, 0.5f, 1f));
                    damage.damage += (NetworkAvatar.MaxMp - NetworkAvatar.MP) * (damagePercent.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
                }
                else if(NetworkAvatar.MP >= NetworkAvatar.MaxMp * (percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f))
                {
                    var d = NetworkAvatar.MaxMp * (percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
                    NetworkAvatar.UseMp(Mathf.RoundToInt(d));
                    damage.SetCustomColor(false, new Color(0.5f, 0.5f, 1f));
                    damage.damage += (NetworkAvatar.MaxMp - NetworkAvatar.MP) * (damagePercent.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
                }
                else if(NetworkAvatar.MP > 0)
                {
                    var d = NetworkAvatar.MP;
                    NetworkAvatar.UseMp(d);
                    damage.SetCustomColor(false, new Color(0.75f, 0.75f, 1f));
                    var a = d / (NetworkAvatar.MaxMp * (percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f));
                    damage.damage += a * (NetworkAvatar.MaxMp - NetworkAvatar.MP) * (damagePercent.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
                }
            }
        }
    }
}
