using FMODUnity;
using MiraItemMod.Utilities;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiraItemMod.Items
{
    public class Charm_FairyJarMkII : Charm_Basic
    {
        [Header("Fairy Jar")]
        public float[] orbCreateChanceByLevel = new float[3] { 10, 15, 20 };

        public int heal = 15;

        public EventReference orbCreateSoundEvent;

        public GameObject orbPrefab;

        public override Loc.KeywordValue[] BuildKeywords(UnitAvatar avatar, int level, int virtualLevelOffset, bool showAllLevel, bool ignoreAvatarStatus)
        {
            string value = (showAllLevel ? (orbCreateChanceByLevel.SafeRandomAccess(0) + "→" + orbCreateChanceByLevel.SafeRandomAccess(maxLevel)) : orbCreateChanceByLevel.SafeRandomAccess(LevelToIdx(level)).ToString());
            return new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("CHANCE", value, Charm_Basic.GetPositiveColor(virtualLevelOffset))
            };
        }
        protected override void OnConnected(int instanceID)
        {
            base.OnConnected(instanceID);
            if(orbPrefab == null)
            {
                orbPrefab = Resources.Load<GameObject>("HPOrb");
            }
        }
        protected override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnBreakProp += OnBreakProp;
        }

        protected override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnBreakProp -= OnBreakProp;
        }

        private void OnBreakProp(BreakableProp prop, DamageInstance damage)
        {
            if (prop.propType != PropEntity.PropType.Decor && orbCreateChanceByLevel.SafeRandomAccess(CurrentLevelToIdx()).Percent())
            {
                CreateOrb(prop.transform.position);
            }
        }

        public void CreateOrb(Vector3 pos)
        {
            if (!orbCreateSoundEvent.IsNull)
            {
                RuntimeManager.PlayOneShot(orbCreateSoundEvent, pos);
            }

            if (base.isServer)
            {
                GameObject obj = UnityEngine.Object.Instantiate(orbPrefab, pos + (Vector3)UnityEngine.Random.insideUnitCircle * 0.1f + new Vector3(0f, 0.125f), Quaternion.identity);
                HPOrb component = obj.GetComponent<HPOrb>();
                component.target = base.NetworkAvatar;
                component.amount = heal;
                component.AddPhysicalForce(UnityEngine.Random.insideUnitCircle * 5.4f, UnityEngine.Random.Range(6f, 11f));
                NetworkServer.Spawn(obj, base.NetworkAvatar.gameObject);
            }
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
