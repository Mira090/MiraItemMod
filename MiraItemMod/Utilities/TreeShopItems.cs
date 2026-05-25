using System;
using System.Collections.Generic;
using System.Text;

namespace MiraItemMod.Utilities
{
    public static class TreeShopItems
    {
        /// <summary>
        /// 最初の左
        /// </summary>
        /// <remarks>例：ファイアボルト、空の剣の握り、ライリーの懐中時計、雷雲追跡コンパス</remarks>
        public static int NewCharmMagic1 { get; } = 1000;
        /// <summary>
        /// 最初の中央の上
        /// </summary>
        /// <remarks>例：雷の裁き、戦闘魔法使いの手袋、サンダーアーマー</remarks>
        public static int NewCharmMagic2 { get; } = 1005;
        /// <summary>
        /// 最初の右
        /// </summary>
        /// <remarks>例：盾のイヤリング、鈍った鈴</remarks>
        public static int NewCharmRight1 { get; } = 1001;
        /// <summary>
        /// 最初の中央
        /// </summary>
        /// <remarks>例：割れた鏡、スターアクアマリン、黒い鱗</remarks>
        public static int NewCharmLeft1 { get; } = 1002;
        /// <summary>
        /// 最初の右の上
        /// </summary>
        /// <remarks>例：スタールビー、翼</remarks>
        public static int NewCharmRight2 { get; } = 1003;
        /// <summary>
        /// 最初の右上
        /// </summary>
        /// <remarks>例：キリンの角、オブラスの血</remarks>
        public static int NewCharmRight3 { get; } = 1004;
        /// <summary>
        /// 左上
        /// </summary>
        /// <remarks>例：フォールトファインダーニードル、さすらい人の首飾り、獣の心臓</remarks>
        public static int NewCharmLeft2 { get; } = 1007;

        /// <summary>
        /// モグディ
        /// </summary>
        /// <remarks>例：ヘルメット、ポーション強化キャップ、蛍</remarks>
        public static int BossMole { get; } = 2000;
        /// <summary>
        /// ラタカ
        /// </summary>
        /// <remarks>例：クナイ、緑の造服、呪いネックレス、入口</remarks>
        public static int BossOink { get; } = 2001;
        /// <summary>
        /// エルマ
        /// </summary>
        /// <remarks>例：六つ葉のクローバー、ガラスハンマー、ベルート、アグマ</remarks>
        public static int BossArmadillo { get; } = 2002;
        /// <summary>
        /// アスカード
        /// </summary>
        /// <remarks>例：白日夢、棘</remarks>
        public static int BossAskard { get; } = 2003;
        /// <summary>
        /// オードナー
        /// </summary>
        /// <remarks>例：多目的ベルト、青い輪、斜め石版</remarks>
        public static int BossBigBomb { get; } = 2004;
        /// <summary>
        /// レリッド
        /// </summary>
        /// <remarks>例：石版</remarks>
        public static int BossPanther { get; } = 2005;
        /// <summary>
        /// 図書館の番人
        /// </summary>
        /// <remarks>例：ヘイタの魂の粉、瞑想の書、確信</remarks>
        public static int BossLibraryGuard { get; } = 2006;
        /// <summary>
        /// コベス
        /// </summary>
        /// <remarks>例：虹の羽</remarks>
        public static int BossBirdDemon { get; } = 2007;
        /// <summary>
        /// クラーズ
        /// </summary>
        /// <remarks>例：神秘の振り子、平和</remarks>
        public static int BossLizardDemon { get; } = 2008;
        /// <summary>
        /// 根の悪魔（1階ボス）
        /// </summary>
        /// <remarks>例：衝撃増幅器、石版3種</remarks>
        public static int BossRootDemon { get; } = 2009;
        /// <summary>
        /// チャプター5
        /// </summary>
        /// <remarks>例：北向きの針のコンパス</remarks>
        public static int Chapter5Complete { get; } = 3001;
        public static int NewCharmBond1 { get; } = 1006;
        public static int NewCharmBond2 { get; } = 14000;
        public static int NewCharmDrunk { get; } = 14001;
        public static int NewCharmSacrifice { get; } = 14002;
        public static int RewardDiceRightUp { get; } = 313;
    }
}
