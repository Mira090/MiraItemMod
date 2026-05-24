using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MiraItemMod.Utilities;

namespace MiraItemMod.Items
{
    public class Charm_ElectricStun : Charm_StatusInstance
    {
        public int[] percentByLevel = new int[] { 3, 5, 7, 9 };
        private int count = 0;
        private int countView;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? percentByLevel.SafeRandomAccess(0) + "→" + percentByLevel.SafeRandomAccess(maxLevel) : percentByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = Mathf.Min(100, countView * percentByLevel.SafeRandomAccess(LevelToIdx(level))) + "%";
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", value2, GetPositiveColor(virtualLevelOffset))
            };
        }
        private void OnValueRecieved(string command, uint netId, int value)
        {
            if (netId == base.netId)
            {
                countView = value;
            }
        }
        private void Awake()
        {
            effectHUD_ID = "Electric_Stun".ToSephiriaUpperId();
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
            NetworkAvatar.OnAddedDebuffOnTarget += OnAddedDebuffOnTarget;
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{Mathf.Min(100, countView * percentByLevel.SafeRandomAccess(CurrentLevelToIdx()))}/{100}");
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAddedDebuffOnTarget -= OnAddedDebuffOnTarget;
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }
        private void OnAddedDebuffOnTarget(CharacterDebuff debuff, string id)
        {
            if(id == SephiriaPrefabs.Electric.ID || id == SephiriaPrefabs.Plasma.ID)
            {
                count++;
                countView = count;
                NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{Mathf.Min(100, countView * percentByLevel.SafeRandomAccess(CurrentLevelToIdx()))}/{100}");
                NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
                Events.CommandValue(NetworkAvatar, Item, countView);
                SaveItemOnServer(SaveManager.CurrentRun);
            }
        }
        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if (!damage.IsSameElementalType(EDamageElementalType.Lightning))
                return;
            foreach(var debuff in avatar.Debuffs)
            {
                if (debuff.ID == SephiriaPrefabs.Electric.ID)
                    return;
            }
            float num = percentByLevel[CurrentLevelToIdx()] * count;
            if (num < 0f || UnityEngine.Random.Range(0f, 1f) > num / 100f)
                return;

            damage.stun = true;
            count = 0;
            countView = count;
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{Mathf.Min(100, countView * percentByLevel.SafeRandomAccess(CurrentLevelToIdx()))}/{100}");
            Events.CommandValue(NetworkAvatar, Item, countView);
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{Mathf.Min(100, countView * percentByLevel.SafeRandomAccess(LevelToIdx(newLevel)))}/{100}");
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_ElectricStun_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_ElectricStun_{Item.InstanceID}_Stack", 0);
            countView = count;
            Events.CommandValue(NetworkAvatar, Item, countView);
        }
    }
}
