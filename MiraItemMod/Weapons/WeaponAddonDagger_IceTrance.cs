using HarmonyLib;
using MiraItemMod.Items;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonDagger_IceTrance : WeaponAddon
    {
        public int previousFury = 0;
        public bool isInIce = false;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            if(parent is WeaponSimple_Dagger dagger)
            {
                if(dagger.currentFury > 0)
                {
                    dagger.NetworkcurrentFury = 0;
                    parent.Networkowner.unitAvatar.DestroyEffectHUD("DaggerImmersion");
                }
                dagger.basicAttackFinal = false;
            }
            previousFury = 0;
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe("IceTrance".ToUpperInvariant(), 1);
        }
        private void Update()
        {
            if (!isEnabled)
                return;
            if (!base.isServer)
                return;
            if (parent is WeaponSimple_Dagger dagger)
            {
                if (dagger.currentFury > previousFury && previousFury == 0 && !isInIce)
                {
                    dagger.Networkowner.unitAvatar.AddCustomStatUnsafe("CHARGINGCHARMAMPLIFY", 1);
                    isInIce = true;
                }
                else if (dagger.currentFury == 0 && previousFury > 0 && isInIce)
                {
                    dagger.Networkowner.unitAvatar.AddCustomStatUnsafe("CHARGINGCHARMAMPLIFY", -1);
                    isInIce = false;
                }
                previousFury = dagger.currentFury;
            }
        }
        public override void OnParry(DamageInstance damage)
        {
            base.OnParry(damage);
            if (parent is WeaponSimple_Dagger dagger)
            {
                if(dagger.currentFury > 0)
                {
                    //氷の遺物加速
                    foreach(var charm in parent.Networkowner.unitAvatar.Inventory.charms.Values)
                    {
                        AddTimer(charm, 0.5f);
                    }
                }
            }
        }
        private void AddTimer(Charm_Basic charm, float t)
        {
            if (charm is Charm_IceHammer hammer)
            {
                hammer.chargingCharm.AddTimer(t);
            }
            else if (charm is Charm_IceSpear spear)
            {
                spear.chargingCharm.AddTimer(t);
            }
            else if (charm is Charm_AirSlash slash)
            {
                slash.chargingCharm.AddTimer(t);
            }
            else if (charm is Charm_IceBow bow)
            {
                bow.chargingCharm.AddTimer(t);
                bow.NetworkreadyArrowCount = Mathf.Min(bow.readyArrowCount + (int)(t / 0.5f), bow.arrowReloadLimit);
            }
        }
        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            if (parent is WeaponSimple_Dagger dagger)
            {
                if(dagger.currentFury > 0)
                {
                    dagger.NetworkcurrentFury = 0;
                    parent.Networkowner.unitAvatar.DestroyEffectHUD("DaggerImmersion");
                }
                dagger.basicAttackFinal = false;
            }
            previousFury = 0;
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe("IceTrance".ToUpperInvariant(), -1);

            if (isInIce)
            {
                parent.Networkowner.unitAvatar.AddCustomStatUnsafe("CHARGINGCHARMAMPLIFY", -1);
                isInIce = false;
            }
        }

        [HarmonyPatch]
        public static class HUDPatch
        {
            [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.CreateEffectHUD), new Type[] { typeof(string), typeof(string) })]
            [HarmonyPrefix]
            private static void CreatePrefix(UnitAvatar __instance, ref string entityID, ref string effectName)
            {
                if (__instance.GetCustomStatUnsafe("IceTrance".ToUpperInvariant()) <= 0)
                    return;
                if (entityID == "IMMERSION")
                {
                    entityID = "IMMERSIONICE";
                }
                if (effectName == "DaggerImmersion")
                {
                    effectName = "DaggerImmersionIce";
                }
            }
            [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.SetEffectHUDFillAmount), new Type[] { typeof(string), typeof(float) })]
            [HarmonyPrefix]
            private static void SetFillAmountPrefix(UnitAvatar __instance, ref string effectName, float fillAmount)
            {
                if (__instance.GetCustomStatUnsafe("IceTrance".ToUpperInvariant()) <= 0)
                    return;
                if (effectName == "DaggerImmersion")
                {
                    effectName = "DaggerImmersionIce";
                }
            }
            [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.SetEffectHUDValue), new Type[] { typeof(string), typeof(string) })]
            [HarmonyPrefix]
            private static void SetValuePrefix(UnitAvatar __instance, ref string effectName, string value)
            {
                if (__instance.GetCustomStatUnsafe("IceTrance".ToUpperInvariant()) <= 0)
                    return;
                if (effectName == "DaggerImmersion")
                {
                    effectName = "DaggerImmersionIce";
                }
            }
            [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.SetEffectHUDFlash), new Type[] { typeof(string) })]
            [HarmonyPrefix]
            private static void SetFlashPrefix(UnitAvatar __instance, ref string effectName)
            {
                if (__instance.GetCustomStatUnsafe("IceTrance".ToUpperInvariant()) <= 0)
                    return;
                if (effectName == "DaggerImmersion")
                {
                    effectName = "DaggerImmersionIce";
                }
            }
            [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.DestroyEffectHUD), new Type[] { typeof(string) })]
            [HarmonyPrefix]
            private static void DestroyPrefix(UnitAvatar __instance, ref string effectName)
            {
                if (__instance.GetCustomStatUnsafe("IceTrance".ToUpperInvariant()) <= 0)
                    return;
                if (effectName == "DaggerImmersion")
                {
                    effectName = "DaggerImmersionIce";
                }
            }
        }
        [HarmonyPatch]
        public static class DaggerPatch
        {
            [HarmonyPatch(typeof(WeaponSimple_Dagger), nameof(WeaponSimple_Dagger.SubAttackButtonDown))]
            [HarmonyPrefix]
            static void FuryPrefix(WeaponSimple_Dagger __instance)
            {
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe("IceTrance".ToUpperInvariant()) > 0)
                {
                    __instance.basicAttackFinal = true;
                }
                else if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe("NoFury".ToUpperInvariant()) > 0)
                {
                    __instance.basicAttackFinal = true;
                }
            }
            [HarmonyPatch(typeof(WeaponSimple_Dagger), nameof(WeaponSimple_Dagger.SubAttackButtonDown))]
            [HarmonyPostfix]
            static void FuryPostfix(WeaponSimple_Dagger __instance)
            {
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe("IceTrance".ToUpperInvariant()) > 0)
                {
                    __instance.basicAttackFinal = false;
                }
                else if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe("NoFury".ToUpperInvariant()) > 0)
                {
                    __instance.basicAttackFinal = false;
                }
            }
            [HarmonyPatch(typeof(WeaponSimple_Dagger), "HandleParry")]
            [HarmonyPrefix]
            static void ParryPrefix(WeaponSimple_Dagger __instance)
            {
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe("NoFury".ToUpperInvariant()) > 0)
                {
                    __instance.basicAttackFinal = true;
                }
            }
            [HarmonyPatch(typeof(WeaponSimple_Dagger), "HandleParry")]
            [HarmonyPostfix]
            static void ParryPostfix(WeaponSimple_Dagger __instance)
            {
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe("NoFury".ToUpperInvariant()) > 0)
                {
                    __instance.basicAttackFinal = false;
                }
            }
        }
    }
}
