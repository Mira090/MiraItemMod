using HarmonyLib;
using MiraItemMod.Entities;
using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MiraItemMod.UI
{
    public class UI_ModJournalPanel : MonoBehaviour
    {
        public static string ArmamentSearchText => "<color=white>" + new LocalizedString("Status_ItemRarity_Armament_Name").ToString();
        public static UI_ModJournalPanel Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// original journal panel
        /// </summary>
        public UI_JournalPanel JournalPanel;
        public UI_JournalContent_Item JournalItem;
        public GameObject TabArea;
        public RectTransform TabAreaRect;


        public UI_JournalPanel_SearchOptionButton ArmamentButton;
        public static bool HasFilterByArmament = false;

        /// <summary>
        /// for UnityExplorer
        /// </summary>
        public string KeywordsString => string.Join("\n", KeywordOptions);
        public List<string> KeywordOptions = new List<string>();

        public static string SelectedKeyword = null;
        /// <summary>
        /// init original journal panel and get necessary references for later use. This should be called right after adding this component to the journal panel.
        /// </summary>
        /// <param name="panel"></param>
        public void Init(UI_JournalPanel panel)
        {
            JournalPanel = panel;
            TabArea = JournalPanel.transform.GetChild(JournalPanel.transform.childCount - 1).gameObject;
            TabAreaRect = TabArea.transform as RectTransform;
        }
        /// <summary>
        /// create gameobjects for keyword search and buttons, and set up their positions and callbacks.
        /// </summary>
        private void Start()
        {
            try
            {
                JournalItem = TabAreaRect.GetChild(0).GetComponent<UI_JournalContent_Item>();

                var scrollOriginal = JournalItem.transform.GetChild(1) as RectTransform;


                var optionButtons = scrollOriginal.transform.GetChild(0).GetChild(1).gameObject;

                ArmamentButton = Instantiate(JournalItem.searchOptionButtonPrefab, optionButtons.transform);
                ArmamentButton.Initialize(ArmamentSearchText, string.Empty, button => OnButtonClick(button), new Color32(100, 130, 160, 255));

                GameObject breakObject = null;
                for (int q = 0; q < optionButtons.transform.childCount; q++)
                {
                    var child = optionButtons.transform.GetChild(q);
                    //Debug.Log("button: " + child.name);
                    if (child.name == "SectorLine")
                    {
                        ArmamentButton.transform.SetSiblingIndex(q + 2);
                        breakObject = child.gameObject;
                    }
                }
            }
            catch (Exception e)
            {
                Core.LoggerError(e);
            }
        }
        private void OnEnable()
        {
            LocalizationManager.Instance.OnLanguageChanged += HandleLanguageChanged;
            HandleLanguageChanged(LocalizationManager.Instance.CurrentLanguage);
        }

        private void OnDisable()
        {
            LocalizationManager.Instance.OnLanguageChanged -= HandleLanguageChanged;
        }

        private void HandleLanguageChanged(string language)
        {
            if (ArmamentButton == null)
                return;
            ArmamentButton.UpdateShowText(ArmamentSearchText);
        }
        /// <summary>
        /// armament button callback. Set selected keyword and refresh journal item list. Also update button states and texts.
        /// </summary>
        /// <param name="button"></param>
        private void OnButtonClick(UI_JournalPanel_SearchOptionButton button)
        {
            HasFilterByArmament = !HasFilterByArmament;
            button.SetSelected(HasFilterByArmament);
            JournalItem.RefreshItems();
        }

        /// <summary>
        /// Patches for initialization and journal closure
        /// </summary>
        [HarmonyPatch]
        public static class JournalPanelPatch
        {
            [HarmonyPatch(typeof(UI_JournalPanel), nameof(UI_JournalPanel.Connect))]
            [HarmonyPostfix]
            static void ConnectPostfix(UI_JournalPanel __instance)
            {
                if (__instance.gameObject.TryGetComponent<UI_ModJournalPanel>(out _))
                    return;

                try
                {
                    __instance.gameObject.AddComponent<UI_ModJournalPanel>().Init(__instance);
                }
                catch(Exception e)
                {
                    Core.LoggerError(e);
                }
            }
        }
        /// <summary>
        /// Filtering Patch
        /// </summary>
        [HarmonyPatch(typeof(UI_JournalContent_Item), nameof(UI_JournalContent_Item.RefreshItems))]
        public static class RefleshPatch
        {
            static void FilterByArmament(UI_JournalContent_Item __instance, PlayerAvatar player)
            {
                var icons = __instance.GetIcons();
                var list = new List<UI_ItemIcon>();


                foreach (var icon in icons)
                {
                    var entity = ItemDatabase.FindItemById(icon.Item.entityID);
                    if (entity == null || entity.type != EItemType.Charm)
                    {
                        Destroy(icon.gameObject);
                        continue;
                    }
                    if (!entity.resourcePrefab.TryGetComponent<Charm_Basic>(out var charm))
                    {
                        Destroy(icon.gameObject);
                        continue;
                    }
                    if (entity is ItemEntity_Armament)
                    {
                        list.Add(icon);
                    }
                    else
                    {
                        Destroy(icon.gameObject);
                    }
                }

                __instance.SetIcons(list);
            }
            static void Postfix(UI_JournalContent_Item __instance, EItemCategory category)
            {
                if (category != EItemCategory.Charm)
                    return;
                if (!HasFilterByArmament)
                    return;
                if (!NetworkClient.localPlayer.TryGetComponent<PlayerAvatar>(out var player))
                    return;

                FilterByArmament(__instance, player);
            }
        }
    }
}
