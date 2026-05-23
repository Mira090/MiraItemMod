using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_VariableMaxLevelWhitePaper : Charm_WhitePaper
    {
        public virtual string StatusName => "STARGAZELEVEL";
        public virtual int ValiableMax => 16;
        public int AdditionalMaxLevel { get; private set; }
        public int OriginalMaxLevel { get; private set; }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            OriginalMaxLevel = maxLevel;
            //Core.Logger($"OnConnected: {OriginalMaxLevel}");
        }
        public override int GetSubIconCount()
        {
            return 0;
        }
        public override Sprite GetSubIconImage(ItemPosition pos, bool isInstance, int idx)
        {
            return null;
        }
        public override void OnPreSetEffectRefreshed()
        {

        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            //OriginalMaxLevel = maxLevel;
            if (NetworkAvatar != null)
            {
                if(!string.IsNullOrEmpty(StatusName))
                    SetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            //maxLevel = OriginalMaxLevel;
        }
        public virtual void SetAdditionalMaxLevel(int level)
        {
            if (OriginalMaxLevel + level > ValiableMax)
            {
                level = ValiableMax - OriginalMaxLevel;
            }
            AdditionalMaxLevel = level;
            maxLevel = OriginalMaxLevel + AdditionalMaxLevel;
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            if (NetworkAvatar != null)
            {
                if (!string.IsNullOrEmpty(StatusName))
                {
                    SetAdditionalMaxLevel(NetworkAvatar.GetCustomStatUnsafe(StatusName));
                    Inventory.UpdatePing(Item.Position);
                }
            }
        }
    }
}
