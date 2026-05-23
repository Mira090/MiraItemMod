using Mirror;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_OnDamagedCosumeDash : Charm_StatusInstance
    {
        public int[] consume = new int[] { 2, 2, 3, 3, 4, 4 };
        public int[] percent = new int[] { 24, 36, 52, 64, 84, 96 };
        public int[] time = new int[] { 20, 19, 18, 16, 14, 12 };
        public Timer cooldownTimer = new Timer(10);
        private bool isInCooldown = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? time.SafeRandomAccess(0) + "→" + time.SafeRandomAccess(maxLevel) : time.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? percent.SafeRandomAccess(0) + "→" + percent.SafeRandomAccess(maxLevel) : percent.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value3 = showAllLevel ? consume.SafeRandomAccess(0) + "→" + consume.SafeRandomAccess(maxLevel) : consume.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("COOLDOWN", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("PERCENT", value2 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DASH", value3, GetNegativeColor(virtualLevelOffset))
            };
        }
        public UnitAvatar EventAvatar;
        private void Start()
        {
            if (NetworkAvatar != null)
            {
                EventAvatar = NetworkAvatar;
                EventAvatar.OnDamagedClientside += OnDamageApplied;
            }
            Events.OnValueRecieved += OnValueRecieved;
        }
        private void OnDestroy()
        {
            if(EventAvatar != null)
            {
                EventAvatar.OnDamagedClientside -= OnDamageApplied;
            }
            Events.OnValueRecieved -= OnValueRecieved;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime * (1 + NetworkAvatar.GetCustomStat(ECustomStat.DashRecovery) / 100f)))
            {
                isInCooldown = false;
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            cooldownTimer.time = time.SafeRandomAccess(CurrentLevelToIdx());
        }

        private void OnValueRecieved(string command, uint netId, int value)
        {
            if(netId == base.netId)
            {
                if(value == 1 && NetworkServer.active)
                {
                    OnDashConsumed();
                }
            }
        }

        private void OnDamageApplied(DamageData damage)
        {
            if(!isInCooldown && NetworkAvatar.CurrentDashModule != null && NetworkAvatar.CurrentDashModule.MaxDashCount - NetworkAvatar.CurrentDashModule.GetCurrentDashCount >= consume.SafeRandomAccess(CurrentLevelToIdx()))
            {
                isInCooldown = true;
                NetworkAvatar.CurrentDashModule.currentDashCount += consume.SafeRandomAccess(CurrentLevelToIdx());
                if (NetworkServer.active)
                {
                    NetworkAvatar.Heal(damage.damage * (percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f));
                    OnDashConsumed();
                }
                else
                {
                    NetworkAvatar.CmdHeal(damage.damage * (percent.SafeRandomAccess(CurrentLevelToIdx()) / 100f));
                    Events.CommandValue(NetworkAvatar, Item, 1);
                }
            }
        }
        private void OnDashConsumed()
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void Charm_OnDamagedCosumeDash::OnDashConsumed()' called when server was not active");
                return;
            }
            if (Core.LogMedium)
                Core.Logger("OnDashConsumed: " + NetworkAvatar);
            if (NetworkAvatar == null)
                return;
            NetworkAvatar.OnDashServerside?.Invoke(NetworkAvatar.transform.position, true);
            return;
            if (WeaponController.currentWeapon.gameObject.TryGetComponent<WeaponAddonKatana_SummonGhost>(out var ghost))
            {
                //ghost
            }
            if (WeaponController.currentWeapon is WeaponSimple_Katana katana)
            {
                //ghost
            }
            if (NetworkAvatar.GetCustomStatUnsafe("KATANADASHSTACK") > 0)//オーバーヒート
            {
                //katana
            }
            if (NetworkAvatar.GetCustomStatUnsafe("DASHATTACKICEHAMMER") > 0)//氷の斧
            {
                //katana
            }
            foreach (var charm in NetworkAvatar.Inventory.charms.Values)
            {
                if (charm is Charm_RockElephant elephant)
                {
                    //elephant
                }
            }
            foreach (var charm in NetworkAvatar.Inventory.charms.Values)
            {
                if (charm is Charm_IceHammer hammer)
                {
                    //hammer
                }
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            cooldownTimer.time = time.SafeRandomAccess(LevelToIdx(newLevel));
        }
    }
}
