using System.Collections.Generic;
using ItemPublic;
using System;

// 全局物品注册表：Id → ItemData。启动时静态初始化。
public static class ItemDatabase {
    private static readonly Dictionary<ItemType, ItemData> _items = [];

    static ItemDatabase() {
        Register(new ItemData(ItemType.None, "空物品", "res://asserts/items/None_64.svg", "无物品", 1, ComsumeTool.Consume));
        Register(new ItemData(ItemType.RED_GHOST_CANDLE, "红色鬼烛", "res://asserts/items/RedGhostCandle_128.png", 
        """
        外观: 猩红，仿佛由鲜血凝聚而成的蜡烛, 就像古典婚庆时用的红色蜡烛, 点燃后, 鬼火散发出碧绿的光芒;
        作用: 点燃这根蜡烛，在烛火没有熄灭之前，只要在烛光覆盖的范围之内，人可以保证不被厉鬼杀死，处于绝对安全;
        燃烧时长: 无法确定, 遇到的情况不同，鬼烛燃烧的速度也不同，越是恐怖的鬼，鬼烛燃烧的速度就越快, 无论怎么样, 至少30min;
        """, 99, ComsumeTool.Consume));
        Register(new ItemData(ItemType.WHITE_GHOST_CANDLE, "白色鬼烛", "res://asserts/items/WhiteGhostCandle_128.png", 
        """
        外观: 就像古典丧时用的白色蜡烛, 烛光呈现诡异灰黑色
        作用: 点燃后可吸引鬼, 鬼被吸引过来之后会袭击距离白色鬼烛最近的人;
        """, 99, ComsumeTool.Consume));
        Register(new ItemData(ItemType.GOAT_SKIN_PAPER, "羊皮纸", "res://asserts/items/GoatSkinPaper_128.png", 
        """
        外观: 一叠暗褐色的纸, 褐色的皮革，柔软带着几分阴寒，仿佛从冰库里拿出来的一样，透露出几分不详和诡异的气息, 上写着一行字
        作用: 推演未来信息, 但是有一定的自主意识, 全相信的话, 会被它利用达成自己的目的;
        """, 1, ComsumeTool.Consume));
        Register(new ItemData(ItemType.GOLDEN_SA_TELLITE_PHONE, "黄金卫星电话", "res://asserts/items/GoldenSATellitePhone_128.png", 
        """
        外观: 手机, 但是是用黄金打造的;
        作用: 在灵异事件中不会被干扰, 除非信号被隔绝;
        """, 1, ComsumeTool.Consume));
        Register(new ItemData(ItemType.REGULAR_PHONE, "普通电话", "res://asserts/items/RegularPhone_128.png", 
        """
        外观: 其貌不扬的手机;
        作用: 略
        """, 1, ComsumeTool.Consume));
        Register(new ItemData(ItemType.DRY_FINGER, "干枯手指", "res://asserts/items/DryFinger_128.png", 
        """
        外观: 又长又瘦的干枯手指，漆黑的指甲狰狞无比，从这根手指的关节来看比例严重失衡;
        作用: 钉住鬼域后, 鬼域无法移动, 鬼域范围受限;
        """, 1, ComsumeTool.Consume));
        Register(new ItemData(ItemType.GHOST_BURIAL_CLOTH, "鬼寿衣", "res://asserts/items/GhostBurialCloth_128.png", 
        """
        外观: 诡异的寿衣，上面写着一个个福字，但却染着斑驳的血迹，不知道被什么东西穿过;
        作用: 穿上后可以保护其不被其他的厉鬼侵害, 副作用: 穿着寿衣越久，他的身体就越是在渐渐的变成一具尸体，一个死人;
        """, 1, ComsumeTool.Consume));
        Register(new ItemData(ItemType.COFFIN_NAIL, "棺材钉", "res://asserts/items/CoffinNail_128.png", 
        """
        外观: 成人手臂粗细的民间丧事用的棺材钉;
        作用: 钉住后, 灵异力量立刻沉寂;
        """, 1, ComsumeTool.Consume));
    }

    private static void Register(ItemData data) => _items[data.Id] = data;
    public static ItemData Get(ItemType id) => _items.TryGetValue(id, out var data) ? data : _items[ItemType.None];
}
