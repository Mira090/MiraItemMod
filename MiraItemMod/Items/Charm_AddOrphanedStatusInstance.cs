using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_AddOrphanedStatusInstance : Charm_Basic
    {
        [System.Serializable]
        public class OrphanedStatusGroup
        {
            public string statusID = "AP";

            public int value;

            public bool hideIfStatValueIsZero;
        }

        [Header("Stats")]
        public Charm_StatusInstance.StatusGroup[] stats = new Charm_StatusInstance.StatusGroup[0];
        public bool showStatsEffectStringFirst;

        /// <summary>
        /// Item_AddOrphanedStatusInstance_Effect
        /// このアーティファクトを消費して、以下の効果を獲得する
        /// </summary>
        public LocalizedString firstEffectString = new LocalizedString("Item_AddOrphanedStatusInstance_Effect");

        private bool reward = false;
        private bool rewardReceived = false;

        public override int GetEffectStringCount()
        {
            return stats.Length + effectsString.Length + 1;
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if(idx == 0)
            {
                return KeywordDatabase.Convert(firstEffectString.ToString(), useColor: false);
            }
            else
            {
                idx--;
            }

            if (!showStatsEffectStringFirst)
            {
                if (idx < effectsString.Length)
                {
                    return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
                }

                idx -= effectsString.Length;
                StatusInstance statusInstance = StatusDatabase.CreateStatusEntity(stats[idx].statusID, stats[idx].valuesByLevel.SafeRandomAccess(0));
                if (stats[idx].hideIfStatValueIsZero && statusInstance.Value == 0)
                {
                    return null;
                }

                Color color = ((maxLevel <= 0) ? Color.white : ((statusInstance.Value < 0) ? Charm_Basic.GetNegativeColor(virtualLevelOffset) : Charm_Basic.GetPositiveColor(virtualLevelOffset)));
                return statusInstance.ToString(reverse: true, color: false, sprite: true, color);
            }

            if (idx >= stats.Length)
            {
                return base.GetEffectString(idx - stats.Length, level, virtualLevelOffset, showAllLevel);
            }

            StatusInstance statusInstance2 = StatusDatabase.CreateStatusEntity(stats[idx].statusID, stats[idx].valuesByLevel.SafeRandomAccess(0));
            if (stats[idx].hideIfStatValueIsZero && statusInstance2.Value == 0)
            {
                return null;
            }

            Color color2 = ((maxLevel <= 0) ? Color.white : ((statusInstance2.Value < 0) ? Charm_Basic.GetNegativeColor(virtualLevelOffset) : Charm_Basic.GetPositiveColor(virtualLevelOffset)));
            return statusInstance2.ToString(reverse: true, color: false, sprite: true, color2);
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            reward = true;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (reward && !rewardReceived)
            {
                foreach (var stat in stats)
                {
                    if (Core.LogMany)
                        Core.Logger("stat: " + stat.statusID);
                    var instance = StatusDatabase.CreateStatusEntity(stat.statusID, stat.valuesByLevel.SafeRandomAccess(0));
                    NetworkAvatar.AddOrphanedStatusInstance(instance);
                    if (Core.LogMedium)
                        Core.Logger("Add Status: " + instance.ToString(false, false, false));
                }
                using (new GridInventory.Permission(NetworkAvatar.Inventory))
                {
                    NetworkAvatar.Inventory.ForceRemoveItem(Item.XIdx, Item.YIdx);
                }
                rewardReceived = true;
            }
        }
    }
}
