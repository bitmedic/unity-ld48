using NUnit.Framework;
using static LD48.Production;

namespace LD48
{
    public class MachineryTests
    {
        [Test]
        public void TestNoProduction()
        {
            Machine machine = new Machine("m1");

            Assert.AreEqual(0, machine.outputStorage.Count);
            machine.FullTick();
            Assert.AreEqual(0, machine.outputStorage.Count);
        }

        [Test]
        public void TestProduction()
        {
            Machine machine = new Machine("m1")
                .WithProduction(new Production("mat1"))
                .WithProduction(new Production("mat2", 3, 2));

            Assert.AreEqual(0, machine.outputStorage.Count);

            machine.FullTick();
            Assert.AreEqual(1, machine.outputStorage.Count);
            Assert.AreEqual(1, machine.outputStorage["mat1"].amount);

            machine.FullTick();
            Assert.AreEqual(2, machine.outputStorage.Count);
            Assert.AreEqual(2, machine.outputStorage["mat1"].amount);
            Assert.AreEqual(3, machine.outputStorage["mat2"].amount);

            machine.FullTick();
            Assert.AreEqual(2, machine.outputStorage.Count);
            Assert.AreEqual(3, machine.outputStorage["mat1"].amount);
            Assert.AreEqual(3, machine.outputStorage["mat2"].amount);

            machine.FullTick();
            Assert.AreEqual(2, machine.outputStorage.Count);
            Assert.AreEqual(4, machine.outputStorage["mat1"].amount);
            Assert.AreEqual(6, machine.outputStorage["mat2"].amount);
        }

        [Test]
        public void TestStorageCapacity()
        {
            Machine machine = new Machine("m1").WithProduction(new Production("mat1")).WithOutputStorage(new Storage("mat1", 2));

            machine.FullTick();
            machine.FullTick();
            machine.FullTick();
            Assert.AreEqual(2, machine.outputStorage["mat1"].amount);
        }

        [Test]
        public void TestConveyor()
        {
            Machine conv1 = new Machine("conv1").WithProduction(new Production(Strategy.Forward));
            Machine machine1 = new Machine("m1").WithProduction(new Production("mat1")).WithOutputPort(new Port(conv1));
            Machine machine2 = new Machine("m2").WithInputPort(new Port(conv1));

            AssemblyLine ass = new AssemblyLine().WithMachines(conv1, machine1, machine2);

            Assert.AreEqual(0, machine1.outputStorage.Count);
            Assert.AreEqual(0, conv1.inputStorage.Count);
            Assert.AreEqual(0, conv1.outputStorage.Count);
            Assert.AreEqual(0, machine2.inputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, machine1.outputStorage["mat1"].amount);
            Assert.AreEqual(0, conv1.inputStorage.Count);
            Assert.AreEqual(0, conv1.outputStorage.Count);
            Assert.AreEqual(0, machine2.inputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, machine1.outputStorage["mat1"].amount);
            Assert.AreEqual(1, conv1.inputStorage.Count);
            Assert.AreEqual(1, conv1.outputStorage.Count);
            Assert.AreEqual(0, machine2.inputStorage.Count);

            ass.Tick();
            Assert.AreEqual(1, machine1.outputStorage["mat1"].amount);
            Assert.AreEqual(1, conv1.inputStorage.Count);
            Assert.AreEqual(1, conv1.outputStorage.Count);
            Assert.AreEqual(1, machine2.inputStorage.Count);
        }
    }
}