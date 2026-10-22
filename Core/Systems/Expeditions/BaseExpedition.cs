using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Core.Systems.World.GenPasses.Expeditions;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Systems.Expeditions;

public class BaseExpedition : Subworld
{
    public int ExpeditionTimer { get; private set; } = 0;

    public override int Width => 1600;
    public override int Height => 1000;

    public sealed override bool ShouldSave => false;

    public sealed override bool NoPlayerSaving => true;

    public override List<GenPass> Tasks => new List<GenPass>()
    {

    };

    public void OnBeginExpedition()
    {
        Main.LocalPlayer.Center = new Vector2(Main.maxTilesX * 16 / 5, 100);
        Main.LocalPlayer.AddBuff(BuffType<NoFallDamageBuff>(), 20 * 60);
        
    }

    public void UpdateExpedition()
    {
        if (ExpeditionTimer == 0)
        {

        }
        ExpeditionTimer++;
    }

    public override void Update()
    {
        UpdateExpedition();
    }

    public override void OnEnter()
    {
        OnBeginExpedition();
    }

    public override void DrawMenu(GameTime gameTime)
    {

    }
}
