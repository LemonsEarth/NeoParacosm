using NeoParacosm.Core.Systems.World.GenPasses.Expeditions;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.Expeditions;

public class TestExpedition : BaseExpedition
{
    public override int Width => 1600;
    public override int Height => 1000;

    public override List<GenPass> Tasks => new List<GenPass>()
    {
        new StandardExpeditionPass("StandardExpeditionPass"),

    };

    public override void Update()
    {
        base.Update();
    }

    public override void DrawMenu(GameTime gameTime)
    {

    }
}
