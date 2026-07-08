using NeoParacosm.Content.Buffs.Debuffs;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;

public class InvisibleManSpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 20;
    public override Vector2 GetTargetVector(Player player) { return player.Center - Vector2.UnitY * 100; }

    public override void SpellAction(Player player)
    {

        player.AddBuff(BuffID.Invisibility, (int)(20 * 60 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Dark, 1.5f)));
        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { PitchRange = (-0.6f, -0.3f) }, player.Center);
        for (int j = 0; j < 6; j++)
        {
            Vector2 randVector = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.Granite, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.0f, 1.4f), newColor: Color.Black).noGravity = true;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.sellPrice(gold: 2);
        Item.rare = ItemRarityID.Blue;
        SpellElements = [SpellElement.Dark];
    }
}

public class InvisibleManSpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Wraith;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<InvisibleManSpell>(), 25, 1, 1));
    }
}