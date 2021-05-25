﻿using System;

namespace Stellar.DAL.Tests.Data
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        public override bool Equals(object obj)
        {
            if (obj is not Entity other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            if (this.IdIsNullOrEmpty() || other.IdIsNullOrEmpty())
            {
                return false;
            }

            return Id == other.Id;
        }

        public static bool operator == (Entity a, Entity b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator != (Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return (GetType().ToString() + Id).GetHashCode();
        }
    }
}