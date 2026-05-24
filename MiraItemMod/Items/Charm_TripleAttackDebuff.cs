using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_TripleAttackDebuff : Charm_VariableMaxLevel
    {
        private int type = 0;

        public Timer debuffInterval = new Timer(0.5f);
        private bool debuff = true;

        public int[] intervalByLevel = new int[] { 50, 40, 30, 25, 20, 16, 12, 8, 6, 4, 2 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? intervalByLevel.SafeRandomAccess(0) / 10f + "→" + intervalByLevel.SafeRandomAccess(maxLevel) / 10f : (intervalByLevel.SafeRandomAccess(LevelToIdx(level)) / 10f).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("INTERVAL", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.OnAttackUnit += OnAttackUnit;

        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            debuffInterval.time = intervalByLevel.SafeRandomAccess(CurrentLevelToIdx()) / 10f;
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            debuffInterval.time = intervalByLevel.SafeRandomAccess(LevelToIdx(newLevel)) / 10f;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if(!debuff && debuffInterval.Update(Time.deltaTime))
            {
                debuff = true;
            }
        }

        private void OnAttackUnit(UnitAvatar unitAvatar, DamageInstance damageInstance)
        {
            if (IsEffectEnabled && debuff)
            {
                if (type == 0 && damageInstance.IsSameElementalType(EDamageElementalType.Fire))
                {
                    type = 1;
                    debuff = false;
                    unitAvatar.ApplyDebuff(SephiriaPrefabs.Burn, NetworkAvatar);
                }
                else if (type == 1 && damageInstance.IsSameElementalType(EDamageElementalType.Ice))
                {
                    type = 2;
                    debuff = false;
                    unitAvatar.ApplyDebuff(SephiriaPrefabs.Frostbite, NetworkAvatar);
                }
                else if (type == 2 && damageInstance.IsSameElementalType(EDamageElementalType.Lightning))
                {
                    type = 0;
                    debuff = false;
                    unitAvatar.ApplyDebuff(SephiriaPrefabs.Electric, NetworkAvatar);
                }
            }
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_TripleAttackDebuff_{Item.InstanceID}_Stack", type);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            type = saveData.GetInt($"CharmSaveData_TripleAttackDebuff_{Item.InstanceID}_Stack", 0);
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
