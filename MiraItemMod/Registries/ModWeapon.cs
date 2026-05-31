using FMODUnity;
using Mirror;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModWeapon : IDisposable
    {
        public static UI_KatanaBar KatanaBar { get; internal set; }
        public static ModWeapon CreateWeapon(string name, int copy, int dependency = -1)
        {
            return new ModWeapon().SetWeapon(name, copy, dependency);
        }
        internal ModWeapon SetWeapon(string name, int copy, int dependency)
        {
            SetItem(name);
            Copy = copy;
            Dependency = dependency;
            return this;
        }
        internal ModWeapon SetItem(string name)
        {
            Name = name;
            LocalizedName = new LocalizedString("Weapon_" + name + "_Name");
            IconFileName = ModUtil.WeaponPath + name;
            MainSpriteFileName = ModUtil.WeaponPath + name + "_Main";
            BladeSpriteFileName = ModUtil.WeaponPath + name + "_Blade";
            SubSpriteFileName = ModUtil.WeaponPath + name + "_Sub";
            HeadSpriteFileName = ModUtil.WeaponPath + name + "_Head";
            return this;
        }
        public string Name { get; internal set; }
        public int Id { get; internal set; }
        public uint AssetId { get; internal set; }
        public WeaponEntity WeaponEntity { get; internal set; }
        public LocalizedString LocalizedName { get; internal set; }
        public GameObject MainWeaponPrefab { get; internal set; }
        public bool SetSizeFromTextureRect { get; internal set; }
        public string MainSpriteFileName { get; internal set; }
        public string SubSpriteFileName { get; internal set; }
        public string BladeSpriteFileName { get; internal set; }
        public string HeadSpriteFileName { get; internal set; }
        public string IconFileName { get; internal set; }
        public Sprite Icon { get; internal set; }
        public int Dependency { get; internal set; }
        public int? EnhanceFromId { get; internal set; }
        public int Copy { get; internal set; }
        public WeaponWieldEntity WeaponWieldEntity { get; internal set; }
        public List<int> StandardEnhancements { get; internal set; } = new List<int>();
        public Action<WeaponSimple> MainPrefabModifier { get; internal set; }
        public bool HasBladeSprite { get; internal set; }
        public Vector3? BladeSpritePosition { get; internal set; }
        public bool HasBladeUnlitSprite { get; internal set; }
        public Vector3? BladeUnlitSpritePosition { get; internal set; }
        public bool HasHeadSprite { get; internal set; }
        public Vector3? HeadSpritePosition { get; internal set; }

        public virtual void Init(int id, uint assetId)
        {
            Id = id;
            AssetId = assetId;
        }
        public virtual void Init(WeaponEntity copy)
        {
            InitPrefab(copy);
            WeaponEntity = CreateWeaponEntity();
        }
        protected virtual Sprite LoadSprite(string fileName)
        {
            var sprite = AssetLoader.LoadSprite(fileName);
            return sprite;
        }
        public void InitPrefab(WeaponEntity copy)
        {
            Core.LoggerMedium("CreateWeaponEntity from " + copy.name);
            WeaponWieldEntity = copy.wieldEntity;
            var main = UnityEngine.Object.Instantiate(copy.mainWeaponPrefab);
            main.name = "Weapon_" + Name;
            UnityEngine.Object.Destroy(main.GetComponent<NetworkIdentity>());

            if (main.TryGetComponent<WeaponSimple>(out var simple))
            {
                if (simple.mainWeaponBody != null)
                {
                    var sprite = LoadSprite(MainSpriteFileName);
                    simple.mainWeaponBody.weaponSpriteRenderer.sprite = sprite;
                    simple.mainWeaponBody.weaponStencilRenderer.sprite = sprite;
                    if (SetSizeFromTextureRect && sprite != null)
                    {
                        //simple.mainWeaponBody.weaponSpriteRenderer.size = sprite.textureRect.size / 16f;
                        simple.mainWeaponBody.bladeArea.size = sprite.textureRect.size / 16f;
                    }

                    if (HasBladeSprite && simple.mainWeaponBody.bladeAddOnRenderer != null)
                    {
                        simple.mainWeaponBody.bladeAddOnRenderer.sprite = LoadSprite(BladeSpriteFileName);
                        if (BladeSpritePosition.HasValue)
                            simple.mainWeaponBody.bladeAddOnRenderer.transform.localPosition = BladeSpritePosition.Value;
                        /*
                        if(SetSizeFromTextureRect && simple.mainWeaponBody.bladeAddOnRenderer.sprite != null)
                        {
                            simple.mainWeaponBody.bladeAddOnRenderer.size = simple.mainWeaponBody.bladeAddOnRenderer.sprite.textureRect.size / 16f;
                        }*/
                    }
                    if (HasBladeUnlitSprite)
                    {
                        var unlit = simple.mainWeaponBody.transform.Find("BladeUnlit");
                        if (unlit != null && unlit.gameObject.TryGetComponent<SpriteRenderer>(out var unlitSprite))
                        {
                            unlitSprite.sprite = LoadSprite(BladeSpriteFileName);
                            if (BladeUnlitSpritePosition.HasValue)
                                unlit.localPosition = BladeUnlitSpritePosition.Value;
                            if (SetSizeFromTextureRect && unlitSprite.sprite != null)
                            {
                                unlitSprite.size = unlitSprite.sprite.textureRect.size / 16f;
                            }
                        }
                    }
                    if (HasHeadSprite)
                    {
                        var head = simple.mainWeaponBody.weaponSpriteRenderer.transform.Find("Head");
                        if (head != null && head.gameObject.TryGetComponent<SpriteRenderer>(out var headSprite))
                        {
                            headSprite.sprite = LoadSprite(HeadSpriteFileName);
                            if (HeadSpritePosition.HasValue)
                                head.localPosition = HeadSpritePosition.Value;

                            /*
                            if(SetSizeFromTextureRect && headSprite.sprite != null)
                            {
                                headSprite.size = headSprite.sprite.textureRect.size / 16f;
                            }*/
                        }
                    }
                }
                if (simple.subWeaponBody != null)
                {
                    var sprite = LoadSprite(SubSpriteFileName);
                    simple.subWeaponBody.weaponSpriteRenderer.sprite = sprite;
                    simple.subWeaponBody.weaponStencilRenderer.sprite = sprite;
                }
                if (simple.subWeapon != null && simple.subWeapon.gameObject.TryGetComponent<SubWeapon>(out var shield))
                {
                    var sprite = LoadSprite(SubSpriteFileName);
                    shield.weaponSpriteRenderer.sprite = sprite;
                    shield.weaponStencilRenderer.sprite = sprite;
                }
                MainPrefabModifier?.Invoke(simple);
            }

            MainWeaponPrefab = main;
        }
        public WeaponEntity CreateWeaponEntity()
        {
            var entity = ScriptableObject.CreateInstance<WeaponEntity>();
            entity.name = Id + "_" + Name;
            entity.id = Id;
            entity.aName = LocalizedName;
            entity.mainWeaponPrefab = MainWeaponPrefab;
            entity.icon = Icon ?? AssetLoader.LoadSprite(IconFileName);
            entity.wieldEntity = WeaponWieldEntity;
            if(EnhanceFromId.HasValue)
                entity.enhanceFromId = EnhanceFromId.Value;
            return entity;
        }
        public Action<NewWeaponFireData[]> BasicAttacksModifier { get; internal set; }
        public List<NewWeaponFireData> NewBasicAttacks { get; internal set; } = new List<NewWeaponFireData>();
        public bool NewBasicAttacksOverride { get; internal set; } = false;
        public Action<NewWeaponFireData[]> DashAttacksModifier { get; internal set; }
        public List<NewWeaponFireData> NewDashAttacks { get; internal set; } = new List<NewWeaponFireData>();
        public bool NewDashAttacksOverride { get; internal set; } = false;
        public Action<NewWeaponFireData[]> SpecialAttacksModifier { get; internal set; }
        public List<NewWeaponFireData> NewSpecialAttacks { get; internal set; } = new List<NewWeaponFireData>();
        public bool NewSpecialAttacksOverride { get; internal set; } = false;
        public virtual void OnSpriteFxRegistered()
        {
            if (MainWeaponPrefab == null)
                return;
            if (!MainWeaponPrefab.TryGetComponent<WeaponSimple>(out var simple))
                return;
            Core.LoggerMany($"OnSpriteFxRegistered: {simple}");

            BasicAttacksModifier?.Invoke(simple.basicComboAttacks);
            //Core.Logger($"OnSpriteFxRegistered2: {BasicAttacksModifier}");

            if (NewBasicAttacksOverride)
            {
                simple.basicComboAttacks = NewBasicAttacks.ToArray();
            }
            else
            {
                for (int q = 0; q < simple.basicComboAttacks.Length; q++)
                {
                    if (NewBasicAttacks.Count <= q)
                        break;
                    simple.basicComboAttacks[q] = NewBasicAttacks[q];
                }
            }
            //Core.Logger($"OnSpriteFxRegistered3: {simple}");

            DashAttacksModifier?.Invoke(simple.dashAttacks);

            if (NewDashAttacksOverride)
            {
                simple.dashAttacks = NewDashAttacks.ToArray();
            }
            else
            {
                for (int q = 0; q < simple.dashAttacks.Length; q++)
                {
                    if (NewDashAttacks.Count <= q)
                        break;
                    simple.dashAttacks[q] = NewDashAttacks[q];
                }
            }
            SpecialAttacksModifier?.Invoke(simple.specialAttacks);

            if (NewSpecialAttacksOverride)
            {
                simple.specialAttacks = NewSpecialAttacks.ToArray();
            }
            else
            {
                for (int q = 0; q < simple.specialAttacks.Length; q++)
                {
                    if (NewSpecialAttacks.Count <= q)
                        break;
                    simple.specialAttacks[q] = NewSpecialAttacks[q];
                }
            }
            //Core.Logger($"OnSpriteFxRegistered: end");
        }
        public static NewWeaponFireData CopyNewWeaponFireData(NewWeaponFireData_MeleeAttack melee)
        {
            var fire = ScriptableObject.CreateInstance<NewWeaponFireData_MeleeAttack>();
            fire.name = melee.name + "_Mod";
            fire.projectilePrefab = melee.projectilePrefab;
            fire.showSwingFxOnMeleeCollision = melee.showSwingFxOnMeleeCollision;
            fire.attackDashDirectionType = melee.attackDashDirectionType;
            fire.attackDashCurveType = melee.attackDashCurveType;
            fire.attackDashSpeed = melee.attackDashSpeed;
            fire.attackDashAngle = melee.attackDashAngle;
            fire.attackDashTime = melee.attackDashTime;
            fire.damageMultiplier = melee.damageMultiplier;
            fire.staggeringLevel = melee.staggeringLevel;
            fire.externalForcePower = melee.externalForcePower;
            fire.swingID = melee.swingID;
            fire.addDamagePerUsedMP = melee.addDamagePerUsedMP;
            fire.damageElementalType = melee.damageElementalType;
            fire.relatedStatFormula = melee.relatedStatFormula;
            fire.useElementalTypeFromRelatedStatFormula = melee.useElementalTypeFromRelatedStatFormula;
            fire.chaosDamageColor = melee.chaosDamageColor;
            fire.hitStopTime = melee.hitStopTime;
            fire.cameraShakeVelocityOnFire = melee.cameraShakeVelocityOnFire;
            fire.aimSupport = melee.aimSupport;
            fire.swingFxPrefab = melee.swingFxPrefab;
            fire.attachSwingFxToOwner = melee.attachSwingFxToOwner;
            fire.swingSoundEvent = melee.swingSoundEvent;
            fire.relatedToMeleeAttackRangeBonus = melee.relatedToMeleeAttackRangeBonus;
            return fire;
        }
        public enum EAttackType
        {
            Basic,
            Dash,
            Special
        }
        public virtual void Dispose()
        {
            if (WeaponEntity != null)
                ScriptableObject.Destroy(WeaponEntity);
            if (MainWeaponPrefab != null)
                GameObject.Destroy(MainWeaponPrefab);
            if(NewBasicAttacks != null)
            {
                foreach (var attack in NewBasicAttacks)
                {
                    if (attack != null)
                        ScriptableObject.Destroy(attack);
                }
                NewBasicAttacks.Clear();
            }
            if(NewDashAttacks != null)
            {
                foreach (var attack in NewDashAttacks)
                {
                    if (attack != null)
                        ScriptableObject.Destroy(attack);
                }
                NewDashAttacks.Clear();
            }
            if(NewSpecialAttacks != null)
            {
                foreach (var attack in NewSpecialAttacks)
                {
                    if (attack != null)
                        ScriptableObject.Destroy(attack);
                }
                NewSpecialAttacks.Clear();
            }
            Icon = null;
        }
    }
}
