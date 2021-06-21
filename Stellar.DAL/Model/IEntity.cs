using System;

namespace Stellar.DAL.Model
{
    public interface IEntity
    {
        public Guid? Id { get; }
    }
}