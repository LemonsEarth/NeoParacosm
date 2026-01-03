namespace NeoParacosm.Core.Globals.GlobalItems;

public class GemItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return entity.type == ItemID.Topaz
            || entity.type == ItemID.Amethyst
            || entity.type == ItemID.Sapphire
            || entity.type == ItemID.Emerald
            || entity.type == ItemID.Ruby
            || entity.type == ItemID.Amber
            || entity.type == ItemID.Diamond;
    }

    int tileID = -1;
    public override bool? UseItem(Item item, Player player)
    {
        if (player.NPArmorPlayer().StoneArmor)
        {
            return true;
        }
        return null;
    }

    public override bool CanUseItem(Item item, Player player)
    {
        return base.CanUseItem(item, player);
    }

    public override void OnConsumeItem(Item item, Player player)
    {
        if (!player.NPArmorPlayer().StoneArmor)
        {
            return;
        }

        switch (item.type)
        {
            case ItemID.Topaz:
                player.AddBuff(BuffID.Spelunker, 20 * 60);
                break;
            case ItemID.Amethyst:
                player.AddBuff(BuffID.WeaponImbueVenom, 30 * 60);
                break;
            case ItemID.Sapphire:
                player.AddBuff(BuffID.Gills, 30 * 60);
                player.AddBuff(BuffID.WaterWalking, 30 * 60);
                break;
            case ItemID.Emerald:
                player.AddBuff(BuffID.Mining, 45 * 60);
                player.AddBuff(BuffID.Lucky, 45 * 60);
                break;
            case ItemID.Ruby:
                player.AddBuff(BuffID.Rage, 30 * 60);
                player.AddBuff(BuffID.Wrath, 30 * 60);
                break;
            case ItemID.Amber:
                player.AddBuff(BuffID.Lifeforce, 60 * 60);
                player.AddBuff(BuffID.Regeneration, 60 * 60);
                break;
            case ItemID.Diamond:
                player.AddBuff(BuffID.Endurance, 60 * 60);
                player.AddBuff(BuffID.ObsidianSkin, 60 * 60);
                break;
        }
    }

    public override void UpdateInventory(Item item, Player player)
    {
        if (player.NPArmorPlayer().StoneArmor)
        {
            if (tileID == -1)
            {
                tileID = item.createTile;
            }
            item.createTile = -1;
            item.useAnimation = item.useTime;
            item.useStyle = ItemUseStyleID.EatFood;
            item.UseSound = SoundID.DD2_CrystalCartImpact;
            //player.ConsumeItem(item.type);
        }
        else
        {
            if (tileID != -1)
            {
                item.createTile = tileID;
            }
            item.useStyle = ItemUseStyleID.Swing;
            item.UseSound = SoundID.Item1;
        }

    }
}
