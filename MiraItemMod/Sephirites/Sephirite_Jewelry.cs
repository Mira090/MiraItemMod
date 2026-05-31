using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiraItemMod.Sephirites
{
    public class Sephirite_Jewelry : Sephirite_Custom
    {
        public static int SephiriteJewelryCount = 0;
        public static bool HasSephirite(NetworkConnectionToClient client)
        {
            return SephiriteJewelryCount > 0;
        }
        protected override void OnConnected(NetworkConnectionToClient client)
        {
            if (!base.isOwned)
                return;
            SephiriteJewelryCount++;
        }
        protected override void OnDisconnected(NetworkConnectionToClient client)
        {
            if (!base.isOwned)
                return;
            SephiriteJewelryCount--;
        }
        protected override int ModifyChoiceCount(int stat)
        {
            return stat - 1;
        }
        protected override List<int> GetCharms(UnitAvatar avatar, PlayerSpawner player)
        {
            var list = new List<int>();
            foreach(var item in Data.Jewelries.Values)
            {
                list.AddRange(item.Select(x => x.Id));
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
        protected override List<int> GetDefaultItems(UnitAvatar avatar, PlayerSpawner player)
        {
            return new List<int> { Data.JewelryAll.Id };
        }
    }
}
