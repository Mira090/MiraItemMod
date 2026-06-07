using MiraItemMod.Registries;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Compats
{
    public class StarsSephiriaModCompat : ModCompat
    {
        public override string ModName => "Star's SephiriaMod Mod";

        public override void OnModLoaded()
        {
            HorayModAPI.OnAllDatabasesReady += OnAllDatabasesReady;
        }

        private void OnAllDatabasesReady()
        {
            if (!HasLoaded)
                return;
            Core.LoggerFew("Comet Patch for " + ModName);
            if (!TryGetType("ModConfig", out var config))
                return;
            if (!config.TryGetStaticProperty<object>("Instance", out var instance) || instance == null)
                return;
            if (!instance.TryGetProperty<bool>("ModifyItemCategory", out var modify) || !modify)
                return;
            foreach (var item in ReflectionExtensions.GetItemDictionary().Values)
            {
                if (!item.categories.Contains(ItemCategories.SkySong))
                    continue;
                var categories = new List<string>();
                foreach (var category in item.categories)
                {
                    if (category == ItemCategories.SkySong)
                        categories.Add(ItemCategories.Comet);
                    else
                        categories.Add(category);
                }
                item.categories = categories;
            }
            var dic = ReflectionExtensions.GetItemCategories();
            if (dic.ContainsKey(ItemCategories.Comet))
            {
                if (dic[ItemCategories.Comet].comboEffectPrefab.TryGetComponent<ComboEffectBase>(out var combo))
                {
                    combo.addStatByCombo = new ComboEffectBase.ComboStat[]
                    {
                        combo.addStatByCombo[0],
                        combo.addStatByCombo[1],
                        new ComboEffectBase.ComboStat()
                        {
                            comboCount = 6,
                            status = new string[]{ "DASH_ATTACK_DAMAGE/30" }
                        },
                        new ComboEffectBase.ComboStat()
                        {
                            comboCount = 8,
                            status = new string[]{ "DASH_ATTACK_DAMAGE/40" }
                        },
                        new ComboEffectBase.ComboStat()
                        {
                            comboCount = 10,
                            status = new string[]{ "DASH_INVINCIBLE_TIME_BONUS/100", "DASH_RECOVERY_SPEED/20" }
                        },
                    };
                }
            }
        }
        public override void OnModUnloaded()
        {
            HorayModAPI.OnAllDatabasesReady -= OnAllDatabasesReady;
        }
    }
}
