using MiraItemMod.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Registries
{
    public interface IModConfigurable
    {
        public Func<ModConfig, bool> ActivePredicate { get; set; }
        public void SetActive();
    }
}
