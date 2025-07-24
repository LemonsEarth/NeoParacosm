using NeoParacosm.Common.Utils;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.NPCs.Misc;

public class ShadowOrbNPC : ModNPC
{
    float AITimer = 0;
    int savedDamage = 10;
    bool crit = false;

    ref float playerIndex => ref NPC.ai[0];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        NPCID.Sets.CannotDropSouls[Type] = true;
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = 30;
        NPC.height = 30;
        NPC.lifeMax = 400;
        NPC.defense = 0;
        NPC.damage = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 0;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.chaseable = false;
        NPC.noGravity = true;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.lifeMax = 400;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {

    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustCircle(NPC.Center, 8, 8, DustID.GemAmethyst, 2f);
        }

        if (AITimer > 1200)
        {
            LemonUtils.DustCircle(NPC.Center, 8, 8, DustID.GemAmethyst, 2f);
            NPC.active = false;
        }
        NPC.Center += Vector2.UnitY * (float)Math.Sin(AITimer / 48f);
        Lighting.AddLight(NPC.Center, 2, 0, 2);
        NPC.velocity = Vector2.Zero;
        AITimer++;
    }

    public override void OnKill()
    {
        LemonUtils.DustCircle(NPC.Center, 8, 8, DustID.GemAmethyst, 2f);
        int count = crit ? 16 : 8;
        for (int i = 0; i < count; i++)
        {
            LemonUtils.QuickProj(NPC, NPC.Center, Main.rand.NextVector2Circular(10, 10), ProjectileID.TinyEater, damage: savedDamage);
        }
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        savedDamage = hit.Damage;
        crit = hit.Crit;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        
    }

    public override bool? CanFallThroughPlatforms()
    {
        return null;
    }
}
