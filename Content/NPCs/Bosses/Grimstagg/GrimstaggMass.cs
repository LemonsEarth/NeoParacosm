using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Death.DeathKnightCaptain;
using NeoParacosm.Core.Systems.Assets;
using System.Linq;
using Terraria.Audio;
using static Microsoft.Xna.Framework.MathHelper;

namespace NeoParacosm.Content.NPCs.Bosses.Grimstagg;

// This boss is spread across multiple files
// This file contains primarily AI and Attack logic

public partial class GrimstaggMass : ModNPC
{
    ref float AITimer => ref NPC.ai[0];
    ref float GrimstaggWhoAmI => ref NPC.ai[1];

    public Vector2 MassTarget { get; private set; }

    NPC grimstagg;

    public override void AI()
    {
        if (AITimer == 0)
        {
            MassTarget = NPC.Center;
        }
        if (DespawnCheck())
        {
            return;
        }

        grimstagg = Main.npc[(int)GrimstaggWhoAmI];
        MassTarget = grimstagg.Center;
        NPC.MoveToPos(grimstagg.Center + Vector2.UnitY * 600, 0.2f, 0.2f, 0.2f, 0.2f);
        NPC.spriteDirection = -1;
        AITimer++;
    }

    bool DespawnCheck()
    {
        if (GrimstaggWhoAmI < 0 ||
            !Main.npc[(int)GrimstaggWhoAmI].active ||
            !Main.npc[(int)GrimstaggWhoAmI].IsAlive() ||
            Main.npc[(int)GrimstaggWhoAmI].type != NPCType<Grimstagg>())
        {
            NPC.active = false;
            NPC.life = 0;
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            return true;
        }
        return false;
    }
}
