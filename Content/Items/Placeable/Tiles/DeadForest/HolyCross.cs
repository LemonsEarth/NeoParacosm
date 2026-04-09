using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
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
        TileObjectData.newTile.Width = 4;
        TileObjectData.newTile.Height = 5;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];

        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.Origin = new Point16(0, 5);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 2, 0);
        TileObjectData.addTile(Type);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];

        // If you are using ModTile.SpecialDraw or PostDraw or PreDraw, use this snippet and add zero to all calls to spriteBatch.Draw
        // The reason for this is to accommodate the shift in drawing coordinates that occurs when using the different Lighting modes
        // While at 100% world zoom, press Shift+F9 to change lighting modes quickly to verify your code works for all lighting modes
        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

        // Because height of third tile is different we change it
        int height = 16;

        float opacitySinValue = MathF.Sin((float)(Main.timeForVisualEffects + i * 16) / 16) * 0.2f + 0.8f;
        float yPosSinValue = MathF.Sin((float)(Main.timeForVisualEffects + i * 16) / 128f) * 4;
        // Firstly we draw the original texture and then glow mask texture
        spriteBatch.Draw(
            TextureAssets.Tile[Type].Value,
            new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 + yPosSinValue - (int)Main.screenPosition.Y) + zero,
            new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height),
            Color.White * opacitySinValue, 0f, default, 1f, SpriteEffects.None, 0f);
        if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
        {
            for (int k = 0; k < 2; k++)
            {

                spriteBatch.Draw(
                    ParacosmTextures.GlowBallTexture.Value,
                    new Vector2(i, j) * 16 + new Vector2(32, 36 + yPosSinValue) - Main.screenPosition + zero,
                    null,
                    Color.LightYellow * opacitySinValue,
                    0f,
                    ParacosmTextures.GlowBallTexture.Size() * 0.5f,
                    1f,
                    SpriteEffects.None,
                    0);
            }
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
        num = fail ? 3 : 1;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.9f;
        g = 0.9f;
        b = 0.6f;
    }
}
