using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Content.Projectiles.Friendly.Melee.HeldProjectiles;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Collections.Generic;
using Terraria.Audio;

namespace NeoParacosm.Core.Players;

public class DeflectPlayer : ModPlayer
{
    public int BlockingTimer { get; set; } = 0;
    public bool Blocking => BlockingTimer > 0;

    public void StartBlocking(int duration)
    {
        BlockingTimer = duration;
    }

    public override void ResetEffects()
    {

    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        // Deflect with angle in mind
        /*Vector2 playerToMouse = Player.DirectionTo(Main.MouseWorld);
        Vector2 playerToNPC = Player.DirectionTo(npc.Center);
        float cosTheta = Vector2.Dot(playerToMouse, playerToNPC);
        if (Blocking && cosTheta > 0.7f)
        {
            modifiers.FinalDamage *= 0;
            modifiers.Knockback *= 0;
            modifiers.DisableSound();
        }*/
        if (Blocking)
        {
            modifiers.FinalDamage *= 0;
            modifiers.Knockback *= 0;
            modifiers.DisableSound();
        }
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        // Deflect with angle in mind
        /*Vector2 playerToMouse = Player.DirectionTo(Main.MouseWorld);
        Vector2 playerToProj = Player.DirectionTo(proj.Center);
        float cosTheta = Vector2.Dot(playerToMouse, playerToProj);
        if (Blocking && cosTheta > 0.7f)
        {
            modifiers.FinalDamage *= 0;
            modifiers.Knockback *= 0;
            modifiers.DisableSound();
        }*/
        if (Blocking)
        {
            modifiers.FinalDamage *= 0;
            modifiers.Knockback *= 0;
            modifiers.DisableSound();
        }
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        // Deflect with angle in mind
        /*Vector2 playerToMouse = Player.DirectionTo(Main.MouseWorld);
        Vector2 playerToNPC = Player.DirectionTo(npc.Center);
        float cosTheta = Vector2.Dot(playerToMouse, playerToNPC);
        if (Blocking && cosTheta > 0.7f)
        {
            SoundEngine.PlaySound(ParacosmSFX.SwordDeflect with { Volume = 0.8f, PitchRange = (0.4f, 0.6f) }, Player.Center);
            LemonUtils.DustBurst(4, Player.Center, DustType<StreakDust>(), 12, 12, 1, 1.2f);
            Player.AddBuff(BuffID.ParryDamageBuff, 120);
        }*/
        if (Blocking)
        {
            SoundEngine.PlaySound(ParacosmSFX.SwordDeflect with { Volume = 0.8f, PitchRange = (0.4f, 0.6f) }, Player.Center);
            LemonUtils.DustBurst(4, Player.Center, DustType<StreakDust>(), 12, 12, 1, 1.2f);
            Player.AddBuff(BuffID.ParryDamageBuff, 120);

            /*foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner == Player.whoAmI && projectile.ModProjectile is CursebindersUnravelHeldProjBlocking deflectSword)
                {
                    deflectSword.Restart();
                }
            }*/
        }
    }

    public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
    {
        // Deflect with angle in mind
        /*Vector2 playerToMouse = Player.DirectionTo(Main.MouseWorld);
        Vector2 playerToProj = Player.DirectionTo(proj.Center);
        float cosTheta = Vector2.Dot(playerToMouse, playerToProj)
        if (Blocking && cosTheta > 0.5f)
        {
            SoundEngine.PlaySound(ParacosmSFX.SwordDeflect with { Volume = 0.8f, PitchRange = (0.4f, 0.6f) }, Player.Center);
            LemonUtils.DustBurst(4, Player.Center, DustType<StreakDust>(), 5, 5, 1, 1.2f);
            Player.AddBuff(BuffID.ParryDamageBuff, 120);
        }*/
        if (Blocking)
        {
            SoundEngine.PlaySound(ParacosmSFX.SwordDeflect with { Volume = 0.8f, PitchRange = (0.4f, 0.6f) }, Player.Center);
            LemonUtils.DustBurst(4, Player.Center, DustType<StreakDust>(), 5, 5, 1, 1.2f);
            Player.AddBuff(BuffID.ParryDamageBuff, 120);

           /* foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner == Player.whoAmI && projectile.ModProjectile is CursebindersUnravelHeldProjBlocking deflectSword)
                {
                    deflectSword.Restart();
                }
            }*/
        }
    }

    public override void PreUpdate()
    {
        if (BlockingTimer > 0)
        {
            BlockingTimer--;
        }
    }
}
