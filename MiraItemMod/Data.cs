using FMODUnity;
using HarmonyLib;
using MiraItemMod.Buffs;
using MiraItemMod.Combos;
using MiraItemMod.Items;
using MiraItemMod.Items.Eternal;
using MiraItemMod.Items.Jewelry;
using MiraItemMod.Items.Pallas;
using MiraItemMod.Items.Savvy;
using MiraItemMod.Miracles;
using MiraItemMod.Passives;
using MiraItemMod.Registries;
using MiraItemMod.Sephirites;
using MiraItemMod.StatusInstances;
using MiraItemMod.Utilities;
using MiraItemMod.Weapons;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using BuffStatus = CharacterBuff_StatusInstance.Status;

namespace MiraItemMod
{
    public static class Data
    {
        public static List<ModItem> All { get; private set; } = new List<ModItem>();
        public static List<ModComboEffect> Combos { get; private set; } = new List<ModComboEffect>();
        public static List<ModEffectHUD> EffectHUDs { get; private set; } = new List<ModEffectHUD>();
        public static List<ModMiracle> Miracles { get; private set; } = new List<ModMiracle>();
        public static List<ModCustomStatus> Statuses { get; private set; } = new List<ModCustomStatus>();
        public static List<ModWeapon> Weapons { get; private set; } = new List<ModWeapon>();
        public static List<CharacterBuffMod> Buffs { get; private set; } = new List<CharacterBuffMod>();
        public static List<ModKeyword> Keywords { get; private set; } = new List<ModKeyword>();
        public static List<ModPassive> Passives { get; private set; } = new List<ModPassive>();
        public static List<ModSpriteFx> SpriteFxs { get; private set; } = new List<ModSpriteFx>();
        public static List<ModSephirite> Sephirites { get; private set; } = new List<ModSephirite>();
        public static List<ModTreeShopItem> TreeShops { get; private set; } = new List<ModTreeShopItem>();
        public static List<string> AllResourcePrefabNames { get; private set; }
        public static Dictionary<string, List<ModCharm>> Jewelries { get; private set; } = new Dictionary<string, List<ModCharm>>();
        /// <summary>
        /// Item_Malice_Name
        /// 利敵
        /// </summary>
        public static ModStoneTablet Malice { get; } = ModStoneTablet.Create("Malice", "O 4\nUP -1\nDOWN -1\nLEFT -1\nRIGHT -1", false)
            .SetRarity(EItemRarity.Common).SetTreeShopItemEntity(TreeShopItems.BossAskard);
        /// <summary>
        /// Item_Bitterness_Name
        /// 辛辣
        /// </summary>
        //public static ModStoneTablet Bitterness { get; } = ModStoneTablet.Create("Bitterness", "UP -1\nDOWN -1\nLEFT +3\nRIGHT +3", false).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_WarCrime_Name
        /// 戦犯
        /// </summary>
        public static ModStoneTablet WarCrime { get; } = ModStoneTablet.Create("WarCrime", "O 8\nUP -2\nDOWN -2\nLEFT -2\nRIGHT -2\nDIAUPLEFT -1\r\nDIAUPRIGHT -1\r\nDIADOWNLEFT -1\r\nDIADOWNRIGHT -1", false)
            .SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossAskard);
        /// <summary>
        /// Item_Transcendent_Name
        /// 超絶
        /// </summary>
        //public static ModStoneTablet Transcendent { get; } = ModStoneTablet.Create("Transcendent", "UP 5\nDOWN 5\nLEFT -2\nRIGHT -2", false).SetRarity(EItemRarity.Legend).SetCannotBeReward();
        /// <summary>
        /// Item_ReservedMPEvasion_Name
        /// 鉄扇
        /// Item_ReservedMPEvasion_FlavorText
        /// 影を起こす扇子。
        /// Item_ReservedMPEvasion_Effect
        /// <tag=MP>を{MANA}<tag=ReservedMP>して以下の効果を得る
        /// </summary>
        public static ModCharmStatus ReservedMPEvasion { get; } = ModCharmStatus.Create<Charm_ReservedMPEvasion>("ReservedMPEvasion", 4, CreateStatusGroup("EVASION", 200, 300, 400, 500, 700), CreateStatusGroup("PHYSICAL_DAMAGE", 0, 0, 1, 2, 3))
            .SetCategory(ItemCategories.Sturdy, ItemCategories.Shadow).SetSimpleEffect().SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon).SetTreeShopItemEntity(TreeShopItems.NewCharmRight1);
        /// <summary>
        /// Item_MaxHP_Name
        /// 活力のお守り
        /// Item_MaxHP_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharmStatus MaxHP { get; } = ModCharmStatus.Create("MaxHP", 1, CreateStatusGroup("MAX_HP", 5, 10))
            .SetCategory(ItemCategories.Vitality).SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_RevivePlayerHaste_Name
        /// 紅い涙
        /// Item_RevivePlayerHaste_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharmStatus RevivePlayerHaste { get; } = ModCharmStatus.Create("RevivePlayerHaste", 3, CreateStatusGroup("REVIVE_PLAYER_HASTE", 20, 30, 40, 50), CreateStatusGroup("MAX_HP", 10, 20, 30, 40))
            .SetCategory(ItemCategories.Vitality).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_AddStarRuby_Name
        /// ルビーの原石
        /// Item_AddStarRuby_FlavorText
        /// 輝く、その時を待っている
        /// Item_AddStarRuby_Effect
        /// ポーションを{QUEST}回飲むと、{REWARD}に変わります\n[ポーションを飲んだ回数：{CURRENT}]
        /// </summary>
        public static ModCharmStatus AddStarRuby { get; } = ModCharmStatus.Create<Charm_AddStarRuby>("AddStarRuby", 0, CreateStatusGroup("HP_POTION_BONUS", 20))
            .SetCategory(ItemCategories.Vitality).SetSimpleEffect().SetIsUniqueEffect().SetRarity(EItemRarity.Rare).SetTreeShopItemEntity(TreeShopItems.BossOink);
        /// <summary>
        /// Item_CreateRegenPotion_Name
        /// 再生の水筒
        /// Item_CreateRegenPotion_FlavorText
        /// こまめな水分補給を忘れずに。
        /// Item_CreateRegenPotion_Effect
        /// {REQUIRE}回ステージを移動するごとに{ITEM}を獲得する
        /// </summary>
        public static ModCharm CreateRegenPotion { get; } = ModCharmStatus.Create<Charm_CreateRegenPotion>("CreateRegenPotion", 2, CreateStatusGroup("FINAL_HP", 5, 10, 20))
            .SetCategory(ItemCategories.Vitality).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetTreeShopItemEntity(TreeShopItems.BossOink);
        /// <summary>
        /// Item_MaxHPAttack_Name
        /// 溢れる生命
        /// Item_MaxHPAttack_FlavorText
        /// フレーバーテキスト募集中
        /// Item_MaxHPAttack_Effect
        /// 敵にダメージを与える時、追加で最大<tag=HP>の{PERCENT}%のダメージを与える\n[ダメージ：{DAMAGE}]
        /// </summary>
        public static ModCharmStatus MaxHPAttack { get; } = ModCharmStatus.Create<Charm_MaxHPAttack>("MaxHPAttack", 5, CreateStatusGroup("MAX_HP", 5, 10, 15, 20, 25, 30), CreateStatusGroup("DEFENSE", -5, -5, -10, -10, -20, -20))
            .SetCategory(ItemCategories.Vitality).SetSimpleEffect().SetIsUniqueEffect().SetDamageId().SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossOink);

        /// <summary>
        /// Item_KillLuck_Name
        /// 無音の羽ペン
        /// Item_KillLuck_FlavorText
        /// そのインクは風に乗って伝わる。
        /// Item_KillLuck_Effect
        /// 敵を{DIVIDE}回倒すごとに<tag=Luck>が{LUCK}増加する\n[現在の追加幸運：{CURRENT}(<tag=Luck>{LUCK}×{COUNT}回/{DIVIDE})]
        /// </summary>
        public static ModCharmStatus KillLuck { get; } = ModCharmStatus.Create<Charm_Kill_Luck>("KillLuck", 3, CreateStatusGroup("ATTACK_SPEED", 0, 4, 8, 8))
            .SetCategory(ItemCategories.Fortune, ItemCategories.WindSong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsDual();

        /// <summary>
        /// Item_LegendaryMania_Name
        /// 英雄の剣
        /// Item_LegendaryMania_FlavorText
        /// 伝説を紡ぐ剣
        /// Item_LegendaryMania_Effect
        /// バッグの伝説アーティファクトの数だけ以下の効果を得る\n[現在の伝説アーティファクト数：{COUNT}個]
        /// </summary>
        public static ModCharm LegendaryMania { get; } = ModCharm.Create<Charm_LegendaryMania>("LegendaryMania", 2, true)
            .SetSimpleEffect().SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.NewCharmRight3);

        /// <summary>
        /// Item_LevelDistributer_Name
        /// 石版の欠片
        /// Item_LevelDistributer_FlavorText
        /// まだ熱い。
        /// Item_LevelDistributer_Effect
        /// このアーティファクトのレベルと同じだけ、上の枠を+1する。
        /// </summary>
        public static ModCharm LevelDistributer { get; } = ModCharm.Create<Charm_LevelDistributer>("LevelDistributer", 6, false)
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffect().SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_MoreStoneTablet_Name
        /// 羅針盤
        /// Item_MoreStoneTablet_FlavorText
        /// 星の向きを示す道具。
        /// Item_MoreStoneTablet_Effect
        /// 報酬で{ITEM_TYPE}が出現する確率が{DROP_PERCENT}増加
        /// </summary>
        public static ModCharm MoreStoneTablet { get; } = ModCharm.Create<Charm_MoreStoneTablet>("MoreStoneTablet", 5, true)
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffect().SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_ChaosDamage_Name
        /// 夜空の香水
        /// Item_ChaosDamage_FlavorText
        /// 景色も変わる星空の香り。
        /// </summary>
        public static ModCharm ChaosDamage { get; } = ModCharm.Create<Charm_ChaosDamage>("ChaosDamage", 5, true)
            .SetCategory(ItemCategories.Stargaze).SetEffects("Charm_FrostiumRing_Effect").SetDamageId().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_DoubleDebuffStack_Name
        /// 稲妻の彗星
        /// Item_DoubleDebuffStack_FlavorText
        /// 星の力が注がれたエネルギーの塊。
        /// </summary>
        public static ModCharm DoubleDebuffStack { get; } = ModCharmStatus.Create<Charm_VariableMaxLevel>("DoubleDebuffStack", 6,
            CreateStatusGroup("BURN_STACK", 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 5, 6, 7),
            CreateStatusGroup("ELECTRIC_STACK", 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4, 5, 6, 7))
            .SetCategory(ItemCategories.Stargaze).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_CreateStoneTablet_Name
        /// 流れ星の結晶
        /// Item_CreateStoneTablet_FlavorText
        /// 隕石が形を変えていく。
        /// Item_CreateStoneTablet_Effect
        /// 敵を{QUEST}回倒すたびに、このアイテムがある枠を-1して石版{REWARD}を獲得する。\n[敵を倒した回数：{CURRENT}]
        /// Item_CreateStoneTablet_Effect2
        /// {MAX}回発動するとこの効果は失われる。\n[現在の発動回数：{COUNT}]
        /// </summary>
        public static ModCharm CreateStoneTablet { get; } = ModCharmStatus.Create<Charm_CreateStoneTablet>("CreateStoneTablet", 0, CreateStatusGroupHide("EXP_DROP", 0, 5, 10, 15, 20, 25, 30))
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffects(2).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon).SetTreeShopItemEntity(TreeShopItems.NewCharmRight1);
        /// <summary>
        /// Item_TripleAttackDebuff_Name
        /// 魔法仕掛けの天球儀
        /// Item_TripleAttackDebuff_FlavorText
        /// 夜空に染まった球体。
        /// Item_TripleAttackDebuff_Effect
        /// 次に火属性のダメージを与えた時、<tag=Burn>デバフを付与し、次に氷属性のダメージを与えた時、<tag=Frostbite>を付与する。この次に雷属性のダメージを与えた時、<tag=Electric>を付与し、最初に戻る。[クールタイム：{INTERVAL}秒]
        /// </summary>
        public static ModCharm TripleAttackDebuff { get; } = ModCharmStatus.Create<Charm_TripleAttackDebuff>("TripleAttackDebuff", 6,
            CreateStatusGroup("FIRE_DAMAGE", 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15, 17, 20),
            CreateStatusGroup("ICE_DAMAGE", 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15, 17, 20),
            CreateStatusGroup("LIGHTNING_DAMAGE", 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15, 17, 20))
            .SetCategory(ItemCategories.Stargaze).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetTreeShopItemEntity(TreeShopItems.NewCharmRight1);
        /// <summary>
        /// Item_ChaosAttack_Name
        /// 三体模型
        /// Item_ChaosAttack_FlavorText
        /// 三つの天体の軌道。
        /// Item_ChaosAttack_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、{CHANCE}の確率で追加の<tag=Elemental_Chaos>ダメージを与える。\n[ダメージ：{DAMAGE}(<tag=FireDamage>{PERCENT}+<tag=IceDamage>{PERCENT}+<tag=LightningDamage>{PERCENT})]
        /// </summary>
        public static ModCharm ChaosAttack { get; } = ModCharm.Create<Charm_ChaosAttack>("ChaosAttack", 5, true)
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffect().SetDamageId().SetRarity(EItemRarity.Rare).SetTreeShopItemEntity(TreeShopItems.NewCharmRight1);
        /// <summary>
        /// Item_StargazeTablet_Name
        /// 星見の石版
        /// Item_StargazeTablet_FlavorText
        /// 昼でもくっきりと星が見える。
        /// Item_StargazeTablet_Effect
        /// 石版を手に入れるたびに破壊し、そのレアリティに応じたポイントを得る。\n{QUEST}ポイント得るごとに{COUNT}回、周囲8マスのうちアーティファクトがないランダムな枠1つを+{REWARD}する。\n[現在の石版ポイント：{CURRENT}]
        /// Item_StargazeTablet_Effect2
        /// <tag=WeaponAction_SpecialAttack>を<tag=Elemental_Chaos>属性に変え、バッグのアーティファクトのレベル合計の{POWER_DAMAGE}のダメージを追加する。\n[追加ダメージ：{DAMAGE}(アーティファクトのレベル合計{POWER}×{POWER_DAMAGE})]
        /// Item_StargazeTablet_Notice
        /// 石版が破壊されました
        /// </summary>
        public static ModCharm StargazeTablet { get; } = ModCharmStatus.Create<Charm_StargazeTablet>("StargazeTablet", 4, CreateStatusGroupHide("SPECIAL_ATTACK_DAMAGE", 0, 0, 0, 0, 0, 12, 18, 24, 32))
            .SetCategory(ItemCategories.Stargaze).SetSimpleEffects(1).SetIsUniqueEffect().SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.NewCharmRight1);

        /// <summary>
        /// Item_CopyAcademy_Name
        /// 原典
        /// Item_CopyAcademy_FlavorText
        /// 知識無き者の脳を蝕む
        /// Item_CopyAcademy_Effect
        /// 戦闘中に魔法を{QUEST}回使用すると、下の枠にある固有でないアカデミーアーティファクトに変わります。\n[魔法を使用した回数：{CURRENT}]
        /// </summary>
        public static ModCharm CopyAcademy { get; } = ModCharmStatus.Create<Charm_CopyAcademy>("CopyAcademy", 1, CreateStatusGroup("COOLDOWN_RECOVERY_SPEED", 20, 40))
            .SetCategory(ItemCategories.Academy).SetIsUniqueEffect().SetEffects("Charm_MagicianCoin_Effect", "Item_Copy_Academy_Effect").SetRarity(EItemRarity.Rare)
            .SetTreeShopItemEntity(TreeShopItems.NewCharmMagic2);

        /// <summary>
        /// Item_AutoBuff_Name
        /// 簡易自律魔法陣
        /// Item_AutoBuff_FlavorText
        /// IndexOutOfRangeException...
        /// Item_AutoBuff_Effect
        /// 下の枠にあるバフ魔法の<tag=Magic>を{COOLDOWN}秒ごとに自動発動する。
        /// </summary>
        public static ModCharm AutoBuff { get; } = ModCharm.Create<Charm_AutoBuff>("AutoBuff", 1, false)
            .SetCategory(ItemCategories.Academy).SetEffects("Charm_MagicianCoin_Effect", "Item_AutoBuff_Effect").SetRarity(EItemRarity.Uncommon)
            .SetTreeShopItemEntity(TreeShopItems.NewCharmMagic2);

        /// <summary>
        /// Item_AutoMagicLegend_Name
        /// 試用自律魔法陣
        /// Item_AutoMagicLegend_FlavorText
        /// NullReferenceException...
        /// Item_AutoMagicLegend_Effect
        /// 上の枠にある<tag=Magic>を{COOLDOWN}秒遅れて自動発動する。
        /// </summary>
        public static ModCharm AutoMagicLegend { get; } = ModCharm.Create<Charm_AutoMagicLegend>("AutoMagicLegend", 5, false)
            .SetCategory(ItemCategories.Academy).SetSimpleEffect().SetRarity(EItemRarity.Legend)
            .SetTreeShopItemEntity(TreeShopItems.NewCharmMagic2);

        /// <summary>
        /// Item_ManyGrimoire_Name
        /// 元素学の写本
        /// Item_ManyGrimoire_FlavorText
        /// 集めた知識が役に立つ時。
        /// Item_ManyGrimoire_Effect
        /// バッグの<tag=Magic>の数だけ<tag=FireDamage><tag=IceDamage><tag=LightningDamage>+{STATUS}
        /// Item_ManyGrimoire_Effect2
        /// 現在の<tag=Magic>数：{COUNT}個
        /// </summary>
        public static ModCharm ManyGrimoire { get; } = ModCharm.Create<Charm_ManyGrimoire>("ManyGrimoire", 4, false)
            .SetCategory(ItemCategories.Academy, ItemCategories.Elemental).SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsDual().SetIsUniqueEffect();

        /// <summary>
        /// Item_FireCooldownRecovery_Name
        /// 烈火の原子時計
        /// Item_FireCooldownRecovery_FlavorText
        /// 摩擦で回りにくくなってしまった。
        /// Item_FireCooldownRecovery_Effect
        /// {REQUIRE}回火属性ダメージを与えるたびにすべての<tag=Magic>のクールタイムを少しだけ加速させる。
        /// </summary>
        public static ModCharm FireCooldownRecovery { get; } = ModCharmStatus.Create<Charm_FireCooldownRecovery>("FireCooldownRecovery", 3, CreateStatusGroup("BURN_SPEED", 5, 10, 15, 20))
            .SetCategory(ItemCategories.Academy, ItemCategories.Ember).SetIsUniqueEffect().SetSimpleEffect().SetIsDual().SetRarity(EItemRarity.Rare).SetTreeShopItemEntity(TreeShopItems.NewCharmBond1);

        /// <summary>
        /// Item_BondMaker_Name
        /// 恋結びのリボン
        /// Item_BondMaker_FlavorText
        /// 淡い絆を結ぶ。
        /// Item_BondMaker_Effect
        /// このアーティファクトのレベルが{LEVEL}になると、このアイテムと左右の枠にあるアーティファクトは壊れる。左右の枠にあったアーティファクトのカテゴリーに含まれるランダムな絆アーティファクト1つを獲得する。
        /// </summary>
        public static ModCharm BondMaker { get; } = ModCharmStatus.Create<Charm_BondMaker>("BondMaker", 3, CreateStatusGroup("BOSS_REWARD_DICE", 1, 2), CreateStatusGroup("LUCK", 4, 8))
            .SetCategory().SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossLizardDemon);
        /// <summary>
        /// Item_WoundWeapon_Name
        /// 血の入れ墨
        /// Item_WoundWeapon_FlavorText
        /// 消えることのない呪われた傷跡。
        /// Item_WoundWeapon_Effect
        /// <tag=BasicAttackDamage>を0%にする
        /// Item_WoundWeapon_Effect2
        /// 敵を<tag=WeaponAction_DirectAttack>が命中した時、<tag=Debuff_Wound>を付与する。
        /// Item_WoundWeapon_Effect3
        /// <tag=Debuff_Wound>スタック {STACK}
        /// Item_WoundWeapon_Effect4
        /// 敵にデバフを付与するたびに<tag=WeaponAction_SpecialAttack>のコスト減少 {COST}\n<tag=WeaponAction_SpecialAttack>をするとリセットされる。\n[現在のコスト減少量：{CURRENT}]
        /// </summary>
        public static ModCharm WoundWeapon { get; } = ModCharmStatus.Create<Charm_WoundWeapon>("WoundWeapon", 5, CreateStatusGroup("DEBUFF_DAMAGE", 3, 6, 9, 12, 16, 20))//, CreateStatusGroup("BASIC_ATTACK_DAMAGE", -100, -125, -150, -175, -200), CreateStatusGroup("DASH_ATTACK_DAMAGE", -200)
            .SetCategory(ItemCategories.Curse).SetIsUniqueEffect().SetSimpleEffects(4).SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossArmadillo);
        /// <summary>
        /// Item_AddDebuffStack_Name
        /// 枯死虫
        /// Item_AddDebuffStack_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm AddDebuffStack { get; } = ModCharmStatus.Create("AddDebuffStack", 3, CreateStatusGroup("ALL_DEBUFF_STACK", 1, 2, 3, 4))
            .SetCategory(ItemCategories.Curse).SetIsUniqueEffect().SetSimpleEffects(0).SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossArmadillo);
        /// <summary>
        /// Item_MaxMPPower_Name
        /// 水神の目
        /// Item_MaxMPPower_FlavorText
        /// 水を生み出す竜の宝石。
        /// Item_MaxMPPower_Effect
        /// MP再生を最終MPに変換する。
        /// Item_MaxMPPower_Effect2
        /// <tag=WeaponAction_SpecialAttack>でダメージを与える時、最大<tag=MP>の{PERCENT}を消費して、空の<tag=MP>数値の{DAMAGE}のダメージを追加する。\n[次の追加ダメージ：およそ{MP}(空の<tag=MP>数値{DAMAGE})]
        /// </summary>
        public static ModCharm MaxMPPower { get; } = ModCharmStatus.Create<Charm_MaxMPPower>("MaxMPPower", 3, CreateStatusGroup("MP_REGEN", 5, 10, 20, 20), CreateStatusGroup("MAX_MP", 5, 10, 20, 20))
            .SetCategory(ItemCategories.Lake).SetIsUniqueEffect().SetSimpleEffects(2).SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.NewCharmLeft2);
        /// <summary>
        /// Item_FollowerDiedHeal_Name
        /// 友情のリストバンド
        /// Item_FollowerDiedHeal_FlavorText
        /// 契約とは程遠い小さな、それでも太い糸。
        /// Item_FollowerDiedHeal_Effect
        /// HP消費以外によるダメージで<tag=Follower>が倒れた時、プレイヤーの<tag=HP>を回復する。（上限を超過して回復する）\n[HP回復量：倒れた<tag=Follower>の最大<tag=HP>{HEAL}]
        /// </summary>
        public static ModCharm FollowerDiedHeal { get; } = ModCharmStatus.Create<Charm_CompanionDiedHeal>("FollowerDiedHeal", 4, CreateStatusGroup("FOLLOWER_REVIVE", 5, 10, 20, 35, 60))
            .SetCategory(ItemCategories.Companion).SetSimpleEffect().SetRarity(EItemRarity.Legend).SetIsUniqueEffect().SetTreeShopItemEntity(TreeShopItems.NewCharmRight3);
        /// <summary>
        /// Item_PallasHeart_Name
        /// ハートのカード
        /// Item_PallasHeart_FlavorText
        /// 赤いハートが描かれたトランプのカード。
        /// Item_PallasHeart_Effect
        /// 左右2マスにあるパラスのカードを強化\n強化されたパラスのカードは命中時に<tag=HP>を{HEAL}回復する（クールダウン{COOLDOWN}秒）
        /// </summary>
        public static ModCharm PallasHeart { get; } = ModCharmStatus.Create<Charm_PallasHeart>("PallasHeart", 2, CreateStatusGroup("LUCK", 1, 2, 3))
            .SetCategory(ItemCategories.Fortune).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Common).SetTreeShopItemEntity(TreeShopItems.BossLibraryGuard);
        /// <summary>
        /// Item_PallasDiamond_Name
        /// ダイヤのカード
        /// Item_PallasDiamond_FlavorText
        /// 赤いダイヤが描かれたトランプのカード。
        /// Item_PallasDiamond_Effect
        /// 上下2マスにあるパラスのカードを強化\n強化されたパラスのカードは発射確率が増加する
        /// </summary>
        public static ModCharm PallasDiamond { get; } = ModCharmStatus.Create<Charm_PallasDiamond>("PallasDiamond", 2, CreateStatusGroup("LUCK", 1, 2, 3))
            .SetCategory(ItemCategories.Fortune).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Common).SetTreeShopItemEntity(TreeShopItems.BossLibraryGuard);
        /// <summary>
        /// Item_PallasClub_Name
        /// クラブのカード
        /// Item_PallasClub_FlavorText
        /// 黒いクラブが描かれたトランプのカード。
        /// Item_PallasClub_Effect
        /// 右下または左上のマスにあるパラスのカードを強化\n強化されたパラスのカードは命中時に5秒間<tag=TrueDamage>が{BUFF}増加する（クールダウン{COOLDOWN}秒）
        /// </summary>
        public static ModCharm PallasClub { get; } = ModCharmStatus.Create<Charm_PallasClub>("PallasClub", 2, CreateStatusGroup("LUCK", 1, 2, 3))
            .SetCategory(ItemCategories.Fortune).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Common).SetTreeShopItemEntity(TreeShopItems.BossLibraryGuard);
        /// <summary>
        /// Item_PallasSpade_Name
        /// スペードのカード
        /// Item_PallasSpade_FlavorText
        /// 黒いスペードが描かれたトランプのカード。
        /// Item_PallasSpade_Effect
        /// 右上または左下のマスにあるパラスのカードを強化\n強化されたパラスのカードはダメージが増加する
        /// </summary>
        public static ModCharm PallasSpade { get; } = ModCharmStatus.Create<Charm_PallasSpade>("PallasSpade", 2, CreateStatusGroup("LUCK", 1, 2, 3))
            .SetCategory(ItemCategories.Fortune).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Common).SetTreeShopItemEntity(TreeShopItems.BossLibraryGuard);
        /// <summary>
        /// Item_PallasAce_Name
        /// パラスのエース
        /// Item_PallasAce_FlavorText
        /// スペードがどこかに行ってしまったんだ。
        /// Item_PallasAce_Effect
        /// ダッシュ攻撃時に{DEFAULT}%の確率でカードを発射
        /// Item_PallasAce_Effect2
        /// <tag=Luck>1ごとに発射確率が{CHANCE}増加（現在：{CURRENT}）\n[ダメージ：{DAMAGE}]
        /// </summary>
        public static ModCharm PallasAce { get; } = ModCharmStatus.Create<Charm_PallasAce>("PallasAce", 4, true)
            .SetCategory(ItemCategories.Fortune).SetSimpleEffects(2).SetRarity(EItemRarity.Uncommon).SetTreeShopItemEntity(TreeShopItems.BossLibraryGuard);
        /// <summary>
        /// Item_PallasJoker_Name
        /// パラスのジョーカー
        /// Item_PallasJoker_FlavorText
        /// 夢の中で失くしたカードがここにある。
        /// Item_PallasJoker_Effect
        /// 周囲8マスにあるパラスのカードを強化（強化されたパラスのカードは発射確率とダメージが増加）
        /// Item_PallasJoker_Effect2
        /// パラスのカードの発射確率が100％を超えた場合、超過した分の確率が追加のカードの発射確率に変換される。\n[発射するカードの枚数：{COUNT}]
        /// </summary>
        public static ModCharm PallasJoker { get; } = ModCharmStatus.Create<Charm_PallasJoker>("PallasJoker", 4, CreateStatusGroup("LUCK", 1, 2, 4, 7, 10))
            .SetCategory(ItemCategories.Fortune).SetIsUniqueEffect().SetSimpleEffects(2).SetDamageId().SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossLibraryGuard);
        /// <summary>
        /// Item_MiniBossDice_Name
        /// 黄金ダイス
        /// Item_MiniBossDice_FlavorText
        /// 黄金に輝くそのサイコロには運命を変える力が宿っている。
        /// </summary>
        public static ModCharm MiniBossDice { get; } = ModCharmStatus.Create("MiniBossDice", 4, CreateStatusGroup("FINAL_DAMAGE", 2, 3, 4, 6, 8), CreateStatusGroup("MINI_BOSS_REWARD_DICE", 1, 1, 2, 2, 3), CreateStatusGroup("LUCK", 0, 0, 2, 2, 4))
            .SetCategory(ItemCategories.Fortune).SetSimpleEffects(0).SetRarity(EItemRarity.Rare).SetTreeShopItemEntity(TreeShopItems.BossRootDemon);

        /// <summary>
        /// Item_ThrowGrimoire_Name
        /// 魔導書キャノン
        /// Item_ThrowGrimoire_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        //public static ModCharm ThrowGrimoire { get; } = ModCharm.Create<Charm_ThrowGrimoire>("ThrowGrimoire", 4, true)
        //.SetCategory(ItemCategories.Academy).SetSimpleEffects(2).SetDamageId().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_ElectricStun_Name
        /// ビリビリクリームクロワッサン
        /// Item_ElectricStun_FlavorText
        /// 痺れるカスタードクリーム入りクロワッサン。
        /// Item_ElectricStun_Effect
        /// 感電を付与するたびに、感電が付与されていない敵に雷属性ダメージを与える時に、気絶させる確率が{PERCENT}増加する（気絶させると確率はリセットされる）\n[現在の気絶確率：{CURRENT}]
        /// </summary>
        public static ModCharm ElectricStun { get; } = ModCharmStatus.Create<Charm_ElectricStun>("ElectricStun", 3, CreateStatusGroup("ELECTRIC_STACK", 0, 1, 1, 2))
            .SetCategory(ItemCategories.Magitech).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.NewCharmLeft1);
        /// <summary>
        /// Item_TranscendentCharm_Name
        /// 断絶の魔石
        /// Item_TranscendentCharm_FlavorText
        /// フレーバーテキスト募集中
        /// Item_TranscendentCharm_Effect
        /// 報酬で{ITEM_TYPE}が出現する確率が{DROP_PERCENT}増加
        /// Item_TranscendentCharm_Effect2
        /// インベントリに横一列に断絶が並ぶと、それらを超絶に変える
        /// Item_TranscendentCharm_Effect3
        /// バッグの断絶の数だけ<tag=PhysicalDamage> {DAMAGE}
        /// Item_TranscendentCharm_Effect4
        /// 現在の断絶の数：{COUNT}個
        /// Item_TranscendentCharm_Effect5
        /// 断絶と超絶は星見の石版に破壊されない
        /// </summary>
        //public static ModCharm TranscendentCharm { get; } = ModCharmStatus.Create<Charm_Transcendent>("TranscendentCharm", 5, CreateStatusGroup("LUCK", 5, 6, 7, 8))
        //.SetCategory(ItemCategories.Mystic).SetIsUniqueEffect().SetSimpleEffects(5).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_AttackSpeed_Name
        /// 風のお守り
        /// Item_AttackSpeed_FlavorText
        /// 嵐の前の静かな風。
        /// </summary>
        public static ModCharm AttackSpeed { get; } = ModCharmStatus.Create("AttackSpeed", 2, CreateStatusGroup("ATTACK_SPEED", 2, 4, 8))
            .SetCategory(ItemCategories.WindSong).SetIsUniqueEffect().SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_DashSpeed_Name
        /// 白いプロペラ
        /// Item_DashSpeed_FlavorText
        /// 空高く舞うフレーバーテキスト募集中
        /// </summary>
        public static ModCharm DashSpeed { get; } = ModCharmStatus.Create("DashSpeed", 3, CreateStatusGroup("DASH_SPEED", 10, 20, 30, 40), CreateStatusGroup("DASH_RECOVERY_SPEED", 5, 10, 20, 30))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon).SetTreeShopItemEntity(TreeShopItems.BossPanther);
        /// <summary>
        /// Item_DashAttackScaleUp_Name
        /// 楽譜「空」
        /// Item_DashAttackScaleUp_FlavorText
        /// 空が吹く。
        /// Item_DashAttackScaleUp_Effect
        /// <tag=WeaponAction_DashAttack>の範囲{RANGE}増加
        /// </summary>
        public static ModCharm DashAttackScaleUp { get; } = ModCharmStatus.Create<Charm_DashAttackScaleUp>("DashAttackScaleUp", 4, CreateStatusGroup("DASH_COUNT", 0, 0, 1, 1, 1))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_DashAttackMore_Name
        /// 追撃の籠手
        /// Item_DashAttackMore_FlavorText
        /// 青く透き通った空から逃れることはできない。
        /// Item_DashAttackMore_Effect
        /// <tag=WeaponAction_DashAttack>が命中した時、{PERCENT}の確率で<tag=DashCount>が1回復する（クールタイム{COOLDOWN}秒）
        /// </summary>
        public static ModCharm DashAttackMore { get; } = ModCharmStatus.Create<Charm_DashAttackMore>("DashAttackMore", 3, CreateStatusGroup("DASH_ATTACK_DAMAGE", 20, 30, 40, 50))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_OnDamagedCosumeDash_Name
        /// 翔ける羽根飾り
        /// Item_OnDamagedCosumeDash_FlavorText
        /// 傷を癒す不死鳥の羽根。
        /// Item_OnDamagedCosumeDash_Effect
        /// ダメージを受けた時、<tag=DashCount>を{DASH}消費することで受けたダメージの{PERCENT}を回復する（クールタイム{COOLDOWN}秒、<tag=DashRecovery>が適用されます）
        /// </summary>
        public static ModCharm OnDamagedCosumeDash { get; } = ModCharmStatus.Create<Charm_OnDamagedCosumeDash>("OnDamagedCosumeDash", 5, CreateStatusGroup("DASH_RECOVERY_SPEED", 0, 5, 5, 10, 10, 20))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetTreeShopItemEntity(TreeShopItems.BossPanther);
        /// <summary>
        /// Item_DoubleDash_Name
        /// 晴れ雲
        /// Item_DoubleDash_FlavorText
        /// 透き通った雲。
        /// Item_DoubleDash_Effect
        /// ダッシュした時、<tag=DashCount>が1回復し、もう一度ダッシュする
        /// </summary>
        public static ModCharm DoubleDash { get; } = ModCharmStatus.Create<Charm_DoubleDash>("DoubleDash", 2, CreateStatusGroup("PHYSICAL_DAMAGE", 2, 3, 5))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_FixedMoveSpeed_Name
        /// 歪んだ靴
        /// Item_FixedMoveSpeed_FlavorText
        /// 靴があなたを履いている。
        /// Item_FixedMoveSpeed_Effect
        /// <tag=MoveSpeed>が{PERCENT}に固定されます
        /// </summary>
        public static ModCharm FixedMoveSpeed { get; } = ModCharmStatus.Create<Charm_FixedMoveSpeed>("FixedMoveSpeed", 3, CreateStatusGroup("DASH_ATTACK_DAMAGE", 20, 40, 80, 200))
            .SetCategory(ItemCategories.SkySong).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossPanther);
        /// <summary>
        /// Item_AnotherExecution_Name
        /// ウリエルの手斧
        /// Item_AnotherExecution_FlavorText
        /// 手に持って使うには少し小さい。
        /// Item_AnotherExecution_Effect
        /// 攻撃の<tag=CriticalChance>が100％を超えた場合、超過した分の確率が<tag=MagicExecution>の発生率に変換される。
        /// Item_AnotherExecution_Effect2
        /// <tag=MagicExecution>の<tag=IgnoreDefence>が{PERCENT}増加する
        /// </summary>
        public static ModCharm AnotherExecution { get; } = ModCharmStatus.Create<Charm_AnotherExecution>("AnotherExecution", 3, CreateStatusGroup("CRITICAL", 250, 500, 750, 1000))
            .SetCategory(ItemCategories.Precision).SetIsUniqueEffect().SetSimpleEffects(1).SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossAskard);
        /// <summary>
        /// Item_ElectricCritical_Name
        /// 雷鳴の槌
        /// Item_ElectricCritical_FlavorText
        /// 騒音の苦情があったため今は使われていません。
        /// Item_ElectricCritical_Effect
        /// <tag=WeaponAction_DirectAttack>または雷属性攻撃<sprite=\"Keyword\" name=LightningDamage>の<tag=CriticalChance>が雷属性ダメージの{PERCENT}増加\n[増加確率：+{CRITICAL}%]
        /// </summary>
        public static ModCharm ElectricCritical { get; } = ModCharmStatus.Create<Charm_ElectricCritical>("ElectricCritical", 5)//, CreateStatusGroup("LIGHTNING_DAMAGE", 0, 2, 4, 6, 8, 10)
            .SetCategory(ItemCategories.Precision, ItemCategories.Magitech).SetIsUniqueEffect().SetSimpleEffect().SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_SeparateDirectAttack_Name
        /// 惜別のアレキサンドライト
        /// Item_SeparateDirectAttack_FlavorText
        /// 赤と青の扉。
        /// Item_SeparateDirectAttack_Effect
        /// 元のダメージが{DAMAGE}以上の<tag=WeaponAction_DirectAttack>を2回に分割する
        /// </summary>
        public static ModCharm SeparateDirectAttack { get; } = ModCharmStatus.Create<Charm_SeparateDirectAttack>("SeparateDirectAttack", 4, CreateStatusGroup("FINAL_WEAPONDAMAGE", 2, 4, 6, 9, 12), CreateStatusGroup("ATTACK_SPEED", 2, 3, 4, 6, 8))
            .SetCategory(ItemCategories.Sturdy, ItemCategories.WindSong).SetIsUniqueEffect().SetSimpleEffect().SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_EvasionFrost_Name
        /// 氷の簪
        /// Item_EvasionFrost_FlavorText
        /// 冷たい表情を伴った簪。
        /// Item_EvasionFrost_Effect
        /// <tag=Evasion>するたびに右の枠の<tag=FrostRelic>アーティファクトのチャージを少しだけ進める
        /// Item_EvasionFrost_Effect2
        /// <tag=Evasion>1につき{PERCENT}の確率で、<tag=FrostRelic>発動時に即時チャージされる（現在：{CURRENT}）
        /// </summary>
        public static ModCharm EvasionFrost { get; } = ModCharmStatus.Create<Charm_EvasionFrost>("EvasionFrost", 2, CreateStatusGroup("EVASION", 200, 400, 700))
            .SetCategory(ItemCategories.Frost, ItemCategories.Shadow).SetIsUniqueEffect().SetSimpleEffects(2).SetIsDual().SetRarity(EItemRarity.Rare).SetTreeShopItemEntity(TreeShopItems.NewCharmBond1);
        /// <summary>
        /// Item_SuperMeteor_Name
        /// メテオライトの指輪
        /// Item_SuperMeteor_FlavorText
        /// 煌びやかな装飾がお気に入り。
        /// Item_SuperMeteor_Effect
        /// 隕石の落下速度 {SPEED}
        /// Item_SuperMeteor_Effect2
        /// 隕石のダメージ増幅 {DAMAGE}
        /// Item_Super_Meteor_Effect3
        /// <tag=Burn>ダメージを除く<tag=FireDamage>を与えた時、{PERCENT}の確率で<tag=FireDamage>{METEOR}の隕石1個を落とす（クールタイム1秒、<tag=AttackSpeed>が適用されます）
        /// </summary>
        public static ModCharm SuperMeteor { get; } = ModCharmStatus.Create<Charm_SuperMeteor>("SuperMeteor", 5, CreateStatusGroup("ATTACK_SPEED", 2, 4, 6, 8, 12, 16))
            .SetCategory(ItemCategories.Ember, ItemCategories.WindSong).SetIsUniqueEffect().SetSimpleEffects(3).SetIsDual().SetRarity(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_BloodMp_Name
        /// 血の祭壇
        /// Item_BloodMp_FlavorText
        /// かつて魔力で甦りを果たそうとした者がいた。
        /// Item_BloodMp_Effect
        /// 血の魔力：<tag=MP>の代わりに<tag=HP>を消費する
        /// Item_BloodMp_Effect2
        /// <tag=Magic>ダメージに空の<tag=HP>数値の{PERCENT}のダメージを追加する
        /// 戦闘中、<tag=MPRegen>を<tag=HPRegen>に変換する
        /// </summary>
        public static ModCharm BloodMp { get; } = ModCharmStatus.Create<Charm_BloodMP>("BloodMp", 3, CreateStatusGroup("MAGIC_DAMAGE_BONUS", 8, 12, 18, 24), CreateStatusGroup("MAX_HP", 10, 15, 20, 25))
            .SetCategory(ItemCategories.Academy, ItemCategories.Vitality).SetIsUniqueEffect().SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_InventoryPower_Name
        /// カジノチップ
        /// Item_InventoryPower_FlavorText
        /// ここでは何の価値も無い。
        /// Item_InventoryPower_Effect
        /// 周囲8枠にあるカジノチップ以外のアーティファクトをカジノチップに変える
        /// Item_InventoryPower_Effect2
        /// 他のアーティファクトをカジノチップに変えた時、元のアーティファクトのカテゴリーに基づいて以下の効果からランダムに獲得する（発動するとこの効果を失う）
        /// </summary>
        public static ModCharm InventoryPower { get; } = ModCharmStatus.Create<Charm_InventoryPower>("InventoryPower", 0, false)
            .SetCategory().SetSimpleEffects(2).SetRarity(EItemRarity.Legend).SetTreeShopItemEntity(TreeShopItems.BossBirdDemon);
        /// <summary>
        /// Item_FirstHeal_Name
        /// 酔狂のお守り
        /// Item_FirstHeal_FlavorText
        /// このお守りを見て思い出す。
        /// Item_FirstHeal_Effect
        /// このアーティファクトを初めて獲得した時、プレイヤーの<tag=HP>を{HEAL}回復する
        /// </summary>
        public static ModCharm FirstHeal { get; } = ModCharmStatus.Create<Charm_FirstHeal>("FirstHeal", 1, CreateStatusGroup("DEFENSE", -2, -3))
            .SetCategory(ItemCategories.Drunk).SetSimpleEffect().SetRarity(EItemRarity.Common);
        /// <summary>
        /// Item_DrunkElemental_Name
        /// 虹の欠片
        /// Item_DrunkElemental_FlavorText
        /// 辛酸と共に零れた七色の欠片。
        /// </summary>
        public static ModCharm DrunkElemental { get; } = ModCharmStatus.Create("DrunkElemental", 3, CreateStatusGroup("DEFENSE", -2, -3, -5, -7),
            CreateStatusGroup("PHYSICAL_DAMAGE", 1, 2, 3, 5),
            CreateStatusGroup("FIRE_DAMAGE", 1, 2, 3, 5),
            CreateStatusGroup("ICE_DAMAGE", 1, 2, 3, 5),
            CreateStatusGroup("LIGHTNING_DAMAGE", 1, 2, 3, 5))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetRarity(EItemRarity.Uncommon);
        /// <summary>
        /// Item_ReviveHeal_Name
        /// 御神酒
        /// Item_ReviveHeal_FlavorText
        /// 狂ったフレーバーテキスト募集中
        /// Item_ReviveHeal_Effect
        /// 他のプレイヤーによって復活した時、復活させたプレイヤーのHPをこのプレイヤーの<tag=Defence>{DEFENSE}ごとに{HEAL}回復する
        /// </summary>
        public static ModCharm ReviveHeal { get; } = ModCharmStatus.Create<Charm_ReviveHeal>("ReviveHeal", 3, CreateStatusGroup("DEFENSE", -2, -5, -8, -12))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_OtherDefense_Name
        /// 先陣の盃
        /// Item_OtherDefense_FlavorText
        /// 狂ったフレーバーテキスト募集中
        /// Item_OtherDefense_Effect
        /// 他の全てのプレイヤーの<tag=Defense> {DEFENSE}
        /// </summary>
        public static ModCharm OtherDefense { get; } = ModCharmStatus.Create<Charm_OtherDefense>("OtherDefense", 5, CreateStatusGroup("DEFENSE", -1, -2, -4, -8, -10, -12))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_HalfHP_Name
        /// 百の薬
        /// Item_HalfHP_FlavorText
        /// 狂ったフレーバーテキスト募集中
        /// Item_HalfHP_Effect
        /// HPが50%以上の時、<tag=FinalDamage> {DAMAGE}
        /// Item_HalfHP_Effect2
        /// HPが50%未満の時、<tag=Toughness> {TOUGHNESS}
        /// </summary>
        public static ModCharm HalfHP { get; } = ModCharmStatus.Create<Charm_HalfHP>("HalfHP", 3, CreateStatusGroup("DEFENSE", -2, -4, -6, -8))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetSimpleEffects(2).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_AllIgnoreDefense_Name
        /// リシュリューの高級ワイン
        /// Item_AllIgnoreDefense_FlavorText
        /// 何年たっても動物を惑わせる香り。
        /// Item_AllIgnoreDefense_Effect
        /// <tag=Defense>{DEFENSE}ごとに次の効果を獲得
        /// Item_AllIgnoreDefense_Effect2
        /// 全てのプレイヤーの防御貫通<sprite=\"Keyword\" name=IgnoreDefense> {IGNORE}
        /// </summary>
        public static ModCharm AllIgnoreDefense { get; } = ModCharmStatus.Create<Charm_AllIgnoreDefense>("AllIgnoreDefense", 4, CreateStatusGroup("DEFENSE", -4, -8, -12, -16, -20), CreateStatusGroup("FINAL_DAMAGE", 0, 2, 4, 8, 12))
            .SetCategory(ItemCategories.Drunk).SetIsUniqueEffect().SetSimpleEffects(2).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_CompanionSacrifice_Name
        /// 裏サイン
        /// Item_CompanionSacrifice_FlavorText
        /// 明かしてはならない相手との契約。
        /// Item_CompanionSacrifice_Effect
        /// <tag=Defense>-50以下の時、\n<tag=FollowerDamage> {FOLLOWER1}
        /// Item_CompanionSacrifice_Effect2
        /// <tag=Defense>-100以下の時、\n<tag=FollowerDamage> {FOLLOWER2}
        /// Item_CompanionSacrifice_Effect3
        /// 死亡するダメージを受けた時、左の枠にある<tag=Follower>アーティファクトが破壊されることで、HPを{HEAL}回復し、一時的に無敵になる。
        /// </summary>
        public static ModCharm CompanionSacrifice { get; } = ModCharmStatus.Create<Charm_CompanionSacrifice>("CompanionSacrifice", 2, CreateStatusGroup("DEFENSE", -8, -15, -20))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Companion).SetIsUniqueEffect().SetSimpleEffects(3).SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_DrunkShadow_Name
        /// 黒い香車
        /// Item_DrunkShadow_FlavorText
        /// 黒く固まった一つの駒。
        /// Item_DrunkShadow_Effect
        /// 死亡するダメージを受ける時、必ず<tag=Evasion>する（クールタイム{COOLDOWN}秒、<tag=Evasion>が適用されます）
        /// </summary>
        public static ModCharm DrunkShadow { get; } = ModCharmStatus.Create<Charm_DrunkShadow>("DrunkShadow", 3, CreateStatusGroup("DEFENSE", -4, -8, -12, -20), CreateStatusGroup("PHYSICAL_DAMAGE", 2, 3, 5, 8), CreateStatusGroup("EVASION", 300, 500, 900, 1300))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Shadow).SetIsUniqueEffect().SetSimpleEffect().SetIsDual().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_DrunkEmber_Name
        /// 汽車の玩具
        /// Item_DrunkEmber_FlavorText
        /// 衝突するまで止まらないブレーキの壊れた汽車。
        /// Item_DrunkEmber_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Defense>-1につき{PERCENT}の確率で<tag=Burn>を付与する（クールタイム0.5秒）
        /// Item_DrunkEmber_Effect2
        /// 最大スタックまで<tag=Burn>デバフを付与した時、全ての<tag=Burn>デバフを解除して、火属性ダメージを与える（ダメージ：解除した<tag=Burn>デバフの数×負の<tag=Defense>{DAMAGE}）
        /// </summary>
        public static ModCharm DrunkEmber { get; } = ModCharmStatus.Create<Charm_DrunkEmber>("DrunkEmber", 6, CreateStatusGroup("DEFENSE", -2, -4, -6, -8, -12, -16, -20), CreateStatusGroup("BURN_STACK", 1, 1, 1, 2, 2, 3, 3))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Ember).SetIsUniqueEffect().SetSimpleEffects(2).SetIsDual().SetRarity(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_DrunkGlacier_Name
        /// 雪の待ち針
        /// Item_DrunkGlacier_FlavorText
        /// その雪だけは溶けることはない。
        /// Item_DrunkGlacier_Effect
        /// <tag=Freeze>のスタン効果が無くなる
        /// Item_DrunkGlacier_Effect2
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Frostbite>を付与する（クールタイム：{COOLDOWN}秒、<tag=Defense>-1につき1%早くなります）
        /// </summary>
        public static ModCharm DrunkGlacier { get; } = ModCharmStatus.Create<Charm_DrunkGlacier>("DrunkGlacier", 5, CreateStatusGroup("DEFENSE", -3, -6, -9, -12, -16, -20), CreateStatusGroup("FREEZE_THRESHOLD", 0, 0, 1, 1, 1, 2))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Glacier).SetIsUniqueEffect().SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_DrunkDarkCloud_Name
        /// 割れた電球
        /// Item_DrunkDarkCloud_FlavorText
        /// 自ら壊れ自由になった光。
        /// Item_DrunkDarkCloud_Effect2
        /// <tag=DarkCloud>の消費速度増幅 {SPEED}
        /// Item_DrunkDarkCloud_Effect
        /// <tag=DarkCloud>発動時に<tag=Defense>-1ごとに追加消費 {SERIES}
        /// </summary>
        public static ModCharm DrunkDarkCloud { get; } = ModCharmStatus.Create<Charm_DrunkDarkCloud>("DrunkDarkCloud", 2, CreateStatusGroup("DEFENSE", -10, -15, -20), CreateStatusGroup("DARK_CLOUD_SPEED", -500))
            .SetCategory(ItemCategories.Drunk, ItemCategories.DarkCloud).SetIsUniqueEffect().SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_DrunkVitality_Name
        /// 経口輸血液
        /// Item_DrunkVitality_FlavorText
        /// 美味しい。
        /// Item_DrunkVitality_Effect
        /// {ITEM}の<tag=HP>回復量 {HEAL}
        /// Item_DrunkVitality_Effect2
        /// ダメージを受けた時、攻撃者のダメージの{PERCENT}を反射する
        /// </summary>
        public static ModCharm DrunkVitality { get; } = ModCharmStatus.Create<Charm_DrunkVitality>("DrunkVitality", 2, CreateStatusGroup("DEFENSE", -2, -5, -10), CreateStatusGroup("MAX_HP", 2, 5, 10))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Vitality).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_DrunkGuardian_Name
        /// 存在の天秤
        /// Item_DrunkGuardian_FlavorText
        /// 天使と悪魔が囁く。
        /// Item_DrunkGuardian_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、追加の<tag=PhysicalDamage>を与える\n[ダメージ：{DAMAGE}（守護アーティファクトの数×酩酊アーティファクトの数{PERCENT} - <tag=Defense>の絶対値）]
        /// </summary>
        public static ModCharm DrunkGuardian { get; } = ModCharmStatus.Create<Charm_DrunkGuardian>("DrunkGuardian", 3, CreateStatusGroup("FINAL_DAMAGE", 5, 10, 15, 20))
            .SetCategory(ItemCategories.Drunk, ItemCategories.Guardian).SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_EvasionCurse_Name
        /// 暗黙の毒針
        /// Item_EvasionCurse_FlavorText
        /// 声は出ない。
        /// Item_EvasionCurse_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、{POISON}の確率で<tag=Debuff_Poison>を付与する。（クールタイム{COOLDOWN}秒）
        /// Item_EvasionCurse_Effect2
        /// <tag=Evasion>の発生率が<tag=Assasination>の発生率に変換される。（<tag=Assasination>発生率：{PERCENT}）
        /// </summary>
        public static ModCharm EvasionCurse { get; } = ModCharmStatus.Create<Charm_EvasionCurse>("EvasionCurse", 2, CreateStatusGroup("EVASION", 100, 200, 400))
            .SetCategory(ItemCategories.Curse, ItemCategories.Shadow).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_RandomDebuff_Name
        /// 赤黒いルーレット
        /// Item_RandomDebuff_FlavorText
        /// 塔の下から流れ着いた様々な思念が詰まった板。
        /// Item_RandomDebuff_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、{PERCENT}の確率でランダムな<tag=Debuff>を付与する。（<tag=Luck>で確率が増加）
        /// </summary>
        public static ModCharm RandomDebuff { get; } = ModCharmStatus.Create<Charm_RandomDebuff>("RandomDebuff", 6, CreateStatusGroup("DEBUFF_DAMAGE", 5, 8, 12, 16, 20, 25, 32), CreateStatusGroup("LUCK", 1, 1, 2, 3, 4, 6, 8))
            .SetCategory(ItemCategories.Curse, ItemCategories.Fortune).SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_DebuffToFrostbite_Name
        /// 停電灯
        /// Item_DebuffToFrostbite_FlavorText
        /// 安全な道を示す灯。
        /// Item_DebuffToFrostbite_Effect
        /// 付与する<tag=Debuff>をすべて<tag=Frostbite>に変える（このアーティファクトによる効果の場合を除く）
        /// Item_DebuffToFrostbite_Effect2
        /// <tag=Freeze>発動時、ランダムな<tag=Debuff>を{COUNT}スタック付与する
        /// </summary>
        public static ModCharm DebuffToFrostbite { get; } = ModCharmStatus.Create<Charm_DebuffToFrostbite>("DebuffToFrostbite", 4, CreateStatusGroup("FREEZE_THRESHOLD", -5))
            .SetCategory(ItemCategories.Curse, ItemCategories.Glacier).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_DashFlameSword_Name
        /// 過熱したエンジン
        /// Item_DashFlameSword_FlavorText
        /// 高い空に憧れた。
        /// Item_DashFlameSword_Effect
        /// ダッシュすると<tag=FlameSword>が発動する
        /// </summary>
        public static ModCharm DashFlameSword { get; } = ModCharmStatus.Create<Charm_DashFlameSword>("DashFlameSword", 4, CreateStatusGroup("FIRE_DAMAGE", 3, 6, 9, 12, 15), CreateStatusGroup("FLAME_SWORD_MAX", 0, 1, 2, 3, 5))
            .SetCategory(ItemCategories.SkySong, ItemCategories.FlameSword).SetIsDual().SetSimpleEffects(1).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_PlanetStargaze_Name
        /// リリィの星図
        /// Item_PlanetStargaze_FlavorText
        /// 光に込められた破れかけの形。
        /// Item_PlanetStargaze_Effect
        /// 夜空のコンボ効果が<tag=WeaponAction_SpecialAttack>ではなく惑星の攻撃で発動する
        /// Item_PlanetStargaze_Effect2
        /// 周囲8枠にある惑星の数だけこのアーティファクトの最大レベル {LEVEL}
        /// </summary>
        public static ModCharm PlanetStargaze { get; } = ModCharmStatus.Create<Charm_PlanetStargaze>("PlanetStargaze", 8, CreateStatusGroup("PLANET_DAMAGE", 5, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80, 100, 120, 140, 160, 180, 200, 225, 250, 275, 300))
            .SetCategory(ItemCategories.Planet, ItemCategories.Stargaze).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_PlanetMystic_Name
        /// 夕焼けの星座
        /// Item_PlanetMystic_FlavorText
        /// 赤くぼんやりと見える。
        /// Item_PlanetMystic_Effect
        /// 神秘のコンボ効果の枠に置いた惑星を巨大化
        /// Item_PlanetMystic_Effect2
        /// 神秘のコンボ効果の枠に置いた惑星のレベル合計1につき惑星攻撃速度 {SPEED}\n[現在：{CURRENT}]
        /// </summary>
        public static ModCharm PlanetMystic { get; } = ModCharmStatus.Create<Charm_PlanetMystic>("PlanetMystic", 4, CreateStatusGroup("PLANET_DAMAGE", 2, 4, 6, 8, 10))
            .SetCategory(ItemCategories.Planet, ItemCategories.Mystic).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_GuardianDarkCloud_Name
        /// オーロラのヴェール
        /// Item_GuardianDarkCloud_FlavorText
        /// フレーバーテキスト募集中
        /// Item_GuardianDarkCloud_Effect
        /// 攻撃を受けた時の無敵時間の<tag=DarkCloud>の消費速度 {PERCENT}
        /// </summary>
        public static ModCharm GuardianDarkCloud { get; } = ModCharmStatus.Create<Charm_GuardianDarkCloud>("GuardianDarkCloud", 4, CreateStatusGroup("DARK_CLOUD_DAMAGE", 10, 25, 40, 60, 90), CreateStatusGroup("DEFENSE", 2, 3, 4, 6, 9))
            .SetCategory(ItemCategories.Guardian, ItemCategories.DarkCloud).SetIsDual().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_ReddewMagicExecution_Name
        /// 赤い山頂
        /// Item_ReddewMagicExecution_FlavorText
        /// フレーバーテキスト募集中
        /// Item_ReddewMagicExecution_Effect
        /// <tag=MagicExecution>によって{ITEM}が発動した時、最も高い属性値の{PERCENT}の追加ダメージを{COUNT}回与える
        /// </summary>
        public static ModCharm ReddewMagicExecution { get; } = ModCharmStatus.Create<Charm_ReddewMagicExecution>("ReddewMagicExecution", 2, CreateStatusGroup("CRITICAL", 300, 600, 1000), CreateStatusGroup("HIGHEST_ELEMENTAL_DAMAGE", 1, 3, 5))
            .SetCategory(ItemCategories.Precision, ItemCategories.Elemental).SetIsDual().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsUniqueEffect().SetDamageId();
        /// <summary>
        /// Item_MagitechFlameSword_Name
        /// 燃える日時計
        /// Item_MagitechFlameSword_FlavorText
        /// フレーバーテキスト募集中
        /// Item_MagitechFlameSword_Effect
        /// <tag=WeaponAction_DirectAttack>や<tag=Magic>によって<tag=FlameSword>が発動しなくなる。かわりに、<tag=Electric>ダメージを与えると<tag=FlameSword>を最大<tag=Electric>スタックの分だけ投げる
        /// </summary>
        public static ModCharm MagitechFlameSword { get; } = ModCharmStatus.Create<Charm_MagitechFlameSword>("MagitechFlameSword", 5, CreateStatusGroup("ELECTRIC_DAMAGE", 25, 30, 40, 55, 75, 100), CreateStatusGroup("FIRE_DAMAGE", 2, 3, 5, 7, 10, 13))
            .SetCategory(ItemCategories.Magitech, ItemCategories.FlameSword).SetIsDual().SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsUniqueEffect().SetTreeShopItemEntity(TreeShopItems.NewCharmBond1);
        /// <summary>
        /// Item_MagitechFrostRelic_Name
        /// 凍える氷磁石
        /// Item_MagitechFrostRelic_FlavorText
        /// フレーバーテキスト募集中
        /// Item_MagitechFrostRelic_Effect
        /// <tag=FrostRelic>が命中した時、{PERCENT}の確率で<tag=Electric>のショックを即座に発動させる。<tag=Electric>のショックが発動した時、「雹の手」バフを獲得
        /// Item_MagitechFrostRelic_Effect2
        /// 雹の手：{BUFF}秒の間、<tag=IceDamage>が2増加（最大20スタック）
        /// </summary>
        public static ModCharm MagitechFrostRelic { get; } = ModCharmStatus.Create<Charm_MagitechFrostRelic>("MagitechFrostRelic", 5, CreateStatusGroup("LIGHTNING_DAMAGE", 2, 3, 5, 7, 10, 13))
            .SetCategory(ItemCategories.Magitech, ItemCategories.Frost).SetIsDual().SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect().SetTreeShopItemEntity(TreeShopItems.NewCharmBond1);
        /// <summary>
        /// Item_Auto_MagicDarkCloud_Name
        /// 巨大実験台
        /// Item_Auto_MagicDarkCloud_FlavorText
        /// フレーバーテキスト募集中
        /// Item_Auto_MagicDarkCloud_Effect
        /// <tag=MP>を消費するかわりに<tag=DarkCloud>を{CLOUD}消費して、上の枠にある<tag=Magic>を{COOLDOWN}秒遅れて自動発動する
        /// </summary>
        public static ModCharm AutoMagicDarkCloud { get; } = ModCharmStatus.Create<Charm_AutoMagicDarkCloud>("AutoMagicDarkCloud", 5, CreateStatusGroup("COOLDOWN_RECOVERY_SPEED", 10, 20, 30, 40, 60, 80))
            .SetCategory(ItemCategories.Academy, ItemCategories.DarkCloud).SetIsDual().SetSimpleEffect().SetRarity(EItemRarity.Rare);
        /// <summary>
        /// Item_MoreShop_Name
        /// 行商人の手形
        /// Item_MoreShop_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm SavvyUncommon { get; } = ModCharmStatus.Create("MoreShop", 2, CreateStatusGroup("AdditionalShop".ToSephiriaId(), 1, 1, 2), CreateStatusGroup("AdditionalMoney".ToSephiriaId(), 500, 1000, 2000))
            .SetCategory(ItemCategories.Savvy).SetSimpleEffects(0).SetRarity(EItemRarity.Uncommon).SetIsUniqueEffect();
        /// <summary>
        /// Item_MoreReplenishment_Name
        /// 勇者優待券
        /// Item_MoreReplenishment_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm SavvyRare { get; } = ModCharmStatus.Create("MoreReplenishment", 3, CreateStatusGroup("NEGOTIATION", 2, 5, 12, 20), CreateStatusGroup("ReplenishmentCharm".ToSephiriaId(), 0, 1, 1, 2), CreateStatusGroup("ReplenishmentTablet".ToSephiriaId(), 0, 0, 1, 1))
            .SetCategory(ItemCategories.Savvy).SetSimpleEffects(0).SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_MoreShopLegendary_Name
        /// 名だたる鑑定書
        /// Item_MoreShopLegendary_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm SavvyLegendary { get; } = ModCharmStatus.Create("MoreShopLegendary", 5, CreateStatusGroup("AdditionalShopLegendary".ToSephiriaId(), 1), CreateStatusGroup("AdditionalShopInventory".ToSephiriaId(), 20, 30, 40, 60, 80, 100))
            .SetCategory(ItemCategories.Savvy).SetSimpleEffects(0).SetRarity(EItemRarity.Legend).SetIsUniqueEffect();
        /// <summary>
        /// Item_AddInventory_Name
        /// バッグの拡張キット
        /// Item_AddInventory_FlavorText
        /// 希少な魔法の布を使用した高級品。
        /// Item_AddInventory_Effect
        /// このアーティファクトを消費して、バッグの枠を{COUNT}拡張します
        /// </summary>
        public static ModCharm AddInventory { get; } = ModCharm.Create<Charm_AddInventory>("AddInventory", 0, true).SetActiveType(EItemActiveType.Hidden)
            .SetCategory().SetSimpleEffect().SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_ShadowFrostbite_Name
        /// 闇色の雪結晶
        /// Item_ShadowFrostbite_FlavorText
        /// フレーバーテキスト募集中
        /// Item_ShadowFrostbite_Effect
        /// <tag=Frostbite>を<tag=BlackFrostbite>に変える
        /// </summary>
        public static ModCharm ShadowFrostbite { get; } = ModCharmStatus.Create<Charm_ShadowFrostbite>("ShadowFrostbite", 4, CreateStatusGroup("ICE_DAMAGE", 2, 4, 6, 9, 13), CreateStatusGroup("EVASION", 200, 400, 600, 900, 1300))
            .SetCategory(ItemCategories.Glacier, ItemCategories.Shadow).SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsDual().SetIsUniqueEffect().
            SetDamageIdDebuff("BlackFreeze", "Status_BlackFreeze_Name").SetTreeShopItemEntity(TreeShopItems.NewCharmBond1);
        /// <summary>
        /// Item_GuardFrostbite_Name
        /// 精霊の盾
        /// Item_GuardFrostbite_FlavorText
        /// フレーバーテキスト募集中
        /// Item_GuardFrostbite_Effect
        /// <tag=WeaponAction_Guard>に成功した時、攻撃者に<tag=Frostbite>を付与する\n<tag=WeaponAction_PerfectGuard>なら、<tag=Frostbite>ではなく<tag=Freeze>を付与する
        /// </summary>
        public static ModCharm GuardFrostbite { get; } = ModCharmStatus.Create<Charm_GuardFrostbite>("GuardFrostbite", 2, CreateStatusGroup("ICE_DAMAGE", 3, 5, 8), CreateStatusGroup("MP_STEAL", 2, 3, 5))
            .SetCategory(ItemCategories.Glacier).SetSimpleEffect().SetRarity(EItemRarity.Legend).SetRelatedWeapon(EWeaponType.SwordAndShield).SetIsUniqueEffect().SetTreeShopItemEntity(TreeShopItems.NewCharmRight2);
        /// <summary>
        /// Item_OverFlameSword_Name
        /// ソリス・クリスタ
        /// Item_OverFlameSword_FlavorText
        /// フレーバーテキスト募集中
        /// Item_OverFlameSword_Effect
        /// 余分に拾った<tag=FlameSword>を地面に投げる
        /// </summary>
        public static ModCharm OverFlameSword { get; } = ModCharmStatus.Create<Charm_OverFlameSword>("OverFlameSword", 3, CreateStatusGroup("FLAME_SWORD_DAMAGE", 5, 5, 10, 15), CreateStatusGroup("FLAME_SWORD_CRITICAL", 5, 8, 11, 15))
            .SetCategory(ItemCategories.FlameSword).SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsUniqueEffect();
        /// <summary>
        /// Item_AddMaxMiracle_Name
        /// 輝く樹の枝
        /// Item_AddMaxMiracle_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm AddMaxMiracle { get; } = ModCharmOrphanedStatus.Create("AddMaxMiracle", CreateStatusGroup("MaxMiracleCount".ToSephiriaId(), 1)).SetActiveType(EItemActiveType.Hidden)
            .SetCategory().SetSimpleEffects(0).SetRarity(EItemRarity.Legend);
        /// <summary>
        /// Item_SelfExplosion_Name
        /// 自爆スイッチ
        /// Item_SelfExplosion_FlavorText
        /// 厳重なセキュリティの下で管理されています。
        /// Item_SelfExplosion_Effect
        /// <tag=Follower>がダメージを{TIME}回与えるたびにチャージ
        /// Item_SelfExplosion_Effect2
        /// <tag=Follower>{COUNT}体が自爆して<tag=FireDamage>を与える（<tag=FollowerDamage>と見なされる）\n[ダメージ：{DAMAGE}（{BASE}+<tag=FireDamage>{PERCENT}）]
        /// </summary>
        public static ModCharm SelfExplosion { get; } = ModCharmStatus.Create<Charm_SelfExplosion>("SelfExplosion", 5)
            .SetCategory(ItemCategories.Companion, ItemCategories.Ember).SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsUniqueEffect().SetIsDual().SetDamageId();
        /// <summary>
        /// Item_Active_Meteor_Name
        /// 赤い蛇の牙
        /// Item_ActiveMeteor_FlavorText
        /// フレーバーテキスト募集中
        /// Item_ActiveMeteor_Effect
        /// プレイヤーの周りに火属性ダメージの{DAMAGE}%の隕石を{COUNT}個落とす
        /// Item_ActiveMeteor_Effect2
        /// 隕石に命中した敵に<tag=Burn>デバフを付与
        /// </summary>
        public static ModCharm ActiveMeteor { get; } = ModCharmStatus.Create<Charm_ActiveMeteor>("ActiveMeteor", 6)
            .SetCategory(ItemCategories.Ember).SetEffects("Charm_FlameGround_Meteor_Effect", "Charm_FlameGround_Meteor_Effect2").SetRarity(EItemRarity.Rare).SetIsUniqueEffect().SetDamageId();
        /// <summary>
        /// Item_CompanionChaosMore_Name
        /// 忠誠の記章
        /// Item_CompanionChaosMore_FlavorText
        /// フレーバーテキスト募集中
        /// Item_CompanionChaosMore_Effect
        /// インベントリ内の同じ列または行に配置された仲間が<tag=Elemental_Chaos>ダメージを与える
        /// </summary>
        public static ModCharm CompanionChaosMore { get; } = ModCharmStatus.Create<Charm_CompanionChaosMore>("CompanionChaosMore", 3, CreateStatusGroup("FOLLOWER_ATTACK_SPEED", 10, 15, 20, 30), CreateStatusGroup("FOLLOWER_CRITICAL", 300, 500, 800, 1200))
            .SetCategory(ItemCategories.Companion).SetSimpleEffect().SetRarity(EItemRarity.Rare).SetIsUniqueEffect().SetTreeShopItemEntity(TreeShopItems.NewCharmRight3);
        /// <summary>
        /// Item_BondCoin_Name
        /// 結束のコイン
        /// Item_BondCoin_FlavorText
        /// 堅い絆の証。
        /// </summary>
        public static ModCharm BondCoin { get; } = ModCharmStatus.Create<Charm_BondCoin>("BondCoin", 2, CreateStatusGroup("NEGOTIATION", 2, 3, 5))
            .SetCategory(ItemCategories.Savvy).SetEffects("Charm_MagicianCoin_Effect").SetRarity(EItemRarity.Common).SetIsUniqueEffect();
        /// <summary>
        /// Item_SavvyPrecision_Name
        /// ギロックの鶴嘴
        /// Item_SavvyPrecision_FlavorText
        /// フレーバーテキスト募集中
        /// Item_SavvyPrecision_Effect
        /// <tag=Critical>発生時に{LEAF}<tag=Leaf>を生成
        /// Item_SavvyPrecision_Effect2
        /// 攻撃の<tag=CriticalChance>が100％を超えた場合、超過した分の確率が<tag=Excavation>の発生率に変換される\n[<tag=Excavation>の成功確率：{PERCENT}]
        /// </summary>
        public static ModCharm SavvyPrecision { get; } = ModCharmStatus.Create<Charm_SavvyPrecision>("SavvyPrecision", 3, CreateStatusGroup("CRITICAL", 250, 500, 750, 1000))
            .SetCategory(ItemCategories.Savvy, ItemCategories.Precision).SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsDual().SetIsUniqueEffect();
        /// <summary>
        /// Item_SavvyCurse_Name
        /// 錆びた銅貨
        /// Item_SavvyCurse_FlavorText
        /// フレーバーテキスト募集中
        /// Item_SavvyCurse_Effect
        /// <tag=Crime>スタック1つごとに次の効果を獲得
        /// Item_SavvyCurse_Effect2
        /// <tag=DebuffDamage> {DAMAGE}
        /// Item_SavvyCurse_Effect3
        /// <tag=DebuffDuration> {DURATION}
        /// Item_SavvyCurse_Effect4
        /// デバフスタック {STACK}
        /// Item_SavvyCurse_Effect5
        /// <tag=Crime>を犯すたび、追加で{LEAF}<tag=Leaf>を生成し、<tag=ItemRarity_Jewelry>アーティファクトを手に入れる
        /// </summary>
        public static ModCharm SavvyCurse { get; } = ModCharmStatus.Create<Charm_SavvyCurse>("SavvyCurse", 3, CreateStatusGroup("HIGHEST_ELEMENTAL_DAMAGE", 1, 2, 2, 3))
            .SetCategory(ItemCategories.Savvy, ItemCategories.Curse).SetSimpleEffects(5).SetRarity(EItemRarity.Rare).SetIsDual().SetIsUniqueEffect();
        /// <summary>
        /// Item_SavvyShadow_Name
        /// 黒の貨幣
        /// Item_SavvyShadow_FlavorText
        /// フレーバーテキスト募集中
        /// Item_SavvyShadow_Effect
        /// <tag=Evasion>成功時、<tag=Looting>が発生する
        /// Item_SavvyShadow_Effect2
        /// {COUNT}回<tag=Looting>するたび<tag=Leaf>ではなく<tag=ItemRarity_Jewelry>アーティファクトを手に入れる
        /// </summary>
        public static ModCharm SavvyShadow { get; } = ModCharmStatus.Create<Charm_SavvyShadow>("SavvyShadow", 5, CreateStatusGroup("EVASION", 200, 300, 500, 700, 1000, 1400))
            .SetCategory(ItemCategories.Savvy, ItemCategories.Shadow).SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsDual().SetIsUniqueEffect();
        /// <summary>
        /// Item_SavvyAcademy_Name
        /// 財宝地図
        /// Item_SavvyAcademy_FlavorText
        /// フレーバーテキスト募集中
        /// Item_SavvyAcademy_Effect
        /// <tag=MP>を<color=red>永久に{MP}</color><color=red><tag=ReservedMP></color>して、{COIN}を1つ手に入れる
        /// Item_SavvyAcademy_Effect2
        /// 呪印：{COIN}を捨てると、このアーティファクトは発動時効果を失い、他の{COIN}はすべて壊れる。そして、<tag=ReservedMP>されている<tag=MP>{PER}ごとに<tag=ItemRarity_Jewelry>アーティファクトを手に入れる
        /// </summary>
        public static ModCharm SavvyAcademy { get; } = ModCharmStatus.Create<Charm_SavvyAcademy>("SavvyAcademy", 2, CreateStatusGroup("COOLDOWN_RECOVERY_SPEED", 5, 10, 20))
            .SetCategory(ItemCategories.Savvy, ItemCategories.Academy).SetSimpleEffects(2).SetRarity(EItemRarity.Rare).SetIsDual().SetIsUniqueEffect();

        #region Jewelries
        /// <summary>
        /// Item_JewelryCoin_Name
        /// 魔法の金貨
        /// Item_JewelryCoin_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryCoin_Effect
        /// <tag=ItemRarity_Jewelry>アーティファクト：所持する<tag=Leaf>を最大400まで消費して、消費した<tag=Leaf>200ごとに最大レベルが1増加する
        /// Item_JewelryCoin_Effect2
        /// 所持する<tag=Leaf>{LEAF}ごとに<tag=MagicDamageBonus> {DAMAGE}
        /// </summary>
        public static ModCharm JewelryCoin { get; } = ModCharmStatus.Create<Charm_JewelryCoin>("JewelryCoin", 0,
            CreateStatusGroup("LeafDrop".ToSephiriaId(), 40))
            .SetCategory(ItemCategories.Academy).SetSimpleEffects(2).SetIsExcludedJewelry(EItemRarity.Uncommon);

        /// <summary>
        /// Item_JewelryStudy_Name
        /// 輝くダイヤモンド
        /// Item_JewelryStudy_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryStudy_Effect
        /// <tag=WeaponAction_DirectAttack>時、<tag=Leaf>を{LEAF}消費して3秒間<tag=FinalWeaponDamage>{DAMAGE}
        /// </summary>
        public static ModCharm JewelryStudy { get; } = ModCharmStatus.Create<Charm_JewelrySturdy>("JewelryStudy", 0,
            CreateStatusGroupBy("PhysicalDamage".ToSephiriaId(), 3),
            CreateStatusGroupBy("WeaponRange".ToSephiriaId(), 3, 20))
            .SetCategory(ItemCategories.Sturdy).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryWind_Name
        /// 靡くエメラルド
        /// Item_JewelryWind_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryWind_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Leaf>を{LEAF}消費して{DAMAGE}の追加<tag=PhysicalDamage>を与える
        /// </summary>
        public static ModCharm JewelryWind { get; } = ModCharmStatus.Create<Charm_JewelryDamage>("JewelryWind", 0,
            CreateStatusGroupBy("AttackSpeed".ToSephiriaId(), 5),
            CreateStatusGroupBy("TrueDamage".ToSephiriaId(), 3, 3))
            .SetCategory(ItemCategories.WindSong).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare).SetDamageId();
        /// <summary>
        /// Item_JewelryPrecision_Name
        /// 煌めくシトリン
        /// Item_JewelryPrecision_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryPrecision_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Leaf>を{LEAF}消費して<tag=CriticalChance>が{CRITICAL}増加する
        /// </summary>
        public static ModCharm JewelryPrecision { get; } = ModCharmStatus.Create<Charm_JewelryCritical>("JewelryPrecision", 0,
            CreateStatusGroupBy("CriticalDamageRate".ToSephiriaId(), 20))
            .SetCategory(ItemCategories.Precision).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryExcavation_Name
        /// 震えるトパーズ
        /// Item_JewelryExcavation_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm JewelryExcavation { get; } = ModCharmStatus.Create<Charm_JewelryExcavation>("JewelryExcavation", 0,
            CreateStatusGroupBy("ExcavationDamage".ToSephiriaId(), 3, 40),
            CreateStatusGroupBy("CriticalDamageRate".ToSephiriaId(), 20))
            .SetCategory(ItemCategories.Precision).SetSimpleEffects(0).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryEmber_Name
        /// 燃えるルビー
        /// Item_JewelryEmber_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryEmber_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Leaf>を{LEAF}消費して<tag=Burn>デバフを付与する
        /// </summary>
        public static ModCharm JewelryEmber { get; } = ModCharmStatus.Create<Charm_JewelryBurn>("JewelryEmber", 0,
            CreateStatusGroupBy("FireDamage".ToSephiriaId(), 3),
            CreateStatusGroup("BurnStack".ToSephiriaId(), 0, 0, 0, 1, 2, 4))
            .SetCategory(ItemCategories.Ember).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryFlame_Name
        /// 焼き尽くすガーネット
        /// Item_JewelryFlame_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryFlame_Effect
        /// <tag=FlameSword>が発動した時、<tag=Leaf>を{LEAF}消費して1回追加発動する
        /// </summary>
        public static ModCharm JewelryFlame { get; } = ModCharmStatus.Create<Charm_JewelryFlameSword>("JewelryFlame", 0,
            CreateStatusGroupBy("FireDamage".ToSephiriaId(), 3),
            CreateStatusGroupBy("FlameSwordFastFall".ToSephiriaId(), 3, 20))
            .SetCategory(ItemCategories.FlameSword).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryGlacier_Name
        /// 冷たいサファイア
        /// Item_JewelryGlacier_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryGlacier_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Leaf>を{LEAF}消費して<tag=Frostbite>デバフを付与する
        /// </summary>
        public static ModCharm JewelryGlacier { get; } = ModCharmStatus.Create<Charm_JewelryFrostbite>("JewelryGlacier", 0,
            CreateStatusGroupBy("IceDamage".ToSephiriaId(), 3),
            CreateStatusGroupBy("FreezeDamage".ToSephiriaId(), 3, 40))
            .SetCategory(ItemCategories.Glacier).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryFrost_Name
        /// 凍て尽くすアクアマリン
        /// Item_JewelryFrost_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryFrost_Effect
        /// <tag=FrostRelic>が発動した時、<tag=Leaf>を{LEAF}消費して1回追加発動する
        /// </summary>
        public static ModCharm JewelryFrost { get; } = ModCharmStatus.Create<Charm_JewelryFrostRelic>("JewelryFrost", 0,
            CreateStatusGroupBy("IceDamage".ToSephiriaId(), 3),
            CreateStatusGroupBy("ChargingCharmBonus".ToSephiriaId(), 3, 30))
            .SetCategory(ItemCategories.Frost).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryElectric_Name
        /// 弾けるターコイズ
        /// Item_JewelryElectric_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryElectric_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Leaf>を{LEAF}消費して<tag=Electric>デバフを付与する
        /// </summary>
        public static ModCharm JewelryElectric { get; } = ModCharmStatus.Create<Charm_JewelryElectric>("JewelryElectric", 0,
            CreateStatusGroupBy("LightningDamage".ToSephiriaId(), 3),
            CreateStatusGroup("ElectricStack".ToSephiriaId(), 0, 0, 0, 1, 2, 4))
            .SetCategory(ItemCategories.Magitech).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryCloud_Name
        /// 貫くマラカイト
        /// Item_JewelryCloud_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryCloud_Effect
        /// <tag=DarkCloud>が発動した時、<tag=Leaf>を{LEAF}消費して点射数が1増加する
        /// </summary>
        public static ModCharm JewelryCloud { get; } = ModCharmStatus.Create<Charm_JewelryDarkCloud>("JewelryCloud", 0,
            CreateStatusGroupBy("LightningDamage".ToSephiriaId(), 3),
            CreateStatusGroupBy("DarkCloudSpeed".ToSephiriaId(), 3, 30))
            .SetCategory(ItemCategories.DarkCloud).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryCurse_Name
        /// 蠢くオパール
        /// Item_JewelryCurse_FlavorText
        /// フレーバーテキスト募集中
        /// Item_JewelryCurse_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Leaf>を{LEAF}消費して<tag=Debuff_Poison>デバフを付与する
        /// </summary>
        public static ModCharm JewelryCurse { get; } = ModCharmStatus.Create<Charm_JewelryDebuff>("JewelryCurse", 0,
            CreateStatusGroupBy("DebuffDamage".ToSephiriaId(), 8),
            CreateStatusGroupBy("DebuffDuration".ToSephiriaId(), 3, 30))
            .SetCategory(ItemCategories.Curse).SetSimpleEffects(1).SetIsUniqueEffect().SetIsJewelry(EItemRarity.Rare);
        /// <summary>
        /// Item_JewelryAll_Name
        /// 鮮やかなアイリスクォーツ
        /// Item_JewelryAll_FlavorText
        /// フレーバーテキスト募集中
        /// </summary>
        public static ModCharm JewelryAll { get; } = ModCharmStatus.Create<Charm_JewelryExcavation>("JewelryAll", 0,
            CreateStatusGroupBy("FINAL_DAMAGE", 4))
            .SetCategory(ItemCategories.Mystic).SetSimpleEffects(0).SetIsExcludedJewelry(EItemRarity.Legend);

        public static readonly System.Random JewelryRandom = new System.Random();
        public static ModCharm GetRandomJewelry(this UnitAvatar avatar)
        {
            if (avatar == null || avatar.Inventory == null)
                return null;
            var list = avatar.Inventory.lastAppliedComboEffects.OrderByDescending(x => x.Value.comboCount).ToList();
            foreach (var combo in list)
            {
                if (combo.Key == ItemCategories.Savvy)
                    continue;
                if (Jewelries.ContainsKey(combo.Key) && Jewelries[combo.Key] != null && Jewelries[combo.Key].Count > 0)
                    return Jewelries[combo.Key].GetRandom();
            }
            return null;
        }
        public static void AddRandomJewelry(this Charm_Basic charm, bool duplicate = false)
        {
            if (!duplicate && Sephirite_Jewelry.HasSephirite(charm.connectionToClient))
                return;
            if (charm.NetworkAvatar.TryGetComponent<LevelController>(out var level))
            {
                level.GenerateItem(Data.SephiriteJewelry, level.currentLevel + charm.NetworkAvatar.Money);
                return;
            }
            else
            {
                Debug.Log("[MiraItemMod][AddRandomJewelry] LevelController not found");
            }

            var random = charm.NetworkAvatar.GetRandomJewelry();
            if (random == null || charm.Inventory == null)
                return;
            charm.Inventory.AddItem(new ItemMetadata(ItemDatabase.GenerateInstanceID(JewelryRandom), random.Id, 1));
        }
        public static void AddRandomJewelry(this Miracle miracle)
        {
            if (Sephirite_Jewelry.HasSephirite(miracle.connectionToClient))
                return;
            if (miracle.Owner == null)
                return;
            if (miracle.Owner.TryGetComponent<LevelController>(out var level))
            {
                level.GenerateItem(Data.SephiriteJewelry, level.currentLevel + miracle.Owner.Money);
                return;
            }

            var random = miracle.Owner.GetRandomJewelry();
            if (random == null || miracle.Owner.Inventory == null)
                return;
            miracle.Owner.Inventory.AddItem(new ItemMetadata(ItemDatabase.GenerateInstanceID(JewelryRandom), random.Id, 1));
        }
        #endregion

        #region Sacrifices
        /// <summary>
        /// Item_SacrificeFire_Name
        /// 炎の儀式
        /// Item_SacrificeFire_FlavorText
        /// 神に捧げる儀式。
        /// Item_SacrificeFire_Effect
        /// 儀式：<tag=FireDamage>を合計{DAMAGE}以上与えると、{REWARD}を1つ獲得する（現在：{CURRENT}）
        public static ModCharm SacrificeFire { get; } = ModCharmSacrificeDamage.Create("SacrificeFire", () => SacrificeFireReward.ItemEntity, 333333, CreateStatusGroup("PHYSICAL_DAMAGE", -10), CreateStatusGroup("DEFENSE", -20))
            .SetCategory(ItemCategories.Ember, ItemCategories.FlameSword).SetSimpleEffect().SetElementalType(EDamageElementalType.Fire);
        /// <summary>
        /// Item_SacrificeFire_Reward_Name
        /// 熾天使の聖剣
        /// Item_SacrificeFire_Reward_FlavorText
        /// 燃え尽きた剣。
        /// Item_SacrificeFire_Reward_Effect
        /// <tag=FlameSword>が<tag=Burn>スタックの数だけ追加発動する
        /// </summary>
        public static ModCharm SacrificeFireReward { get; } = ModCharmStatus.Create<Charm_EmberFlameSword>("SacrificeFire_Reward", 5, CreateStatusGroup("FIRE_DAMAGE", 2, 3, 5, 7, 10, 15), CreateStatusGroup("FLAME_SWORD_MAX", 1, 2, 3, 4, 5, 6))
            .SetCategory(ItemCategories.Ember, ItemCategories.FlameSword).SetIsDual().SetRarity(EItemRarity.Eternal).SetIsUniqueEffect().SetSimpleEffect();
        /// <summary>
        /// Item_SacrificeIce_Name
        /// 氷の儀式
        /// Item_SacrificeIce_FlavorText
        /// 神に捧げる儀式。
        /// Item_SacrificeIce_Effect
        /// 儀式：<tag=IceDamage>を合計{DAMAGE}以上与えると、{REWARD}を1つ獲得する（現在：{CURRENT}）
        public static ModCharm SacrificeIce { get; } = ModCharmSacrificeDamage.Create("SacrificeIce", () => SacrificeIceReward.ItemEntity, 333333, CreateStatusGroup("PHYSICAL_DAMAGE", -10), CreateStatusGroup("ATTACK_SPEED", -20))
            .SetCategory(ItemCategories.Glacier, ItemCategories.Frost).SetSimpleEffect().SetElementalType(EDamageElementalType.Ice);
        /// <summary>
        /// Item_SacrificeIce_Reward_Name
        /// 極氷の曲剣
        /// Item_SacrificeIce_Reward_FlavorText
        /// 凍てついた剣。
        /// Item_SacrificeIce_Reward_Effect
        /// <tag=Freeze>発動時に<tag=FrostRelic>のチャージを加速させる
        /// </summary>
        public static ModCharm SacrificeIceReward { get; } = ModCharmStatus.Create<Charm_GlacierFrost>("SacrificeIce_Reward", 5, CreateStatusGroup("ICE_DAMAGE", 2, 3, 5, 7, 10, 15), CreateStatusGroup("CHARGING_CHARM_BONUS", 15, 30, 45, 60, 75, 90))
            .SetCategory(ItemCategories.Glacier, ItemCategories.Frost).SetIsDual().SetRarity(EItemRarity.Eternal).SetIsUniqueEffect().SetSimpleEffect();
        /// <summary>
        /// Item_SacrificeLightning_Name
        /// 雷の儀式
        /// Item_SacrificeLightning_FlavorText
        /// 神に捧げる儀式。
        /// Item_SacrificeLightning_Effect
        /// 儀式：<tag=LightningDamage>を合計{DAMAGE}以上与えると、{REWARD}を1つ獲得する（現在：{CURRENT}）
        public static ModCharm SacrificeLightning { get; } = ModCharmSacrificeDamage.Create("SacrificeLightning", () => SacrificeLightningReward.ItemEntity, 333333, CreateStatusGroup("PHYSICAL_DAMAGE", -10), CreateStatusGroup("CRITICAL", -2000))
            .SetCategory(ItemCategories.Magitech, ItemCategories.DarkCloud).SetSimpleEffect().SetElementalType(EDamageElementalType.Lightning);
        /// <summary>
        /// Item_SacrificeLightning_Reward_Name
        /// 迅雷の直剣
        /// Item_SacrificeLightning_Reward_FlavorText
        /// いなずまの剣。
        /// Item_SacrificeLightning_Reward_Effect
        /// <tag=DarkCloud>の稲妻が命中した敵に<tag=Electric>デバフを付与する
        /// </summary>
        public static ModCharm SacrificeLightningReward { get; } = ModCharmStatus.Create<Charm_MagitechDarkCloud>("SacrificeLightning_Reward", 5, CreateStatusGroup("LIGHTNING_DAMAGE", 2, 3, 5, 7, 10, 15), CreateStatusGroup("ELECTRIC_STACK", 1, 1, 2, 2, 3, 3))
            .SetCategory(ItemCategories.Magitech, ItemCategories.DarkCloud).SetIsDual().SetRarity(EItemRarity.Eternal).SetIsUniqueEffect().SetSimpleEffect();
        #endregion

        #region ComboEffects
        /// <summary>
        /// ItemCategory_Vitality
        /// 生命
        /// </summary>
        public static ModComboEffect Vitality { get; } = ModComboEffect.Create("Vitality").SetStats(CreateComboStat(3, "MAX_HP/10"), CreateComboStat(6, "MAX_HP/10"), CreateComboStat(9, "MAX_HP/10", "FINAL_HP/10"));

        /// <summary>
        /// ItemCategory_Stargaze
        /// 夜空
        /// </summary>
        public static ModComboEffect Stargaze { get; } = ModComboEffect.Create<ComboEffect_Stargaze>("Stargaze").SetDamageIdAbility().SetDefaultEffect().SetStats(
            CreateComboStat(4, "STARGAZE_LEVEL/1"), CreateComboStat(6, "STARGAZE_LEVEL/1"),
            CreateComboStat(8, "STARGAZE_LEVEL/1"), CreateComboStat(10, "STARGAZE_LEVEL/1"));
        /// <summary>
        /// ItemCategory_SkySong
        /// 空の歌
        /// </summary>
        public static ModComboEffect SkySong { get; } = ModComboEffect.Create("SkySong").SetStats(CreateComboStat(2, "DASH_COUNT/1", "DASH_RECOVERY_SPEED/20"), CreateComboStat(4, "DASH_ATTACK_DAMAGE/20"),
            CreateComboStat(6, "DASH_ATTACK_DAMAGE/30"), CreateComboStat(8, "DASH_ATTACK_DAMAGE/40"), CreateComboStat(10, "DASH_INVINCIBLE_TIME_BONUS/100", "DASH_RECOVERY_SPEED/20"));
        /// <summary>
        /// ItemCategory_Drunk
        /// 酩酊
        /// </summary>
        public static ModComboEffect Drunk { get; } = ModComboEffect.Create<ComboEffect_Drunk>("Drunk").SetStats(CreateComboStat(4, "DEFENSE/-10"), CreateComboStat(6, "DEFENSE/-10"),
            CreateComboStat(8, "DEFENSE/-20"), CreateComboStat(10, "FINAL_DAMAGE/30")).SetDefaultEffect();
        /// <summary>
        /// ItemCategory_Fortune
        /// 運命
        /// </summary>
        public static ModComboEffect Fortune { get; } = ModComboEffect.Create("Fortune").SetStats(CreateComboStat(2, "LUCK/4"), CreateComboStat(4, "LUCK/8"));
        /// <summary>
        /// ItemCategory_Grimoire
        /// 魔導書
        /// ComboEffectDefault_Grimoire_Effect2
        /// +3 火・氷・雷属性ダメージ
        /// ComboEffectDefault_Grimoire_Effect4
        /// +6 火・氷・雷属性ダメージ
        /// ComboEffectDefault_Grimoire_Effect6
        /// +9 火・氷・雷属性ダメージ
        /// ComboEffectDefault_Grimoire_Effect8
        /// +12 火・氷・雷属性ダメージ
        /// ComboEffectDefault_Grimoire_Effect10
        /// +15 火・氷・雷属性ダメージ
        /// </summary>
        public static ModComboEffect Grimoire { get; } = ModComboEffect.Create("Grimoire").SetStats(CreateComboStatThreeDamage(2, "Grimoire", 3), CreateComboStatThreeDamage(4, "Grimoire", 6),
            CreateComboStatThreeDamage(6, "Grimoire", 9), CreateComboStatThreeDamage(8, "Grimoire", 12), CreateComboStatThreeDamage(10, "Grimoire", 15));
        #endregion

        #region EffectHUDs
        /// <summary>
        /// EffectHUD_PhysicalDamageBuff_Name
        /// 暗影の凶刃
        /// EffectHUD_PhysicalDamageBuff_FlavorText
        /// 物理ダメージ増加（最大4スタック）
        /// </summary>
        public static ModEffectHUD EffectPhysicalDamageBuff { get; } = ModEffectHUD.CreateStackEffectHUD("PhysicalDamageBuff", UI_EffectHUD_Basic.EEffectType.Boon);
        public static CharacterBuffMod_StatusInstance PhysicalDamageBuff { get; } = CreateBuff("PhysicalDamageBuff", "PhysicalDamageBuff", 4, CreateBuffStatus("PHYSICAL_DAMAGE", 5))
            .SetDefaultDuration(8f);
        /// <summary>
        /// EffectHUD_MagitechFrostRelicBuff_Name
        /// 雹の手
        /// EffectHUD_MagitechFrostRelicBuff_FlavorText
        /// 氷属性ダメージ増加（最大20スタック）
        /// </summary>
        public static ModEffectHUD EffectMagitechFrostRelicBuff { get; } = ModEffectHUD.CreateStackEffectHUD("MagitechFrostRelicBuff", UI_EffectHUD_Basic.EEffectType.Boon);
        public static CharacterBuffMod_StatusInstance MagitechFrostRelicBuff { get; } = CreateBuff("MagitechFrostRelicBuff", "MagitechFrostRelicBuff", 20, CreateBuffStatus("ICE_DAMAGE", 2))
            .SetDefaultDuration(Charm_MagitechFrostRelic.BuffDuration);
        /// <summary>
        /// EffectHUD_SoulStealBuff_Name
        /// 吸魂
        /// EffectHUD_SoulStealBuff_FlavorText
        /// 近接攻撃範囲増加（最大50スタック）
        /// </summary>
        public static ModEffectHUD EffectSoulStealBuff { get; } = ModEffectHUD.CreateStackEffectHUD("SoulStealBuff", UI_EffectHUD_Basic.EEffectType.Boon);
        public static CharacterBuffMod_StatusInstance SoulStealBuff { get; } = CreateBuff("SoulStealBuff", "SoulStealBuff", 50, CreateBuffStatus("WEAPON_RANGE", 2))
            .SetDefaultDuration(30f);
        /// <summary>
        /// EffectHUD_WeaponDamageBuff_Name
        /// ダイヤモンド
        /// EffectHUD_WeaponDamageBuff_FlavorText
        /// <tag=FinalWeaponDamage>が増加します。
        /// </summary>
        public static ModEffectHUD EffectWeaponDamageBuff { get; } = ModEffectHUD.CreateStackEffectHUD("WeaponDamageBuff", UI_EffectHUD_Basic.EEffectType.Boon);
        public static CharacterBuffMod_StatusInstance WeaponDamageBuff { get; } = CreateBuff("WeaponDamageBuff", "WeaponDamageBuff", 1, CreateBuffStatus("FINAL_WEAPONDAMAGE", 8))
            .SetDefaultDuration(3f);
        /// <summary>
        /// EffectHUD_PallasBuff_Name
        /// クラブのカード
        /// EffectHUD_PallasBuff_FlavorText
        /// <tag=TrueDamage>増加（最大5スタック）
        /// </summary>
        public static ModEffectHUD EffectPallasBuff { get; } = ModEffectHUD.CreateStackEffectHUD("PallasBuff", UI_EffectHUD_Basic.EEffectType.Boon);
        public static CharacterBuffMod_StatusInstance PallasBuff { get; } = CreateBuff("PallasBuff", "PallasBuff", 5, CreateBuffStatus("TRUE_DAMAGE", 1))
            .SetDefaultDuration(5f);

        /// <summary>
        /// EffectHUD_StargazeTablet_Name
        /// 星見の石版
        /// EffectHUD_StargazeTablet_FlavorText
        /// 破壊された石版の欠片。
        /// </summary>
        public static ModEffectHUD EffectStargazeTablet { get; } = ModEffectHUD.CreateStackEffectHUD("StargazeTablet", UI_EffectHUD_Basic.EEffectType.Boon);
        /// <summary>
        /// EffectHUD_CreateStoneTablet_Name
        /// 流れ星の結晶
        /// EffectHUD_CreateStoneTablet_FlavorText
        /// 倒した敵の数。
        /// </summary>
        public static ModEffectHUD EffectCreateStoneTablet { get; } = ModEffectHUD.CreateStackEffectHUD("CreateStoneTablet", UI_EffectHUD_Basic.EEffectType.Boon);
        /// <summary>
        /// EffectHUD_CopyAcademy_Name
        /// 原典
        /// EffectHUD_CopyAcademy_FlavorText
        /// <tag=Grimoire>を一定回数使用するとアカデミーアーティファクトを複製する
        /// </summary>
        public static ModEffectHUD EffectCopyAcademy { get; } = ModEffectHUD.CreateStackEffectHUD("CopyAcademy", UI_EffectHUD_Basic.EEffectType.Boon);
        /// <summary>
        /// EffectHUD_ElectricStun_Name
        /// ビリビリクリームクロワッサン
        /// EffectHUD_ElectricStun_FlavorText
        /// <tag=Electric>が付与されていない敵に<tag=LightningDamage>を与えた時の気絶確率
        /// </summary>
        public static ModEffectHUD EffectElectricStun { get; } = ModEffectHUD.CreateStackEffectHUD("ElectricStun", UI_EffectHUD_Basic.EEffectType.Boon);
        /// <summary>
        /// EffectHUD_ImmersionIce_Name
        /// 冷静
        /// EffectHUD_ImmersionIce_FlavorText
        /// <tag=FrostRelic>が1回追加発動します。
        /// </summary>
        public static ModEffectHUD EffectIceTrance { get; } = ModEffectHUD.CreateStackEffectHUD("ImmersionIce", UI_EffectHUD_Basic.EEffectType.Boon).SetHasStackText();
        /// <summary>
        /// EffectHUD_SavvyShadow_Name
        /// 黒の貨幣
        /// EffectHUD_SavvyShadow_FlavorText
        /// <tag=Looting>した回数
        /// </summary>
        public static ModEffectHUD EffectSavvyShadow { get; } = ModEffectHUD.CreateStackEffectHUD("SavvyShadow", UI_EffectHUD_Basic.EEffectType.Boon);
        #endregion

        #region Keywords and Stats
        /// <summary>
        /// Status_StargazeLevel_Name
        /// 夜空アーティファクト最大レベル
        /// Status_StargazeLevel_Description
        /// インベントリにある夜空アーティファクトの最大レベルが増加します
        /// </summary>
        public static ModCustomStatus StargazeLevel { get; } = ModCustomStatus.CreateStatus("StargazeLevel").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_InvLevel_Name
        /// カジノチップの最大レベル
        /// Status_InvLevel_Description
        /// インベントリにあるカジノチップの最大レベルが増加します
        /// </summary>
        public static ModCustomStatus InvLevel { get; } = ModCustomStatus.CreateStatus("InvLevel").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_AddGrimoire_Name
        /// 
        /// Status_AddGrimoire_Description
        /// 
        /// </summary>
        public static ModCustomStatus AddGrimoire { get; } = ModCustomStatus.CreateStatus("AddGrimoire").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_AdditionalShop_Name
        /// ステージを移動した時、そのステージにいる商人の品数が{VALUE}増加
        /// Status_AdditionalShop_Description
        /// 商人が持つアイテムの数が増加します。
        /// </summary>
        public static ModCustomStatus AdditionalShop { get; } = ModCustomStatus.CreateStatus("AdditionalShop").SetNotIncludePositiveNegativeSign()
            .DoKeyword(keyword => keyword.SetNotDisplayDetails().SetNeedParseValueOnVisualText());
        /// <summary>
        /// Status_AdditionalShopLegendary_Name
        /// ステージを移動した時、そのステージにいる商人に伝説アーティファクトを{VALUE}個追加
        /// Status_AdditionalShopLegendary_Description
        /// 商人が持つ伝説アーティファクトの数が増加します
        /// </summary>
        public static ModCustomStatus AdditionalShopLegendary { get; } = ModCustomStatus.CreateStatus("AdditionalShopLegendary").SetNotIncludePositiveNegativeSign()
            .DoKeyword(keyword => keyword.SetNotDisplayDetails().SetNeedParseValueOnVisualText());
        /// <summary>
        /// Status_AdditionalShopInventory_Name
        /// ステージを移動した時、そのステージにいる商人がバッグの枠拡張キットを売る確率
        /// Status_AdditionalShopInventory_Description
        /// 確率で商人がバッグの枠拡張キットを売ります
        /// </summary>
        public static ModCustomStatus AdditionalShopInventory { get; } = ModCustomStatus.CreateStatus("AdditionalShopInventory").SetSymbol("%").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_AdditionalMoney_Name
        /// ステージを移動した時、そのステージにいる<tag=MerchantLeaf>が{VALUE}増加
        /// Status_AdditionalMoney_Description
        /// 商人が持つ<tag=Leaf>が増加します。
        /// </summary>
        public static ModCustomStatus AdditionalMoney { get; } = ModCustomStatus.CreateStatus("AdditionalMoney").SetNotIncludePositiveNegativeSign()
            .DoKeyword(keyword => keyword.SetNotDisplayDetails().SetNeedParseValueOnVisualText());
        /// <summary>
        /// Status_ReplenishmentCharm_Name
        /// サファイアを使った時に商人が入荷するアーティファクトの数
        /// Status_ReplenishmentCharm_Description
        /// サファイアを使って商人に入荷させた時のアーティファクトの数が増加します
        /// </summary>
        public static ModCustomStatus ReplenishmentCharm { get; } = ModCustomStatus.CreateStatus("ReplenishmentCharm").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_ReplenishmentTablet_Name
        /// サファイアを使った時に商人が入荷する石版の数
        /// Status_ReplenishmentTablet_Description
        /// サファイアを使って商人に入荷させた時の石版の数が増加します
        /// </summary>
        public static ModCustomStatus ReplenishmentTablet { get; } = ModCustomStatus.CreateStatus("ReplenishmentTablet").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_MaxMiracleCount_Name
        /// 奇跡の最大数
        /// Status_MaxMiracleCount_Description
        /// 奇跡の最大数が増加します
        /// </summary>
        public static ModCustomStatus MaxMiracleCount { get; } = ModCustomStatus.CreateStatus<StatusInstance_MaxMiracleCount>("MaxMiracleCount").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_AllDebuffStack_Name
        /// すべての<tag=Debuff>スタック
        /// Status_AllDebuffStack_Description
        /// すべての<tag=Debuff>スタック数が増加します
        /// </summary>
        public static ModCustomStatus AllDebuffStack { get; } = ModCustomStatus.CreateStatus<StatusInstance_AllDebuffStack>("AllDebuffStack").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_JewelryCount_Name
        /// <tag=ItemRarity_Jewelry>アーティファクトの獲得回数
        /// Status_JewelryCount_Description
        /// <tag=ItemRarity_Jewelry>アーティファクトを入手した回数です
        /// </summary>
        public static ModCustomStatus JewelryCount { get; } = ModCustomStatus.CreateStatus("JewelryCount").DoKeyword(keyword => keyword.SetNotDisplayDetails());
        /// <summary>
        /// Status_ExcavationDamage_Name
        /// 発掘ダメージ
        /// Status_ExcavationDamage_Description
        /// <tag=Excavation>時のダメージが増加します
        /// </summary>
        public static ModCustomStatus ExcavationDamage { get; } = ModCustomStatus.CreateStatus("ExcavationDamage").SetSymbol("%").DoKeyword(keyword => keyword.SetNotDisplayDetails().SetKeywordImage(() => CustomSpriteAsset.Excavation));
        /// <summary>
        /// Status_MiniBossRewardDice_Name
        /// ミニボスを倒した時、サイコロを{VALUE}個獲得
        /// Status_MiniBossRewardDice_Description
        /// ミニボスを倒した時、サイコロを{VALUE}個獲得します
        /// </summary>
        public static ModCustomStatus MiniBossRewardDice { get; } = ModCustomStatus.CreateStatus("MiniBossRewardDice").SetNotIncludePositiveNegativeSign()
            .DoKeyword(keyword => keyword.SetNotDisplayDetails().SetNeedParseValueOnVisualText());
        /// <summary>
        /// Status_MagicExecution_Name
        /// 天罰
        /// Status_MagicExecution_Description
        /// 最も高い属性ダメージまたは物理ダメージを、最も低い属性ダメージまたは物理ダメージで割った値を掛けた<tag=Elemental_Chaos>ダメージを与えます（最大8倍、クリティカル扱いされます）
        /// </summary>
        public static ModKeyword MagicExecution { get; } = ModKeyword.CreateKeyword("MagicExecution").SetOriginal("MagicDamageBonus").SetConnectedDetailEntities("Elemental_Chaos");
        /// <summary>
        /// Status_BinaryPlanet_Name
        /// 連星
        /// Status_BinaryPlanet_Description
        /// 惑星が3回攻撃します
        /// </summary>
        public static ModKeyword BinaryPlanet { get; } = ModKeyword.CreateKeyword("BinaryPlanet").SetTextColor(new Color(0.7f, 0.4f, 0.1f)).SetKeywordImage(() => CustomSpriteAsset.BinaryPlanet);
        /// <summary>
        /// Status_Assasination_Name
        /// 暗閃
        /// Status_Assasination_Description
        /// <tag=WeaponAction_DirectAttack>が命中した時、確率で対象に付与されたデバフ1つにつき、<tag=HP>の2%の追加ダメージを与えます
        /// </summary>
        public static ModKeyword Assasination { get; } = ModKeyword.CreateKeyword("Assasination").SetTextColor(new Color(0.9f, 0.1f, 0.1f)).SetKeywordImage(() => CustomSpriteAsset.Assasination);
        /// <summary>
        /// Status_MerchantLeaf_Name
        /// 商人のリーフ
        /// Status_MerchantLeaf_Description
        /// 商人が持つ<tag=Leaf>です
        /// </summary>
        public static ModKeyword MerchantLeaf { get; } = ModKeyword.CreateKeyword("MerchantLeaf").SetNotDisplayDetails().SetKeywordImage(() => CustomSpriteAsset.MerchantLeaf);
        /// <summary>
        /// Status_WeaponAction_IceTrance_Name
        /// 冷静
        /// Status_WeaponAction_IceTrance_Description
        /// <tag=FrostRelic>が1回追加発動します。
        /// </summary>
        public static ModKeyword IceTrance { get; } = ModKeyword.CreateKeyword("WeaponAction_IceTrance").SetTextColorOriginal("WeaponAction_Trance");
        /// <summary>
        /// Status_BlackFrostbite_Name
        /// 黒い霜焼け
        /// Status_BlackFrostbite_Description
        /// <tag=Frostbite>の効果に加えて、対象の攻撃をスタックごとに4%の確率で回避します。5回蓄積すると<tag=Freeze>ではなく<tag=BlackFreeze>に変更されます。
        /// </summary>
        public static ModKeyword BlackFrostbite { get; } = ModKeyword.CreateKeyword("BlackFrostbite").SetTextColor(new Color(0.2f, 0.2f, 0.2f))
            .SetKeywordImage(() => CustomSpriteAsset.BlackFrostbite).SetConnectedDetailEntities("Freeze", "BlackFreeze");
        /// <summary>
        /// Status_BlackFreeze_Name
        /// 黒い氷結
        /// Status_BlackFreeze_Description
        /// <tag=Freeze>の効果に加えて、即座に<tag=Evasion>250%のダメージを与えます。対象のスタン耐性が減少します。
        /// </summary>
        public static ModKeyword BlackFreeze { get; } = ModKeyword.CreateKeyword("BlackFreeze").SetTextColor(new Color(0.2f, 0.2f, 0.2f))
            .SetKeywordImage(() => CustomSpriteAsset.BlackFrostbite);
        /// <summary>
        /// Status_SoulSteal_Name
        /// 吸魂
        /// Status_SoulSteal_Description
        /// 近接攻撃範囲がスタックごとに2%増加します。最大50スタックまで蓄積します。
        /// </summary>
        public static ModKeyword SoulSteal { get; } = ModKeyword.CreateKeyword("SoulSteal").SetTextColor(new Color(0.8f, 0.2f, 0.4f));
        /// <summary>
        /// Status_ItemRarity_Jewelry_Name
        /// 宝飾
        /// Status_ItemRarity_Jewelry_Description
        /// 価値の高い特殊なアーティファクト。このアーティファクトを手に入れた時、所持する<tag=Leaf>をすべて消費して、消費した<tag=Leaf>200ごとに最大レベルが1増加します。（最大5レベルまで）
        /// </summary>
        public static ModKeyword ItemRarityJewelry { get; } = ModKeyword.CreateKeyword("ItemRarity_Jewelry").SetTextColor(new Color32(255, 120, 0, 255));
        /// <summary>
        /// Status_Excavation_Name
        /// 発掘
        /// Status_Excavation_Description
        /// 所持する<tag=Leaf>500ごとに1%の確率で<tag=ItemRarity_Jewelry>アーティファクトを手に入れます。<tag=ItemRarity_Jewelry>アーティファクトを入手するたび、必要な所持する<tag=Leaf>が100%増加します
        /// </summary>
        public static ModKeyword Excavation { get; } = ModKeyword.CreateKeyword("Excavation").SetTextColor(new Color32(200, 100, 0, 255))
            .SetConnectedDetailEntities("ItemRarity_Jewelry").SetKeywordImage(() => CustomSpriteAsset.Excavation);
        public static ModKeyword Crime { get; } = ModKeyword.CreateKeyword("Crime", "Debuff_Crime", "Debuff_Crime_Description").SetTextColor(new Color32(160, 0, 0, 255)).SetKeywordImage(() => CustomSpriteAsset.Crime);
        /// <summary>
        /// Status_Looting_Name
        /// 略奪
        /// Status_Looting_Description
        /// 対象の<tag=Leaf>を盗み取ります。<tag=Evasion>と同様の確率で盗み取る<tag=Leaf>が増加します。
        /// </summary>
        public static ModKeyword Looting { get; } = ModKeyword.CreateKeyword("Looting").SetTextColor(new Color32(200, 100, 0, 255)).SetKeywordImage(() => CustomSpriteAsset.Looting);
        /// <summary>
        /// Status_LeafSteal_Name
        /// リーフ吸収
        /// Status_LeafSteal_Description
        /// 自分が与えたダメージの割合に応じて<tag=Leaf>を獲得します。(数値ごとに0.1%)
        /// </summary>
        public static ModKeyword LeafSteal { get; } = ModKeyword.CreateKeyword("LeafSteal").SetTextColor(new Color32(200, 120, 0, 255)).SetKeywordImage(() => CustomSpriteAsset.LeafSteal);
        #endregion


        #region Miracles
        /// <summary>
        /// Miracle_FlameSword_Name
        /// 炎の鍛冶屋
        /// </summary>
        public static ModMiracle FlameSword { get; } = ModMiracleStatus.Create("FlameSword", CreatePositiveStat("FLAME_SWORD_MAX/3"), CreatePositiveStat("FLAME_SWORD_DAMAGE/10"))
            .SetCategories(ItemCategories.FlameSword);
        /// <summary>
        /// Miracle_Magitech_Name
        /// エンジニア
        /// </summary>
        public static ModMiracle Magitech { get; } = ModMiracleStatus.Create("Magitech", CreatePositiveStat("LIGHTNING_DAMAGE/6"), CreatePositiveStat("ELECTRIC_STACK/1"), CreateNegativeStat("CRITICAL/-1600"))
            .SetCategories(ItemCategories.Magitech);
        /// <summary>
        /// Miracle_IgnoreDefence_Name
        /// 解体屋
        /// </summary>
        public static ModMiracle IgnoreDefence { get; } = ModMiracleStatus.Create("IgnoreDefence", CreatePositiveStat("IGNORE_DEFENSE/20"), CreatePositiveStat("BASIC_ATTACK_DAMAGE/10"))
            .SetCategories(ItemCategories.Sturdy).SetNotGiveItem().SetNotAutoGenerateEffectString(2, EEffectType.Positive, true);
        /// <summary>
        /// Miracle_Buffer_Name
        /// VTuber
        /// </summary>
        public static ModMiracle Buffer { get; } = ModMiracleStatus.Create("Buffer", CreatePositiveStat("BUFF_DURATION/100"), CreatePositiveStat("MAX_MP/20"))
            .SetCategories(ItemCategories.Academy);
        /// <summary>
        /// Miracle_ExtraLife_Name
        /// アンデッド
        /// </summary>
        public static ModMiracle ExtraLife { get; } = ModMiracleStatus.Create("ExtraLife", CreatePositiveStat("EXTRA_LIFE/1"), CreatePositiveStat("TOUGHNESS/5"), CreateNegativeStat("MAX_HP/-20"))
            .SetCategories();
        /// <summary>
        /// Miracle_HpSteal_Name
        /// 看護師
        /// </summary>
        public static ModMiracle HpSteal { get; } = ModMiracleStatus.Create("HpSteal", CreatePositiveStat("HP_STEAL/3"), CreateNegativeStat("CRITICAL_DAMAGE_RATE/-40"))
            .SetCategories();
        /// <summary>
        /// Miracle_True_Name
        /// 占い師
        /// </summary>
        public static ModMiracle True { get; } = ModMiracleStatus.Create("True", CreatePositiveStat("TRUE_DAMAGE/12"), CreateNegativeStat("FINAL_WEAPONDAMAGE/-30"))
            .SetCategories();
        /// <summary>
        /// Miracle_Merchant_Name
        /// 商人
        /// </summary>
        public static ModMiracle Merchant { get; } = ModMiracleStatus.Create("Merchant", CreatePositiveStat("NEGOTIATION/10"), CreatePositiveStat("ADDITIONAL_SHOP/2"))
            .SetCategories(ItemCategories.Savvy);
        /// <summary>
        /// Miracle_Executioner_Name
        /// 執行人
        /// </summary>
        public static ModMiracle Executioner { get; } = ModMiracleStatus.Create("Executioner", CreatePositiveStat("DEBUFF_DAMAGE/12"), CreatePositiveStat("HIGHEST_ELEMENTAL_DAMAGE/3"))
            .SetCategories(ItemCategories.Curse);
        /// <summary>
        /// Miracle_Astrologist_Name
        /// 航海士
        /// </summary>
        public static ModMiracle Astrologist { get; } = ModMiracleStatus.Create("Astrologist", CreatePositiveStat("STARGAZE_LEVEL/2"))
            .SetCategories(ItemCategories.Stargaze);
        /// <summary>
        /// Miracle_Miner_Name
        /// 採鉱者
        /// Miracle_Miner_Effect
        /// <tag=ItemRarity_Jewelry>アーティファクトを獲得
        /// </summary>
        public static ModMiracle Miner { get; } = ModMiracleStatus.Create<Miracle_GiveJewelry>("Miner", CreateNegativeStat("ATTACK_SPEED/-15"), CreateNegativeStat("MOVE_SPEED/-12"))
            .SetCategories(ItemCategories.Savvy).SetJewelryGivenItems().SetNotAutoGenerateEffectString(3, new EEffectType[] { EEffectType.Positive, EEffectType.Negative, EEffectType.Negative });
        #endregion

        #region Weapon Enhancements
        /// <summary>
        /// Weapon_SwordAndShield_Fire_T2_Name
        /// 熱い鼓動
        /// WeaponAddon_SwordAndShield_Fire_T2_Effect
        /// <tag=FireDamage>が{VAL0}増加します。
        /// </summary>
        public static ModWeapon SwordShieldFire { get; } = ModWeapon.CreateWeapon("SwordAndShield_Fire_T2", 1016, 0).SetStandardEnhancements(GetFirstWeaponId() + 1, GetFirstWeaponId() + 2).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_Status>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T2_Effect");
                status.status = new WeaponAddonCommon_Status.Stat[] { CreateWeaponStat(ECustomStat.FireDamage, 5) };
            }
        });
        /// <summary>
        /// Weapon_SwordAndShield_Fire_T3_Burn_Name
        /// カゲロウ
        /// WeaponAddon_SwordAndShield_Fire_T3_Burn_Effect
        /// <tag=BurnStack>が{VAL0}増加し、<tag=BurnSpeed>が{VAL1}%増加します。
        /// </summary>
        public static ModWeapon SwordShieldFireBurn { get; } = ModWeapon.CreateWeapon("SwordAndShield_Fire_T3_Burn", 1016).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_Status>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T2_Effect");
                status.status = new WeaponAddonCommon_Status.Stat[] { CreateWeaponStat(ECustomStat.FireDamage, 5) };

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_StatusUnsafe>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T3_Burn_Effect");
                @unsafe.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("BURNSTACK", 2), CreateWeaponStat("BURNSPEED", 100) };
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        });
        /// <summary>
        /// Weapon_SwordAndShield_Fire_T3_Critical_Name
        /// 気炎鳳凰
        /// WeaponAddon_SwordAndShield_Fire_T3_Critical_Effect
        /// <tag=CriticalChance>が{VAL0}%増加します。クリティカルヒット時、アーティファクト鳳凰の羽のクールダウンが{COOLDOWN}秒減少します。
        /// </summary>
        public static ModWeapon SwordShieldFireCritical { get; } = ModWeapon.CreateWeapon("SwordAndShield_Fire_T3_Critical", 1016).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_Status>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T2_Effect");
                status.status = new WeaponAddonCommon_Status.Stat[] { CreateWeaponStat(ECustomStat.FireDamage, 5) };

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_CriticalFire>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_SwordAndShield_Fire_T3_Critical_Effect");
                @unsafe.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("CRITICAL", 3000) };
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        });
        /// <summary>
        /// Weapon_Dagger_Evasion_T3_Another_Name
        /// 暗影の凶刃
        /// WeaponAddon_Dagger_Evasion_T3_Another_Effect
        /// <tag=WeaponAction_Fury>による無敵効果は<tag=Evasion>扱いされます。<tag=Evasion>すると、8秒間<tag=PhysicalDamage>が5増加します。（最大4スタック）
        /// </summary>
        public static ModWeapon DaggerEvasionAnother { get; } = ModWeapon.CreateWeapon("Dagger_Evasion_T3_Another", 1208, 1208).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_CounterEvasion>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Evasion_T3_Another_Effect");
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        });
        /// <summary>
        /// Weapon_Dagger_Fury_T3_MP_Name
        /// 荒れ狂う知識
        /// WeaponAddon_Dagger_Fury_T3_MP_Effect
        /// <tag=MPSkillDamage>が20%増加します。<tag=WeaponAction_Fury>を使用すると、<tag=MP>が20%回復します。
        /// </summary>
        public static ModWeapon DaggerFuryMP { get; } = ModWeapon.CreateWeapon("Dagger_Fury_T3_MP", 28, 23).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonDagger_PassFury>(out var status))
            {
                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_FuryMP>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Fury_T3_MP_Effect");
                @unsafe.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("MP_SKILL_DAMAGE", 20) };
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
            if (main.gameObject.TryGetComponent<WeaponAddonDagger_DoubleFury>(out var fury))
            {
                UnityEngine.Object.Destroy(fury);
            }
        });

        /// <summary>
        /// Weapon_Dagger_Flame_T3_Fury_Name
        /// 打ち焦がす爪
        /// WeaponAddon_Dagger_Flame_T3_Fury_Effect
        /// <tag=FireDamage>を10回与えると、<tag=WeaponAction_Trance>を獲得します。（<tag=WeaponAction_Trance>は最大2回まで充電可能）
        public static ModWeapon DaggerFlameFury { get; } = ModWeapon.CreateWeapon("Dagger_Flame_T3_Fury", 1202, 1200).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_Status>(out var status))
            {
                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_FireFury>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Flame_T3_Fury_Effect");
                @unsafe.parent = status.parent;

                if (main is WeaponSimple_Dagger dagger)
                {
                    dagger.maxFury = 2;
                }

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_SpecialAttackDebuff>(out var fury))
            {
                UnityEngine.Object.Destroy(fury);
            }
        }).SetBladeSprite(Vector3.zero).SetHeadSprite();
        /// <summary>
        /// Weapon_Katana_Ice_T3_Flame_Name
        /// ソリス・フロスト
        /// WeaponAddon_Katana_Ice_T3_Flame_Effect
        /// <tag=WeaponAction_DirectAttack>時、対象に<tag=FireDamage>の40%分の追加ダメージを与えます。\n{VAL0}秒ごとに<tag=FlameSword>を1つ生成します。<tag=FrostRelic>チャージ速度1%につき{VAL1}間隔が短くなり、<tag=FrostRelic>の追加発動1回につき生成する太陽剣が1つ増加します。
        public static ModWeapon KatanaIceFlame { get; } = ModWeaponKatana.CreateKatana("Katana_Ice_T3_Flame", 421, 419).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonKatana_Deflecting>(out var status))
            {
                var @unsafe = main.gameObject.AddComponent<WeaponAddonKatana_FrostFlameSword>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Katana_Ice_T3_Flame_Effect");
                @unsafe.additionalDamagePercent = 40;
                @unsafe.elementalType = EDamageElementalType.Fire;
                @unsafe.statId = ECustomStat.FireDamage;
                @unsafe.damageId = "Weapon_AdditionalElementalDamage_Fire";
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
            if (main.gameObject.TryGetComponent<WeaponAddonKatana_Nastrond>(out var fury))
            {
                UnityEngine.Object.Destroy(fury);
            }
        });

        /// <summary>
        /// Weapon_Crossbow_Planet_T2_Name
        /// 巨大ボーガン：望遠レンズ
        /// WeaponAddon_Crossbow_Planet_T2_Effect
        /// <tag=PlanetDamage>が12%増加します。
        public static ModWeapon CrossbowPlanet { get; } = ModWeaponCrossbow.CreateCrossbow("Crossbow_Planet_T2", 106, 100).SetStandardEnhancements(14008, 14009, 14010).SetMainPrefabModifier(main =>
        {
            if (main is WeaponSimple_Crossbow crossbow)
            {
                crossbow.defaultMagazineCapacity = 11;
                crossbow.defaultMagazineCount = 2;
                crossbow.reloadTime = 3;
                crossbow.fireIntervalTimer.time = 1f / 4;
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                //var @unsafe = main.gameObject.AddComponent<WeaponAddonKatana_FrostFlameSword>();


                //@unsafe.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");
                //@unsafe.additionalDamagePercent = 40;
                //@unsafe.elementalType = EDamageElementalType.Fire;
                //@unsafe.statId = ECustomStat.FireDamage;
                //@unsafe.damageId = "Weapon_AdditionalElementalDamage_Fire";
                //@unsafe.parent = status.parent;

                //main.addons = [status, @unsafe];
                status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("PLANETDAMAGE", 12) };
                status.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");
                main.addons = new WeaponAddon[] { status };
            }
        });
        /// <summary>
        /// Weapon_Crossbow_Planet_T3_Weapon_Name
        /// 巨大ボーガン：電波収束
        /// WeaponAddon_Crossbow_Planet_T3_Weapon_Effect
        /// 惑星がダメージを与えるごとに<tag=FinalWeaponDamage>が積み重なり、最大で50%増加します。
        public static ModWeapon CrossbowPlanetWeapon { get; } = ModWeaponCrossbow.CreateCrossbow("Crossbow_Planet_T3_Weapon", 106).SetMainPrefabModifier(main =>
        {
            if (main is WeaponSimple_Crossbow crossbow)
            {
                crossbow.defaultMagazineCapacity = 9;
                crossbow.defaultMagazineCount = 2;
                crossbow.reloadTime = 2;
                crossbow.fireIntervalTimer.time = 1f / 4;
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("PLANETDAMAGE", 12) };
                status.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_PlanetWeaponDamage>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T3_Weapon_Effect");
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        });
        /// <summary>
        /// Weapon_Crossbow_Planet_T3_Attack_Name
        /// 巨大ボーガン：超新星
        /// WeaponAddon_Crossbow_Planet_T3_Attack_Effect
        /// <tag=WeaponAction_BaiscAttack>時に5%の確率で惑星が即時攻撃します。
        public static ModWeapon CrossbowPlanetAttack { get; } = ModWeaponCrossbow.CreateCrossbow("Crossbow_Planet_T3_Attack", 106).SetMainPrefabModifier(main =>
        {
            if (main is WeaponSimple_Crossbow crossbow)
            {
                crossbow.defaultMagazineCapacity = 16;
                crossbow.defaultMagazineCount = 1;
                crossbow.reloadTime = 2.4f;
                crossbow.fireIntervalTimer.time = 1f / 5.5f;
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("PLANETDAMAGE", 12) };
                status.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_DirectFirePlanet>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T3_Attack_Effect");
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        });
        /// <summary>
        /// Weapon_Crossbow_Planet_T3_Binary_Name
        /// 巨大ボーガン：引力形成
        /// WeaponAddon_Crossbow_Planet_T3_Binary_Effect
        /// 惑星の<tag=CriticalChance>が<tag=BinaryPlanet>の発生率に変換されます。
        public static ModWeapon CrossbowPlanetBinary { get; } = ModWeaponCrossbow.CreateCrossbow("Crossbow_Planet_T3_Binary", 106).SetMainPrefabModifier(main =>
        {
            if (main is WeaponSimple_Crossbow crossbow)
            {
                crossbow.defaultMagazineCapacity = 6;
                crossbow.defaultMagazineCount = 3;
                crossbow.reloadTime = 2f;
                crossbow.fireIntervalTimer.time = 1f / 3f;
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("PLANETDAMAGE", 12) };
                status.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_BinaryPlanet>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Crossbow_Planet_T3_Binary_Effect");
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        });
        /// <summary>
        /// Weapon_GreatSword_Fire_T3_FlameSwordRange_Name
        /// ソリス・レアノ
        /// WeaponAddon_GreatSword_Fire_T3_FlameSwordRange_Effect
        /// <tag=WeaponAction_Sweep>をした時、すべての<tag=FlameSword>を失う代わりに、12個の<tag=FlameSword>を生成して周囲に投げます。
        public static ModWeapon GreatSwordFlameSwordRange { get; } = ModWeapon.CreateWeapon("GreatSword_Fire_T3_FlameSwordRange", 1124, 1122).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_AdditionalElementalDamage>(out var additional))
            {

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_SpecialFlameSword>();

                //@unsafe.status = [CreateWeaponStat("FLAME_SWORD_MAX", -4)];
                @unsafe.effectText = new LocalizedString("WeaponAddon_GreatSword_Fire_T3_FlameSwordRange_Effect");
                @unsafe.parent = additional.parent;

                main.addons = new WeaponAddon[] { additional, @unsafe };
            }
        }).SetBladeSprite().SetBladeUnlitSprite();
        /// <summary>
        /// Weapon_Dagger_Ice_T2_Name
        /// 氷の短剣
        /// WeaponAddon_Dagger_Ice_T2_Effect
        /// <tag=WeaponAction_DirectAttack>時、対象に<tag=IceDamage>の20%分の追加ダメージを与えます。
        public static ModWeapon DaggerIce { get; } = ModWeapon.CreateWeapon("Dagger_Ice_T2", 1203, 20).SetStandardEnhancements(14013, 14014, 14015).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_AdditionalElementalDamage>(out var status))
            {
                status.elementalType = EDamageElementalType.Ice;
                status.additionalDamagePercent = 20;
                status.statId = ECustomStat.IceDamage;
                status.damageId = "Weapon_AdditionalElementalDamage_Ice";
                status.effectText = new LocalizedString("WeaponAddon_Dagger_Ice_T2_Effect");

                main.addons = new WeaponAddon[] { status };
            }
        }).SetBladeSprite(Vector3.zero)
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Basic, 20, () => new ModSpriteFx[] { DaggerIceAttack1Fx, DaggerIceAttack2Fx, DaggerIceAttack3Fx, DaggerIceAttack4Fx, DaggerIceAttack5Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Dash, 20, () => new ModSpriteFx[] { DaggerIceDashFx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Special, 20, () => new ModSpriteFx[] { DaggerIceFuryFx, DaggerIceParryFx });
        /// <summary>
        /// Weapon_Dagger_Ice_T3_Frostbite_Name
        /// 冷風の刃
        /// WeaponAddon_Dagger_Ice_T3_Frostbite_Effect
        /// <tag=Frostbite>状態の敵の攻撃を<tag=WeaponAction_Parry>すると、<tag=WeaponAction_Trance>を追加で獲得します。\n<tag=WeaponAction_SpecialAttack>が<tag=IceDamage>に変更されます。<tag=WeaponAction_Fury>が命中した時、<tag=Frostbite>を付与します。
        public static ModWeapon DaggerIceFrostbite { get; } = ModWeapon.CreateWeapon("Dagger_Ice_T3_Frostbite", 1203).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_AdditionalElementalDamage>(out var status))
            {
                status.elementalType = EDamageElementalType.Ice;
                status.additionalDamagePercent = 20;
                status.statId = ECustomStat.IceDamage;
                status.damageId = "Weapon_AdditionalElementalDamage_Ice";
                status.effectText = new LocalizedString("WeaponAddon_Dagger_Ice_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_FuryFrostbite>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Ice_T3_Frostbite_Effect");
                @unsafe.parent = status.parent;

                if (main is WeaponSimple_Dagger dagger)
                {
                    dagger.maxFury = 2;
                }

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        }).SetBladeSprite(Vector3.zero)
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Basic, 20, () => new ModSpriteFx[] { DaggerIceAttack1Fx, DaggerIceAttack2Fx, DaggerIceAttack3Fx, DaggerIceAttack4Fx, DaggerIceAttack5Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Dash, 20, () => new ModSpriteFx[] { DaggerIceDashFx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Special, 20, () => new ModSpriteFx[] { DaggerIceFuryFx, DaggerIceParryFx })
            .AddFireDataModifier(ModWeapon.EAttackType.Special, x => x.SetDamageElemental(EDamageElementalType.Ice));
        /// <summary>
        /// Weapon_Dagger_Ice_T3_Frost_Name
        /// 静かな氷菓
        /// WeaponAddon_Dagger_Ice_T3_Frost_Effect
        /// <tag=WeaponAction_Parry>に成功すると<tag=WeaponAction_Trance>ではなく<tag=WeaponAction_IceTrance>を獲得します。\n<tag=WeaponAction_IceTrance>状態で<tag=WeaponAction_Parry>に成功すると、すべての<tag=FrostRelic>のチャージが加速します。
        public static ModWeapon DaggerIceFrost { get; } = ModWeapon.CreateWeapon("Dagger_Ice_T3_Frost", 1203).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_AdditionalElementalDamage>(out var status))
            {
                status.elementalType = EDamageElementalType.Ice;
                status.additionalDamagePercent = 20;
                status.statId = ECustomStat.IceDamage;
                status.damageId = "Weapon_AdditionalElementalDamage_Ice";
                status.effectText = new LocalizedString("WeaponAddon_Dagger_Ice_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_IceTrance>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Ice_T3_Frost_Effect");
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        }).SetBladeSprite(Vector3.zero)
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Basic, 20, () => new ModSpriteFx[] { DaggerIceAttack1Fx, DaggerIceAttack2Fx, DaggerIceAttack3Fx, DaggerIceAttack4Fx, DaggerIceAttack5Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Dash, 20, () => new ModSpriteFx[] { DaggerIceDashFx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Special, 20, () => new ModSpriteFx[] { DaggerIceFuryFx, DaggerIceParryFx });
        /// <summary>
        /// Weapon_Dagger_Ice_T3_Magic_Name
        /// 凍てつく剣先
        /// WeaponAddon_Dagger_Ice_T3_Magic_Effect
        /// <tag=IceDamage>が6増加します。<tag=WeaponAction_Parry>に成功した時、霜の短剣が発動します。
        public static ModWeapon DaggerIceMagic { get; } = ModWeapon.CreateWeapon("Dagger_Ice_T3_Magic", 1203).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_AdditionalElementalDamage>(out var status))
            {
                status.elementalType = EDamageElementalType.Ice;
                status.additionalDamagePercent = 20;
                status.statId = ECustomStat.IceDamage;
                status.damageId = "Weapon_AdditionalElementalDamage_Ice";
                status.effectText = new LocalizedString("WeaponAddon_Dagger_Ice_T2_Effect");

                var @unsafe = main.gameObject.AddComponent<WeaponAddonDagger_IceDagger>();


                @unsafe.effectText = new LocalizedString("WeaponAddon_Dagger_Ice_T3_Magic_Effect");
                @unsafe.parent = status.parent;
                @unsafe.status = new WeaponAddonCommon_Status.Stat[] { CreateWeaponStat(ECustomStat.IceDamage, 6) };

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
        }).SetBladeSprite(Vector3.zero)
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Basic, 20, () => new ModSpriteFx[] { DaggerIceAttack1Fx, DaggerIceAttack2Fx, DaggerIceAttack3Fx, DaggerIceAttack4Fx, DaggerIceAttack5Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Dash, 20, () => new ModSpriteFx[] { DaggerIceDashFx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Special, 20, () => new ModSpriteFx[] { DaggerIceFuryFx, DaggerIceParryFx });
        /// <summary>
        /// Weapon_Staff_Sickle_T2_Name
        /// 怨嗟の双鎌
        /// WeaponAddon_Staff_Sickle_T2_Effect
        /// <tag=WeaponAction_DirectAttack>のダメージ量が20%減少しますが、<tag=HighestElementalDamage>に変更されます。
        public static ModWeapon QuarterstaffSickle { get; } = ModWeaponStaff.CreateStaff("Staff_Sickle_T2", 526).SetEnhanceFromId(500).SetStandardEnhancements(14017, 14018).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_ElementalBased>(out var elemental))
            {
                UnityEngine.Object.DestroyImmediate(elemental);
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_Staff_Sickle_T2_Effect");
                status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("STAFFEXTEND", 4) };

                main.addons = new WeaponAddon[] { status };
            }
            if (main is WeaponSimple_QuartterStaff staff)
            {
                staff.currentStaffExtend = 4 * staff.extendPixelPerUnit;
            }
        }).SetBladeSprite(Vector3.zero).SetBorder(new Vector4(0, 18, 0, 19)).SetSizeFromTextureRect()
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Basic, 500, () => new ModSpriteFx[] { StaffSickleAttack1Fx, null, StaffSickleAttack3Fx, StaffSickleAttack1Fx, null, StaffSickleAttack4Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Dash, 500, () => new ModSpriteFx[] { StaffSickleAttack1Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Special, 500, () => new ModSpriteFx[] { StaffSickleAttack3Fx })
            .SetSecondFireDataOverrideSpriteFx(500, true, () => new ModSpriteFx[] { StaffSickleAttack5Fx, StaffSickleAttack5Fx })
            .AddFireDataModifiers(x => x.SetRelatedStatFormulaAndChaos(new Color32(200, 50, 100, 255), "HIGHEST"))
            .AddFireDataModifiers(x => x.SetDamageMultiplier(1.75f * 0.8f));
        /// <summary>
        /// Weapon_Staff_Sickle_T3_Debuff_Name
        /// ユークリッド
        /// WeaponAddon_Staff_Sickle_T3_Debuff_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、<tag=Defense>の絶対値10ごとに、30%の確率で<tag=Debuff_Poison><tag=Burn><tag=Frostbite><tag=Electric>を付与します。
        public static ModWeapon QuarterstaffSickleDebuff { get; } = ModWeaponStaff.CreateStaff("Staff_Sickle_T3_Debuff", 526).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_ElementalBased>(out var elemental))
            {
                UnityEngine.Object.DestroyImmediate(elemental);
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_Staff_Sickle_T2_Effect");
                status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("STAFFEXTEND", 4) };

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_DebuffByDefense>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_Staff_Sickle_T3_Debuff_Effect");
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
            if (main is WeaponSimple_QuartterStaff staff)
            {
                staff.currentStaffExtend = 4 * staff.extendPixelPerUnit;
            }
        }).SetBladeSprite(Vector3.zero).SetBorder(new Vector4(0, 18, 0, 19)).SetSizeFromTextureRect()
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Basic, 500, () => new ModSpriteFx[] { StaffSickleAttack1Fx, null, StaffSickleAttack3Fx, StaffSickleAttack1Fx, null, StaffSickleAttack4Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Dash, 500, () => new ModSpriteFx[] { StaffSickleAttack1Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Special, 500, () => new ModSpriteFx[] { StaffSickleAttack3Fx })
            .SetSecondFireDataOverrideSpriteFx(500, true, () => new ModSpriteFx[] { StaffSickleAttack5Fx, StaffSickleAttack5Fx })
            .AddFireDataModifiers(x => x.SetRelatedStatFormulaAndChaos(new Color32(200, 50, 100, 255), "HIGHEST"))
            .AddFireDataModifiers(x => x.SetDamageMultiplier(1.75f * 0.8f));
        /// <summary>
        /// Weapon_Staff_Sickle_T3_Heal_Name
        /// ラスティ
        /// WeaponAddon_Staff_Sickle_T3_Heal_Effect
        /// <tag=HP>を回復するたび吸魂バフを獲得します。\n<tag=Elemental_Chaos>ダメージを与える時、最大5スタックまで消費して、消費した吸魂スタックごとに<tag=CriticalChance>が10%増加します。
        public static ModWeapon QuarterstaffSickleHeal { get; } = ModWeaponStaff.CreateStaff("Staff_Sickle_T3_Heal", 526).SetMainPrefabModifier(main =>
        {
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_ElementalBased>(out var elemental))
            {
                UnityEngine.Object.DestroyImmediate(elemental);
            }
            if (main.gameObject.TryGetComponent<WeaponAddonCommon_StatusUnsafe>(out var status))
            {
                status.effectText = new LocalizedString("WeaponAddon_Staff_Sickle_T2_Effect");
                status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("STAFFEXTEND", 4) };

                var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_HealWeaponRange>();

                @unsafe.effectText = new LocalizedString("WeaponAddon_Staff_Sickle_T3_Heal_Effect");
                @unsafe.parent = status.parent;

                main.addons = new WeaponAddon[] { status, @unsafe };
            }
            if (main is WeaponSimple_QuartterStaff staff)
            {
                staff.currentStaffExtend = 4 * staff.extendPixelPerUnit;
            }
        }).SetBladeSprite(Vector3.zero).SetBorder(new Vector4(0, 18, 0, 19)).SetSizeFromTextureRect()
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Basic, 500, () => new ModSpriteFx[] { StaffSickleAttack1Fx, null, StaffSickleAttack3Fx, StaffSickleAttack1Fx, null, StaffSickleAttack4Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Dash, 500, () => new ModSpriteFx[] { StaffSickleAttack1Fx })
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Special, 500, () => new ModSpriteFx[] { StaffSickleAttack3Fx })
            .SetSecondFireDataOverrideSpriteFx(500, true, () => new ModSpriteFx[] { StaffSickleAttack5Fx, StaffSickleAttack5Fx })
            .AddFireDataModifiers(x => x.SetRelatedStatFormulaAndChaos(new Color32(200, 50, 100, 255), "HIGHEST"))
            .AddFireDataModifiers(x => x.SetDamageMultiplier(1.75f * 0.8f));
        /// <summary>
        /// Weapon_Staff_Flag_T2_Name
        /// 黄色い旗
        /// WeaponAddon_Staff_Flag_T2_Effect
        /// <tag=Negotiation>が10増加し、<tag=Leaf>の獲得量が50%増加します。
        public static ModWeapon QuarterstaffFlag { get; } = ModWeaponStaff.CreateStaff("Staff_Flag_T2", 500).SetEnhanceFromId(500).SetMainPrefabModifier(main =>
        {
            var status = main.gameObject.AddComponent<WeaponAddonCommon_StatusUnsafe>();
            status.effectText = new LocalizedString("WeaponAddon_Staff_Flag_T2_Effect");
            status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("STAFFEXTEND", 4), CreateWeaponStat("WEAPONRANGE", 30), CreateWeaponStat("MONEYDROP", 50), CreateWeaponStat("NEGOTIATION", 10) };
            status.parent = main;

            main.addons = new WeaponAddon[] { status };

            if (main is WeaponSimple_QuartterStaff staff)
            {
                staff.currentStaffExtend = 4 * staff.extendPixelPerUnit;
            }
        }).SetBladeSprite(Vector3.zero).SetBorder(new Vector4(0, 17, 0, 20)).SetSizeFromTextureRect();
        /// <summary>
        /// Weapon_Staff_Flag_T3_Cheer_Name
        /// 先陣の旗
        /// WeaponAddon_Staff_Flag_T3_Cheer_Effect
        /// <tag=WeaponAction_DirectAttack>が命中した時、{PERCENT}の確率で<tag=Leaf>を{LEAF}消費して、攻撃速度を{SPEED}増加させる励ましの旗を置きます。
        public static ModWeapon QuarterstaffFlagCheer { get; } = ModWeaponStaff.CreateStaff("Staff_Flag_T3_Cheer", 500).SetEnhanceFromId(500).SetEnhanceFromId(14019).SetMainPrefabModifier(main =>
        {
            var status = main.gameObject.AddComponent<WeaponAddonCommon_StatusUnsafe>();
            status.effectText = new LocalizedString("WeaponAddon_Staff_Flag_T2_Effect");
            status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("STAFFEXTEND", 4), CreateWeaponStat("WEAPONRANGE", 30), CreateWeaponStat("MONEYDROP", 50), CreateWeaponStat("NEGOTIATION", 10) };
            status.parent = main;

            var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_CheerFlag>();
            @unsafe.effectText = new LocalizedString("WeaponAddon_Staff_Flag_T3_Cheer_Effect");
            @unsafe.parent = status.parent;

            main.addons = new WeaponAddon[] { status, @unsafe };

            if (main is WeaponSimple_QuartterStaff staff)
            {
                staff.currentStaffExtend = 4 * staff.extendPixelPerUnit;
            }
        }).SetBladeSprite(Vector3.zero).SetBorder(new Vector4(0, 16, 0, 21)).SetSizeFromTextureRect();
        /// <summary>
        /// Weapon_Staff_Flag_T3_Roger_Name
        /// ドレッドノート
        /// WeaponAddon_Staff_Flag_T3_Roger_Effect
        /// <tag=Negotiation>1ごとに<tag=Thorns>が{THORNS}増加します。\n<tag=WeaponAction_BasicAttack>の最後の連撃のダメージが<tag=Thorns>ダメージの80%に変更されます。
        public static ModWeapon QuarterstaffFlagRoger { get; } = ModWeaponStaff.CreateStaff("Staff_Flag_T3_Roger", 500).SetEnhanceFromId(500).SetEnhanceFromId(14019).SetMainPrefabModifier(main =>
        {
            var status = main.gameObject.AddComponent<WeaponAddonCommon_StatusUnsafe>();
            status.effectText = new LocalizedString("WeaponAddon_Staff_Flag_T2_Effect");
            status.status = new WeaponAddonCommon_StatusUnsafe.Stat[] { CreateWeaponStat("STAFFEXTEND", 4), CreateWeaponStat("WEAPONRANGE", 30), CreateWeaponStat("MONEYDROP", 50), CreateWeaponStat("NEGOTIATION", 10) };
            status.parent = main;

            var @unsafe = main.gameObject.AddComponent<WeaponAddonCommon_StealLeaf>();
            @unsafe.effectText = new LocalizedString("WeaponAddon_Staff_Flag_T3_Roger_Effect");
            @unsafe.parent = status.parent;

            main.addons = new WeaponAddon[] { status, @unsafe };

            if (main is WeaponSimple_QuartterStaff staff)
            {
                staff.currentStaffExtend = 4 * staff.extendPixelPerUnit;
            }
        }).SetBladeSprite(Vector3.zero).SetBorder(new Vector4(0, 16, 0, 21)).SetSizeFromTextureRect()
            .SetFireDataChangeSpriteFx(ModWeapon.EAttackType.Basic, 500, () => new ModSpriteFx[] { null, null, null, null, null, StaffFlagCannonAttackFx })
            .AddLastFireDataModifiers(ModWeapon.EAttackType.Basic, x => x.SetRelatedStatFormula(Events.DefenseRelatedStatFormula));
        #endregion

        #region Passives
        /// <summary>
        /// Passive_Grimoire_Name
        /// 追憶
        /// Passive_Grimoire_Description
        /// 魔法に使う様々な記憶を思い出します。
        /// Passive_Grimoire_Effect_LV5
        /// <tag=CooldownRecovery>が+30%増加します。
        /// Passive_Grimoire_Effect_LV10
        /// 全ての<tag=Magic>に魔導書コンボが追加されます。MP再生が+4増加します。
        /// Passive_Grimoire_Effect_LV20
        /// <tag=Magic>ダメージが+20%増加し、<tag=Magic>クリティカル確率が+50%増加します。
        /// </summary>
        public static ModPassive GrimoirePassive { get; } = ModPassive.CreatePassive("Grimoire", new Color32(66, 152, 245, byte.MaxValue), "MAX_MP/2")
            .CreatePerk(EPassivePerkLv.lv5, "CooldownRecovery").SetPerkSupplierStatus("COOLDOWN_RECOVERY_SPEED/30").Parent
            .CreatePerk(EPassivePerkLv.lv10, "AddGrimoire").SetPerkSupplierStatus("ADD_GRIMOIRE/1", "MP_REGEN/4").Parent
            .CreatePerk(EPassivePerkLv.lv20, "AddMagicDamage").SetPerkSupplierStatus("MAGIC_DAMAGE_BONUS/20", "MAGIC_CRITICAL/5000").Parent;

        /// <summary>
        /// Passive_Vampire_Name
        /// 摂理
        /// Passive_Vampire_Description
        /// 王たる理を追求します。
        /// Passive_Vampire_Effect_LV5
        /// <tag=HP>の回復量が{VAL0}%減少しますが、最大HPが{VAL1}増加します。
        /// Passive_Vampire_Effect_LV10
        /// さらに<tag=HP>の回復量が{VAL0}%減少しますが、<tag=WeaponAction_DirectAttack>が命中した時、<tag=Debuff_Poison>デバフを付与します。
        /// Passive_Vampire_Effect_LV20
        /// <tag=Debuff_Poison>デバフの最大スタック数が{VAL1}増加し、<tag=Elemental_Chaos>ダメージの<tag=HPSteal>が{VAL0}増加します。
        /// </summary>
        public static ModPassive VampirePassive { get; } = ModPassive.CreatePassive("Vampire", new Color32(229, 61, 101, byte.MaxValue), "HP_STEAL/1", "TRUE_DAMAGE/-1")
            .CreatePerk(EPassivePerkLv.lv5, "HPDecrease").SetPerkSupplierStatus("HEALING_PENALTY/-25", "MAX_HP/15").Parent
            .CreatePerk(EPassivePerkLv.lv10, "DirectAttackPoison").SetPerkSupplierStatus<ModPassivePerk, PassiveObject_DirectAttackPoison>("HEALING_PENALTY/-25").Parent
            .CreatePerk(EPassivePerkLv.lv20, "ChaosSteal").SetPerkSupplier<ModPassivePerk, PassiveObject_ChaosSteal>().Parent;
        /*/// <summary>
        /// Passive_Movement_Name
        /// 機動
        /// Passive_Movement_Description
        /// 昔から足が早いのが取り柄でした。
        /// Passive_Movement_Effect_LV5
        /// <tag=WeaponAction_DirectAttack>が命中した時、{VAL0}の追加<tag=PhysicalDamage>を与えます。
        /// Passive_Movement_Effect_LV10
        /// <tag=Toughness>と<tag=TrueDamage>が{VAL0}増加します。
        /// Passive_Movement_Effect_LV20
        /// <tag=WeaponAction_DirectAttack>が命中するたび、「旋風」バフを獲得します。（旋風：<tag=SpecialAttackSpeed>が2%増加します）
        /// </summary>
        public static ModPassive MovementPassive { get; } = ModPassive.CreatePassive("Movement", new Color32(168, 226, 61, byte.MaxValue), "MOVE_SPEED/2")
            .CreatePerk(EPassivePerkLv.lv5, "AdditionalDamage").SetPerkSupplier<ModPassivePerk, PassiveObject_AdditionalDamage>().Parent
            .CreatePerk(EPassivePerkLv.lv10, "TrueDamage").SetPerkSupplierStatus("TOUGHNESS/5", "TRUE_DAMAGE/5").Parent
            .CreatePerk(EPassivePerkLv.lv20, "SpecialAttackSpeed").SetPerkSupplier<ModPassivePerk, PassiveObject_SpecialAttackSpeed>().Parent;*/
        #endregion

        #region SpriteFxs
        public static ModSpriteFx DaggerIceAttack1Fx { get; } = ModSpriteFx.CreateSpriteFx("DaggerSwing1Fx_Ice", "DaggerSwing1Fx", $"{ModUtil.WeaponPath}Dagger_Ice\\Weapon_Dagger_Attack1_", 3);
        public static ModSpriteFx DaggerIceAttack2Fx { get; } = ModSpriteFx.CreateSpriteFx("DaggerSwing2Fx_Ice", "DaggerSwing2Fx", $"{ModUtil.WeaponPath}Dagger_Ice\\Weapon_Dagger_Attack2_", 3);
        public static ModSpriteFx DaggerIceAttack3Fx { get; } = ModSpriteFx.CreateSpriteFx("DaggerSwing3Fx_Ice", "DaggerSwing3Fx", $"{ModUtil.WeaponPath}Dagger_Ice\\Weapon_Dagger_Attack3_", 3);
        public static ModSpriteFx DaggerIceAttack4Fx { get; } = ModSpriteFx.CreateSpriteFx("DaggerSwing4Fx_Ice", "DaggerSwing4Fx", $"{ModUtil.WeaponPath}Dagger_Ice\\Weapon_Dagger_Attack4_", 3);
        public static ModSpriteFx DaggerIceAttack5Fx { get; } = ModSpriteFx.CreateSpriteFx("DaggerSwing5Fx_Ice", "DaggerSwing5Fx", $"{ModUtil.WeaponPath}Dagger_Ice\\Weapon_Dagger_Attack5_", 4);
        public static ModSpriteFx DaggerIceDashFx { get; } = ModSpriteFx.CreateSpriteFx("DaggerDashFx_Ice", "DaggerDashFx", $"{ModUtil.WeaponPath}Dagger_Ice\\Weapon_Dagger_DashAttack_", 2);
        public static ModSpriteFx DaggerIceParryFx { get; } = ModSpriteFx.CreateSpriteFx("DaggerParryFx_Ice", "DaggerParryFx", $"{ModUtil.WeaponPath}Dagger_Ice\\Weapon_Dagger_ParryAttack_", 6).SetCopyPivot();
        public static ModSpriteFx DaggerIceFuryFx { get; } = ModSpriteFx.CreateSpriteFx("DaggerFuryFx_Ice", "DaggerFuryFx_Stack", $"{ModUtil.WeaponPath}Dagger_Ice\\Weapon_Dagger_Fury0_", 6);
        public static ModSpriteFx StaffSickleAttack1Fx { get; } = ModSpriteFx.CreateSpriteFx("StaffSwing123Fx1_Sickle", "StaffSwingFx_Attack1", $"{ModUtil.WeaponPath}Staff_Sickle\\Weapon_Staff_Attack1_", 8).SetFps(20);
        public static ModSpriteFx StaffSickleAttack2Fx { get; } = ModSpriteFx.CreateSpriteFx("StaffSwing123Fx2_Sickle", "StaffSwingFx_Attack2", $"{ModUtil.WeaponPath}Staff_Sickle\\Weapon_Staff_Attack2_", 7).SetFps(20);
        public static ModSpriteFx StaffSickleAttack3Fx { get; } = ModSpriteFx.CreateSpriteFx("StaffSwing123Fx3_Sickle", "StaffSwingFx_Attack2", $"{ModUtil.WeaponPath}Staff_Sickle\\Weapon_Staff_Attack3_", 7).SetFps(20);
        public static ModSpriteFx StaffSickleAttack4Fx { get; } = ModSpriteFx.CreateSpriteFx("StaffSwing123Fx4_Sickle", "StaffSwingFx_Attack3", $"{ModUtil.WeaponPath}Staff_Sickle\\Weapon_Staff_Attack4_", 5).SetFps(20);
        public static ModSpriteFx StaffSickleAttack5Fx { get; } = ModSpriteFx.CreateSpriteFx("StaffSwing123Fx5_Sickle", "StaffSwingFx_Attack1", $"{ModUtil.WeaponPath}Staff_Sickle\\Weapon_Staff_Attack5_", 8).SetFps(20);
        public static ModSpriteFx StaffFlagCannonAttackFx { get; } = ModSpriteFx.CreateSpriteFx("StaffSwing123Fx_FlagCannon", "StaffSwingFx_Attack3", $"{ModUtil.WeaponPath}Staff_Cannon\\Weapon_Staff_Attack4_", 7).SetFps(30);
        #endregion

        public static ModSephirite SephiriteJewelry { get; } = ModSephirite.Create<Sephirite_Jewelry>("Jewelry").SetAppearLimit(2);
        public static ModSephirite SephiriteBond { get; } = ModSephirite.Create<Sephirite_Bond>("Bond").SetAppearLimit(3);

        public static ModTreeShopItem NewCharmBond2 { get; } = ModTreeShopItem.CreateTreeShopItem("NewCharm_Bond2", string.Empty,
            TreeShopItems.NewCharmBond2, TreeShopItems.NewCharmBond1, ModTreeShopItem.ELinePos.Right, 8);
        public static ModTreeShopItem NewCharmDrunk { get; } = ModTreeShopItem.CreateTreeShopItem("NewCharm_Drunk", string.Empty,
            TreeShopItems.NewCharmDrunk, TreeShopItems.NewCharmBond1, ModTreeShopItem.ELinePos.Center, 7).SetIcon(() => CustomSpriteAsset.TreeIconArtifact);
        public static ModTreeShopItem NewCharmSacrifice { get; } = ModTreeShopItem.CreateTreeShopItem("NewCharm_Sacrifice", "NewCharm_Sacrifice",
            TreeShopItems.NewCharmSacrifice, TreeShopItems.RewardDiceRightUp, ModTreeShopItem.ELinePos.Right, 5).SetIcon(() => CustomSpriteAsset.TreeIconArtifactSacrifice);
        
        public static Sprite IconInWorldPotion { get; internal set; }
        public static Sprite IconInWorldCharm { get; internal set; }
        public static Sprite IconInWorldTablet { get; internal set; }

        public static EventReference DefaultEnableSound { get; internal set; }


        public static void GenerateItem(this LevelController level, ModSephirite sephirite, int seed)
        {
            Debug.Log("[MiraItemMod][LevelController] GenerateItem: " + sephirite.Name);
            GameObject gameObject = UnityEngine.Object.Instantiate(sephirite.Prefab, new Vector3(-1000f, -1000f), Quaternion.identity);
            var identity = gameObject.AddComponent<NetworkIdentity>();
            identity.SetAssetId(sephirite.AssetId);
            Sephirite component = gameObject.GetComponent<Sephirite>();
            component.Initialize(seed);
            NetworkServer.Spawn(gameObject, level.gameObject);
            Debug.Log("[MiraItemMod][LevelController] Spawn ModSephirite: " + gameObject.name);
            level.levelUpQueue.Add(component);
        }

        #region CreateParameters
        public static Charm_StatusInstance.StatusGroup CreateStatusGroup(string id, params int[] values)
        {
            return new Charm_StatusInstance.StatusGroup() { statusID = id, valuesByLevel = values };
        }
        public static Charm_StatusInstance.StatusGroup CreateStatusGroupHide(string id, params int[] values)
        {
            return new Charm_StatusInstance.StatusGroup() { statusID = id, valuesByLevel = values, hideIfStatValueIsZero = true };
        }
        public static Charm_StatusInstance.StatusGroup CreateStatusGroupBy(string id, int baseValue)
        {
            var list = new List<int>();
            for(int q = 0; q < 20; q++)
            {
                list.Add(baseValue * (q + 1));
            }
            return new Charm_StatusInstance.StatusGroup() { statusID = id, valuesByLevel = list.ToArray(), hideIfStatValueIsZero = false };
        }
        public static Charm_StatusInstance.StatusGroup CreateStatusGroupBy(string id, int zero, int baseValue)
        {
            var list = new List<int>();
            for (int q = 0; q < 20; q++)
            {
                if (q < zero)
                    list.Add(0);
                else
                    list.Add(baseValue * ((q - zero) + 1));
            }
            return new Charm_StatusInstance.StatusGroup() { statusID = id, valuesByLevel = list.ToArray(), hideIfStatValueIsZero = false };
        }
        public static Charm_AddOrphanedStatusInstance.OrphanedStatusGroup CreateOrphanedStatusGroup(string id, int value)
        {
            return new Charm_AddOrphanedStatusInstance.OrphanedStatusGroup() { statusID = id, value = value };
        }
        public static ComboEffectBase.ComboStat CreateComboStat(int count, params string[] status)
        {
            return new ComboEffectBase.ComboStat() { comboCount = count, status = status };
        }
        public static ComboEffectBase.ComboStat CreateComboStatReplaceText(int count, string id, params string[] status)
        {
            return new ComboEffectBase.ComboStat() { comboCount = count, status = status, replaceEffectText = true, replaceEffectTextString = new LocalizedString($"ComboEffectDefault_{id}_Effect{count}") };
        }
        public static ComboEffectBase.ComboStat CreateComboStatThreeDamage(int count, string id, int status)
        {
            return new ComboEffectBase.ComboStat() { comboCount = count, status = new string[] { "FIRE_DAMAGE/" + status, "ICE_DAMAGE/" + status, "LIGHTNING_DAMAGE/" + status },
                replaceEffectText = true, replaceEffectTextString = new LocalizedString($"ComboEffectDefault_{id}_Effect{count}") };
        }
        public static Miracle_StatusInstance.StatInfo CreatePositiveStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Positive, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreateNegativeStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Negative, status = status };
        }
        public static Miracle_StatusInstance.StatInfo CreateNeutralStat(string status)
        {
            return new Miracle_StatusInstance.StatInfo() { type = EEffectType.Neutral, status = status };
        }
        public static WeaponAddonCommon_Status.Stat CreateWeaponStat(ECustomStat statId, int value)
        {
            return new WeaponAddonCommon_Status.Stat() { statId = statId, value = value };
        }
        public static WeaponAddonCommon_StatusUnsafe.Stat CreateWeaponStat(string statId, int value)
        {
            return new WeaponAddonCommon_StatusUnsafe.Stat() { statId = statId, value = value };
        }
        #endregion

        public static void Init()
        {
            IconInWorldCharm = AssetLoader.LoadSprite(ModUtil.MiscPath + "ItemInWorld_Charm");
            IconInWorldTablet = AssetLoader.LoadSprite(ModUtil.MiscPath + "ItemInWorld_TabletStone");
            IconInWorldPotion = AssetLoader.LoadSprite(ModUtil.MiscPath + "ItemInWorld_All");

            var type = typeof(Data);
            var pros = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType.IsSubclassOf(typeof(ModItem)));
            int id = GetFirstModId();
            uint assetId = GetFirstAssetId();
            foreach (var pro in pros)
            {
                if (Core.LogFew)
                    Core.Logger("New Item: " + pro.Name);
                var moditem = pro.GetValue(type) as ModItem;
                moditem.Init(id++, assetId);
                assetId = GetNextAssetId(assetId);
                All.Add(moditem);
                if (moditem.IsJewelry && !moditem.IsExcludedJewelry)
                {
                    var category = moditem.Categories.FirstOrDefault();
                    if (!Jewelries.ContainsKey(category))
                        Jewelries[category] = new List<ModCharm>();
                    Jewelries[category].Add(moditem as ModCharm);
                }
            }
            AllResourcePrefabNames = new List<string>();
            foreach (var item in All)
            {
                //Core.Logger("LateInit Item: " + item.Name);
                item.LateInit();
                AllResourcePrefabNames.Add(item.ResourcePrefab.name);
            }
            //AllResourcePrefabNames = All.Select(x => x.ResourcePrefab.name).ToList();


            var pros2 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModComboEffect) || p.PropertyType.IsSubclassOf(typeof(ModComboEffect)));
            foreach (var pro in pros2)
            {
                if (Core.LogFew)
                    Core.Logger("New Category: " + pro.Name);
                var moditem = pro.GetValue(type) as ModComboEffect;
                moditem.Init(assetId);
                assetId = GetNextAssetId(assetId);
                Combos.Add(moditem);
            }

            var pros3 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModEffectHUD) || p.PropertyType.IsSubclassOf(typeof(ModEffectHUD)));
            foreach (var pro in pros3)
            {
                var moditem = pro.GetValue(type) as ModEffectHUD;
                if (Core.LogFew)
                    Core.Logger("New EffectHUD: " + pro.Name);
                EffectHUDs.Add(moditem);
            }

            var pros4 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModMiracle) || p.PropertyType.IsSubclassOf(typeof(ModMiracle)));
            foreach (var pro in pros4)
            {
                var moditem = pro.GetValue(type) as ModMiracle;
                if (Core.LogFew)
                    Core.Logger("New Miracle: " + pro.Name);
                moditem.Init(assetId);
                assetId = GetNextAssetId(assetId);
                Miracles.Add(moditem);
            }
            var pros5 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModCustomStatus) || p.PropertyType.IsSubclassOf(typeof(ModCustomStatus)));
            foreach (var pro in pros5)
            {
                var moditem = pro.GetValue(type) as ModCustomStatus;
                if (Core.LogFew)
                    Core.Logger("New Status: " + pro.Name);
                moditem.Init();
                Statuses.Add(moditem);
            }
            id = GetFirstWeaponId();
            var pros6 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModWeapon) || p.PropertyType.IsSubclassOf(typeof(ModWeapon)));
            foreach (var pro in pros6)
            {
                var moditem = pro.GetValue(type) as ModWeapon;
                if (Core.LogFew)
                    Core.Logger("New Weapon: " + pro.Name);
                moditem.Init(id++, assetId);
                assetId = GetNextAssetId(assetId);
                Weapons.Add(moditem);
            }
            var pros7 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(CharacterBuffMod) || p.PropertyType.IsSubclassOf(typeof(CharacterBuffMod)));
            foreach (var pro in pros7)
            {
                var moditem = pro.GetValue(type) as CharacterBuffMod;
                if (Core.LogFew)
                    Core.Logger("New Buff: " + pro.Name);
                moditem.AssetId = assetId;
                assetId = GetNextAssetId(assetId);
                Buffs.Add(moditem);
            }
            var pros8 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModKeyword) || p.PropertyType.IsSubclassOf(typeof(ModKeyword)));
            foreach (var pro in pros8)
            {
                var moditem = pro.GetValue(type) as ModKeyword;
                if (Core.LogFew)
                    Core.Logger("New Keyword: " + pro.Name);
                moditem.Init();
                Keywords.Add(moditem);
            }
            var passiveId = GetFirstPassiveId();
            var pros9 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModPassive) || p.PropertyType.IsSubclassOf(typeof(ModPassive)));
            foreach (var pro in pros9)
            {
                var moditem = pro.GetValue(type) as ModPassive;
                if (Core.LogFew)
                    Core.Logger("New Passive: " + pro.Name);
                var a = assetId;
                assetId = GetNextAssetId(assetId);
                var b = assetId;
                assetId = GetNextAssetId(assetId);
                var c = assetId;
                assetId = GetNextAssetId(assetId);
                moditem.Init(passiveId++, a, b, c);
                Passives.Add(moditem);
            }
            var pros10 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModSpriteFx) || p.PropertyType.IsSubclassOf(typeof(ModSpriteFx)));
            foreach (var pro in pros10)
            {
                var moditem = pro.GetValue(type) as ModSpriteFx;
                if (Core.LogFew)
                    Core.Logger("New SpriteFx: " + pro.Name);
                //moditem.Init(passiveId++, assetId++, assetId++, assetId++);
                SpriteFxs.Add(moditem);
            }
            var pros11 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModSephirite) || p.PropertyType.IsSubclassOf(typeof(ModSephirite)));
            foreach (var pro in pros11)
            {
                var moditem = pro.GetValue(type) as ModSephirite;
                if (Core.LogFew)
                    Core.Logger("New Sephirite: " + pro.Name);
                moditem.Init(assetId);
                assetId = GetNextAssetId(assetId);
                Sephirites.Add(moditem);
            }
            var pros12 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModTreeShopItem) || p.PropertyType.IsSubclassOf(typeof(ModTreeShopItem)));
            foreach (var pro in pros12)
            {
                var moditem = pro.GetValue(type) as ModTreeShopItem;
                if (Core.LogFew)
                    Core.Logger("New TreeShop: " + pro.Name);
                TreeShops.Add(moditem);
            }
            //CustomCostumeDatabase.Initialize();
        }
        #region Registers
        public static void Register(List<UnityEngine.Object> list)
        {
            foreach (var moditem in All)
            {
                list.Add(moditem.ItemEntity);
            }
        }
        public static void RegisterItems()
        {
            foreach(var moditem in All)
            {
                ItemDatabase.Register(moditem.ItemEntity);
            }
        }
        public static void RegisterDamageIds(List<UnityEngine.Object> list)
        {
            foreach (var moditem in All)
            {
                if (moditem is IModDamageId charm && charm.HasDamageId)
                {
                    if (Core.LogFew)
                        Core.Logger("New DamageId: " + charm.DamageIdEntity.name);
                    list.Add(charm.DamageIdEntity);
                }
            }
            foreach (var moditem in Combos)
            {
                if (moditem is IModDamageId charm && charm.HasDamageId)
                {
                    if (Core.LogFew)
                        Core.Logger("New DamageId: " + charm.DamageIdEntity.name);
                    list.Add(charm.DamageIdEntity);
                }
            }
            foreach (var moditem in Passives)
            {
                if (moditem.Lv5Perk != null && moditem.Lv5Perk is IModDamageId perk5 && perk5.HasDamageId)
                {
                    if (Core.LogFew)
                        Core.Logger("New DamageId: " + perk5.DamageIdEntity.name);
                    list.Add(perk5.DamageIdEntity);
                }
                if (moditem.Lv10Perk != null && moditem.Lv10Perk is IModDamageId perk10 && perk10.HasDamageId)
                {
                    if (Core.LogFew)
                        Core.Logger("New DamageId: " + perk10.DamageIdEntity.name);
                    list.Add(perk10.DamageIdEntity);
                }
                if (moditem.Lv20Perk != null && moditem.Lv20Perk is IModDamageId perk20 && perk20.HasDamageId)
                {
                    if (Core.LogFew)
                        Core.Logger("New DamageId: " + perk20.DamageIdEntity.name);
                    list.Add(perk20.DamageIdEntity);
                }
            }
        }
        public static void RegisterCombos(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Combos)
            {
                list.Add(moditem.ItemCategoryEntity);
            }
        }

        public static void RegisterEffectHUDs(List<UnityEngine.Object> list)
        {
            GameObject stack = null;
            foreach(var o in list)
            {
                if(o is EffectHUDEntity entity)
                {
                    if(entity.id == "ABANDONEDGOLDRING")
                    {
                        stack = entity.hudPrefab;
                    }
                }
            }
            foreach(var moditem in EffectHUDs)
            {
                if(moditem.Type == ModEffectHUD.EffectHUDType.Stack && stack != null)
                {
                    var prefab = UnityEngine.Object.Instantiate(stack);
                    moditem.SetResourcePrefab(prefab);
                    list.Add(moditem.CreateEntity());
                }
            }
        }
        public static void RegisterMiracles(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Miracles)
            {
                list.Add(moditem.Prefab);
            }
        }
        public static void RegisterMiracles()
        {
            foreach (var moditem in Miracles)
            {
                MiracleDatabase.Register(moditem.Prefab);
            }
        }
        public static void LoadMiracleManuallyGivenItems()
        {
            foreach (var miracle in Data.Miracles)
            {
                miracle.LoadManuallyGivenItems();
            }
        }
        public static void RegisterTreeShopItems()
        {
            foreach(var entity in TreeShopItemDatabase.GetAll())
            {
                var list = new List<ItemEntity>(entity.items);
                /*
                if (entity.id == 14000)//絆解放2
                {
                    var locked = Data.All.Where(x => x.IsDual && x.Rarity == EItemRarity.Rare).Select(x => x.ItemEntity).ToArray();
                    locked.Do(entity => entity.activeType = EItemActiveType.Locked);
                    entity.items = entity.items.AddRangeToArray(locked);
                    //entity.items = entity.items.AddItem(ItemDatabase.FindItemById(1188)).ToArray();
                }
                if (entity.id == 1005)
                {
                    var locked = new ModItem[] { Data.CopyAcademy, Data.AutoBuff, Data.AutoMagicLegend };
                    locked.Do(entity => entity.ItemEntity.activeType = EItemActiveType.Locked);
                    entity.items = entity.items.AddRange(locked);
                }*/
                foreach (var item in Data.All)
                {
                    if (!item.HasTreeShopItemEntity && entity.id == TreeShopItems.NewCharmDrunk && item.Categories.Contains(ItemCategories.Drunk))
                    {
                        item.ItemEntity.activeType = EItemActiveType.Locked;
                        list.Add(item.ItemEntity);
                    }
                    if (!item.HasTreeShopItemEntity && entity.id == TreeShopItems.NewCharmSacrifice && item.Rarity == ECustomItemRarity.Sacrifice.ToSephiria())
                    {
                        item.ItemEntity.activeType = EItemActiveType.Locked;
                        list.Add(item.ItemEntity);
                    }
                    if (!item.HasTreeShopItemEntity && entity.id == TreeShopItems.NewCharmBond2 && item.IsDual && item.Rarity == EItemRarity.Rare && !item.Categories.Contains(ItemCategories.Drunk))
                    {
                        item.ItemEntity.activeType = EItemActiveType.Locked;
                        list.Add(item.ItemEntity);
                    }
                    if (item.HasTreeShopItemEntity && entity.id == item.TreeShopItemEntity.Value)
                    {
                        item.ItemEntity.activeType = EItemActiveType.Locked;
                        list.Add(item.ItemEntity);
                    }
                }
                entity.items = list.ToArray();
            }
            //3001 Chapter5Complete 例：北向きの針の時計
            //2009 RootDemon 例：衝撃増幅器、石版
            //2008 LizardDemon 例：神秘の振り子、平和
            //2007 BirdDemon 例：虹の羽
            //2006 LibraryGuard 例：霜焼けクラゲ、瞑想の書
            //2005 Boss Panther 例：白い紙
            //2004 Boss BigBomb 例：多目的ベルト、青い輪、仲間モグディ
            //2003 Boss Askard 例：石版（Daydream、Spike）
            //2002 Boss Armadillo 例：六つ葉のクローバー、ガラスハンマー、ベルート、アグマ
            //2001 Boss Oink 例：クナイ、緑色の造服、呪いデバフ延長
            //2000 Boss Mole 例：ヘルメット、ポーション強化キャップ、蛍
            //1007 例：フォールトファインダーニードル
            //1006 絆 例：
            //1005 魔法2 例：ライトニングアーマー
            //1004 例：キリンの角、オブラスの血
            //1003 例：スタールビー、翼
            //1002 例：割れた鏡、スターアクアマリン、黒い鱗
            //1001 例：盾のイヤリング
            //1000 魔法1　例：ファイアボルト
        }
        public static void RegisterStatuses(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Statuses)
            {
                list.Add(moditem.StatusEntity);
            }
        }
        public static void RegisterStatuses()
        {
            foreach (var moditem in Statuses)
            {
                StatusDatabase.Register(moditem.StatusEntity);
            }
        }
        public static void RegisterWeapons(List<UnityEngine.Object> list)
        {
            var weapons = list.Select(x => x as WeaponEntity).ToList();


            foreach (var moditem in Weapons)
            {
                WeaponEntity copy = null;
                foreach(var w in weapons)
                {
                    if(moditem.Copy == w.id)
                    {
                        copy = w;
                    }
                }
                if (copy == null)
                    continue;
                if (moditem.WeaponEntity == null)
                    moditem.Init(copy);
                else if (moditem.MainWeaponPrefab == null)
                {
                    moditem.InitPrefab(copy);
                    moditem.WeaponEntity.mainWeaponPrefab = moditem.MainWeaponPrefab;
                }
                list.Add(moditem.WeaponEntity);
            }


            foreach (var w in weapons)
            {
                foreach (var moditem in Weapons)
                {
                    if (moditem.Dependency == -1)
                        continue;
                    if (w.id == moditem.Dependency && moditem.WeaponEntity != null && !w.standardEnhancements.Select(x => x.enhanced.id).Contains(moditem.Id))
                    {
                        w.standardEnhancements.Add(new EnhancementMetadata() { enabled  = true, enhanced = moditem.WeaponEntity });
                    }
                }
            }

            var newweapons = list.Select(x => x as WeaponEntity).ToList();
            foreach (var moditem in Weapons)
            {
                var enhances = new List<WeaponEntity>();
                foreach(var enhancement in moditem.StandardEnhancements)
                {
                    WeaponEntity copy = null;
                    foreach (var w in newweapons)
                    {
                        if (enhancement == w.id)
                        {
                            copy = w;
                        }
                    }
                    if (copy == null)
                        continue;
                    enhances.Add(copy);
                }
                if (moditem.WeaponEntity != null)
                    moditem.WeaponEntity.standardEnhancements = enhances.Select(x => new EnhancementMetadata() { enabled = true, enhanced = x }).ToList();
            }
        }
        public static void RegisterWeapons()
        {
            var weapons = WeaponDatabase.GetAll();


            foreach (var moditem in Weapons)
            {
                WeaponEntity copy = null;
                foreach (var w in weapons)
                {
                    if (moditem.Copy == w.id)
                    {
                        copy = w;
                    }
                }
                if (copy == null)
                    continue;
                if (moditem.WeaponEntity == null)
                    moditem.Init(copy);
                else if (moditem.MainWeaponPrefab == null)
                {
                    moditem.InitPrefab(copy);
                    moditem.WeaponEntity.mainWeaponPrefab = moditem.MainWeaponPrefab;
                }
                WeaponDatabase.Register(moditem.WeaponEntity);
                weapons.Add(moditem.WeaponEntity);
            }


            foreach (var w in weapons)
            {
                foreach (var moditem in Weapons)
                {
                    if (moditem.Dependency == -1)
                        continue;
                    if (w.id == moditem.Dependency && moditem.WeaponEntity != null && !w.standardEnhancements.Select(x => x.enhanced.id).Contains(moditem.Id))
                    {
                        w.standardEnhancements.Add(new EnhancementMetadata() { enabled = true, enhanced = moditem.WeaponEntity });
                    }
                }
            }

            var newweapons = weapons.ToList();
            foreach (var moditem in Weapons)
            {
                var enhances = new List<WeaponEntity>();
                foreach (var enhancement in moditem.StandardEnhancements)
                {
                    WeaponEntity copy = null;
                    foreach (var w in newweapons)
                    {
                        if (enhancement == w.id)
                        {
                            copy = w;
                        }
                    }
                    if (copy == null)
                        continue;
                    enhances.Add(copy);
                }
                if (moditem.WeaponEntity != null)
                    moditem.WeaponEntity.standardEnhancements = enhances.Select(x => new EnhancementMetadata() { enabled = true, enhanced = x }).ToList();
            }
        }
        public static void RegisterCostume(List<UnityEngine.Object> list)
        {
            if (list[0] is CostumeEntity entity && entity.costumePrefab.TryGetComponent<PlayerAvatarCostume>(out var costume))
            {
                CustomCostumeDatabase.InitializeGameObject(costume);
                list.AddRange(CustomCostumeDatabase.CreateAll());
            }
        }
        public static void RegisterCostumeSkin(List<UnityEngine.Object> list)
        {
            list.AddRange(CustomCostumeDatabase.CreateAllSkin());
        }
        public static void RegisterKeywords(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Keywords)
            {
                list.Add(moditem.KeywordEntity);
            }
            foreach (var moditem in Statuses)
            {
                if(moditem.HasKeyword)
                    list.Add(moditem.KeywordEntity);
            }
            foreach (var item in list)
            {
                if (item is KeywordEntity entity)
                {
                    foreach (var moditem in Keywords)
                    {
                        moditem.Init(entity);
                    }
                }
            }
            foreach (var item in list)
            {
                if (item is KeywordEntity entity)
                {
                    foreach (var moditem in Statuses)
                    {
                        if (moditem.HasKeyword)
                            moditem.Keyword.Init(entity);
                    }
                }
            }
        }

        public static void RegisterPassives(List<UnityEngine.Object> list)
        {
            foreach (var moditem in Passives)
            {
                list.Add(moditem.PassiveEntity);
            }
        }

        public static void RegisterBuffs(List<UnityEngine.Object> list)
        {
            foreach(var moditem in Buffs)
            {
                list.Add(moditem);
            }
        }
        public static void RegisterDebuffs(List<UnityEngine.Object> list)
        {

        }
        public static void RegisterTreeShops(List<UnityEngine.Object> list)
        {
            foreach(var item in list)
            {
                if (item is TreeShopItemEntity entity)
                {
                    foreach (var moditem in TreeShops)
                    {
                        if(moditem.Copy == entity.id)
                        {
                            moditem.Init(entity);
                        }
                    }
                }
            }
            foreach (var moditem in TreeShops)
            {
                list.Add(moditem.Entity);
            }
            foreach (var item in list)
            {
                if (item is TreeShopItemEntity entity)
                {
                    foreach (var moditem in TreeShops)
                    {
                        if (moditem.Dependency == entity.id)
                        {
                            moditem.SetDependency(entity);
                        }
                    }
                }
            }
        }

        public static void Dispose()
        {
            foreach(var moditem in All)
            {
                moditem.Dispose();
            }
            foreach(var moditem in Buffs)
            {
                GameObject.Destroy(moditem.gameObject);
            }
            foreach(var moditem in Statuses)
            {
                moditem.Dispose();
            }
            foreach(var moditem in Weapons)
            {
                moditem.Dispose();
            }
            foreach(var moditem in Passives)
            {
                moditem.Dispose();
            }
            foreach(var moditem in SpriteFxs)
            {
                //moditem.Dispose();
            }
            foreach(var moditem in Sephirites)
            {
                moditem.Dispose();
            }
            foreach(var moditem in TreeShops)
            {
                moditem.Dispose();
            }
            foreach(var moditem in Keywords)
            {
                moditem.Dispose();
            }
            All = new List<ModItem>();
            Combos = new List<ModComboEffect>();
            EffectHUDs = new List<ModEffectHUD>();
            Miracles = new List<ModMiracle>();
            Buffs = new List<CharacterBuffMod>();
            Statuses = new List<ModCustomStatus>();
            Weapons = new List<ModWeapon>();
            Passives = new List<ModPassive>();
            SpriteFxs = new List<ModSpriteFx>();
            Sephirites = new List<ModSephirite>();
            TreeShops = new List<ModTreeShopItem>();
            Keywords = new List<ModKeyword>();
            Jewelries = new Dictionary<string, List<ModCharm>>();

        }
        #endregion

        #region GetId
        public static int GetFirstModId()
        {
            return 14000;
        }
        public static int GetFirstWeaponId()
        {
            return 14000;
        }
        public static uint GetFirstAssetId()
        {
            return 5;
        }
        public static uint GetNextAssetId(uint previous)
        {
            do
            {
                previous++;
            }
            while (NetworkClient.prefabs.ContainsKey(previous));
            return previous;
        }
        public static ulong GetFirstPassiveId()
        {
            return 140;
        }
        #endregion

        public static T CreateBuff<T>(string name, string hud) where T : CharacterBuffMod
        {
            var ob = new GameObject("CharacterBuff_" + name);
            ob.hideFlags = HideFlags.HideAndDontSave;
            var buff = ob.AddComponent<T>();
            buff.id = name.ToFileNameUpper();
            buff.HUD_ID = hud.ToSephiriaUpperId();
            buff.enabled = false;
            return buff;
        }
        public static T CreateBuff<T>(string name, string hud, int stack, params BuffStatus[] status) where T : CharacterBuffMod_StatusInstance
        {
            var ob = new GameObject("CharacterBuff_" + name);
            ob.hideFlags = HideFlags.HideAndDontSave;
            var buff = ob.AddComponent<T>();
            buff.id = name.ToFileNameUpper();
            buff.HUD_ID = hud.ToSephiriaUpperId();
            buff.maxStackCount = stack;
            buff.add = status;
            buff.enabled = false;
            return buff;
        }
        public static CharacterBuffMod_StatusInstance CreateBuff(string name, string hud, int stack, params BuffStatus[] status)
        {
            var ob = new GameObject("CharacterBuff_" + name);
            ob.hideFlags = HideFlags.HideAndDontSave;
            var log = ob.AddComponent<LogComponent>();
            var buff = ob.AddComponent<CharacterBuffMod_StatusInstance>();
            buff.id = name.ToFileNameUpper();
            buff.HUD_ID = hud.ToSephiriaUpperId();
            buff.maxStackCount = stack;
            buff.add = status;
            buff.enabled = false;
            return buff;
        }
        public static BuffStatus CreateBuffStatus(string id, int value)
        {
            return new BuffStatus() { id = id, value = value };
        }
    }
}
