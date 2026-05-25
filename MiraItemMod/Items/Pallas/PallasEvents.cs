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
                //
                var hasJoker = HasJoker(avatar, __instance);
                if (!hasJoker)
                    return;

                float[] throwChance = new float[] { 0f, 5f, 10f, 15f, 20f };
                float defaultChance = 100f;
                string value = showAllLevel ? (throwChance.SafeRandomAccess(0) * num).ToString("0.#") + "→" + (throwChance.SafeRandomAccess(__instance.maxLevel) * num).ToString("0.#") + "%" : (throwChance.SafeRandomAccess(__instance.LevelToIdx(level)) * num).ToString("0.#") + "%";
                float num2 = defaultChance * num;

                if (!ignoreAvatarStatus)
                {
                    float num3 = num2 + throwChance.SafeRandomAccess(__instance.LevelToIdx(level)) * Mathf.Clamp(avatar.GetCustomStat(ECustomStat.Luck), 0, 9999) * num;
                    __result = new Loc.KeywordValue[4]
                        {
                                        new Loc.KeywordValue("DEFAULT", num2.ToString("0.#")),
                                    new Loc.KeywordValue("CHANCE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                                    new Loc.KeywordValue("DAMAGE", "30"),
                                    new Loc.KeywordValue("CURRENT", num3.ToString("0.#") + "%")
                        };
                }
                else
                {
                    __result = new Loc.KeywordValue[3]
                        {
                                        new Loc.KeywordValue("DEFAULT", num2.ToString("0.#")),
                                    new Loc.KeywordValue("CHANCE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
                                    new Loc.KeywordValue("DAMAGE", "30")
                        };
                }
                return;
            }
            static bool HasJoker(UnitAvatar avatar, Charm_Basic __instance)
            {
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
        }
    }
}
