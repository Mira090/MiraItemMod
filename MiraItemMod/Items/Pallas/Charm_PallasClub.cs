using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Pallas
{
    public class Charm_PallasClub : Charm_PallasEnhancer
    {
        public static ItemPosition[] Directions = new ItemPosition[]
        {
        new ItemPosition(-1, -1),
        new ItemPosition(1, 1)
        };
        protected override ItemPosition[] GetDirections()
            => Directions;
        protected override void SetStats(CustomPallasController card)
        {
            card.HasClub = true;
            if (card.NetworkAvatar == null)
                return;
            card.NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }
        protected override void ClearStats(CustomPallasController card)
        {
            card.HasClub = false;
            if (card.NetworkAvatar == null)
                return;
            card.NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString()),
            new Loc.KeywordValue("BUFF", buff.ToString())
            };
        }
        public Timer cooldownTimer = new Timer(3f);

        private bool isInCooldown;

        public int buff = 1;
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.id != "Charm_PallasCard" && damage.id != "Charm_PallasJoker")
                return;
            if (isInCooldown)
                return;
            isInCooldown = true;
            NetworkAvatar.ApplyBuff(Data.PallasBuff, buff, NetworkAvatar, true);
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
