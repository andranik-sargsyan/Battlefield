using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Battlefield.Models
{
    class Obstacle : GameObject
    {
        private Control _control;
        private SoundPlayer _soundPlayerHitWall = new SoundPlayer(@"Sounds\hit-wall.wav");
        private SoundPlayer _soundPlayerDestroyWall = new SoundPlayer(@"Sounds\destroy-wall.wav");

        public int Health { get; set; }

        public Obstacle(Control control, Point position)
        {
            _control = control;
            Position = position;

            Picture.Image = Image.FromFile(@"Images\wall.jpg");
            Health = 3;

            control.Controls.Add(Picture);
        }

        public override void Collide(GameObject ob)
        {
            if (ob is Bullet bullet)
            {
                if (Health > 0)
                {
                    Health -= bullet.Damage;
                }

                if (Health <= 0)
                {
                    IsDestroyed = true;

                    _soundPlayerDestroyWall.Play();
                }
                else
                {
                    _soundPlayerHitWall.Play();
                }
            }
        }

        public override void Draw(Graphics g)
        {
            if (Health == 3 && Picture.ImageLocation != @"Images\wall.jpg")
            {
                Picture.ImageLocation = @"Images\wall.jpg";
            }
            if (Health == 2 && Picture.ImageLocation != @"Images\wall-broken-1.jpg")
            {
                Picture.ImageLocation = @"Images\wall-broken-1.jpg";
            }
            else if (Health == 1 && Picture.ImageLocation != @"Images\wall-broken-2.jpg")
            {
                Picture.ImageLocation = @"Images\wall-broken-2.jpg";
            }
        }
    }
}
