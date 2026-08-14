using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_UpMachineDamage : Charm_MachinaBasic, IDependencyConditionCharm
    {
        public int[] damageBonusByLevel = new int[10] { 3, 5, 8, 12, 16, 23, 27, 34, 42, 50 };
        public bool machina = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string text = (showAllLevel ? (damageBonusByLevel.SafeRandomAccess(0).ToString("+0;-#") + "→" + damageBonusByLevel.SafeRandomAccess(maxLevel)) : damageBonusByLevel.SafeRandomAccess(LevelToIdx(level)).ToString("+0;-#"));
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("DAMAGE", text + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            };
        }
        public override void RefreshCharm()
        {
            base.RefreshCharm();
            if (machina)
            {
                Inventory.RemoveCharmDependency(Inventory.IdxToPos(MachinaSlot), this);
                machina = false;
            }

            if (!machina)
            {
                Inventory.AddCharmDependency(Inventory.IdxToPos(MachinaSlot), this);
                machina = true;
            }
        }
        protected override int OnRequestCharmDamageBonus(Charm_Basic rootCharm)
        {
            if (rootCharm == null)
            {
                return 0;
            }

            if (IsDependencyValid(rootCharm))
            {
                return damageBonusByLevel.SafeRandomAccess(CurrentLevelToIdx());
            }

            return 0;
        }
        public bool IsDependencyValid(Charm_Basic request)
        {
            if (request == this)
            {
                return false;
            }

            if (!(request is IAttackableCharm attackableCharm) || !attackableCharm.IsAttackableCharm())
            {
                return false;
            }

            if(request is Charm_MachinaBasic machina)
            {
                return machina.IsInMachinaSlot;
            }

            return false;
        }
    }
}
