using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaMod.Sephirites
{
    public class Sephirite_Jewelry : Sephirite_Custom
    {
        public static Dictionary<NetworkConnectionToClient, int> SephiriteJewelryCount = new Dictionary<NetworkConnectionToClient, int>();
        public static bool HasSephirite(NetworkConnectionToClient client)
        {
            if (!SephiriteJewelryCount.ContainsKey(client))
                return false;
            return SephiriteJewelryCount[client] > 0;
        }
        protected override void OnConnected(NetworkConnectionToClient client)
        {
            if (!SephiriteJewelryCount.ContainsKey(client))
            {
                SephiriteJewelryCount[client] = 0;
            }
            SephiriteJewelryCount[client]++;
        }
        protected override void OnDisconnected(NetworkConnectionToClient client)
        {
            if (!SephiriteJewelryCount.ContainsKey(client))
                return;
            SephiriteJewelryCount[client]--;
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
