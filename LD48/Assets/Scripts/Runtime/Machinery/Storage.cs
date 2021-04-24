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

        public Storage(string material, int capacity = 0, int amount = 0) : this()
        {
            this.material = material;
            this.capacity = capacity;
            this.amount = amount;
        }

        public override string ToString()
        {
            if (capacity > 0) return $"Storage of {material} ({amount}/{capacity})";
            return $"Storage of {material} ({amount}/Unlimited)";
        }
    }
}