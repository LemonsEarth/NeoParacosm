using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Core.Systems.World.GenPasses.Expeditions;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria.GameContent.Creative;
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
        Player oldPlayer = Main.LocalPlayer;
        Player player = new Player { whoAmI = oldPlayer.whoAmI, name = oldPlayer.name };
        SetupPlayerStatsAndInventoryBasedOnDifficulty(player);

        Main.player[oldPlayer.whoAmI] = player;
        player.Center = new Vector2(Main.maxTilesX * 16 / 5, 100);
        player.AddBuff(BuffType<NoFallDamageBuff>(), 20 * 60);
    }

    // Copied and edited from vanilla
    public void SetupPlayerStatsAndInventoryBasedOnDifficulty(Player player)
    {
        int num = 0;
        player.inventory[num].SetDefaults(ItemID.CopperShortsword);
        player.inventory[num++].Prefix(-1);
        player.inventory[num].SetDefaults(ItemID.CopperPickaxe);
        player.inventory[num++].Prefix(-1);
        player.inventory[num].SetDefaults(ItemID.CopperAxe);
        player.inventory[num++].Prefix(-1);

        player.savedPerPlayerFieldsThatArentInThePlayerClass = new Player.SavedPlayerDataWithAnnoyingRules();
        CreativePowerManager.Instance.ResetDataForNewPlayer(player);
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
