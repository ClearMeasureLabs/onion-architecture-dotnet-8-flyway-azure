using System;

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
            
        }

        public virtual string Name
        {
            get; set;
        }

        public virtual Guid Id { get; set; }

        public virtual bool CanCreateWorkOrder { get; set; }

        public virtual bool CanFulfillWorkOrder { get; set; }
    }
}