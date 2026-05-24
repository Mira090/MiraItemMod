using MiraItemMod.Buffs;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Miracle;
using static StoneTablet;

namespace MiraItemMod.Registries
{
    public static class ModItemExtensions
    {
        public static T SetRarity<T>(this T item, EItemRarity rarity) where T : ModItem
        {
            item.Rarity = rarity;
            item.Cost = rarity switch
            {
                EItemRarity.Common => 400,
                EItemRarity.Uncommon => 600,
                EItemRarity.Rare => 800,
                EItemRarity.Legend => 1100,
                _ => 200
            };
            item.SapphirePrice = rarity switch
            {
                EItemRarity.Common => 2,
                EItemRarity.Uncommon => 3,
                EItemRarity.Rare => 4,
                EItemRarity.Legend => 5,
                _ => 1
            };
            return item;
        }
        public static T SetEntityRarity<T>(this T item, EItemRarity rarity) where T : ItemEntity
        {
            item.rarity = rarity;
            item.cost = rarity switch
            {
                EItemRarity.Common => 400,
                EItemRarity.Uncommon => 600,
                EItemRarity.Rare => 800,
                EItemRarity.Legend => 1100,
                _ => 200
            };
            item.sapphirePrice = rarity switch
            {
                EItemRarity.Common => 2,
                EItemRarity.Uncommon => 3,
                EItemRarity.Rare => 4,
                EItemRarity.Legend => 5,
                _ => 1
            };
            return item;
        }
        public static T SetSacrifice<T>(this T item) where T : ModItem
        {
            item.Rarity = ECustomItemRarity.Sacrifice.ToSephiria();
            item.Cost = 999999999;
            item.SapphirePrice = 0;
            //item.CannotBeReward = true;
            item.CannotThrow = true;
            if(item is ModCharm charm)
                charm.IsUniqueEffect = true;
            item.IconFileName = ModUtil.ItemPath + "Sacrifice";
            return item;
        }
        public static T SetCategory<T>(this T item, params string[] categories) where T : ModItem
        {
            item.Categories = categories.ToList();
            return item;
        }
        public static T SetEffects<T>(this T item, params LocalizedString[] effects) where T : ModCharm
        {
            item.Effects = effects;
            return item;
        }
        public static T SetEffects<T>(this T item, params string[] effects) where T : ModCharm
        {
            item.Effects = effects.Select(effect => new LocalizedString(effect)).ToArray();
            return item;
        }
        public static T SetActiveType<T>(this T item, EItemActiveType activeType) where T : ModItem
        {
            item.ActiveType = activeType;
            return item;
        }
        public static T SetCannotBeReward<T>(this T item, bool cannotBeReward = true) where T : ModItem
        {
            item.CannotBeReward = cannotBeReward;
            return item;
        }
        public static T SetCannotThrow<T>(this T item, bool cannotThrow = true) where T : ModItem
        {
            item.CannotThrow = cannotThrow;
            return item;
        }
        public static T SetIsDual<T>(this T item, bool isDual = true) where T : ModItem
        {
            item.IsDual = isDual;
            return item;
        }
        public static T SetIsJewelry<T>(this T item, EItemRarity rarity) where T : ModItem
        {
            item.SetRarity(rarity);
            item.IsJewelry = true;
            item.Cost += item.Cost;
            item.ActiveType = EItemActiveType.Hidden;
            return item;
        }
        public static T SetIsExcludedJewelry<T>(this T item, EItemRarity rarity) where T : ModItem
        {
            item.SetRarity(rarity);
            item.IsJewelry = true;
            item.IsExcludedJewelry = true;
            item.Cost += item.Cost;
            item.ActiveType = EItemActiveType.Hidden;
            return item;
        }
        public static T SetTreeShopItemEntity<T>(this T item, int id) where T : ModItem
        {
            item.TreeShopItemEntity = id;
            return item;
        }
        public static T SetSimpleEffect<T>(this T item) where T : ModCharm
        {
            item.Effects = new LocalizedString[] { new LocalizedString("Item_" + item.Name + "_Effect") };
            return item;
        }
        public static T SetSimpleEffects<T>(this T item, int count) where T : ModCharm
        {
            var list = new List<LocalizedString>();
            for(int q = 0; q < count; q++)
            {
                if (q == 0)
                    list.Add(new LocalizedString("Item_" + item.Name + "_Effect"));
                else
                    list.Add(new LocalizedString("Item_" + item.Name + "_Effect" + (q + 1).ToString()));
            }
            item.Effects = list.ToArray();
            return item;
        }
        public static T SetIsUniqueEffect<T>(this T item, bool isUniqueEffect = true) where T : ModCharm
        {
            item.IsUniqueEffect = isUniqueEffect;
            return item;
        }
        public static T SetRelatedWeapon<T>(this T item, EWeaponType relatedWeapon) where T : ModCharm
        {
            item.RelatedWeapon = relatedWeapon;
            item.IsWeaponRelatedCharm = true;
            return item;
        }
        public static T SetDamageId<T>(this T item) where T : ModCharm
        {
            item.DamageId = ModDamageId.CreateCharm(item.Name);
            return item;
        }
        public static T SetDamageIdAbility<T>(this T item) where T : ModComboEffect
        {
            item.DamageId = ModDamageId.CreateAbility(item.Name);
            return item;
        }
        public static T SetDamageIdAbility<T>(this T item, string id) where T : ModCharm
        {
            item.DamageId = ModDamageId.CreateAbility(id);
            return item;
        }
        public static T SetDamageIdDebuff<T>(this T item, string id) where T : ModCharm
        {
            item.DamageId = ModDamageId.CreateDebuff(id);
            return item;
        }
        public static T SetDamageIdDebuff<T>(this T item, string id, string key) where T : ModCharm
        {
            item.DamageId = ModDamageId.CreateDebuff(id, key);
            return item;
        }
        public static T SetDamageIdPerk<T>(this T item) where T : ModPassivePerk
        {
            item.DamageId = ModDamageId.CreatePerk(item.Name);
            return item;
        }
        public static T SetStats<T>(this T item, params Charm_StatusInstance.StatusGroup[] stats) where T : ModCharmStatus
        {
            item.Stats = stats;
            return item;
        }
        public static T SetFromType<T>(this T item, EDamageFromType fromType) where T : ModCharmSacrificeDamage
        {
            item.FromType = fromType;
            return item;
        }
        public static T SetElementalType<T>(this T item, EDamageElementalType elementalType) where T : ModCharmSacrificeDamage
        {
            item.ElementalType = elementalType;
            return item;
        }
        public static T SetDamageType<T>(this T item, EDamageFromType fromType, EDamageElementalType elementalType) where T : ModCharmSacrificeDamage
        {
            item.FromType = fromType;
            item.ElementalType = elementalType;
            return item;
        }
        public static T SetConditionQuery<T>(this T item, string query) where T : ModStoneTablet
        {
            item.ConditionQuery = query;
            return item;
        }
        public static T SetStats<T>(this T item, params ComboEffectBase.ComboStat[] stats) where T : ModComboEffect
        {
            item.Stats = stats;
            return item;
        }
        public static T SetDefaultEffect<T>(this T item, LocalizedString defaultEffect) where T : ModComboEffect
        {
            item.DefaultEffect = defaultEffect;
            return item;
        }
        public static T SetDefaultEffect<T>(this T item, string defaultEffect) where T : ModComboEffect
        {
            item.DefaultEffect = new LocalizedString(defaultEffect);
            return item;
        }
        public static T SetDefaultEffect<T>(this T item) where T : ModComboEffect
        {
            item.DefaultEffect = new LocalizedString("ComboEffectDefault_" + item.Name + "_Description");
            return item;
        }
        public static T SetCategories<T>(this T item, params string[] categories) where T : ModMiracle
        {
            item.Categories = categories;
            item.GiveItem = EItemGiveType.GiveItem;
            return item;
        }
        public static T SetMiracleEffects<T>(this T item, params Effect[] effects) where T : ModMiracle
        {
            item.Effects = effects;
            return item;
        }
        public static T SetManuallyGivenItems<T>(this T item, Func<ItemEntity[]> manuallyGivenItems) where T : ModMiracle
        {
            item.ManuallyGivenItems = manuallyGivenItems;
            item.GiveItem = EItemGiveType.GiveItemManually;
            return item;
        }
        public static T SetJewelryGivenItems<T>(this T item) where T : ModMiracle
        {
            item.ManuallyGivenItems = () =>
            {
                var list = new List<ItemEntity>();
                foreach (var jewelry in Data.Jewelries.Values)
                {
                    list.AddRange(jewelry.Select(x => x.ItemEntity));
                }
                return list.ToArray();
            };
            item.GiveItem = EItemGiveType.GiveItemManually;
            return item;
        }
        public static T SetNotGiveItem<T>(this T item) where T : ModMiracle
        {
            item.GiveItem = EItemGiveType.NotGiveItem;
            item.Effects = new Effect[] { CreateNegativeEffect("Miracle_Gambler_Effect2") };
            return item;
        }
        public static T SetGiveItem<T>(this T item) where T : ModMiracle
        {
            item.GiveItem = EItemGiveType.GiveItem;
            return item;
        }
        public static T SetGiveItem<T>(this T item, EItemGiveType giveItem) where T : ModMiracle
        {
            item.GiveItem = giveItem;
            return item;
        }
        public static T SetTier<T>(this T item, ETier tier) where T : ModMiracle
        {
            item.Tier = tier;
            return item;
        }
        public static T SetNotAutoGenerateEffectString<T>(this T item) where T : ModMiracleStatus
        {
            item.AutoGenerateEffectString = false;
            return item;
        }
        public static T SetNotAutoGenerateEffectString<T>(this T item, params Effect[] effects) where T : ModMiracleStatus
        {
            item.AutoGenerateEffectString = false;
            item.SetMiracleEffects(effects);
            return item;
        }
        public static T SetNotAutoGenerateEffectString<T>(this T item, int count, EEffectType type, bool notGiveItem = false) where T : ModMiracleStatus
        {
            item.AutoGenerateEffectString = false;
            var list = new List<Effect>();
            for (int q = 1; q <= count; q++)
                list.Add(CreateEffect("Miracle_" + item.Name + "_Effect" + (q == 1 ? "" : q.ToString()), type));
            if(notGiveItem)
                list.Add(CreateNegativeEffect("Miracle_Gambler_Effect2"));
            item.SetMiracleEffects(list.ToArray());
            return item;
        }
        public static T SetNotAutoGenerateEffectString<T>(this T item, int count, EEffectType[] types, bool notGiveItem = false) where T : ModMiracleStatus
        {
            item.AutoGenerateEffectString = false;
            var list = new List<Effect>();
            for (int q = 1; q <= count; q++)
                list.Add(CreateEffect("Miracle_" + item.Name + "_Effect" + (q == 1 ? "" : q.ToString()), types.SafeRandomAccess(q - 1)));
            if (notGiveItem)
                list.Add(CreateNegativeEffect("Miracle_Gambler_Effect2"));
            item.SetMiracleEffects(list.ToArray());
            return item;
        }
        public static T SetDivideForDisplay<T>(this T item, int divideForDisplay) where T : ModCustomStatus
        {
            item.DivideForDisplay = divideForDisplay;
            return item;
        }
        public static T SetPercentSymbol<T>(this T item) where T : ModCustomStatus
        {
            item.Symbol = "%";
            return item;
        }
        public static T SetSymbol<T>(this T item, string symbol) where T : ModCustomStatus
        {
            item.Symbol = symbol;
            return item;
        }
        public static T SetLocalizedSymbol<T>(this T item, string symbol) where T : ModCustomStatus
        {
            item.LocalizedSymbol = new LocalizedString(symbol);
            item.UseLocalizedSymbol = true;
            return item;
        }
        public static T SetIncludePositiveNegativeSign<T>(this T item, bool includePositiveNegativeSign = false) where T : ModCustomStatus
        {
            item.IncludePositiveNegativeSign = includePositiveNegativeSign;
            return item;
        }
        public static T SetDisplayValue<T>(this T item, bool displayValue = false) where T : ModCustomStatus
        {
            item.DisplayValue = displayValue;
            return item;
        }
        public static T DoKeyword<T>(this T item, Action<ModKeyword> action) where T : ModCustomStatus
        {
            action?.Invoke(item.Keyword);
            return item;
        }
        public static T SetHasStackText<T>(this T item, bool hasStackText = false) where T : ModEffectHUD
        {
            item.HasStackText = hasStackText;
            return item;
        }
        public static T SetNeedParseValueOnVisualText<T>(this T item, bool needParseValueOnVisualText = true) where T : ModKeyword
        {
            item.NeedParseValueOnVisualText = needParseValueOnVisualText;
            return item;
        }
        public static T SetNotDisplayDetails<T>(this T item, bool displayDetails = false) where T : ModKeyword
        {
            item.DisplayDetails = displayDetails;
            return item;
        }
        public static T SetDisplayDetails<T>(this T item, LocalizedString detailedValue) where T : ModKeyword
        {
            item.DetailedValue = detailedValue;
            return item;
        }
        public static T SetTextColor<T>(this T item, Color color) where T : ModKeyword
        {
            item.TextColor = color;
            item.TextColorOriginal = null;
            return item;
        }
        public static T SetKeywordImage<T>(this T item, Func<Sprite> image) where T : ModKeyword
        {
            item.KeywordImage = image;
            item.KeywordImageOriginal = null;
            return item;
        }
        public static T SetOriginal<T>(this T item, string keyword) where T : ModKeyword
        {
            item.TextColorOriginal = keyword;
            item.KeywordImageOriginal = keyword;
            return item;
        }
        public static T SetTextColorOriginal<T>(this T item, string keyword) where T : ModKeyword
        {
            item.TextColorOriginal = keyword;
            return item;
        }
        public static T SetKeywordImageOriginal<T>(this T item, string keyword) where T : ModKeyword
        {
            item.KeywordImageOriginal = keyword;
            return item;
        }
        public static T SetConnectedDetailEntities<T>(this T item, params string[] keywords) where T : ModKeyword
        {
            item.ConnectedDetailEntities = keywords.ToList();
            return item;
        }
        public static T SetStandardEnhancements<T>(this T item, params int[] enhancements) where T : ModWeapon
        {
            item.StandardEnhancements = enhancements.ToList();
            return item;
        }
        public static T SetMainPrefabModifier<T>(this T item, Action<WeaponSimple> modifier) where T : ModWeapon
        {
            item.MainPrefabModifier = modifier;
            return item;
        }
        public static T SetBladeSprite<T>(this T item, Vector3? pos = null) where T : ModWeapon
        {
            item.HasBladeSprite = true;
            item.BladeSpritePosition = pos;
            return item;
        }
        public static T SetBladeUnlitSprite<T>(this T item, Vector3? pos = null) where T : ModWeapon
        {
            item.HasBladeUnlitSprite = true;
            item.BladeUnlitSpritePosition = pos;
            return item;
        }
        public static T SetHeadSprite<T>(this T item, Vector3? pos = null) where T : ModWeapon
        {
            item.HasHeadSprite = true;
            item.HeadSpritePosition = pos;
            return item;
        }
        public static T SetSizeFromTextureRect<T>(this T item, bool setSizeFromTextureRect = true) where T : ModWeapon
        {
            item.SetSizeFromTextureRect = setSizeFromTextureRect;
            return item;
        }
        public static T SetEnhanceFromId<T>(this T item) where T : ModWeapon
        {
            item.EnhanceFromId = item.Dependency;
            item.Dependency = -1;
            return item;
        }
        public static T SetEnhanceFromId<T>(this T item, int enhanceFromId) where T : ModWeapon
        {
            item.EnhanceFromId = enhanceFromId;
            item.Dependency = -1;
            return item;
        }
        public static T SetBorder<T>(this T item, Vector4 border) where T : ModWeaponStaff
        {
            item.Border = border;
            return item;
        }
        public static T SetFireDataModifier<T>(this T item, ModWeapon.EAttackType type, Action<NewWeaponFireData[]> modifier) where T : ModWeapon
        {
            if(type == ModWeapon.EAttackType.Basic)
            {
                item.BasicAttacksModifier = modifier;
            }
            else if (type == ModWeapon.EAttackType.Dash)
            {
                item.DashAttacksModifier = modifier;
            }
            else if (type == ModWeapon.EAttackType.Special)
            {
                item.SpecialAttacksModifier = modifier;
            }
            return item;
        }
        public static T SetFireDataChangeSpriteFx<T>(this T item, ModWeapon.EAttackType type, int original, Func<ModSpriteFx[]> fxsFunc) where T : ModWeapon
        {
            //Core.Logger($"SetFireDataChangeSpriteFx: " + item.Name);
            if (type == ModWeapon.EAttackType.Basic)
            {
                item.BasicAttacksModifier = attacks =>
                {
                    //Core.Logger($"BasicAttacksModifier");
                    if (item.NewBasicAttacks.Count > 0)
                        return;

                    var weapon = WeaponDatabase.FindWeaponById(original);
                    //Core.Logger($"weapon {weapon}");
                    if (weapon == null || weapon.mainWeaponPrefab == null || !weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple>(out var simple))
                        return;

                    var fxs = fxsFunc.Invoke();

                    //Core.Logger($"BasicAttacksModifier] {fxs}");
                    for (int q = 0; q < fxs.Length; q++)
                    {
                        //Core.Logger($"BasicAttacksModifier] {simple.basicComboAttacks}");
                        if (simple.basicComboAttacks.Length <= q)
                            break;
                        if (simple.basicComboAttacks[q] is NewWeaponFireData_MeleeAttack melee)
                        {
                            var fire = ModWeapon.CopyNewWeaponFireData(melee);
                            if (fxs[q] != null)
                                fire.swingFxPrefab = fxs[q].ResourcePrefab;
                            item.NewBasicAttacks.Add(fire);
                        }
                    }
                    //Core.Logger($"BasicAttacksModifier] end");
                };
            }
            else if (type == ModWeapon.EAttackType.Dash)
            {
                item.DashAttacksModifier = attacks =>
                {
                    if (item.NewDashAttacks.Count > 0)
                        return;

                    var weapon = WeaponDatabase.FindWeaponById(original);
                    if (weapon == null || weapon.mainWeaponPrefab == null || !weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple>(out var simple))
                        return;
                    var fxs = fxsFunc.Invoke();

                    for (int q = 0; q < fxs.Length; q++)
                    {
                        if (simple.dashAttacks.Length <= q)
                            break;
                        if (simple.dashAttacks[q] is NewWeaponFireData_MeleeAttack melee)
                        {
                            var fire = ModWeapon.CopyNewWeaponFireData(melee);
                            if (fxs[q] != null)
                                fire.swingFxPrefab = fxs[q].ResourcePrefab;
                            item.NewDashAttacks.Add(fire);
                        }
                    }
                };
            }
            else if (type == ModWeapon.EAttackType.Special)
            {
                item.SpecialAttacksModifier = attacks =>
                {
                    if (item.NewSpecialAttacks.Count > 0)
                        return;

                    var weapon = WeaponDatabase.FindWeaponById(original);
                    if (weapon == null || weapon.mainWeaponPrefab == null || !weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple>(out var simple))
                        return;
                    var fxs = fxsFunc.Invoke();

                    for (int q = 0; q < fxs.Length; q++)
                    {
                        if (simple.specialAttacks.Length <= q)
                            break;
                        if (simple.specialAttacks[q] is NewWeaponFireData_MeleeAttack melee)
                        {
                            var fire = ModWeapon.CopyNewWeaponFireData(melee);
                            if (fxs[q] != null)
                                fire.swingFxPrefab = fxs[q].ResourcePrefab;
                            item.NewSpecialAttacks.Add(fire);
                        }
                    }
                };
            }
            return item;
        }
        public static T SetFireDataOverrideSpriteFx<T>(this T item, ModWeapon.EAttackType type, int original, bool nullToKeep, Func<ModSpriteFx[]> fxsFunc) where T : ModWeapon
        {
            //Core.Logger($"SetFireDataChangeSpriteFx: " + item.Name);
            if (type == ModWeapon.EAttackType.Basic)
            {
                item.NewBasicAttacksOverride = true;
                item.BasicAttacksModifier = attacks =>
                {
                    //Core.Logger($"BasicAttacksModifier");
                    if (item.NewBasicAttacks.Count > 0)
                        return;

                    var weapon = WeaponDatabase.FindWeaponById(original);
                    //Core.Logger($"weapon {weapon}");
                    if (weapon == null || weapon.mainWeaponPrefab == null || !weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple>(out var simple))
                        return;

                    var fxs = fxsFunc.Invoke();

                    //Core.Logger($"BasicAttacksModifier] {fxs}");
                    for (int q = 0; q < fxs.Length; q++)
                    {
                        //Core.Logger($"BasicAttacksModifier] {simple.basicComboAttacks}");
                        if (simple.basicComboAttacks.Length <= q)
                            break;
                        if (simple.basicComboAttacks[q] is NewWeaponFireData_MeleeAttack melee)
                        {
                            if (fxs[q] != null)
                            {
                                var fire = ModWeapon.CopyNewWeaponFireData(melee);
                                fire.swingFxPrefab = fxs[q].ResourcePrefab;
                                item.NewBasicAttacks.Add(fire);
                            }
                            else if (nullToKeep)
                            {
                                var fire = ModWeapon.CopyNewWeaponFireData(melee);
                                item.NewBasicAttacks.Add(fire);
                            }
                        }
                    }
                    //Core.Logger($"BasicAttacksModifier] end");
                };
            }
            else if (type == ModWeapon.EAttackType.Dash)
            {
                item.NewDashAttacksOverride = true;
                item.DashAttacksModifier = attacks =>
                {
                    if (item.NewDashAttacks.Count > 0)
                        return;

                    var weapon = WeaponDatabase.FindWeaponById(original);
                    if (weapon == null || weapon.mainWeaponPrefab == null || !weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple>(out var simple))
                        return;
                    var fxs = fxsFunc.Invoke();

                    for (int q = 0; q < fxs.Length; q++)
                    {
                        if (simple.dashAttacks.Length <= q)
                            break;
                        if (simple.dashAttacks[q] is NewWeaponFireData_MeleeAttack melee)
                        {
                            if (fxs[q] != null)
                            {
                                var fire = ModWeapon.CopyNewWeaponFireData(melee);
                                fire.swingFxPrefab = fxs[q].ResourcePrefab;
                                item.NewDashAttacks.Add(fire);
                            }
                            else if (nullToKeep)
                            {
                                var fire = ModWeapon.CopyNewWeaponFireData(melee);
                                item.NewDashAttacks.Add(fire);
                            }
                        }
                    }
                };
            }
            else if (type == ModWeapon.EAttackType.Special)
            {
                item.NewSpecialAttacksOverride = true;
                item.SpecialAttacksModifier = attacks =>
                {
                    if (item.NewSpecialAttacks.Count > 0)
                        return;

                    var weapon = WeaponDatabase.FindWeaponById(original);
                    if (weapon == null || weapon.mainWeaponPrefab == null || !weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple>(out var simple))
                        return;
                    var fxs = fxsFunc.Invoke();

                    for (int q = 0; q < fxs.Length; q++)
                    {
                        if (simple.specialAttacks.Length <= q)
                            break;
                        if (simple.specialAttacks[q] is NewWeaponFireData_MeleeAttack melee)
                        {
                            if (fxs[q] != null)
                            {
                                var fire = ModWeapon.CopyNewWeaponFireData(melee);
                                fire.swingFxPrefab = fxs[q].ResourcePrefab;
                                item.NewSpecialAttacks.Add(fire);
                            }
                            else if (nullToKeep)
                            {
                                var fire = ModWeapon.CopyNewWeaponFireData(melee);
                                item.NewSpecialAttacks.Add(fire);
                            }
                        }
                    }
                };
            }
            return item;
        }
        public static T AddFireDataChangeDamageElemental<T>(this T item, ModWeapon.EAttackType type, EDamageElementalType elemental) where T : ModWeapon
        {
            //Core.Logger($"SetFireDataChangeSpriteFx: " + item.Name);
            if (type == ModWeapon.EAttackType.Basic)
            {
                item.BasicAttacksModifier += attacks =>
                {
                    foreach(var fire in item.NewBasicAttacks)
                    {
                        fire.damageElementalType = elemental;
                    }
                };
            }
            else if (type == ModWeapon.EAttackType.Dash)
            {
                item.DashAttacksModifier += attacks =>
                {
                    foreach (var fire in item.NewDashAttacks)
                    {
                        fire.damageElementalType = elemental;
                    }
                };
            }
            else if (type == ModWeapon.EAttackType.Special)
            {
                item.SpecialAttacksModifier += attacks =>
                {
                    foreach (var fire in item.NewSpecialAttacks)
                    {
                        fire.damageElementalType = elemental;
                    }
                };
            }
            return item;
        }

        public static T SetSecondFireDataOverrideSpriteFx<T>(this T item, int original, bool nullToKeep, Func<ModSpriteFx[]> fxsFunc) where T : ModWeaponStaff
        {
            //Core.Logger($"SetFireDataChangeSpriteFx: " + item.Name);
            item.NewSecondSpecialAttacksOverride = true;
            item.SecondSpecialAttacksModifier = attacks =>
            {
                //Core.Logger($"BasicAttacksModifier");
                if (item.NewSecondSpecialAttacks.Count > 0)
                    return;

                var weapon = WeaponDatabase.FindWeaponById(original);
                //Core.Logger($"weapon {weapon}");
                if (weapon == null || weapon.mainWeaponPrefab == null || !weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_QuartterStaff>(out var simple))
                    return;

                var fxs = fxsFunc.Invoke();

                //Core.Logger($"BasicAttacksModifier] {fxs}");
                NewWeaponFireData[] array = new NewWeaponFireData[] { simple.secondSpecialAttackFireData, simple.secondSpecialAttackEnhancedFireData };
                for (int q = 0; q < fxs.Length; q++)
                {
                    //Core.Logger($"BasicAttacksModifier] {simple.basicComboAttacks}");
                    if (array.Length <= q)
                        break;
                    if (array[q] is NewWeaponFireData_MeleeAttack melee)
                    {
                        if (fxs[q] != null)
                        {
                            var fire = ModWeapon.CopyNewWeaponFireData(melee);
                            fire.swingFxPrefab = fxs[q].ResourcePrefab;
                            item.NewSecondSpecialAttacks.Add(fire);
                        }
                        else if (nullToKeep)
                        {
                            var fire = ModWeapon.CopyNewWeaponFireData(melee);
                            item.NewSecondSpecialAttacks.Add(fire);
                        }
                    }
                }
                //Core.Logger($"BasicAttacksModifier] end");
            };
            return item;
        }
        public static T AddFireDataModifier<T>(this T item, ModWeapon.EAttackType type, Action<NewWeaponFireData> action) where T : ModWeapon
        {
            //Core.Logger($"SetFireDataChangeSpriteFx: " + item.Name);
            if (type == ModWeapon.EAttackType.Basic)
            {
                item.BasicAttacksModifier += attacks =>
                {
                    foreach (var fire in item.NewBasicAttacks)
                    {
                        action?.Invoke(fire);
                    }
                };
            }
            else if (type == ModWeapon.EAttackType.Dash)
            {
                item.DashAttacksModifier += attacks =>
                {
                    foreach (var fire in item.NewDashAttacks)
                    {
                        action?.Invoke(fire);
                    }
                };
            }
            else if (type == ModWeapon.EAttackType.Special)
            {
                item.SpecialAttacksModifier += attacks =>
                {
                    foreach (var fire in item.NewSpecialAttacks)
                    {
                        action?.Invoke(fire);
                    }
                };
            }
            return item;
        }
        public static T AddFireDataModifiers<T>(this T item, Action<NewWeaponFireData> action) where T : ModWeapon
        {
            //Core.Logger($"SetFireDataChangeSpriteFx: " + item.Name);
            item.BasicAttacksModifier += attacks =>
            {
                foreach (var fire in item.NewBasicAttacks)
                {
                    action?.Invoke(fire);
                }
            };
            item.DashAttacksModifier += attacks =>
            {
                foreach (var fire in item.NewDashAttacks)
                {
                    action?.Invoke(fire);
                }
            };
            item.SpecialAttacksModifier += attacks =>
            {
                foreach (var fire in item.NewSpecialAttacks)
                {
                    action?.Invoke(fire);
                }
            };
            if(item is ModWeaponStaff staff)
            {
                staff.SecondSpecialAttacksModifier += attacks =>
                {
                    foreach (var fire in staff.NewSecondSpecialAttacks)
                    {
                        action?.Invoke(fire);
                    }
                };
            }
            return item;
        }
        public static T AddSecondFireDataModifier<T>(this T item, Action<NewWeaponFireData> action) where T : ModWeaponStaff
        {
            //Core.Logger($"SetFireDataChangeSpriteFx: " + item.Name);
            item.SecondSpecialAttacksModifier += attacks =>
            {
                foreach (var fire in item.NewSecondSpecialAttacks)
                {
                    action?.Invoke(fire);
                }
            };
            return item;
        }
        public static T SetDamageElemental<T>(this T fire, EDamageElementalType elemental) where T : NewWeaponFireData
        {
            fire.damageElementalType = elemental;
            return fire;
        }
        public static T SetRelatedStatFormula<T>(this T fire, string relatedStatFormula, bool useElemental = true) where T : NewWeaponFireData
        {
            fire.relatedStatFormula = relatedStatFormula;
            fire.useElementalTypeFromRelatedStatFormula = useElemental;
            return fire;
        }
        public static T SetRelatedStatFormulaAndChaos<T>(this T fire, Color color, string relatedStatFormula) where T : NewWeaponFireData
        {
            fire.relatedStatFormula = relatedStatFormula;
            fire.useElementalTypeFromRelatedStatFormula = false;
            fire.damageElementalType = EDamageElementalType.Chaos;
            fire.chaosDamageColor = color;
            return fire;
        }
        public static T SetDamageMultiplier<T>(this T fire, float damageMultiplier) where T : NewWeaponFireData
        {
            fire.damageMultiplier = damageMultiplier;
            return fire;
        }
        public static T SetDefaultDuration<T>(this T item, float duration) where T : CharacterBuffMod
        {
            item.defaultDuration = duration;
            return item;
        }
        public static ModPassivePerk CreatePerk<T>(this T item, EPassivePerkLv lv, string name) where T : ModPassive
        {
            var perk = ModPassivePerk.CreatePassivePerk(item, name, lv);
            if(lv == EPassivePerkLv.lv5)
            {
                item.Lv5Perk = perk;
            }
            else if(lv == EPassivePerkLv.lv10)
            {
                item.Lv10Perk = perk;
            }
            else if(lv == EPassivePerkLv.lv20)
            {
                item.Lv20Perk = perk;
            }
            return perk;
        }
        public static T SetPerkSupplier<T>(this T item, Func<GameObject, PassiveObject> supplier) where T : ModPassivePerk
        {
            item.PerkSupplier = supplier;
            return item;
        }
        public static T SetPerkSupplier<T, U>(this T item) where T : ModPassivePerk where U : PassiveObject
        {
            item.PerkSupplier = gameObject =>
            {
                var perk = gameObject.AddComponent<U>();
                return perk;
            };
            return item;
        }
        public static T SetPerkSupplierStatus<T, U>(this T item, params string[] stats) where T : ModPassivePerk where U : PassiveObject_StatusInstance
        {
            item.PerkSupplier = gameObject =>
            {
                var perk = gameObject.AddComponent<U>();
                perk.stats = stats;
                return perk;
            };
            return item;
        }
        public static T SetPerkSupplierStatus<T>(this T item, params string[] stats) where T : ModPassivePerk
        {
            item.PerkSupplier = gameObject =>
            {
                var perk = gameObject.AddComponent<PassiveObject_StatusInstance>();
                perk.stats = stats;
                return perk;
            };
            return item;
        }
        public static T SetCopyPivot<T>(this T item, bool copyPivot = true) where T : ModSpriteFx
        {
            item.CopyPivot = copyPivot;
            return item;
        }
        public static T SetFps<T>(this T item, int fps) where T : ModSpriteFx
        {
            item.Fps = fps;
            return item;
        }
        public static T SetAppearLimit<T>(this T item, int appearLimit) where T : ModSephirite
        {
            item.AppearLimit = appearLimit;
            return item;
        }


        public static Charm_StatusInstance.StatusGroup CreateStatusGroup(string id, params int[] values)
        {
            return new Charm_StatusInstance.StatusGroup() { statusID = id, valuesByLevel = values };
        }
        public static ComboEffectBase.ComboStat CreateComboStat(int count, params string[] status)
        {
            return new ComboEffectBase.ComboStat() { comboCount = count, status = status };
        }
        public static Effect CreateEffect(string effect, EEffectType effectType)
        {
            return new Effect() { effectString = new LocalizedString(effect), effectType = effectType };
        }
        public static Effect CreatePositiveEffect(string effect)
        {
            return new Effect() { effectString = new LocalizedString(effect), effectType = EEffectType.Positive };
        }
        public static Effect CreateNegativeEffect(string effect)
        {
            return new Effect() { effectString = new LocalizedString(effect), effectType = EEffectType.Negative };
        }
        public static Effect CreateNeutralEffect(string effect)
        {
            return new Effect() { effectString = new LocalizedString(effect), effectType = EEffectType.Neutral };
        }
        public static Miracle_StatusInstance.StatInfo CreateStat(string status, EEffectType type)
        {
            return new Miracle_StatusInstance.StatInfo() { type = type, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreatePositiveStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Positive, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreateNegativeStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Negative, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreateNeutralStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Neutral, status = status };
        }
    }
}
