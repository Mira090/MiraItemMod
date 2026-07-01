using Mirror;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModWeaponKatana : ModWeapon
    {
        public static ModWeaponKatana CreateKatana(string name, int copy, int dependency = -1)
        {
            return new ModWeaponKatana().SetKatana(name, copy, dependency);
        }
        internal ModWeaponKatana SetKatana(string name, int copy, int dependency)
        {
            SetWeapon(name, copy, dependency);
            ScabbardSpriteFileName = ModUtil.WeaponPath + name + "_Scabbard";
            SheathSpriteFileName = ModUtil.WeaponPath + name + "_Sheath";
            return this;
        }
        public string ScabbardSpriteFileName { get; internal set; }
        public string SheathSpriteFileName { get; internal set; }

        public override void InitPrefab(WeaponEntity copy)
        {
            //Core.Logger("CreateWeaponEntity from " + copy.name);
            //WeaponWieldEntity = copy.wieldEntity;
            var main = UnityEngine.Object.Instantiate(copy.mainWeaponPrefab);
            main.name = "Weapon_" + Name;
            main.SetAssetId(AssetId);

            if (main.TryGetComponent<WeaponSimple>(out var simple))
            {
                if (simple.mainWeaponBody != null)
                {
                    simple.mainWeaponBody.weaponSpriteRenderer.sprite = AssetLoader.LoadSprite(MainSpriteFileName);
                    simple.mainWeaponBody.weaponStencilRenderer.sprite = AssetLoader.LoadSprite(MainSpriteFileName);

                    if (simple.mainWeaponBody.weaponSpriteRenderer.gameObject.TryGetComponent<Animator2D_MultipleSpriteRenderer>(out var animator))
                    {
                        var set = ScriptableObject.CreateInstance<AnimationSet>();
                        set.name = Name;
                        set.sprites = new List<AnimationSet.StateInfo>();
                        foreach (var state in animator.currentSet.sprites)
                        {
                            var newState = new AnimationSet.StateInfo();
                            newState.fps = state.fps;
                            newState.state = state.state;
                            newState.repeat = state.repeat;
                            newState.frameEvents = state.frameEvents;
                            newState.soundEvents = state.soundEvents;
                            newState.transformAttributes = state.transformAttributes;
                            if (state.state == "SHEATH" && !string.IsNullOrEmpty(SheathSpriteFileName))
                                newState.timeline = new List<AnimationSet.StateInfo.SpriteKeyFrame> { new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 0, sprite = AssetLoader.LoadSprite(SheathSpriteFileName) ?? AssetLoader.LoadSprite(MainSpriteFileName) } };
                            else
                                newState.timeline = new List<AnimationSet.StateInfo.SpriteKeyFrame> { new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 0, sprite = AssetLoader.LoadSprite(MainSpriteFileName) } };
                            set.sprites.Add(newState);
                        }
                        animator.currentSet = set;
                    }

                    /*
                    Core.Logger(this.Name);
                    for (int q = 0;q< simple.mainWeaponBody.transform.childCount; q++)
                    {
                        var chil = simple.mainWeaponBody.transform.GetChild(q);
                        Core.Logger(chil.name);
                        Core.Logger("sprite: " + chil.TryGetComponent<SpriteRenderer>(out var _));
                    }
                    if (simple is WeaponSimple_Katana k)
                    {
                        if (k.katanaHardeningAnimator != null)
                            Core.Logger("Animator1: " + k.katanaHardeningAnimator.GetType());
                        if (k.katanaHardeningScabbardAnimator != null)
                            Core.Logger("Animator2: " + k.katanaHardeningScabbardAnimator.GetType());
                        if (k.scabbardAnimator != null)
                        {
                            Core.Logger("Animator3: " + k.scabbardAnimator.GetType());
                            var set = k.scabbardAnimator.currentSet;
                            foreach (var state in set.sprites)
                            {
                                Core.Logger("State: " + state.state);
                                foreach (var frame in state.timeline)
                                {
                                    Core.Logger("Frame: " + frame.frameIdx + " Sprite: " + frame.sprite.name);
                                    //AssetLoader.SaveSprite(frame.sprite, Name + "_Scabbard_Frame" + frame.frameIdx);
                                }
                            }
                        }
                        if (k.katanaAnimator != null)
                        {
                            Core.Logger("Animator4: " + k.katanaAnimator.GetType());
                            var set = k.katanaAnimator.currentSet;
                            foreach (var state in set.sprites)
                            {
                                Core.Logger("State: " + state.state);
                                foreach (var frame in state.timeline)
                                {
                                    Core.Logger("Frame: " + frame.frameIdx + " Sprite: " + frame.sprite.name);
                                    //AssetLoader.SaveSprite(frame.sprite, Name + "_Scabbard_Frame" + frame.frameIdx);
                                }
                            }
                        }
                    }*/
                    /*
                    var scabbard = simple.mainWeaponBody.transform.Find("Scabbard");
                    var scabbardSprite = AssetLoader.LoadSprite(ScabbardSpriteFileName);
                    if (scabbardSprite != null && scabbard != null && scabbard.gameObject.TryGetComponent<SpriteRenderer>(out var scabbardRenderer))
                    {
                        scabbardRenderer.sprite = scabbardSprite;
                    }*/
                    if (simple is WeaponSimple_Katana katana && katana.scabbardAnimator != null)
                    {
                        var set = ScriptableObject.CreateInstance<AnimationSet>();
                        set.name = Name;
                        set.sprites = new List<AnimationSet.StateInfo>();
                        foreach (var state in katana.scabbardAnimator.currentSet.sprites)
                        {
                            var newState = new AnimationSet.StateInfo();
                            newState.fps = state.fps;
                            newState.state = state.state;
                            newState.repeat = state.repeat;
                            newState.frameEvents = state.frameEvents;
                            newState.soundEvents = state.soundEvents;
                            newState.transformAttributes = state.transformAttributes;
                            if (state.state != "SHEATH" && !string.IsNullOrEmpty(ScabbardSpriteFileName))
                                newState.timeline = new List<AnimationSet.StateInfo.SpriteKeyFrame> { new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 0, sprite = AssetLoader.LoadSprite(ScabbardSpriteFileName, new Vector2(0.5f, 0f)) ?? state.timeline[0].sprite } };
                            else
                                newState.timeline = new List<AnimationSet.StateInfo.SpriteKeyFrame> { new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 0, sprite = state.timeline[0].sprite } };
                            set.sprites.Add(newState);
                        }
                        katana.scabbardAnimator.currentSet = set;
                    }

                    if (HasBladeSprite && simple.mainWeaponBody.bladeAddOnRenderer != null)
                    {
                        simple.mainWeaponBody.bladeAddOnRenderer.sprite = AssetLoader.LoadSprite(BladeSpriteFileName);
                        if (BladeSpritePosition.HasValue)
                            simple.mainWeaponBody.bladeAddOnRenderer.transform.localPosition = BladeSpritePosition.Value;
                    }
                    if (HasBladeUnlitSprite)
                    {
                        var unlit = simple.mainWeaponBody.transform.Find("BladeUnlit");
                        if (unlit != null && unlit.gameObject.TryGetComponent<SpriteRenderer>(out var unlitSprite))
                        {
                            unlitSprite.sprite = AssetLoader.LoadSprite(BladeSpriteFileName);
                            if (BladeUnlitSpritePosition.HasValue)
                                unlit.localPosition = BladeUnlitSpritePosition.Value;
                        }
                    }
                    if (HasHeadSprite)
                    {
                        var head = simple.mainWeaponBody.weaponSpriteRenderer.transform.Find("Head");
                        if (head != null && head.gameObject.TryGetComponent<SpriteRenderer>(out var headSprite))
                        {
                            headSprite.sprite = AssetLoader.LoadSprite(HeadSpriteFileName);
                            if (HeadSpritePosition.HasValue)
                                head.localPosition = HeadSpritePosition.Value;
                        }
                    }
                }
                if (simple.subWeaponBody != null)
                {
                    simple.subWeaponBody.weaponSpriteRenderer.sprite = AssetLoader.LoadSprite(SubSpriteFileName);
                    simple.subWeaponBody.weaponStencilRenderer.sprite = AssetLoader.LoadSprite(SubSpriteFileName);
                }
                if (simple.subWeapon != null && simple.subWeapon.gameObject.TryGetComponent<SubWeapon>(out var shield))
                {
                    shield.weaponSpriteRenderer.sprite = AssetLoader.LoadSprite(SubSpriteFileName);
                    shield.weaponStencilRenderer.sprite = AssetLoader.LoadSprite(SubSpriteFileName);
                }
                MainPrefabModifier?.Invoke(simple);
            }

            MainWeaponPrefab = main;
        }
    }
}
