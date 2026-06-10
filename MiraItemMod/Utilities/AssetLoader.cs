using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Utilities
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

        public static void SaveSprite(Sprite sprite, string name)
        {
            if (sprite == null)
                return;
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            System.IO.DirectoryInfo directoryInfo = Directory.GetParent(dllPath);
            string dllDirectory = directoryInfo.FullName;
            var path = dllDirectory + @"\Outputs\" + name + ".png";
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            try
            {
                Texture2D texture = sprite.texture;
                RenderTexture temp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                Graphics.Blit(texture, temp);
                RenderTexture active = RenderTexture.active;
                RenderTexture.active = temp;
                Texture2D texture2D = new Texture2D(texture.width, texture.height);
                texture2D.ReadPixels(new Rect(0f, 0f, temp.width, temp.height), 0, 0);
                texture2D.Apply();
                RenderTexture.active = active;
                RenderTexture.ReleaseTemporary(temp);
                File.WriteAllBytes(path, texture2D.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture2D);
            }
            catch(Exception e)
            {
                Core.LoggerWarning(e);
            }
        }
        public static void LoadSound()
        {
            FMOD.Sound playerSound;
            FMOD.RESULT result = FMODUnity.RuntimeManager.CoreSystem.createSound(
                GetAssetsFolder("\\audio.wav"),
                FMOD.MODE._2D,
                out playerSound
            );

            if (result == FMOD.RESULT.OK)
            {
                // Play the sound once loaded
                FMODUnity.RuntimeManager.CoreSystem.playSound(playerSound, new FMOD.ChannelGroup(), false, out var channel);
                //FMODUnity.RuntimeManager.CreateInstance(playerSound);
            }
        }
    }
}
