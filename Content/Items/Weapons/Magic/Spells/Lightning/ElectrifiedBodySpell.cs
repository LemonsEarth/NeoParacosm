using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Lightning;

public class ElectrifiedBodySpell : BaseSpell
{
    public override int AttackCooldown => 30;
    public override int ManaCost => 0;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        player.AddBuff(BuffType<ElectrifiedBodyBuff>(), ((int)(20 * 60 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Lightning, 2))));
        SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (0.3f, 0.4f) }, player.Center);
        for (int j = 0; j < 6; j++)
        {
            Dust.NewDustDirect(player.RandomPos(-player.width / 2, -player.height / 2), 2, 2, DustType<StreakDust>(), Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6), Scale: Main.rand.NextFloat(0.5f, 0.75f)).noGravity = true;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 0;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 4);
        Item.rare = ItemRarityID.LightRed;
        SpellElements = [SpellElement.Lightning];
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {

    }
}

public class ElectrifiedBodySpellDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.AngryNimbus;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<ElectrifiedBodySpell>(), 20, 1, 1));
    }
}