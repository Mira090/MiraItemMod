using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaMod.Weapons
{
    public class WeaponAddonCommon_DirectFirePlanet : WeaponAddon
    {
        public int percent = 3;
        protected override void OnEnableAddon()
        {
            base.OnEnableAddon();
            parent.Networkowner.OnBaisAttackSwing += OnBaisAttackSwing;
        }
        private void OnBaisAttackSwing(int idx)
        {
            if (UnityEngine.Random.Range(0f, 1f) > percent / 100f)
                return;
            foreach(var charm in parent.Networkowner.unitAvatar.Inventory.charms.Values)
            {
                if(charm is Charm_SummonGreenBat planet)
                {
                    var greenbat = planet.GetGreenbatObject();
                    if (greenbat == null)
                        continue;

                    greenbat.Fire(1);
                }
            }
        }
        protected override void OnDisableAddon()
        {
            base.OnDisableAddon();
            parent.Networkowner.OnBaisAttackSwing -= OnBaisAttackSwing;
        }
    }
}
