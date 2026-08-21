using MiraItemMod.Combos;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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

        protected List<ItemPosition> AdjacentPositions = new List<ItemPosition> { new ItemPosition(0, 1), new ItemPosition(0, -1), new ItemPosition(1, 0), new ItemPosition(-1, 0) };
        protected List<Charm_MachinaBasic> GetAdjacentMachinas()
        {
            var list = new List<Charm_MachinaBasic>();
            try
            {
                if (Item == null)
                    return list;
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var direction in AdjacentPositions)
            {
                var charm = NetworkAvatar.Inventory.FindItem(Item.Position + direction);
                if (charm == null)
                    continue;
                if(charm.Charm is Charm_MachinaBasic machina)
                {
                    list.Add(machina);
                }
            }
            return list;
        }
        protected List<Charm_MachinaBasic> GetAdjacentMachinas(GridInventory inventory)
        {
            var list = new List<Charm_MachinaBasic>();
            try
            {
                if (Item == null)
                    return list;
            }
            catch (Exception e)
            {
                return list;
            }
            foreach (var direction in AdjacentPositions)
            {
                var charm = inventory.FindItem(Item.Position + direction);
                if (charm == null)
                    continue;
                if (charm.Charm is Charm_MachinaBasic machina)
                {
                    list.Add(machina);
                }
            }
            return list;
        }
        public List<Charm_MachinaBasic> GetConnectedMachinas()
            => GetConnectedMachinas(new List<Charm_MachinaBasic>());
        protected List<Charm_MachinaBasic> GetConnectedMachinas(List<Charm_MachinaBasic> list)
        {
            try
            {
                if (Item == null)
                    return list;
            }
            catch (Exception e)
            {
                return list;
            }
            if (list.Contains(this))
                return list;
            list.Add(this);
            foreach(var adjacent in GetAdjacentMachinas())
            {
                if (list.Contains(adjacent))
                    continue;
                adjacent.GetConnectedMachinas(list);
            }
            return list;
        }
        public List<Charm_MachinaBasic> GetConnectedMachinas(GridInventory inventory)
            => GetConnectedMachinas(inventory, new List<Charm_MachinaBasic>());
        protected List<Charm_MachinaBasic> GetConnectedMachinas(GridInventory inventory, List<Charm_MachinaBasic> list)
        {
            try
            {
                if (Item == null)
                    return list;
            }
            catch (Exception e)
            {
                return list;
            }
            if (list.Contains(this))
                return list;
            list.Add(this);
            foreach (var adjacent in GetAdjacentMachinas(inventory))
            {
                if (list.Contains(adjacent))
                    continue;
                adjacent.GetConnectedMachinas(inventory, list);
            }
            return list;
        }
    }
}
