using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class TwilightStarMagnet : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();
    public override void SetDefaults()
    {
        Item.width = 54;
        Item.height = 50;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<TwilightStarMagnetPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe1 = CreateRecipe();
        recipe1.AddIngredient(ItemType<StarMagnet>());
        recipe1.AddIngredient(ItemID.CelestialMagnet);
        recipe1.AddTile(TileID.TinkerersWorkbench);
        recipe1.Register();
    }
}

public class TwilightStarMagnetPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }
}

public class TwilightStarMagnetGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public bool IsNaturalStar { get; private set; } = false;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return entity.type == ItemID.FallenStar || entity.type == ItemID.Star;
    }
    public override void GrabRange(Item item, Player player, ref int grabRange)
    {
        if (player.GetModPlayer<TwilightStarMagnetPlayer>().Active)
        {
            grabRange += 24 * 16;
        }
    }

    public override void OnSpawn(Item item, IEntitySource source)
    {
        if (source is EntitySource_DropAsItem src && src.Entity is Projectile proj && proj.type == ProjectileID.FallingStar)
        {
            IsNaturalStar = true;
        }
    }

    public override bool OnPickup(Item item, Player player)
    {
        if (item.type == ItemID.FallenStar && player.GetModPlayer<TwilightStarMagnetPlayer>().Active && IsNaturalStar)
        {
            player.AddBuff(BuffType<StarSpeedBuff>(), 10 * 60);
            IsNaturalStar = false;
        }
        return true;
    }
}

public class StarSpeedBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.moveSpeed += 1f;
    }
}
