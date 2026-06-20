using NeoParacosm.Content.Items.Materials;
using Terraria.Audio;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.FrozenFossil;

[AutoloadEquip(EquipType.Head)]
public class FrozenSkull : ModItem
{
    static readonly float critChanceBoost = 6;
    static readonly float damageBoost = 2;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critChanceBoost);

    public override void SetStaticDefaults()
    {
        //ArmorIDs.Head.Sets.
    }

    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.defense = 6;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(0, 3, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Generic).Flat += damageBoost;
        player.GetCritChance(DamageClass.Generic) += critChanceBoost;
        player.GetModPlayer<FrozenSkullPlayer>().Active = true;
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<FrigidFossil>(), 10);
        recipe.AddTile(TileID.IceMachine);
        recipe.Register();
    }
}

public class FrozenSkullPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (!Active)
        {
            return;
        }
        SoundEngine.PlaySound(SoundID.Item28, Player.Center);
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.CanBeChasedBy() && npc.DistanceSQ(Player.Center) < 300 * 300)
            {
                npc.AddBuff(BuffID.Frostburn, 300);
            }
        }
    }
}

