using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_DashAttackMore : Charm_StatusInstance
    {
        public int[] time = new int[] { 10, 8, 6, 4 };
        public int[] percent = new int[] { 20, 40, 60, 80 };
        public Timer cooldownTimer = new Timer(10);
        private bool isInCooldown = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? time.SafeRandomAccess(0) + "→" + time.SafeRandomAccess(maxLevel) : time.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("COOLDOWN", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", value2 + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnDashAttack += OnDashAttack;
            cooldownTimer.time = time.SafeRandomAccess(CurrentLevelToIdx());
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }
        private void OnDashAttack(CombatBehaviour combat, DamageInstance damage, ProjectileBase projectile)
        {
            if (!isInCooldown && UnityEngine.Random.Range(0f, 100f) < percent.SafeRandomAccess(CurrentLevelToIdx()))
            {
                isInCooldown = true;
                NetworkAvatar.CurrentDashModule.RestoreDashCount(1);
                /*
                if (NetworkClient.active)
                {
                    if (NetworkAvatar.CurrentDashModule.currentDashCount > 0)
                        NetworkAvatar.CurrentDashModule.currentDashCount--;
                }
                else
                {
                    DungeonManager.Instance.Chat(NetworkAvatar as PlayerAvatar, "Mod", "/dash_heal");
                }*/
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            cooldownTimer.time = time.SafeRandomAccess(LevelToIdx(newLevel));
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnDashAttack -= OnDashAttack;
        }
    }
}
