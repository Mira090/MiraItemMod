using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Sephirites
{
    public class Sephirite_Machina : Sephirite_Custom
    {
        public static int SephiriteMachinaCount = 0;
        public static bool HasSephirite(NetworkConnectionToClient client)
        {
            return SephiriteMachinaCount > 0;
        }
        protected override void OnConnected(NetworkConnectionToClient client)
        {
            if (!base.isOwned)
                return;
            SephiriteMachinaCount++;
        }
        protected override void OnDisconnected(NetworkConnectionToClient client)
        {
            if (!base.isOwned)
                return;
            SephiriteMachinaCount--;
        }
        protected override int ModifyChoiceCount(int stat)
        {
            return stat - 1;
        }
        protected override List<int> GetCharms(UnitAvatar avatar, PlayerSpawner player)
        {
            var list = new List<int>();
            foreach (var item in Data.All)
            {
                if (item.ItemEntity != null && item.ItemEntity.categories.Contains(ItemCategories.Machina))
                    list.Add(item.Id);
            }
            return list;
        }
        protected override double GetCharmProbability()
        {
            return 1.0;
        }
        protected override double GetTabletProbability()
        {
            return 0;
        }
    }
}
