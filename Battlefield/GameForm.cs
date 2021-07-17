using Battlefield.Enums;
using Battlefield.Models;
using Battlefield.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace Battlefield
{
    public partial class GameForm : Form
    {
        public static readonly Random Random = new Random();
        public static readonly List<GameObject> GameObjects = new List<GameObject>();
        public static readonly Size UnitSize = new Size(50, 50);
        public static Size GameClientSize;

        private PlayerCharacter _player;
        private Timer _timer = new Timer();
        private SoundPlayer _soundPlayerWin = new SoundPlayer(@"Sounds\win.wav");
        private SoundPlayer _soundPlayerGameOver = new SoundPlayer(@"Sounds\game-over.wav");
        private LevelReaderService _levelReaderService = new LevelReaderService();
        private int _defaultEnemyCount = 10;
        private int _enemyCountMultiplier = 5;
        private int _startLevel = 1;
        private int _lastLevel = 5;
        private int _level;
        private int _currentEnemyCount;
        private int _score;
        private bool _gameIsRunning = false;

        public GameForm()
        {
            InitializeComponent();
            SetInitialSizes();
            InitTimer();
            PrepareNewGame();
            InitGameObjects();

            _gameIsRunning = true;
        }

        private void SetInitialSizes()
        {
            int newWidth = ClientSize.Width - ClientSize.Width % UnitSize.Width;
            int newHeight = ClientSize.Height - ClientSize.Height % UnitSize.Height;

            ClientSize = new Size(newWidth, newHeight);

            GameClientSize = ClientSize;
        }

        private void InitGameObjects()
        {
            int halfWidth = ClientSize.Width / 2;
            int halfHeight = ClientSize.Height / 2;
            halfWidth = halfWidth / UnitSize.Width * UnitSize.Width + UnitSize.Width;
            halfHeight = halfHeight / UnitSize.Height * UnitSize.Height + UnitSize.Height;

            GameObjects.ForEach(ob => Controls.Remove(ob.Picture));
            GameObjects.Clear();

            _player = new PlayerCharacter(this, new Point(halfWidth - 3 * UnitSize.Width + 1, ClientSize.Height - UnitSize.Height + 1));

            GameObjects.Add(_player);

            GameObjects.Add(new EnemyCharacter(this, _level, new Point(1, 1), (DirectionEnum)Random.Next(1, 5)));
            GameObjects.Add(new EnemyCharacter(this, _level, new Point(halfWidth - UnitSize.Width + 1, 1), (DirectionEnum)Random.Next(1, 5)));
            GameObjects.Add(new EnemyCharacter(this, _level, new Point(ClientSize.Width - UnitSize.Width + 1, 1), (DirectionEnum)Random.Next(1, 5)));

            GameObjects.Add(new Capybara(this, new Point(halfWidth - UnitSize.Width, ClientSize.Height - UnitSize.Height)));

            GameObjects.Add(new Obstacle(this, new Point(halfWidth - 2 * UnitSize.Width, ClientSize.Height - 2 * UnitSize.Height), true));
            GameObjects.Add(new Obstacle(this, new Point(halfWidth - UnitSize.Width, ClientSize.Height - 2 * UnitSize.Height), true));
            GameObjects.Add(new Obstacle(this, new Point(halfWidth, ClientSize.Height - 2 * UnitSize.Height), true));
            GameObjects.Add(new Obstacle(this, new Point(halfWidth - 2 * UnitSize.Width, ClientSize.Height - UnitSize.Height), true));
            GameObjects.Add(new Obstacle(this, new Point(halfWidth, ClientSize.Height - UnitSize.Height), true));

            InitObstacles(halfWidth, halfHeight);
        }

        private void InitObstacles(int halfWidth, int halfHeight)
        {
            var obstaclePoints = _levelReaderService.GetLevelPoints(_level);

            var height = obstaclePoints.GetLength(0);
            var width = obstaclePoints.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (obstaclePoints[i, j] == 1)
                    {
                        GameObjects.Add(new Obstacle(this, new Point(j * UnitSize.Height, (i + 1) * UnitSize.Width)));
                    }
                }
            }
        }

        private void InitTimer()
        {
            _timer.Interval = 10;
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (!_gameIsRunning)
            {
                return;
            }

            foreach (Unit gameObject in GameObjects.Where(ob => ob is Unit).ToList())
            {
                gameObject.Move();
            }

            var removedObjects = GameObjects.Where(ob => ob.IsDestroyed).ToList();
            foreach (var gameObject in removedObjects)
            {
                if (gameObject is EnemyCharacter enemyCharacter)
                {
                    _score += enemyCharacter.IsStrong ? 2 : 1;
                    _currentEnemyCount--;

                    if (enemyCharacter.IsStrong && !enemyCharacter.IsExploded && _currentEnemyCount > 0)
                    {
                        Point bonusPoint = default;

                        do
                        {
                            bonusPoint.X = Random.Next(ClientSize.Width / UnitSize.Width) * UnitSize.Width;
                            bonusPoint.Y = Random.Next(ClientSize.Height / UnitSize.Height) * UnitSize.Height;
                        } while (GameObjects.Any(ob => ob.HasCollision(bonusPoint, UnitSize)));

                        var existingBonus = GameObjects.FirstOrDefault(ob => ob is Bonus);
                        if (existingBonus != null)
                        {
                            existingBonus.IsDestroyed = true;
                        }

                        GameObjects.Add(new Bonus(this, bonusPoint, (BonusTypeEnum)Random.Next(6)));
                    }
                }

                Controls.Remove(gameObject.Picture);
                GameObjects.Remove(gameObject);
            }

            if (!GameObjects.Any(ob => ob is Capybara) || !GameObjects.Any(ob => ob is PlayerCharacter))
            {
                _player.Stop();

                _gameIsRunning = false;

                _soundPlayerGameOver.Play();

                var result = MessageBox.Show($"Game is over. Your score is {_score}. Start a new game?", "Info", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    PrepareNewGame();
                    InitGameObjects();

                    _gameIsRunning = true;
                }
                else
                {
                    Close();
                }
            }

            var enemiesVisible = GameObjects.Count(ob => ob is EnemyCharacter);
            if (_currentEnemyCount == 0)
            {
                _player.Stop();

                _gameIsRunning = false;

                _level++;
                _currentEnemyCount = _defaultEnemyCount + _enemyCountMultiplier * _level;

                _soundPlayerWin.PlaySync();

                if (_level == _lastLevel + 1)
                {
                    var result = MessageBox.Show($"Congratulations, you won. Your score is {_score}. Start a new game?", "Info", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        PrepareNewGame();
                        InitGameObjects();

                        _gameIsRunning = true;
                    }
                    else
                    {
                        Close();
                    }
                }
                else
                {
                    InitGameObjects();

                    _gameIsRunning = true;
                }

                //TODO: do not collide enemy bullets
            }
            else if (_currentEnemyCount >= 3 && enemiesVisible < 3 || _currentEnemyCount > 0 && _currentEnemyCount < 3 && enemiesVisible < _currentEnemyCount)
            {
                var spawnPoints = new List<Point>
                {
                    new Point(1, 1),
                    new Point(ClientSize.Width / 2 - UnitSize.Width + 1, 1),
                    new Point(ClientSize.Width - UnitSize.Width + 1, 1)
                };

                var characterSize = new Size(UnitSize.Width - 2, UnitSize.Height - 2);

                int tries = 0;
                int maxTries = 30;
                int spawnIndex;
                do
                {
                    spawnIndex = Random.Next(3);
                    tries++;
                } while (GameObjects.Any(ob => ob.HasCollision(spawnPoints[spawnIndex], characterSize)) && tries < maxTries);

                if (tries < maxTries)
                {
                    bool isStrong = Random.Next(5) < 2;
                    GameObjects.Add(new EnemyCharacter(this, _level, spawnPoints[spawnIndex], (DirectionEnum)Random.Next(1, 5), isStrong));
                }
            }

            SetInfoText();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            foreach (var gameObject in GameObjects)
            {
                gameObject.Draw(e.Graphics);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_gameIsRunning && e.KeyCode != Keys.Escape)
            {
                return;
            }

            bool isStopped = !_player.IsMoving;

            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                case Keys.Left:
                    _player.HandleArrowKeyDown((DirectionEnum)Enum.Parse(typeof(DirectionEnum), e.KeyCode.ToString()));
                    break;
                case Keys.Space:
                    _player.Shoot();
                    break;
                case Keys.Escape:
                    _gameIsRunning = !_gameIsRunning;
                    SetInfoText();
                    break;
                default:
                    break;
            }

            if (isStopped && _player.IsMoving)
            {
                _player.Ignite();
            }

            Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            bool isMoving = _player.IsMoving;

            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                case Keys.Left:
                    _player.HandleArrowKeyUp((DirectionEnum)Enum.Parse(typeof(DirectionEnum), e.KeyCode.ToString()));
                    break;
                default:
                    break;
            }

            if (isMoving && !_player.IsMoving)
            {
                _player.Stop();
            }

            Invalidate();
        }

        private void SetInfoText()
        {
            string pausedText = _gameIsRunning ? string.Empty : " - Paused";

            Text = $"Battlefield{pausedText} | LVL - {_level} | 🚚 - {_currentEnemyCount} | 🗲 - {_player.Speed}/{_player.MaxSpeed} | ⁍⁍ - {_player.BonusBulletSpeed} | ★ - {_player.Damage} | ♥ - {_player.Health} | $ - {_score}";
        }

        private void PrepareNewGame()
        {
            _level = _startLevel;
            _currentEnemyCount = _defaultEnemyCount;
            _score = 0;
        }
    }
}
