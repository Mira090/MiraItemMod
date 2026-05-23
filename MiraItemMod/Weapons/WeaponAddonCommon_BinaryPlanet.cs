using HarmonyLib;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonCommon_BinaryPlanet : WeaponAddon
    {
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe("BINARYPLANET", 1);
            parent.Networkowner.unitAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if (ModUtil.PlanetDamageIds.Contains(damage.id))
            {
                damage.criticalChancePercent = -1000;
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.AddCustomStatUnsafe("BINARYPLANET", -1);
            parent.Networkowner.unitAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }
        [HarmonyPatch(typeof(GreenBat), nameof(GreenBat.Fire), new Type[] { typeof(int) })]
        public class PlanetFirePatch
        {
            static void Prefix(ref int amplify, GreenBat __instance)
            {
                var charm = __instance.GetCharm();
                if (charm == null || charm.NetworkAvatar == null || charm.NetworkAvatar.IsDead || charm.NetworkAvatar.GetCustomStatUnsafe("BINARYPLANET") <= 0)
                    return;

                var percent = charm.NetworkAvatar.GetCustomStat(ECustomStat.Critical) / 100f;
                if (UnityEngine.Random.Range(0f, 1f) > percent / 100f)
                    return;

                amplify = 3;
            }
        }
    }
}
