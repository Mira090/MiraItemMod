using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaMod.Items
{
    public class Charm_Transcendent : Charm_StatusInstance
    {
        public int Transcendent => 2049;//Data.Transcendent.Id;
        public LocalizedString semanticName = new LocalizedString("Item_StoneTablet_Disconnect_Name");
        public string semantic = "DISCONNECT";
        public string semanticNo = "NO_DISCONNECT";
        public int[] semanticDropWeight = new int[] { 4, 6, 8, 10 };

        public int[] damageByLevel = new int[] { 4, 5, 6, 8, 10, 12 };
        private int enabledCriticalBonus;
        private int savedStoneTabletCount;

        private int quest = -1;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            var count = 0;
            if(avatar != null)
            {
                count = avatar.Inventory.stoneTablets.Values.Count(tablet => tablet.entityID == 2049 || tablet.entityID == Transcendent);
            }
            string value = showAllLevel ? semanticDropWeight.SafeRandomAccess(0) * 50 + "→" + semanticDropWeight.SafeRandomAccess(maxLevel) * 50 : (semanticDropWeight.SafeRandomAccess(LevelToIdx(level)) * 50).ToString();
            string value2 = showAllLevel ? damageByLevel.SafeRandomAccess(0) + "→" + damageByLevel.SafeRandomAccess(maxLevel) : damageByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[4]
            {
            new Loc.KeywordValue("ITEM_TYPE", semanticName.ToString()),
            new Loc.KeywordValue("DROP_PERCENT", value + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", "+" + value2, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", count.ToString())
            };
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if(quest > 0)
            {
                using (new GridInventory.Permission(Inventory))
                {
                    for (int q = 0; q < 6; q++)
                    {
                        var pos = new ItemPosition(q, quest);
                        var item = Inventory.FindItem(pos);
                        if (item == null || item.EntityID != 2049)
                            continue;
                        var instanceId = item.InstanceID;
                        Inventory.ForceRemoveItem(pos.x, pos.y);
                        Inventory.AddItemAtPosition(new ItemMetadata(instanceId, Transcendent, 1), pos);
                    }
                }
                quest = -1;
            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (quest >= 0)
                return;
            for (int q = 0; q < Inventory.Height; q++)
            {
                bool flag = true;
                for(int q2 = 0;q2 < 6; q2++)
                {
                    var item = Inventory.FindItem(new ItemPosition(q2, q));
                    if (item == null || item.EntityID != 2049)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    quest = q;
                    return;
                }
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semanticNo, -semanticDropWeight.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("DISCONNECT", 1);

            if (!(NetworkAvatar.Inventory == null))
            {
                NetworkAvatar.Inventory.OnCharmEffectRefreshedForServer += OnItemUpdated;
                savedStoneTabletCount = NetworkAvatar.Inventory.stoneTablets.Values.Count(tablet => tablet.entityID == 2049 || tablet.entityID == Transcendent);
                enabledCriticalBonus = damageByLevel.SafeRandomAccess(CurrentLevelToIdx()) * savedStoneTabletCount;
                NetworkAvatar.AddCustomStat(ECustomStat.PhysicalDamage, enabledCriticalBonus);
            }
        }
        private void OnItemUpdated()
        {
            NetworkAvatar.AddCustomStat(ECustomStat.PhysicalDamage, -enabledCriticalBonus);
            savedStoneTabletCount = NetworkAvatar.Inventory.stoneTablets.Values.Count(tablet => tablet.entityID == 2049 || tablet.entityID == Transcendent);
            enabledCriticalBonus = damageByLevel.SafeRandomAccess(CurrentLevelToIdx()) * savedStoneTabletCount;
            NetworkAvatar.AddCustomStat(ECustomStat.PhysicalDamage, enabledCriticalBonus);
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semanticNo, +semanticDropWeight.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("DISCONNECT", -1);
            NetworkAvatar.AddCustomStat(ECustomStat.PhysicalDamage, -enabledCriticalBonus);
            savedStoneTabletCount = 0;
            enabledCriticalBonus = 0;
            if (!(NetworkAvatar.Inventory == null))
            {
                NetworkAvatar.Inventory.OnCharmEffectRefreshedForServer -= OnItemUpdated;
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, -semanticDropWeight.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semantic, semanticDropWeight.SafeRandomAccess(LevelToIdx(newLevel)));
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semanticNo, semanticDropWeight.SafeRandomAccess(LevelToIdx(oldLevel)));
            NetworkAvatar.Inventory.AddItemDropBonusBySemantic(semanticNo, -semanticDropWeight.SafeRandomAccess(LevelToIdx(newLevel)));
            NetworkAvatar.AddCustomStat(ECustomStat.PhysicalDamage, -enabledCriticalBonus);
            enabledCriticalBonus = damageByLevel.SafeRandomAccess(LevelToIdx(newLevel)) * savedStoneTabletCount;
            NetworkAvatar.AddCustomStat(ECustomStat.PhysicalDamage, enabledCriticalBonus);
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
