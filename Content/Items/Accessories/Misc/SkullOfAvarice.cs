using NeoParacosm.Content.Projectiles.Effect;

namespace NeoParacosm.Content.Items.Accessories.Misc;

public class SkullOfAvarice : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<SkullOfAvaricePlayer>().skullOfAvarice = true;
    }
}

public class SkullOfAvaricePlayer : ModPlayer
{
    public bool skullOfAvarice { get; set; } = false;

    public override void ResetEffects()
    {
        skullOfAvarice = false;
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
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<SkullOfAvariceProjectile>(), 1, 1, Main.myPlayer);
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
            target.AddBuff(BuffType<SkullOfAvariceDebuff>(), 180);
        }
    }
}

public class SkullOfAvariceDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}

public class SkullOfAvariceNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    bool avariceLootBonus = false;
    public override void OnKill(NPC npc)
    {
        if (npc.SpawnedFromStatue) return;

        if (Main.rand.NextBool(5) && !avariceLootBonus && npc.HasBuff(BuffType<SkullOfAvariceDebuff>()))
        {
            avariceLootBonus = true;
            for (int i = 0; i < 4; i++)
            {
                npc.NPCLoot();
            }
        }
    }
}