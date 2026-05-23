using FMODUnity;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_StargazeTablet : Charm_VariableMaxLevel
    {
        private int[] damageByLevel = new int[] { 1 };
        private string damageId = "Stargaze";
        private int count;
        private int countRequire = 3;
        private int countView = 0;

        private bool questCleared;
        public static LocalizedString Notice = new LocalizedString("Item_Stargaze_Tablet_Notice");

        private int[] levelByLevel = new int[] { 1, 2, 3, 4, 5, 6, 8, 10, 12, 15, 18 };

        private int power = 0;

        //StudioEventEmitter DestroyTabletSound;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            int p = 0;
            if ((bool)avatar)
            {
                p = avatar.Inventory.inventoryMatrix.Values.Where(charm => charm.Charm != null).Sum(charm => charm.Charm.CurrentLevelToIdx());
            }
            string value = showAllLevel ? levelByLevel.SafeRandomAccess(0) + "→" + levelByLevel.SafeRandomAccess(maxLevel) : levelByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? 100 * damageByLevel.SafeRandomAccess(0) + "→" + 100 * damageByLevel.SafeRandomAccess(maxLevel) : (100 * damageByLevel.SafeRandomAccess(LevelToIdx(level))).ToString();
            string value3 = showAllLevel ? p * damageByLevel.SafeRandomAccess(0) + "→" + p * damageByLevel.SafeRandomAccess(maxLevel) : (p * damageByLevel.SafeRandomAccess(LevelToIdx(level))).ToString();
            return new Loc.KeywordValue[4]
            {
                new Loc.KeywordValue("QUEST", countRequire.ToString()),
                new Loc.KeywordValue("REWARD", "1"),
            new Loc.KeywordValue("COUNT", value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("CURRENT", countView.ToString(), GetPositiveColor(virtualLevelOffset)),
            //new Loc.KeywordValue("POWER_DAMAGE", value2 + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            //new Loc.KeywordValue("DAMAGE", "+" + value3, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            //new Loc.KeywordValue("POWER", p.ToString(), Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (questCleared)
            {
                ItemPosition pos = new ItemPosition(Item.XIdx, Item.YIdx);
                GridInventory inventory = Inventory;

                using (new GridInventory.Permission(inventory))
                {
                    //inventory.ForceRemoveItem(base.Item.XIdx, base.Item.YIdx);
                    var list = new List<ItemPosition>();


                    if (IsValidPos(inventory, pos.x + 1, pos.y))
                        list.Add(new ItemPosition(pos.x + 1, pos.y));
                    if (IsValidPos(inventory, pos.x - 1, pos.y))
                        list.Add(new ItemPosition(pos.x - 1, pos.y));
                    if (IsValidPos(inventory, pos.x, pos.y + 1))
                        list.Add(new ItemPosition(pos.x, pos.y + 1));
                    if (IsValidPos(inventory, pos.x, pos.y - 1))
                        list.Add(new ItemPosition(pos.x, pos.y - 1));
                    if (IsValidPos(inventory, pos.x + 1, pos.y + 1))
                        list.Add(new ItemPosition(pos.x + 1, pos.y + 1));
                    if (IsValidPos(inventory, pos.x - 1, pos.y + 1))
                        list.Add(new ItemPosition(pos.x - 1, pos.y + 1));
                    if (IsValidPos(inventory, pos.x + 1, pos.y - 1))
                        list.Add(new ItemPosition(pos.x + 1, pos.y - 1));
                    if (IsValidPos(inventory, pos.x - 1, pos.y - 1))
                        list.Add(new ItemPosition(pos.x - 1, pos.y - 1));

                    var idx = CurrentLevelToIdx();
                    for(int q = 0; q < levelByLevel[idx] && list.Count > 0; q++)
                    {
                        Inventory.AddDungeonTempLevel(list.GetRandom(), 1);
                    }
                }
                power = NetworkAvatar.Inventory.charms.Values.Sum(charm => charm.CurrentLevelToIdx());
                questCleared = false;
                count -= countRequire;
                questCleared = count >= countRequire;
                SaveItemOnServer(SaveManager.CurrentRun);
            }
        }
        private static bool IsValidPos(GridInventory inventory, int x, int y)
        {
            if (x < 0 || y < 0)
                return false;
            if (x >= inventory.Width)
                return false;
            if (inventory.PosToIdx(new ItemPosition(x, y)) >= inventory.CurrentInventoryStorage)
                return false;
            return !inventory.charms.TryGetValue(new ItemPosition(x, y), out var _);
        }
        private void OnAttackUnit(UnitAvatar unitAvatar, DamageInstance damageInstance)
        {
            if (!NetworkAvatar.IsDead && IsEffectEnabled && damageInstance.id != damageId)
            {
                float customStatUnsafe = power * damageByLevel.SafeRandomAccess(CurrentLevelToIdx());
                if (customStatUnsafe > 0 && unitAvatar != null)
                {
                    DamageInstance damage = DamageInstance.GetDamage(NetworkAvatar, damageId, unitAvatar.transform.position, 4294967295L, customStatUnsafe, EDamageType.ElementalEffectDamage, EDamageFromType.None, Vector2.zero, 0, 0f);
                    damage.elementalType = EDamageElementalType.Chaos;
                    unitAvatar.ApplyDamage(damage);
                }
            }
        }
        private string CreateQuery(GridInventory inventory, ItemPosition pos)
        {
            StringBuilder sb = new StringBuilder();
            if(inventory.charms.TryGetValue(new ItemPosition(pos.x + 1, pos.y), out var _))
            {
                sb.AppendLine("RIGHT " + levelByLevel[CurrentLevelToIdx()]);
            }
            if (inventory.charms.TryGetValue(new ItemPosition(pos.x - 1, pos.y), out var _))
            {
                sb.AppendLine("LEFT " + levelByLevel[CurrentLevelToIdx()]);
            }
            if (inventory.charms.TryGetValue(new ItemPosition(pos.x, pos.y - 1), out var _))
            {
                sb.AppendLine("UP " + levelByLevel[CurrentLevelToIdx()]);
            }
            if (inventory.charms.TryGetValue(new ItemPosition(pos.x, pos.y + 1), out var _))
            {
                sb.AppendLine("DOWN " + levelByLevel[CurrentLevelToIdx()]);
            }
            if (inventory.charms.TryGetValue(new ItemPosition(pos.x + 1, pos.y - 1), out var _))
            {
                sb.AppendLine("DIAUPRIGHT " + levelByLevel[CurrentLevelToIdx()]);
            }
            if (inventory.charms.TryGetValue(new ItemPosition(pos.x - 1, pos.y - 1), out var _))
            {
                sb.AppendLine("DIAUPLEFT " + levelByLevel[CurrentLevelToIdx()]);
            }
            if (inventory.charms.TryGetValue(new ItemPosition(pos.x + 1, pos.y + 1), out var _))
            {
                sb.AppendLine("DIADOWNRIGHT " + levelByLevel[CurrentLevelToIdx()]);
            }
            if (inventory.charms.TryGetValue(new ItemPosition(pos.x - 1, pos.y + 1), out var _))
            {
                sb.AppendLine("DIADOWNLEFT " + levelByLevel[CurrentLevelToIdx()]);
            }
            return sb.ToString();
        }
        private void Awake()
        {
            effectHUD_ID = "STARGAZETABLET";
        }
        public void Start()
        {
            Events.OnValueRecieved += OnValueRecieved;
        }
        public void OnDestroy()
        {
            Events.OnValueRecieved -= OnValueRecieved;
        }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            UnitAvatar networkAvatar = NetworkAvatar;
            //networkAvatar.OnStopBattle += OnStopBattle;
            networkAvatar.Inventory.OnItemUpdatedForServer += OnItemUpdatedForServer;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.Inventory.OnItemAddedForServer += OnItemAddedForServer;
            //networkAvatar.Inventory.OnItemAddedForClient += OnItemAddedForClient;
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{countView}/{countRequire}");
            //networkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        private void OnValueRecieved(string command, uint netId, int value)
        {
            if(netId == base.netId)
            {
                countView = value;
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            UnitAvatar networkAvatar = NetworkAvatar;
            networkAvatar.Inventory.OnItemAddedForServer -= OnItemAddedForServer;
            //networkAvatar.Inventory.OnItemAddedForClient -= OnItemAddedForClient;
            //networkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }

        private void OnItemUpdatedForServer()
        {
            power = NetworkAvatar.Inventory.charms.Values.Sum(charm => charm.CurrentLevelToIdx());
            //SaveItemOnServer(SaveManager.CurrentRun);
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar unit, DamageInstance damage)
        {
            if (damage.id == "Weapon_SpecialAttack" || damage.id == "Weapon_SpecialAttack_Amethyst")
            {
                damage.SetCustomColor(true, new Color(0.5f, 0, 0));
                damage.damage += power * damageByLevel.SafeRandomAccess(CurrentLevelToIdx());
            }
        }
        private void OnItemAddedForServer(NewItemOwnInstance item)
        {
            if ((bool)item.StoneTablet)
            {
                if(NetworkAvatar.GetCustomStatUnsafe("DISCONNECT") > 0 && (item.EntityID == 2049))// || item.EntityID == Data.Transcendent.Id
                {
                    return;
                }
                if ((bool)DungeonManager.Instance)
                {
                    DungeonManager.Instance.Chat(NetworkAvatar as PlayerAvatar, "Mod", "/stargaze");
                }
                using (new GridInventory.Permission(NetworkAvatar.Inventory))
                {
                    Inventory.ForceRemoveItem(item.XIdx, item.YIdx);
                }
                count = count + item.Entity.rarity switch
                {
                    EItemRarity.Common => 1,
                    EItemRarity.Uncommon => 2,
                    EItemRarity.Rare => 3,
                    EItemRarity.Legend => 5,
                    EItemRarity.Eternal => 10,
                    _ => 1
                };

                countView = count % countRequire;
                NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{countView}/{countRequire}");
                NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
                Events.CommandValue(NetworkAvatar, Item, count % countRequire);
                questCleared = count >= countRequire;
                SaveItemOnServer(SaveManager.CurrentRun);
            }
        }
        private void OnItemAddedForClient(NewItemOwnInstance item)
        {
            if ((bool)item.StoneTablet)
            {
                //UIManager.Instance.GetElement<UI_SystemMessage>().Open(notice.ToString(), 2.7f);
            }
        }

        private void OnStopBattle()
        {
            if (questCleared)
                return;
            count++;
            questCleared = count >= countRequire;
            NetworkAvatar.SetEffectHUDValue(GetCharmHUDID(), $"{count}/{countRequire}");
            NetworkAvatar.SetEffectHUDFlash(GetCharmHUDID());
            SaveItemOnServer(SaveManager.CurrentRun);
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            UnitAvatar networkAvatar = NetworkAvatar;
            //networkAvatar.OnStopBattle -= OnStopBattle;
            networkAvatar.Inventory.OnItemUpdatedForServer -= OnItemUpdatedForServer;
        }

        public override void SaveItemOnServer(ISaveData saveData)
        {
            base.SaveItemOnServer(saveData);
            saveData.SetInt($"CharmSaveData_StargazeTablet_{Item.InstanceID}_Stack", count);
        }

        public override void LoadItemOnServer(ISaveData saveData)
        {
            base.LoadItemOnServer(saveData);
            count = saveData.GetInt($"CharmSaveData_StargazeTablet_{Item.InstanceID}_Stack", 0);
            countView = count;
            Events.CommandValue(NetworkAvatar, Item, countView);
        }
    }
}
