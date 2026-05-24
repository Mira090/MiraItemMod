using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Utilities
{
    public class AssetLoader
    {
        public static string GetAssetsPath(string name)
        {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = dllDirectory + @"\Assets\" + name + ".png";
            return path;
        }
        public static string GetAssetsFolder(string name)
        {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = dllDirectory + @"\Assets\" + name;
            return path;
        }
        public static Sprite LoadSprite(string name)
        {
            var path = GetAssetsPath(name);
            if (!File.Exists(path))
            {
                Core.LoggerWarning(path + " is not exist!");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), 16
            );
            sprite.name = name;
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        public static Sprite LoadSprite(string name, Vector2 pivot)
        {
            var path = GetAssetsPath(name);
            if (!File.Exists(path))
            {
                Core.LoggerWarning(path + " is not exist!");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                pivot, 16
            );
            sprite.name = name;
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        public static Sprite LoadSpriteWithBorder(string name, Vector4 border)
        {
            var path = GetAssetsPath(name);
            if (!File.Exists(path))
            {
                Core.LoggerWarning(path + " is not exist!");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f), 16, 0, SpriteMeshType.FullRect, border
            );
            sprite.name = name;
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        public static Sprite LoadSprite(string name, Rect rect)
        {
            var path = GetAssetsPath(name);
            if (!File.Exists(path))
            {
                Core.LoggerWarning(path + " is not exist!");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            var sprite = Sprite.Create(
                tex,
                rect,
                new Vector2(0.5f, 0.5f), 16
            );
            sprite.name = name;
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        public static Sprite CreateSprite(Texture2D tex, string name, Rect rect)
        {
            var sprite = Sprite.Create(
                tex,
                rect,
                new Vector2(0.5f, 0.5f), 16
            );
            sprite.name = name;
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        /// <summary>
        /// ピボットがLoadSprite()と違う
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Sprite LoadSpriteForCostume(string path)
        {
            if (!File.Exists(path))
            {
                Core.LoggerWarning(path + " is not exist!");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.2f), 16
            );
            sprite.name = Path.GetFileNameWithoutExtension(path);
            sprite.texture.filterMode = FilterMode.Point;
            //sprite.bounds.extents = new Vector3(sprite.bounds.extents.x * 6, sprite.bounds.extents.y * 6, sprite.bounds.extents.z);
            return sprite;
        }
        public static void LoadLocalization(HorayModLocalizationContext context)
        {
            if (!Directory.Exists(AssetLoader.GetAssetsFolder(ModUtil.LocalizationPath)))
                return;
            foreach (string item in Directory.EnumerateFiles(Path.Combine(AssetLoader.GetAssetsFolder(ModUtil.LocalizationPath)), "*.json"))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
                if (fileNameWithoutExtension.StartsWith("._") || ShouldSkipNonLocaleJsonFile(fileNameWithoutExtension))
                {
                    continue;
                }

                SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
                try
                {
                    using FileStream stream = new FileStream(item, FileMode.Open, FileAccess.Read);
                    using StreamReader streamReader = new StreamReader(stream);
                    sortedDictionary = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(streamReader.ReadToEnd());
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    sortedDictionary = null;
                }

                if (sortedDictionary != null)
                {
                    foreach (var pair in sortedDictionary)
                    {
                        context.AddText(fileNameWithoutExtension, pair.Key, pair.Value);
                    }
                }
            }
        }
        private static bool ShouldSkipNonLocaleJsonFile(string fileNameWithoutExtension)
        {
            return string.Equals(fileNameWithoutExtension, "glossary", StringComparison.OrdinalIgnoreCase);
        }
    }
}
