using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class StoneSwordspearHeldProj : ModProjectile
{
    int AITimer = 0;
    bool released = false;

    int releasedTimer = 0;

    ref float Direction => ref Projectile.ai[0];
    ref float ChargeAmount => ref Projectile.ai[1];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.velocity *= (1 - target.knockBackResist);
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 90;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }

        float rotation = 0;
        float startRotation = MathHelper.ToRadians(-75f);
        float goalRotation = MathHelper.ToRadians(150f);
        if (!released)
        {
            if (ChargeAmount < 180)
            {
                ChargeAmount++;
            }

            if (AITimer % 10 == 0)
            {
                for (int i = 0; i < ChargeAmount / 60; i++)
                {
                    Dust.NewDustPerfect(Projectile.RandomPos(), DustID.Stone, Vector2.Zero);
                }
            }

            rotation = startRotation;
        }

        if (!player.channel && !released)
        {
            released = true;
            SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (-0.5f, -0.3f) }, Projectile.Center);
        }

        if (released)
        {
            for (int i = 0; i < ChargeAmount / 60; i++)
            {
                Dust.NewDustPerfect(Projectile.RandomPos(), DustID.Stone, Vector2.Zero);
            }
            float lerpSpeed = player.GetAttackSpeed(DamageClass.Melee) * 1.5f;
            float lerpT = lerpSpeed * releasedTimer;
            rotation = MathHelper.SmoothStep(startRotation, goalRotation, lerpT / 30f);
            if (lerpT > 30)
            {
                Projectile.Kill();
                return;
            }
            releasedTimer++;
        }

        SetPositionRotationDirection(player, rotation);

        if (AITimer == 0)
        {

        }

        HeldProjStuff(player);


        AITimer++;
    }

    void HeldProjStuff(Player player)
    {
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(30);
        Projectile.timeLeft = 3;
        Projectile.velocity = Vector2.Zero;
    }

    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
        Vector2 pos = player.Center + new Vector2(-player.direction * Projectile.width / 2, -Projectile.height / 2).RotatedBy(movedRotation * player.direction);
        player.ChangeDir((int)Direction);
        player.SetCompositeArmBack(
            true,
            Player.CompositeArmStretchAmount.Full,
            movedRotation * player.direction + player.direction * ThreePiOverFour + MathHelper.ToRadians(Main.rand.NextFloat(5f, 10f) * ChargeAmount / 60f));
        player.SetCompositeArmFront(
            true,
            Player.CompositeArmStretchAmount.Full,
            movedRotation * player.direction + player.direction * ThreePiOverFour + MathHelper.ToRadians(Main.rand.NextFloat(5f, 10f) * ChargeAmount / 60f));
        Projectile.Center = pos;
        if (!released)
        {
            Projectile.Center += new Vector2(Main.rand.NextFloat() * ChargeAmount / 60f, Main.rand.NextFloat() * ChargeAmount / 60f);
        }
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return released;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= (ChargeAmount / 60f) + 1f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
