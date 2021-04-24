using System.Linq;
using NUnit.Framework;
using static LD48.Production;

namespace LD48
{
    public class MachineryTests
    {
        [Test]
        public void TestNoProduction()
        {
            MachineInfo info = new MachineInfo("m1");
            Machine machine = new Machine(info);

            Assert.AreEqual(0, machine.outputStorage.Count);
            machine.FullTick();
            Assert.AreEqual(0, machine.outputStorage.Count);
        }

        [Test]
        public void TestPortSetup()
        {
            MachineInfo info = new MachineInfo("m1").WithInputPort(new PortDefinition()).WithInputPort(new PortDefinition()).WithOutputPort(new PortDefinition());
            Machine machine = new Machine(info);

            Assert.AreEqual(2, machine.inputPorts.Count);
            Assert.AreEqual(1, machine.outputPorts.Count);
        }

        [Test]
        public void TestProduction()
        {
            MachineInfo info = new MachineInfo("m1")
                .WithProduction(new Production("mat1"))
                .WithProduction(new Production("mat2", 3, 2));
            Machine machine = new Machine(info);

            Assert.AreEqual(0, machine.outputStorage.Count);

            machine.FullTick();
            Assert.AreEqual(1, machine.outputStorage.Count);

            machine.FullTick();
            Assert.AreEqual(5, machine.outputStorage.Count);
            Assert.AreEqual(2, machine.outputStorage.Count(p => p.material == "mat1"));
            Assert.AreEqual(3, machine.outputStorage.Count(p => p.material == "mat2"));

            machine.FullTick();
            Assert.AreEqual(6, machine.outputStorage.Count);
            Assert.AreEqual(3, machine.outputStorage.Count(p => p.material == "mat1"));
            Assert.AreEqual(3, machine.outputStorage.Count(p => p.material == "mat2"));

            machine.FullTick();
            Assert.AreEqual(10, machine.outputStorage.Count);
            Assert.AreEqual(4, machine.outputStorage.Count(p => p.material == "mat1"));
            Assert.AreEqual(6, machine.outputStorage.Count(p => p.material == "mat2"));
        }

        [Test]
        public void TestMultiInputProduction()
        {
            MachineInfo info = new MachineInfo("m1");
            Machine machine = new Machine(info);

            Production production = new Production("mat3");
            production.strategy = Strategy.Formula;
            production.formula.Add("mat1", 2);
            production.formula.Add("mat2", 1);
            info.production.Add(production);

            machine.FullTick();
            Assert.AreEqual(0, machine.outputStorage.Count);

            machine.inputStorage.Add(new Package("mat2"));
            machine.FullTick();
            Assert.AreEqual(0, machine.outputStorage.Count);

            machine.inputStorage.Add(new Package("mat1"));
            machine.FullTick();
            Assert.AreEqual(0, machine.outputStorage.Count);

            machine.inputStorage.Add(new Package("mat1"));
            machine.FullTick();
            Assert.AreEqual(1, machine.outputStorage.Count);
            Assert.AreEqual("mat3", machine.outputStorage[0].material);
        }

        [Test]
        public void TestStorageCapacity()
        {
            MachineInfo info = new MachineInfo("m1").WithProduction(new Production("mat1")).WithOutputCapacity("mat1", 2);
            Machine machine = new Machine(info);

            machine.FullTick();
            machine.FullTick();
            machine.FullTick();
            Assert.AreEqual(2, machine.outputStorage.Count);
        }

        [Test]
        public void TestConveyor()
        {
            MachineInfo info = new MachineInfo("conv1").WithProduction(new Production(Strategy.Forward));
            MachineInfo info2 = new MachineInfo("m1").WithProduction(new Production("mat1"));
            MachineInfo info3 = new MachineInfo("m2").WithProduction(new Production("mat1")).WithOutputCapacity("mat1", 2);
            Machine conv1 = new Machine(info);
            Machine machine1 = new Machine(info2).WithOutputPort(new Port(conv1));
            Machine machine2 = new Machine(info3).WithInputPort(new Port(conv1));

            AssemblyLine ass = new AssemblyLine().WithMachines(conv1, machine1, machine2);

            Assert.AreEqual(0, machine1.outputStorage.Count);
            Assert.AreEqual(0, conv1.inputStorage.Count);
            Assert.AreEqual(0, conv1.outputStorage.Count);
            Assert.AreEqual(0, machine2.inputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, machine1.outputStorage.Count);
            Assert.AreEqual(0, conv1.inputStorage.Count);
            Assert.AreEqual(0, conv1.outputStorage.Count);
            Assert.AreEqual(0, machine2.inputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, machine1.outputStorage.Count);
            Assert.AreEqual(0, conv1.inputStorage.Count);
            Assert.AreEqual(1, conv1.outputStorage.Count);
            Assert.AreEqual(0, machine2.inputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, machine1.outputStorage.Count);
            Assert.AreEqual(0, conv1.inputStorage.Count);
            Assert.AreEqual(1, conv1.outputStorage.Count);
            Assert.AreEqual(1, machine2.inputStorage.Count);
        }
    }
}