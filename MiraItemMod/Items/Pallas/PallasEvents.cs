using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Pallas
{
    public static class PallasEvents
    {
        public static event Action<Charm_PallasCard, int> OnPallasCardSpawn;
        public static event Action<Charm_PallasAce, int> OnPallasAceSpawn;

        public static void SetEnhancement(this Charm_PallasCard pallas, bool enhance)
        {
            if (enhance)
            {
                pallas.bulletDamage = 30;
                pallas.defaultChance = 100;
                pallas.throwChanceByLevel = new float[] { 0f, 5f, 10f, 15f, 20f };
                //pallas.throwChanceByLevel = new float[] {0f, 4f, 8f, 12f, 16f};
                pallas.throwIntervalTimer.time = 0.05f;
            }
            else
            {
                pallas.bulletDamage = 24;
                pallas.defaultChance = 50;
                pallas.throwChanceByLevel = new float[] { 0f, 2.5f, 5f, 7.5f, 10f };
                pallas.throwIntervalTimer.time = 0.1f;
            }
        }
        #region パラスのジョーカー
        [HarmonyPatch(typeof(Charm_PallasCard), "OnBeginAttackAnimation")]
        public static class Charm_PallasCardOnBeginAttackAnimationPatch
        {
            static void Prefix(int idx, Charm_PallasCard __instance)
            {
                if (!__instance.WeaponController || !__instance.WeaponController.currentWeapon || !__instance.throwIntervalTimer.Check())
                {
                    return;
                }
                OnPallasCardSpawn?.Invoke(__instance, idx);
            }
        }
        public static void InvokeOnPallasAceSpawn(int idx, Charm_PallasAce __instance)
        {
            OnPallasAceSpawn?.Invoke(__instance, idx);
        }
        #endregion


        [HarmonyPatch(typeof(Charm_PallasCard), nameof(Charm_PallasCard.BuildKeywords), new Type[] { typeof(UnitAvatar), typeof(int), typeof(int), typeof(bool), typeof(bool) })]
        public static class KeywordsPatch
        {
            static void Postfix(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus, Charm_PallasCard __instance, ref Loc.KeywordValue[] __result)
            {
                if (avatar == null)
                    return;

                float num = 1f;
                if ((bool)avatar && avatar.TryGetComponent<WeaponControllerSimple>(out var component) && (bool)component.currentWeapon)
                {
                    num = component.currentWeapon.AttackWeightPerSwing;
                }
                
                var hasJoker = HasJoker(avatar, __instance);
                var hasDiamond = HasDiamond(avatar, __instance);
                var hasSpade = HasSpade(avatar, __instance);

                var parameters = CustomPallasController.GetCardParameters(hasDiamond, hasSpade, hasJoker);
                string value = showAllLevel ? (parameters.throwChanceByLevel.SafeRandomAccess(0) * num).ToString("0.#") + "→" + (parameters.throwChanceByLevel.SafeRandomAccess(__instance.maxLevel) * num).ToString("0.#") + "%" : (parameters.throwChanceByLevel.SafeRandomAccess(__instance.LevelToIdx(level)) * num).ToString("0.#") + "%";
                float num2 = parameters.defaultChance * num;

                if (!ignoreAvatarStatus)
                {
                    float num3 = num2 + parameters.throwChanceByLevel.SafeRandomAccess(__instance.LevelToIdx(level)) * Mathf.Clamp(avatar.GetCustomStat(ECustomStat.Luck), 0, 9999) * num;
                    __result = new Loc.KeywordValue[]
                        {
                            new Loc.KeywordValue("DEFAULT", num2.ToString("0.#")),
                            new Loc.KeywordValue("CHANCE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                            new Loc.KeywordValue("DAMAGE", parameters.bulletDamage.ToString()),
                            new Loc.KeywordValue("CURRENT", num3.ToString("0.#") + "%")
                        };
                }
                else
                {
                    __result = new Loc.KeywordValue[]
                        {
                            new Loc.KeywordValue("DEFAULT", num2.ToString("0.#")),
                            new Loc.KeywordValue("CHANCE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                            new Loc.KeywordValue("DAMAGE", parameters.bulletDamage.ToString()),
                            new Loc.KeywordValue("CURRENT", "-")
                        };
                }
                return;
            }
        }


        public static bool HasJoker(UnitAvatar avatar, Charm_Basic __instance)
        {
            if (avatar == null || avatar.Inventory == null)
                return false;
            ItemPosition[] array = Charm_PallasJoker.Directions;
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = avatar.Inventory.FindItem(new ItemPosition(__instance.xIdx, __instance.yIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is Charm_PallasJoker)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool HasSpade(UnitAvatar avatar, Charm_Basic __instance)
        {
            if (avatar == null || avatar.Inventory == null)
                return false;
            ItemPosition[] array = Charm_PallasSpade.Directions;
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = avatar.Inventory.FindItem(new ItemPosition(__instance.xIdx, __instance.yIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is Charm_PallasSpade)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool HasDiamond(UnitAvatar avatar, Charm_Basic __instance)
        {
            if (avatar == null || avatar.Inventory == null)
                return false;
            ItemPosition[] array = Charm_PallasDiamond.Directions;
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = avatar.Inventory.FindItem(new ItemPosition(__instance.xIdx, __instance.yIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is Charm_PallasDiamond)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool HasClub(UnitAvatar avatar, Charm_Basic __instance)
        {
            if (avatar == null || avatar.Inventory == null)
                return false;
            ItemPosition[] array = Charm_PallasClub.Directions;
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = avatar.Inventory.FindItem(new ItemPosition(__instance.xIdx, __instance.yIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is Charm_PallasClub)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool HasHeart(UnitAvatar avatar, Charm_Basic __instance)
        {
            if (avatar == null || avatar.Inventory == null)
                return false;
            ItemPosition[] array = Charm_PallasHeart.Directions;
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = avatar.Inventory.FindItem(new ItemPosition(__instance.xIdx, __instance.yIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is Charm_PallasHeart)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
