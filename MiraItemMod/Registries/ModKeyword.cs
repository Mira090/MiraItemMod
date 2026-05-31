using HarmonyLib;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModKeyword : IDisposable
    {
        public static ModKeyword CreateKeyword(string name)
        {
            return new ModKeyword().SetKeyword(name);
        }
        public static ModKeyword CreateKeyword(string name, string nameKey, string descKey)
        {
            var keyword = new ModKeyword().SetKeyword(name);
            keyword.VisualText = new LocalizedString(nameKey);
            keyword.Description = new LocalizedString(descKey);
            return keyword;
        }
        internal ModKeyword SetKeyword(string name)
        {
            Keyword = name;
            VisualText = new LocalizedString($"Status_{name}_Name");
            Description = new LocalizedString($"Status_{name}_Description");
            return this;
        }
        public string Keyword { get; internal set; }
        public LocalizedString VisualText { get; internal set; }
        public LocalizedString Description { get; internal set; }
        public LocalizedString DetailedValue { get; internal set; } = new LocalizedString();
        public bool DisplayDetails { get; internal set; } = true;
        public bool NeedParseValueOnVisualText { get; internal set; } = false;
        public string TextColorOriginal { get; internal set; } = null;
        public Color TextColor { get; internal set;  } = Color.white;
        public string KeywordImageOriginal { get; internal set; } = null;
        public Func<Sprite> KeywordImage { get; internal set; } = null;
        public List<string> ConnectedDetailEntities { get; internal set; } = new List<string>();
        public KeywordEntity KeywordEntity { get; internal set; }

        public bool UseCopyTextColor => TextColorOriginal != null;
        public bool UseCopyKeywordImage => KeywordImageOriginal != null;

        public void Init()
        {
            KeywordEntity = CreateEntity();
        }
        public void Init(KeywordEntity original)
        {
            if (KeywordEntity == null)
                return;
            if (ConnectedDetailEntities.Contains(original.keyword) && !KeywordEntity.connectedDetailEntities.Contains(original))
            {
                KeywordEntity.connectedDetailEntities = KeywordEntity.connectedDetailEntities.AddItem(original).ToArray();
            }
            if (UseCopyTextColor && TextColorOriginal == original.keyword)
            {
                KeywordEntity.textColor = original.textColor;
            }
            if (UseCopyKeywordImage && KeywordImageOriginal == original.keyword)
            {
                KeywordEntity.keywordImage = original.keywordImage;
            }
        }
        public KeywordEntity CreateEntity()
        {
            var entity = ScriptableObject.CreateInstance<KeywordEntity>();
            entity.name = Keyword;
            entity.keyword = Keyword;
            entity.visualText = VisualText;
            entity.description = Description;
            entity.detailedValue = DetailedValue;
            entity.displayDetails = DisplayDetails;
            entity.needParseValueOnVisualText = NeedParseValueOnVisualText;
            if (!UseCopyTextColor)
                entity.textColor = TextColor;
            if(!UseCopyKeywordImage && KeywordImage != null)
                entity.keywordImage = KeywordImage();
            entity.connectedDetailEntities = new KeywordEntity[0];
            return entity;
        }
        public void Dispose()
        {
            if (KeywordEntity != null)
                ScriptableObject.Destroy(KeywordEntity);
        }
    }
}
