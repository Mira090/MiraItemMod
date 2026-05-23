using HarmonyLib;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace SephiriaMod.Entities
{
    public class ItemEntity_Jewelry : ItemEntity
    {

        [HarmonyPatch(typeof(UI_CharmTooltip), ("UpdateData"))]
        public static class CharmTooltipPatch
        {
            static void Postfix(UI_CharmTooltip __instance, ITooltip data, int virtualLevelOffset)
            {
                ItemEntity itemEntity = null;
                if (data is ItemEntity itemEntity3)
                {
                    itemEntity = itemEntity3;
                }
                else if (data is IItemEntity itemEntity2)
                {
                    itemEntity = ItemDatabase.FindItemById(itemEntity2.IEntityID);
                }
                if (data is NewItemOwnInstance item && item.Entity is ItemEntity_Jewelry jewelry)
                {
                    Modify(__instance, jewelry);
                }
                else if(itemEntity is ItemEntity_Jewelry jewelry2)
                {
                    Modify(__instance, jewelry2);
                }
            }
            static void Modify(UI_CharmTooltip __instance, ItemEntity_Jewelry jewelry)
            {
                KeywordEntity keyword = KeywordDatabase.GetEntity("ItemRarity_Jewelry");
                if (keyword == null)
                {
                    if (Core.LogFew)
                        Core.LoggerWarning("ItemRarity_Jewelry does not found!");
                    return;
                }

                Color colorViaItemRarity = ItemDatabase.GetColorViaItemRarity(jewelry.rarity);
                VertexGradient colorGradient = new VertexGradient(colorViaItemRarity, colorViaItemRarity, keyword.textColor, keyword.textColor);
                var nameText = __instance.GetNameText();
                nameText.color = Color.white;
                nameText.colorGradient = colorGradient;
                nameText.enableVertexGradient = true;

                var typeText = __instance.GetTypeText();
                if (!typeText.gameObject.activeSelf)
                    return;

                string text13 = "";
                Charm_Basic component5 = jewelry.resourcePrefab.GetComponent<Charm_Basic>();
                if (component5)
                {
                    if (component5 is Charm_Magic)
                    {
                        text13 = "(" + Loc._("ItemType_Charm_Magic") + ")";
                    }
                    else if (component5 is Charm_WeaponSkill)
                    {
                        text13 = "(" + Loc._("ItemType_Charm_WeaponSkill") + ")";
                    }
                }

                string text14 = Loc._("#SeparatorSpace");
                string text15 = keyword.Convert(true, false);
                string text16 = ItemDatabase.GetItemRarityName(jewelry.rarity).ToString();
                string text17 = ItemDatabase.GetItemTypeName(jewelry.type).ToString();
                bool flag4 = LocalizationManager.Instance.CurrentLanguage == "pt-BR";


                if (flag4)
                {
                    typeText.text = string.Concat(new string[]
                    {
                            text17,
                            text14,
                            text15,
                            text14,
                            text16,
                            text13
                    });
                }
                else
                {
                    typeText.text = string.Concat(new string[]
                    {
                            text16,
                            text14,
                            text15,
                            text14,
                            text17,
                            text13
                    });
                }
            }
        }
    }
}
