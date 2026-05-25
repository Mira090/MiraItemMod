using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Pallas
{
    public class Charm_PallasHeart : Charm_PallasEnhancer
    {
        public static ItemPosition[] Directions = new ItemPosition[]
        {
        new ItemPosition(1, 0),
        new ItemPosition(-1, 0)
        };
        protected override ItemPosition[] GetDirections()
            => Directions;
        protected override void SetStats(CustomPallasController card)
        {
            card.HasHeart = true;
            if (card.NetworkAvatar == null)
                return;
            card.NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }
        protected override void ClearStats(CustomPallasController card)
        {
            card.HasHeart = false;
            if (card.NetworkAvatar == null)
                return;
            card.NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString()),
            new Loc.KeywordValue("HEAL", heal.ToString())
            };
        }

        public Timer cooldownTimer = new Timer(3f);

        private bool isInCooldown;

        public int heal = 1;
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.id != "Charm_PallasCard" && damage.id != "Charm_PallasJoker")
                return;
            if (isInCooldown)
                return;
            isInCooldown = true;
            NetworkAvatar.Heal(heal);
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }
    }
}
