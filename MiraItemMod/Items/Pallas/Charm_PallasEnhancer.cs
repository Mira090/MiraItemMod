using MiraItemMod.Items.Machina;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace MiraItemMod.Items.Pallas
{
    public abstract class Charm_PallasEnhancer : Charm_StatusInstance, IDependencyConditionCharm
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
            foreach(var charm in GetPallasCards())
            {
                if (charm != null && (charm is Charm_PallasCard || charm is Charm_PallasAce) && charm.TryGetComponent<CustomPallasController>(out var pallas))
                {
                    SetStats(pallas);
                    pallas.UpdateStats();
                    Cards.Add(pallas);
                }
            }
        }
        protected List<Charm_Basic> GetPallasCards()
        {
            var list = new List<Charm_Basic>();
            ItemPosition[] array = GetDirections();
            foreach (ItemPosition itemPosition in array)
            {
                NewItemOwnInstance newItemOwnInstance = NetworkAvatar.Inventory.FindItem(new ItemPosition(Item.XIdx, Item.YIdx) + itemPosition);
                if (newItemOwnInstance != null)
                {
                    Charm_Basic charm = newItemOwnInstance.Charm;
                    if (charm == null)
                        continue;
                    list.Add(charm);
                }
            }
            return list;
        }
        protected abstract void SetStats(CustomPallasController card);
        protected abstract void ClearStats(CustomPallasController card);


        public bool dependency;
        public List<ItemPosition> positions = new List<ItemPosition>();

        public bool IsDependencyValid(Charm_Basic request)
        {
            if (request == null)
                return false;
            if (request is Charm_PallasCard || request is Charm_PallasAce)
            {
                ItemPosition[] array = GetDirections();
                foreach (ItemPosition itemPosition in array)
                {
                    if (Item.XIdx + itemPosition.x == request.xIdx && Item.YIdx + itemPosition.y == request.yIdx)
                        return true;
                }
            }
            return false;
        }


        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            ClearCard();
            if (IsEffectEnabled)
            {
                SearchCard();
            }

            if (dependency)
            {
                ClearDependency();
                dependency = false;
            }

            if (!dependency)
            {
                ApplyDependency();
                dependency = true;
            }
        }
        public override void RefreshCharm()
        {
            base.RefreshCharm();
            if (dependency)
            {
                ClearDependency();
                dependency = false;
            }

            if (!dependency)
            {
                ApplyDependency();
                dependency = true;
            }
        }
        protected void ClearDependency()
        {
            foreach (var pos in positions)
            {
                Inventory.RemoveCharmDependency(pos, this);
            }
            positions.Clear();
        }
        protected void ApplyDependency()
        {
            foreach (var charm in GetPallasCards())
            {
                if (charm.Item != null && (charm is Charm_PallasCard || charm is Charm_PallasAce))
                {
                    Inventory.AddCharmDependency(charm.Item.Position, this);
                    positions.Add(charm.Item.Position);
                }
            }
        }
    }
}
