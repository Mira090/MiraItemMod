using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Registries
{
    public class ModMeleeCollision : ModProjectile
    {

        public string Name { get; internal set; } = "DaggerDashFx_Ice";
        public string OriginalName { get; internal set; }
        public void InitPrefab(MeleeCollision original)
        {
            var projectile = UnityEngine.Object.Instantiate(original);
            projectile.gameObject.name = Name;
            if(projectile is MeleeCollision_Circle circle)
            {
                circle.radius = 2f;
            }


            ResourcePrefab = projectile.gameObject;
        }
    }
}
