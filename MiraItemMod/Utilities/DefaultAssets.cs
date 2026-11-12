using FMODUnity;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Utilities
{
    public static class DefaultAssets
    {
        public static Sprite IconInWorldPotion { get; internal set; }
        public static Sprite IconInWorldCharm { get; internal set; }
        public static Sprite IconInWorldTablet { get; internal set; }

        public static Sprite DealUICharm { get; internal set; }
        public static Sprite DealUIAbility { get; internal set; }
        public static Sprite DealUIWeapon { get; internal set; }
        public static Sprite DealUIMagic { get; internal set; }
        public static Sprite DealUIUnknown { get; internal set; }

        public static Sprite EmptyIcon { get; internal set; }

        public static EventReference DefaultEnableSound { get; internal set; }

        public static Sprite GetDealUI(DamageIdEntity.ECategory category)
        {
            return category switch
            {
                DamageIdEntity.ECategory.Weapon => DealUIWeapon,
                DamageIdEntity.ECategory.Magic => DealUIMagic,
                DamageIdEntity.ECategory.Charm => DealUICharm,
                DamageIdEntity.ECategory.Ability => DealUIAbility,
                DamageIdEntity.ECategory.Unknown => DealUIUnknown,
                _ => DealUIUnknown,
            };
        }
        public static void Initialize()
        {
            IconInWorldCharm = AssetLoader.LoadSprite(EAssetType.Misc.ToPath("ItemInWorld_Charm"));
            IconInWorldTablet = AssetLoader.LoadSprite(EAssetType.Misc.ToPath("ItemInWorld_TabletStone"));
            IconInWorldPotion = AssetLoader.LoadSprite(EAssetType.Misc.ToPath("ItemInWorld_All"));

            DealUICharm = AssetLoader.LoadSprite(EAssetType.Misc.ToPath("DealUIArtifact"));
            DealUIAbility = AssetLoader.LoadSprite(EAssetType.Misc.ToPath("DealUIAbility"));
            DealUIWeapon = AssetLoader.LoadSprite(EAssetType.Misc.ToPath("DealUIWeapon"));
            DealUIMagic = AssetLoader.LoadSprite(EAssetType.Misc.ToPath("DealUIMagic"));
            DealUIUnknown = AssetLoader.LoadSprite(EAssetType.Misc.ToPath("DealUIUnknown"));

            EmptyIcon = AssetLoader.LoadSprite(EAssetType.EffectHUD.ToPath("Empty"));
        }
        public static void Destroy()
        {
            UnityEngine.Object.Destroy(IconInWorldCharm);
            UnityEngine.Object.Destroy(IconInWorldTablet);
            UnityEngine.Object.Destroy(IconInWorldPotion);

            UnityEngine.Object.Destroy(DealUICharm);
            UnityEngine.Object.Destroy(DealUIAbility);
            UnityEngine.Object.Destroy(DealUIWeapon);
            UnityEngine.Object.Destroy(DealUIMagic);
            UnityEngine.Object.Destroy(DealUIUnknown);

            UnityEngine.Object.Destroy(EmptyIcon);
        }
    }
}
