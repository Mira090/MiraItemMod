using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MiraItemMod
{
    public enum ECustomItemRarity
    {
        Sacrifice = -1,
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Legend = 3,
        Eternal = 4,
    }
    public static class CustomItemRarityPatches
    {
        public static readonly bool HasSacrificeTablet = false;
        public static EItemRarity ToSephiria(this ECustomItemRarity rarity)
        {
            return (EItemRarity)rarity;
        }
        public static string GetRarityName(this ECustomItemRarity rarity)
        {
            return rarity switch
            {
                ECustomItemRarity.Sacrifice => "Sacrifice",
                ECustomItemRarity.Common => "Common",
                ECustomItemRarity.Uncommon => "Uncommon",
                ECustomItemRarity.Rare => "Rare",
                ECustomItemRarity.Legend => "Legend",
                ECustomItemRarity.Eternal => "Eternal",
                _ => "Unknown"
            };
        }
        [HarmonyPatch(typeof(ItemDatabase), nameof(ItemDatabase.Initialize))]
        public static class ItemDatabaseInitializePatch
        {
            public static void Postfix()
            {
                var colors = ReflectionExtensions.GetItemColorByRarity();
                var rarityNames = ReflectionExtensions.GetRarityNames();

                colors[ECustomItemRarity.Sacrifice.ToSephiria()] = new Color(0.75f, 0, 0);
                rarityNames[ECustomItemRarity.Sacrifice.ToSephiria()] = new LocalizedString("ItemRarity_Sacrifice");
            }
        }
        [HarmonyPatch(typeof(UI_DimensionPocketPanel), "Awake")]
        public static class UI_DimensionPocketPanelAwakePatch
        {
            public static void Prefix(UI_DimensionPocketPanel __instance)
            {
                EItemRarity rarity = ECustomItemRarity.Sacrifice.ToSephiria();
                UI_JournalPanel_SearchOptionButton ui_JournalPanel_SearchOptionButton = GameObject.Instantiate<UI_JournalPanel_SearchOptionButton>(__instance.searchOptionButtonPrefab, __instance.searchOptionButtonContainer);
                UI_JournalPanel_SearchOptionButton ui_JournalPanel_SearchOptionButton2 = ui_JournalPanel_SearchOptionButton;
                string showText = ItemDatabase.GetItemRarityName(rarity).ToString();
                string data = rarity.ToString();
                Action<UI_JournalPanel_SearchOptionButton> onButtonClick = new Action<UI_JournalPanel_SearchOptionButton>(__instance.SelectRarity);
                ui_JournalPanel_SearchOptionButton2.Initialize(showText, data, onButtonClick, new Color(0.75f, 0, 0), null);
                __instance.GetRarityOptionButtons().Add(ui_JournalPanel_SearchOptionButton);
            }
        }
        [HarmonyPatch(typeof(UI_DimensionPocketPanel), nameof(UI_DimensionPocketPanel.GetCapacity), new Type[] { typeof(EItemRarity) })]
        public static class UI_DimensionPocketPanelGetCapacityPatch
        {
            public static void Postfix(EItemRarity rarity, ref int __result)
            {
                if(rarity == ECustomItemRarity.Sacrifice.ToSephiria())
                {
                    __result = 0;
                }
            }
        }
        [HarmonyPatch(typeof(UI_JournalContent_Item), nameof(UI_JournalContent_Item.Initialize))]
        public static class UI_JournalContent_ItemInitializePatch
        {
            public static void Prefix(UI_JournalContent_Item __instance)
            {
                EItemRarity value = ECustomItemRarity.Sacrifice.ToSephiria();
                UI_JournalPanel_SearchOptionButton uI_JournalPanel_SearchOptionButton = UnityEngine.Object.Instantiate(__instance.searchOptionButtonPrefab, __instance.searchOptionButtonContainer);
                UI_JournalPanel_SearchOptionButton uI_JournalPanel_SearchOptionButton2 = uI_JournalPanel_SearchOptionButton;
                string showText = ItemDatabase.GetItemRarityName(value).ToString();
                string data = value.ToString();
                Action<UI_JournalPanel_SearchOptionButton> onButtonClick = __instance.SelectRarity;
                uI_JournalPanel_SearchOptionButton2.Initialize(showText, data, onButtonClick, new Color(0.75f, 0, 0));
                __instance.GetRarityOptionButtons().Add(uI_JournalPanel_SearchOptionButton);
            }
        }
        [HarmonyPatch(typeof(UI_ItemIcon), nameof(UI_ItemIcon.UpdateRarityBG))]
        public static class UI_ItemIconUpdateRarityBGPatch
        {
            public static Sprite SacrificeBGSprite;
            public static void Postfix(UI_ItemIcon __instance)
            {
                if(SacrificeBGSprite == null)
                {
                    SacrificeBGSprite = AssetLoader.LoadSprite(ModUtil.UIPath + "InventorySlot_New1_1Sacrifice");
                }

                ItemEntity itemEntity = ItemDatabase.FindItemById(__instance.Item.entityID);
                if (itemEntity == null)
                    return;

                if (itemEntity.rarity == ECustomItemRarity.Sacrifice.ToSephiria())
                {
                    Image image = __instance.bgImage;
                    image.sprite = SacrificeBGSprite;
                }
            }
        }
        [HarmonyPatch(typeof(UI_NewInventoryIcon), nameof(UI_NewInventoryIcon.UpdateIcon))]
        public static class UI_NewInventoryIconUpdateIconPatch
        {
            public static void Postfix(UI_NewInventoryIcon __instance)
            {
                if (UI_ItemIconUpdateRarityBGPatch.SacrificeBGSprite == null)
                {
                    UI_ItemIconUpdateRarityBGPatch.SacrificeBGSprite = AssetLoader.LoadSprite(ModUtil.UIPath + "InventorySlot_New1_1Sacrifice");
                }

                NewItemOwnInstance newItemOwnInstance = __instance.Inventory.FindItem(__instance.X, __instance.Y);
                if (newItemOwnInstance == null)
                    return;
                ItemEntity itemEntity = ItemDatabase.FindItemById(newItemOwnInstance.EntityID);
                if (itemEntity == null)
                    return;

                if (itemEntity.rarity == ECustomItemRarity.Sacrifice.ToSephiria())
                {
                    Image image = __instance.bgImage;
                    image.sprite = UI_ItemIconUpdateRarityBGPatch.SacrificeBGSprite;
                }
            }
        }
        [HarmonyPatch(typeof(UI_DimensionVaultItemIcon), nameof(UI_DimensionVaultItemIcon.SetCost))]
        public static class UI_DimensionVaultItemIconSetCostPatch
        {
            public static Sprite SacrificeBGSprite;
            public static Sprite SacrificeBGSpriteSelected;
            public static Sprite DefaultBGSprite;
            public static Sprite DefaultBGSpriteSelected;
            public static void Postfix(EItemRarity rarity, UI_DimensionVaultItemIcon __instance)
            {
                if (SacrificeBGSprite == null)
                {
                    SacrificeBGSprite = AssetLoader.LoadSprite(ModUtil.UIPath + "InventorySlot0_Sacrifice");
                }
                if (SacrificeBGSpriteSelected == null)
                {
                    SacrificeBGSpriteSelected = AssetLoader.LoadSprite(ModUtil.UIPath + "InventorySlot1_Sacrifice");
                }
                if (DefaultBGSprite == null && __instance.bgImage != SacrificeBGSprite)
                {
                    DefaultBGSprite = __instance.bgImage.sprite;
                }
                if (DefaultBGSpriteSelected == null && __instance.bgImage != SacrificeBGSprite)
                {
                    DefaultBGSpriteSelected = __instance.horayButton.spriteState.selectedSprite;
                }

                if (rarity == ECustomItemRarity.Sacrifice.ToSephiria())
                {
                    __instance.costImage.rectTransform.sizeDelta = new Vector2(0, __instance.costImage.rectTransform.sizeDelta.y);
                    __instance.bgImage.sprite = SacrificeBGSprite;
                    __instance.horayButton.spriteState = new SpriteState
                    {
                        highlightedSprite = SacrificeBGSprite,
                        pressedSprite = SacrificeBGSprite,
                        disabledSprite = SacrificeBGSprite,
                        selectedSprite = SacrificeBGSpriteSelected
                    };
                }
                else if (__instance.bgImage.sprite == SacrificeBGSprite)
                {
                    __instance.bgImage.sprite = DefaultBGSprite;
                    __instance.horayButton.spriteState = new SpriteState
                    {
                        highlightedSprite = DefaultBGSprite,
                        pressedSprite = DefaultBGSprite,
                        disabledSprite = DefaultBGSprite,
                        selectedSprite = DefaultBGSpriteSelected
                    };
                }
            }
        }
        [HarmonyPatch(typeof(UI_MysticPotPanel), "UpdateSelectedItems")]
        public static class UI_MysticPotPanelUpdateSelectedItemsPatch
        {
            public static void Postfix(UI_MysticPotPanel __instance)
            {
                if (__instance.GetConnectedPot().SelectedItems.Count < 1)
                    return;
                if (__instance.GetConnectedPot().SelectedItems[0].Entity.rarity == ECustomItemRarity.Sacrifice.ToSephiria())
                {
                    if(__instance.GetConnectedPot().SelectedItems.Count >= 3 && HasSacrificeTablet)
                    {
                        Color color = __instance.rarityScreenEffectImage.color;
                        color.a = __instance.screenEffectAlpha;
                        __instance.rarityScreenEffectImage.color = color;
                        __instance.rarityEffectCanvasGroup.alpha = 1f;
                        __instance.rarityEffectTransform.localScale = new Vector3(1f, __instance.rarityEffectScaleMax, 1f);
                        __instance.targetRarity = ECustomItemRarity.Sacrifice.ToSephiria();
                    }
                    else
                    {
                        __instance.SetIsReadyToMix(false);
                        __instance.previewGroup.SetActive(false);
                        __instance.mixButton.interactable = false;
                    }
                }
            }
        }
    }
}
