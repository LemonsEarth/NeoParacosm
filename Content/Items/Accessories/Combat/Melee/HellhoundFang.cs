using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class HellhoundFang : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 44;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<HellhoundFangPlayer>().Active = true;
    }
}

public class HellhoundFangPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active)
        {
            modifiers.ArmorPenetration += 10;
        }

    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && proj.CountsAsTrueMelee())
        {
            modifiers.ArmorPenetration += 10;
        }
    }
}

public class HellhoundFangNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Hellhound;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsPumpkinMoon(), ItemType<HellhoundFang>(), 25));
    }
}
