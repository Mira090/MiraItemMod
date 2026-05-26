using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Sephirites
{
    public class Sephirite_Bond : Sephirite_Custom
    {
        protected override int ModifyChoiceCount(int stat)
        {
            return stat - 1;
        }
        protected override List<int> GetCharms(UnitAvatar avatar, PlayerSpawner player)
        {
            return player.unlockedCharms.Where(x => ItemDatabase.FindItemById(x)?.isDual ?? false).ToList();
        }
        protected override double GetCharmProbability()
        {
            return 1.0;
        }
        protected override double GetTabletProbability()
        {
            return 0;
        }
        protected override List<int> GetDefaultItems(UnitAvatar avatar, PlayerSpawner player)
        {
            return player.unlockedCharms.ToList();
        }
    }
}
