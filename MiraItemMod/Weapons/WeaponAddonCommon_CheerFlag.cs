using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Weapons
{
    public class WeaponAddonCommon_CheerFlag : WeaponAddon
    {
        public int percent = 2;
        public int flagTime = 15;
        public int effectRadius = 3;
        public int speed = 20;
        public int leaf = 20;
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("PERCENT", percent + "%"),
                new Loc.KeywordValue("SPEED", speed + "%"),
                new Loc.KeywordValue("LEAF", leaf.ToString())
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
            if(parent.Networkowner.unitAvatar.Money < leaf)
                return;
            if (!percent.Percent())
                return;
            parent.Networkowner.unitAvatar.AddMoney(-leaf);
            GameObject obj = UnityEngine.Object.Instantiate(SephiriaPrefabs.CheerFlag, avatar.transform.position, Quaternion.identity, parent.Networkowner.unitAvatar.transform.parent);
            NetworkServer.Spawn(obj);
            if (obj.TryGetComponent<FlagOfCheer>(out var component))
            {
                component.Initialize(parent.Networkowner.unitAvatar, flagTime, effectRadius, speed);
            }
        }

        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.unitAvatar.OnAttackUnit -= OnAttackUnit;
        }
    }
}
