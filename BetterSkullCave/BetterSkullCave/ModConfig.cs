namespace BetterSkullCave
{
    /// <summary>The mod configuration, edited through Generic Mod Config Menu.</summary>
    public sealed class ModConfig
    {
        /// <summary>The red component of the floor-counter color (0-255).</summary>
        public int Red { get; set; } = 255;

        /// <summary>The green component of the floor-counter color (0-255).</summary>
        public int Green { get; set; } = 255;

        /// <summary>The blue component of the floor-counter color (0-255).</summary>
        public int Blue { get; set; } = 255;

        /// <summary>Whether to replace hat treasures in the Skull Cavern with a configurable item.</summary>
        public bool EnableTreasureReplacement { get; set; } = true;

        /// <summary>Allow treasure replacement to produce machines (Crystalarium, Seed Maker, Auto Grabber, Auto Petter).</summary>
        public bool EnableMachine { get; set; } = true;

        /// <summary>Allow treasure replacement to produce seeds.</summary>
        public bool EnableSeed { get; set; } = false;

        /// <summary>Allow treasure replacement to produce bombs.</summary>
        public bool EnableBomb { get; set; } = false;

        /// <summary>Allow treasure replacement to produce medicine (Life Elixir, Energy Tonic).</summary>
        public bool EnableMedicine { get; set; } = false;

        /// <summary>Allow treasure replacement to produce saplings.</summary>
        public bool EnableSapling { get; set; } = false;

        /// <summary>Whether jumping down a shaft stops at the next whole-hundred floor instead of skipping it.</summary>
        public bool KeepTreasureFloors { get; set; } = true;
    }
}
