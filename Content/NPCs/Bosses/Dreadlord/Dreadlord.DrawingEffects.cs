using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.NPCs.Bosses.Dreadlord;

// This boss is spread across multiple files
// This file contains drawing and visual/audio effect logic

public partial class Dreadlord : ModNPC
{
    // Wing animation stuff
    int wingFrame = 0;
    int wingAnimTimer = 0;

    // Whether eye lasers should be active
    bool GreenLaserEnabled = false;
    bool YellowLaserEnabled = false;

    // Body shader
    bool shaderIsActive = false;

    void PlayRoar(float bonusPitch = 0f)
    {
        SoundEngine.PlaySound(SoundID.Roar with { Pitch = -1f + bonusPitch }, NPC.Center);
        SoundEngine.PlaySound(SoundID.NPCDeath62 with { Pitch = -0.5f + bonusPitch }, NPC.Center);
    }

    private void AuraBurst(int count, Vector2 speed)
    {
        for (int i = 0; i < count; i++)
        {
            Dust.NewDustDirect(NPC.RandomPos(200, 100), 2, 2, DustID.GemTopaz, speed.X, speed.Y, Scale: Main.rand.NextFloat(2f, 3f));
        }
    }

    void ShakeCrimsonHead(float height = 32, float intensity = 24)
    {
        HeadCrimson.Position = HeadCrimson.DefaultPosition - Vector2.UnitY * height + Main.rand.NextVector2Circular(intensity, intensity);
    }

    void ShakeCorruptHead(float height = 32, float intensity = 24)
    {
        HeadCorrupt.Position = HeadCorrupt.DefaultPosition - Vector2.UnitY * height + Main.rand.NextVector2Circular(intensity, intensity);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ) return;

    }

    public void SetDefaultBodyPartPositions()
    {
        Body.DefaultPosition = NPC.Center;
        Body.Origin = Body.Texture.Size() * 0.5f;
        Body.MiscPosition1 = Body.Position + new Vector2(-Body.Width * 0.25f, -Body.Height * 0.2f) * NPC.scale;
        Body.MiscPosition2 = Body.Position + new Vector2(+Body.Width * 0.25f, -Body.Height * 0.2f) * NPC.scale;
        Body.Opacity = NPC.Opacity;
        Body.Scale = NPC.scale;

        LegCorrupt.DefaultPosition = Body.Position + new Vector2(-Body.Width * 0.57f, -Body.Height * 0.1f) * NPC.scale;
        LegCorrupt.MiscPosition1 = LegCorrupt.Position + new Vector2(LegCorrupt.Width * 0.1f, LegCorrupt.Height * 0.7f).RotatedBy(LegCorrupt.Rotation) * NPC.scale;
        LegCorrupt.Origin = new Vector2(LegCorrupt.Width * 0.5f, LegCorrupt.Height * 0.17f);
        LegCorrupt.Frames = 2;
        LegCorrupt.Opacity = NPC.Opacity;
        LegCorrupt.Scale = NPC.scale;

        LegCrimson.DefaultPosition = Body.Position + new Vector2(Body.Width * 0.57f, -Body.Height * 0.1f) * NPC.scale;
        LegCrimson.MiscPosition1 = LegCrimson.Position + new Vector2(LegCrimson.Width * 0.1f, LegCrimson.Height * 0.7f).RotatedBy(LegCrimson.Rotation) * NPC.scale;
        LegCrimson.Origin = new Vector2(LegCrimson.Width * 0.5f, LegCrimson.Height * 0.17f);
        LegCrimson.Opacity = NPC.Opacity;
        LegCrimson.Scale = NPC.scale;
        LegCrimson.Frames = 2;
        LegCrimson.Scale = NPC.scale;

        BackLegs.DefaultPosition = Body.Position + new Vector2(0, BackLegs.Height * 0.25f) * NPC.scale;
        BackLegs.Origin = BackLegs.Texture.Size() * 0.5f;
        BackLegs.Opacity = NPC.Opacity;
        BackLegs.Scale = NPC.scale;

        HeadCorrupt.DefaultPosition = Body.MiscPosition1 + new Vector2(0, -50) * NPC.scale;
        HeadCorrupt.MiscPosition1 = HeadCorrupt.DefaultPosition + new Vector2(0, -64) * NPC.scale;
        HeadCorrupt.Origin = HeadCorrupt.Texture.Size() * 0.5f;
        HeadCorrupt.Frames = 2;
        HeadCorrupt.Opacity = NPC.Opacity;
        HeadCorrupt.Scale = NPC.scale;

        HeadCrimson.DefaultPosition = Body.MiscPosition2 + new Vector2(0, -50) * NPC.scale;
        HeadCrimson.MiscPosition1 = HeadCrimson.DefaultPosition + new Vector2(0, -64) * NPC.scale;
        HeadCrimson.Origin = HeadCrimson.Texture.Size() * 0.5f;
        HeadCrimson.Frames = 2;
        HeadCrimson.Opacity = NPC.Opacity;
        HeadCrimson.Scale = NPC.scale;

        WingCorrupt.DefaultPosition = Body.Position - new Vector2(0, Body.Height * 0.2f) * NPC.scale;
        WingCorrupt.Origin = new Vector2(WingCorrupt.Width, WingCorrupt.Height * 0.5f);
        WingCorrupt.Frames = 6;
        WingCorrupt.Opacity = NPC.Opacity;
        WingCorrupt.Scale = NPC.scale;

        WingCrimson.DefaultPosition = Body.Position - new Vector2(0, Body.Height * 0.2f) * NPC.scale;
        WingCrimson.Origin = new Vector2(0, WingCrimson.Height * 0.5f);
        WingCrimson.Frames = 6;
        WingCrimson.Opacity = NPC.Opacity;
        WingCrimson.Scale = NPC.scale;
    }

    public void SetBodyPartPositions(Vector2 headCorruptTargetPosition = default,
                                    Vector2 headCrimsonTargetPosition = default,
                                    Vector2 legCorruptTargetPosition = default,
                                    Vector2 legCrimsonTargetPosition = default,
                                    Vector2 bodyTargetPosition = default,
                                    float headLerpSpeed = 1 / 10f,
                                    float legLerpSpeed = 1 / 10f,
                                    float bodyLerpSpeed = 1 / 10f)
    {
        if (headCorruptTargetPosition == default)
        {
            headCorruptTargetPosition = HeadCorrupt.DefaultPosition;
        }

        if (headCrimsonTargetPosition == default)
        {
            headCrimsonTargetPosition = HeadCrimson.DefaultPosition;
        }

        if (legCorruptTargetPosition == default)
        {
            legCorruptTargetPosition = LegCorrupt.DefaultPosition;
        }

        if (legCrimsonTargetPosition == default)
        {
            legCrimsonTargetPosition = LegCrimson.DefaultPosition;
        }

        if (bodyTargetPosition == default)
        {
            bodyTargetPosition = Body.DefaultPosition;
        }

        Body.Position = Vector2.Lerp(Body.Position, bodyTargetPosition, bodyLerpSpeed);

        LegCorrupt.Position = Vector2.Lerp(LegCorrupt.Position, legCorruptTargetPosition, legLerpSpeed);
        LegCrimson.Position = Vector2.Lerp(LegCrimson.Position, legCrimsonTargetPosition, legLerpSpeed);

        BackLegs.Position = BackLegs.DefaultPosition;
        HeadCorrupt.Position = Vector2.Lerp(HeadCorrupt.Position, headCorruptTargetPosition, headLerpSpeed);
        HeadCrimson.Position = Vector2.Lerp(HeadCrimson.Position, headCrimsonTargetPosition, headLerpSpeed);

        WingCorrupt.Position = WingCorrupt.DefaultPosition;
        WingCrimson.Position = WingCrimson.DefaultPosition;

    }

    void LerpScale(float targetScale, float time)
    {
        // rounding stuff
        if (targetScale == 1f && MathF.Abs(targetScale - NPC.scale) < 0.05f)
        {
            NPC.scale = 1f;
        }
        NPC.scale = MathHelper.Lerp(NPC.scale, targetScale, time);
    }

    #region Animation Methods
    void ResetLegFrames()
    {
        SetLegCorruptFrame(LEG_STANDARD);
        SetLegCrimsonFrame(LEG_STANDARD);
    }

    void ResetMouthFrames()
    {
        SetHeadCorruptFrame(MOUTH_CLOSED);
        SetHeadCrimsonFrame(MOUTH_CLOSED);
    }

    void SetLegCorruptFrame(int frame)
    {
        LegCorrupt.CurrentFrame = frame;
    }

    void SetLegCrimsonFrame(int frame)
    {
        LegCrimson.CurrentFrame = frame;
    }

    void SetHeadCorruptFrame(int frame)
    {
        HeadCorrupt.CurrentFrame = frame;
    }

    void SetHeadCrimsonFrame(int frame)
    {
        HeadCrimson.CurrentFrame = frame;
    }

    void AnimateWings(int frameDuration)
    {
        if (wingAnimTimer > frameDuration)
        {
            wingAnimTimer = 0;
            wingFrame++;
        }
        if (wingFrame >= 6)
        {
            wingFrame = 0;
        }

        WingCorrupt.CurrentFrame = wingFrame;
        WingCrimson.CurrentFrame = wingFrame;
        wingAnimTimer++;
    }
    #endregion

    #region Drawing

    void EnableLasers(bool enabled)
    {
        GreenLaserEnabled = enabled;
        YellowLaserEnabled = enabled;
    }

    void DrawLaserCorrupt()
    {
        LemonUtils.DrawLaser(HeadCorrupt.Position + new Vector2(-0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f), player.Center, 2, Color.GreenYellow);
        LemonUtils.DrawLaser(HeadCorrupt.Position + new Vector2(0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f), player.Center, 2, Color.GreenYellow);
    }

    void DrawLaserCrimson()
    {
        LemonUtils.DrawLaser(HeadCrimson.Position + new Vector2(-0.2f * HeadCrimson.Width, -HeadCrimson.Height * 0.43f), player.Center, 2, Color.Yellow);
        LemonUtils.DrawLaser(HeadCrimson.Position + new Vector2(0.2f * HeadCrimson.Width, -HeadCrimson.Height * 0.43f), player.Center, 2, Color.Yellow);
    }

    void DrawNeck(Vector2 neckBase, Vector2 destination, Asset<Texture2D> texture, float brightness = 1)
    {
        Vector2 baseToDestination = neckBase.DirectionTo(destination);
        float distanceLeft = neckBase.Distance(destination);
        float rotation = baseToDestination.ToRotation() - MathHelper.PiOver2;

        Vector2 drawPos = neckBase;

        while (distanceLeft > -texture.Height() * 0.9f * NPC.scale)
        {
            Main.EntitySpriteDraw(texture.Value,
                drawPos - Main.screenPosition,
                null,
                new Color(brightness, brightness, brightness) * NPC.Opacity,
                rotation,
                texture.Size() * 0.5f,
                NPC.scale,
                SpriteEffects.None);
            if (shaderIsActive)
            {
                var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
                shader.Shader.Parameters["uTime"].SetValue(AITimer);
                shader.Shader.Parameters["color"].SetValue(Color.Yellow.ToVector4() * NPC.Opacity);
                shader.Shader.Parameters["moveSpeed"].SetValue(2f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
                Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
                shader.Apply();
                Main.EntitySpriteDraw(texture.Value,
                                    drawPos - Main.screenPosition,
                                    null,
                                    Color.White * NPC.Opacity,
                                    rotation,
                                    texture.Size() * 0.5f,
                                    NPC.scale,
                                    SpriteEffects.None);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            drawPos += baseToDestination * texture.Height() * 0.9f * NPC.scale;
            distanceLeft -= texture.Height() * 0.9f * NPC.scale;
        }
    }

    void DrawBodyParts(float brightness = 1)
    {
        BackLegs.Draw(shaderIsActive, brightness, (int)AITimer);
        WingCorrupt.Draw(shaderIsActive, brightness, (int)AITimer);
        WingCrimson.Draw(shaderIsActive, brightness, (int)AITimer);
        Body.Draw(shaderIsActive, brightness, (int)AITimer);
        LegCorrupt.Draw(shaderIsActive, brightness, (int)AITimer);
        LegCrimson.Draw(shaderIsActive, brightness, (int)AITimer);
        DrawNeck(Body.MiscPosition1, HeadCorrupt.Position, NeckTextureCorrupt, brightness);
        DrawNeck(Body.MiscPosition2, HeadCrimson.Position, NeckTextureCrimson, brightness);
        HeadCorrupt.Draw(shaderIsActive, brightness, (int)AITimer);
        HeadCrimson.Draw(shaderIsActive, brightness, (int)AITimer);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        DrawBodyParts(NPC.scale * NPC.scale);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        //Eye glow
        if (HeadCorrupt.CurrentFrame == MOUTH_CLOSED)
        {
            LemonUtils.DrawGlow(HeadCorrupt.Position + new Vector2(-0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f) * NPC.scale, Color.LightGreen * NPC.Opacity, 0.8f * NPC.scale, 1f);
            LemonUtils.DrawGlow(HeadCorrupt.Position + new Vector2(0.2f * HeadCorrupt.Width, -HeadCorrupt.Height * 0.43f) * NPC.scale, Color.LightGreen * NPC.Opacity, 0.8f * NPC.scale, 1f);
        }

        if (HeadCrimson.CurrentFrame == MOUTH_CLOSED)
        {
            LemonUtils.DrawGlow(HeadCrimson.Position + new Vector2(0.2f * HeadCrimson.Width, -HeadCrimson.Height * 0.43f) * NPC.scale, Color.Yellow * NPC.Opacity, 0.8f * NPC.scale, 1f);
        }

        if (GreenLaserEnabled)
        {
            DrawLaserCorrupt();
        }

        if (YellowLaserEnabled)
        {
            DrawLaserCrimson();
        }
    }
    #endregion
}
