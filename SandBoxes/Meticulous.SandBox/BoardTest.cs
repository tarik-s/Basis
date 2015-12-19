using System;
using System.Collections.Generic;
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

    public enum Direction
    {
        Left, 
        Up, 
        Right, 
        Down
    };

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public class Cell
    {
        private readonly Board _board;

        public Cell(Board board)
        {
            _board = board;
        }

        public Board Board
        {
            get { return _board; }
        }
    }

    public class Wall
    {
        private readonly Board _board;
        private readonly Orientation _orientation;

        public Wall(Board board, Orientation orientation, Cell first, Cell second)
        {
            _board = board;
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

    public class Board
    {

    }

    public class BoardBuilder : IBuilder<Board>
    {
        public Board Build()
        {
            return new Board();
        }
    }
}
