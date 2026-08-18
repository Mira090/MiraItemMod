using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaCooldown : Charm_Machina
    {
        public override string DamageId => "Charm_MachinaCooldown";
        public override float AttackDashScale => 0f;
        public override float RangeBonus => 0.5f;
        protected override void Awake()
        {
            cooldownTimer.time = 3f;
        }
        protected override void OnUpdate()
        {
            if (!NetworkAvatar.IsInBattle)
                return;
            base.OnUpdate();
            if (isInCooldown)
                return;
            isInCooldown = true;
            Attack();
        }
    }
}
