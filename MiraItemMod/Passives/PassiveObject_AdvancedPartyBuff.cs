using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MiraItemMod.Passives
{
    public class PassiveObject_AdvancedPartyBuff : PassiveObject
    {
        public static readonly string Stat = "AdvancedPartyBuff".ToSephiriaUpperId();
        protected override void OnEffectEnabled(PlayerAvatar player, bool runtime)
        {
            base.OnEffectEnabled(player, runtime);
            player.AddCustomStatUnsafe(Stat, 1);
        }
        protected override void OnEffectDisabled()
        {
            base.OnEffectDisabled();
            player.AddCustomStatUnsafe(Stat, -1);
        }

        [HarmonyPatch]
        public static class Patch
        {
            static MethodBase TargetMethod()
            {
                return typeof(UnitAvatar).GetMethod(nameof(UnitAvatar.ApplyBuff))?.MakeGenericMethod(typeof(CharacterBuff));
            }

            static void Prefix(UnitAvatar __instance, CharacterBuff buffPrefab, float amplified, UnitAvatar caster, bool hudFlash)
            {
                if (caster != null && __instance != caster)
                    return;
                Core.LoggerMedium("ApplyBuff: " + buffPrefab.GetType());
                if (__instance.GetCustomStatUnsafe(Stat) > 0)
                {
                    foreach (PlayerSpawner multiplayer in PlayerSpawner.MultiplayerList)
                    {
                        if (!multiplayer || !multiplayer.PlayerAvatar)
                            return;
                        if (multiplayer.PlayerAvatar == __instance)
                            return;
                        if ((multiplayer.transform.position - __instance.transform.position).magnitude < 10f)
                        {
                            multiplayer.PlayerAvatar.ApplyBuff(buffPrefab, amplified, __instance, hudFlash);
                        }
                    }
                }
            }
        }
    }
}
