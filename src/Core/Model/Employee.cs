using System;
using System.Collections.Generic;

namespace Core.Model
{
    public class Employee : IComparable<Employee>, IEquatable<Employee>
    {
        public Employee()
        {
            UserName = null!;
            EmailAddress = null!;
            FirstName = null!;
            LastName = null!;
        }

        public Employee(string userName, string firstName, string lastName, string emailAddress)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
        }

        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        private ISet<Role> Roles { get; set; } = new HashSet<Role>();

        public int CompareTo(Employee? other)
        {
            int compareResult = String.Compare(LastName, other!.LastName, StringComparison.Ordinal);
            if (compareResult == 0)
            {
                compareResult = String.Compare(FirstName, other.FirstName, StringComparison.Ordinal);
            }

            return compareResult;
        }

        public string GetFullName()
        {
            return string.Format("{0} {1}", FirstName, LastName);
        }

        public override string ToString()
        {
            return GetFullName();
        }

        public bool CanCreateWorkOrder()
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

        public bool CanFulfilWorkOrder()
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

        public void AddRole(Role role)
        {
            Roles.Add(role);
        }

        public Role[] GetRoles()
        {
            return new List<Role>(Roles).ToArray();
        }

        public string GetNotificationEmail(DayOfWeek day)
        {
            return EmailAddress;
        }

        #region Equality Members

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Employee)) return false;
            return Equals((Employee)obj);
        }

        public virtual bool Equals(Employee? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && !Id.Equals(Guid.Empty);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Employee left, Employee right)
        {
            if (ReferenceEquals(left, null))
                return ReferenceEquals(right, null);

            return left.Equals(right);
        }

        public static bool operator !=(Employee left, Employee right)
        {
            return !(left == right);
        }

        #endregion
    }
}