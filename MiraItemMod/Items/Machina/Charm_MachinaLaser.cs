using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaLaser : Charm_Machina, IDependencyConditionCharm
    {
        public int[] radius = new int[] { 5 };
        public int per = 3;
        public bool IsDependencyValid(Charm_Basic request)
        {
            if (request == null || !(request is Charm_MachinaBasic machina))
                return false;
            return GetConnectedMachinas().Contains(machina);
        }
        public int GetCount()
        {
            return 1 + GetConnectedMachinas().Count / per;
        }
        public int GetCount(GridInventory inventory)
        {
            return 1 + GetConnectedMachinas(inventory).Count / per;
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            var keywords = base.BuildKeywords(avatar, level, virtualLevelOffset, showAllLevel, ignoreAvatarStatus).ToList();
            keywords.Add(new Loc.KeywordValue("PER", per.ToString()));
            if (!ignoreAvatarStatus && avatar != null && avatar.Inventory != null)
                keywords.Add(new Loc.KeywordValue("LASER", GetCount(avatar.Inventory).ToString(), GetPositiveColor(virtualLevelOffset)));
            else
                keywords.Add(new Loc.KeywordValue("LASER", "-", GetPositiveColor(virtualLevelOffset)));
            return keywords.ToArray();
        }
        protected override NewWeaponFireData GetFireData()
        {
            var weapon = WeaponDatabase.FindWeaponById(17);
            if (weapon == null)
                return null;
            if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_GreatSword>(out var simple))
                return null;
            return simple.basicComboAttacks.FirstOrDefault();
        }
        public override float AttackDashScale => 0f;
        public override float RangeBonus => 1f;
        protected override void Awake()
        {
            cooldownTimer.time = 2f;
            damageByLevel = new int[10] { 50, 60, 70, 80, 90, 100, 120, 140, 160, 180 };
        }
        protected override void OnUpdate()
        {
            if (!NetworkAvatar.IsInBattle)
                return;
            base.OnUpdate();
            if (isInCooldown)
                return;
            isInCooldown = true;

            var targets = GetTargets();
            for(int q = 0; q < GetCount(); q++)
            {
                var target = targets.SafeRandomAccess(q, ArrayExtensions.ERandomAccessType.Repeat);
                Attack(target);
            }
        }
        public List<UnitAvatar> GetTargets()
        {
            List<UnitAvatar> targets = new List<UnitAvatar>();
            var cachedHits = new Collider2D[10];
            int num = HorayPhysics2D.OverlapCircle(NetworkAvatar.transform.position, radius.SafeRandomAccess(CurrentLevelToIdx()), cachedHits, CombatManager.Topdown1FLayerMask);
            for (int j = 0; j < num; j++)
            {
                var hitbox = cachedHits[j].GetComponent<Hitbox>();
                if (hitbox == null)
                    continue;
                CombatBehaviour combatBehaviour = hitbox.GetCombatBehaviour(0);
                if (combatBehaviour == null)
                    continue;

                UnitAvatar unitAvatar = combatBehaviour as UnitAvatar;
                if (unitAvatar != null && !unitAvatar.IsDead && !unitAvatar.IsInvulnerable && unitAvatar.canBeTarget.IsTrue() && CombatManager.ContainsAttackableFaction(unitAvatar.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.faction))
                {
                    targets.Add(unitAvatar);
                }
            }
            return targets;
        }
        public override string GetRelatedStatFormula(NewWeaponFireData fireData)
        {
            return "HIGHEST";
        }
        public override EDamageElementalType? GetDamageElementalType(NewWeaponFireData fireData)
        {
            return EDamageElementalType.Chaos;
        }
        public override Vector3 FirePosition(WeaponControllerSimple simple)
        {
            var delta = base.FirePosition(simple) - simple.transform.position;
            return simple.transform.position - delta;
        }
    }
}
