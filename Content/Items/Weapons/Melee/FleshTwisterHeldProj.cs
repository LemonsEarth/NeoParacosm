using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class FleshTwisterHeldProj : ModProjectile
{
    int AITimer = 0;
    int releasedTimer = 0;
    bool released = false;
    Vector2 savedMousePos
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

    int chargeCount = 0;
    Vector2 toMouse;
    float toMouseDistance;

    static Asset<Texture2D> chainTexture;

    public override void Load()
    {
        chainTexture = Request<Texture2D>("NeoParacosm/Content/Items/Weapons/Melee/FleshTwisterChain");
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (released)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = target.Center + new Vector2(Main.rand.NextFloat(-100, 100), -500);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<PurpleLightning>(), Projectile.damage, 1f, Projectile.owner, ai1: target.Center.X, ai2: target.Center.Y);
            }
        }

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 66;
        Projectile.height = 66;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        Projectile.timeLeft = 6000;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 36);
        Projectile.velocity = Vector2.Zero;

        if (!player.channel && !released)
        {
            released = true;
            if (Main.myPlayer == Projectile.owner)
            {
                savedMousePos = Main.MouseWorld;
            }
            Projectile.netUpdate = true;

            toMouse = player.DirectionTo(savedMousePos);
            toMouseDistance = player.Distance(savedMousePos);
        }

        if (player.channel && !released)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Electric, Scale: chargeCount * 0.25f).noGravity = true;
            if (AITimer > 0 && AITimer % 60 == 0 && chargeCount < 5)
            {
                chargeCount++;
                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
            }
            Vector2 targetPos = player.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(AITimer * 6)) * 32;
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 1 / 10f);
        }

        if (released)
        {
            Vector2 targetPos = player.Center + toMouse * toMouseDistance;
            toMouse = toMouse.RotatedBy(MathHelper.TwoPi * 1.5f * player.GetAttackSpeed(DamageClass.Melee) / 60f);

            float duration = chargeCount * 60 * player.GetAttackSpeed(DamageClass.Melee);

            if (duration - releasedTimer < 30)
            {
                Projectile.Opacity -= 1 / 30f;
            }

            if (releasedTimer > duration)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Vector2.Lerp(playerCenter, targetPos, MathHelper.Clamp(releasedTimer * player.GetAttackSpeed(DamageClass.Melee) / 30f, 0f, 1f));
            if (releasedTimer % 10 == 0)
            {
                if (LemonUtils.NotClient())
                {
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromAI(),
                        Projectile.Center,
                        Vector2.Zero,
                        ProjectileType<FleshTwisterFollowerProj>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner,
                        ai0: 180
                        );
                }
            }

            releasedTimer++;
        }

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        Vector2 startPos = Projectile.GetOwner().Center;
        int segmentCount = (int)(Projectile.Center.Distance(startPos) / 10) + 1;
        int segmentsDrawn = 0;

        Vector2 startToProj = startPos.DirectionTo(Projectile.Center);
        float startToProjDistance = startPos.Distance(Projectile.Center);

        while (segmentsDrawn < segmentCount)
        {
            Vector2 controlPoint = startPos + (Projectile.Center - startPos) * 0.5f;
            controlPoint += startToProj.RotatedBy(MathHelper.PiOver2) * 60;
            Vector2 chainPos = LemonUtils.BezierCurve(startPos, Projectile.Center, controlPoint, segmentsDrawn / (float)segmentCount);
            Vector2 nextChainPos = LemonUtils.BezierCurve(startPos, Projectile.Center, controlPoint, (segmentsDrawn + 1) / (float)segmentCount);
            float rotation = chainPos.DirectionTo(nextChainPos).ToRotation() - MathHelper.PiOver2;
            Main.EntitySpriteDraw(chainTexture.Value, chainPos - Main.screenPosition, null, Color.White * Projectile.Opacity, rotation, new Vector2(8, 8), Projectile.scale, SpriteEffects.None);
            segmentsDrawn++;
        }

        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection), 0);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
