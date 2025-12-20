using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Biomes.TheDepths;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
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

namespace NeoParacosm.Core.Players;

public class NPPlayer : ModPlayer
{
    int timer = 0;
    public static float savedMusicVolume { get; set; } = -1f;

    public override void ResetEffects()
    {
        if (savedMusicVolume != -1f)
        {
            Main.musicVolume = savedMusicVolume;
        }
    }

    public override void PostUpdate()
    {
        if (NPC.FindFirstNPC(NPCType<Researcher>()) > 0 && Main.npc[NPC.FindFirstNPC(NPCType<Researcher>())].Distance(Player.Center) > 500)
        {
            AscensionUISystem UISystem = GetInstance<AscensionUISystem>();
            UISystem.HideUI();
        }
        timer++;
    }
}
