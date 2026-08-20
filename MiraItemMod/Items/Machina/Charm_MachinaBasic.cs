using MiraItemMod.Combos;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaBasic : Charm_VariableMaxLevel
    {
        public override string StatusName => string.Empty;
        public override int ValiableMax => 9;
        public int MachinaSlot => ComboEffect_Machina.GetMachinaSlot(Inventory);
        public int DemolitionSlot => ComboEffect_Machina.GetDemolitionSlot(Inventory);
        public bool IsInMachinaSlot
        {
            get
            {
                if (NetworkAvatar == null || Inventory == null || Item == null)
                    return false;
                var combo = Inventory.FindComboEffect(ItemCategories.Machina);
                if (combo is ComboEffect_Machina machina && machina.comboCount >= machina.slotActivateComboCount)
                {
                    return Inventory.PosToIdx(Item.Position) == MachinaSlot;
                }
                return false;
            }
        }
        public bool IsInDemolitionSlot
        {
            get
            {
                if (NetworkAvatar == null || Item == null)
                    return false;
                var combo = NetworkAvatar.Inventory.FindComboEffect(ItemCategories.Machina);
                if (combo is ComboEffect_Machina machina && machina.comboCount >= machina.slotActivateComboCount)
                {
                    return NetworkAvatar.Inventory.PosToIdx(Item.Position) == DemolitionSlot;
                }
                return false;
            }
        }

        public bool destroy;
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            LoadItemOnServer(SaveManager.CurrentRun);
            Inventory.UpdatePing(Item.Position);
        }
        public void Repair(int add = 1)
        {
            var added = AdditionalMaxLevel + add;
            SetAdditionalMaxLevel(added);
            RpcSetAdditionalMaxLevel(added);
            Inventory.UpdatePing(Item.Position);
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        public void Dismantle()
        {
            if(maxLevel == 0)
            {
                destroy = true;
                return;
            }

            var added = AdditionalMaxLevel - 1;
            SetAdditionalMaxLevel(added);
            RpcSetAdditionalMaxLevel(added);
            Inventory.UpdatePing(Item.Position);
            SaveItemOnServer(SaveManager.CurrentRun);
        }
        public override void OnCharmEffectRefreshed()
        {

        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (destroy)
            {
                destroy = false;

                using (new GridInventory.Permission(Inventory))
                {
                    Inventory.ForceRemoveItem(Item.Position);
                }
            }
        }
        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_MachinaBasic_{Item.InstanceID}_Level", AdditionalMaxLevel);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            var level = saveData.GetInt($"CharmSaveData_MachinaBasic_{Item.InstanceID}_Level", AdditionalMaxLevel);
            SetAdditionalMaxLevel(level);
            RpcSetAdditionalMaxLevel(level);
        }
    }
}
