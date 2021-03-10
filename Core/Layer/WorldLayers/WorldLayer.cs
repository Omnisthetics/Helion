using Helion.Audio;
using Helion.Resources.Archives.Collection;
using Helion.Util.Configs;
using Helion.Util.Consoles;
using Helion.World;

namespace Helion.Layer.WorldLayers
{
    /// <summary>
    /// A layer that represents a world which can be interacted with. This is
    /// extendable for single player worlds, client worlds, demos, etc.
    /// </summary>
    public abstract class WorldLayer : GameLayer
    {
        protected readonly Config Config;
        protected readonly HelionConsole Console;
        protected readonly ArchiveCollection ArchiveCollection;
        protected readonly IAudioSystem AudioSystem;
        protected override double Priority => 0.25f;

        public abstract WorldBase World { get; }

        public WorldLayer(Config config, HelionConsole console, ArchiveCollection archiveCollection,
            IAudioSystem audioSystem)
        {
            Config = config;
            Console = console;
            ArchiveCollection = archiveCollection;
            AudioSystem = audioSystem;
        }
    }
}