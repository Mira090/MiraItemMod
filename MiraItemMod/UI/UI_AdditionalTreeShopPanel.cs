using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.UI
{
    public class UI_AdditionalTreeShopPanel : MonoBehaviour
    {
        public void Initialize(UI_TreeShopPanel treeShopPanel)
        {
            var examples = treeShopPanel.treeShopContents.GetComponentsInChildren<UI_TreeShopItem>(true);
            foreach(var example in examples)
            {
                Core.Logger("example: " + example.connected.id);
                Core.Logger("example: " + (example.transform as RectTransform).anchoredPosition);
            }

            var ex = treeShopPanel.treeShopContents.GetComponentInChildren<UI_TreeShopItem>(true);
            CreateTreeShopItem(treeShopPanel, ex, TreeShopItems.NewCharmBond2, 302 + 60, 750 + 31);
            CreateTreeShopItem(treeShopPanel, ex, TreeShopItems.NewCharmDrunk, 302, 750 + 31 * 2);
        }
        public void CreateTreeShopItem(UI_TreeShopPanel treeShopPanel, UI_TreeShopItem original, int id, float x, float y)
        {
            var item = Instantiate(original, treeShopPanel.treeShopContents);
            var transform = item.transform as RectTransform;
            transform.anchoredPosition = new Vector2(x, y);//1x = x 60, y 31
            Core.Logger("item: " + transform.anchoredPosition);
            item.connected = TreeShopItemDatabase.FindById(id);
            Core.Logger("item: " + item.connected);
        }

        [HarmonyPatch(typeof(UI_TreeShopPanel), nameof(UI_TreeShopPanel.Connect))]
        public static class UI_TreeShopPanelPatch
        {
            static void Postfix(UI_TreeShopPanel __instance)
            {
                if(!__instance.gameObject.TryGetComponent<UI_AdditionalTreeShopPanel>(out var _))
                {
                    __instance.gameObject.AddComponent<UI_AdditionalTreeShopPanel>().Initialize(__instance);
                }
            }
        }
    }
}
