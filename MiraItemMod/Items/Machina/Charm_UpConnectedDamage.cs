using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_UpConnectedDamage : Charm_MachinaBasic, IDependencyConditionCharm
    {
        public bool IsDependencyValid(Charm_Basic request)
        {
            if (request == null || !(request is Charm_MachinaBasic machina))
                return false;
            return GetConnectedMachinas().Contains(machina);
        }

        public int[] damageBonusByLevel = new int[10] { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
        public bool machina = false;
        public List<ItemPosition> positions;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string text = (showAllLevel ? (damageBonusByLevel.SafeRandomAccess(0).ToString("+0;-#") + "→" + damageBonusByLevel.SafeRandomAccess(maxLevel)) : damageBonusByLevel.SafeRandomAccess(LevelToIdx(level)).ToString("+0;-#"));
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("DAMAGE", text + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            };
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();

            if (machina)
            {
                ClearDependency();
                machina = false;
            }

            if (!machina)
            {
                ApplyDependency();
                machina = true;
            }
        }
        public override void RefreshCharm()
        {
            base.RefreshCharm();
            if (machina)
            {
                ClearDependency();
                machina = false;
            }

            if (!machina)
            {
                ApplyDependency();
                machina = true;
            }
        }
        protected void ClearDependency()
        {
            foreach(var pos in positions)
            {
                Inventory.RemoveCharmDependency(pos, this);
            }
            positions.Clear();
        }
        protected void ApplyDependency()
        {
            foreach(var charm in GetConnectedMachinas())
            {
                if(charm is Charm_Machina machina && machina.Item != null)
                {
                    Inventory.AddCharmDependency(machina.Item.Position, this);
                    positions.Add(machina.Item.Position);
                }
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
    }
}
