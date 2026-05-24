using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModSpriteFx
    {
        public static ModSpriteFx CreateSpriteFx(string name, string original, string path, int timeline)
        {
            return new ModSpriteFx().SetSpriteFx(name, original, path, timeline);
        }
        internal ModSpriteFx SetSpriteFx(string name, string original, string path, int timeline)
        {
            Name = name;
            SpritePath = path;
            OriginalName = original;
            TimelineCount = timeline;
            return this;
        }
        public string Name { get; internal set; } = "DaggerDashFx_Ice";
        public string OriginalName { get; internal set; }
        public int TimelineCount { get; internal set; }
        public string SpritePath { get; internal set; } = ModUtil.WeaponPath + "Dagger_Ice\\Weapon_Dagger_DashAttack_";
        public bool CopyPivot { get; internal set; } = false;
        public int? Fps { get; internal set; }

        public GameObject ResourcePrefab { get; internal set; }

        public void InitPrefab(SpriteFx original)
        {
            var fx = UnityEngine.Object.Instantiate(original);
            fx.gameObject.name = Name;

            var set = ScriptableObject.CreateInstance<AnimationSet>();
            set.name = Name;
            set.sprites = new List<AnimationSet.StateInfo>();
            foreach (var state in fx.animator2D.currentSet.sprites)
            {
                var newState = new AnimationSet.StateInfo();
                if(Fps.HasValue)
                    newState.fps = Fps.Value;
                else
                    newState.fps = state.fps;
                newState.state = state.state;
                newState.repeat = state.repeat;
                newState.frameEvents = state.frameEvents;
                newState.soundEvents = state.soundEvents;
                newState.transformAttributes = state.transformAttributes;

                var list = new List<AnimationSet.StateInfo.SpriteKeyFrame>();
                for (int q = 0; q < TimelineCount; q++)
                {
                    if (CopyPivot)
                    {
                        var s = state.timeline.SafeRandomAccess(q).sprite;
                        list.Add(new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = q, sprite = AssetLoader.LoadSprite(SpritePath + q, new Vector2(s.pivot.x / s.rect.width, s.pivot.y / s.rect.height)) });
                    }
                    else
                    {
                        list.Add(new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = q, sprite = AssetLoader.LoadSprite(SpritePath + q) });
                    }
                }
                newState.timeline = list;
                set.sprites.Add(newState);
            }

            fx.animator2D.currentSet = set;
            fx.animator2D.ChangeSet(set);

            ResourcePrefab = fx.gameObject;
        }
    }
}
