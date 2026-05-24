using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public static class CustomSpriteAsset
    {
        public static TMP_SpriteAsset SpriteAsset;
        public static Sprite NewKeywordSprite;

        public static Sprite Planet;
        public static Sprite BinaryPlanet;
        public static Sprite IceExecution;
        public static Sprite FireExecution;
        public static Sprite LightningExecution;
        public static Sprite Assasination;
        public static Sprite MerchantLeaf;
        public static Sprite BlackFrostbite;
        public static Sprite Excavation;
        public static Sprite ExcavationJewelry;
        public static Sprite Crime;
        public static Sprite Looting;

        public static int width = 10;
        public static int height = 10;

        public static Sprite NoneCategoryIcon;
        public static void InitSprites()
        {
            NoneCategoryIcon = AssetLoader.LoadSprite(ModUtil.ItemCategoryPath + "None");

            NewKeywordSprite = AssetLoader.LoadSprite(ModUtil.UIPath + "Keyword");
            if(NewKeywordSprite == null)
            {
                Core.LoggerWarning("Failed to load Keyword sprite sheet!\nPlease place the keyword image in path " + AssetLoader.GetAssetsPath(ModUtil.UIPath + "Keyword"));
                return;
            }
            NewKeywordSprite.texture.wrapMode = TextureWrapMode.Clamp;
            NewKeywordSprite.texture.wrapModeU = TextureWrapMode.Clamp;
            NewKeywordSprite.texture.wrapModeV = TextureWrapMode.Clamp;
            NewKeywordSprite.texture.wrapModeW = TextureWrapMode.Clamp;
            NewKeywordSprite.texture.name = "ModKeyword";
            int max = 100 - height;

            Planet = AssetLoader.CreateSprite(NewKeywordSprite.texture, "Planet", new Rect(0, max, width, height));
            BinaryPlanet = AssetLoader.CreateSprite(NewKeywordSprite.texture, "BinaryPlanet", new Rect(0, height * 1, width, height));
            IceExecution = AssetLoader.CreateSprite(NewKeywordSprite.texture, "IceExecution", new Rect(0, height * 2, width, height));
            FireExecution = AssetLoader.CreateSprite(NewKeywordSprite.texture, "FireExecution", new Rect(0, height * 3, width, height));
            LightningExecution = AssetLoader.CreateSprite(NewKeywordSprite.texture, "LightningExecution", new Rect(0, height * 4, width, height));
            Assasination = AssetLoader.CreateSprite(NewKeywordSprite.texture, "Assasination", new Rect(0, height * 5, width, height));
            MerchantLeaf = AssetLoader.CreateSprite(NewKeywordSprite.texture, "MerchantLeaf", new Rect(0, height * 6, width, height));
            BlackFrostbite = AssetLoader.CreateSprite(NewKeywordSprite.texture, "BlackFrostbite", new Rect(0, height * 7, width, height));
            Excavation = AssetLoader.CreateSprite(NewKeywordSprite.texture, "Excavation", new Rect(0, height * 8, width, height));
            ExcavationJewelry = AssetLoader.CreateSprite(NewKeywordSprite.texture, "ExcavationJewelry", new Rect(0, height * 9, width, height));
            Crime = AssetLoader.CreateSprite(NewKeywordSprite.texture, "Crime", new Rect(width, height * 0, width, height));
            Looting = AssetLoader.CreateSprite(NewKeywordSprite.texture, "Looting", new Rect(width, height * 1, width, height));
        }
        public static void InitSpriteAsset()
        {
            if(NewKeywordSprite == null)
            {
                Core.LoggerWarning("Keyword sprite sheet is null, cannot create sprite asset!");
                return;
            }
            if (SpriteAsset != null)
                return;
            int max = 100 - height;
            SpriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            SpriteAsset.name = "Keyword";
            SpriteAsset.spriteSheet = NewKeywordSprite.texture;
            SpriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite"));
            SpriteAsset.material.mainTexture = NewKeywordSprite.texture;
            SpriteAsset.spriteInfoList = new List<TMP_Sprite>();
            int id = 0;
            int unicodeStart = 0xE000;
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max, name = "Planet", sprite = Planet });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 1, name = "BinaryPlanet", sprite = BinaryPlanet });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 2, name = "IceExecution", sprite = IceExecution });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 3, name = "FireExecution", sprite = FireExecution });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 4, name = "LightningExecution", sprite = LightningExecution });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 5, name = "Assasination", sprite = Assasination });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 6, name = "MerchantLeaf", sprite = MerchantLeaf });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 7, name = "BlackFrostbite", sprite = BlackFrostbite });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 8, name = "Excavation", sprite = Excavation });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = 0, y = max - height * 9, name = "ExcavationJewelry", sprite = ExcavationJewelry });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = width * 1, y = max - height * 0, name = "Crime", sprite = Crime });
            SpriteAsset.spriteInfoList.Add(new TMP_Sprite() { x = width * 1, y = max - height * 1, name = "Looting", sprite = Looting });
            foreach (var sprite in SpriteAsset.spriteInfoList)
            {
                sprite.id = id++;
                sprite.scale = 1.1f;
                sprite.width = width;
                sprite.height = height;
                sprite.yOffset = 9;
                //if (sprite.x == 0)
                    //sprite.xOffset = -1;
                sprite.xAdvance = 10;
                sprite.unicode = unicodeStart + id - 1;
                sprite.hashCode = TMP_TextUtilities.GetSimpleHashCode(sprite.name);
            }

            SpriteAsset.UpdateLookupTables();

            if(TMP_Settings.defaultSpriteAsset != null)
            {
                TMP_Settings.defaultSpriteAsset.fallbackSpriteAssets.Add(SpriteAsset);
            }
            else
            {
                TMP_Settings.instance.SetDefaultSpriteAsset(SpriteAsset);
            }

            int count = 0;
            foreach (var item in SpriteAsset.spriteCharacterTable)
            {
                item.glyph = SpriteAsset.spriteGlyphTable[count];
                count++;
            }
        }
    }
}
