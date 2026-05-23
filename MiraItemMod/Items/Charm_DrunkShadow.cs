using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_DrunkShadow : Charm_StatusInstance
    {
        public int[] time = new int[] { 20, 18, 15, 12 };
        public Timer cooldownTimer = new Timer(20);
        private bool isInCooldown = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? time.SafeRandomAccess(0) + "→" + time.SafeRandomAccess(maxLevel) : time.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("COOLDOWN", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime * (1 + NetworkAvatar.GetCustomStat(ECustomStat.Evasion) / 100f)))
            {
                isInCooldown = false;
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            cooldownTimer.time = time.SafeRandomAccess(CurrentLevelToIdx());
            NetworkAvatar.OnCalculateDamage += OnCalculateDamage;
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            cooldownTimer.time = time.SafeRandomAccess(LevelToIdx(newLevel));
        }

        private void OnCalculateDamage(DamageInstance damage)
        {
            if (NetworkAvatar == null || isInCooldown)
                return;
            var d = damage.damage;
            int ignore = damage.ignoreDefense;
            ignore += (damage.origin as UnitAvatar)?.GetCustomStatUnsafe("IGNOREDEFENSE") ?? 0;
            var defe = NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction);

            float reduc;
            if (defe > 0f)
            {
                reduc = d * Mathf.Log(defe / 40f + 1f) * 0.445f;
                if (reduc > d)
                {
                    reduc = d;
                }
            }
            else if (defe < 0f)
            {
                reduc = d * defe / 100f;
            }
            else
            {
                reduc = 0f;
            }
            if (ignore > 0)
            {
                reduc *= 1f - ignore / 100f;
            }
            d -= reduc;

            if(d >= NetworkAvatar.hp)
            {
                isInCooldown = true;
                damage.absoluteEvasionPercent = 100f;
                //Core.Logger("Evade!");
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnCalculateDamage -= OnCalculateDamage;
        }
    }
}
