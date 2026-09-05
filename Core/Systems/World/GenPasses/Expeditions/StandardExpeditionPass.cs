using NeoParacosm.Content.Items.Placeable.Tiles.DeadForest;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.World.GenPasses.Expeditions;

public class StandardExpeditionPass : GenPass
{
    public StandardExpeditionPass(string name) : base(name, 100) { }
    static int AverageSurfaceLevel => (int)Main.worldSurface - 50;
    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        /*NeoParacosm.Instance.Logger.Info($"Rock layer: {Main.rockLayer}");
        NeoParacosm.Instance.Logger.Info($"Rock layer high: {GenVars.rockLayerHigh}");
        NeoParacosm.Instance.Logger.Info($"Rock layer low: {GenVars.rockLayerLow}");
        NeoParacosm.Instance.Logger.Info($"World surface: {Main.worldSurface}");
        NeoParacosm.Instance.Logger.Info($"World surface high: {GenVars.worldSurfaceHigh}");
        NeoParacosm.Instance.Logger.Info($"World surface low: {GenVars.worldSurfaceLow}");*/
        GenerateDirt();

        GenerateMountains();
        FixSingleHolesAndBulges();

        GrowGrassOnSurface();


        GenerateStone();
        GenerateAsh();
    }

    void PlaceGrass(int i, int j)
    {
        WorldGen.PlaceTile(i, j, TileID.Dirt, true);
        WorldGen.PlaceTile(i, j, TileID.Grass, true);
    }

    int[] surfaceHeights = new int[Main.maxTilesX];
    void GenerateDirt()
    {
        int currentY = AverageSurfaceLevel;
        surfaceHeights = new int[Main.maxTilesX];
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            //PlaceGrass(i, currentY);
            for (int j = currentY; j < Main.worldSurface; j++)
            {
                WorldGen.PlaceTile(i, j, TileID.Dirt, true);
            }
            surfaceHeights[i] = currentY;
            if (Main.rand.NextBool(4))
            {
                currentY += Main.rand.Next(-1, 1 + 1);
            }
        }
    }

    int[] postMountainsSurfaceHeights = new int[Main.maxTilesX];
    void GenerateMountains()
    {
        postMountainsSurfaceHeights = new int[Main.maxTilesX];
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            postMountainsSurfaceHeights[i] = surfaceHeights[i];
        }

        for (int c = 0; c < 5; c++)
        {
            Random rand = new Random();
            int startTileX = rand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
            int mountainWidth = rand.Next(120, 180);
            int mountainHeight = rand.Next(60, 90);
            int peakTileX = startTileX + (mountainWidth / 2);
            int endTileX = startTileX + mountainWidth;
            int currentMountainHeight = 0;
            int peakWidth = rand.Next(20, (mountainWidth * 3) / 4);
            int leftPeakTileX = peakTileX - peakWidth / 2;
            int rightPeakTileX = peakTileX + peakWidth / 2;

            for (int i = startTileX; i < rightPeakTileX; i++)
            {
                int surfaceY = surfaceHeights[i];
                int y = surfaceY + currentMountainHeight;
                if (!WorldGen.InWorld(i, y))
                {
                    continue;
                }
                if (y < postMountainsSurfaceHeights[i])
                {
                    postMountainsSurfaceHeights[i] = y;
                }
                for (int j = y; j < surfaceY; j++)
                {
                    WorldGen.PlaceTile(i, j, TileID.Dirt, true);
                }

                float heightPercent = MathHelper.Clamp(MathF.Abs(currentMountainHeight) / mountainHeight, 0, 1);
                float heightMul = (1 - heightPercent) + 0.5f;
                if (i < leftPeakTileX)
                {
                    currentMountainHeight -= rand.Next((int)(1 * heightMul), (int)(4 * heightMul));
                }
                else
                {
                    if (rand.Next(4) == 0)
                    {
                        currentMountainHeight += rand.Next(-1, 1 + 1);
                    }
                }
            }
            int fallOffStart = rightPeakTileX;
            int fallOffTileX = fallOffStart;
            while (currentMountainHeight < 0)
            {
                int surfaceY = surfaceHeights[fallOffTileX];
                int y = surfaceY + currentMountainHeight;
                if (!WorldGen.InWorld(fallOffTileX, y))
                {
                    continue;
                }
                if (y < postMountainsSurfaceHeights[fallOffTileX])
                {
                    postMountainsSurfaceHeights[fallOffTileX] = y;
                }
                for (int j = y; j < surfaceY; j++)
                {
                    WorldGen.PlaceTile(fallOffTileX, j, TileID.Dirt, true);
                }

                float heightPercent = MathHelper.Clamp(MathF.Abs(currentMountainHeight) / mountainHeight, 0, 1);
                float heightMul = (1 - heightPercent) + 0.5f;
                currentMountainHeight += rand.Next((int)(1 * heightMul), (int)(4 * heightMul));
                fallOffTileX++;
            }
            /*WorldGen.PlaceTile(startTileX, surfaceHeights[startTileX], TileID.SapphireGemspark, forced: true);
            WorldGen.PlaceTile(endTileX, surfaceHeights[endTileX], TileID.RubyGemspark, forced: true);
            WorldGen.PlaceTile(peakTileX, surfaceHeights[peakTileX], TileID.EmeraldGemspark, forced: true);
            WorldGen.PlaceTile(leftPeakTileX, surfaceHeights[leftPeakTileX], TileID.EmeraldGemspark, forced: true);
            WorldGen.PlaceTile(rightPeakTileX, surfaceHeights[rightPeakTileX], TileID.EmeraldGemspark, forced: true);*/
        }
    }

    void FixSingleHolesAndBulges()
    {
        // Slightly smooth out terrain
        for (int i = 1; i < Main.maxTilesX - 1; i++)
        {
            int current = postMountainsSurfaceHeights[i];
            int prev = postMountainsSurfaceHeights[i - 1];
            int next = postMountainsSurfaceHeights[i + 1];
            if (prev > current && next > current)
            {
                WorldGen.KillTile(i, current);
                current++;
                postMountainsSurfaceHeights[i]++;
                //WorldGen.PlaceTile(i, current, TileID.Grass, true);
            }
            else if (prev < current && next < current)
            {
                current--;
                postMountainsSurfaceHeights[i]--;
                WorldGen.PlaceTile(i, current, TileID.Dirt, true);
            }
        }
    }

    void GrowGrassOnSurface()
    {
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            WorldGen.PlaceTile(i, postMountainsSurfaceHeights[i], TileID.Grass);
        }
        /*for (int i = 0; i < Main.maxTilesX; i++)
        {
            for (int j = 0; j < surfaceHeights[i]; j++)
            {
                if (Main.tile[i, j].HasTile) break;
                if (Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType == TileID.Dirt)
                {
                    WorldGen.PlaceTile(i, j + 1, TileID.Grass);
                    break;
                }
            }
        }*/
    }

    void GenerateStoneSpikyMountains()
    {
        int startTileX = Main.rand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
        int mountainWidth = Main.rand.Next(100, 200);
        int mountainHeight = Main.rand.Next(100, 150);
        int peakTileX = startTileX + (mountainWidth / 2) + Main.rand.Next(-mountainWidth / 4, mountainWidth / 4);

        int currentMountainHeight = 0;
        for (int i = startTileX; i < startTileX + mountainWidth; i++)
        {
            if (i >= Main.maxTilesX)
            {
                break;
            }
            int surfaceY = surfaceHeights[i];
            int y = surfaceY + currentMountainHeight;
            WorldGen.PlaceTile(i, y, TileID.Stone, true);
            for (int j = y; j < surfaceY; j++)
            {
                WorldGen.PlaceTile(i, j, TileID.Stone, true);
            }
            if (i < peakTileX - 10)
            {
                currentMountainHeight -= Main.rand.Next(-2, 4);
            }
            else if (i > peakTileX - 10)
            {
                currentMountainHeight += Main.rand.Next(-2, 4);
            }
        }
    }

    void GenerateStone()
    {
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            for (int j = (int)Main.worldSurface; j < Main.UnderworldLayer; j++)
            {
                WorldGen.PlaceTile(i, j, TileID.Stone, true);
            }
        }
    }

    void GenerateAsh()
    {
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            for (int j = Main.UnderworldLayer; j < Main.maxTilesY; j++)
            {
                WorldGen.PlaceTile(i, j, TileID.Ash, true);
            }
        }
        Main.spawnTileX = 1600 / 2;
        Main.spawnTileY = 1000 / 10;
    }
}
