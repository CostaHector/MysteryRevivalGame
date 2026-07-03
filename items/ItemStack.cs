using System.Security;

public sealed class ItemStack(ItemData item, int count = 1, int eachItemRemainingTime = 1)
{
    static public ItemStack GetABatchOfItems(ItemData item, int count = 1) {
        // 每个物品都是最大可使用次数
        return new(item, count, item.MaxRemainingTime);
    }
    public ItemData Item = item;   // 非空
    public int Count = count;      // >= 1
    public int EachItemRemainingTime = eachItemRemainingTime;
    public ItemStack Clone() => new(Item, Count, EachItemRemainingTime);
    public bool IsStackFull() => Count == Item.MaxStack;
    private bool IsSameItemStack(ItemStack rhs) => Item.Id == rhs.Item.Id && EachItemRemainingTime == rhs.EachItemRemainingTime;
    public bool CanMerge(ItemStack rhs) {
        return IsSameItemStack(rhs) && !IsStackFull();
    }
    public void Merge(ItemStack rhs) {
        if (CanMerge(rhs)) {
            Count += rhs.Count;
        }
    }
}