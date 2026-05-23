using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items.Savvy
{
    public class Charm_SavvyShadow : Charm_StatusInstance
    {
        public int count = 0;
        public int countRequire = 50;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("COUNT", countRequire.ToString())
            };
        }
        public void Loot(UnitAvatar avatar)
        {
            count++;
            if (count % countRequire == 0)
            {
                this.AddRandomJewelry(true);
            }
            else
            {
                var evasion = ModUtil.GetEvasionPercent(NetworkAvatar.GetCustomStat(ECustomStat.Evasion));
                if ((evasion / 2).Percent())
                {
                    if ((evasion / 4).Percent())
                    {
                        if ((evasion / 8).Percent())
                        {
                            SephiriaPrefabs.SpawnMoney(100, avatar.transform.position);
                        }
                        else
                        {
                            SephiriaPrefabs.SpawnMoney(50, avatar.transform.position);
                        }
                    }
                    else
                    {
                        SephiriaPrefabs.SpawnMoney(20, avatar.transform.position);
                    }
                }
                else
                {
                    SephiriaPrefabs.SpawnMoney(10, avatar.transform.position);
                }
            }
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count % countRequire}/{countRequire}");
            NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
        }

        private void Awake()
        {
            effectHUD_ID = "Savvy_Shadow".ToSephiriaUpperId();
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count % countRequire}/{countRequire}");
            NetworkAvatar.OnEvade += OnEvade;
        }

        private void OnEvade(DamageInstance damage)
        {
            if (damage.origin is UnitAvatar avatar)
                Loot(avatar);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnEvade -= OnEvade;
        }

        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_SavvyShadow_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_SavvyShadow_{Item.InstanceID}_Stack", 0);
        }
    }
}
