using System;
using System.ComponentModel;
using System.Security;
using System.Threading.Tasks.Dataflow;
using Godot;
using ItemPublic;

// 物品的静态定义（模板），不含数量。同一 Id 全局共享一个实例。
public sealed class ItemData(ItemType id, string name, string iconPath, string description, int maxStack, ComsumeTool.ConsumeDelegate pConsume, int  maxRemainingTime = 1)
{
    public readonly ItemType Id = id;
    public readonly string Name = name;                    // 显示名，如 "红鬼烛"
    public readonly string IconPath = iconPath;            // 图标资源路径 res://asserts/xxx.png
    public readonly string Description = description;      // tooltip 文案（需求 5 暂统一为此）
    public readonly int MaxStack = maxStack;               // 最大堆叠数

    public readonly ComsumeTool.ConsumeDelegate ConsumeFunc = pConsume;
    public readonly int MaxRemainingTime = maxRemainingTime;              // 最大可使用次数

    public readonly string UseEffectId = String.Empty;     // 决策 6 预留：使用效果标识；null=本期仅扣数量、无额外效果

    // 延迟加载图标，避免重复 GD.Load
    private Texture2D _icon = null;
    public Texture2D GetIcon() => _icon ??= GD.Load<Texture2D>(IconPath);
}