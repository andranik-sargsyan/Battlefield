using Battlefield.Enums;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace Battlefield.Models
{
    abstract class Character : Unit
    {
        private string _color;
        private int _health;

        protected Control _control;

        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
                if (_health <= 0)
                {
                    IsDestroyed = true;
                }
            }
        }
        public int Damage { get; set; }
        public int MaxSpeed { get; set; } = 3;
        public int BonusBulletSpeed { get; set; } = 0;

        public Character()
        {
            _color = this is PlayerCharacter ? "blue" : "red";

            Size = new Size(GameForm.UnitSize.Width - 2, GameForm.UnitSize.Height - 2);

            _windowsMediaPlayer = new WindowsMediaPlayer();
        }

        public override void Collide(GameObject ob)
        {
            if (ob is Bullet bullet)
            {
                if (Health > 0)
                {
                    Health -= bullet.Damage;

                    if (this is EnemyCharacter enemyCharacter && Health <= 5)
                    {
                        enemyCharacter.Speed = 1;
                    }
                    else if (this is PlayerCharacter playerCharacter)
                    {
                        if (playerCharacter.Speed > 1)
                        {
                            playerCharacter.Speed--;
                        }
                        playerCharacter.MaxSpeed = 3;
                    }
                }

                if (Health > 0)
                {
                    _windowsMediaPlayer.URL = @"Sounds\hit-vehicle.wav";
                }
                else
                {
                    _windowsMediaPlayer.URL = @"Sounds\destroy-vehicle.wav";
                }
            }
        }

        public override void Draw(Graphics g)
        {
            string strongSuffix = this is EnemyCharacter enemy && enemy.IsStrong ? "-w" : string.Empty;
            string suffix = Health > 1 ? string.Empty : "-damaged";

            switch (Direction)
            {
                case DirectionEnum.Up:
                    Picture.ImageLocation = $@"Images\tank-{_color}{strongSuffix}-up{suffix}.png";
                    break;
                case DirectionEnum.Right:
                    Picture.ImageLocation = $@"Images\tank-{_color}{strongSuffix}-right{suffix}.png";
                    break;
                case DirectionEnum.Down:
                    Picture.ImageLocation = $@"Images\tank-{_color}{strongSuffix}-down{suffix}.png";
                    break;
                case DirectionEnum.Left:
                    Picture.ImageLocation = $@"Images\tank-{_color}{strongSuffix}-left{suffix}.png";
                    break;
                default:
                    break;
            }
        }

        public virtual void Shoot()
        {
            var doShoot = false;
            Point bulletPosition = Position;

            switch (Direction)
            {
                case DirectionEnum.Up:
                    if (Position.Y > GameForm.UnitSize.Height / 2)
                    {
                        doShoot = true;
                        bulletPosition = new Point(Position.X + GameForm.UnitSize.Width / 2 - GameForm.UnitSize.Width / 6, Position.Y - GameForm.UnitSize.Height / 3);
                    }
                    break;
                case DirectionEnum.Right:
                    if (Position.X + Size.Width < GameForm.GameClientSize.Width - GameForm.UnitSize.Width / 2)
                    {
                        doShoot = true;
                        bulletPosition = new Point(Position.X + GameForm.UnitSize.Width, Position.Y + GameForm.UnitSize.Height / 2 - GameForm.UnitSize.Height / 6);
                    }
                    break;
                case DirectionEnum.Down:
                    if (Position.Y + Size.Height < GameForm.GameClientSize.Height - GameForm.UnitSize.Height / 2)
                    {
                        doShoot = true;
                        bulletPosition = new Point(Position.X + GameForm.UnitSize.Width / 2 - GameForm.UnitSize.Width / 6, Position.Y + GameForm.UnitSize.Height);
                    }
                    break;
                case DirectionEnum.Left:
                    if (Position.X > GameForm.UnitSize.Width / 2)
                    {
                        doShoot = true;
                        bulletPosition = new Point(Position.X - GameForm.UnitSize.Width / 3, Position.Y + GameForm.UnitSize.Height / 2 - GameForm.UnitSize.Height / 6);
                    }
                    break;
                default:
                    break;
            }

            if (doShoot)
            {
                PlayShootingSound();

                GameForm.GameObjects.Add(new Bullet(_control, this, bulletPosition, Direction, Damage, BonusBulletSpeed));
            }
        }

        public void PlayShootingSound()
        {
            if (Damage > 1)
            {
                _windowsMediaPlayer.URL = @"Sounds\shooting-strong.wav";
            }
            else
            {
                if (this is PlayerCharacter)
                {
                    _windowsMediaPlayer.URL = @"Sounds\shooting-player.wav";
                }
                else
                {
                    _windowsMediaPlayer.URL = @"Sounds\shooting-enemy.wav";
                }
            }
        }
    }
}
