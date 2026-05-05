using NeoParacosm.Content.Items.Accessories.Combat.Defensive;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Earth;

public class ExplosionSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 24;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            SoundEngine.PlaySound(SoundID.Item7, player.Center);

            Projectile proj = Projectile.NewProjectileDirect(
                Item.GetSource_FromAI(),
                player.Center,
                Vector2.Zero,
                ProjectileType<BombExplosion>(),
                (int)(GetDamage(player)),
                1f,
                player.whoAmI,
                ai1: 5 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2));
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 30;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 15);
        Item.rare = ItemRarityID.Blue;
        SpellElements = [SpellElement.Earth, SpellElement.Fire];
    }
}

public class ExplosionSpellShopNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Demolitionist;
    }

    public override void ModifyShop(NPCShop shop)
    {
        shop.Add(ItemType<ExplosionSpell>());
    }
}