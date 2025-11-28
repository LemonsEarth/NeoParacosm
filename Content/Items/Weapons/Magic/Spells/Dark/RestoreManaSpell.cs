using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Dark;

public class RestoreManaSpell : BaseSpell
{
    public override int AttackCooldown => 10;
    public override int ManaCost => 0;
    public override Vector2 TargetVector { get; set; } = -Vector2.UnitY;

    public override void SpellAction(Player player)
    {
        TargetVector = player.Center - Vector2.UnitY * 100;
        float boostAdjusted = 1 + (player.GetElementalExpertiseBoost(SpellElement.Dark) - 1) * 3;
        player.RestoreMana((int)(30 * MathF.Max(boostAdjusted, 0)));
        player.statLife -= 7;
        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { PitchRange = (0.3f, 0.5f) }, player.Center);
        for (int j = 0; j < 3; j++)
        {
            Vector2 randVector = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.GemSapphire, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.0f, 1.4f)).noGravity = true;
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.IceTorch, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.0f, 1.4f)).noGravity = true;
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
        SpellElements = [SpellElement.Dark];
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        
    }
}