using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Items.Pickups;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Content.Projectiles.None;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Players;

public class NPAcessoryPlayer : ModPlayer
{
    int timer = 0;

    public bool roundShield { get; set; } = false;
    public bool forestCrest { get; set; } = false;
    int forestCrestPickupCooldown = 0;

    public bool corruptedLifeCrystal { get; set; } = false;
    public bool skullOfAvarice { get; set; } = false;

    public List<Projectile> CrimsonTendrils { get; set; } = new List<Projectile>();

    public override void ResetEffects()
    {
        roundShield = false;
        forestCrest = false;
        corruptedLifeCrystal = false;
        skullOfAvarice = false;

        CrimsonTendrils.RemoveAll(p => !p.active);
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (roundShield && !Player.HasBuff(ModContent.BuffType<KnockbackCooldown>()))
        {
            Player.AddBuff(ModContent.BuffType<KnockbackCooldown>(), 1800);
        }

        if (Player.HasBuff(ModContent.BuffType<BaneflyHiveBuff>()))
        {
            Player.AddBuff(BuffID.CursedInferno, 120);
        }
    }

    void SkullOfAvariceEffect(ref Player.HurtModifiers modifiers)
    {
        if (skullOfAvarice)
        {
            if (Main.rand.NextBool(3))
            {
                modifiers.FinalDamage *= 2f;
                foreach (int buff in Player.buffType)
                {
                    if (buff > 0 && !Main.debuff[buff])
                    {
                        Player.ClearBuff(buff);
                    }
                }
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<SkullOfAvariceProjectile>(), 1, 1, Main.myPlayer);
            }
        }
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        SkullOfAvariceEffect(ref modifiers);
    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        SkullOfAvariceEffect(ref modifiers);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (skullOfAvarice)
        {
            target.AddBuff(ModContent.BuffType<SkullOfAvariceDebuff>(), 180);
        }
    }

    public override void NaturalLifeRegen(ref float regen)
    {
        if (corruptedLifeCrystal)
        {
            regen *= 0;
        }
    }

    public override void PostUpdateEquips()
    {
        if (roundShield && !Player.HasBuff(ModContent.BuffType<KnockbackCooldown>())) // On Hurt code is in Main
        {
            Player.noKnockback = true;
        }

        if (corruptedLifeCrystal)
        {
            Player.statLifeMax2 += 60;
        }

        if (forestCrestPickupCooldown > 0) forestCrestPickupCooldown--;

        if (Player.HasBuff(ModContent.BuffType<CrimsonTendrilBuff>()))
        {
            if (CrimsonTendrils.Count < 3)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    int damage = Main.hardMode ? 60 : 30;
                    Projectile p = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<CrimsonTendrilFriendly>(), (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(damage), 2f, Player.whoAmI);
                    CrimsonTendrils.Add(p);
                }
            }
        }

        if (Player.HasBuff(ModContent.BuffType<BaneflyHiveBuff>()))
        {
            if (timer % 300 == 0 && Main.myPlayer == Player.whoAmI)
            {
                for (int i = 0; i < 4; i++)
                {
                    int damage = Main.hardMode ? 20 : 10;
                    Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Banefly>(), (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(damage), 0.5f, Player.whoAmI);
                }
            }
        }

        timer++;
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {
        if (corruptedLifeCrystal)
        {
            healValue += 20;
        }
    }

    public override void OnHitAnything(float x, float y, Entity victim)
    {
        if (victim is NPC npc && forestCrest)
        {
            if (Main.rand.NextBool(10) && forestCrestPickupCooldown == 0)
            {
                Item item = Player.QuickSpawnItemDirect(Player.GetSource_OnHit(victim, "Forest Crest Hit"), ModContent.ItemType<SmallFlowerPickup>(), 1);
                item.position = new Vector2(x, y);
                NetMessage.SendData(MessageID.SyncItem, number: item.whoAmI, number2: 0);
                forestCrestPickupCooldown = 30;
            }
        }
    }
}
