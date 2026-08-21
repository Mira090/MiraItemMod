using MiraItemMod.Combos;
using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Machina
{
    public class Charm_MachinaConnectedElemental : Charm_MachinaBasic, IDependencyConditionCharm
    {
        public bool IsDependencyValid(Charm_Basic request)
        {
            if(request == null || !(request is Charm_MachinaBasic machina))
                return false;
            return GetConnectedMachinas().Contains(machina);
        }
        public int[] stat = new int[10] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
        public EDamageElementalType ElementalType = EDamageElementalType.Normal;

        public SyncList<EDamageElementalType> ElementalTypes = new SyncList<EDamageElementalType>();

        public LocalizedString statString = new LocalizedString("");
        public Charm_MachinaConnectedElemental()
        {
            InitSyncObject(ElementalTypes);
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? stat.SafeRandomAccess(0) + "→" + stat.SafeRandomAccess(maxLevel) : stat.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[] {
                new Loc.KeywordValue("DAMAGE", "+" + value, GetPositiveColor(virtualLevelOffset))
            };
        }
        public override int GetEffectStringCount()
        {
            var list = new List<EDamageElementalType>();
            foreach(var type in ElementalTypes)
            {
                if (!list.Contains(type))
                    list.Add(type);
            }
            return base.GetEffectStringCount() + list.Count;
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            idx -= base.GetEffectStringCount();
            if (idx < 0)
                return base.GetEffectString(idx + base.GetEffectStringCount(), level, virtualLevelOffset, showAllLevel);

            var list = new List<EDamageElementalType>();
            foreach (var type in ElementalTypes)
            {
                if (!list.Contains(type))
                    list.Add(type);
            }

            return KeywordDatabase.Convert(GetText(list.SafeRandomAccess(idx)), useColor: false);
        }
        protected string GetText(EDamageElementalType elemental)
        {
            return elemental switch
            {
                EDamageElementalType.Physical => "<tag=PhysicalDamage> {DAMAGE}",
                EDamageElementalType.Fire => "<tag=FireDamage> {DAMAGE}",
                EDamageElementalType.Ice => "<tag=IceDamage> {DAMAGE}",
                EDamageElementalType.Lightning => "<tag=LightningDamage> {DAMAGE}",
                EDamageElementalType.Chaos => "<tag=HighestElementalDamage> {DAMAGE}",
                _ => "??? {DAMAGE}"
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
            ElementalTypes.Clear();
        }
        protected void Apply(int value)
        {
            var charms = GetConnectedMachinas();
            foreach (var charm in charms)
            {
                if(charm is Charm_Machina machina)
                {
                    ElementalTypes.AddRange(GetElementalTypes(machina));
                }
            }
            AddStat(value);
        }
        protected void AddStat(int value)
        {
            if (ElementalTypes.Contains(EDamageElementalType.Physical))
                NetworkAvatar.AddCustomStat(ECustomStat.PhysicalDamage, value);
            if (ElementalTypes.Contains(EDamageElementalType.Fire))
                NetworkAvatar.AddCustomStat(ECustomStat.FireDamage, value);
            if (ElementalTypes.Contains(EDamageElementalType.Ice))
                NetworkAvatar.AddCustomStat(ECustomStat.IceDamage, value);
            if (ElementalTypes.Contains(EDamageElementalType.Lightning))
                NetworkAvatar.AddCustomStat(ECustomStat.LightningDamage, value);
            if (ElementalTypes.Contains(EDamageElementalType.Chaos))
                NetworkAvatar.NetworkhighestElementalBonus += value;
        }
        protected List<EDamageElementalType> GetElementalTypes(Charm_Machina machina)
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
            var results = new List<EDamageElementalType>();


            if (result == EDamageElementalType.IceAndLightning)
            {
                results.Add(EDamageElementalType.Ice);
                results.Add(EDamageElementalType.Lightning);
            }
            else if (result == EDamageElementalType.FireAndIce)
            {
                results.Add(EDamageElementalType.Fire);
                results.Add(EDamageElementalType.Ice);
            }
            else if (result == EDamageElementalType.FireAndLightning)
            {
                results.Add(EDamageElementalType.Fire);
                results.Add(EDamageElementalType.Lightning);
            }
            else
            {
                results.Add(result);
            }
            return results;
        }
    }
}
