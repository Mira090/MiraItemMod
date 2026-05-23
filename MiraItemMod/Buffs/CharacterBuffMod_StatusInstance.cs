using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BuffStatus = CharacterBuff_StatusInstance.Status;

namespace SephiriaMod.Buffs
{
    public class CharacterBuffMod_StatusInstance : CharacterBuffMod, IKeywordValueConvertible
    {

        [Serializable]
        public class BuffStatusA
        {
            public string id = "ATTACK_SPEED";

            public int value = 3;

            public StatusInstance instance;
        }


        public int maxStackCount = 1;

        public BuffStatus[] add = new BuffStatus[0];

        public override int MaxStackCount => maxStackCount;

        protected override void InitializeInner(UnitAvatar target, float amplified)
        {
            base.InitializeInner(target, amplified);
            BuffStatus[] array = add;
            foreach (BuffStatus status in array)
            {
                status.instance = StatusDatabase.CreateStatusEntity(status.id, status.value);
                status.instance.SetTarget(target);
            }
        }

        protected override void DestroyInner()
        {
            base.DestroyInner();
            BuffStatus[] array = add;
            foreach (BuffStatus obj in array)
            {
                obj.instance.ClearTarget();
                obj.instance = null;
            }
        }

        protected override void ApplyStatusInner()
        {
            base.ApplyStatusInner();
            BuffStatus[] array = add;
            foreach (BuffStatus status in array)
            {
                status.instance.SetValue((int)(status.value * Amplified * CurrentStack));
                status.instance.ApplyStatus(fromRuntime: false);
            }
        }

        protected override void RemoveStatusInner()
        {
            base.RemoveStatusInner();
            BuffStatus[] array = add;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].instance.RemoveStatus();
            }
        }

        public override List<(string, string)> ConvertValue()
        {
            List<(string, string)> list = new List<(string, string)>();
            int num = 0;
            BuffStatus[] array = add;
            foreach (BuffStatus status in array)
            {
                list.Add(("VAL" + num, status.value.ToString()));
                num++;
            }

            list.Add(("STACK", MaxStackCount.ToString()));
            return list;
        }

        public override bool Weaved()
        {
            return true;
        }
    }
}
