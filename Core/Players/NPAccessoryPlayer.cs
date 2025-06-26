using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Items.Pickups;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using System.Collections.Generic;

namespace NeoParacosm.Core.Players;

public class NPAcessoryPlayer : ModPlayer
{
    public bool roundShield { get; set; } = false;
    public bool forestCrest { get; set; } = false;
    int forestCrestPickupCooldown = 0;

    public bool corruptedLifeCrystal { get; set; } = false;

    public List<Projectile> CrimsonTendrils { get; set; } = new List<Projectile>();

    public override void ResetEffects()
    {
        roundShield = false;
        forestCrest = false;
        corruptedLifeCrystal = false;

        CrimsonTendrils.RemoveAll(p => !p.active);
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (roundShield && !Player.HasBuff(ModContent.BuffType<KnockbackCooldown>()))
        {
            Player.AddBuff(ModContent.BuffType<KnockbackCooldown>(), 1800);
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
                    Projectile p = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<CrimsonTendrilFriendly>(), (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(30), 2f, Player.whoAmI);
                    CrimsonTendrils.Add(p);
                }
            }
        }
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
