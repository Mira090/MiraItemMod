using MiraItemMod.Combos;
using MiraItemMod.Utilities;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaSlotElemental : Charm_MachinaBasic
    {
        public int[] stat = new int[10] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
        public EDamageElementalType ElementalType = EDamageElementalType.Normal;
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? stat.SafeRandomAccess(0) + "→" + stat.SafeRandomAccess(maxLevel) : stat.SafeRandomAccess(LevelToIdx(level)).ToString();
            EDamageElementalType elemental = EDamageElementalType.Normal;
            if (!ignoreAvatarStatus && avatar != null && avatar.Inventory != null)
            {
                var charm = avatar.Inventory.FindItem(avatar.Inventory.IdxToPos(ComboEffect_Machina.GetMachinaSlot(avatar.Inventory)));
                if (charm != null && charm.Charm is Charm_Machina machina)
                {
                    elemental = GetElementalType(machina);
                }
            }
            var tag = elemental switch
            {
                EDamageElementalType.Physical => "<tag=PhysicalDamage>",
                EDamageElementalType.Fire => "<tag=FireDamage>",
                EDamageElementalType.Ice => "<tag=IceDamage>",
                EDamageElementalType.Lightning => "<tag=LightningDamage>",
                EDamageElementalType.Chaos => "<tag=HighestElementalDamage>",
                _ => "???"
            };
            return new Loc.KeywordValue[] {
                new Loc.KeywordValue("ELEMENTAL", tag + " "),
                new Loc.KeywordValue("DAMAGE", "+" + value, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            Apply(stat.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            Clear(stat.SafeRandomAccess(CurrentLevelToIdx()));
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            Clear(stat.SafeRandomAccess(CurrentLevelToIdx()));
            Apply(stat.SafeRandomAccess(CurrentLevelToIdx()));
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            Clear(stat.SafeRandomAccess(LevelToIdx(oldLevel)));
            Apply(stat.SafeRandomAccess(LevelToIdx(newLevel)));
        }
        protected void Clear(int value)
        {
            AddStat(-value);
            ElementalType = EDamageElementalType.Normal;
        }
        protected void Apply(int value)
        {
            var charm = Inventory.FindItem(Inventory.IdxToPos(MachinaSlot));
            if (charm == null)
                return;
            if(charm.Charm is Charm_Machina machina)
            {
                ElementalType = GetElementalType(machina);
                AddStat(value);
            }
        }
        protected void AddStat(int value)
        {
            if (ElementalType == EDamageElementalType.Physical)
                NetworkAvatar.AddCustomStat(ECustomStat.PhysicalDamage, value);
            if (ElementalType == EDamageElementalType.Fire)
                NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, value);
            if (ElementalType == EDamageElementalType.Ice)
                NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, value);
            if (ElementalType == EDamageElementalType.Lightning)
                NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, value);
            if (ElementalType == EDamageElementalType.Chaos)
                NetworkAvatar.NetworkhighestElementalBonus += value;
        }
        protected EDamageElementalType GetElementalType(Charm_Machina machina)
        {
            var elemental = machina.GetDamageElementalType(machina.FireData);
            var related = machina.GetRelatedStatFormula(machina.FireData);

            var result = related switch
            {
                "FIREDAMAGE" => EDamageElementalType.Fire,
                "ICEDAMAGE" => EDamageElementalType.Ice,
                "LIGHTNINGDAMAGE" => EDamageElementalType.Lightning,
                "PHYSICALDAMAGE" => EDamageElementalType.Physical,
                "HIGHEST" => EDamageElementalType.Chaos,
                "LOWEST" => EDamageElementalType.Chaos,
                "AVERAGE" => EDamageElementalType.Chaos,
                "AVERAGEALL" => EDamageElementalType.Chaos,
                _ => EDamageElementalType.Physical,
            };
            if (string.IsNullOrEmpty(related))
            {
                result = elemental ?? result;
            }


            if(result == EDamageElementalType.IceAndLightning)
            {
                result = 50.Percent() ? EDamageElementalType.Ice : EDamageElementalType.Lightning;
            }
            else if(result == EDamageElementalType.FireAndIce)
            {
                result = 50.Percent() ? EDamageElementalType.Fire : EDamageElementalType.Ice;
            }
            else if(result == EDamageElementalType.FireAndLightning)
            {
                result = 50.Percent() ? EDamageElementalType.Fire : EDamageElementalType.Lightning;
            }
            return result;
        }
    }
}
