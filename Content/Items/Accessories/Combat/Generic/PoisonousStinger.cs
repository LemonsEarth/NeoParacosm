using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class PoisonousStinger : ModItem
{
    readonly float dmgBoost = 8f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(dmgBoost);
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 40;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.HasAnyPoisonDebuff())
        {
            player.GetDamage(DamageClass.Generic) += dmgBoost / 100f;
        }

        player.GetModPlayer<PoisonousStingerPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Stinger, 3);
        recipe.AddIngredient(ItemID.JungleSpores, 6);
        recipe.AddIngredient(ItemID.Vine, 2);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}

public class PoisonousStingerPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }
}

public class PoisonousStingerItem : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return entity.healLife > 0;
    }

    public override bool? UseItem(Item item, Player player)
    {
        if (player.GetModPlayer<PoisonousStingerPlayer>().Active)
        {
            player.AddBuff(BuffID.Poisoned, 45 * 60);
        }

        return null;
    }
}

public class PoisonousStingerDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Hornet;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<PoisonousStinger>(), 50));
    }
}
