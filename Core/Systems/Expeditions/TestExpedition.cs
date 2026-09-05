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

    public override void Update()
    {
        Main.NewText($"Player Coords: {Main.LocalPlayer.Center.ToTileCoordinates()}");
        //Main.NewText($"World Surface: {Main.worldSurface}");
    }

    public override void DrawMenu(GameTime gameTime)
    {

    }
}
