using HarmonyLib;
using MiraItemMod.Registries;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static MiraItemMod.Registries.ModTreeShopItem;

namespace MiraItemMod.UI
{
    public class UI_AdditionalTreeShopPanel : MonoBehaviour
    {
        public static readonly Dictionary<ELinePos, int> LinePosExampleMap = new Dictionary<ELinePos, int>()
        {
            { ELinePos.Right, TreeShopItems.NewCharmBond1 },
            { ELinePos.Center, TreeShopItems.NewCharmMagic2 },
            { ELinePos.Left, TreeShopItems.NewCharmLeft1 },
            { ELinePos.RightUp, TreeShopItems.NewCharmLeft1 },
            { ELinePos.CenterUp, TreeShopItems.NewCharmMagic2 },
            { ELinePos.LeftUp, TreeShopItems.NewCharmBond1 }
        };
        public static readonly Dictionary<ELinePos, Vector2> LinePosOffsetMap = new Dictionary<ELinePos, Vector2>()
        {
            { ELinePos.Right, new Vector2(-30, -15) },
            { ELinePos.Center, new Vector2(0, -31) },
            { ELinePos.Left, new Vector2(30, -15) },
            { ELinePos.RightUp, new Vector2(-30, -15) },
            { ELinePos.CenterUp, new Vector2(0, 31) },
            { ELinePos.LeftUp, new Vector2(30, 15) }
        };
        public static readonly Dictionary<ELinePos, Vector2> LinePosNextMap = new Dictionary<ELinePos, Vector2>()
        {
            { ELinePos.Right, new Vector2(60, 31) },
            { ELinePos.Center, new Vector2(0, 62) },
            { ELinePos.Left, new Vector2(-60, 31) },
            { ELinePos.RightUp, new Vector2(60, -31) },
            { ELinePos.CenterUp, new Vector2(0, -62) },
            { ELinePos.LeftUp, new Vector2(-60, -31) }
        };
        public void Initialize(UI_TreeShopPanel treeShopPanel)
        {
            var examples = treeShopPanel.treeShopContents.GetComponentsInChildren<UI_TreeShopItem>(true);
            /*
            UI_TreeShopItem bond1 = null;
            UI_TreeShopItem magic2 = null;
            UI_TreeShopItem left1 = null;
            foreach (var example in examples)
            {
                if(example.connected == null)
                    continue;
                //Core.Logger("example: " + example.connected.id);
                //Core.Logger("example: " + (example.transform as RectTransform).anchoredPosition);
                if (example.connected.id == TreeShopItems.NewCharmBond1)
                {
                    bond1 = example;
                }
                if (example.connected.id == TreeShopItems.NewCharmMagic2)
                {
                    magic2 = example;
                }
                if (example.connected.id == TreeShopItems.NewCharmLeft1)
                {
                    left1 = example;
                }
            }
            if (bond1 != null)
            {
                CreateTreeShopItem(treeShopPanel, bond1, TreeShopItems.NewCharmBond2, 302 + 60, 750 + 31, ELinePos.Left);
            }
            if (magic2 != null)
            {
                CreateTreeShopItem(treeShopPanel, magic2, TreeShopItems.NewCharmDrunk, 302, 750 + 31 * 2, ELinePos.Center);
            }
            if (bond1 != null)
            {
                CreateTreeShopItem(treeShopPanel, bond1, TreeShopItems.NewCharmSacrifice, 302 + 60 * 2, 750 - 31 * 2, ELinePos.Left);
            }*/
            foreach(var item in Data.TreeShops)
            {
                CreateTreeShopItem(treeShopPanel, item, examples);
            }
        }
        public void CreateTreeShopItem(UI_TreeShopPanel treeShopPanel, ModTreeShopItem item, UI_TreeShopItem[] examples)
        {
            var original = examples.FirstOrDefault(x => x.connected != null && x.connected.id == LinePosExampleMap[item.LinePos]);
            var dependency = examples.FirstOrDefault(x => x.connected != null && x.connected.id == item.Dependency);
            if (original == null || dependency == null)
                return;
            var pos = (dependency.transform as RectTransform).anchoredPosition;
            pos += LinePosNextMap[item.LinePos];
            CreateTreeShopItem(treeShopPanel, original, item.Id, pos.x, pos.y, item.LinePos);
        }
        public void CreateTreeShopItem(UI_TreeShopPanel treeShopPanel, UI_TreeShopItem original, int id, float x, float y, ELinePos pos)
        {
            var item = Instantiate(original, treeShopPanel.treeShopContents);
            var transform = item.transform as RectTransform;
            transform.anchoredPosition = new Vector2(x, y);//1x = x 60, y 31
            Core.Logger("item: " + transform.anchoredPosition);
            item.connected = TreeShopItemDatabase.FindById(id);
            Core.Logger("item: " + item.connected);
            var lines = item.prevLines.Select(x => Instantiate(x, x.transform.parent)).ToArray();
            item.prevLines = lines;
            foreach (var line in item.prevLines)
            {
                var rect = line.transform as RectTransform;
                rect.anchoredPosition = new Vector2(x, y) + LinePosOffsetMap[pos];
                //rect.SetAsFirstSibling();
            }
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
