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
        private BonusTypeEnum _type;

        public Bonus(Control control, Point position, BonusTypeEnum type)
        {
            _control = control;
            _type = type;

            Position = position;

            Picture.Image = Image.FromFile($@"Images\bonus-{type.ToString().ToLower()}.gif");

            control.Controls.Add(Picture);
        }

        public override void Collide(GameObject ob)
        {
            switch (_type)
            {
                case BonusTypeEnum.Health:
                    (ob as Character).Health++;
                    break;
                case BonusTypeEnum.Damage:
                    (ob as Character).Damage++;
                    break;
                case BonusTypeEnum.Wall:
                    GameForm.GameObjects.ForEach(obj =>
                    {
                        if (obj is Obstacle obstacle)
                        {
                            obstacle.Health = 3;
                        }
                    });
                    break;
                default:
                    break;
            }

            _soundPlayerBonus.Play();

            IsDestroyed = true;
        }

        public override void Draw(Graphics g)
        {
            //nothing to do for now
        }
    }
}
