using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class Banefly : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
        Projectile.penetrate = 3;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.scale = 1;
        Projectile.tileCollide = true;
        Projectile.DamageType = DamageClass.Generic;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.ArmorPenetration += 5;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.CursedInferno, 180);
        Projectile.velocity -= Projectile.velocity * 2;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (Main.myPlayer == Projectile.owner)
        {
            Projectile.MoveToPos(Main.MouseWorld, Main.rand.NextFloat(0.04f, 0.1f), Main.rand.NextFloat(0.04f, 0.1f), Main.rand.NextFloat(0.1f, 0.2f), Main.rand.NextFloat(0.1f, 0.2f));
        }

        if (AITimer % 10 == 0)
        {
            Projectile.netUpdate = true;
        }
        Projectile.StandardAnimation(6, 4);
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X) != 0 ? Math.Sign(Projectile.velocity.X) : 1;

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        Dust.NewDustPerfect(Projectile.Center, DustID.GemEmerald);
        SoundEngine.PlaySound(SoundID.NPCDeath66 with { Volume = 0.2f }, Projectile.Center);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity -= oldVelocity;
        return false;
    }
}
