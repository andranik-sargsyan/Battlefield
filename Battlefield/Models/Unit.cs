using Battlefield.Enums;

namespace Battlefield.Models
{
    abstract class Unit : GameObject
    {
        public DirectionEnum Direction { get; set; }
        public int Speed { get; set; }

        public abstract void Move();
    }
}
