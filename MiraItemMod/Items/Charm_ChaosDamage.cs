using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_ChaosDamage : Charm_VariableMaxLevel, IAttackableCharm
    {
        public EDamageElementalType elementalType = EDamageElementalType.Chaos;

        public int[] damageByLevel = new int[] { 5, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80 };

        public string damageId = "Charm_ChaosDamage";

        public Timer cooldownTimer = new Timer(3f);

        private bool isInCooldown;

        private LocalizedString damageString = new LocalizedString("Skill_Specs_Common");

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? damageByLevel.SafeRandomAccess(0) + "→" + damageByLevel.SafeRandomAccess(maxLevel) : damageByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("ELEMENTAL", "<tag=Elemental_Chaos>" + damageString.ToString()),
            new Loc.KeywordValue("DAMAGE", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("TIME", cooldownTimer.time.ToString("0.##"))
            };
        }

        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            NetworkAvatar.OnAttackUnit += HandleAttackUnit;
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            NetworkAvatar.OnAttackUnit -= HandleAttackUnit;
        }

        private void HandleAttackUnit(UnitAvatar target, DamageInstance damageInstance)
        {
            if (!NetworkAvatar.IsDead && IsEffectEnabled && !isInCooldown && (damageInstance.fromType == EDamageFromType.DirectAttack || damageInstance.fromType == EDamageFromType.Magic) && !target.IsDead)
            {
                isInCooldown = true;
                float damage = Charm_Basic.CalculateDamage(this);
                DamageInstance damage2 = DamageInstance.GetDamage(NetworkAvatar, damageId, target.transform.position, 4294967295L, damage, EDamageType.Slice, EDamageFromType.None, Vector2.zero, 0, 0f);
                damage2.SetCustomColor(true, new Color(0.5f, 0, 0));
                target.ApplyDamage(damage2);
            }
        }

        public override bool Weaved()
        {
            return true;
        }

        public float GetDamage(UnitAvatar avatar)
        {
            float damage = damageByLevel.SafeRandomAccess(CurrentLevelToIdx());
            return damage;
        }
    }
}
