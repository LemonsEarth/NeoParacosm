using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Hostile.Misc;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;

namespace NeoParacosm.Content.NPCs.Hostile.Underground;

public class ExplosivePot : ModNPC
{
    int AITimer = 0;
    bool falling = false;
    int tileX = -1;
    int tileY = -1;
    ref float RandomDistanceFromTile => ref NPC.ai[0];
    int potType = 0;

    string RopePath => Texture + "Rope";
    static Asset<Texture2D> ropeTexture;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.TrailCacheLength[NPC.type] = 5;
        NPCID.Sets.TrailingMode[NPC.type] = 2;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        NPC.DontDropAnything();
        NPCID.Sets.DontDoHardmodeScaling[NPC.type] = true;
        ropeTexture = Request<Texture2D>(RopePath);
    }

    public override void SetDefaults()
    {
        NPC.width = 32;
        NPC.height = 32;
        NPC.lifeMax = 99000;
        NPC.defense = 9999;
        NPC.SuperArmor = true;
        NPC.damage = 0;
        NPC.HitSound = SoundID.Dig;
        NPC.DeathSound = SoundID.Shatter;
        NPC.value = 0;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
        NPC.knockBackResist = 0f;
        NPC.hide = true;
        NPC.ShowNameOnHover = false;
        NPC.chaseable = false;
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {

    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>()
        {
            //BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.,
        });
    }

    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        falling = true;
        NPC.velocity.Y = 0.2f;
        NPC.velocity.X = hit.HitDirection * Main.rand.NextFloat(1, 2);
        AITimer = 0;
    }

    public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        falling = true;
        NPC.velocity.Y = 0.2f;
        NPC.velocity.X = hit.HitDirection * Main.rand.NextFloat(1, 2);
        AITimer = 0;
    }

    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {

    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            // find tile to hang on to
            Point16 NPCTileCoords = NPC.Center.ToTileCoordinates16();
            tileX = NPCTileCoords.X;
            tileY = NPCTileCoords.Y;
            int maxSearchHeight = 60;
            bool foundTile = false;
            for (int y = 0; y < maxSearchHeight; y++)
            {
                tileY -= 1;
                Tile tile = Framing.GetTileSafely(new Point16(tileX, tileY));
                if (!tile.HasTile || tile.IsActuated || !Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])
                {
                    continue;
                }
                switch (tile.TileType)
                {
                    case TileID.HardenedSand or TileID.CorruptHardenedSand or TileID.HallowHardenedSand or TileID.CrimsonHardenedSand
                    or TileID.Sandstone or TileID.CorruptSandstone or TileID.HallowSandstone or TileID.CrimsonSandstone:
                        potType = 1;
                        break;
                    case TileID.SnowBlock or TileID.IceBlock or TileID.CorruptIce or TileID.HallowedIce or TileID.FleshIce:
                        potType = 2;
                        break;
                    case TileID.JungleGrass or TileID.Mud or TileID.LihzahrdBrick:
                        potType = 3;
                        break;
                    default:
                        potType = 0;
                        break;
                }
                foundTile = true;
                break;
            }

            if (!foundTile)
            {
                NPC.active = false;
                return;
            }

            if (LemonUtils.NotClient())
            {
                RandomDistanceFromTile = Main.rand.NextFloat(32, 90);
            }
            NPC.netUpdate = true;

        }
        Tile tile2 = Framing.GetTileSafely(new Point16(tileX, tileY));
        if ((!tile2.HasTile || tile2.IsActuated || !Main.tileSolid[tile2.TileType] || Main.tileSolidTop[tile2.TileType]) && !falling)
        {
            falling = true;
            NPC.velocity.Y = 0.2f;
            AITimer = 0;
        }

        if (!falling)
        {
            NPC.Center = new Vector2(tileX * 16, tileY * 16 + RandomDistanceFromTile);
            NPC.noGravity = true;
            NPC.velocity = Vector2.Zero;
        }
        else
        {
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            if (NPC.velocity.Y == 0 || NPC.wet)
            {
                NPC.StrikeInstantKill();
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickProj(NPC, NPC.Center, Vector2.Zero, ProjectileType<BombExplosion>(), 200);
                }
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                return;
            }
        }
        NPC.rotation = MathHelper.ToRadians(AITimer * LemonUtils.Sign(NPC.velocity.X, 1) * NPC.velocity.Y);
        AITimer++;
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        return false;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = potType * frameHeight;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (AITimer == 0)
        {
            return false;
        }
        if (!falling)
        {
            Vector2 ropePos = NPC.Center;
            float distanceToTile = MathF.Abs(ropePos.Y - tileY * 16);
            while (distanceToTile > 0)
            {
                Main.EntitySpriteDraw(ropeTexture.Value, ropePos - Main.screenPosition, null, drawColor, NPC.rotation, ropeTexture.Size() * 0.5f, NPC.scale, SpriteEffects.None);
                ropePos.Y -= ropeTexture.Height();
                distanceToTile -= ropeTexture.Height();
            }
        }


        Texture2D texture = TextureAssets.Npc[NPC.type].Value;
        Rectangle sourceRect = NPC.frame;
        Vector2 drawOrigin = sourceRect.Size() * 0.5f;
        Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition, sourceRect, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

        return false;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.Player.ZoneRockLayerHeight || spawnInfo.Player.ZoneDirtLayerHeight ? 0.1f : 0f;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, minimumDropped: 1, maximumDropped: 3));
    }

    public override bool? CanFallThroughPlatforms()
    {
        return true;
    }
}
