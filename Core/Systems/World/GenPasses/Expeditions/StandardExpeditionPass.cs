using NeoParacosm.Core.Systems.World.TerrainTypes.Caves;
using NeoParacosm.Core.Systems.World.TerrainTypes.Mountains;
using NeoParacosm.Core.Systems.World.TerrainTypes.SurfaceTerrain;
using SubworldLibrary;
using System.Linq;
using Terraria.IO;
using Terraria.WorldBuilding;
using static tModPorter.ProgressUpdate;

namespace NeoParacosm.Core.Systems.World.GenPasses.Expeditions;

public class StandardExpeditionPass : GenPass
{
    public StandardExpeditionPass(string name) : base(name, 100) { }
    static int AverageSurfaceLevel => (int)Main.worldSurface - 50;

    SurfaceTerrain initialSurface;
    SurfaceTerrain postMountainsSurface;

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        try
        {
            MoveLayersVertically(100);
            /*NeoParacosm.Instance.Logger.Info($"Rock layer: {Main.rockLayer}");
            NeoParacosm.Instance.Logger.Info($"Rock layer high: {GenVars.rockLayerHigh}");
            NeoParacosm.Instance.Logger.Info($"Rock layer low: {GenVars.rockLayerLow}");
            NeoParacosm.Instance.Logger.Info($"World surface: {Main.worldSurface}");
            NeoParacosm.Instance.Logger.Info($"World surface high: {GenVars.worldSurfaceHigh}");
            NeoParacosm.Instance.Logger.Info($"World surface low: {GenVars.worldSurfaceLow}");*/
            GenerateDirt();
            GenerateMountains();
            FixSingleHolesAndBulges();
            GenerateStone();
            GenerateDirtInStone();
            GenerateAsh();
            SurfaceTerrainGenerator.GrowGrassOnSurface(postMountainsSurface.Heights.Min());
            GenerateCaves();
            GenerateCave();
            GenerateStoneInDirt(0, (int)Main.worldSurface, 4, 15, 5, 30, 0.001f);
            PlaceSmallPilesUnderground();
            GenerateTrees();
            SlopeTiles();
            //GenerateSplotches(TileID.Sand, (int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh, 4, 10, 5, 30, 0.0025f);
            //GenerateSplotches(TileID.Mud, (int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh, 4, 10, 5, 30, 0.0025f);
            //GenerateSplotches(TileID.Pearlstone, (int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh, 4, 10, 5, 30, 0.0005f);
            //GenerateSplotches(TileID.Crimstone, (int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh, 4, 10, 5, 30, 0.0005f);
            //GenerateSplotches(TileID.Ebonstone, (int)GenVars.worldSurfaceHigh, (int)GenVars.rockLayerHigh, 4, 10, 5, 30, 0.0005f);
        }
        catch (Exception e)
        {
            SubworldSystem.Exit();
            NeoParacosm.Instance.Logger.Error(e.StackTrace);
        }
    }

    void SlopeTiles()
    {
        for (int i = 1; i < Main.maxTilesX - 1; i++)
        {
            for (int j = postMountainsSurface.Heights[i]; j < Main.maxTilesY - 1; j++)
            {
                if (!Main.rand.NextBool(4))
                {
                    continue;
                }
                Tile tile = Main.tile[i, j];
                if (!tile.HasTile)
                {
                    continue;
                }
                Tile leftTile = Main.tile[i - 1, j];
                Tile rightTile = Main.tile[i + 1, j];
                Tile belowTile = Main.tile[i, j + 1];
                Tile aboveTile = Main.tile[i, j - 1];

                if (leftTile.HasTile && rightTile.HasTile)
                {
                    continue;
                }

                if (belowTile.HasTile && aboveTile.HasTile)
                {
                    continue;
                }

                if (leftTile.HasTile)
                {
                    if (belowTile.HasTile)
                    {
                        if (Main.rand.NextBool(4))
                        {
                            WorldGen.PoundTile(i, j);
                        }
                        else
                        {

                            WorldGen.SlopeTile(i, j, (int)SlopeType.SlopeDownLeft);
                        }

                    }
                    else if (aboveTile.HasTile)
                    {

                        WorldGen.SlopeTile(i, j, (int)SlopeType.SlopeUpLeft);

                    }
                }
                else if (rightTile.HasTile)
                {
                    if (belowTile.HasTile)
                    {
                        if (Main.rand.NextBool(4))
                        {
                            WorldGen.PoundTile(i, j);
                        }
                        else
                        {

                            WorldGen.SlopeTile(i, j, (int)SlopeType.SlopeDownRight);
                        }
                    }
                    else if (aboveTile.HasTile)
                    {

                        WorldGen.SlopeTile(i, j, (int)SlopeType.SlopeUpRight);

                    }
                }
            }
        }
    }

    void PlaceSmallPilesUnderground()
    {
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            for (int j = postMountainsSurface.Heights[i] + 1; j < Main.maxTilesY; j++)
            {
                if (Main.rand.NextBool(10))
                {
                    PlaceSmallPile(i, j);
                }
            }
        }
    }

    void PlaceSmallPile(int i, int j)
    {
        if (Main.tile[i, j].HasTile || !Main.tile[i, j + 1].HasTile)
        {
            return;
        }
        Tile tileBelow = Main.tile[i, j + 1];
        int x = 0;
        int y = 0;
        switch (tileBelow.TileType)
        {
            case TileID.Dirt:
                if (Main.rand.NextBool(5))
                {
                    goto default;
                }
                x = Main.rand.Next(6, 11 + 1);
                WorldGen.PlaceSmallPile(i, j, x, y);
                break;
            case TileID.Stone:
                if (Main.rand.NextBool(5))
                {
                    goto default;
                }
                x = Main.rand.Next(0, 5 + 1);
                WorldGen.PlaceSmallPile(i, j, x, y);
                break;
            default:
                x = Main.rand.Next(12, 35 + 1);
                WorldGen.PlaceSmallPile(i, j, x, y);
                break;
        }
    }

    void MoveLayersVertically(int amount)
    {
        Main.rockLayer += amount;
        Main.worldSurface += amount;
        GenVars.worldSurface += amount;
        GenVars.worldSurfaceHigh += amount;
        GenVars.worldSurfaceLow += amount;
        GenVars.rockLayer += amount;
        GenVars.rockLayerHigh += amount;
        GenVars.rockLayerLow += amount;
    }

    void GenerateTrees()
    {
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            if (Main.rand.NextBool(10))
            {

                int y = postMountainsSurface.Heights[i];
                WorldGen.GrowEpicTree(i, y);
            }
        }
    }

    void GenerateDirt()
    {
        initialSurface = new SurfaceTerrain(0, AverageSurfaceLevel);
        int surfaceLevel = AverageSurfaceLevel;
        Random rand = new Random();
        for (int i = 0; i < 7; i++)
        {
            int minrand = rand.Next(1, 3);
            int maxrand = 1;
            if (minrand == 1)
            {
                maxrand = rand.Next(1, 3);
            }
            SurfaceTerrain surface = SurfaceTerrainGenerator.GenerateDirtSurface(
                surfaceLevel,
                0 + i * Main.maxTilesX / 7,
                Main.maxTilesX * (i + 1) / 7,
                4,
                minrand,
                maxrand
                );
            surfaceLevel = surface.Heights.Last();
            initialSurface.Heights = initialSurface.Heights.Concat(surface.Heights).ToArray();
        }

    }

    void GenerateMountains()
    {
        postMountainsSurface = new SurfaceTerrain(Main.maxTilesX, AverageSurfaceLevel);
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            postMountainsSurface.Heights[i] = initialSurface.Heights[i];
        }

        for (int c = 0; c < 4; c++)
        {
            Random rand = new Random();
            int startTileX = rand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
            int mountainWidth = rand.Next(350, 500);
            int mountainHeight = rand.Next(80, 130);
            int peakWidth = rand.Next(60, (mountainWidth * 3) / 4);
            MountainGenerator.GenerateNormalMountain(
                startTileX,
                mountainWidth,
                mountainHeight,
                peakWidth,
                initialSurface.Heights,
                postMountainsSurface.Heights
                );
        }

        for (int c = 0; c < 6; c++)
        {
            Random rand = new Random();
            int startTileX = rand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
            int mountainWidth = rand.Next(120, 180);
            int mountainHeight = rand.Next(60, 180);
            MountainGenerator.GenerateSharpMountain(
                startTileX,
                mountainWidth,
                mountainHeight,
                initialSurface.Heights,
                postMountainsSurface.Heights
                );
        }
    }

    void GenerateSplotches(int tileType, int minY, int maxY, int strengthMin, int strengthMax, int stepsMin, int stepsMax, float tileCountMultiplier)
    {
        int tileCount = (int)(Main.maxTilesX * Main.maxTilesY * tileCountMultiplier);

        for (int i = 0; i < tileCount; i++)
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next(minY, maxY);

            int strength = WorldGen.genRand.Next(strengthMin, strengthMax);
            int steps = WorldGen.genRand.Next(stepsMin, stepsMax);

            WorldGen.TileRunner(
                x,
                y,
                strength,
                steps,
                tileType
            );
        }
    }

    void GenerateStoneInDirt(int minY, int maxY, int strengthMin, int strengthMax, int stepsMin, int stepsMax, float stoneCountMultiplier)
    {
        GenerateSplotches(TileID.Stone, minY, maxY, strengthMin, strengthMax, stepsMin, stepsMax, stoneCountMultiplier);
        int surfaceRockCount = (int)(Main.maxTilesX * Main.maxTilesY * stoneCountMultiplier);
    }

    void FixSingleHolesAndBulges()
    {
        // Slightly smooth out terrain
        for (int i = 1; i < Main.maxTilesX - 1; i++)
        {
            int current = postMountainsSurface.Heights[i];
            int prev = postMountainsSurface.Heights[i - 1];
            int next = postMountainsSurface.Heights[i + 1];
            if (prev > current && next > current)
            {
                WorldGen.KillTile(i, current);
                current++;
                postMountainsSurface.Heights[i]++;
                //WorldGen.PlaceTile(i, current, TileID.Grass, true);
            }
            else if (prev < current && next < current)
            {
                current--;
                postMountainsSurface.Heights[i]--;
                WorldGen.PlaceTile(i, current, TileID.Dirt, true);
            }
        }
    }

    void GenerateDirtInStone()
    {
        float count = (Main.maxTilesX * Main.maxTilesY) * 0.005f;
        for (int i = 0; i < count; i++)
        {
            WorldGen.TileRunner(
                WorldGen.genRand.Next(0, Main.maxTilesX),
                WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY),
                WorldGen.genRand.Next(2, 6),
                WorldGen.genRand.Next(2, 40),
                TileID.Dirt);
        }
    }

    void GenerateCave()
    {
        Point p1 = new Point(0, (int)Main.worldSurface);
        Point p2 = new Point(Main.maxTilesX, (int)Main.worldSurface);
        CaveGenerator.GenerateCaveBetweenPoints(p1.X, p1.Y, p2.X, p2.Y, (i) => Main.rand.Next(3, 6));
    }

    void GenerateCaves()
    {
        int xCaveCount = 6;
        for (int cx = 0; cx < xCaveCount; cx++)
        {
            int x = Main.rand.Next(cx * Main.maxTilesX / xCaveCount, (cx + 1) * Main.maxTilesX / xCaveCount);

            int yCaveCount = 8;
            for (int cy = 0; cy < yCaveCount; cy++)
            {
                int factor = Main.maxTilesY - postMountainsSurface.Heights[x];
                int y = Main.rand.Next(postMountainsSurface.Heights[x] + cy * factor / yCaveCount, postMountainsSurface.Heights[x] + (cy + 1) * factor / yCaveCount);

                int baseWidth = Main.rand.Next(4, 8);
                int length = Main.rand.Next(80, 120);
                CaveGenerator.GenerateCave(
                    x, y,
                       new Vector2(Main.rand.Next(-1, 1 + 1), Main.rand.Next(-1, 1 + 1)),
                    length,
                    (i) => baseWidth + Main.rand.Next(-3, 3),
                    (i) => CaveGenerator.LinearCaveAngleFuncWithRandomSharpTurn(i, length, MathHelper.Pi / 4f, MathHelper.Pi / 2f, 12));
            }

        }

        /*for (int c = 0; c < 40; c++)
        {
            int x = Main.rand.Next(0, Main.maxTilesX);

            int y = Main.rand.Next(initialSurface.Heights[x], Main.maxTilesY);
            int baseWidth = Main.rand.Next(4, 6);
            int length = Main.rand.Next(80, 150);
            CaveGenerator.GenerateCave(
                x, y,
                new Vector2(Main.rand.Next(-1, 1 + 1), Main.rand.Next(-1, 1 + 1)),
                length,
                (i) => baseWidth + Main.rand.Next(-3, 3),
                (i) => CaveGenerator.LinearCaveAngleFuncWithRandomSharpTurn(i, length, MathHelper.Pi / 4f, MathHelper.Pi / 2f, 12));

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
            int surfaceY = initialSurface.Heights[i];
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
                if (j > Main.UnderworldLayer - 100)
                {
                    int chance = Main.UnderworldLayer - j;
                    if (Main.rand.NextBool(chance / 10 + 1))
                    {
                        WorldGen.PlaceTile(i, j, TileID.Ash, true);
                        continue;
                    }
                }
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
