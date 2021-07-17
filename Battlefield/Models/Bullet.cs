using Battlefield.Enums;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace Battlefield.Models
{
    class Bullet : Unit
    {
        public int Damage { get; set; }
        public Character Character { get; set; }

        private Control _control;
        private SoundPlayer _soundPlayerHitBound = new SoundPlayer(@"Sounds\hit-bound.wav");
        private SoundPlayer _soundPlayerHitBullet = new SoundPlayer(@"Sounds\hit-bullet.wav");

        public Bullet(Control control, Character character, Point position, DirectionEnum direction, int damage = 1, int bonusSpeed = 0)
        {
            _control = control;

            Character = character;
            Position = position;
            Direction = direction;
            Speed = 5 + bonusSpeed;
            Damage = damage;

            Size = new Size(GameForm.UnitSize.Width / 3, GameForm.UnitSize.Height / 3);

            var damageSuffix = damage > 1 ? "-r" : string.Empty;
            Picture.Image = Image.FromFile($@"Images\bullet{damageSuffix}.png");

            control.Controls.Add(Picture);
        }

        public override void Collide(GameObject ob)
        {
            if (ob is Bullet bullet)
            {
                IsDestroyed = true;
                ob.IsDestroyed = true;

                _soundPlayerHitBullet.Play();
            }
        }

        public override void Draw(Graphics g)
        {
            //nothing to do for now
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

            if (isInBounds && (collidedObject == null || collidedObject.GetType() == Character.GetType()))
            {
                Position = newPosition;
            }
            else
            {
                if (!isInBounds)
                {
                    _soundPlayerHitBound.Play();
                }
                else if (collidedObject != null)
                {
                    collidedObject.Collide(this);
                }

                IsDestroyed = true;
            }
        }
    }
}
