using Mirror;
using MiraItemMod.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_ActiveMeteor : Charm_FireBulletInRange, IAttackableCharm
    {
        public static string DamageId = "Charm_ActiveMeteor";
        public int[] meteor = new int[] { 130, 140, 150, 170, 180, 200, 220 };
        public int[] count = new int[] { 3, 3, 4, 4, 6, 7, 9 };

        private void Awake()
        {
            coolDownTimer.time = 8f;
            //activeText = new LocalizedString("CharmActive_Effect_IceBow");
        }
        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (meteor.SafeRandomAccess(0).ToString() + "→" + meteor.SafeRandomAccess(maxLevel)) : meteor.SafeRandomAccess(LevelToIdx(level)).ToString());//"+0;-#"
            string value2 = (showAllLevel ? (count.SafeRandomAccess(0).ToString() + "→" + count.SafeRandomAccess(maxLevel)) : count.SafeRandomAccess(LevelToIdx(level)).ToString());//"+0;-#"

            return new Loc.KeywordValue[3]
            {
            new Loc.KeywordValue("DAMAGE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("COUNT", value2, Charm_Basic.GetPositiveColor(virtualLevelOffset)),
            new Loc.KeywordValue("TIME", coolDownTimer.time.ToString("0.#"))
            };
        }
        protected override void FireCastingServer(Vector3 aimedPosition, UnitAvatar aimedTarget)
        {
            if (this.GetChance())
            {
                this.SetChance(false);
                NetworkestimatedFireChanceTime = (float)NetworkTime.time;

                SpawnMeteor(count.SafeRandomAccess(CurrentLevelToIdx()));
            }
        }

        private List<UnitAvatar> SearchTarget(Vector3 aimPosition)
        {
            List<UnitAvatar> list = new List<UnitAvatar>();
            foreach (UnitAvatar allCreature in CombatManager.Instance.AllCreatures)
            {
                if ((bool)allCreature && !allCreature.IsDead && !(allCreature == NetworkAvatar) && !allCreature.canBeTarget.IsFalse() && !(allCreature.TopdownActor.YPos > 5f) && allCreature.gameObject.activeSelf && CombatManager.ContainsAttackableFaction(allCreature.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.faction) && !(Vector3.Distance(allCreature.transform.position, aimPosition) > 12f))
                {
                    list.Add(allCreature);
                }
            }

            return list;
        }

        private void SpawnMeteor(int fireCount)
        {
            StartCoroutine(SpawnMeteorCoroutine(fireCount));
        }

        private IEnumerator SpawnMeteorCoroutine(int fireCount)
        {
            for (int i = 0; i < fireCount; i++)
            {
                List<UnitAvatar> list = SearchTarget(base.WeaponController.aimedPositionClientside);
                Vector2 vector;
                if (list.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, list.Count);
                    vector = list[index].transform.position;
                    list.RemoveAt(index);
                }
                else
                {
                    vector = (Vector2)base.NetworkAvatar.transform.position + UnityEngine.Random.insideUnitCircle * 4f;
                }

                float x = ((UnityEngine.Random.Range(0, 2) == 0) ? 2f : (-2f));
                float damage = ModUtil.CalculateDamage(this);
                Bullet.Pool.Spawn(SephiriaPrefabs.MeteorBullet, vector, canBeTransparentOnMultiplayer: true, EDamageFromType.None, DamageId, damage, 25, 3f, base.NetworkAvatar, base.NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), 4.5f, vector + new Vector2(x, 0f), vector, null, HandleAttack, 0f, EDamageElementalType.Fire);
                yield return new WaitForSeconds(0.05f);
            }
        }
        private void SpawnMeteor()
        {
            List<UnitAvatar> list = SearchTarget(WeaponController.aimedPositionClientside);
            Vector2 vector;
            if (list.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, list.Count);
                vector = list[index].transform.position;
                list.RemoveAt(index);
            }
            else
            {
                vector = (Vector2)NetworkAvatar.transform.position + UnityEngine.Random.insideUnitCircle * 4f;
            }

            float x = UnityEngine.Random.Range(0, 2) == 0 ? 2f : -2f;
            float damage = ModUtil.CalculateDamage(this);
            Bullet.Pool.Spawn(SephiriaPrefabs.MeteorBullet, vector, canBeTransparentOnMultiplayer: true, EDamageFromType.None, DamageId, damage, 25, 3f, NetworkAvatar, NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), 4.5f, vector + new Vector2(x, 0f), vector, null, HandleAttack, 0f, EDamageElementalType.Fire);
        }
        private void HandleAttack(CombatBehaviour behaviour, DamageInstance instance, ProjectileBase @base)
        {
            if (behaviour is UnitAvatar unitAvatar && !instance.ignoreDebuff)
            {
                unitAvatar.ApplyDebuff(SephiriaPrefabs.Burn, base.NetworkAvatar);
            }
        }
        public new float GetDamage(UnitAvatar avatar)
        {
            float damage = (float)base.NetworkAvatar.GetCustomStat(ECustomStat.FireDamage) * (meteor.SafeRandomAccess(CurrentLevelToIdx()) / 100f);
            return damage;
        }
    }
}
