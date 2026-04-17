using NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Ice;

public class HailfireSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 16;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = Main.MouseWorld;
        if (LemonUtils.NotClient())
        {
            for (int i = 0; i < 7; i++)
            {
                Vector2 dir = player.DirectionTo(Main.MouseWorld).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
                Projectile.NewProjectile(Item.GetSource_FromAI(), player.Center,
                    dir * Main.rand.NextFloat(3, 7) * player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Ice] - Vector2.UnitY * 2,
                    ProjectileType<Hailfireball>(),
                    (int)(GetDamage(player) * 0.25f),
                    1f,
                    player.whoAmI);
            }
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 6;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.Blue;
        SpellElements = [SpellElement.Ice];
    }
}

public class HailfireSpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.IceElemental;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<HailfireSpell>(), 20, 1, 1));
    }
}

public class HailfireSpellChestItem : ModSystem
{
    public override void PostWorldGen()
    {
        LemonUtils.GenerateItemInChest(ItemType<HailfireSpell>(), 11, 6, true);
    }
}