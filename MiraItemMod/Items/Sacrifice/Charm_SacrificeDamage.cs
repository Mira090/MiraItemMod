using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items.Sacrifice
{
    public class Charm_SacrificeDamage : Charm_Sacrifice
    {
        public float damaged = 0;
        public int damagedClient = 0;
        public float requiredDamage = 1000f;
        public bool useFromType = false;
        public EDamageFromType fromType;
        public bool useElementalType = false;
        public EDamageElementalType elementalType;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? requiredDamage.ToString(".##") + "→" + requiredDamage.ToString(".##") : requiredDamage.ToString(".##");
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("DAMAGE", value),
            new Loc.KeywordValue("REWARD", rewardEntity.aName.ToString()),
            new Loc.KeywordValue("CURRENT", damagedClient.ToString())
            };
        }
        public void Start()
        {
            Events.OnValueRecieved += OnValueRecieved;
        }
        public void OnDestroy()
        {
            Events.OnValueRecieved -= OnValueRecieved;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (avatar is DamageDummy)
                return;
            if(useFromType && damage.fromType != fromType)
                return;
            if (useElementalType && !damage.IsSameElementalType(elementalType))
                return;
            damaged += damage.damage;
            damagedClient = (int)damaged;
            Events.CommandValue(NetworkAvatar, Item, damagedClient);
            if (damaged >= requiredDamage)
            {
                quest = true;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        private void OnValueRecieved(string command, uint netId, int value)
        {
            //Core.Logger("OnValueRecieved: " + netId + " to " + base.netId);
            if (netId == base.netId)
            {
                damagedClient = value;
            }
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetFloat($"CharmSaveData_SacrificeDamage_{Item.InstanceID}_Stack", damaged);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            damaged = saveData.GetFloat($"CharmSaveData_SacrificeDamage_{Item.InstanceID}_Stack", 0);
            damagedClient = (int)damaged;
            Events.CommandValue(NetworkAvatar, Item, damagedClient);
        }
    }
}
