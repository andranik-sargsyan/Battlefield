using Battlefield.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace Battlefield.Models
{
    class PlayerCharacter : Character
    {
        private WindowsMediaPlayer _windowsMediaPlayerMoving = new WindowsMediaPlayer();
        private Stopwatch _stopwatch = new Stopwatch();
        private List<DirectionEnum> _downKeys = new List<DirectionEnum>();

        public bool IsMoving { get; set; }

        public PlayerCharacter(Control control, Point position, DirectionEnum direction = DirectionEnum.Up)
        {
            _control = control;

            Position = position;
            Health = 4;
            Damage = 1;
            Direction = direction;
            Speed = 1;

            Picture.Image = Image.FromFile(@"Images\tank-blue-up.png");

            _windowsMediaPlayerMoving.settings.setMode("Loop", true);

            control.Controls.Add(Picture);
        }

        public override void Move()
        {
            if (!IsMoving)
            {
                return;
            }

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

            if (_stopwatch.Elapsed.Seconds > 2 && Health > 1)
            {
                Speed = 2;
            }
            if (_stopwatch.Elapsed.Seconds > 4 && Health > 2)
            {
                Speed = 3;
            }
            if (_stopwatch.Elapsed.Seconds > 6 && Health > 3 && MaxSpeed == 4)
            {
                Speed = 4;
            }

            if (IsInBounds(newPosition, _control) &&
                !GameForm.GameObjects.Any(ob => !(ob is PlayerCharacter) && !(ob is Bonus) && ob.HasCollision(newPosition, Size)))
            {
                Position = newPosition;
            }
            else
            {
                Speed = 1;
            }

            var bonus = GameForm.GameObjects.FirstOrDefault(ob => ob is Bonus);
            if (bonus != null && HasCollision(bonus))
            {
                bonus.Collide(this);
            }
        }

        public void Ignite()
        {
            _stopwatch.Restart();

            _windowsMediaPlayerMoving.URL = @"Sounds\moving.wav";
        }

        public void Stop()
        {
            _stopwatch.Stop();
            _windowsMediaPlayerMoving.controls.stop();

            Speed = 1;
        }

        public void HandleArrowKeyDown(DirectionEnum direction)
        {
            IsMoving = true;
            Direction = direction;

            if (!_downKeys.Contains(direction))
            {
                _downKeys.Add(direction);
            }
        }

        public void HandleArrowKeyUp(DirectionEnum direction)
        {
            _downKeys.Remove(direction);

            if (_downKeys.Count == 0)
            {
                IsMoving = false;
            }
            else
            {
                Direction = _downKeys[_downKeys.Count - 1];
            }
        }
    }
}
