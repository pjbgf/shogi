using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Shogi.Pieces
{
    public abstract class Piece
    {
        public Player OwnerPlayer { get; }
        public string Position { get; private set; }
        public char ShortName { get; protected set; }
        public bool CanMoveBack { get; protected set; }
        public bool CanMoveForwards { get; protected set; }
        public bool CanMoveForwardInRange { get; protected set; }
        public bool CanMoveDiagonallyInRange { get; protected set; }
        public bool CanMoveForwardsDiagonally { get; protected set; }
        public bool CanMoveBackwardsDiagonally { get; protected set; }
        public bool CanMoveHorizontallyAndVerticallyInRange { get; protected set; }

        public List<KeyValuePair<MovementValue, string>> PossibleMovements { get; protected set; }

        protected Piece(Player ownerPlayer, string position)
        {
            OwnerPlayer = ownerPlayer;
            PossibleMovements = new List<KeyValuePair<MovementValue, string>>();
            Position = position;
        }

        public virtual bool IsMoveLegal(string toPosition)
        {
            return (CanMoveBack && HasMovedBack(toPosition)) ||
                   (CanMoveForwards && HasMovedForwards(toPosition)) ||
                   (CanMoveForwardsDiagonally && HasMovedForwardsDiagonally(toPosition)) ||
                   (CanMoveBackwardsDiagonally && HasMovedBackwardsDiagonally(toPosition)) ||
                   (CanMoveDiagonallyInRange && HasMovedDiagonallyInRange(toPosition)) ||
                   (CanMoveForwardInRange && HasMovedForwardInRange(toPosition) ||
                    (CanMoveHorizontallyAndVerticallyInRange && HasMovedHorizontallyOrVerticallyInRange(toPosition)));
        }

        public virtual string Move(string toPosition)
        {
            var movementDescription = $"{ShortName}{Position}-{toPosition}";
            Position = toPosition;

            return movementDescription;
        }

        private bool HasMovedBack(string toPosition)
        {
            if (OwnerPlayer == Player.Black)
                return (Position[0] == toPosition[0] && (Position[1] + 1 == toPosition[1]));

            return (Position[0] == toPosition[0] && (Position[1] - 1 == toPosition[1]));
        }

        private bool HasMovedForwards(string toPosition)
        {
            if (OwnerPlayer == Player.Black)
                return (Position[0] == toPosition[0] && (Position[1] - 1 == toPosition[1]));

            return (Position[0] == toPosition[0] && (Position[1] + 1 == toPosition[1]));
        }

        private bool HasMovedForwardInRange(string toPosition)
        {
            if (OwnerPlayer == Player.Black)
                return (Position[0] == toPosition[0] && (Position[1] > toPosition[1]));

            return (Position[0] == toPosition[0] && (Position[1] < toPosition[1]));
        }

        private bool HasMovedForwardsDiagonally(string toPosition)
        {
            if (OwnerPlayer == Player.Black)
                return ((Position[0] + 1 == toPosition[0] || Position[0] - 1 == toPosition[0]) &&
                        (Position[1] - 1 == toPosition[1]));

            return ((Position[0] + 1 == toPosition[0] || Position[0] - 1 == toPosition[0]) &&
                    (Position[1] + 1 == toPosition[1]));
        }

        private bool HasMovedDiagonallyInRange(string toPosition)
        {
            var columnDiff = toPosition[0] - Position[0];
            var rowDiff = toPosition[1] - Position[1];

            if (columnDiff < 0)
                columnDiff *= -1;

            if (rowDiff < 0)
                rowDiff *= -1;

            return (rowDiff == columnDiff);
        }

        private bool HasMovedBackwardsDiagonally(string toPosition)
        {
            if (OwnerPlayer == Player.Black)
                return ((Position[0] + 1 == toPosition[0] || Position[0] - 1 == toPosition[0]) &&
                        (Position[1] + 1 == toPosition[1]));

            return ((Position[0] + 1 == toPosition[0] || Position[0] - 1 == toPosition[0]) &&
                    (Position[1] - 1 == toPosition[1]));
        }

        protected bool HasMovedHorizontallyOrVerticallyInRange(string toPosition)
        {
            return (Position[0] == toPosition[0] || Position[1] == toPosition[1]);
        }

        public virtual IEnumerable<string> GetPossibleMovements()
        {
            var possibleMovements = new List<string>();

            AddForwardMovements(possibleMovements);
            AddRangeForwardMovements(possibleMovements);
            AddRangeDiagonalMovements(possibleMovements);
            AddRangeBackwardMovements(possibleMovements);

            return possibleMovements;
        }

        private void AddRangeBackwardMovements(List<string> possibleMovements)
        {
            if (CanMoveHorizontallyAndVerticallyInRange)
            {
                for (int i = Position[1] - 1; i >= 'a'; i--)
                    possibleMovements.Add(string.Concat(Position, Position[0], Convert.ToChar(i)));
                for (int i = Position[1] + 1; i <= 'i'; i++)
                    possibleMovements.Add(string.Concat(Position, Position[0], Convert.ToChar(i)));
                for (int i = Position[0] - 1; i >= '1'; i--)
                    possibleMovements.Add(string.Concat(Position, Convert.ToChar(i), Position[1]));
                for (int i = Position[0] + 1; i <= '9'; i++)
                    possibleMovements.Add(string.Concat(Position, Convert.ToChar(i), Position[1]));
            }
        }

        private void AddRangeDiagonalMovements(ICollection<string> possibleMovements)
        {
            if (CanMoveDiagonallyInRange)
            {
                for (int i = Position[1]; i > 'a'; i--)
                {
                    var diff = (Position[1] - i) + 1;
                    possibleMovements.Add(string.Concat(Position, Convert.ToChar(Position[0] - diff),
                        Convert.ToChar(Position[1] - diff)));
                    possibleMovements.Add(string.Concat(Position, Convert.ToChar(Position[0] - diff),
                        Convert.ToChar(Position[1] + diff)));
                }

                for (int i = Position[1]; i < 'i'; i++)
                {
                    var diff = (i - Position[1]) + 1;
                    possibleMovements.Add(string.Concat(Position, Convert.ToChar(Position[0] + diff),
                        Convert.ToChar(Position[1] + diff)));
                    possibleMovements.Add(string.Concat(Position, Convert.ToChar(Position[0] + diff),
                        Convert.ToChar(Position[1] - diff)));
                }
            }
        }

        private void AddRangeForwardMovements(ICollection<string> possibleMovements)
        {
            if (CanMoveForwardInRange)
            {
                if (OwnerPlayer == Player.Black)
                    for (int i = Position[1] - 1; i >= 'a'; i--)
                        possibleMovements.Add(string.Concat(Position, Position[0], Convert.ToChar(i)));
                else
                    for (int i = Position[1] + 1; i <= 'i'; i++)
                        possibleMovements.Add(string.Concat(Position, Position[0], Convert.ToChar(i)));
            }
        }

        private void AddForwardMovements(ICollection<string> possibleMovements)
        {
            if (CanMoveForwards)
            {
                if (OwnerPlayer == Player.Black)
                    possibleMovements.Add(string.Concat(Position, Position[0], Convert.ToChar(Position[1] - 1)));
                else
                    possibleMovements.Add(string.Concat(Position, Position[0], Convert.ToChar(Position[1] + 1)));
            }
        }
    }
}