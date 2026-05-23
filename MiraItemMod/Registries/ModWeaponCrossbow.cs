using Mirror;
using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    internal class ModWeaponCrossbow : ModWeapon
    {
        public static ModWeaponCrossbow CreateCrossbow(string name, int copy, int dependency = -1)
        {
            return new ModWeaponCrossbow().SetCrossbow(name, copy, dependency);
        }
        internal ModWeaponCrossbow SetCrossbow(string name, int copy, int dependency)
        {
            SetWeapon(name, copy, dependency);
            return this;
        }

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
                    simple.mainWeaponBody.weaponSpriteRenderer.sprite = SpriteLoader.LoadSprite(MainSpriteFileName);
                    simple.mainWeaponBody.weaponStencilRenderer.sprite = SpriteLoader.LoadSprite(MainSpriteFileName);

                    if (simple.mainWeaponBody.weaponSpriteRenderer.gameObject.TryGetComponent<Animator2D_SpriteRenderer>(out var animator))
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
                            if(state.state != "FIRE")
                            {
                                newState.timeline = new List<AnimationSet.StateInfo.SpriteKeyFrame> { new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 0, sprite = SpriteLoader.LoadSprite(MainSpriteFileName) } };
                            }
                            else
                            {
                                newState.timeline = new List<AnimationSet.StateInfo.SpriteKeyFrame>() {
                                    new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 0, sprite = SpriteLoader.LoadSprite(MainSpriteFileName + "_01") },
                                    new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 1, sprite = SpriteLoader.LoadSprite(MainSpriteFileName + "_02") },
                                    new AnimationSet.StateInfo.SpriteKeyFrame() { frameIdx = 2, sprite = SpriteLoader.LoadSprite(MainSpriteFileName + "_03") }
                                    };
                            }
                            set.sprites.Add(newState);
                        }
                        animator.currentSet = set;
                    }

                    if (HasBladeSprite && simple.mainWeaponBody.bladeAddOnRenderer != null)
                    {
                        simple.mainWeaponBody.bladeAddOnRenderer.sprite = SpriteLoader.LoadSprite(BladeSpriteFileName);
                        if (BladeSpritePosition.HasValue)
                            simple.mainWeaponBody.bladeAddOnRenderer.transform.localPosition = BladeSpritePosition.Value;
                    }
                    if (HasBladeUnlitSprite)
                    {
                        var unlit = simple.mainWeaponBody.transform.Find("BladeUnlit");
                        if (unlit != null && unlit.gameObject.TryGetComponent<SpriteRenderer>(out var unlitSprite))
                        {
                            unlitSprite.sprite = SpriteLoader.LoadSprite(BladeSpriteFileName);
                            if (BladeUnlitSpritePosition.HasValue)
                                unlit.localPosition = BladeUnlitSpritePosition.Value;
                        }
                    }
                    if (HasHeadSprite)
                    {
                        var head = simple.mainWeaponBody.weaponSpriteRenderer.transform.Find("Head");
                        if (head != null && head.gameObject.TryGetComponent<SpriteRenderer>(out var headSprite))
                        {
                            headSprite.sprite = SpriteLoader.LoadSprite(HeadSpriteFileName);
                            if (HeadSpritePosition.HasValue)
                                head.localPosition = HeadSpritePosition.Value;
                        }
                    }
                }
                if (simple.subWeaponBody != null)
                {
                    simple.subWeaponBody.weaponSpriteRenderer.sprite = SpriteLoader.LoadSprite(SubSpriteFileName);
                    simple.subWeaponBody.weaponStencilRenderer.sprite = SpriteLoader.LoadSprite(SubSpriteFileName);
                }
                if (simple.subWeapon != null && simple.subWeapon.gameObject.TryGetComponent<SubWeapon>(out var shield))
                {
                    shield.weaponSpriteRenderer.sprite = SpriteLoader.LoadSprite(SubSpriteFileName);
                    shield.weaponStencilRenderer.sprite = SpriteLoader.LoadSprite(SubSpriteFileName);
                }
                MainPrefabModifier?.Invoke(simple);
            }

            MainWeaponPrefab = main;

            WeaponEntity = CreateWeaponEntity();
        }
    }
}
