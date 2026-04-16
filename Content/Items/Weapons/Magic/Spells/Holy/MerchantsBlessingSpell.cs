using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Items.Accessories.Combat.Defensive;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;

public class MerchantsBlessingSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 40;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        player.AddBuff(BuffType<MerchantsBlessingBuff>(), (int)(30 * 60 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Holy, 4)));
        SoundEngine.PlaySound(SoundID.CoinPickup with { PitchRange = (0.1f, 0.3f) }, player.Center);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Holy];
    }
}

public class MerchantsBlessingSpellShopNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Merchant;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<MerchantsBlessingSpell>(), Condition.LanternNight);
    }
}