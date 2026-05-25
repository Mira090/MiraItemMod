using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_RandomDebuff : Charm_StatusInstance
    {
        public Timer cooldownTimer = new Timer(1f);
        public bool isInCooldown;
        public float[] chance = new float[] { 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            if (avatar == null || ignoreAvatarStatus)
            {
                return new Loc.KeywordValue[2]
                {
                    new Loc.KeywordValue("PERCENT", "-" + "%"),
                new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString())
                };
            }
            float percent = GetDebuffChance(avatar.GetCustomStat(ECustomStat.Luck), LevelToIdx(level));
            string value = percent.ToString(".##");
            return new Loc.KeywordValue[2]
            {
                new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString())
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }

        public float GetDebuffChance(int luck, int idx)
        {
            luck *= 100;
            return 100f * Mathf.Log(luck / 6200f + 1f) * chance.SafeRandomAccess(idx);
        }
        public CharacterDebuff GetDebuff(UnitAvatar target)
        {
            var list = new List<CharacterDebuff>
            {
                SephiriaPrefabs.Burn,
                SephiriaPrefabs.Frostbite,
                SephiriaPrefabs.Electric,
                SephiriaPrefabs.Wound,
                SephiriaPrefabs.Poison,
                SephiriaPrefabs.Plasma
            };
            foreach (var debuff in target.Debuffs)
            {
                if(debuff.CurrentStack == debuff.MaxStackCount)
                {
                    list.Remove(debuff);
                }
                if (debuff.ID == SephiriaPrefabs.Freeze.ID)
                {
                    list.Remove(SephiriaPrefabs.Frostbite);
                }
            }

            return list.GetRandom();
        }
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            var chance = GetDebuffChance(NetworkAvatar.GetCustomStat(ECustomStat.Luck), CurrentLevelToIdx());
            if (IsEffectEnabled && !isInCooldown && chance.Percent())
            {
                isInCooldown = true;
                avatar.ApplyDebuff(GetDebuff(avatar), NetworkAvatar);
            }
        }
    }
}
