using System;
using System.Collections.Generic;
using System.Linq;

using AbstractDevelop.RiscMachine.Properties;
using AbstractDevelop.Projects;

namespace AbstractDevelop.Machines
{
    public sealed class RiscPlatform :
        IPlatformProvider
    {
        public int ID => 1;

        public string Name => Translate.Key("RiscPlatformName", source: Resources.ResourceManager);

        public IEnumerable<Type> AvailableMachineTypes { get { yield return typeof(RiscMachine); } }

        public AbstractMachine CurrentMachine { get; internal set; }

        public AbstractMachine CreateMachine(AbstractProject project)
        {
            return (CurrentMachine = new RiscMachine(
                          project.Settings.TryParse("memorySize", int.Parse, 256),
                          project.Settings.TryParse("registerCount", int.Parse, 8)));
        }

        public AbstractMachine CreateMachine(Type machineType, AbstractProject project)
        {
            if (AvailableMachineTypes.Contains(machineType))
                return CreateMachine(project);
            else
                throw new InvalidCastException(Translate.Key("WrongMachineType", source: Resources.ResourceManager));
        }

        public void Initialize(IExtensibilityProvider extensibilityProvider)
        {
            // nothing here to do right now
        }
    }
}
