using HarmonyLib;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonKatana_FrostFlameSword : WeaponAddonCommon_AdditionalElementalDamage
    {
        public float restoreFlameSwordTime = 2f;
        public float percent = 3f;

        private float restoreTimer;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            //parent.Networkowner.unitAvatar.AddCustomStatUnsafe("FrostFlameSword".ToSephiriaUpperId(), 1);
            //parent.Networkowner.unitAvatar.AddCustomStatUnsafe("FLAMESWORDCALLBACKFROST", 1);
        }
        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            //parent.Networkowner.unitAvatar.AddCustomStatUnsafe("FrostFlameSword".ToSephiriaUpperId(), -1);
            //parent.Networkowner.unitAvatar.AddCustomStatUnsafe("FLAMESWORDCALLBACKFROST", -1);
        }
        /* v0.10.6より前
        [HarmonyPatch(typeof(ComboEffect_FlameSword), "HandleAttackUnit", [typeof(UnitAvatar), typeof(DamageInstance)])]
        public class FlameSwordPatch
        {
            static List<string> FrostRelics = ["Charm_IceSpear", "Charm_IceHammer", "Charm_IceBow", "Charm_AirSlash"];
            static void Postfix(UnitAvatar avatar, DamageInstance damage, ComboEffect_FlameSword __instance)
            {
                if (avatar && !avatar.IsDead)
                {
                    if (__instance.Networkavatar.GetCustomStatUnsafe("FrostFlameSword".ToSephiriaUpperId()) <= 0)
                        return;
                    if (__instance.GetIsCooldown())
                    {
                        return;
                    }
                    if (FrostRelics.Contains(damage.id))
                    {
                        __instance.SetIsCooldown(true);
                        __instance.SetCooldownTimer(0.1f);
                        __instance.InvokeLocalFireSword(avatar.transform.position);
                    }
                }
            }
        }*/

        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new List<Loc.KeywordValue>(base.BuildKeywords())
            {
                new Loc.KeywordValue("VAL0", restoreFlameSwordTime.ToString()),
                new Loc.KeywordValue("VAL1", percent + "%")
            }.ToArray();
        }


        private void Update()
        {
            if (parent == null || parent.Networkowner == null)
                return;
            var NetworkAvatar = parent.Networkowner.unitAvatar;
            if (!NetworkAvatar || NetworkAvatar.IsDead || !NetworkAvatar.Inventory)
            {
                return;
            }

            var amp = NetworkAvatar.GetCustomStatUnsafe("CHARGINGCHARMAMPLIFY");
            if (amp < 0)
                amp = 0;
            var charge = NetworkAvatar.GetCustomStatUnsafe("CHARGINGCHARMBONUS");
            if (charge < 0)
                charge = 0;

            ComboEffectBase comboEffectBase = NetworkAvatar.Inventory.FindComboEffect("FLAMESWORD");
            if ((bool)comboEffectBase && comboEffectBase is ComboEffect_FlameSword { isFlameSwordEnabled: true } comboEffect_FlameSword && !comboEffect_FlameSword.IsMax())
            {
                restoreTimer += Time.deltaTime * (1f + charge / 100f * percent);
                if (restoreTimer >= restoreFlameSwordTime)
                {
                    restoreTimer = 0f;
                    int b = comboEffect_FlameSword.maxSword + comboEffect_FlameSword.Networkavatar.GetCustomStatUnsafe("FLAMESWORDMAX");
                    if(comboEffect_FlameSword.NetworkcurrentSword + 1 + amp > b)
                    {
                        comboEffect_FlameSword.AddSword(b - comboEffect_FlameSword.NetworkcurrentSword);
                    }
                    else
                    {
                        comboEffect_FlameSword.AddSword(1 + amp);
                    }
                }
            }
        }
    }
}
