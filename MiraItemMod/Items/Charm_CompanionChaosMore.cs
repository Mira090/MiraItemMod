using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items
{
    public class Charm_CompanionChaosMore : Charm_StatusInstance
    {
        private List<ICompanionCharm> companions = new List<ICompanionCharm>();

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            ClearCompanion();
            SearchCompanion();
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            ClearCompanion();
        }

        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            ClearCompanion();
            if (IsEffectEnabled)
            {
                SearchCompanion();
            }
        }

        private void ClearCompanion()
        {
            foreach (ICompanionCharm companion in companions)
            {
                companion?.SetChaoticMode(false);
            }

            companions.Clear();
        }

        private void SearchCompanion()
        {
            for (int i = 0; i < NetworkAvatar.Inventory.Width; i++)
            {
                NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(i, Item.YIdx));
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is ICompanionCharm companionCharm)
                    {
                        companionCharm.SetChaoticMode(true);
                        companions.Add(companionCharm);
                    }
                }
            }
            for (int i = 0; i < NetworkAvatar.Inventory.Height; i++)
            {
                NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx, i));
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if ((bool)charm && charm is ICompanionCharm companionCharm)
                    {
                        companionCharm.SetChaoticMode(true);
                        companions.Add(companionCharm);
                    }
                }
            }
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
