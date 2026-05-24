using HarmonyLib;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static SephiriaMod.Items.Charm_EvasionFrost.ChargingCharmPatch;

namespace SephiriaMod.Items
{
    public class Charm_EvasionFrost : Charm_StatusInstance
    {
        public static List<int> FrostRelics = new List<int> { 1014, 1137, 1208, 1247 };//祝詞の鞘、ヴォルスパ、吹雪のハンマー、氷の翼
        public int[] cooldown = new int[] { 50 };
        public Timer cooldownTimer = new Timer(0.1f, resetOnTime: false);
        public int[] percent = new int[] { 10, 20, 30 };
        public Sprite iconSprite;
        public void Start()
        {
            iconSprite = AssetLoader.LoadSprite(ModUtil.MiscPath + "Frost_Charm");
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? (percent.SafeRandomAccess(0) / 100f).ToString("0.##") + "→" + (percent.SafeRandomAccess(maxLevel) / 100f).ToString("0.##") : (percent.SafeRandomAccess(LevelToIdx(level)) / 100f).ToString("0.##");
            if(avatar != null && !ignoreAvatarStatus)
            {
                string value2 = showAllLevel ? GetChargeChance(avatar, percent.SafeRandomAccess(0)).ToString("0.##") + "→" + GetChargeChance(avatar, percent.SafeRandomAccess(maxLevel)).ToString("0.##") : GetChargeChance(avatar, percent.SafeRandomAccess(LevelToIdx(level))).ToString("0.##");
                return new Loc.KeywordValue[2]
                {
                new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("CURRENT", value2 + "%", GetPositiveColor(virtualLevelOffset))
                };
            }
            return new Loc.KeywordValue[2]
            {
                new Loc.KeywordValue("PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
                new Loc.KeywordValue("CURRENT", "-%")
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
            NetworkAvatar.AddCustomStatUnsafe("EVASIONCHARGE", percent.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.OnEvade += OnEvade;
        }

        private void OnEvade(DamageInstance damage)
        {
            if (!cooldownTimer.Check())
                return;
            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition((sbyte)(Item.XIdx + 1), Item.YIdx));

            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled)
                return;

            var charm = newItemOwnInstance.Charm;
            if(charm is Charm_IceHammer hammer)
            {
                cooldownTimer.Ratio = 0f;
                hammer.chargingCharm.AddTimer(cooldown.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
            }
            if (charm is Charm_IceSpear spear)
            {
                cooldownTimer.Ratio = 0f;
                spear.chargingCharm.AddTimer(cooldown.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
            }
            if (charm is Charm_AirSlash slash)
            {
                cooldownTimer.Ratio = 0f;
                slash.chargingCharm.AddTimer(cooldown.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
            }
            if (charm is Charm_IceBow bow)
            {
                cooldownTimer.Ratio = 0f;
                bow.chargingCharm.AddTimer(cooldown.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
                if(bow.NetworkreadyArrowCount < bow.arrowReloadLimit)
                    bow.NetworkreadyArrowCount = bow.readyArrowCount + cooldown.SafeRandomAccess(CurrentLevelToIdx()) * 2 / 100;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("EVASIONCHARGE", -percent.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.OnEvade -= OnEvade;
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStatUnsafe("EVASIONCHARGE", -percent.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStatUnsafe("EVASIONCHARGE", percent.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            Inventory.UpdatePing(Item.Position);
        }
        public override CharmConnectionData[] GetConnectedCharmPositions()
        {
            if (!IsEffectEnabled)
            {
                return null;
            }

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition((sbyte)(Item.XIdx + 1), Item.YIdx));
            if (newItemOwnInstance == null || !newItemOwnInstance.Charm || !newItemOwnInstance.Charm.IsEffectEnabled)
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(iconSprite, new Vector2Int(Item.XIdx + 1, Item.YIdx))
                };
            }
            if(!FrostRelics.Contains(newItemOwnInstance.EntityID))
            {
                return new CharmConnectionData[1]
                {
                new CharmConnectionData(iconSprite, new Vector2Int(Item.XIdx + 1, Item.YIdx))
                };
            }

            return null;
        }

        public override int GetSubIconCount()
        {
            return 1;
        }

        public override Sprite GetSubIconImage(ItemPosition pos, bool isInstance, int idx)
        {
            if (!isInstance)
            {
                return null;
            }

            if (!NetworkAvatar)
            {
                return null;
            }

            NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(pos.x + 1, pos.y));
            if (newItemOwnInstance != null && (bool)newItemOwnInstance.Charm && newItemOwnInstance.Charm.IsEffectEnabled)
            {
                if (FrostRelics.Contains(newItemOwnInstance.EntityID))//祝詞の鞘、ヴォルスパ、吹雪のハンマー、氷の翼
                    return newItemOwnInstance.Entity.Icon;
            }

            return null;
        }

        public override Vector2 GetSubIconImageOffset(int index)
        {
            return new Vector2(4f, -4f);
        }
        [HarmonyPatch(typeof(ChargingCharm), nameof(ChargingCharm.ActivateChargingCharm), new Type[] { typeof(float), typeof(bool), typeof(string) })]
        public static class ChargingCharmPatch
        {
            public static float GetChargeChance(UnitAvatar avatar)
            {
                return Mathf.FloorToInt(avatar.GetCustomStat(ECustomStat.Evasion) / 100f) * (avatar.GetCustomStatUnsafe("EVASIONCHARGE") / 100f);
            }
            public static float GetChargeChance(UnitAvatar avatar, int percent)
            {
                return Mathf.FloorToInt(avatar.GetCustomStat(ECustomStat.Evasion) / 100f) * (percent / 100f);
            }
            static bool Prefix(float damageMultiplier, bool canRetrigger, string parameter, ChargingCharm __instance)
            {
                var avatar = __instance.GetAvatar();
                if(avatar != null && avatar.GetCustomStatUnsafe("EVASIONCHARGE") > 0)
                {
                    float percent = GetChargeChance(avatar);
                    if (UnityEngine.Random.Range(0f, 100f) < percent)
                    {
                        __instance.ActivateChargingCharmNoCount(damageMultiplier, canRetrigger, parameter);
                        return false;
                    }
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(Charm_IceBow), "Fire", new Type[] { typeof(int), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(string) })]
        public static class IceBowPatch
        {
            static void Prefix(Charm_IceBow __instance)
            {
                var avatar = __instance.NetworkAvatar;
                if (avatar != null && avatar.GetCustomStatUnsafe("EVASIONCHARGE") > 0)
                {
                    var count = __instance.readyArrowCount;
                    for (int q = 0; q < count; q++)
                    {
                        float percent = GetChargeChance(avatar);
                        if (UnityEngine.Random.Range(0f, 100f) < percent)
                        {
                            __instance.NetworkreadyArrowCount = __instance.readyArrowCount + 1;
                        }
                    }
                }
            }
        }
    }
}
