namespace Mini_Banking.Domain.Entities
{
    public class Entity
    {
        public int ID { get; private set; }

        public Entity()
        {
            
        }

        public Entity(int id)
        {
            this.ID = id;
        }
    }
}
