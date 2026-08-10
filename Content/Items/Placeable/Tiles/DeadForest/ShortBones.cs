namespace NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;

public class ShortBones : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileCut[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.ReplaceTileBreakUp[Type] = true;
        TileID.Sets.SwaysInWindBasic[Type] = true;
        TileID.Sets.IgnoredByGrowingSaplings[Type] = true;

        DustType = DustID.Ash;

        HitSound = SoundID.NPCHit2 with { PitchRange = (0.6f, 0.8f) };

        AddMapEntry(new Color(120, 120, 120));
    }

    public override void RandomUpdate(int i, int j)
    {

    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 3 : 1;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        Tile tileBelow = Framing.GetTileSafely(i, j + 1);

        if (tileBelow.HasTile && tileBelow.TileType == TileType<DeadDirtBlock>())
        {
            return true;
        }

        WorldGen.KillTile(i, j);

        return true;
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        offsetY = 4;
    }
}
