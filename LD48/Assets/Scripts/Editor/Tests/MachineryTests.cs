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
        public void TestTimedProduction()
        {
            MachineInfo info = new MachineInfo("m1").WithProduction(new Production("mat1", 1, 2));
            Machine machine = new Machine(info);

            MachineInfo info2 = new MachineInfo("m2");
            Machine machine2 = new Machine(info2);

            AssemblyLine ass = new AssemblyLine().WithMachine(machine).WithMachine(machine2);

            ass.Tick();
            Assert.AreEqual(0, machine.outputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, machine.outputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, machine.outputStorage.Count);

            ass.Tick();
            Assert.AreEqual(2, machine.outputStorage.Count);
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
            Assert.AreEqual(0, machine.inputStorage.Count);
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
        public void TestStorageCapacity2()
        {
            MachineInfo info = new MachineInfo("m1").WithProduction(new Production("mat1"));
            info.totalOutputCapacity = 2;
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
            Assert.AreEqual(1, conv1.inputStorage.Count);
            Assert.AreEqual(1, conv1.outputStorage.Count);
            Assert.AreEqual(0, machine2.inputStorage.Count);
        }

        [Test]
        public void TestInputCapacity()
        {
            MachineInfo info = new MachineInfo("conv1").WithProduction(new Production(Strategy.Forward));
            MachineInfo info2 = new MachineInfo("m1").WithProduction(new Production("mat1"));
            MachineInfo info3 = new MachineInfo("m2").WithInputCapacity("mat1", 2);
            Machine conv1 = new Machine(info);
            Machine machine1 = new Machine(info2).WithOutputPort(new Port(conv1));
            Machine machine2 = new Machine(info3).WithInputPort(new Port(conv1));

            AssemblyLine ass = new AssemblyLine().WithMachines(conv1, machine1, machine2);

            ass.Tick();
            ass.Tick();
            ass.Tick();
            ass.Tick();
            ass.Tick();
            ass.Tick();
            Assert.AreEqual(2, machine2.inputStorage.Count);
        }

        [Test]
        public void TestInputFilter()
        {
            MachineInfo info = new MachineInfo("conv1").WithProduction(new Production(Strategy.Forward));
            MachineInfo info2 = new MachineInfo("m1").WithProduction(new Production("mat2"));
            MachineInfo info3 = new MachineInfo("m2").WithInputCapacity("mat1", 2);
            Machine conv1 = new Machine(info);
            Machine machine1 = new Machine(info2).WithOutputPort(new Port(conv1));
            Machine machine2 = new Machine(info3).WithInputPort(new Port(conv1));

            AssemblyLine ass = new AssemblyLine().WithMachines(conv1, machine1, machine2);

            ass.Tick();
            ass.Tick();
            ass.Tick();
            ass.Tick();
            ass.Tick();
            ass.Tick();
            Assert.AreEqual(0, machine2.inputStorage.Count);
        }

        [Ignore("Using shuffle right now which is non-deterministic")]
        [Test]
        public void TestConveyorDistribution()
        {
            MachineInfo info = new MachineInfo("m1").WithProduction(new Production("mat1"));
            MachineInfo info2 = new MachineInfo("conv1").WithProduction(new Production(Strategy.Forward));
            MachineInfo info3 = new MachineInfo("conv2").WithProduction(new Production(Strategy.Forward));
            MachineInfo info4 = new MachineInfo("conv3").WithProduction(new Production(Strategy.Forward));
            MachineInfo info5 = new MachineInfo("conv4").WithProduction(new Production(Strategy.Forward));
            Machine conv2 = new Machine(info3);
            Machine conv1 = new Machine(info2).WithOutputPort(new Port(conv2));
            Machine conv4 = new Machine(info5);
            Machine conv3 = new Machine(info4).WithOutputPort(new Port(conv4));
            Machine machine1 = new Machine(info).WithOutputPort(new Port(conv1)).WithOutputPort(new Port(conv3));

            AssemblyLine ass = new AssemblyLine().WithMachines(conv1, conv2, conv3, conv4, machine1);

            ass.Tick();
            ass.Tick();
            Assert.AreEqual(1, conv1.outputStorage.Count);
            Assert.AreEqual(0, conv2.outputStorage.Count);
            Assert.AreEqual(0, conv3.outputStorage.Count);
            Assert.AreEqual(0, conv4.outputStorage.Count);

            ass.Tick();
            Assert.AreEqual(0, conv1.outputStorage.Count);
            Assert.AreEqual(1, conv2.outputStorage.Count);
            Assert.AreEqual(1, conv3.outputStorage.Count);
            Assert.AreEqual(0, conv4.outputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, conv1.outputStorage.Count);
            Assert.AreEqual(1, conv2.outputStorage.Count);
            Assert.AreEqual(0, conv3.outputStorage.Count);
            Assert.AreEqual(1, conv4.outputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, conv1.outputStorage.Count);
            Assert.AreEqual(1, conv2.outputStorage.Count);
            Assert.AreEqual(1, conv3.outputStorage.Count);
            Assert.AreEqual(1, conv4.outputStorage.Count);
        }
    }
}