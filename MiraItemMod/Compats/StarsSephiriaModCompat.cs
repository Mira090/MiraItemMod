using MiraItemMod.Config;
using MiraItemMod.Items;
using MiraItemMod.Registries;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Compats
{
    public class StarsSephiriaModCompat : ModCompat
    {
        public override string ModName => "Star's SephiriaMod Mod";
        public override string ModId => "stars-sephiriamod";

        public override void OnCompatLoaded()
        {
            HorayModAPI.OnLoadItemDatabase += OnLoadItemDatabase;
            HorayModAPI.OnAllDatabasesReady += OnAllDatabasesReady;
        }

        private void OnLoadItemDatabase()
        {
            if (!HasLoaded)
                return;
            Core.LoggerFew("OnLoadItemDatabase Patch for " + ModName);
            Data.ManyGrimoire.ChangeEffect(0, new LocalizedString("Item_ManyGrimoire_Effect_SPMod"));
            if(Data.MachinaElemental is ModCharmStatus status)
            {
                status.ChangeStats(
            Data.CreateStatusGroup("PHYSICAL_DAMAGE", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10),
            Data.CreateStatusGroup("FIRE_DAMAGE", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10),
            Data.CreateStatusGroup("ICE_DAMAGE", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10),
            Data.CreateStatusGroup("LIGHTNING_DAMAGE", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10),
            Data.CreateStatusGroup("WIND_DAMAGE", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10)
                );
            }
            if(Data.LegendaryMania.ResourcePrefab.TryGetComponent<Charm_LegendaryMania>(out var legendary))
            {
                legendary.stats = new Charm_LegendaryMania.StatusGroup[] {
            Charm_LegendaryMania.CreateStatusGroup("PHYSICAL_DAMAGE", 0, 1, 2),
            Charm_LegendaryMania.CreateStatusGroup("FIRE_DAMAGE", 0, 1, 2),
            Charm_LegendaryMania.CreateStatusGroup("ICE_DAMAGE", 0, 1, 2),
            Charm_LegendaryMania.CreateStatusGroup("LIGHTNING_DAMAGE", 0, 1, 2),
            Charm_LegendaryMania.CreateStatusGroup("WIND_DAMAGE", 0, 1, 2),
            Charm_LegendaryMania.CreateStatusGroup("CRITICAL", 50, 100, 150, 200),
            Charm_LegendaryMania.CreateStatusGroup("CRITICAL_DAMAGE_RATE", 0, 1, 2, 84),
            Charm_LegendaryMania.CreateStatusGroup("ATTACK_SPEED", 1, 2, 4, 8),
            Charm_LegendaryMania.CreateStatusGroup("DASH_RECOVERY_SPEED", 1, 2, 4, 8),
            Charm_LegendaryMania.CreateStatusGroup("COOLDOWN_RECOVERY_SPEED", 2, 4, 6, 8),
            Charm_LegendaryMania.CreateStatusGroup("MAX_HP", 2, 4, 6, 8),
            Charm_LegendaryMania.CreateStatusGroup("MP_REGEN", 1, 2, 3, 4),
            Charm_LegendaryMania.CreateStatusGroup("DEFENSE", 1, 2, 3, 4),
            Charm_LegendaryMania.CreateStatusGroup("EVASION", 100, 200, 300, 400),
            Charm_LegendaryMania.CreateStatusGroup("EXP_DROP", 1, 2, 3, 4),
            Charm_LegendaryMania.CreateStatusGroup("LUCK", 1, 2, 3, 4),
                };
            }
        }

        public bool GetConfig(string name, bool defaultValue)
        {
            if (!TryGetType("ModConfig", out var config))
                return defaultValue;
            if (!config.TryGetStaticProperty<object>("Instance", out var instance) || instance == null)
                return defaultValue;
            if (!instance.TryGetProperty<bool>(name, out var modify))
                return defaultValue;
            return modify;
        }
        private void OnAllDatabasesReady()
        {
            if (!HasLoaded)
                return;
            Core.LoggerFew("OnAllDatabasesReady Patch for " + ModName);
            /*
            if (!TryGetType("ModConfig", out var config))
                return;
            if (!config.TryGetStaticProperty<object>("Instance", out var instance) || instance == null)
                return;
            if (!instance.TryGetProperty<bool>("AddCategory", out var modify) || !modify)
                return;*/
            foreach (var item in ReflectionExtensions.GetItemDictionary().Values)
            {
                if (!item.categories.Contains(ItemCategories.SkySong))
                    continue;
                item.categories = item.categories.Select(x => x == ItemCategories.SkySong ? ItemCategories.Comet : x).ToList();
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
                if (dic[ItemCategories.Fortune].comboEffectPrefab.TryGetComponent<ComboEffectBase>(out var combo2))
                {
                    combo2.addStatByCombo = new ComboEffectBase.ComboStat[]
                    {
                        combo2.addStatByCombo[0],
                        combo2.addStatByCombo[1],
                        new ComboEffectBase.ComboStat()
                        {
                            comboCount = 6,
                            status = new string[]{ "LUCK/4", "MINI_BOSS_REWARD_DICE/1" }
                        },
                        new ComboEffectBase.ComboStat()
                        {
                            comboCount = 8,
                            status = new string[]{ "LUCK/8", "TRUE_DAMAGE/4" }
                        },
                    };
                }
            }
            ModifyItems();
        }
        private void ModifyItems()
        {
            if (ConfigManager.Config != null && !ConfigManager.Config.ModifyItem)
                return;
            if (ConfigManager.Config == null || (ConfigManager.Config.AddItem && ConfigManager.Config.AddVitality))
            {
                ItemDatabase.Modify(1196, Core.SetItemCategories(ItemCategories.Vitality));//生命の手
                ItemDatabase.Modify(1120, Core.SetItemCategories(ItemCategories.Vitality));//血石の指輪
            }
        }
        public override void OnCompatUnloaded()
        {
            HorayModAPI.OnLoadItemDatabase -= OnLoadItemDatabase;
            HorayModAPI.OnAllDatabasesReady -= OnAllDatabasesReady;
        }
    }
}
