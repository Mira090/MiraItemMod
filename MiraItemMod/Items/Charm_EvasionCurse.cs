using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MiraItemMod.Items
{
    public class Charm_EvasionCurse : Charm_StatusInstance
    {
        public int[] poison = new int[] { 5, 10, 20 };

        public Timer cooldownTimer = new Timer(7f);
        public bool isInCooldown;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value2 = showAllLevel ? poison.SafeRandomAccess(0) + "→" + poison.SafeRandomAccess(maxLevel) : poison.SafeRandomAccess(LevelToIdx(level)).ToString();
            if (avatar == null || ignoreAvatarStatus)
            {
                return new Loc.KeywordValue[3]
                {
                    new Loc.KeywordValue("PERCENT", "-" + "%"),
                new Loc.KeywordValue("POISON", value2 + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString())
                };
            }
            float percent = GetAssasinateChance(avatar.GetCustomStat(ECustomStat.Evasion));
            string value = percent.ToString(".##");
            return new Loc.KeywordValue[3]
            {
                new Loc.KeywordValue("PERCENT", value + "%"),
                new Loc.KeywordValue("POISON", value2 + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString())
            };
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("ABSOLUTEEVASION", -10000);
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("ABSOLUTEEVASION", 10000);
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }
        private void OnAttackUnit(UnitAvatar unitAvatar, DamageInstance damageInstance)
        {
            if (IsEffectEnabled && !isInCooldown && poison.SafeRandomAccess(CurrentLevelToIdx()).Percent() && damageInstance.fromType == EDamageFromType.DirectAttack)
            {
                isInCooldown = true;
                unitAvatar.ApplyDebuff(SephiriaPrefabs.Poison, NetworkAvatar);
            }
        }

        public float GetAssasinateChance(int evasion)
        {
            if (evasion > 10000)
                evasion = 10000;
            return 100f * Mathf.Log(evasion / 6200f + 1f) * 0.2f;
        }
        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            var evasion = NetworkAvatar.GetCustomStat(ECustomStat.Evasion);
            float rate = GetAssasinateChance(evasion);
            var assassinate = Mathf.Clamp(rate, 0f, 100f);
            //Core.Logger("暗閃: " + assassinate + "%");
            if (Random.Range(0f, 100f) < assassinate)
            {
                damage.criticalChancePercent = 0;
                damage.color = new Color(1, 0, 0, 0);
                damage.useCustomColor = true;
                damage.damage += avatar.hp * 0.02f * avatar.Debuffs.Count();
                //damage.ignoreDefense += ignoreDegence.SafeRandomAccess(CurrentLevelToIdx());
                //Core.Logger("暗閃!: " + (max / min));
            }
        }
    }
}
