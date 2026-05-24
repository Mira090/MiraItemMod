using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModWeaponStaff : ModWeapon
    {
        public Vector4? Border { get; internal set; }
        public static ModWeaponStaff CreateStaff(string name, int copy, int dependency = -1)
        {
            return new ModWeaponStaff().SetStaff(name, copy, dependency);
        }
        internal ModWeaponStaff SetStaff(string name, int copy, int dependency)
        {
            SetWeapon(name, copy, dependency);
            return this;
        }
        protected override Sprite LoadSprite(string fileName)
        {
            if (Border.HasValue)
            {
                return AssetLoader.LoadSpriteWithBorder(fileName, Border.Value);
            }
            else
            {
                return base.LoadSprite(fileName);
            }
        }

        public Action<NewWeaponFireData[]> SecondSpecialAttacksModifier { get; internal set; }
        public List<NewWeaponFireData> NewSecondSpecialAttacks { get; internal set; } = new List<NewWeaponFireData>();
        public bool NewSecondSpecialAttacksOverride { get; internal set; } = false;
        public override void OnSpriteFxRegistered()
        {
            base.OnSpriteFxRegistered();
            if (MainWeaponPrefab == null)
                return;
            if (!MainWeaponPrefab.TryGetComponent<WeaponSimple_QuartterStaff>(out var simple))
                return;


            SecondSpecialAttacksModifier?.Invoke(new NewWeaponFireData[] { simple.secondSpecialAttackFireData, simple.secondSpecialAttackEnhancedFireData });

            if (NewSecondSpecialAttacks.Count == 0)
                return;
            if (NewSecondSpecialAttacksOverride)
            {
                simple.secondSpecialAttackFireData = NewSecondSpecialAttacks.SafeRandomAccess(0);
                simple.secondSpecialAttackEnhancedFireData = NewSecondSpecialAttacks.SafeRandomAccess(1);
            }
            else
            {
                if (NewSecondSpecialAttacks.Count > 0)
                    simple.secondSpecialAttackFireData = NewSecondSpecialAttacks.SafeRandomAccess(0);
                if (NewSecondSpecialAttacks.Count > 1)
                    simple.secondSpecialAttackEnhancedFireData = NewSecondSpecialAttacks.SafeRandomAccess(1);
            }
        }
    }
}
