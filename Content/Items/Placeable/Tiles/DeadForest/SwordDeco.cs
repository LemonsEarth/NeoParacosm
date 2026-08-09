using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class SwordDeco : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = false;
        MinPick = 0;
        MineResist = 4f;
        DustType = DustID.Iron;

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
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform, 1, 0);
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

        // Firstly we draw the original texture and then glow mask texture
        int frameY = (i + j) % 6;
        UnifiedRandom random = new UnifiedRandom(i * 10000 + j);
        float rotation = MathHelper.ToRadians(random.NextFloat(-30, 30));
        Texture2D texture = TextureAssets.Tile[Type].Value;
        Rectangle frame = texture.Frame(1, 6, 0, frameY);
        Vector2 origin = new Vector2(11, 58);
        Vector2 drawPos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 32) + zero;
        spriteBatch.Draw(
            texture,
            drawPos,
            frame,
            Color.White,
            rotation,
            origin,
            1f,
            SpriteEffects.None,
            0f);

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
        // r = 0.9f;
        // g = 0.9f;
        // b = 0.6f;
    }
}

public class SwordDecoItem : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.DirtBlock;
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileType<SwordDeco>());
        Item.width = 22;
        Item.height = 64;
    }
}
