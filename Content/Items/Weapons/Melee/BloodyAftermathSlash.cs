using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class BloodyAftermathSlash : ModProjectile
{
    int AITimer = 0;
    int chargeTimer = 0;
    bool flipVertically = false;
    bool released = false;
    ref float FlipVertically => ref Projectile.ai[0];
    ref float Direction => ref Projectile.ai[1];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.Crit)
        {
            for (int i = 0; i < 2 * Projectile.scale; i++)
            {
                LemonUtils.QuickProj(Projectile, Projectile.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * 8, ProjectileType<CrimsonThornFriendly>(), hit.Damage / 3);
            }
        }

        float speed = Projectile.scale * Projectile.scale;
        LemonUtils.DustBurst(2, target.Center, DustType<FireDust>(), speed, speed, 0.5f, 1f, Color.Red);
        LemonUtils.DustBurst(2, target.Center, DustType<FireDust>(), speed, speed, 0.5f, 1f, Color.DarkRed);
        LemonUtils.DustBurst(2, target.Center, DustType<FireDust>(), speed, speed, 0.5f, 1f, Color.Black);

        BloodyAftermathPlayer baPlayer = Projectile.GetOwner().GetModPlayer<BloodyAftermathPlayer>();
        if (baPlayer.HitCount < BloodyAftermathPlayer.MAX_HIT_COUNT)
        {
            if (chargeTimer >= 180)
            {
                baPlayer.HitCount += 5;
            }
            else
            {
                baPlayer.HitCount++;
            }

            if (baPlayer.HitCount >= BloodyAftermathPlayer.MAX_HIT_COUNT)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath52 with { Volume = 0.5f, PitchRange = (-0.8f, -0.2f) }, baPlayer.Player.Center);

                LemonUtils.DustBurst(8, baPlayer.Player.Center, DustType<FireDust>(), 2, 2, 0.5f, 1.5f, Color.Red);
                LemonUtils.DustBurst(8, baPlayer.Player.Center, DustType<FireDust>(), 2, 2, 0.5f, 1.5f, Color.DarkRed);
                LemonUtils.DustBurst(8, baPlayer.Player.Center, DustType<FireDust>(), 2, 2, 0.5f, 1.5f, Color.Black);
            }
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= Projectile.scale * Projectile.scale;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults()
    {
        Projectile.width = 160;
        Projectile.height = 140;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 10;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.timeLeft = 10;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 180;
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }
        if (AITimer == 0)
        {
            if (Direction == 0)
            {
                Direction = 1;
            }
        }

        if (!player.channel && !released)
        {
            released = true;
            SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
        }
        SetPositionRotationDirection(player);

        if (player.channel && !released)
        {
            Direction = LemonUtils.Sign(player.DirectionTo(Main.MouseWorld).X, 1);
            Projectile.timeLeft = 10;
            player.moveSpeed *= 0.5f;
            player.SetDummyItemTime(2);
            player.heldProj = Projectile.whoAmI;
            if (chargeTimer < 180)
            {
                chargeTimer++;
                Dust.NewDustPerfect(player.RandomPos(), DustType<FireDust>(), -Vector2.UnitY * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(0.1f, 0.3f), newColor: Color.Red).noGravity = true;
            }
            if (chargeTimer == 179)
            {
                LemonUtils.QuickPulse(Projectile, player.Center, 2, 3, 8, Color.Red, player);
                SoundEngine.PlaySound(SoundID.DD2_DrakinShot, Projectile.Center);
                LemonUtils.QuickScreenShake(player.Center, 10, 5, 30, 500);
                for (int i = 0; i < 2; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), player.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), GoreType<RedGore>());
                    Gore.NewGore(Projectile.GetSource_FromThis(), player.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), GoreType<RedSmokeGore>());
                }
            }
            return;
        }
        Projectile.scale = 1f + MathHelper.Clamp(chargeTimer / 180f, 0f, 1f);
        Projectile.Resize(Projectile.scale * 160, Projectile.scale * 140);
        Projectile.StandardAnimation(2, 5);

        AITimer++;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return released;
    }

    public override void OnKill(int timeLeft)
    {

    }

    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Projectile.velocity = Vector2.Zero;
        Projectile.Center = player.Center + new Vector2(Direction * Projectile.width * 0.5f, 0);
        Projectile.spriteDirection = (int)Direction;
        player.ChangeDir((int)Direction);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (!released)
        {
            Texture2D swordTexture = TextureAssets.Item[ItemType<BloodyAftermath>()].Value;
            SpriteEffects spriteDir = Direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Direction == 1 ? -MathHelper.PiOver4 : MathHelper.PiOver4;
            Vector2 pos = Projectile.GetOwner().Center + new Vector2(-Direction * 32, 0);
            Main.EntitySpriteDraw(swordTexture, pos - Main.screenPosition, null, Color.White, rotation, swordTexture.Size() * 0.5f, Projectile.scale * 0.5f, spriteDir);
        }
        else
        {
            SpriteEffects spriteEffects = LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection);
            if (FlipVertically == 1)
            {
                spriteEffects = spriteEffects | SpriteEffects.FlipVertically;
            }
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle sourceRect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(sourceRect.Width, sourceRect.Height) * 0.5f;
            Main.EntitySpriteDraw(texture, drawPos, sourceRect, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
        }
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
