using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static Miracle;

namespace MiraItemMod.Registries
{
    public class ModEffectHUD : IDisposable
    {
        /// <summary>
        /// テキストに「数値/数値」と書く場合
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hasStackText"></param>
        /// <returns></returns>
        public static ModEffectHUD CreateStackEffectHUD(string name, bool hasStackText = true, Image.FillMethod fill = Image.FillMethod.Radial360)
        {
            var hud = new ModEffectHUD();
            hud.Type = EffectHUDType.Stack;
            hud.EffectType = UI_EffectHUD_Basic.EEffectType.Boon;
            hud.Name = name;
            hud.Id = name.ToSephiriaUpperId();
            hud.LocalizedName = new LocalizedString("EffectHUD_" + name + "_Name");
            hud.FlavorText = new LocalizedString("EffectHUD_" + name + "_FlavorText");
            hud.IconFileName = ModUtil.EffectHUDPath + name;
            hud.HasStackText = hasStackText;
            hud.FillMethod = fill;
            return hud;
        }
        /// <summary>
        /// テキストに「数値」と書く場合
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hasStackText"></param>
        /// <returns></returns>
        public static ModEffectHUD CreateBuffEffectHUD(string name, bool hasStackText = true, Image.FillMethod fill = Image.FillMethod.Vertical)
        {
            var hud = new ModEffectHUD();
            hud.Type = EffectHUDType.Buff;
            hud.EffectType = UI_EffectHUD_Basic.EEffectType.Condition;
            hud.Name = name;
            hud.Id = name.ToSephiriaUpperId();
            hud.LocalizedName = new LocalizedString("EffectHUD_" + name + "_Name");
            hud.FlavorText = new LocalizedString("EffectHUD_" + name + "_FlavorText");
            hud.IconFileName = ModUtil.EffectHUDPath + name;
            hud.HasStackText = hasStackText;
            hud.FillMethod = fill;
            return hud;
        }
        public EffectHUDType Type { get; internal set; }
        public string Name { get; internal set; }
        public string Id { get; internal set; }
        public Sprite Icon { get; internal set; }
        public string IconFileName { get; internal set; }
        public UI_EffectHUD_Basic.EEffectType EffectType { get; internal set; }
        public LocalizedString LocalizedName { get; internal set; }
        public LocalizedString FlavorText {  get; internal set; }
        public bool HasStackText { get; internal set; }
        public Image.FillMethod FillMethod { get; internal set; }
        public GameObject ResourcePrefab { get; internal set; }
        public void SetResourcePrefab(GameObject prefab)
        {
            //Core.Logger("SetResourcePrefab: " + LocalizedName.ToString());
            var basic = prefab.GetComponent<UI_EffectHUD_Basic>();
            basic.effectName = LocalizedName;
            basic.effectFlavorText = FlavorText;
            basic.effectType = EffectType;
            if(basic is UI_EffectHUD_Stack stack)
            {
                stack.stackText.gameObject.SetActive(HasStackText);
                if(stack.fillImage != null)
                {
                    stack.fillImage.fillMethod = FillMethod;
                }
            }
            if(basic is UI_BuffHUD buff)
            {
                if (!HasStackText)
                {
                    UnityEngine.Object.Destroy(buff.stackText.gameObject);
                }
            }
            var rect = prefab.transform as RectTransform;
            var icon = rect.GetChild(1).GetComponent<Image>();//Icon
            icon.sprite = (Icon ?? AssetLoader.LoadSprite(IconFileName)) ?? AssetLoader.LoadSprite(ModUtil.EffectHUDPath + "Empty");
            ResourcePrefab = prefab;
        }
        public EffectHUDEntity CreateEntity()
        {
            var entity = ScriptableObject.CreateInstance<EffectHUDEntity>();
            entity.name = Name;
            entity.id = Id;
            entity.hudPrefab = ResourcePrefab;
            return entity;
        }
        public enum EffectHUDType
        {
            Stack,
            Buff
        }
        public void Dispose()
        {
            if (ResourcePrefab != null)
                GameObject.Destroy(ResourcePrefab);
            Icon = null;
        }
    }
}
