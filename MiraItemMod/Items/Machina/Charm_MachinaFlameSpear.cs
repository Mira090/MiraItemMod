using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaFlameSpear : Charm_Machina
    {
        private bool recentLuck;
        public override float RangeBonus => recentLuck ? base.RangeBonus : base.RangeBonus + 0.33f;
        protected override void Awake()
        {
            damageByLevel = new int[10] { 115, 120, 130, 140, 150, 160, 170, 190, 210, 240 };
        }
        public override string DamageId => "Charm_MachinaFlameSpear";
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(519);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponAddonCommon_ChangeWeaponAction>(out var addon))
                return null;
            return addon.fireData;
        }
        protected override float ModifyDamage(float damage)
        {
            damage += damage * NetworkAvatar.GetCustomStatUnsafe("FLAMESWORDDAMAGE") / 100f;
            recentLuck = ComboEffect_FlameSword.CheckLuck(NetworkAvatar);
            if (recentLuck)
            {
                int luck = KeywordDatabase.GetConstValue("flameSwordLuckBonusDamagePercent");
                damage += damage * luck / 100f;
            }
            if (NetworkAvatar.GetCustomStatUnsafe("FLAMESWORDMAGICDAMAGE") > 0)
            {
                damage += damage * (NetworkAvatar.GetCustomStat(ECustomStat.MagicDamageBonus) / 100f);
            }
            return base.ModifyDamage(damage);
        }
        ///パッチはCharm_EmberFlameSword

        protected override void OnCreateAttack(int idx, ProjectileBase projectile)
        {
            if (NetworkAvatar == null || projectile == null)
                return;
            projectile.additionalCriticalChancePercent += NetworkAvatar.GetCustomStatUnsafe("FLAMESWORDCRITICAL");
            projectile.additionalCriticalDamageRate += NetworkAvatar.GetCustomStatUnsafe("FLAMESWORDCRITICALDAMAGERATE");
            projectile.ignoreDefense += NetworkAvatar.GetCustomStatUnsafe("FLAMESWORDIGNOREDEFENSE");
        }
    }
}
