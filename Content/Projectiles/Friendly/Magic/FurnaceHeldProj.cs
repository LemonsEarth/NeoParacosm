using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class FurnaceHeldProj : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];
    bool released = false;
    int releasedTimer = 0;
    int chargeAmount = 0;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return false;
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        Projectile.velocity = Vector2.Zero;
        if (!player.IsAlive())
        {
            Projectile.Kill();
            return;
        }

        if (!player.channel && !released)
        {
            released = true;
            player.SetDummyItemTime(60);
            Projectile.timeLeft = 60;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Projectile.DirectionTo(Main.MouseWorld) *
                    6 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2f),
                    ProjectileType<FurnaceArrow>(),
                    (int)(Projectile.damage * (10 * chargeAmount / 120f)),
                    Projectile.knockBack,
                    player.whoAmI,
                    600,
                    chargeAmount
                );
            }
        }

        if (!released)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { PitchRange = (0.6f, 0.8f), Volume = 0.3f, MaxInstances = 12 });
            player.SetDummyItemTime(2);
            player.heldProj = Projectile.whoAmI;
            SetPositionRotationDirection(player, Main.MouseWorld);
            if (chargeAmount == 119)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { PitchRange = (0.2f, 0.6f), Volume = 1f, MaxInstances = 0 });

                for (int i = 0; i < 5; i++)
                {
                    ParticleSystem.SpawnParticle(
                        ParticleID.Glowy,
                        Projectile.Center,
                        Main.rand.NextVector2Circular(4, 4),
                        Main.rand.NextFromList(Color.OrangeRed),
                        0f,
                        scale: 1.5f,
                        data0: 30,
                        data1: 5,
                        data2: 5,
                        data3: 0.93f
                    );
                }
            }
            if (chargeAmount < 120)
            {
                chargeAmount++;
            }
        }
        else
        {
            releasedTimer++;
        }

        Vector2 playerToProj = player.DirectionTo(Projectile.Center);
        for (int i = -1; i <= 1; i += 2)
        {
            ParticleSystem.SpawnParticle(
                ParticleID.Glowy,
                Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                -playerToProj.RotatedBy(MathHelper.Pi / 6f * i) * 5,
                Main.rand.NextFromList(Color.OrangeRed),
                0f,
                scale: 1.5f,
                data0: 30,
                data1: 5,
                data2: 5,
                data3: 0.93f
            );
        }

        ParticleSystem.SpawnParticle(
            ParticleID.Glowy,
            Projectile.Center + Main.rand.NextVector2Circular(8, 8),
            -playerToProj * 10,
            Main.rand.NextFromList(Color.OrangeRed),
            0f,
            scale: 1f,
            data0: 30,
            data1: 5,
            data2: 5,
            data3: 0.93f
        );

        AITimer++;
    }

    public void SetPositionRotationDirection(Player player, Vector2 targetPosition)
    {
        Vector2 dir = player.Center.DirectionTo(targetPosition);
        if (!dir.HasNaNs())
        {
            player.ChangeDir(Math.Sign(dir.X));
        }
        float movedRotation = dir.ToRotation();
        float armRotValue = -MathHelper.PiOver2;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation + armRotValue);
        float itemRotValue = player.direction == 1 ? 0f : MathHelper.Pi;
        player.itemRotation = movedRotation + itemRotValue;
        Projectile.Center = player.Center + dir * (Projectile.Size * 0.5f).Length();
        Projectile.rotation = movedRotation + MathHelper.PiOver4;
        Projectile.spriteDirection = 1;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {

        return false;

    }

    public override void PostDraw(Color lightColor)
    {

    }
}
