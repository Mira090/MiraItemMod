using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_LegendaryMania : Charm_Basic
    {
        private int savedCount;

        [Serializable]
        public class StatusGroup
        {
            public string statusID = "AP";

            public int[] valuesByLevel = new int[0];

            public bool hideIfStatValueIsZero;
        }
        public static StatusGroup CreateStatusGroup(string statusID, params int[] values)
        {
            return new StatusGroup { statusID = statusID, valuesByLevel = values };
        }

        [Header("Stats")]
        public StatusGroup[] stats = new StatusGroup[] {
            CreateStatusGroup("PHYSICAL_DAMAGE", 0, 1, 2),
            CreateStatusGroup("FIRE_DAMAGE", 0, 1, 2),
            CreateStatusGroup("ICE_DAMAGE", 0, 1, 2),
            CreateStatusGroup("LIGHTNING_DAMAGE", 0, 1, 2),
            CreateStatusGroup("CRITICAL", 50, 100, 150, 200),
            CreateStatusGroup("CRITICAL_DAMAGE_RATE", 1, 2, 4, 8),
            CreateStatusGroup("ATTACK_SPEED", 1, 2, 4, 8),
            //CreateStatusGroup("DASH_RECOVERY_SPEED", 2, 4, 6, 8),
            CreateStatusGroup("COOLDOWN_RECOVERY_SPEED", 2, 4, 6, 8),
            CreateStatusGroup("MAX_HP", 2, 4, 6, 8),
            CreateStatusGroup("MP_REGEN", 1, 2, 3, 4),
            CreateStatusGroup("DEFENSE", 1, 2, 3, 4),
            CreateStatusGroup("EVASION", 100, 200, 300, 400),
            CreateStatusGroup("EXP_DROP", 1, 2, 4, 8),
            CreateStatusGroup("LUCK", 1, 2, 3, 4),
            //CreateStatusGroup("BURN_SPEED", 1, 2, 3, 4),
            //CreateStatusGroup("FROSTBITE_DAMAGE", 1, 2, 3, 4),
            //CreateStatusGroup("ELECTRIC_QUICKNESS", 0, 0, 1, 1),
            CreateStatusGroup("FLAME_SWORD_FAST_FALL", 1, 2, 4, 8),
            CreateStatusGroup("CHARGING_CHARM_BONUS", 1, 3, 5, 10),
            CreateStatusGroup("DARK_CLOUD_SPEED", 5, 10, 15, 20),
            CreateStatusGroup("FOLLOWER_DAMAGE", 2, 4, 6, 8),
            CreateStatusGroup("PLANET_DAMAGE", 2, 4, 6, 8),
        };

        public bool showStatsEffectStringFirst;

        private StatusInstance[] instances;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            int num = 0;
            if ((bool)avatar)
            {
                num = avatar.Inventory.charms.Values.Count(charm => charm.Item.Entity.rarity == EItemRarity.Legend);
            }

            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("COUNT", num.ToString())
            };
        }
        public override int GetEffectStringCount()
        {
            return stats.Length + effectsString.Length;
        }

        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (!showStatsEffectStringFirst)
            {
                if (idx < effectsString.Length)
                {
                    return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
                }

                idx -= effectsString.Length;
                StatusInstance statusInstance = StatusDatabase.CreateStatusEntity(stats[idx].statusID, stats[idx].valuesByLevel.SafeRandomAccess(level));
                if (stats[idx].hideIfStatValueIsZero && statusInstance.Value == 0)
                {
                    return null;
                }

                Color color = statusInstance.Value < 0 ? GetNegativeColor(virtualLevelOffset) : GetPositiveColor(virtualLevelOffset);
                return statusInstance.ToString(reverse: true, color: false, sprite: true, color);
            }

            if (idx >= stats.Length)
            {
                return base.GetEffectString(idx - stats.Length, level, virtualLevelOffset, showAllLevel);
            }

            StatusInstance statusInstance2 = StatusDatabase.CreateStatusEntity(stats[idx].statusID, stats[idx].valuesByLevel.SafeRandomAccess(level));
            if (stats[idx].hideIfStatValueIsZero && statusInstance2.Value == 0)
            {
                return null;
            }

            Color color2 = statusInstance2.Value < 0 ? GetNegativeColor(virtualLevelOffset) : GetPositiveColor(virtualLevelOffset);
            return statusInstance2.ToString(reverse: true, color: false, sprite: true, color2);
        }

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (!(NetworkAvatar.Inventory == null))
            {
                NetworkAvatar.Inventory.OnCharmEffectRefreshedForServer += OnItemUpdated;
                savedCount = NetworkAvatar.Inventory.charms.Values.Count(charm => charm.Item.Entity.rarity == EItemRarity.Legend);
            }
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }

                instances = null;
            }

            instances = new StatusInstance[stats.Length];
            for (int j = 0; j < instances.Length; j++)
            {
                instances[j] = StatusDatabase.CreateStatusEntity(stats[j].statusID, stats[j].valuesByLevel.SafeRandomAccess(CurrentLevelToIdx()) * savedCount);
                instances[j].SetTarget(NetworkAvatar);
                instances[j].ApplyStatus(fromRuntime: true);
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }

                instances = null;
            }
            savedCount = 0;
            if (!(NetworkAvatar.Inventory == null))
            {
                NetworkAvatar.Inventory.OnCharmEffectRefreshedForServer -= OnItemUpdated;
            }
        }

        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }

                instances = null;
            }

            instances = new StatusInstance[stats.Length];
            for (int j = 0; j < instances.Length; j++)
            {
                instances[j] = StatusDatabase.CreateStatusEntity(stats[j].statusID, stats[j].valuesByLevel.SafeRandomAccess(LevelToIdx(newLevel)) * savedCount);
                instances[j].SetTarget(NetworkAvatar);
                instances[j].ApplyStatus(fromRuntime: true);
            }
        }

        private void OnItemUpdated()
        {
            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }

                instances = null;
            }
            savedCount = NetworkAvatar.Inventory.charms.Values.Count(charm => charm.Item.Entity.rarity == EItemRarity.Legend);

            instances = new StatusInstance[stats.Length];
            for (int j = 0; j < instances.Length; j++)
            {
                instances[j] = StatusDatabase.CreateStatusEntity(stats[j].statusID, stats[j].valuesByLevel.SafeRandomAccess(CurrentLevelToIdx()) * savedCount);
                instances[j].SetTarget(NetworkAvatar);
                instances[j].ApplyStatus(fromRuntime: true);
            }
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
