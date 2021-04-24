using NUnit.Framework;

namespace LD48
{
    public class MachineryTests
    {
        [Test]
        public void TestNoProduction()
        {
            Machine machine = new Machine("m1");

            Assert.AreEqual(0, machine.outputStorage.Count);
            machine.Tick();
            Assert.AreEqual(0, machine.outputStorage.Count);
        }

        [Test]
        public void TestProduction()
        {
            Machine machine = new Machine("m1")
                .WithProduction(new Production("mat1", 1, 1))
                .WithProduction(new Production("mat2", 3, 2));

            Assert.AreEqual(0, machine.outputStorage.Count);

            machine.Tick();
            Assert.AreEqual(1, machine.outputStorage.Count);
            Assert.AreEqual(1, machine.outputStorage["mat1"].amount);

            machine.Tick();
            Assert.AreEqual(2, machine.outputStorage.Count);
            Assert.AreEqual(2, machine.outputStorage["mat1"].amount);
            Assert.AreEqual(3, machine.outputStorage["mat2"].amount);

            machine.Tick();
            Assert.AreEqual(2, machine.outputStorage.Count);
            Assert.AreEqual(3, machine.outputStorage["mat1"].amount);
            Assert.AreEqual(3, machine.outputStorage["mat2"].amount);

            machine.Tick();
            Assert.AreEqual(2, machine.outputStorage.Count);
            Assert.AreEqual(4, machine.outputStorage["mat1"].amount);
            Assert.AreEqual(6, machine.outputStorage["mat2"].amount);
        }
    }
}