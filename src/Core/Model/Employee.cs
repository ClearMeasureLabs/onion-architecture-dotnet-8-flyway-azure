using System;
using System.Collections.Generic;

namespace Core.Model
{
    public class Employee : IComparable<Employee>
    {
        private string _emailAddress;
        private string _firstName;
        private Guid _id;
        private string _lastName;
        private ISet<Role> _roles = new HashSet<Role>();
        private string _userName;
        
        public Employee()
        {
        }

        public Employee(string userName, string firstName, string lastName, string emailAddress)
        {
            _userName = userName;
            _firstName = firstName;
            _lastName = lastName;
            _emailAddress = emailAddress;
        }

        public virtual Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public virtual string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public virtual string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public virtual string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        private ISet<Role> Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        public virtual int CompareTo(Employee other)
        {
            int compareResult = LastName.CompareTo(other.LastName);
            if (compareResult == 0)
            {
                compareResult = FirstName.CompareTo(other.FirstName);
            }

            return compareResult;
        }

        public virtual string GetFullName()
        {
            return string.Format("{0} {1}", FirstName, LastName);
        }

        public override string ToString()
        {
            return GetFullName();
        }

        public virtual bool CanCreateWorkOrder()
        {
            foreach (Role role in Roles)
            {
                if (role.CanCreateWorkOrder)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool CanFulfilWorkOrder()
        {
            foreach (Role role in Roles)
            {
                if (role.CanFulfillWorkOrder)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void AddRole(Role role)
        {
            Roles.Add(role);
        }

        public virtual Role[] GetRoles()
        {
            return new List<Role>(Roles).ToArray();
        }

        public virtual string GetNotificationEmail(DayOfWeek day)
        {
            return EmailAddress;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Employee)) return false;
            return Equals((Employee) obj);
        }

        public virtual bool Equals(Employee other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Id == Guid.Empty) return false;
            return other._id.Equals(_id);
        }

        public override int GetHashCode()
        {
            if (Id == Guid.Empty) return base.GetHashCode();
            return _id.GetHashCode();
        }

        public static bool operator ==(Employee a, Employee b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(Employee a, Employee b)
        {
            return !(a == b);
        }
    }
}