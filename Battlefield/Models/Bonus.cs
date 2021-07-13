using Battlefield.Enums;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Battlefield.Models
{
    class Bonus : GameObject
    {
        private Control _control;
        private SoundPlayer _soundPlayerBonus = new SoundPlayer(@"Sounds\bonus.wav");

        public Bonus(Control control, Point position, BonusTypeEnum type)
        {
            _control = control;
            Position = position;

            Picture.Image = Image.FromFile($@"Images\bonus-{type.ToString().ToLower()}.gif");

            control.Controls.Add(Picture);
        }

        public override void Collide(GameObject ob)
        {
            _soundPlayerBonus.Play();

            IsDestroyed = true;
        }

        public override void Draw(Graphics g)
        {
            //nothing to do for now
        }
    }
}
