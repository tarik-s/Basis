using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Patterns;

namespace Meticulous.SandBox
{
    public enum Side
    {
        Left, 
        Top, 
        Right, 
        Bottom
    }

    [Flags]
    public enum Direction
    {
        Left = 1, 
        Up = 2, 
        Right = 4, 
        Down = 8,

        LeftUp = Left|Up,
        RightUp = Right|Up,
        RightDown = Right|Down,
        LeftDown = Left|Down
    };

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public class Wall
    {
        private readonly Board _board;
        private readonly Orientation _orientation;

        public Wall(Board board, Orientation orientation, Cell first, Cell second)
        {
            _board = board;
            _orientation = orientation;
        }

        public Board Board
        {
            get { return _board; }
        }

        public Orientation Orientation
        {
            get { return _orientation; }
        }
    }


    internal static class Helper
    {
        private static readonly Direction[] s_directions;

        static Helper()
        {
            s_directions = new Direction[]
            {
                Direction.Left, Direction.Up, Direction.Right, Direction.Down,
                Direction.LeftUp, Direction.RightUp, Direction.RightDown, Direction.LeftDown
            };
        }

        public static void CheckDirection(Direction direction)
        {
            if (Array.IndexOf(s_directions, direction) == -1)
                throw new ArgumentException("Invalid direction", "direction");
        }

        public static void CheckPosition(Point position, Size size)
        {
            if (position.X < 0 || position.Y < 0 || position.X >= size.Width || position.Y >= size.Height)
                throw new ArgumentException("Invalid position", "position");
        }

        public static bool IsValid(Point position, Size size)
        {
            if (position.X < 0 || position.Y < 0 || position.X >= size.Width || position.Y >= size.Height)
                return false;

            return true;
        }

        public static Point GetOffset(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left: return new Point(-1, 0);
                case Direction.Up: return new Point(0, -1);
                case Direction.Right: return new Point(1, 0);
                case Direction.Down: return new Point(0, 1);

                case Direction.LeftUp: return new Point(-1, -1);
                case Direction.RightUp: return new Point(1, -1);
                case Direction.RightDown: return new Point(1, 1);
                case Direction.LeftDown: return new Point(-1, 1);
                default:
                    throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }
    }

    public class Cell
    {
        private readonly Board _board;
        private readonly Point _pos;
        private CellState _state;

        public Cell(Board board, Point position)
        {
            _board = board;
            _pos = position;
            _state = new CellState(this);
        }

        public Board Board
        {
            get { return _board; }
        }

        public int X
        {
            get { return _pos.X; }
        }

        public int Y
        {
            get { return _pos.Y; }
        }

        public Point Position
        {
            get { return _pos; }
        }

        public Cell GetNext(Direction direction)
        {
            return _board.GetNextCell(_pos, direction);
        }
    }

    public class CellState
    {
        private readonly Cell _cell;

        public CellState(Cell cell)
        {
            _cell = cell;
        }

        public Cell Cell
        {
            get { return _cell; }
        }
    }

    public class Board
    {
        private readonly Size _size;
        private readonly Cell[,] _cells;

        internal Board(BoardBuilder builder)
        {
            _size = builder.Size;
            _cells = new Cell[_size.Width, _size.Height];
        }

        public Size Size
        {
            get { return _size; }
        }

        public Cell GetCell(Point position)
        {
            Helper.CheckPosition(position, _size);

            return _cells[position.X, position.Y];
        }

        public Cell GetNextCell(Point position, Direction direction)
        {
            Helper.CheckPosition(position, _size);

            var newPos = position;
            var offset = Helper.GetOffset(direction);
            newPos.Offset(offset);
            if (!Helper.IsValid(newPos, _size))
                return null;

            return _cells[newPos.X, newPos.Y];
        }
    }

    public class BoardBuilder : IBuilder<Board>
    {
        private readonly Size _size;
#pragma warning disable 414
        private readonly int _minSequenceSize;
#pragma warning restore 414
        private readonly int _maxSequenceSize;

        public BoardBuilder(Size size)
        {
            _size = size;
            _minSequenceSize = 1;
            _maxSequenceSize = _size.Width * _size.Height;
        }

        public Size Size
        {
            get { return _size; }
        }

        public Board Build()
        {
            return new Board(this);
        }
    }
}
