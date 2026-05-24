using Mirror;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
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
            return this;
        }
        public string ScabbardSpriteFileName { get; internal set; }

        public override void Init(WeaponEntity copy)
        {
            //Core.Logger("CreateWeaponEntity from " + copy.name);
            WeaponWieldEntity = copy.wieldEntity;
            var main = UnityEngine.Object.Instantiate(copy.mainWeaponPrefab);
            main.name = "Weapon_" + Name;
            UnityEngine.Object.Destroy(main.GetComponent<NetworkIdentity>());

            if (main.TryGetComponent<WeaponSimple>(out var simple))
            {
                if (simple.mainWeaponBody != null)
                {
                    simple.mainWeaponBody.weaponSpriteRenderer.sprite = AssetLoader.LoadSprite(MainSpriteFileName);
                    simple.mainWeaponBody.weaponStencilRenderer.sprite = AssetLoader.LoadSprite(MainSpriteFileName);

                    if(simple.mainWeaponBody.weaponSpriteRenderer.gameObject.TryGetComponent<Animator2D_MultipleSpriteRenderer>(out var animator))
                    {
                        var set = ScriptableObject.CreateInstance<AnimationSet>();
                        set.name = Name;
                        set.sprites = new List<AnimationSet.StateInfo>();
                        foreach(var state in animator.currentSet.sprites)
                        {
                            var newState = new AnimationSet.StateInfo();
                            newState.fps = state.fps;
                            newState.state = state.state;
                            newState.repeat = state.repeat;
                            newState.frameEvents = state.frameEvents;
                            newState.soundEvents = state.soundEvents;
                            newState.transformAttributes = state.transformAttributes;
                            newState.timeline = new List<AnimationSet.StateInfo.SpriteKeyFrame> { new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 0, sprite = AssetLoader.LoadSprite(MainSpriteFileName) } };
                            set.sprites.Add(newState);
                        }
                        animator.currentSet = set;
                    }

                    var scabbard = simple.mainWeaponBody.transform.Find("Scabbard");
                    var scabbardSprite = AssetLoader.LoadSprite(ScabbardSpriteFileName);
                    if (scabbardSprite != null && scabbard != null && scabbard.gameObject.TryGetComponent<SpriteRenderer>(out var scabbardRenderer))
                    {
                        scabbardRenderer.sprite = scabbardSprite;
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

            WeaponEntity = CreateWeaponEntity();
        }
    }
}
