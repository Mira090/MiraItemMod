using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items
{
    public class Charm_LevelDistributer : Charm_VariableMaxLevel
    {
        public ItemPosition Pos1 { get; set; }
        public int Level { get; set; }
        private bool flag = false;
        private bool enableFlag = false;
        private bool disableFlag = false;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? 0 + "→" + maxLevel : LevelToIdx(level).ToString();
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("LEVEL", "+" + value, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (flag)
            {
                flag = false;
                using (new GridInventory.Permission(Inventory))
                {
                    if(Level > 0)
                    {
                        Inventory.AddDungeonTempLevel(Pos1, -Level);
                    }
                    Pos1 = new ItemPosition(xIdx, yIdx - 1);
                    Level = CurrentLevelToIdx();
                    Inventory.AddDungeonTempLevel(Pos1, Level);
                }
                Inventory.UpdatePing(Pos1);
            }
            if (enableFlag)
            {
                enableFlag = false;
                using (new GridInventory.Permission(Inventory))
                {
                    Pos1 = new ItemPosition(xIdx, yIdx - 1);
                    Level = CurrentLevelToIdx();
                    Inventory.AddDungeonTempLevel(Pos1, Level);
                }
                Inventory.UpdatePing(Pos1);
            }
            if (disableFlag)
            {
                disableFlag = false;
                using (new GridInventory.Permission(Inventory))
                {
                    Inventory.AddDungeonTempLevel(Pos1, -Level);
                    Pos1 = new ItemPosition(xIdx, yIdx - 1);
                    Level = CurrentLevelToIdx();
                }
                Inventory.UpdatePing(Pos1);
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            //NetworkAvatar.Inventory.OnItemUpdatedForServer += OnItemUpdatedForServer;
            //enableFlag = true;
            if(Level > 0)
            {
                using (new GridInventory.Permission(Inventory))
                {
                    Pos1 = new ItemPosition(xIdx, yIdx - 1);
                    Level = CurrentLevelToIdx();
                    Inventory.AddDungeonTempLevel(Pos1, Level);
                }
            }
        }

        private void OnItemUpdatedForServer()
        {
            //if (IsEffectEnabled)
                //flag = Pos1.x != xIdx || Pos1.y != yIdx - 1 || Level != CurrentLevelToIdx();
            //Core.Logger(flag + $": {Pos1.x}, {Pos1.y} => {xIdx}, {yIdx - 1} / {Level} => {CurrentLevelToIdx()}");
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            //NetworkAvatar.Inventory.OnItemUpdatedForServer -= OnItemUpdatedForServer;
            //disableFlag = true;
            //Core.Logger("OnDisabledEffect: " + base.Inventory.GetWritePermission());
            if (Level > 0)
            {
                //Core.Logger(flag + $"[3]: {Pos1.x}, {Pos1.y} => {xIdx}, {yIdx - 1} / {Level} => {CurrentLevelToIdx()}");
                if (Inventory.GetWritePermission())
                {
                    Inventory.AddDungeonTempLevel(Pos1, -Level);
                    Pos1 = new ItemPosition(xIdx, yIdx - 1);
                    Level = -1;
                }
                else
                {
                    using (new GridInventory.Permission(Inventory))
                    {
                        Inventory.AddDungeonTempLevel(Pos1, -Level);
                        Pos1 = new ItemPosition(xIdx, yIdx - 1);
                        Level = -1;
                    }
                }
            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (IsEffectEnabled)
                flag = Pos1.x != xIdx || Pos1.y != yIdx - 1 || Level != CurrentLevelToIdx();
            //Core.Logger(flag + $"[1]: {Pos1.x}, {Pos1.y} => {xIdx}, {yIdx - 1} / {Level} => {CurrentLevelToIdx()}");
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            if (IsEffectEnabled)
                flag = LevelToIdx(oldLevel) != LevelToIdx(newLevel);
            //Core.Logger(flag + $"[2]: {Pos1.x}, {Pos1.y} => {xIdx}, {yIdx - 1} / {oldLevel} => {newLevel}");
            /*
            base.OnUpdatedLevel(oldLevel, newLevel);
            using (new GridInventory.Permission(base.Inventory))
            {
                base.Inventory.AddDungeonTempLevel(Pos1, -LevelToIdx(oldLevel));
                Pos1 = new(xIdx, yIdx - 1);
                base.Inventory.AddDungeonTempLevel(Pos1, LevelToIdx(newLevel));
            }*/
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
