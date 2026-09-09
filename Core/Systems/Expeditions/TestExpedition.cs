using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.World.GenPasses;
using NeoParacosm.Core.Systems.World.GenPasses.Expeditions;
using ReLogic.Content;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.Expeditions;

public class TestExpedition : Subworld
{
    public override int Width => 1600;
    public override int Height => 1000;

    public override bool ShouldSave => false;

    public override bool NoPlayerSaving => true;

    public override List<GenPass> Tasks => new List<GenPass>()
    {
        new StandardExpeditionPass("StandardExpeditionPass")
    };

    public override void Load()
    {

    }

    public override void Unload()
    {

    }

    public override void OnLoad()
    {

    }

    public override void OnUnload()
    {

    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    void GenerateCavePoints(int startTileX, int startTileY, Vector2 initialDirection, int caveWidth, int length, float randomAngleMax)
    {
        Point[] cavePoints = new Point[length];
        Vector2 currentDirection = initialDirection;
        Point currentTile = new Point(startTileX, startTileY);
        for (int i = 0; i < length; i++)
        {
            cavePoints[i] = currentTile;
            currentDirection = currentDirection.RotatedBy(Main.rand.NextFloat(-randomAngleMax, randomAngleMax));
            if (Main.rand.NextBool(10))
            {
                currentDirection = currentDirection.RotatedBy(MathHelper.PiOver2);
            }
            currentTile += (currentDirection * caveWidth).ToPoint();
        }
        foreach (var point in cavePoints)
        {
            //WorldGen.PlaceTile(point.X, point.Y, TileID.DiamondGemspark, forced: true);
            WorldGen.TileRunner(point.X, point.Y, caveWidth * 2, 20, -1);
        }
    }

    public override void Update()
    {
        Main.NewText($"Player Coords: {Main.LocalPlayer.Center.ToTileCoordinates()}");
        //Main.NewText($"World Surface: {Main.worldSurface}");

        /*if (Main.mouseLeft && Main.mouseLeftRelease && Main.LocalPlayer.HeldItem.type == ItemID.None)
        {
            GenerateCavePoints(LemonUtils.GetMouseTile().X, LemonUtils.GetMouseTile().Y, new Vector2(1, 0), 10, 100, MathHelper.PiOver4);
        }*/
    }

    public override void DrawMenu(GameTime gameTime)
    {

    }
}
