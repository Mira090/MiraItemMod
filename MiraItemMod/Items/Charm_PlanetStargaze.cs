using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_PlanetStargaze : Charm_VariableMaxLevel
    {
        private static ItemPosition[] directions = new ItemPosition[8]
        {
        new ItemPosition(-1, 0),
        new ItemPosition(1, 0),
        new ItemPosition(0, -1),
        new ItemPosition(0, 1),
        new ItemPosition(-1, -1),
        new ItemPosition(1, -1),
        new ItemPosition(-1, 1),
        new ItemPosition(1, 1)
        };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            return new Loc.KeywordValue[] { new Loc.KeywordValue("LEVEL", "+1", GetPositiveColor(virtualLevelOffset)) };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("PlanetStargaze".ToUpperInvariant(), 1);
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("PlanetStargaze".ToUpperInvariant(), -1);
        }
        public override void SetAdditionalMaxLevel(int level)
        {
            base.SetAdditionalMaxLevel(level + GetAroundPlanets());
        }
        public int GetAroundPlanets()
        {
            int count = 0;
            ItemPosition[] array = directions;
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = base.NetworkAvatar.Inventory.FindItem(new ItemPosition(base.Item.XIdx, base.Item.YIdx) + itemPosition);
                if (newItemOwnInstance != null && newItemOwnInstance.Entity.categories.Contains(ItemCategories.Planet))
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is Charm_SummonGreenBat charm_SummonGreenBat)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
