using HarmonyLib;
using MiraItemMod.Registries;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonKatana_Plasma : WeaponAddon
    {
        public static Dictionary<int, NewWeaponFireData> PlasmaAttacks = new Dictionary<int, NewWeaponFireData>();
        public int percent = 70;
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("PERCENT", percent.ToString())
            };
        }
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit += OnAttackUnit;
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack)
                return;
            if (parent.Networkowner.unitAvatar.GetCustomStatUnsafe("PlasmaKatana".ToUpperInvariant()) <= 0)
                return;
            if (percent.Percent())
            {
                if(damage.IsSameElementalType(EDamageElementalType.FireAndLightning))
                {
                    parent.Networkowner.unitAvatar.AddCustomStatUnsafe("PLASMAACTIVE", 1);
                    avatar.ApplyDebuff(50.Percent() ? SephiriaPrefabs.Burn : SephiriaPrefabs.Electric, parent.Networkowner.unitAvatar);
                    parent.Networkowner.unitAvatar.AddCustomStatUnsafe("PLASMAACTIVE", -1);
                }
                else
                {
                    avatar.ApplyDebuff(SephiriaPrefabs.Burn, parent.Networkowner.unitAvatar);
                }
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
        }
        [HarmonyPatch(typeof(WeaponSimple_Katana), nameof(WeaponSimple_Katana.GetBasicAttack))]
        public static class GetBasicAttackPatch
        {
            static void Postfix(WeaponSimple_Katana __instance, ref NewWeaponFireData __result, int idx, string parameter)
            {
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe("PlasmaKatana".ToUpperInvariant()) <= 0)
                    return;
                foreach (var addon in __instance.addons)
                {
                    if(addon is WeaponAddonKatana_Plasma plasma)
                    {
                        if (PlasmaAttacks.ContainsKey(idx))
                        {
                            __result = PlasmaAttacks[idx];
                            return;
                        }
                        else if(__result is NewWeaponFireData_MeleeAttack melee)
                        {
                            var attack = ModWeapon.CopyNewWeaponFireData(melee);
                            attack.SetRelatedStatFormula(Events.PlasmaRelatedStatFormula);
                            attack.swingFxPrefab = Data.KatanaPlasmaAttack.SafeRandomAccess(idx).ResourcePrefab;
                            PlasmaAttacks[idx] = attack;
                            __result = attack;
                            return;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_Katana), "Update")]
        public static class Patch2
        {
            static void Postfix(WeaponSimple_Katana __instance)
            {
                if (!__instance.isServer)
                    return;
                foreach (var addon in __instance.addons)
                {
                    if (addon is WeaponAddonKatana_Plasma plasma)
                    {
                        if(__instance.GetCurrentKatanaGauge() >= 1f)
                        {
                            __instance.SetCurrentKatanaGauge(0f);
                            __instance.Networkowner.unitAvatar.ApplyBuff(Data.PlasmaKatanaBuff, 1, __instance.Networkowner.unitAvatar, true);
                        }
                        return;
                    }
                }
            }
        }
    }
}
