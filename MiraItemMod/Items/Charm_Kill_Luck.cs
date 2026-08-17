using Mirror;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_Kill_Luck : Charm_StatusInstance
    {
        public int[] luckByLevel = new int[4] { 1, 1, 1, 2 };
        private int divide = 5;
        public int max = 20;

        [SyncVar]
        public int Count;

        public int NetworkCount
        {
            get
            {
                return Count;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref Count, 512uL, null);
            }
        }

        public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
        {
            base.SerializeSyncVars(writer, forceAll);
            if (forceAll)
            {
                writer.WriteInt(Count);
                return;
            }

            writer.WriteVarULong(syncVarDirtyBits);
            if ((syncVarDirtyBits & 0x200L) != 0L)
            {
                writer.WriteInt(Count);
            }
        }

        public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            base.DeserializeSyncVars(reader, initialState);
            if (initialState)
            {
                GeneratedSyncVarDeserialize(ref Count, null, reader.ReadInt());
                return;
            }

            long num = (long)reader.ReadVarULong();
            if ((num & 0x200L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref Count, null, reader.ReadInt());
            }
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? luckByLevel.SafeRandomAccess(0) + "→" + luckByLevel.SafeRandomAccess(maxLevel) : luckByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            var count = 0;
            if(!ignoreAvatarStatus && avatar != null)
            {
                try
                {
                    count = NetworkCount;
                }
                catch
                {

                }
            }

            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("LUCK", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", "+" + (showAllLevel ? GetLuck(maxLevel, count).ToString() : GetLuck(LevelToIdx(level), count).ToString()), GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", count.ToString(), GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DIVIDE", divide.ToString()),
            new Loc.KeywordValue("MAX", "+" + max.ToString())
            };
        }

        private int GetLuck(int idx, int count)
        {
            return Mathf.Clamp(luckByLevel.SafeRandomAccess(idx) * (count / divide), 0, max);
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, GetLuck(CurrentLevelToIdx(), NetworkCount));
            networkAvatar.OnKillUnit += OnKillUnit;
            //networkAvatar.OnStartBattle += OnStartBattle;
        }


        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -GetLuck(CurrentLevelToIdx(), NetworkCount));
            networkAvatar.OnKillUnit -= OnKillUnit;
            //networkAvatar.OnStartBattle -= OnStartBattle;
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            UnitAvatar networkAvatar = NetworkAvatar;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -GetLuck(LevelToIdx(oldLevel), NetworkCount));
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, GetLuck(LevelToIdx(newLevel), NetworkCount));
        }
        protected void OnKillUnit(UnitAvatar avatar, DamageInstance damage)
        {
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -GetLuck(CurrentLevelToIdx(), NetworkCount));
            NetworkCount++;
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, GetLuck(CurrentLevelToIdx(), NetworkCount));
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        protected void OnStartBattle()
        {
            NetworkAvatar.AddCustomStat(ECustomStat.Luck, -GetLuck(CurrentLevelToIdx(), NetworkCount));
            NetworkCount = 0;
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_KillLuck_{Item.InstanceID}_Stack", NetworkCount);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            NetworkCount = saveData.GetInt($"CharmSaveData_KillLuck_{Item.InstanceID}_Stack", 0);
        }
    }
}
