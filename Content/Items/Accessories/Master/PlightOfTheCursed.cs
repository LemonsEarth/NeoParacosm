using NeoParacosm.Content.Projectiles.Effect;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Accessories.Master;

public class PlightOfTheCursed : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.master = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<PlightOfTheCursedPlayer>().PlightOfTheCursed = true;
    }
}

public class PlightOfTheCursedPlayer : ModPlayer
{
    public bool PlightOfTheCursed { get; set; } = false;

    public override void ResetEffects()
    {
        PlightOfTheCursed = false;
    }

    void PlightOfTheCursedEffect(ref Player.HurtModifiers modifiers)
    {
        if (PlightOfTheCursed)
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
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ProjectileType<PlightOfTheCursedProjectile>(), 1, 1, Main.myPlayer, ai1: 0.5f);
            }
        }
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        PlightOfTheCursedEffect(ref modifiers);
    }

    public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
    {
        PlightOfTheCursedEffect(ref modifiers);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (PlightOfTheCursed)
        {
            target.AddBuff(BuffType<PlightOfTheCursedDebuff>(), 180);
        }
    }
}

public class PlightOfTheCursedDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = false;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}

public class PlightOfTheCursedNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    bool avariceLootBonus = false;
    public override void OnKill(NPC npc)
    {
        if (npc.SpawnedFromStatue) return;

        if (Main.rand.NextBool(5) && !avariceLootBonus && npc.HasBuff(BuffType<PlightOfTheCursedDebuff>()))
        {
            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ProjectileType<PlightOfTheCursedProjectile>(), 1, 1, Main.myPlayer);

            avariceLootBonus = true;
            for (int i = 0; i < 4; i++)
            {
                npc.NPCLoot();
            }
        }
    }
}

public class PlightOfTheCursedDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.SkeletronHead;
    }

    public override void OnKill(NPC npc)
    {

    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(Condition.InMasterMode.ToDropCondition(ShowItemDropInUI.Always), ItemType<PlightOfTheCursed>()));
    }
}