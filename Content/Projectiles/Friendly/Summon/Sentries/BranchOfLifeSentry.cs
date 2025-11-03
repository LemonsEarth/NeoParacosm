using Microsoft.Xna.Framework;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Projectiles.None;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;

public class BranchOfLifeSentry : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float AttackTimer => ref Projectile.ai[1];

    const int BUFF_DISTANCE = 400;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 116;
        Projectile.height = 64;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.tileCollide = true;
        Projectile.timeLeft = Projectile.SentryLifeTime;
        Projectile.friendly = false;
        Projectile.sentry = true;
    }

    public override void AI()
    {

        if (AITimer % 30 == 0)
        {
            foreach (Player player in Main.player)
            {
                if (Vector2.Distance(Projectile.Center, player.Center) < BUFF_DISTANCE)
                {
                    player.AddBuff(BuffType<BranchedOfLifedBuff>(), 60);
                }
            }
        }

        Projectile.velocity.Y = 10f;
        if (AttackTimer % 120 == 0)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && Vector2.Distance(Projectile.Center, npc.Center) < BUFF_DISTANCE + 200)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 offset = new Vector2(Main.rand.NextFloat(-20, 20), Main.rand.NextFloat(-20, 20));
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), npc.Center + offset, Vector2.Zero, ProjectileType<BranchOfLifeProj>(), Projectile.damage, 1f, Projectile.owner);
                        }
                    }
                    SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.3f, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
                    AttackTimer = 0;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<PulseEffect>(), ai0: 1, ai1: 8, ai2: 2);
            }
        }

        AttackTimer++;

        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 16)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
        }
        Lighting.AddLight(Projectile.Center, 5, 2, 2);
        AITimer++;
    }

    public NPC GetClosestNPC(int distance)
    {
        NPC closestEnemy = null;
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.CanBeChasedBy() && Projectile.Center.Distance(npc.Center) < distance)
            {
                if (closestEnemy == null)
                {
                    closestEnemy = npc;
                }
                float distanceToNPC = Projectile.Center.DistanceSQ(npc.Center);
                if (distanceToNPC < Projectile.Center.DistanceSQ(closestEnemy.Center))
                {
                    closestEnemy = npc;
                }
            }
        }
        return closestEnemy;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity.Y = 0;
        return false;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }
}
