using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Conditions;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ObjectData;

namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

// This file contains the tile, tile entity and item
public class HolyCross : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;

        MinPick = 0;
        MineResist = 4f;
        DustType = DustID.Gold;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.UsesCustomCanPlace = true;
        //TileObjectData.newTile.StyleHorizontal = true;
        //TileObjectData.newTile.StyleWrapLimit = 15;
        TileObjectData.newTile.Width = 1;
        TileObjectData.newTile.Height = 1;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = [16];

        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 2, 0);
        TileObjectData.addTile(Type);
    }

    public override bool CanDrop(int i, int j)
    {
        return false;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];

        // If you are using ModTile.SpecialDraw or PostDraw or PreDraw, use this snippet and add zero to all calls to spriteBatch.Draw
        // The reason for this is to accommodate the shift in drawing coordinates that occurs when using the different Lighting modes
        // While at 100% world zoom, press Shift+F9 to change lighting modes quickly to verify your code works for all lighting modes
        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

        float opacitySinValue = MathF.Sin((float)(Main.timeForVisualEffects + i * 16) / 32) * 0.2f + 0.8f;
        float yPosSinValue = MathF.Sin((float)(Main.timeForVisualEffects + i * 16) / 128f) * 4;
        // Firstly we draw the original texture and then glow mask texture
        int direction = i % 2 == 0 ? 1 : -1;
        Texture2D texture = TextureAssets.Tile[Type].Value;
        Vector2 drawPos = new Vector2(i * 16 - (int)Main.screenPosition.X, 28 + j * 16 + yPosSinValue - (int)Main.screenPosition.Y) + zero;
        spriteBatch.Draw(
            texture,
            drawPos,
            null,
            Color.White * opacitySinValue,
            0f,
            new Vector2(texture.Width * 0.5f, texture.Height),
            1f,
            direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
            0f);

        Vector2 glowDrawPos = new Vector2(i, j) * 16 + new Vector2(0, 20 + -texture.Height * 0.5f + yPosSinValue) - Main.screenPosition + zero;
        for (int k = 0; k < 2; k++)
        {
            spriteBatch.Draw(
                ParacosmTextures.GlowBallTexture.Value,
                glowDrawPos,
                null,
                Color.LightYellow * opacitySinValue,
                0f,
                ParacosmTextures.GlowBallTexture.Size() * 0.5f,
                1f,
                SpriteEffects.None,
                0);
        }

        return false;
    }

    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        //spriteBatch.Draw(ParacosmTextures.GlowBallTexture.Value, new Vector2(i, j) * 16 - Main.screenPosition, null, Color.White * 1f, 0f, ParacosmTextures.GlowBallTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 10;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.9f;
        g = 0.9f;
        b = 0.6f;
    }
}

public class HolyCrossItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.DirtBlock;
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<HolyCross>());
        Item.width = 72;
        Item.height = 90;
    }

    public override void AddRecipes()
    {
        CreateRecipe(10)
            .AddIngredient(ItemID.Headstone)
            .AddTile(TileID.DemonAltar)
            .Register();
    }
}
