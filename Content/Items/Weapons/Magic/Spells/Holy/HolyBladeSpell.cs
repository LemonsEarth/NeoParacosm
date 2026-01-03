using NeoParacosm.Content.Buffs.GoodBuffs;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Holy;

public class HolyBladeSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 30;
    public override Vector2 TargetVector { get; set; } = Main.MouseWorld;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        player.AddBuff(BuffType<HolyBladeBuff>(), (int)(40 * 60 * player.GetElementalExpertiseBoost(SpellElement.Holy)));
        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { PitchRange = (0.4f, 0.5f) }, player.Center);
        for (int j = 0; j < 6; j++)
        {
            Vector2 randVector = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.OrangeStainedGlass, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.0f, 1.4f)).noGravity = true;
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.GemTopaz, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.0f, 1.4f)).noGravity = true;
        }
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

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        
    }
}