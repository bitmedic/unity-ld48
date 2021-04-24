namespace LD48
{
    public class Storage
    {
        public string material;
        public int capacity; // 0 = unlimited
        public int amount;

        public Storage()
        {
        }

        public Storage(string material, int amount = 0, int capacity = 0) : this()
        {
            this.material = material;
            this.amount = amount;
            this.capacity = capacity;
        }
    }
}