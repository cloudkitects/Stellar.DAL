using System;

namespace Stellar.DAL.Model
{
    public static class Extensions
    {
        public static bool IdIsNull(this Entity entity)
        {
            return entity == null;
        }

        public static bool IdIsEmpty(this Entity entity)
        {
            return entity.Id.Equals(Guid.Empty);
        }

        public static bool IdIsNullOrEmpty(this Entity entity)
        {
            return IdIsNull(entity) || IdIsEmpty(entity);
        }

    }
}