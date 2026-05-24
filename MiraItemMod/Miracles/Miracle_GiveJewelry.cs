using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Miracles
{
    public class Miracle_GiveJewelry : Miracle_StatusInstance, IMiracleCustomGiveItems
    {
        public Miracle Base => this;

        public bool UseCategory => false;
        public List<int> GetAllItems(bool generateInstanceID, MiracleController identity, int instanceID)
        {
            var list = new List<int>();
            foreach (var jewelry in Data.Jewelries.Values)
            {
                list.AddRange(jewelry.Select(x => x.Id));
            }
            return list;
        }
    }
}
