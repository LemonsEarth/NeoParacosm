using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Buffs.Debuffs;

public class SnowgraveDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }
}

public class SnowgraveNPC : GlobalNPC
{
    public override void PostAI(NPC npc)
    {
        if (npc.HasBuff(BuffType<SnowgraveDebuff>()) && !npc.boss)
        {
            npc.velocity = Vector2.Zero;
        }
    }
}
