using Mirror;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using MiraItemMod.Registries;

namespace MiraItemMod.Buffs
{
    public abstract class CharacterBuffMod : CharacterBuff
    {
        public string id = "CUSTOM_";
        public override string ID => id;
        protected override void InitializeInner(UnitAvatar target, float amplified)
        {
            base.InitializeInner(target, amplified);
            enabled = true;
        }
        public void Init(uint assetId)
        {
            gameObject.SetAssetId(assetId);
        }
        public void SetCurrentStack(int stack)
        {
            CurrentStack = stack;
            if(CurrentStack > MaxStackCount)
            {
                CurrentStack = MaxStackCount;
            }
            if (CurrentStack <= 0)
            {
                RequestEnd();
            }
        }
    }
}
