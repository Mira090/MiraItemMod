using MiraItemMod.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items
{
    public class Charm_DebuffToFrostbite : Charm_StatusInstance
    {
        public bool toFrostbite = true;
        public int RequireLevel = 2;
        public int[] count = new int[] { 0, 0, 1, 2, 4 };
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? count.SafeRandomAccess(0) + "→" + count.SafeRandomAccess(maxLevel) : count.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[]
            {
            new Loc.KeywordValue("COUNT", value, GetPositiveColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        public override string GetEffectString(int idx, int level, int virtualLevelOffset, bool showAllLevel)
        {
            if (idx == 2 && level < RequireLevel)
                return null;
            return base.GetEffectString(idx, level, virtualLevelOffset, showAllLevel);
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            HorayModAPI.OnGetDebuff += OnGetDebuff;
            NetworkAvatar.OnAddedDebuffOnTarget += OnAddedDebuff;
        }

        public CharacterDebuff GetDebuff(UnitAvatar target)
        {
            var list = new List<CharacterDebuff>
            {
                SephiriaPrefabs.Burn,
                SephiriaPrefabs.Frostbite,
                SephiriaPrefabs.Electric,
                SephiriaPrefabs.Wound,
                SephiriaPrefabs.Poison,
                SephiriaPrefabs.Plasma
            };
            foreach (var debuff in target.Debuffs)
            {
                if (debuff.CurrentStack == debuff.MaxStackCount)
                {
                    list.Remove(debuff);
                }
                if(debuff.ID == SephiriaPrefabs.Freeze.ID)
                {
                    list.Remove(SephiriaPrefabs.Frostbite);
                }
            }

            return list.GetRandom();
        }
        private void OnAddedDebuff(CharacterDebuff debuff, string id)
        {
            if (CurrentLevelToIdx() < RequireLevel)
                return;
            if (debuff.ID != SephiriaPrefabs.Freeze.ID)
                return;
            toFrostbite = false;

            var random = GetDebuff(debuff.NetworkTarget);

            for(int q = 0; q < count.SafeRandomAccess(CurrentLevelToIdx()); q++)
            {
                debuff.NetworkTarget.ApplyDebuff(random, NetworkAvatar);
            }

            toFrostbite = true;
        }

        private CharacterDebuff OnGetDebuff(CharacterDebuff debuff, UnitAvatar avatar)
        {
            if(!toFrostbite || avatar != NetworkAvatar)
                return debuff;
            if (debuff.ID == SephiriaPrefabs.Freeze.ID)
                return debuff;
            return SephiriaPrefabs.Frostbite;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            HorayModAPI.OnGetDebuff -= OnGetDebuff;
            NetworkAvatar.OnAddedDebuffOnTarget -= OnAddedDebuff;
        }
    }
}
