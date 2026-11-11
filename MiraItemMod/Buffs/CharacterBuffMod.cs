using MiraItemMod.Registries;
using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MiraItemMod.Buffs
{
    public abstract class CharacterBuffMod : CharacterBuff
    {
        public string id = "CUSTOM_";
        public override string ID => id;
        protected override void InitializeInner(UnitAvatar target, float amplified)
        {
            base.InitializeInner(target, amplified);
            //enabled = true;
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
        public virtual void OnRegister()
        {
            HorayModAPI.RegisterNetworkPrefab(gameObject, $"{Core.ModId}:{ID}");
        }
    }
}
