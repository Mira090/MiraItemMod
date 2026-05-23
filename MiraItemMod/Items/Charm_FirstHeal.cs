using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace SephiriaMod.Items
{
    public class Charm_FirstHeal : Charm_StatusInstance
    {
        private bool heal = false;
        private int healPercent = 5;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            int heal = healPercent;
            if(avatar != null)
            {
                heal += avatar.GetCustomStatUnsafe("DRUNKHEAL");
            }
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("HEAL", healPercent + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (heal && idx == 1)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (!heal)
            {
                heal = true;
                NetworkAvatar.HealPercent(healPercent + NetworkAvatar.GetCustomStatUnsafe("DRUNKHEAL"));
                SaveItemOnServer(SaveManager.CurrentRun);
                Events.ChatSound(NetworkAvatar, nameof(Events.HealSound));
            }
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetBool($"CharmSaveData_FirstHeal_{Item.InstanceID}_Stack", heal);
        }
        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            heal = saveData.GetBool($"CharmSaveData_FirstHeal_{Item.InstanceID}_Stack", false);
        }
    }
}
