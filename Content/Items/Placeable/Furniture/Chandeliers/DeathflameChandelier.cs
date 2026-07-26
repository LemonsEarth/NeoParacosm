using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.Localization;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Furniture.Chandeliers;

public class DeathflameChandelierBlock : ModTile
{
    private Asset<Texture2D> flameTexture;

    public override void Load()
    {
        flameTexture = Request<Texture2D>(Texture + "_Flames");
    }

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileLighted[Type] = true;
        // We don't set Main.tileFlame
        TileID.Sets.IsAMechanism[Type] = true;
        TileID.Sets.MultiTileSway[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.Origin = new Point16(1, 0);
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
        TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        TileObjectData.newTile.LavaDeath = true;
        // Rather than many different items, the single item placing this tile places a random style.
        TileObjectData.newTile.DrawYOffset = -2;
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(116, 116, 116), Language.GetText("MapObject.Chandelier"));

        // Since we are using RandomStyleRange without StyleMultiplier, we'll need to manually register the item drop for the tile styles other than style 0. Here we register the default drop for any style.
        RegisterItemDrop(ItemType<DeathflameChandelier>());
    }

    public override void HitWire(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        int topX = i - tile.TileFrameX % 54 / 18;
        int topY = j - tile.TileFrameY % 54 / 18;

        short frameAdjustment = (short)(tile.TileFrameY >= 54 ? -54 : 54);

        for (int x = topX; x < topX + 3; x++)
        {
            for (int y = topY; y < topY + 3; y++)
            {
                Main.tile[x, y].TileFrameY += frameAdjustment;
                Wiring.SkipWire(x, y);
            }
        }

        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            NetMessage.SendTileSquare(-1, topX, topY, 3, 3);
        }
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        //Main.NewText(Main.tile[i, j].TileFrameY);
        if (Main.tile[i, j].TileFrameY / 54 != 0)
        {
            return;
        }

        r = 0.5f;
        g = 0.5f;
        b = 0.5f;
    }

    public override void EmitParticles(int i, int j, Tile tileCache, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (Main.rand.NextBool(40) && tileFrameY < 54)
        {
            // The following math makes dust only spawn at the tile coordinates of the flames:
            // ---
            // O-O
            // ---

            int tileColumn = tileFrameX / 18 % 3;
            if (tileFrameY / 18 % 3 == 1 && tileColumn != 1)
            {
                int dustChoice = DustID.GemDiamond;

                if (dustChoice != -1)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(i * 16, j * 16 + 2), 14, 6, dustChoice, 0f, 0f, 100);

                    dust.noGravity = true;
                    dust.velocity *= 0.3f;
                    dust.velocity.Y -= 1.5f;
                }
            }
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];

        if (TileObjectData.IsTopLeft(tile))
        {
            // Makes this tile sway in the wind and with player interaction when used with TileID.Sets.MultiTileSway
            Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.MultiTileVine);
        }

        // We must return false here to prevent the normal tile drawing code from drawing the default static tile. Without this a duplicate tile will be drawn.
        return false;
    }

    public override void GetTileFlameData(int i, int j, ref TileDrawing.TileFlameData tileFlameData)
    {
        ulong flameSeed = Main.TileFrameSeed ^ (ulong)(((long)i << 32) | (uint)j);

        tileFlameData.flameTexture = flameTexture.Value;
        tileFlameData.flameSeed = flameSeed;

        tileFlameData.flameCount = 7;
        tileFlameData.flameColor = new Color(180, 180, 180, 55);
        tileFlameData.flameRangeXMin = -10;
        tileFlameData.flameRangeXMax = 11;
        tileFlameData.flameRangeYMin = -10;
        tileFlameData.flameRangeYMax = 1;
        tileFlameData.flameRangeMultX = 0.15f;
        tileFlameData.flameRangeMultY = 0.35f;
    }

    public override void AdjustMultiTileVineParameters(int i, int j, ref float? overrideWindCycle, ref float windPushPowerX, ref float windPushPowerY, ref bool dontRotateTopTiles, ref float totalWindMultiplier, ref Texture2D glowTexture, ref Color glowColor)
    {
        // Vanilla chandeliers all share these parameters.
        overrideWindCycle = 1f;
        windPushPowerY = 0;
        overrideWindCycle = 0f;
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
       
    }
}

public class DeathflameChandelier : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<DeathflameChandelierBlock>());
        Item.value = Item.sellPrice(copper: 50);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<PrisonBrickItem>(), 10)
            .AddIngredient(ItemID.Torch, 3)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
