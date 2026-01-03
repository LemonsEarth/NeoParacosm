using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using Terraria.Graphics.Effects;

namespace NeoParacosm.Core.Players.NPEffectPlayers;

public partial class NPEffectPlayer : ModPlayer
{
    public int Timer { get; private set; } = 0;

    public override void PostUpdate()
    {

        Timer++;
    }

    public override void PostUpdateMiscEffects()
    {
        DeadForestEffects();
        DCEffects();

        if (Timer % 10 == 0)
        {
            if (!NPC.AnyNPCs(NPCType<Deathbird>()))
            {
                Filters.Scene.Deactivate("NeoParacosm:DeathbirdArenaShader");
            }
        }
    }
}
