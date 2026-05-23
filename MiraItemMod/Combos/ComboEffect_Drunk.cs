using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Combos
{
    public class ComboEffect_Drunk : ComboEffectBase
    {
        public int debuffActivateComboCount = 2;

        public LocalizedString debuffActivateEffectNameString = new LocalizedString("ComboEffect_DrunkActivateName");

        public LocalizedString debuffActivateEffectString = new LocalizedString("ComboEffect_DrunkActivate");
        private int stat = 0;
        private int divide = 1;

        public override Loc.KeywordValue[] BuildDefaultEffectKeyword()
        {
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("EFFECTNAME", debuffActivateEffectNameString.ToString()),
            new Loc.KeywordValue("DEVIDE", divide.ToString())
            };
        }
        protected override void OnRequestComboData(UnitAvatar avatar, List<ComboEffectElement> elements)
        {
            base.OnRequestComboData(avatar, elements);
            Loc.KeywordValue[] keywordValues = new Loc.KeywordValue[1]
            {
            new Loc.KeywordValue("EFFECTNAME", debuffActivateEffectNameString.ToString()),
            };
            elements.Add(new ComboEffectElement
            {
                comboCount = debuffActivateComboCount,
                effectName = KeywordDatabase.Convert(Loc.Convert(debuffActivateEffectString.ToString(), keywordValues), useColor: false)
            });
        }
        protected override int OnEnableEffect(int comboCount, int oldComboCount)
        {
            base.OnEnableEffect(comboCount, oldComboCount);
            int result = base.OnEnableEffect(comboCount, oldComboCount);
            if (comboCount >= debuffActivateComboCount)
            {
                result = debuffActivateComboCount;
                Networkavatar.Inventory.OnCharmEffectRefreshedForServer += OnCharmEffectRefreshedForServer;
                Networkavatar.AddCustomStat(ECustomStat.AllDamageBonus, -stat);
                stat = Mathf.Max(0, -Networkavatar.GetCustomStat(ECustomStat.DamageReduction)) / divide;
                Networkavatar.AddCustomStat(ECustomStat.AllDamageBonus, stat);
            }
            else if(comboCount < debuffActivateComboCount && oldComboCount >= debuffActivateComboCount)
            {
                Networkavatar.Inventory.OnCharmEffectRefreshedForServer -= OnCharmEffectRefreshedForServer;
                Networkavatar.AddCustomStat(ECustomStat.AllDamageBonus, -stat);
                stat = 0;
            }
            return result;
        }
        protected override void OnDisableEffect()
        {
            base.OnDisableEffect();
            if ((bool)Networkavatar)
            {
                Networkavatar.Inventory.OnCharmEffectRefreshedForServer -= OnCharmEffectRefreshedForServer;
                Networkavatar.AddCustomStat(ECustomStat.AllDamageBonus, -stat);
                stat = 0;
            }
        }

        private void OnCharmEffectRefreshedForServer()
        {
            Networkavatar.AddCustomStat(ECustomStat.AllDamageBonus, -stat);
            stat = Mathf.Max(0, -Networkavatar.GetCustomStat(ECustomStat.DamageReduction)) / divide;
            Networkavatar.AddCustomStat(ECustomStat.AllDamageBonus, stat);
        }
        public override bool Weaved()
        {
            return true;
        }
    }
}
