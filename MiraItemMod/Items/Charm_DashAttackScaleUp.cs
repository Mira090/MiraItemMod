using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace MiraItemMod.Items
{
    public class Charm_DashAttackScaleUp : Charm_StatusInstance
    {
        public int[] rangeByLevel = new int[] { 30, 40, 50, 60, 70 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? rangeByLevel.SafeRandomAccess(0) + "→" + rangeByLevel.SafeRandomAccess(maxLevel) : rangeByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("RANGE", "+" + value + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStat("DashAttackRange", rangeByLevel.SafeRandomAccess(CurrentLevelToIdx()));
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStat("DashAttackRange", -rangeByLevel.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.AddCustomStat("DashAttackRange", -rangeByLevel.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.AddCustomStat("DashAttackRange", rangeByLevel.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        [HarmonyPatch]
        public static class DashAttackRangePatch
        {
            [HarmonyPatch(typeof(NewWeaponFireData), nameof(NewWeaponFireData.CreateAttack), new Type[]{typeof(EDamageFromType), typeof(float), typeof(string),
                typeof(bool), typeof(UnitAvatar), typeof(Vector2), typeof(Vector2), typeof(float), typeof(Action<int, ProjectileBase>), typeof(List<CombatBehaviour>),
                typeof(float), typeof(Action<CombatBehaviour, DamageInstance, ProjectileBase>), typeof(bool), typeof(float), typeof(float), typeof(int), typeof(EDamageElementalType?) })]
            [HarmonyPrefix]
            static void PrefixNormal(string damageId, UnitAvatar owner, ref float rangeBonus)
            {
                if (damageId != "Weapon_DashAttack")
                    return;
                rangeBonus += owner.GetCustomStat("DashAttackRange") / 100f;
            }
            [HarmonyPatch(typeof(NewWeaponFireData), nameof(NewWeaponFireData.CreateAttack), new Type[]{typeof(EDamageFromType), typeof(float), typeof(string),
                typeof(bool), typeof(UnitAvatar), typeof(Vector2), typeof(Vector2), typeof(float), typeof(Action<int, ProjectileBase>), typeof(List<CombatBehaviour>),
                typeof(float), typeof(Action<CombatBehaviour, DamageInstance, ProjectileBase>), typeof(bool), typeof(float), typeof(float), typeof(int), typeof(EDamageElementalType?)})]
            [HarmonyPrefix]
            static void PrefixOverrideElementalDamage(string damageId, UnitAvatar owner, ref float rangeBonus)
            {
                if (damageId != "Weapon_DashAttack")
                    return;
                rangeBonus += owner.GetCustomStat("DashAttackRange") / 100f;
            }
            [HarmonyPatch(typeof(WeaponSimple), nameof(WeaponSimple.CreateDashSwingFx), new Type[] { typeof(int), typeof(Vector3), typeof(Vector3) })]
            [HarmonyPrefix]
            static bool PrefixCreateFx(int idx, Vector3 position, Vector3 eulerAngles, WeaponSimple __instance)
            {
                if (!NetworkClient.active)
                {
                    Debug.LogWarning("[Client] function 'System.Void WeaponSimple::CreateDashSwingFx(System.Int32,UnityEngine.Vector3,UnityEngine.Vector3)' called when client was not active");
                    return false;
                }
                float fxScale = 1f + (float)__instance.Networkowner.unitAvatar.GetCustomStat(ECustomStat.WeaponRange) / 100f + __instance.Networkowner.unitAvatar.GetCustomStat("DashAttackRange") / 100f;
                NewWeaponFireData dashAttack = __instance.GetDashAttack(idx);
                bool flag = false;
                foreach (PlayerSpawner playerSpawner in PlayerSpawner.MultiplayerList)
                {
                    if (playerSpawner && __instance.Networkowner.gameObject == playerSpawner.gameObject)
                    {
                        flag = true;
                        break;
                    }
                }
                bool canBeTransparentOnMultiplayer = false;
                if (flag)
                {
                    canBeTransparentOnMultiplayer = true;
                }
                position += new Vector3(0f, __instance.Networkowner.shoulder.Position.y, 0f);
                dashAttack.CreateSwingFx(canBeTransparentOnMultiplayer, __instance.Networkowner.transform, position, eulerAngles, fxScale, flag ? (__instance.Networkowner.isOwned ? 1 : 0) : -1, 0f);
                return false;
            }
        }
    }
}
