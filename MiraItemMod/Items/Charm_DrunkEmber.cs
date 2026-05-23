using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_DrunkEmber : Charm_StatusInstance, IAttackableCharm
    {
        public string damageId = "Charm_DrunkEmber";
        public int[] damage = new int[] { 35, 40, 45, 55, 65, 80, 100 };
        public float[] time = new float[] { 0.5f };
        public Timer cooldownTimer = new Timer(0.5f);
        private bool isInCooldown = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? time.SafeRandomAccess(0).ToString("0.#") + "→" + time.SafeRandomAccess(maxLevel).ToString("0.#") : time.SafeRandomAccess(LevelToIdx(level)).ToString("0.#");
            string value2 = showAllLevel ? damage.SafeRandomAccess(0) + "→" + damage.SafeRandomAccess(maxLevel) : damage.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("COOLDOWN", value),
            new Loc.KeywordValue("DAMAGE", value2 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", "1%", GetPositiveColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
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
            cooldownTimer.time = time.SafeRandomAccess(CurrentLevelToIdx());
            NetworkAvatar.OnAddedDebuffOnTarget += OnAddedDebuffOnTarget;
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if(!isInCooldown && damage.fromType == EDamageFromType.DirectAttack)
            {
                var percent = Mathf.Max(0, -NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction));
                if (UnityEngine.Random.Range(0f, 100f) < percent)
                {
                    isInCooldown = true;
                    avatar.ApplyDebuff(SephiriaPrefabs.Burn, NetworkAvatar);
                }
            }
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            cooldownTimer.time = time.SafeRandomAccess(LevelToIdx(newLevel));
        }
        private void OnAddedDebuffOnTarget(CharacterDebuff debuff, string id)
        {
            if (id != "BURN")
                return;
            if (debuff.CurrentStack == debuff.MaxStackCount)
            {
                var unitAvatar = debuff.Target;
                var stack = debuff.CurrentStack;
                debuff.RequestEnd();

                for(int q = 0; q < stack; q++)
                {
                    float customStatUnsafe = Charm_Basic.CalculateDamage(this);
                    if (customStatUnsafe > 0 && unitAvatar != null)
                    {
                        DamageInstance d = DamageInstance.GetDamage(NetworkAvatar, damageId, unitAvatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 1, 1f);
                        d.elementalType = EDamageElementalType.Fire;
                        unitAvatar.ApplyDamage(d);
                    }
                }
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAddedDebuffOnTarget -= OnAddedDebuffOnTarget;
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        public float GetDamage(UnitAvatar avatar)
        {
            float customStatUnsafe = damage.SafeRandomAccess(CurrentLevelToIdx()) / 100f * Mathf.Max(0, -NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction));
            customStatUnsafe += customStatUnsafe * NetworkAvatar.GetCustomStatUnsafe("BURNDAMAGE") / 100f;
            return customStatUnsafe;
        }
    }
}
