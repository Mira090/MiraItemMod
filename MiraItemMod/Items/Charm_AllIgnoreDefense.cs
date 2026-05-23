using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Items
{
    public class Charm_AllIgnoreDefense : Charm_StatusInstance
    {
        public int[] defense = new int[] { -10 };
        public int[] ignore = new int[] { 1, 2, 3, 4, 5 };
        public Dictionary<PlayerAvatar, int> stat = new Dictionary<PlayerAvatar, int>();
        public int GetIgnoreDefence => Mathf.Max(0, NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction) / defense.SafeRandomAccess(CurrentLevelToIdx()) * ignore.SafeRandomAccess(CurrentLevelToIdx()));
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            //var maxHP = avatar.MaxHp;
            string value = showAllLevel ? defense.SafeRandomAccess(0) + "→" + defense.SafeRandomAccess(maxLevel) : defense.SafeRandomAccess(LevelToIdx(level)).ToString();
            string value2 = showAllLevel ? ignore.SafeRandomAccess(0) + "→" + ignore.SafeRandomAccess(maxLevel) : ignore.SafeRandomAccess(LevelToIdx(level)).ToString();
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("DEFENSE", value),
            new Loc.KeywordValue("IGNORE", "+" + value2 + "%", GetPositiveColor(virtualLevelOffset))
            };
        }
        private void Awake()
        {
            showStatsEffectStringFirst = true;
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            foreach(var value in NetworkServer.connections.Values)
            {
                if (value != null && value.identity != null && value.identity.gameObject != null && value.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                {
                    stat[player] = GetIgnoreDefence;
                    player.AddCustomStatUnsafe("IGNOREDEFENSE", stat[player]);
                }
            }
        }
        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            foreach (var value in NetworkServer.connections.Values)
            {
                if (value != null && value.identity != null && value.identity.gameObject != null && value.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                {
                    if (!stat.ContainsKey(player))
                        stat[player] = 0;
                    player.AddCustomStatUnsafe("IGNOREDEFENSE", -stat[player]);
                    stat[player] = 0;
                }
            }
        }
        protected override void OnUpdatedLevel(int oldLevel, int newLevel)
        {
            base.OnUpdatedLevel(oldLevel, newLevel);
            foreach (var value in NetworkServer.connections.Values)
            {
                if (value != null && value.identity != null && value.identity.gameObject != null && value.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                {
                    if (!stat.ContainsKey(player))
                        stat[player] = 0;
                    player.AddCustomStatUnsafe("IGNOREDEFENSE", -stat[player]);
                    stat[player] = GetIgnoreDefence;
                    player.AddCustomStatUnsafe("IGNOREDEFENSE", stat[player]);
                }
            }
        }
        public override void OnCharmEffectRefreshed()
        {
            base.OnCharmEffectRefreshed();
            foreach (var value in NetworkServer.connections.Values)
            {
                if (value != null && value.identity != null && value.identity.gameObject != null && value.identity.gameObject.TryGetComponent<PlayerAvatar>(out var player))
                {
                    if (!stat.ContainsKey(player))
                        stat[player] = 0;
                    player.AddCustomStatUnsafe("IGNOREDEFENSE", -stat[player]);
                    stat[player] = GetIgnoreDefence;
                    player.AddCustomStatUnsafe("IGNOREDEFENSE", stat[player]);
                }
            }
        }
    }
}
