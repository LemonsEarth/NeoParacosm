using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class CrossedWireHeldProj : PrimProjectile
{
    int AITimer = 0;
    Color electricColor = Color.White;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.Opacity = 1f;
        Projectile.ArmorPenetration = 20;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        float distance = MathHelper.Clamp(target.Distance(Main.MouseWorld), 0, 300);
        float damageMod = (300 - distance) / 300;
        float clampedDamageMod = MathHelper.Clamp(damageMod, 0.5f, 1f);
        modifiers.FinalDamage *= clampedDamageMod;
    }

    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 dir = player.Center.DirectionTo(Main.MouseWorld);
        float armRotValue = player.direction == 1 ? -MathHelper.PiOver2 : -MathHelper.PiOver2;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation + armRotValue);
        Projectile.Center = player.Center + dir * 28;
        Projectile.rotation = movedRotation + MathHelper.PiOver4;
        Projectile.spriteDirection = 1;
        if (!dir.HasNaNs())
        {
            player.ChangeDir(Math.Sign(dir.X));
        }
    }

    float spaceBetweenPoints = 32;
    float baseSpaceBetweenPoints = 32;
    float normalOffset = 16;
    float baseNormalOffset = 16;
    List<Vector2> positions = new List<Vector2>();
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.Alive())
        {
            Projectile.Kill();
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);

        if (AITimer % 2 == 0 && !player.CheckMana(player.HeldItem.mana, true, false))
        {
            Projectile.Kill();
            return;
        }


        Projectile.timeLeft = 2;

        if (AITimer == 0)
        {

        }

        if (Main.myPlayer == Projectile.owner)
        {
            Vector2 mouseDir = player.Center.DirectionTo(Main.MouseWorld);
            SetPositionRotationDirection(player, mouseDir.ToRotation());
        }
        if (!player.channel)
        {
            Projectile.Kill();
            return;
        }

        if (AITimer % 1 == 0)
        {
            positions.Clear();
            positions.Add(Projectile.Center);
            positions.Add(Projectile.Center);

            Vector2 pos = Projectile.Center;
            Vector2 toMouse = (Main.MouseWorld - pos);
            float toMouseLength = toMouse.Length();
            Vector2 toMouseNormalized = toMouse.SafeNormalize(Vector2.Zero);
            int pointCount = (int)MathF.Ceiling(toMouseLength / spaceBetweenPoints);
            while (toMouseLength > spaceBetweenPoints)
            {
                spaceBetweenPoints = Main.rand.NextFloat(0, baseSpaceBetweenPoints);
                normalOffset = Main.rand.NextFloat(-baseNormalOffset, baseNormalOffset);

                toMouse = (Main.MouseWorld - pos);
                toMouseLength = toMouse.Length();
                toMouseNormalized = toMouse.SafeNormalize(Vector2.Zero);

                Vector2 normalDir = toMouseNormalized.RotatedBy(MathHelper.PiOver2);
                pos = pos + toMouseNormalized * spaceBetweenPoints;
                Vector2 posAdjustedForWidth1 = pos + normalDir * normalOffset;
                Vector2 posAdjustedForWidth2 = pos + normalDir * normalOffset * 0.5f;

                if (Projectile.Center.Distance(pos) > Projectile.Center.Distance(Main.MouseWorld))
                {
                    break;
                }

                positions.Add(posAdjustedForWidth1);
                positions.Add(posAdjustedForWidth2);
            }
            positions.Add(Main.MouseWorld);
            positions.Add(Main.MouseWorld);
        }

        SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { PitchRange = (1f, 1.2f), Volume = 0.5f }, Projectile.Center);

        AITimer++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = float.NaN;
        Vector2 endPos = Main.MouseWorld;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPos, baseNormalOffset, ref _);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int quadCount = positions.Count / 2 - 1; // 6 points -> 2 quads etc because of duplicates
        if (quadCount <= 0) return false;
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[quadCount * 6]; // 6 vertices per quad (each quad is 2 triangles, where 2 of the vertices are used to construct both)
        VertexPositionColorTexture QuickVertexPCT(Vector2 pos)
        {
            return new VertexPositionColorTexture(new Vector3(pos, 0), electricColor * Projectile.Opacity, Vector2.Zero);
        }
        for (int i = 0; i < quadCount; i++)
        {
            // Out of 4 vertices in a quad, the last 2 are the ones used for constructing the next quad as well
            // So each quad's vertices start at 2 * i
            // Without duplicate vertices, each quad would start at 4 * i
            Vector2 left0 = positions[2 * i];
            Vector2 right0 = positions[2 * i + 1];
            Vector2 left1 = positions[2 * i + 2];
            Vector2 right1 = positions[2 * i + 3];

            // 6 vertices per quad
            vertices[6 * i] = QuickVertexPCT(left0);
            vertices[6 * i + 1] = QuickVertexPCT(right0);
            vertices[6 * i + 2] = QuickVertexPCT(left1);

            vertices[6 * i + 3] = QuickVertexPCT(left1);
            vertices[6 * i + 4] = QuickVertexPCT(right0);
            vertices[6 * i + 5] = QuickVertexPCT(right1);
        }

        BasicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
        BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        Viewport viewport = GraphicsDevice.Viewport;
        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 10);
        GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
        BasicEffect.CurrentTechnique.Passes[0].Apply();
        GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
        foreach (Vector2 pos in positions)
        {
            LemonUtils.DrawGlow(pos, new Color(230, 230, 255), 0.2f, 0.7f);
        }

        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        return false;
    }
}
