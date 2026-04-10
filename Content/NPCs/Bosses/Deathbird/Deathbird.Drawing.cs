using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Biomes.DeadForest;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Items.BossBags;
using NeoParacosm.Content.Items.Placeable.Relics;
using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;
using NeoParacosm.Content.Items.Weapons.Melee;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Content.Items.Weapons.Summon;
using NeoParacosm.Content.NPCs.Hostile.Minions;
using NeoParacosm.Content.Projectiles.Effect;
using NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Data;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.NPCs.Bosses.Deathbird;

public partial class Deathbird : ModNPC
{
    float wingOutlineScale = 1f;
    float wingScale = 0.9f;
    float darkColorBoost = 0f;

    void DefaultBody()
    {
        body.pos = NPC.Center;
        body.origin = bodyTexture.Size() * 0.5f;
        body.rot = NPC.rotation;
    }
    void DefaultHead()
    {
        head.pos = body.pos - Vector2.UnitY.RotatedBy(NPC.rotation) * bodyTexture.Height() * 0.5f;
        head.origin = headTexture.Size() * 0.5f;
        head.rot = NPC.rotation;
    }
    void DefaultLeftLeg1()
    {
        leftLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(NPC.rotation) * bodyTexture.Height() * 0.4f;
        leftLeg1.origin = new Vector2(leftLeg1Texture.Width(), 0);
        leftLeg1.rot = NPC.rotation;
    }
    void DefaultLeftLeg2()
    {
        leftLeg2.pos = leftLeg1.pos + new Vector2(-leftLeg1Texture.Width() * 0.5f, leftLeg1Texture.Height() * 0.8f).RotatedBy(leftLeg1.rot);
        leftLeg2.origin = new Vector2(leftLeg2Texture.Width(), 0);
        leftLeg2.rot = NPC.rotation;
    }
    void DefaultRightLeg1()
    {
        rightLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(NPC.rotation) * bodyTexture.Height() * 0.4f;
        rightLeg1.origin = Vector2.Zero;
        rightLeg1.rot = NPC.rotation;
    }
    void DefaultRightLeg2()
    {
        rightLeg2.pos = rightLeg1.pos + new Vector2(rightLeg1Texture.Width() * 0.5f, rightLeg1Texture.Height() * 0.8f).RotatedBy(rightLeg1.rot);
        rightLeg2.origin = Vector2.Zero;
        rightLeg2.rot = NPC.rotation;
    }
    void SetDefaultBodyPartValues()
    {
        DefaultBody();
        DefaultHead();
        DefaultLeftLeg1();
        DefaultLeftLeg2();
        DefaultRightLeg1();
        DefaultRightLeg2();
    }
    void BasicMovementAnimation()
    {
        float headRot = MathHelper.Clamp(NPC.velocity.X * 6, -90, 90);
        float bodyRot = MathHelper.Clamp(NPC.velocity.X * 3, -60, 60);
        float legRot = MathHelper.Clamp(NPC.velocity.X * 10, -75, 75);
        float leg2Rot = MathHelper.Clamp(NPC.velocity.X * 10, -90, 90);
        head.rot = Utils.AngleLerp(head.rot, MathHelper.ToRadians(headRot), 1 / 20f);
        body.rot = Utils.AngleLerp(body.rot, MathHelper.ToRadians(bodyRot), 1 / 20f);

        leftLeg1.rot = Utils.AngleLerp(leftLeg1.rot, MathHelper.ToRadians(legRot), 1 / 20f);
        leftLeg2.rot = Utils.AngleLerp(leftLeg2.rot, MathHelper.ToRadians(leg2Rot), 1 / 10f);

        rightLeg1.rot = Utils.AngleLerp(rightLeg1.rot, MathHelper.ToRadians(legRot), 1 / 20f);
        rightLeg2.rot = Utils.AngleLerp(rightLeg2.rot, MathHelper.ToRadians(leg2Rot), 1 / 10f);
    }


    void SetBodyPartPositions()
    {
        body.pos = NPC.Center;
        head.pos = body.pos - Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.5f;
        leftLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.4f;
        leftLeg2.pos = leftLeg1.pos + new Vector2(-leftLeg1Texture.Width() * 0.5f, leftLeg1Texture.Height() * 0.8f).RotatedBy(leftLeg1.rot);
        rightLeg1.pos = body.pos + Vector2.UnitY.RotatedBy(body.rot) * bodyTexture.Height() * 0.4f;
        rightLeg2.pos = rightLeg1.pos + new Vector2(rightLeg1Texture.Width() * 0.5f, rightLeg1Texture.Height() * 0.8f).RotatedBy(rightLeg1.rot);
    }

    Color defaultColor => Color.White * NPC.Opacity;
    bool drawClone = false;
    Vector2 clonePos = Vector2.Zero;
    float cloneOpacity = 0f;
    float cloneScale = 1f;
    void DrawClone(Vector2 pos, float opacityIncrementValue, float scale)
    {
        drawClone = true;
        clonePos = pos;
        cloneOpacity += opacityIncrementValue;
        cloneScale = scale;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.Opacity < 1)
        {
            return false;
        }

        // Setting up positions and origin

        //NPC.rotation += MathHelper.ToRadians(1);
        NPC.rotation = 0;

        // Wings
        var shader = GameShaders.Misc["NeoParacosm:DeathbirdWingShader"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["tolerance"].SetValue(0.5f);
        shader.Shader.Parameters["darkColorBoost"].SetValue(darkColorBoost);
        shader.Shader.Parameters["color"].SetValue(Color.White.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.75f);

        // First the "outline"/afterimage/effect wings
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Vector2 wingLeftPos = body.pos;
        Vector2 wingLeftOrigin = new Vector2(wingsLeftTexture.Frame(1, 5, 0, 0).Width, wingsLeftTexture.Frame(1, 5, 0, 0).Height / 2);
        Vector2 wingRightPos = body.pos;
        Vector2 wingRightOrigin = new Vector2(0, wingsRightTexture.Frame(1, 5, 0, 0).Height / 2);
        Main.EntitySpriteDraw(wingsLeftTexture.Value, wingLeftPos - Main.screenPosition, wingsLeftTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingLeftOrigin, NPC.scale * wingOutlineScale, SpriteEffects.None, 0);
        shader.Shader.Parameters["moveSpeed"].SetValue(-0.75f);
        Main.EntitySpriteDraw(wingsRightTexture.Value, wingRightPos - Main.screenPosition, wingsRightTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingRightOrigin, NPC.scale * wingOutlineScale, SpriteEffects.None, 0);
        Main.spriteBatch.End();

        // Actual wings
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Shader.Parameters["moveSpeed"].SetValue(0.75f);
        Main.EntitySpriteDraw(wingsLeftTexture.Value, wingLeftPos - Main.screenPosition, wingsLeftTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingLeftOrigin, NPC.scale * wingScale, SpriteEffects.None, 0);
        shader.Shader.Parameters["moveSpeed"].SetValue(-0.75f);
        Main.EntitySpriteDraw(wingsRightTexture.Value, wingRightPos - Main.screenPosition, wingsRightTexture.Frame(1, 5, 0, NPC.frame.Y / 200), Color.White, body.rot, wingRightOrigin, NPC.scale * wingScale, SpriteEffects.None, 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


        // Body
        Main.EntitySpriteDraw(bodyTexture.Value, body.pos - Main.screenPosition, null, defaultColor, body.rot, body.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(headTexture.Value, head.pos - Main.screenPosition, null, defaultColor, head.rot, head.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(leftLeg1Texture.Value, leftLeg1.pos - Main.screenPosition, null, defaultColor, leftLeg1.rot, leftLeg1.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(leftLeg2Texture.Value, leftLeg2.pos - Main.screenPosition, null, defaultColor, leftLeg2.rot, leftLeg2.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(rightLeg1Texture.Value, rightLeg1.pos - Main.screenPosition, null, defaultColor, rightLeg1.rot, rightLeg1.origin, NPC.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(rightLeg2Texture.Value, rightLeg2.pos - Main.screenPosition, null, defaultColor, rightLeg2.rot, rightLeg2.origin, NPC.scale, SpriteEffects.None);

        // Clone
        if (drawClone && clonePos != Vector2.Zero)
        {
            Main.EntitySpriteDraw(headTexture.Value, clonePos - Main.screenPosition, null, Color.White * cloneOpacity, 0f, head.origin, cloneScale, SpriteEffects.None);
        }
        return false;
    }

}
