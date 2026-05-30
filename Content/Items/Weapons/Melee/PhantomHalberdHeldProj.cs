using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
namespace NeoParacosm.Content.Items.Weapons.Melee;

public class PhantomHalberdHeldProj : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        LemonUtils.DustCircle(target.Center, 8, 5f, DustID.GemDiamond, 1.5f);
        if (Main.rand.NextBool(4))
        {
            Projectile.GetOwner().Heal(5);
        }
    }

    string trailPath => Texture + "Trail";
    static Asset<Texture2D> trailTexture;

    public override void SetStaticDefaults()
    {
        trailTexture = Request<Texture2D>(trailPath);
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 96;
        Projectile.height = 96;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    float mouseDifferenceValue = 1f;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }

        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item20 with { Volume = 0.8f, PitchRange = (-0.4f, -0.2f) }, Projectile.Center);
        }

        if (AITimer >= TimeLeft)
        {
            Projectile.Kill();
            return;
        }
        Vector2 mouseDifference = new Vector2(Main.mouseX - Main.lastMouseX, Main.mouseY - Main.lastMouseY);
        mouseDifferenceValue = MathHelper.Clamp(mouseDifference.Length(), 0f, 20f);
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        SetPositionRotationDirection(player);

        AITimer++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= 1 + (mouseDifferenceValue / 20f);
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 5f, DustID.GemDiamond, 1.5f);
    }

    void SetPositionRotationDirection(Player player)
    {
        Vector2 dir = player.Center.DirectionTo(Main.MouseWorld);
        float rot = dir.ToRotation();
        //rot += MathHelper.ToRadians(AITimer * 2 * RandDirection);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot - MathHelper.PiOver2);
        float progress = (MathF.Sin(AITimer / 4f) + 1) * 0.5f;
        float length = Projectile.width * 0.75f;
        Projectile.Center = player.MountedCenter + dir * progress * length;
        Projectile.rotation = rot + MathHelper.PiOver4;

        Projectile.velocity = Vector2.Zero;

        if (!dir.HasNaNs())
        {
            player.ChangeDir(Math.Sign(dir.X));
            // Projectile.spriteDirection = player.direction;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = trailTexture.Value;
        Rectangle sourceRect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        Vector2 drawOrigin = new Vector2(sourceRect.Width, sourceRect.Height) * 0.5f;
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
            Color color = (Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length)) * 1f;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
        //LemonUtils.DrawGlow(Projectile.Center, Color.Gold, Projectile.Opacity * 0.5f, Projectile.scale * 1.5f);
        return true;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
