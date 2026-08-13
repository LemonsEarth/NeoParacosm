using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Accessories.Combat.Summon;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon;

public class SentryTurret : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float TimeLeft => ref Projectile.ai[1];

    bool landing = false;
    bool landed = false;

    bool IsOnLand()
    {
        Point v1 = new Point(0, 1);
        Point tileBelowPos = Projectile.Center.ToTileCoordinates() + v1;
        Point tileBelowPosLeft = (Projectile.Center + new Vector2(-Projectile.width / 2, 0)).ToTileCoordinates() + v1;
        Point tileBelowPosRight = (Projectile.Center + new Vector2(Projectile.width / 2, 0)).ToTileCoordinates() + v1;
        bool hasTile = tileBelowPos.HasSolidTile() || tileBelowPosLeft.HasSolidTile() || tileBelowPosRight.HasSolidTile();
        return landing && hasTile;
    }

    NPC closestEnemy;

    static Asset<Texture2D> baseTexture;

    public override void SetStaticDefaults()
    {
        baseTexture = Request<Texture2D>(Texture + "Base");
        Main.projFrames[Projectile.type] = 4;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 18;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.tileCollide = true;

        Projectile.timeLeft = 3600;
        Projectile.friendly = true;
        Projectile.hide = true;
        Projectile.ContinuouslyUpdateDamageStats = true;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (oldVelocity.Y > 0)
        {
            landing = true;
        }
        return false;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return false;
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        Projectile.timeLeft = 3;

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }

        if (landing)
        {
            closestEnemy = LemonUtils.GetClosestNPCWithLOS(Projectile, 1000);
            Projectile.velocity.X *= 0.9f;
            Projectile.velocity.Y += 0.15f;

            if (IsOnLand() && !landed)
            {
                landed = true;
                Projectile.Center -= Vector2.UnitY * 20;
                Projectile.Resize(24, 38);
            }

            if (landed)
            {
                if (TimeLeft - AITimer > 24)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frame < 3 && Projectile.frameCounter % 6 == 0)
                    {
                        Projectile.frame++;
                    }
                }
                else
                {
                    Projectile.frameCounter++;
                    if (Projectile.frame > 0 && Projectile.frameCounter % 6 == 0)
                    {
                        Projectile.Resize(24, Projectile.height - 8);
                        Projectile.frame--;
                    }
                }

                if (closestEnemy != null && closestEnemy.DistanceSQ(Projectile.Center) < 400 * 400)
                {
                    Vector2 dir = Projectile.DirectionTo(closestEnemy.Center);
                    float targetRotation = dir.ToRotation();
                    if (Projectile.spriteDirection == -1)
                    {
                        targetRotation += MathHelper.Pi;
                    }
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, targetRotation, 1 / 10f);
                    if (dir.X < 0)
                    {
                        Projectile.spriteDirection = -1;
                    }
                    else
                    {
                        Projectile.spriteDirection = 1;
                    }
                    if (AITimer % 60 == 0)
                    {
                        SoundEngine.PlaySound(SFX.Gunshot with { PitchRange = (0.2f, 0.5f) }, Projectile.Center);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile.NewProjectileDirect(
                                Projectile.GetSource_FromAI(),
                                Projectile.Center,
                                dir * 10,
                                ProjectileID.Bullet,
                                Projectile.damage,
                                Projectile.knockBack,
                                Projectile.owner
                                );
                        }
                    }
                }
                else
                {
                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, 0f, 1 / 8f);
                }
            }
        }
        else
        {
            Projectile.rotation = MathHelper.ToRadians(LemonUtils.Sign(Projectile.velocity.X, 1) * AITimer * 6);
            Projectile.velocity.Y += 0.2f;
        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D basetexture = baseTexture.Value;
        Texture2D texture = Projectile.GetTexture();
        Rectangle frame = texture.Frame(1, 4, 0, Projectile.frame);
        Vector2 drawOrigin = frame.Size() * 0.5f;
        if (landing)
        {
            Main.EntitySpriteDraw(basetexture, Projectile.Center - Main.screenPosition, frame, lightColor, 0f, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection), 0);

        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindProjectiles.Add(index);
    }

    public override void OnKill(int timeLeft)
    {

    }
}
