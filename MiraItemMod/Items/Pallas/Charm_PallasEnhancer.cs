using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Pallas
{
    public abstract class Charm_PallasEnhancer : Charm_StatusInstance
    {

        protected List<CustomPallasController> Cards = new List<CustomPallasController>();
        protected abstract ItemPosition[] GetDirections();

        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            ClearCard();
            SearchCard();
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            ClearCard();
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            ClearCard();
            if (IsEffectEnabled)
            {
                SearchCard();
            }
        }

        private void ClearCard()
        {
            foreach (var pallas in Cards)
            {
                ClearStats(pallas);
                pallas.UpdateStats();
            }

            Cards.Clear();
        }

        private void SearchCard()
        {
            ItemPosition[] array = GetDirections();
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx, Item.YIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if (charm != null && (charm is Charm_PallasCard || charm is Charm_PallasAce) && charm.TryGetComponent<CustomPallasController>(out var pallas))
                    {
                        SetStats(pallas);
                        pallas.UpdateStats();
                        Cards.Add(pallas);
                    }
                }
            }
        }
        protected abstract void SetStats(CustomPallasController card);
        protected abstract void ClearStats(CustomPallasController card);
    }
}
