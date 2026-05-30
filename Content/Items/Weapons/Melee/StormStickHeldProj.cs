using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using Terraria.GameContent;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class StormStickHeldProj : ModProjectile
{
    int AITimer = 0;

    ref float special => ref Projectile.ai[0];
    ref float direction => ref Projectile.ai[1];
    ref float useCounter => ref Projectile.ai[2];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.LocalPlayer.HasBuff(BuffType<OverchargedBuff>()))
        {

            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Vector2.UnitY.RotatedBy(MathHelper.PiOver4 * i) * 8;
                Dust.NewDustPerfect(target.RandomPos(), DustType<StreakDust>(), velocity, newColor: Color.Gold, Scale: 0.5f).noGravity = true;
            }

        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 112;
        Projectile.height = 112;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    float rot = 0;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }
        Dust.NewDustPerfect(Projectile.RandomPos(), DustID.GemTopaz, Vector2.Zero, newColor: Color.Gold).noGravity = true;

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);

        Projectile.velocity = Vector2.Zero;

        Projectile.timeLeft = 3;
        float overchargedBoost = player.HasBuff(BuffType<OverchargedBuff>()) ? 2f : 1f;

        rot = player.direction * MathHelper.ToRadians(AITimer * 16) * (player.GetAttackSpeed(DamageClass.Melee)) * overchargedBoost;
        Projectile.localNPCHitCooldown = (int)(10 / (player.GetAttackSpeed(DamageClass.Melee) * overchargedBoost));
        //Projectile.localNPCHitCooldown.NewText();
        if (!player.channel)
        {
            Projectile.Kill();
        }
        SetPositionRotationDirection(player);
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    void SetPositionRotationDirection(Player player)
    {
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot);
        Projectile.Center = player.Center;
        Projectile.rotation = rot;
        //Projectile.spriteDirection = player.direction * (int)direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Projectile.DrawAfterimages(Color.White, 4f);
        Projectile.DrawAfterimages(Color.White, 1f);
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
