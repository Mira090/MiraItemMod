using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_ShadowFrostbite : Charm_StatusInstance
    {
        public static readonly string ShadowFrostbite = "ShadowFrostbite".ToUpperInvariant();
        public static readonly string AbilityDamageId = "Debuff_BlackFreeze";
        public static readonly int AbsoluteEvasionPercent = 4;
        public static readonly float BlackFreezeMultiplier = 2.5f;

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(ShadowFrostbite, 1);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(ShadowFrostbite, -1);
        }

        [HarmonyPatch(typeof(UnitAvatar), nameof(UnitAvatar.ApplyDamage))]
        public static class ApplyDamagePatch
        {
            static void Prefix(UnitAvatar __instance, ref DamageInstance damage)
            {
                if (damage.origin is UnitAvatar attacker)
                {
                    foreach (var debuff in attacker.Debuffs)
                    {
                        if (debuff is CharacterDebuff_Frostbite frostbite && frostbite.NetworkAttacker.GetCustomStatUnsafe(ShadowFrostbite) > 0)
                        {
                            damage.absoluteEvasionPercent += AbsoluteEvasionPercent * frostbite.CurrentStack;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(CharacterDebuff), "CreateLoopFx")]
        public static class CreateLoopFxPatch
        {
            static void Postfix(CharacterDebuff __instance)
            {
                if (__instance is CharacterDebuff_Frostbite frostbite && frostbite.NetworkAttacker.GetCustomStatUnsafe(ShadowFrostbite) > 0)
                {
                    var particle = __instance.GetLoopElementalParticle();
                    if(particle != null)
                    {
                        var main = particle.particle.main;
                        main.startColor = new Color(0.1f, 0.1f, 0.1f);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(CharacterDebuff_Freeze), nameof(CharacterDebuff_Freeze.OnStartClient))]
        public static class OnStartClientPatch
        {
            static void Postfix(CharacterDebuff_Freeze __instance)
            {
                if (__instance.NetworkAttacker.GetCustomStatUnsafe(ShadowFrostbite) <= 0)
                    return;
                var freeze = __instance.GetFreezeFx();
                if (freeze == null)
                    return;

                if (freeze.TryGetComponent(out SpriteRenderer renderer))
                {
                    renderer.color = new Color(0.1f, 0.1f, 0.1f, renderer.color.a);
                }
                foreach (var fragment in freeze.fragments)
                {
                    if (fragment.transform.childCount > 0 && fragment.transform.GetChild(0).TryGetComponent(out SpriteRenderer fragmentRenderer))
                    {
                        fragmentRenderer.color = new Color(0.1f, 0.1f, 0.1f, fragmentRenderer.color.a);
                    }
                }
            }
        }
        [HarmonyPatch(typeof(CharacterDebuff_Freeze), "ApplyStatusInner")]
        public static class ApplyStatusInnerPatch
        {
            static void Prefix(CharacterDebuff_Freeze __instance)
            {
                if (__instance.NetworkAttacker.GetCustomStatUnsafe(ShadowFrostbite) <= 0)
                    return;
                if (__instance.NetworkTarget == null || __instance.NetworkTarget.IsDead)
                    return;


                float num = ((float)__instance.NetworkAttacker.GetCustomStat(ECustomStat.Evasion) / 100f) * BlackFreezeMultiplier * (float)__instance.CurrentStack;
                int customStatUnsafe = __instance.NetworkAttacker.GetCustomStatUnsafe("FREEZEDAMAGE");
                num += num * (float)customStatUnsafe / 100f;
                DamageInstance damage = DamageInstance.GetDamage(__instance.NetworkAttacker, AbilityDamageId, __instance.NetworkTarget.transform.position, 4294967295L, num, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                damage.SetCustomColor(true, new Color(0.3f, 0.3f, 0.3f));
                __instance.NetworkTarget.ApplyDamage(damage);


                if (__instance.NetworkTarget.isInStun || __instance.NetworkTarget.stunCooldown || __instance.NetworkTarget.stunImmunity > 0)
                {
                    return;
                }

                if (__instance.NetworkTarget.monsterType == EMonsterType.Boss || __instance.NetworkTarget.monsterType == EMonsterType.Miniboss)
                {
                    var timer = __instance.NetworkTarget.GetStunCooldownTimer();
                    timer.time -= 8f;
                }
            }
        }
    }
}
