using Mirror;
using MiraItemMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Buffs
{
    public abstract class CharacterBuffMod : CharacterBuff
    {
        public uint AssetId;
        public string id = "CUSTOM_";
        public override string ID => id;
        protected override void InitializeInner(UnitAvatar target, float amplified)
        {
            base.InitializeInner(target, amplified);
            enabled = true;
            var identity = gameObject.AddComponent<NetworkIdentity>();
            identity.SetAssetId(AssetId);
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
