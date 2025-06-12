namespace Core.Model
{
    public class Role
    {
        public Role(string name, bool canCreate, bool canFulfill)
        {
            Name = name;
            CanCreateWorkOrder = canCreate;
            CanFulfillWorkOrder = canFulfill;
        }

        public Role()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public bool CanCreateWorkOrder { get; set; }

        public bool CanFulfillWorkOrder { get; set; }
    }
}