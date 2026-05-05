using NeoParacosm.Content.Items.Weapons.Magic.Spells.Nature;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Defensive;

public class BombVest : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetDefaults()
    {
        Item.width = 64;  
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 15);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<BombVestPlayer>().Active = true;
    }

}

public class BombVestShopNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Demolitionist;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<BombVest>(), Condition.DownedEyeOfCthulhu);
    }
}

public class BombVestPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
    {
        if (Active && ProjectileID.Sets.Explosive[proj.type])
        {
            modifiers.FinalDamage *= 0.25f;
            modifiers.Knockback *= 0f;
        }
    }

    public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
    {
        if (Active && ProjectileID.Sets.Explosive[proj.type])
        {
            Player.AddBuff(BuffType<ExplosiveDamageBuff>(), 1800);
        }
    }
}

public class ExplosiveDamageBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetDamage(DamageClass.Generic) += 12f / 100f;
    }
}