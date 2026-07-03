namespace ItemPublic
{
    public enum ItemType : uint
    {
        None = 0,
        RED_GHOST_CANDLE, // 红鬼烛
        WHITE_GHOST_CANDLE, // 白鬼烛
        GOAT_SKIN_PAPER, // 羊皮纸
        GOLDEN_SA_TELLITE_PHONE, // 黄金卫星电话
        REGULAR_PHONE, // 普通电话
        DRY_FINGER, // 干枯手指
        GHOST_BURIAL_CLOTH, // 鬼寿衣
        COFFIN_NAIL, // 棺材钉
        RUSTY_FIREWOOD_CHOPPER, // 生锈的柴刀
        RUSTY_SCISSORS, // 生锈的鬼剪刀
        GHOST_ROPE, // 鬼绳
        GOLD_PISTOL, // 黄金枪
        GOLD_BULLET, // 黄金枪弹
        GOLDWOVEN_POUCH, // 黄金编织袋
        REGULAR_PISTOL, // 普通枪
        REGULAR_BULLET, // 普通枪弹
        LETTER_PAPER_IN_POST_OFFICE, // 邮局信纸
        MUSIC_BOX, // 八音盒
        BURIAL_SHROUD, // 裹尸布
        FISHING_ROD, // 钓鱼竿
        GHOST_PAPER_MONEY_3, // 3元鬼钱币
        GHOST_PAPER_MONEY_4, // 4元鬼钱币
        GHOST_PAPER_MONEY_7, // 7元鬼钱币
        YELLOW_JOSS_PAPER, // 黄纸
        GHOST_COIN, // 鬼硬币
        DEATH_SUBSTITUTE_DO, // 替死娃娃
        RED_LATTERN, // 红灯笼
        WHITE_LATTERN, // 白灯笼
        GHOST_NEWSPAPER, // 鬼报纸
        GHOST_NEWSPAPER_SCRAP, // 鬼报纸碎片
    }

    public struct ComsumeTool
    {
        // 剩余次数, 消耗点数
        public delegate bool ConsumeDelegate(ref int remainingTimes, int consumePoint);

        public static bool Consume(ref int remainingTimes, int consumePoint = 1)
        {
            if (remainingTimes <= consumePoint)
            {
                remainingTimes = 0;
                return false; // 用掉
            }
            remainingTimes -= consumePoint;
            return true; // 还有剩余
        }

    };

}