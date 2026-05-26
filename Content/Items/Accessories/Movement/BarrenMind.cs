using NeoParacosm.Content.Items.Materials;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Movement;

public class BarrenMind : ModItem
{
    float flightTimeBoost = 25f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(flightTimeBoost);
    public override void SetDefaults()
    {
        Item.width = 74;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 10);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.buffImmune[BuffID.Suffocation] = true;
        player.GetModPlayer<BarrenMindPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<SandSharkHead>(), 1)
            .AddIngredient(ItemType<BoneBiterHead>(), 1)
            .AddIngredient(ItemType<CrystalThresherHead>(), 1)
            .AddTile(TileID.CrystalBall)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemType<SandSharkHead>(), 1)
            .AddIngredient(ItemType<FleshReaverHead>(), 1)
            .AddIngredient(ItemType<CrystalThresherHead>(), 1)
            .AddTile(TileID.CrystalBall)
            .Register();
    }
}

public class BarrenMindPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateEquips()
    {
        if (Active)
        {
            Point playerBottomPoint = Player.Bottom.ToTileCoordinates();
            Tile? floorTileNullable = Player.GetFloorTile(playerBottomPoint.X, playerBottomPoint.Y);
            if (!floorTileNullable.HasValue)
            {
                return;
            }

            Tile floorTileValue = floorTileNullable.Value;
            bool IsSand(Tile floorTile)
            {
                return floorTile.HasTile && (floorTile.TileType == TileID.Sand
                || floorTile.TileType == TileID.Ebonsand || floorTile.TileType == TileID.Crimsand || floorTile.TileType == TileID.Pearlsand);
            }

            bool isSand = IsSand(floorTileValue);
            //Main.NewText(isSand);

            if (isSand)
            {
                float tilesPerSecond = 5f;
                float speed = tilesPerSecond / 60f;
                if (Player.HasBuff(BuffID.Flipper) || Player.accFlipper)
                {
                    speed *= 50f;
                }


                Vector2 playerCenter = Player.Center;
                if (Player.controlDownHold)
                {
                    playerCenter.Y += speed;
                }

                Tile playerTile = Main.tile[playerCenter.ToTileCoordinates()];
                if (IsSand(playerTile))
                {
                    if (Player.controlJump)
                    {
                        playerCenter.Y -= speed;
                    }

                    if (Player.controlLeft)
                    {
                        playerCenter.X -= speed;
                    }
                    else if (Player.controlRight)
                    {
                        playerCenter.X += speed;
                    }

                }
                Player.Center = playerCenter;
            }
        }
    }
}
