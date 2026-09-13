using NeoParacosm.Core.Systems.World.TerrainTypes.Caves;
using NeoParacosm.Core.Systems.World.TerrainTypes.Mountains;
using NeoParacosm.Core.Systems.World.TerrainTypes.SurfaceTerrain;
using SubworldLibrary;
using System.Linq;
using Terraria.IO;
using Terraria.WorldBuilding;

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
            GenerateAsh();
            SurfaceTerrainGenerator.GrowGrassOnSurface(postMountainsSurface.SurfaceHeights.Min());
            GenerateCaves();
            GenerateCave();
        }
        catch (Exception e)
        {
            SubworldSystem.Exit();
            NeoParacosm.Instance.Logger.Error(e.StackTrace);
        }
    }

    void GenerateDirt()
    {
        int currentY = AverageSurfaceLevel;
        initialSurface = new SurfaceTerrain(Main.maxTilesX);
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            //PlaceGrass(i, currentY);
            for (int j = currentY; j < Main.worldSurface; j++)
            {
                WorldGen.PlaceTile(i, j, TileID.Dirt, true);
            }
            initialSurface.SurfaceHeights[i] = currentY;
            if (Main.rand.NextBool(4))
            {
                currentY += Main.rand.Next(-1, 1 + 1);
            }
        }
    }

    void GenerateMountains()
    {
        postMountainsSurface = new SurfaceTerrain(Main.maxTilesX);
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            postMountainsSurface.SurfaceHeights[i] = initialSurface.SurfaceHeights[i];
        }

        for (int c = 0; c < 4; c++)
        {
            Random rand = new Random();
            int startTileX = rand.Next((int)(Main.maxTilesX * 0.1f), (int)(Main.maxTilesX * 0.9f));
            int mountainWidth = rand.Next(350, 500);
            int mountainHeight = rand.Next(180, 240);
            int peakWidth = rand.Next(60, (mountainWidth * 3) / 4);
            MountainGenerator.GenerateNormalMountain(
                startTileX,
                mountainWidth,
                mountainHeight,
                peakWidth,
                initialSurface.SurfaceHeights,
                postMountainsSurface.SurfaceHeights
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
                initialSurface.SurfaceHeights,
                postMountainsSurface.SurfaceHeights
                );
        }
    }

    void FixSingleHolesAndBulges()
    {
        // Slightly smooth out terrain
        for (int i = 1; i < Main.maxTilesX - 1; i++)
        {
            int current = postMountainsSurface.SurfaceHeights[i];
            int prev = postMountainsSurface.SurfaceHeights[i - 1];
            int next = postMountainsSurface.SurfaceHeights[i + 1];
            if (prev > current && next > current)
            {
                WorldGen.KillTile(i, current);
                current++;
                postMountainsSurface.SurfaceHeights[i]++;
                //WorldGen.PlaceTile(i, current, TileID.Grass, true);
            }
            else if (prev < current && next < current)
            {
                current--;
                postMountainsSurface.SurfaceHeights[i]--;
                WorldGen.PlaceTile(i, current, TileID.Dirt, true);
            }
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
        for (int c = 0; c < 10; c++)
        {
            int x = Main.rand.Next(0, Main.maxTilesX);

            int y = postMountainsSurface.SurfaceHeights[x];
            int baseWidth = Main.rand.Next(4, 8);
            int length = Main.rand.Next(80, 120);
            CaveGenerator.GenerateCave(
                x, y,
                Vector2.UnitY,
                length,
                (i) => baseWidth + Main.rand.Next(-2, 3),
                (i) => CaveGenerator.LinearCaveAngleFuncWithRandomSharpTurn(i, length, MathHelper.Pi / 4f, MathHelper.Pi / 3f, 20));

        }

        for (int c = 0; c < 40; c++)
        {
            int x = Main.rand.Next(0, Main.maxTilesX);

            int y = Main.rand.Next(initialSurface.SurfaceHeights[x], Main.maxTilesY);
            int baseWidth = Main.rand.Next(4, 6);
            int length = Main.rand.Next(80, 150);
            CaveGenerator.GenerateCave(
                x, y,
                new Vector2(Main.rand.Next(-1, 1 + 1), Main.rand.Next(-1, 1 + 1)),
                length,
                (i) => baseWidth + Main.rand.Next(-3, 3),
                (i) => CaveGenerator.LinearCaveAngleFuncWithRandomSharpTurn(i, length, MathHelper.Pi / 4f, MathHelper.Pi / 2f, 12));

        }
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
            int surfaceY = initialSurface.SurfaceHeights[i];
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
