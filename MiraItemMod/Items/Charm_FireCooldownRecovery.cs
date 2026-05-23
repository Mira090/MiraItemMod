using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_FireCooldownRecovery : Charm_StatusInstance
    {
        private int[] cooldownByLevel = new int[] { 2, 2, 2, 2 };
        private int[] requireCount = new int[] { 5, 3, 2, 1 };
        private int count = 0;
        public Timer cooldownTimer = new Timer(0.04f, resetOnTime: false);
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? requireCount.SafeRandomAccess(0) + "→" + requireCount.SafeRandomAccess(maxLevel) : requireCount.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? cooldownByLevel.SafeRandomAccess(0) + "→" + cooldownByLevel.SafeRandomAccess(maxLevel) : cooldownByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("REQUIRE", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COOLDOWN", value2 + "%"),
            new Loc.KeywordValue("CURRENT", count.ToString())
            };
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (!cooldownTimer.Check())
            {
                cooldownTimer.AddTimer(Time.deltaTime);
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar unit, DamageInstance damage)
        {
            if(damage.IsSameElementalType(EDamageElementalType.Fire))
            {
                count++;
                if (count >= requireCount.SafeRandomAccess(CurrentLevelToIdx()) && cooldownTimer.Check())
                {
                    count = 0;
                    foreach (var charm in NetworkAvatar.Inventory.charms.Values)
                    {
                        if (charm is Charm_Magic magic)
                        {
                            //Core.Logger("Acc");
                            if (!magic.IsEffectEnabled || magic.currentAmmo >= magic.maxAmmo)
                                continue;
                            magic.AddCooldownBonus(cooldownByLevel.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
                        }
                    }
                    cooldownTimer.Ratio = 0f;
                }
                NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count}/{requireCount.SafeRandomAccess(CurrentLevelToIdx())}");
                NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
                SaveItemOnServer(SaveManager.CurrentRun);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_FireCooldownRecovery_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_FireCooldownRecovery_{Item.InstanceID}_Stack", 0);
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
