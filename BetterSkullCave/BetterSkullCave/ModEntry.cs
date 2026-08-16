using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BetterSkullCave
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary>
        /// The loaded configuration. Patches read this directly, so options changed through
        /// Generic Mod Config Menu take effect immediately without restarting the game.
        /// </summary>
        internal static ModConfig Config = null!;

        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            Harmony harmony = new(ModManifest.UniqueID);
            ModPatches.Apply(harmony);

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>Register the mod's options with Generic Mod Config Menu, if it is installed.</summary>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            IGenericModConfigMenuApi? configMenu =
                Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // --- Floor counter color ---
            configMenu.AddSectionTitle(ModManifest, () => "Floor Counter Color");
            configMenu.AddParagraph(ModManifest, () => "Color of the floor counter in the top-left of the Skull Cavern.");
            configMenu.AddNumberOption(ModManifest, () => Config.Red, v => Config.Red = v, () => "Red", min: 0, max: 255);
            configMenu.AddNumberOption(ModManifest, () => Config.Green, v => Config.Green = v, () => "Green", min: 0, max: 255);
            configMenu.AddNumberOption(ModManifest, () => Config.Blue, v => Config.Blue = v, () => "Blue", min: 0, max: 255);

            // --- Treasure replacement ---
            configMenu.AddSectionTitle(ModManifest, () => "Treasure Replacement");
            configMenu.AddParagraph(ModManifest, () => "Replace hat treasures in the Skull Cavern with a configurable item.");
            configMenu.AddBoolOption(ModManifest, () => Config.EnableTreasureReplacement, v => Config.EnableTreasureReplacement = v, () => "Enable", () => "Replace hat treasures with the items selected below.");
            configMenu.AddBoolOption(ModManifest, () => Config.EnableMachine, v => Config.EnableMachine = v, () => "Machines", () => "Crystalarium, Seed Maker, Auto Grabber, Auto Petter");
            configMenu.AddBoolOption(ModManifest, () => Config.EnableBomb, v => Config.EnableBomb = v, () => "Bombs", () => "Cherry Bomb, Bomb");
            configMenu.AddBoolOption(ModManifest, () => Config.EnableSeed, v => Config.EnableSeed = v, () => "Seeds", () => "Random seed packets");
            configMenu.AddBoolOption(ModManifest, () => Config.EnableMedicine, v => Config.EnableMedicine = v, () => "Medicine", () => "Life Elixir, Energy Tonic");
            configMenu.AddBoolOption(ModManifest, () => Config.EnableSapling, v => Config.EnableSapling = v, () => "Saplings", () => "Random fruit-tree sapling");

            // --- Shaft floor cap ---
            configMenu.AddSectionTitle(ModManifest, () => "Shaft Floor Cap");
            configMenu.AddParagraph(ModManifest, () => "Stop shaft falls from skipping whole-hundred treasure floors (100, 200, 300...).");
            configMenu.AddBoolOption(ModManifest, () => Config.KeepTreasureFloors, v => Config.KeepTreasureFloors = v, () => "Keep treasure floors", () => "When jumping down a shaft, land on the next whole-hundred floor instead of skipping past it.");
        }
    }
}
