using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class GloveOfRetaliation : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 32;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 3);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<GloveOfRetaliationPlayer>().gloveOfRetaliation = true;
    }
}

public class GloveOfRetaliationPlayer : ModPlayer
{
    public bool gloveOfRetaliation { get; set; } = false;

    public override void ResetEffects()
    {
        gloveOfRetaliation = false;
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (gloveOfRetaliation && hurtInfo.Damage > Player.statLifeMax2 * 0.1f)
        {
            Player.AddBuff(BuffType<GloveOfRetaliationBuff>(), 60);
        }
    }
}

public class GloveOfRetaliationBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetDamage(DamageClass.Melee) += 300f / 100f;
    }
}
