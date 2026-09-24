using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Sacrifice
{
    public class Charm_SacrificeDamage : Charm_Sacrifice
    {
        public float requiredDamage = 1000f;
        public bool useFromType = false;
        public EDamageFromType fromType;
        public bool useElementalType = false;
        public EDamageElementalType elementalType;

        [SyncVar]
        public float Count;

        public float NetworkCount
        {
            get
            {
                return Count;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref Count, 0x200L, null);
            }
        }

        public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
        {
            base.SerializeSyncVars(writer, forceAll);
            if (forceAll)
            {
                writer.WriteFloat(Count);
                return;
            }

            writer.WriteVarULong(syncVarDirtyBits);
            if ((syncVarDirtyBits & 0x200L) != 0L)
            {
                writer.WriteFloat(Count);
            }
        }

        public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            base.DeserializeSyncVars(reader, initialState);
            if (initialState)
            {
                GeneratedSyncVarDeserialize(ref Count, null, reader.ReadFloat());
                return;
            }

            long num = (long)reader.ReadVarULong();
            if ((num & 0x200L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref Count, null, reader.ReadFloat());
            }
        }


        private void Awake()
        {
            effectHUD_ID = "SacrificeDamage".ToSephiriaUpperId();
        }

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? requiredDamage.ToString(".##") + "→" + requiredDamage.ToString(".##") : requiredDamage.ToString(".##");
            string count = "-";
            if (!ignoreAvatarStatus && avatar != null)
            {
                try
                {
                    count = ((int)NetworkCount).ToString();
                }
                catch (Exception e)
                {
                    Core.LoggerError(e);
                }
            }
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("DAMAGE", value),
            new Loc.KeywordValue("REWARD", rewardEntity.aName.ToString()),
            new Loc.KeywordValue("CURRENT", count)
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
            NetworkAvatar.SetEffectHUDFillAmount(GetCharmHUDID(), 1 - NetworkCount / requiredDamage);
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), ((int)NetworkCount).ToString());
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (avatar is DamageDummy)
                return;
            if(useFromType && damage.fromType != fromType)
                return;
            if (useElementalType && !damage.IsSameElementalType(elementalType))
                return;
            NetworkCount += damage.damage;
            if (NetworkCount >= requiredDamage)
            {
                quest = true;
            }
            NetworkAvatar.SetEffectHUDFillAmount(GetCharmHUDID(), 1 - NetworkCount / requiredDamage);
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), ((int)NetworkCount).ToString());
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetFloat($"CharmSaveData_SacrificeDamage_{Item.InstanceID}_Stack", NetworkCount);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            NetworkCount = saveData.GetFloat($"CharmSaveData_SacrificeDamage_{Item.InstanceID}_Stack", 0);
        }
    }
}
