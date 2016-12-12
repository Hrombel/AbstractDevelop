using System;
using System.Collections.Generic;
using System.Linq;

using AbstractDevelop.RiscMachine.Properties;

namespace AbstractDevelop.Machines
{
    public sealed class RiscPlatform :
        IPlatformProvider
    {
        public int ID => 1;

        public string Name => Translate.Key("RiscPlatformName", source: Resources.ResourceManager);

        public IEnumerable<Type> AvailableMachineTypes { get { yield return typeof(RiscMachine); } }

        public AbstractMachine CurrentMachine { get; internal set; }

        public AbstractMachine CreateMachine(Type machineType, Dictionary<string, string> settings)
        {
            if (AvailableMachineTypes.Contains(machineType))
                return (CurrentMachine = new RiscMachine(
                    settings.TryParse("memorySize", int.Parse, 256),
                    settings.TryParse("registerCount", int.Parse, 8)));
            else
                throw new InvalidCastException(Translate.Key("WrongMachineType", source: Resources.ResourceManager));
        }

        public void Initialize(IExtensibilityProvider extensibilityProvider)
        {
            // nothing here to do right now
        }
    }
}
