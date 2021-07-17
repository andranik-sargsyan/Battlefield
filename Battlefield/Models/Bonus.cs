using Battlefield.Enums;
using System.Drawing;
using System.Linq;
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

                    int halfWidth = GameForm.GameClientSize.Width / 2;
                    halfWidth = halfWidth / GameForm.UnitSize.Width * GameForm.UnitSize.Width + GameForm.UnitSize.Width;

                    var shieldPoints = new Point[]
                    {
                        new Point(halfWidth - 2 * GameForm.UnitSize.Width, GameForm.GameClientSize.Height - 2 * GameForm.UnitSize.Height),
                        new Point(halfWidth - GameForm.UnitSize.Width, GameForm.GameClientSize.Height - 2 * GameForm.UnitSize.Height),
                        new Point(halfWidth, GameForm.GameClientSize.Height - 2 * GameForm.UnitSize.Height),
                        new Point(halfWidth - 2 * GameForm.UnitSize.Width, GameForm.GameClientSize.Height - GameForm.UnitSize.Height),
                        new Point(halfWidth, GameForm.GameClientSize.Height - GameForm.UnitSize.Height)
                    };

                    foreach (var point in shieldPoints)
                    {
                        if (!GameForm.GameObjects.Any(obj => obj.HasCollision(point, GameForm.UnitSize)))
                        {
                            GameForm.GameObjects.Add(new Obstacle(_control, point, true));
                        }
                    }
                    break;
                case BonusTypeEnum.Speed:
                    if (ob is PlayerCharacter playerCharacter)
                    {
                        playerCharacter.MaxSpeed = 4;
                    }
                    else if (ob is EnemyCharacter enemyCharacter)
                    {
                        enemyCharacter.MaxSpeed = 3;
                        if (enemyCharacter.Speed < enemyCharacter.MaxSpeed)
                        {
                            enemyCharacter.Speed++;
                        }
                    }
                    break;
                case BonusTypeEnum.Bullet:
                    (ob as Character).BonusBulletSpeed++;
                    break;
                case BonusTypeEnum.Explode:
                    if (ob is PlayerCharacter)
                    {
                        var characters = GameForm.GameObjects.Where(obj => obj is EnemyCharacter);
                        foreach (EnemyCharacter character in characters)
                        {
                            character.IsExploded = true;
                            character.IsDestroyed = true;
                        }
                    }
                    else if (ob is EnemyCharacter)
                    {
                        var shieldObstacles = GameForm.GameObjects.Where(obj => obj is Obstacle obstacle && obstacle.IsShield);
                        foreach (Obstacle obstacle in shieldObstacles)
                        {
                            obstacle.IsDestroyed = true;
                        }

                        Character character = GameForm.GameObjects.FirstOrDefault(obj => obj is PlayerCharacter) as Character;
                        character.Health--;
                    }
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
