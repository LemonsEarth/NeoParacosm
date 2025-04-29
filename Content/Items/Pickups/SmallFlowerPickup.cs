using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.GoodBuffs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Items.Pickups
{
    public class SmallFlowerPickup : ModItem
    {
        int frameNum = 0;
        float rot = 0;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.IsAPickup[Type] = true;
            ItemID.Sets.ItemSpawnDecaySpeed[Type] = 40;
            ItemID.Sets.ItemNoGravity[Type] = true;
            //Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(100000, 3));
        }

        public override void OnSpawn(IEntitySource source)
        {
            frameNum = Main.rand.Next(0, 3);
            rot = MathHelper.ToRadians(Main.rand.NextFloat(0, 360));
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Item.type, out Texture2D texture, out Rectangle frame);
            Vector2 drawOrigin = frame.Size() / 2;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);
            spriteBatch.Draw(texture, drawPosition, texture.Frame(1, 3, 0, frameNum), lightColor, rot, drawOrigin, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.accessory = true;
            Item.value = Item.buyPrice(0, 0);
            Item.rare = ItemRarityID.White;
        }

        public override bool OnPickup(Player player)
        {
            player.AddBuff(ModContent.BuffType<ForestCrestBuff>(), 300);
            SoundEngine.PlaySound(SoundID.Grab, player.Center);
            return false;
        }
    }
}
