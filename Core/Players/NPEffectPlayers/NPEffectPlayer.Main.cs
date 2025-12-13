using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Biomes.TheDepths;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Content.Projectiles.Hostile.Evil;
using NeoParacosm.Core.Systems;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Runtime.InteropServices;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

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
