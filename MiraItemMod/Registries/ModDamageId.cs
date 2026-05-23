using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public class ModDamageId
    {
        public string Id { get; internal set; }
        public LocalizedString Name { get; internal set; }
        public DamageIdEntity.ECategory Category { get; internal set; }
        public Sprite Icon { get; internal set; }
        public string IconFileName { get; internal set; }
        public static ModDamageId CreateCharm(string id)
        {
            var damageId = new ModDamageId
            {
                Name = new LocalizedString("Item_" + id + "_Name"),
                Id = "Charm_" + id.Replace("_", ""),
                Category = DamageIdEntity.ECategory.Charm,
                IconFileName = ModUtil.MiscPath + "DealUIArtifact"
            };
            return damageId;
        }
        public static ModDamageId CreateAbility(string id)
        {
            var damageId = new ModDamageId
            {
                Name = new LocalizedString("Ability_" + id + "_Name"),
                Id = "Ability_" + id.Replace("_", ""),
                Category = DamageIdEntity.ECategory.Ability,
                IconFileName = ModUtil.MiscPath + "DealUIAbility"
            };
            return damageId;
        }
        public static ModDamageId CreateDebuff(string id)
        {
            var damageId = new ModDamageId
            {
                Name = new LocalizedString("Debuff_" + id + "_Name"),
                Id = "Debuff_" + id.Replace("_", ""),
                Category = DamageIdEntity.ECategory.Ability,
                IconFileName = ModUtil.MiscPath + "DealUIAbility"
            };
            return damageId;
        }
        public static ModDamageId CreateDebuff(string id, string key)
        {
            var damageId = new ModDamageId
            {
                Name = new LocalizedString(key),
                Id = "Debuff_" + id.Replace("_", ""),
                Category = DamageIdEntity.ECategory.Ability,
                IconFileName = ModUtil.MiscPath + "DealUIAbility"
            };
            return damageId;
        }
        public static ModDamageId CreatePerk(string id)
        {
            var damageId = new ModDamageId
            {
                Name = new LocalizedString("Perk_" + id + "_Name"),
                Id = "Perk_" + id.Replace("_", ""),
                Category = DamageIdEntity.ECategory.Ability,
                IconFileName = ModUtil.MiscPath + "DealUIAbility"
            };
            return damageId;
        }
        public DamageIdEntity CreateEntity()
        {
            var entity = ScriptableObject.CreateInstance<DamageIdEntity>();
            entity.aName = Name;
            entity.category = Category;
            entity.icon = Icon ?? SpriteLoader.LoadSprite(IconFileName);
            entity.name = Id;
            entity.id = Id;
            return entity;
        }
    }
}
