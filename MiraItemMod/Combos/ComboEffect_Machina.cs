using HarmonyLib;
using MiraItemMod.Items.Machina;
using MiraItemMod.Sephirites;
using MiraItemMod.Utilities;
using Mirror;
using Mirror.RemoteCalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MiraItemMod.Combos
{
    public class ComboEffect_Machina : ComboEffectBase
    {
        public static bool HasSlotInitizalized(GridInventory inventory)
        {
            return inventory.mysticPositions.Count > 0;
        }
        public static int GetMachinaSlot(GridInventory inventory)
        {
            if (inventory.mysticPositions.Count == 0)
                return 0;
            return Mathf.RoundToInt((float)inventory.mysticPositions.Average(x => inventory.PosToIdx(x)));
        }
        public static int GetDemolitionSlot(GridInventory inventory)
        {
            if (inventory.mysticPositions.Count == 0)
                return 0;
            return inventory.PosToIdx(inventory.mysticPositions.LastOrDefault());
        }

        public Charm_Basic GetDemolitionCharm()
        {
            if (Networkavatar == null || Networkavatar.Inventory == null)
                return null;
            int demolitionSlot = GetDemolitionSlot(Networkavatar.Inventory);
            ItemPosition pos = Networkavatar.Inventory.IdxToPos(demolitionSlot);
            if (!Networkavatar.Inventory.charms.ContainsKey(pos))
                return null;
            return Networkavatar.Inventory.charms[pos];
        }
        public Charm_Basic GetMachinaCharm()
        {
            if (Networkavatar == null || Networkavatar.Inventory == null)
                return null;
            int machinaSlot = GetMachinaSlot(Networkavatar.Inventory);
            ItemPosition pos = Networkavatar.Inventory.IdxToPos(machinaSlot);
            if (!Networkavatar.Inventory.charms.ContainsKey(pos))
                return null;
            return Networkavatar.Inventory.charms[pos];
        }
        private ItemPosition _demolitionSlot;
        private ItemPosition _machinaSlot;

        public int slotActivateComboCount = 2;

        public LocalizedString debuffActivateEffectNameString = new LocalizedString("ComboEffect_MachinaActivateName");

        public LocalizedString debuffActivateEffectString = new LocalizedString("ComboEffect_MachinaActivate");

        public override Loc.KeywordValue[] BuildDefaultEffectKeyword()
        {
            return new Loc.KeywordValue[]
            {
            };
        }
        protected override void OnRequestComboData(UnitAvatar avatar, List<ComboEffectElement> elements)
        {
            base.OnRequestComboData(avatar, elements);
            Loc.KeywordValue[] keywordValues = new Loc.KeywordValue[]
            {
            };
            elements.Add(new ComboEffectElement
            {
                comboCount = slotActivateComboCount,
                effectName = KeywordDatabase.Convert(Loc.Convert(debuffActivateEffectString.ToString(), keywordValues), useColor: false)
            });
        }
        protected override int OnEnableEffect(int comboCount, int oldComboCount)
        {
            Networkavatar.Inventory.GenerateServerMysticPositions();

            int result = base.OnEnableEffect(comboCount, oldComboCount);
            if (comboCount >= slotActivateComboCount)
            {
                result = slotActivateComboCount;
                RpcSetGear(true, false);
                RpcSetGear(true, true);
                Networkavatar.OnEndSpawnerBattle += OnEndSpawnerBattle;
            }
            else if (comboCount < slotActivateComboCount && oldComboCount >= slotActivateComboCount)
            {
                RpcSetGear(false, false);
                RpcSetGear(false, true);
                Networkavatar.OnEndSpawnerBattle -= OnEndSpawnerBattle;
            }

            return result;
        }

        protected override void OnDisableEffect()
        {
            base.OnDisableEffect();
            RpcSetGear(false, false);
            RpcSetGear(false, true);
            Networkavatar.OnEndSpawnerBattle -= OnEndSpawnerBattle;
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            if (isOwned)
            {
                ClearGear();
            }
        }

        private void OnEndSpawnerBattle()
        {
            if (comboCount < slotActivateComboCount)
                return;
            var demolitionCharm = GetDemolitionCharm();
            var machinaCharm = GetMachinaCharm();
            if (demolitionCharm == null)
                return;
            if (machinaCharm != null && machinaCharm is Charm_MachinaBasic machina && machina.maxLevel < machina.ValiableMax)
            {
                machina.Repair(1 + Mathf.Max(0, Networkavatar.GetCustomStatUnsafe("ADDITIONALREPAIR")));
            }
            else
            {
                return;
            }

            if (demolitionCharm is Charm_MachinaBasic dismantled)
            {
                dismantled.Dismantle();
            }
            else
            {
                using (new GridInventory.Permission(Networkavatar.Inventory))
                {
                    Networkavatar.Inventory.ForceRemoveItem(demolitionCharm.Item.Position);
                }
            }
        }

        //[HarmonyPatch(typeof(GridInventory), nameof(GridInventory.LocalRemoveDestructibleItems))]
        [Obsolete]
        public static class RemoveDestructibleItemsPatch
        {
            static void Prefix(GridInventory __instance)
            {
                try
                {
                    var combo = __instance.FindComboEffect(ItemCategories.Machina);
                    if (combo == null)
                        return;
                    if (combo is ComboEffect_Machina machina)
                    {
                        machina.RpcSetGear(false, false);
                    }
                }
                catch (Exception e)
                {
                    Core.LoggerError(e);
                }
            }
        }
        //[HarmonyPatch(typeof(GridInventory), nameof(GridInventory.ForceRemoveAll))]
        [Obsolete]
        public static class RestartPatch
        {
            static void Prefix(GridInventory __instance)
            {
                try
                {
                    var combo = __instance.FindComboEffect(ItemCategories.Machina);
                    if (combo == null)
                        return;
                    if (combo is ComboEffect_Machina machina)
                    {
                        machina.RpcSetGear(false, false);
                    }
                }
                catch(Exception e)
                {
                    Core.LoggerError(e);
                }
            }
        }

        [HarmonyPatch(typeof(UI_NewInventoryIcon), "Start")]
        public static class NewInventoryIconPatch
        {
            public static readonly string GearObjectName = "Gear";
            public static Sprite Gear1Sprite;
            public static Sprite Gear2Sprite;
            static void Postfix(UI_NewInventoryIcon __instance)
            {
                if(Gear1Sprite == null)
                {
                    Gear1Sprite = AssetLoader.LoadSprite(ModUtil.UIPath + "Gear1");
                }
                if(Gear2Sprite == null)
                {
                    Gear2Sprite = AssetLoader.LoadSprite(ModUtil.UIPath + "Gear2");
                }
                if (!HasGearImage(__instance))
                {
                    Patch(__instance);
                }
            }
            private static bool HasGearImage(UI_NewInventoryIcon __instance)
            {
                for (int q = 0; q < __instance.transform.childCount; q++)
                {
                    if (__instance.transform.GetChild(q).name == GearObjectName)
                        return true;
                }
                return false;
            }
            private static void Patch(UI_NewInventoryIcon __instance)
            {
                var gameObject = new GameObject(GearObjectName);
                gameObject.transform.SetParent(__instance.transform);
                gameObject.transform.SetAsFirstSibling();
                var rect = gameObject.AddComponent<RectTransform>();
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 48 * 4);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 48 * 4);
                var image = gameObject.AddComponent<Image>();
                image.sprite = Gear1Sprite;
                image.raycastTarget = false;
                var animation = gameObject.AddComponent<SimpleRotateAnimation>();
                animation.speed = 24f;

                gameObject.SetActive(false);
            }
        }

        public Sprite NormalSlotSprite;
        public Sprite MachinaSlotSprite;
        public Sprite DemolitionSlotSprite;
        private void Awake()
        {
            NormalSlotSprite = AssetLoader.LoadSprite(ModUtil.UIPath + "InventorySlot0");
            MachinaSlotSprite = AssetLoader.LoadSprite(ModUtil.UIPath + "InventorySlot0_Machina");
            DemolitionSlotSprite = AssetLoader.LoadSprite(ModUtil.UIPath + "InventorySlot0_Demolition");
        }
        public void EnableGear(UI_NewInventoryIcon icon, bool isDemolition)
        {
            try
            {
                if (icon.transform.Find(NewInventoryIconPatch.GearObjectName) is Transform gear)
                {
                    gear.gameObject.SetActive(true);
                    if(gear.TryGetComponent<Image>(out var image))
                    {
                        image.sprite = isDemolition ? NewInventoryIconPatch.Gear2Sprite : NewInventoryIconPatch.Gear1Sprite;
                    }
                }
                icon.defaultBGSprite = isDemolition ? DemolitionSlotSprite : MachinaSlotSprite;
                icon.UpdateIcon();
                icon.transform.SetAsLastSibling();
            }
            catch(Exception e)
            {
                Core.LoggerError($"Error enabling gear image for icon {icon.name}: {e.Message}");
            }
        }
        public void DisableGear(UI_NewInventoryIcon icon)
        {
            try
            {
                if (icon.transform.Find(NewInventoryIconPatch.GearObjectName) is Transform gear)
                {
                    gear.gameObject.SetActive(false);
                }
                icon.defaultBGSprite = NormalSlotSprite;
                icon.UpdateIcon();
            }
            catch (Exception e)
            {
                Core.LoggerError($"Error disabling gear image for icon {icon.name}: {e.Message}");
            }
        }

        [ClientRpc]
        public void RpcSetGear(bool enabled, bool isDemolition)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteBool(enabled);
            writer.WriteBool(isDemolition);
            var func = "System.Void ComboEffect_Machina::RpcSetGear(System.Boolean,System.Boolean)";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }
        protected virtual void UserCode_RpcSetGear(bool enabled, bool isDemolition)
        {
            if (UIManager.Instance == null)
                return;
            UI_CharacterStatusPanel element = UIManager.Instance.GetElement<UI_CharacterStatusPanel>();
            if (element == null)
                return;
            Core.LoggerFew("SetGear");
            if (Networkavatar == null || Networkavatar.Inventory == null)
                return;
            if (!HasSlotInitizalized(Networkavatar.Inventory))
                return;
            ItemPosition position = Networkavatar.Inventory.IdxToPos(isDemolition ? GetDemolitionSlot(Networkavatar.Inventory) : GetMachinaSlot(Networkavatar.Inventory));
            if (isDemolition)
                _demolitionSlot = position;
            else
                _machinaSlot = position;
            UI_NewInventoryIcon itemIcon = element.GetItemIcon(position);
            if (itemIcon)
            {
                if (enabled)
                {
                    Core.LoggerFew("EnableGear");
                    EnableGear(itemIcon, isDemolition);
                }
                else
                {
                    Core.LoggerFew("DisableGear");
                    DisableGear(itemIcon);
                }
            }
            else
            {
                Core.LoggerError($"Could not find item icon for position {position} when trying to {(enabled ? "enable" : "disable")} gear image.");
            }
        }
        protected virtual void ClearGear()
        {
            if (UIManager.Instance == null)
                return;
            UI_CharacterStatusPanel element = UIManager.Instance.GetElement<UI_CharacterStatusPanel>();
            if (element == null)
                return;
            UI_NewInventoryIcon demolitionIcon = element.GetItemIcon(_demolitionSlot);
            UI_NewInventoryIcon machinaIcon = element.GetItemIcon(_machinaSlot);
            if (demolitionIcon)
            {
                Core.LoggerFew("DisableGear");
                DisableGear(demolitionIcon);
            }
            else
            {
                Core.LoggerError($"Could not find item icon for position {_demolitionSlot} when trying to disable gear image.");
            }
            if (machinaIcon)
            {
                Core.LoggerFew("DisableGear");
                DisableGear(machinaIcon);
            }
            else
            {
                Core.LoggerError($"Could not find item icon for position {_machinaSlot} when trying to disable gear image.");
            }
        }

        protected static void InvokeUserCode_RpcSetGear(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcSetGear called on server.");
            }
            else
            {
                ((ComboEffect_Machina)obj).UserCode_RpcSetGear(reader.ReadBool(), reader.ReadBool());
            }
        }

        static ComboEffect_Machina()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(ComboEffect_Machina), "System.Void ComboEffect_Machina::RpcSetGear(System.Boolean,System.Boolean)", InvokeUserCode_RpcSetGear);
        }
    }
}
