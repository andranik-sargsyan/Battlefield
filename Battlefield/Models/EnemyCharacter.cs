using Battlefield.Enums;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Battlefield.Models
{
    class EnemyCharacter : Character
    {
        public bool IsStrong { get; set; }

        private int _shootProbability = 200;
        private int _level;

        public EnemyCharacter(Control control, int level, Point position, DirectionEnum direction = DirectionEnum.Down, bool isStrong = false)
        {
            _control = control;
            _level = level;

            Position = position;
            Health = isStrong ? 8 : 5;
            Damage = isStrong ? 2 : 1;
            Direction = direction;
            Speed = isStrong ? 2 : 1;
            IsStrong = isStrong;

            var strongSuffix = isStrong ? "-w" : string.Empty;
            Picture.Image = Image.FromFile($@"Images\tank-red{strongSuffix}-down.png");

            control.Controls.Add(Picture);
        }

        public override void Move()
        {
            var newPosition = Position;

            switch (Direction)
            {
                case DirectionEnum.Up:
                    newPosition.Y = Position.Y - Speed;
                    break;
                case DirectionEnum.Right:
                    newPosition.X = Position.X + Speed;
                    break;
                case DirectionEnum.Down:
                    newPosition.Y = Position.Y + Speed;
                    break;
                case DirectionEnum.Left:
                    newPosition.X = Position.X - Speed;
                    break;
                default:
                    break;
            }

            var isInBounds = IsInBounds(newPosition, _control);
            var collidedObject = GameForm.GameObjects.FirstOrDefault(ob => ob != this && !(ob is Bonus) && ob.HasCollision(newPosition, Size));

            if (isInBounds && collidedObject == null)
            {
                Position = newPosition;
                
                if (GameForm.Random.Next(_shootProbability - _level * 15) == 0)
                {
                    Shoot();
                }
            }
            else
            {
                Direction = (DirectionEnum)GameForm.Random.Next(1, 5);
            }

            var bonus = GameForm.GameObjects.FirstOrDefault(ob => ob is Bonus);
            if (bonus != null && HasCollision(bonus))
            {
                bonus.Collide(this);
            }
        }
    }
}
