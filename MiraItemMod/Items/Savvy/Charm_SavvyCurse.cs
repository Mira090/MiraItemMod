using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Items.Savvy
{
    public class Charm_SavvyCurse : Charm_StatusInstance
    {
        public static readonly string Crime = "CRIME";

        public int applied = 0;

        public int[] duration = new int[] { 10, 20, 30, 50 };
        public int[] damage = new int[] { 16, 22, 29, 36 };
        public int[] stack = new int[] { 1, 1, 2, 2 };

        public int[] moneyByLevel = new int[] { 400, 500, 600, 800 };

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = showAllLevel ? moneyByLevel.SafeRandomAccess(0) + "→" + moneyByLevel.SafeRandomAccess(maxLevel) : moneyByLevel.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? duration.SafeRandomAccess(0) + "→" + duration.SafeRandomAccess(maxLevel) : duration.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value3 = showAllLevel ? damage.SafeRandomAccess(0) + "→" + damage.SafeRandomAccess(maxLevel) : damage.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value4 = showAllLevel ? stack.SafeRandomAccess(0) + "→" + stack.SafeRandomAccess(maxLevel) : stack.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[4]
            {
            new Loc.KeywordValue("LEAF", "+" + value, GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DURATION", "+" + value2 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("DAMAGE", "+" + value3 + "%", GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("STACK", "+" + value4, GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            //NetworkAvatar.OnAddedBuff += OnAddedBuff;
            NetworkAvatar.OnRemovedBuff += OnRemovedBuff;
            Events.OnAppliedBuff += OnAppliedBuff;
            //NetworkAvatar.OnKillUnit += OnKillUnit;
            Apply();
        }

        private void OnAppliedBuff(CharacterBuff buff)
        {
            if (Core.LogMany)
                Core.Logger("[Charm_SavvyCurse] OnAppliedBuff: " + buff.ID);
            if (buff.NetworkTarget != NetworkAvatar)
                return;
            if (buff.ID != Crime)
                return;
            UpdateCrime(buff);

            int crime = SaveManager.CurrentRun.GetInt("CrimeCount", 0);

            NetworkAvatar.AddMoney(moneyByLevel.SafeRandomAccess(CurrentLevelToIdx()));
            this.AddRandomJewelry();
            if (crime > 1)
            {
                //NetworkAvatar.AddMoney(moneyByLevel.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            //NetworkAvatar.OnAddedBuff -= OnAddedBuff;
            NetworkAvatar.OnRemovedBuff -= OnRemovedBuff;
            Events.OnAppliedBuff -= OnAppliedBuff;
            //NetworkAvatar.OnKillUnit -= OnKillUnit;
            Remove(CurrentLevelToIdx());
        }
        private void OnKillUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (!avatar.TryGetComponent<UnitAI_NewBasic>(out var npc) || string.IsNullOrWhiteSpace(npc.socialID))
                return;
            if (RuntimeFactionManager.Instance.GetRelationValue(npc.Avatar.faction, "Player") <= 21)
                return;
        }
        private void OnRemovedBuff(CharacterBuff buff)
        {
            if (buff.ID != Crime)
                return;
            UpdateCrime();
            //BURNSTACK
            //ELECTRICSTACK
            //POISONSTACK
            //WOUNDSTACK
        }

        private void OnAddedBuff(CharacterBuff buff)
        {
            if (Core.LogMedium)
                Core.Logger("[Charm_SavvyCurse] OnAddedBuff: " + buff.ID);
            if (buff.ID != Crime)
                return;
            UpdateCrime();
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            Remove(LevelToIdx(oldLevel));
            Apply();
        }
        public void UpdateCrime()
        {
            Remove(CurrentLevelToIdx());
            Apply();
        }
        public void UpdateCrime(CharacterBuff buff)
        {
            Remove(CurrentLevelToIdx());
            Apply(buff);
        }
        public void Remove(int idx)
        {
            var crime = -applied;
            NetworkAvatar.AddCustomStatUnsafe("BURNSTACK", crime * stack.SafeRandomAccess(idx));
            NetworkAvatar.AddCustomStatUnsafe("ELECTRICSTACK", crime * stack.SafeRandomAccess(idx));
            NetworkAvatar.AddCustomStatUnsafe("POISONSTACK", crime * stack.SafeRandomAccess(idx));
            NetworkAvatar.AddCustomStatUnsafe("WOUNDSTACK", crime * stack.SafeRandomAccess(idx));

            NetworkAvatar.AddCustomStatUnsafe("DEBUFFDAMAGE", crime * damage.SafeRandomAccess(idx));
            NetworkAvatar.AddCustomStat(ECustomStat.DebuffDuration, crime * duration.SafeRandomAccess(idx));
            applied += crime;
        }
        public void Apply()
        {
            var crime = GetCrimeCount();
            NetworkAvatar.AddCustomStatUnsafe("BURNSTACK", crime * stack.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("ELECTRICSTACK", crime * stack.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("POISONSTACK", crime * stack.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("WOUNDSTACK", crime * stack.SafeRandomAccess(CurrentLevelToIdx()));

            NetworkAvatar.AddCustomStatUnsafe("DEBUFFDAMAGE", crime * damage.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStat(ECustomStat.DebuffDuration, crime * duration.SafeRandomAccess(CurrentLevelToIdx()));
            
            applied += crime;
        }
        public void Apply(CharacterBuff buff)
        {
            var crime = GetCrimeCount(buff);
            NetworkAvatar.AddCustomStatUnsafe("BURNSTACK", crime * stack.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("ELECTRICSTACK", crime * stack.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("POISONSTACK", crime * stack.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStatUnsafe("WOUNDSTACK", crime * stack.SafeRandomAccess(CurrentLevelToIdx()));

            NetworkAvatar.AddCustomStatUnsafe("DEBUFFDAMAGE", crime * damage.SafeRandomAccess(CurrentLevelToIdx()));
            NetworkAvatar.AddCustomStat(ECustomStat.DebuffDuration, crime * duration.SafeRandomAccess(CurrentLevelToIdx()));

            applied += crime;
        }
        public int GetCrimeCount()
        {
            foreach(var buff in NetworkAvatar.Buffs)
            {
                if (buff.ID == Crime)
                    return buff.CurrentStack;
            }
            return 0;
        }
        public int GetCrimeCount(CharacterBuff buff)
        {
            if (buff.ID == Crime)
                return buff.CurrentStack;
            return 0;
        }
    }
}
