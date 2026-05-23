using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_BondCoin : Charm_StatusInstance
    {
        public LocalizedString semanticName = new LocalizedString("BondArtifact");

        public string semantic = "DUAL";

        public int[] semanticDropWeight = new int[] { 2, 3, 4 };

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (GetValue(semanticDropWeight.SafeRandomAccess(0)).ToString() + "→" + GetValue(semanticDropWeight.SafeRandomAccess(maxLevel))) : GetValue(semanticDropWeight.SafeRandomAccess(LevelToIdx(level))).ToString());//"+0;-#"
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("ITEM_TYPE", semanticName.ToString()),
            new Loc.KeywordValue("DROP_PERCENT", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        private int GetValue(int weight)
        {
            return weight * KeywordDatabase.GetConstValue("adaptiveItemDropUnitPercent");
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight.SafeRandomAccess(CurrentLevelToIdx()));
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight.SafeRandomAccess(LevelToIdx(oldLevel)));
            base.NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight.SafeRandomAccess(LevelToIdx(newLevel)));
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
