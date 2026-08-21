using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class MawOfHerberusHeldProj : ModProjectile
{
    private static Asset<Texture2D> chainTexture;

    int AITimer;
    ref float ChargeTimer => ref Projectile.ai[0];
    Vector2 SavedPos
    {
        get
        {
            return new Vector2(Projectile.ai[1], Projectile.ai[2]);
        }
        set
        {
            Projectile.ai[1] = value.X;
            Projectile.ai[2] = value.Y;
        }
    }
    int releasedTimer = 0;
    bool released = false;

    public override void SetStaticDefaults()
    {
        chainTexture = Request<Texture2D>(Texture + "Chain");
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.timeLeft = 600;

        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = -1;

        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;

        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }

    Player player => Projectile.GetOwner();

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        if (!player.active || player.dead || player.noItems || player.CCed || Projectile.Center.DistanceSQ(player.Center) > 900f * 900f)
        {
            Projectile.Kill();
            return;
        }

        if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
        {
            Projectile.Kill();
            return;
        }
        Projectile.timeLeft = 2;
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        Projectile.rotation = player.Center.DirectionTo(Projectile.Center).ToRotation();
        if (!released)
        {
            if (ChargeTimer == 179)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { PitchRange = (-0.5f, -0.3f), Volume = 0.5f }, Projectile.Center);
                if (player.GetLifePercent() > 0.5f)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDustPerfect(Projectile.RandomPos(), DustID.RuneWizard, -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Dust.NewDustPerfect(Projectile.RandomPos(), DustID.OrangeStainedGlass, -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                        Dust.NewDustPerfect(Projectile.RandomPos(), DustID.GemTopaz, -Vector2.UnitY * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1, 2f)).noGravity = true;
                    }
                }
            }
            if (ChargeTimer < 180)
            {
                ChargeTimer++;
            }
            float rotSpeed = MathHelper.ToRadians(player.direction * AITimer * 24 * player.GetAttackSpeed(DamageClass.Melee));
            Vector2 adjustedPos = player.Center + new Vector2(48, 0).RotatedBy(rotSpeed);
            Projectile.Center = adjustedPos;
        }

        if (!player.channel && !released)
        {
            released = true;
            Projectile.knockBack *= MathF.Max(ChargeTimer / 60f, 1);
            if (Main.myPlayer == Projectile.owner)
            {
                SavedPos = Main.MouseWorld;
            }
            Projectile.netUpdate = true;
            Projectile.Center = player.Center;
            Projectile.velocity = player.Center.DirectionTo(SavedPos) * 20f;
        }

        if (released)
        {
            player.ChangeDir(LemonUtils.Sign(Projectile.Center.X - player.Center.X, 1));
            if (releasedTimer == 15 && ChargeTimer > 60f)
            {
                if (player.GetLifePercent() > 0.5f)
                {
                    for (int i = -(int)(ChargeTimer / 60f); i <= ChargeTimer / 60f; i++)
                    {
                        SoundEngine.PlaySound(SoundID.Item45 with { PitchRange = (-0.5f, -0.1f) }, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item28 with { PitchRange = (0.2f, 0.4f), Volume = 0.5f }, Projectile.Center);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 velocity =
                                Projectile.velocity.SafeNormalize(Vector2.Zero)
                                .RotatedBy(i * MathHelper.Pi / 8f)
                                * Main.rand.NextFloat(12, 15) * ChargeTimer / 60f;
                            LemonUtils.QuickProj(
                            Projectile,
                            Projectile.Center - velocity.SafeNormalize(Vector2.Zero) * 64,
                            velocity,
                            ProjectileType<HerberusSporeBigFriendly>(),
                            (Projectile.damage / 2f) * ChargeTimer / 60f,
                            ai0: 10
                            );
                        }
                    }
                }
                else
                {
                    for (int i = -(int)(ChargeTimer / 60f); i <= ChargeTimer / 60f; i++)
                    {
                        SoundEngine.PlaySound(SoundID.Item45 with { PitchRange = (-0.5f, -0.1f) }, Projectile.Center);
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { PitchRange = (-0.2f, 0.1f), Volume = 0.7f, MaxInstances = 3 }, Projectile.Center);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 velocity =
                                Projectile.velocity.SafeNormalize(Vector2.Zero)
                                .RotatedBy(i * MathHelper.Pi / 8f)
                                * Main.rand.NextFloat(15, 18) * ChargeTimer / 60f;
                            LemonUtils.QuickProj(
                            Projectile,
                            Projectile.Center - velocity.SafeNormalize(Vector2.Zero) * 64,
                            velocity,
                            ProjectileType<HerberusSporeBigFireFriendly>(),
                            Projectile.damage * ChargeTimer / 60f,
                            ai0: 30
                            );
                        }
                    }
                }
            }
            if (releasedTimer > 15)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(player.Center) * 20;
                if (Projectile.DistanceSQ(player.Center) < 32 * 32)
                {
                    Projectile.Kill();
                    return;
                }
            }
            releasedTimer++;
        }

        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 startDrawPos = player.Center;
        Vector2 endDrawPos = Projectile.Center;
        Vector2 startToEnd = endDrawPos - startDrawPos;
        Vector2 startToEndDir = startToEnd.SafeNormalize(Vector2.Zero);
        Vector2 drawPos = startDrawPos;
        float rot = startToEnd.ToRotation();

        int segmentCountToDraw = (int)(startToEnd.Length() / 10) + 1;
        int segmentsDrawn = 0;
        while (segmentsDrawn < segmentCountToDraw)
        {
            Rectangle frame = chainTexture.Frame(1, 4, 0, segmentsDrawn % 4);
            Main.EntitySpriteDraw(chainTexture.Value, drawPos - Main.screenPosition, frame, lightColor, rot, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            drawPos += startToEndDir * 10;
            segmentsDrawn++;
        }
        return true;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
