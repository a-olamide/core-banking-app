using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domain.Primitives
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public sealed override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            var other = (ValueObject)obj;

            return GetEqualityComponents()
                .SequenceEqual(other.GetEqualityComponents());
        }

        public sealed override int GetHashCode()
        {
            var hash = new HashCode();

            foreach (var component in GetEqualityComponents())
                hash.Add(component);

            return hash.ToHashCode();
        }

        public static bool operator ==(ValueObject? a, ValueObject? b)
            => a is null ? b is null : a.Equals(b);

        public static bool operator !=(ValueObject? a, ValueObject? b)
            => !(a == b);
    }
}
