using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items.Pallas
{
    public class CustomPallasController : MonoBehaviour
    {
        public class Parameters
        {
            public int bulletDamage;
            public int defaultChance;
            public float[] throwChanceByLevel;
            public float throwIntervalTimer;
            public Parameters(int bulletDamage, int defaultChance, float[] throwChanceByLevel, float throwIntervalTimer)
            {
                this.bulletDamage = bulletDamage;
                this.defaultChance = defaultChance;
                this.throwChanceByLevel = throwChanceByLevel;
                this.throwIntervalTimer = throwIntervalTimer;
            }
        }

        public Charm_PallasCard PallasCard;
        public Charm_PallasAce PallasAce;

        public bool HasJoker = false;
        public bool HasHeart = false;
        public bool HasDiamond = false;
        public bool HasClub = false;
        public bool HasSpade = false;

        public UnitAvatar NetworkAvatar => PallasCard != null ? PallasCard.NetworkAvatar : PallasAce != null ? PallasAce.NetworkAvatar : null;

        public void Set(Charm_PallasCard card)
        {
            PallasCard = card;
        }
        public void Set(Charm_PallasAce ace)
        {
            PallasAce = ace;
        }
        public void UpdateStats()
        {
            UpdateCard();
            UpdateAce();
        }
        private void UpdateCard()
        {
            if (PallasCard == null)
                return;
            var parameters = GetCardParameters(HasDiamond, HasSpade, HasJoker);
            PallasCard.bulletDamage = parameters.bulletDamage;
            PallasCard.defaultChance = parameters.defaultChance;
            PallasCard.throwChanceByLevel = parameters.throwChanceByLevel;
            PallasCard.throwIntervalTimer.time = parameters.throwIntervalTimer;
        }

        public static Parameters GetCardParameters(bool diamond, bool spade, bool joker)
        {
            var bulletDamage = 24;
            var defaultChance = 50;
            var throwChanceByLevel = new float[] { 0f, 2.5f, 5f, 7.5f, 10f };
            var throwIntervalTimer = 0.1f;
            if (diamond)
            {
                defaultChance += 25;
            }
            if (spade)
            {
                bulletDamage += 6;
            }
            if (joker)
            {
                bulletDamage += 6;
                defaultChance += 25;
                throwChanceByLevel = throwChanceByLevel.Select(x => x * 2).ToArray();
                throwIntervalTimer *= 0.5f;
            }
            return new Parameters(bulletDamage, defaultChance, throwChanceByLevel, throwIntervalTimer);
        }

        private void UpdateAce()
        {
            if (PallasAce == null)
                return;
            var parameters = GetAceParameters(HasDiamond, HasSpade, HasJoker);
            PallasAce.bulletDamage = parameters.bulletDamage;
            PallasAce.defaultChance = parameters.defaultChance;
            PallasAce.throwChanceByLevel = parameters.throwChanceByLevel;
            PallasAce.throwIntervalTimer.time = parameters.throwIntervalTimer;
        }
        public static Parameters GetAceParameters(bool diamond, bool spade, bool joker)
        {
            var bulletDamage = 32;
            var defaultChance = 50;
            var throwChanceByLevel = new float[] { 0f, 2.5f, 5f, 7.5f, 10f };
            var throwIntervalTimer = 0.1f;
            if (diamond)
            {
                defaultChance += 25;
            }
            if (spade)
            {
                bulletDamage += 8;
            }
            if (joker)
            {
                bulletDamage += 8;
                defaultChance += 25;
                throwChanceByLevel = throwChanceByLevel.Select(x => x * 2).ToArray();
                throwIntervalTimer *= 0.5f;
            }
            return new Parameters(bulletDamage, defaultChance, throwChanceByLevel, throwIntervalTimer);
        }
    }
}
