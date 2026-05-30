using NeoParacosm.Content.Items.Weapons.Magic;
using System.Collections.Generic;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class SpaceShredderPlanet : ModProjectile
{
    int AITimer = 0;
    ref float BaseScale => ref Projectile.ai[0];
    ref float SpinDirection => ref Projectile.ai[1];
    ref float RotOffset => ref Projectile.ai[2];
    bool released = false;
    int releasedTimer = 0;
    int front = 1; // 1 if it should be in front of player, -1 if it should be behind

    bool alreadyFlipped = false;

    enum PlanetType
    {
        Fire,
        Ice,
        Confusion,
        BrokenArmor,
        Poison,
        ShadowFlame
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults()
    {
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 360;
        Projectile.scale = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.hide = true;
    }

    public override void AI()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        if (AITimer == 0)
        {
            Projectile.frame = Main.rand.Next(0, Main.projFrames[Type]);
            if (SpinDirection == 0) SpinDirection = 1;
        }
        Player player = Projectile.GetOwner();
        Projectile.Resize((int)(80 * Projectile.scale), (int)(80 * Projectile.scale));
        if (AITimer % 60 == 0)
        {
            SoundEngine.PlaySound(SoundID.Item7 with { PitchRange = (-0.5f, -0.4f) }, Projectile.Center);
        }
        //Projectile.Center.NewText();
        if (!player.IsAlive() || player.HeldItem.type != ItemType<SpaceShredder>())
        {
            Projectile.Kill();
            return;
        }
        Lighting.AddLight(Projectile.Center, 1, 0, 0);

        if (player.channel && !released)
        {
            if (Projectile.velocity.Length() < 2f)
            {
                if (!alreadyFlipped)
                {
                    alreadyFlipped = true;
                    if (front == 1) front = -1;
                    else front = 1;
                }
            }
            else
            {
                alreadyFlipped = false;
            }
            front = -LemonUtils.Sign(Projectile.velocity.X, 1);
            float distanceToPlayer = Projectile.Center.Distance(player.Center);
            float lerpTargetScale = front == 1 ? BaseScale + 0.5f : BaseScale * 0.5f;
            Projectile.scale = MathHelper.Lerp(lerpTargetScale, BaseScale, MathHelper.Clamp(distanceToPlayer / 300f, 0.1f, 1f));
            Projectile.timeLeft = 360;
            Vector2 toPlayer = Projectile.DirectionTo(player.Center);
            if (!toPlayer.HasNaNs())
            {
                Projectile.velocity += toPlayer;
            }
        }

        if (!player.channel && !released)
        {
            released = true;
            Projectile.penetrate = -1;
        }

        if (released)
        {
            Projectile.scale = MathHelper.Lerp(Projectile.scale, BaseScale, 1 / 20f);
            if (releasedTimer < 180 + RotOffset * 10)
            {
                float angle = RotOffset * (MathHelper.Pi / 5f) + MathHelper.ToRadians(AITimer * 2);
                Vector2 lerpPos = player.Center - Vector2.UnitY.RotatedBy(angle) * 400;
                Projectile.Center = Vector2.Lerp(Projectile.Center, lerpPos, 1f / 20f);
                Projectile.velocity = Vector2.Zero;
            }
            else if (releasedTimer == 180 + RotOffset * 10)
            {
                Projectile.penetrate = 1;

                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * 30;
                }
                Projectile.netUpdate = true;
            }
            releasedTimer++;
        }

        var dust = Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemDiamond);
        dust.noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 18 * SpinDirection);
        AITimer++;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        //Projectile.damage = (int)(Projectile.damage * 1.2f);
        /*if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }

        // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }*/
        return true;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        if (front == 1)
        {
            overPlayers.Add(index);
        }
        else
        {
            behindProjectiles.Add(index);
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.velocity *= 0.9f;
        switch (Projectile.frame)
        {
            case (int)PlanetType.Fire:
                target.AddBuff(BuffID.OnFire, 300);
                break;
            case (int)PlanetType.Ice:
                target.AddBuff(BuffID.Frostburn, 300);
                break;
            case (int)PlanetType.Confusion:
                target.AddBuff(BuffID.Confused, 300);
                break;
            case (int)PlanetType.BrokenArmor:
                target.AddBuff(BuffID.BrokenArmor, 300);
                break;
            case (int)PlanetType.Poison:
                target.AddBuff(BuffID.Poisoned, 300);
                break;
            case (int)PlanetType.ShadowFlame:
                target.AddBuff(BuffID.ShadowFlame, 300);
                break;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(lightColor, 0.5f);
        Projectile.DrawProjectile(lightColor);
        return false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (releasedTimer >= 180)
        {
            modifiers.SetCrit();
        }
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 8f, MathHelper.Pi / 8f)) * 0.8f;
            LemonUtils.QuickProj(
                Projectile,
                Projectile.RandomPos(-16f, -16f),
                velocity,
                Main.rand.Next(ProjectileID.Meteor1, ProjectileID.Meteor3 + 1),
                Projectile.damage / 2f,
                Projectile.knockBack / 2f,
                ai1: 1f
                );
        }
        LemonUtils.DustCircle(Projectile.Center, 16, 8, DustID.GemDiamond, 4);
        SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.6f }, Projectile.Center);

    }
}
