namespace Mini_Banking.Infrastructure.PersistenceModels
{
    internal class EntityPersistenceModel
    {
        public int RowVersion { get; private set; } = default!;
    }
}
