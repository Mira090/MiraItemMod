using MiraItemMod.Items.Savvy;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Jewelry
{
    public class Charm_JewelryCoin : Charm_Jewelry
    {
        public override int ValiableMax => 2;
        protected override int FirstLevel => 0;
        protected override int SecondLevel => 0;
        protected override bool ConsumeAll => false;
        public override int MoneyPerLevel => 200;

        public int[] damage = new int[] { 0, 1, 2 };
        public int per = 100;
        public int applied = 0;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (damage.SafeRandomAccess(0).ToString() + "→" + damage.SafeRandomAccess(maxLevel)) : damage.SafeRandomAccess(LevelToIdx(level)).ToString());//"+0;-#"

            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("DAMAGE", "+" + value + "%", Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("LEAF", per.ToString())
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnMoneyChanged += OnMoneyChanged;
            Apply();
        }

        private void OnMoneyChanged(int money)
        {
            Remove();
            Apply();
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnMoneyChanged -= OnMoneyChanged;
            Remove();
        }
        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            if (Inventory == null)
                return;
            foreach (var charm in Inventory.charms)
            {
                if (charm.Value is Charm_SavvyAcademy savvy)
                {
                    savvy.release = true;
                }
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            Remove();
            Apply();
        }
        public void Apply(int level)
        {
            var money = NetworkAvatar.Money;
            applied = money / per;
            applied *= damage.SafeRandomAccess(LevelToIdx(level));
            NetworkAvatar.AddCustomStat(ECustomStat.MagicDamageBonus, applied);
        }
        public void Apply()
        {
            Apply(limitedEffectEnabledLevel);
        }
        public void Remove()
        {
            NetworkAvatar.AddCustomStat(ECustomStat.MagicDamageBonus, -applied);
            applied = 0;
        }
    }
}
