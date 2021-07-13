using System.Drawing;
using System.Windows.Forms;

namespace Battlefield.Models
{
    class Capybara : GameObject
    {
        public Capybara(Control control, Point position)
        {
            Position = position;

            Picture.Image = Image.FromFile(@"Images\capybara.png");

            control.Controls.Add(Picture);
        }

        public override void Collide(GameObject ob)
        {
            if (ob is Bullet)
            {
                IsDestroyed = true;
            }
        }

        public override void Draw(Graphics g)
        {
            //nothing to do for now
        }
    }
}
