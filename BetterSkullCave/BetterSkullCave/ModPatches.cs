using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Locations;

namespace BetterSkullCave
{
    /// <summary>All Harmony patches applied by the mod.</summary>
    internal static class ModPatches
    {
        /// <summary>Fast accessor for <see cref="MineShaft"/>'s private <c>lastLevelsDownFallen</c> field.</summary>
        private static readonly AccessTools.FieldRef<MineShaft, int> LastLevelsDownFallen =
            AccessTools.FieldRefAccess<MineShaft, int>("lastLevelsDownFallen");

        /// <summary>Register all patches.</summary>
        public static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.drawAboveAlwaysFrontLayer)),
                postfix: new HarmonyMethod(typeof(ModPatches), nameof(AfterDrawAboveAlwaysFrontLayer))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.getTreasureRoomItem)),
                postfix: new HarmonyMethod(typeof(ModPatches), nameof(AfterGetTreasureRoomItem))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.enterMineShaft)),
                postfix: new HarmonyMethod(typeof(ModPatches), nameof(AfterEnterMineShaft))
            );
        }

        // === Floor counter color ===

        /// <summary>
        /// Draw the Skull Cavern floor counter again in a light, readable color on top of the
        /// original purple text.
        /// </summary>
        private static void AfterDrawAboveAlwaysFrontLayer(MineShaft __instance, SpriteBatch b)
        {
            ModConfig config = ModEntry.Config;
            if (__instance.getMineArea() != 121 || __instance.isSideBranch() || Game1.game1.takingMapScreenshot)
                return;

            // The game displays the floor number as (mineLevel - 120), so floor 121 shows "1".
            string text = (__instance.mineLevel - 120).ToString();

            Rectangle titleSafeArea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
            int height = SpriteText.getHeightOfString(text);

            Color color = new(
                System.Math.Clamp(config.Red, 0, 255),
                System.Math.Clamp(config.Green, 0, 255),
                System.Math.Clamp(config.Blue, 0, 255)
            );

            // Redraw the same dark scroll background + the floor number, but with the configured color.
            SpriteText.drawString(
                b,
                text,
                titleSafeArea.Left + 16,
                titleSafeArea.Top + 16,
                999999,
                -1,
                height,
                1f,
                1f,
                junimoText: false,
                2,
                "",
                color
            );
        }

        // === Treasure replacement (integrated from NoHatTreasureSkull) ===

        private enum TreasureKind { Bomb, Machine, Medicine, Sapling, Seed }

        /// <summary>
        /// Replace a hat treasure (category -95) with a configurable item, mirroring the
        /// NoHatTreasureSkull mod.
        /// </summary>
        private static void AfterGetTreasureRoomItem(ref Item __result)
        {
            ModConfig config = ModEntry.Config;
            if (!config.EnableTreasureReplacement || __result.Category != -95)
                return;

            List<TreasureKind> kinds = new();
            if (config.EnableBomb)
                kinds.Add(TreasureKind.Bomb);
            if (config.EnableMachine)
                kinds.Add(TreasureKind.Machine);
            if (config.EnableMedicine)
                kinds.Add(TreasureKind.Medicine);
            if (config.EnableSapling)
                kinds.Add(TreasureKind.Sapling);
            if (config.EnableSeed)
                kinds.Add(TreasureKind.Seed);

            if (kinds.Count == 0)
                return;

            switch (kinds[Game1.random.Next(kinds.Count)])
            {
                case TreasureKind.Bomb:
                    // Cherry Bomb (286) or Bomb (287).
                    __result = ItemRegistry.Create("(O)" + Game1.random.Next(286, 288), Game1.random.Next(1, 5) * 5);
                    break;

                case TreasureKind.Machine:
                    // Crystalarium (21), Seed Maker (25), Auto Grabber (165), Auto Petter (272).
                    int[] machines = { 21, 25, 165, 272 };
                    __result = ItemRegistry.Create("(BC)" + machines[Game1.random.Next(machines.Length)]);
                    break;

                case TreasureKind.Medicine:
                    // Life Elixir (773), Energy Tonic (349).
                    int[] medicine = { 773, 349 };
                    __result = ItemRegistry.Create("(O)" + medicine[Game1.random.Next(medicine.Length)], Game1.random.Next(2, 5));
                    break;

                case TreasureKind.Sapling:
                    __result = ItemRegistry.Create("(O)" + Game1.random.Next(628, 634));
                    break;

                case TreasureKind.Seed:
                    __result = ItemRegistry.Create("(O)" + Game1.random.Next(472, 499), Game1.random.Next(1, 5) * 5);
                    break;
            }
        }

        // === Shaft floor cap ===

        /// <summary>
        /// After the game computes how many floors a shaft fall drops the player, cap it so the fall
        /// never skips past a whole-hundred treasure floor (mineLevel 220, 320, 420, ... which are
        /// displayed as floors 100, 200, 300, ...).
        /// </summary>
        private static void AfterEnterMineShaft(MineShaft __instance)
        {
            ModConfig config = ModEntry.Config;
            if (!config.KeepTreasureFloors || __instance.getMineArea() != 121)
                return;

            int level = __instance.mineLevel;
            int fallen = LastLevelsDownFallen(__instance);

            // Treasure rooms are on mineLevel where mineLevel % 100 == 20.
            int nextTreasure = level - (level % 100) + 20;
            if (nextTreasure <= level)
                nextTreasure += 100;

            int capped = nextTreasure - level;
            if (nextTreasure < level + fallen)
            {
                LastLevelsDownFallen(__instance) = capped;

                // The game already deducted health for the longer fall; give back the difference.
                Game1.player.health = System.Math.Min(
                    Game1.player.maxHealth,
                    Game1.player.health + (fallen - capped) * 3
                );
            }
        }
    }
}
