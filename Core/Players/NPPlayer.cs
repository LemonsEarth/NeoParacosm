using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Collections.Generic;

namespace NeoParacosm.Core.Players;

public class NPPlayer : ModPlayer
{
    int timer = 0;
    public bool NoMusic { get; set; } = false;

    public static HashSet<int> BlockProjectiles { get; set; } = new HashSet<int>();
    public static List<Projectile> BlockProjectileInstances { get; set; } = new List<Projectile>();

    public override void ResetEffects()
    {
        NoMusic = false;

        BlockProjectileInstances.RemoveAll(p => !p.active);
    }

    public override void SetStaticDefaults()
    {
        BlockProjectiles = new HashSet<int>() {
            ProjectileType<WallP>(),
            ProjectileType<CorruptPillar>(),
        };
    }

    public override void PreUpdateMovement()
    {
        foreach (var projectile in Main.ActiveProjectiles)
        {
            // If walkable projectiles are expanded upon, some HashSet should probably be made for them
            if (!BlockProjectiles.Contains(projectile.type))
            {
                continue;
            }
            BlockProjectileCollision(projectile);
        }
    }

    /// <summary>
    /// Checks if the player is standing on a projectile, handles snapping and movement.
    /// Doesn't handle horizontal collision, and doesn't prevent the player from moving into the projectile from below.
    /// Used for "moving platforms".
    /// </summary>
    /// <param name="projectile">A collideable projectile</param>
    public void PlatformProjectileCollision(Projectile projectile)
    {
        // TODO: Fix weird vertical snap when moving over tiles/platforms
        if (!projectile.Hitbox.IntersectsExact(Player.Hitbox))
        {
            return;
        }
        if (Player.velocity.Y > 0)
        {
            Player.velocity.Y = 0;

            // Prevents snapping/twitching when the projectile is moving downwards slowly by adding downward velocity to the player
            float extraYVelocity = MathHelper.Clamp(projectile.velocity.Y, 0, Player.maxFallSpeed);
            Player.Bottom = new Vector2(Player.Bottom.X, projectile.Top.Y + extraYVelocity);
        }
    }

    /// <summary>
    /// Checks if the player is standing or running into a projectile, handles snapping and movement.
    /// Used for "moving blocks".
    /// </summary>
    /// <param name="projectile">A collideable projectile</param>
    public void BlockProjectileCollision(Projectile projectile)
    {
        if (!projectile.Hitbox.IntersectsExact(Player.Hitbox))
        {
            return;
        }

        if (Player.Center.X > projectile.position.X && Player.Center.X < projectile.position.X + projectile.width)
        {
            if (Player.Center.Y <= projectile.Center.Y && Player.velocity.Y >= 0) // Falling onto block from above
            {
                Player.velocity.Y = 0;

                // Prevents snapping when the projectile is moving downwards by adding downward velocity to the player
                float extraYVelocity = MathHelper.Clamp(projectile.velocity.Y, 0, Player.maxFallSpeed);
                Player.Bottom = new Vector2(Player.Bottom.X, projectile.Top.Y + extraYVelocity);
            }
            else if(Player.Center.Y > projectile.Center.Y && Player.velocity.Y <= 0) // Rising into block from below
            {
                Player.velocity.Y = 0;
                Player.Top = new Vector2(Player.Top.X, projectile.Bottom.Y);
            }
        }

        if (Player.Top.Y < projectile.Bottom.Y - 2 && Player.Bottom.Y > projectile.Top.Y + 2)
        {
            if (Player.Center.X <= projectile.Center.X && Player.velocity.X >= 0) // Moving into block from the left
            {
                Player.velocity.X = 0;
                Player.Right = new Vector2(projectile.Left.X, Player.Right.Y);
            }
            else if (Player.Center.X > projectile.Center.X && Player.velocity.X <= 0) // Moving into block from the left
            {
                Player.velocity.X = 0;
                Player.Left = new Vector2(projectile.Right.X, Player.Left.Y);
            }
        }
    }

    public override void PostUpdate()
    {
        //Dust.QuickDust(new Point(Main.dungeonX, Main.dungeonY), Color.White);
        int researcherIndex = NPC.FindFirstNPC(NPCType<Researcher>());
        if (researcherIndex >= 0 && Main.npc[researcherIndex].Distance(Player.Center) > 500)
        {
            AscensionUISystem UISystem = GetInstance<AscensionUISystem>();
            UISystem.HideUI();
        }
        timer++;
    }

    /// <summary>
    /// Checks if there are tiles above the player which would prevent them from moving upward
    /// </summary>
    public bool TopCollision()
    {
        return Collision.SolidTiles(Player.Top + new Vector2(-Player.width / 2, -16), 16, 1);
    }
}
