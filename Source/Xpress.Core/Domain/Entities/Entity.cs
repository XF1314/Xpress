using System;
using System.Reflection;
using Xpress.Core.Extensions;
using Xpress.Core.Utils;

namespace Xpress.Core.Domain.Entities
{
    [Serializable]
    public abstract class Entity : IEntity
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}] Keys = {GetKeys().JoinAsString(", ")}";
        }

        public abstract object[] GetKeys();
    }

    public abstract class Entity<TIdentity> : Entity, IEntity<TIdentity>
        where TIdentity : IIdentity
    {
        /// <inheritdoc/>
        public virtual TIdentity Id { get; set; }

        protected Entity()
        {

        }

        protected Entity(TIdentity id)
        {
            Id = id;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TIdentity>))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            var other = (Entity<TIdentity>)obj;
            if (EntityHelper.HasDefaultId(this) && EntityHelper.HasDefaultId(other))
            {
                return false;
            }

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType().GetTypeInfo();
            var typeOfOther = other.GetType().GetTypeInfo();
            if (!typeOfThis.IsAssignableFrom(typeOfOther) && !typeOfOther.IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if (Id == null)
            {
                return 0;
            }

            return Id.GetHashCode();
        }

        public static bool operator ==(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            return !(left == right);
        }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[ENTITY: {GetType().Name}] Id = {Id}";
        }
    }
}
