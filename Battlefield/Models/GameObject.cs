using System.Drawing;
using System.Windows.Forms;

namespace Battlefield.Models
{
    public abstract class GameObject
    {
        public bool IsDestroyed { get; set; }

        public PictureBox Picture { get; set; } = new PictureBox();

        public Point Position
        {
            get => Picture.Location;
            set => Picture.Location = value;
        }

        public Size Size
        {
            get => Picture.Size;
            set => Picture.Size = value;
        }

        public static bool HasCollision(Point point1, Size size1, Point point2, Size size2)
        {
            return !(
                point2.X >= point1.X + size1.Width ||
                point1.X >= point2.X + size2.Width ||
                point2.Y >= point1.Y + size1.Height ||
                point1.Y >= point2.Y + size2.Height);
        }

        public bool HasCollision(GameObject ob)
        {
            return HasCollision(ob.Position, ob.Size, Position, Size);
        }

        public bool HasCollision(Point point, Size size)
        {
            return HasCollision(point, size, Position, Size);
        }

        public bool IsInBounds(Point point, Control control)
        {
            return point.X >= 0 && point.X <= control.ClientSize.Width - Size.Width &&
                point.Y >= 0 && point.Y <= control.ClientSize.Height - Size.Height;
        }

        public bool ContainsPoint(Point point)
        {
            return Position.X <= point.X && point.X <= Position.X + Size.Width &&
                Position.Y <= point.X && point.X <= Position.Y + Size.Height;
        }

        public abstract void Collide(GameObject ob);
        public abstract void Draw(Graphics g);

        public GameObject()
        {
            Size = GameForm.UnitSize;

            Picture.SizeMode = PictureBoxSizeMode.StretchImage;
            Picture.Location = Position;
            Picture.Size = Size;
        }
    }
}
