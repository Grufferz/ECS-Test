using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxRooms;
        private readonly int _roomMaxSize;
        private readonly int _roomMinSize;

        private readonly Core.DungeonMap _map;
        private EntityManager _entManager;

        public MapGenerator(int width, int height, int maxRooms,
            int roomMaxSize, int roomMinSize, int mapLevel, EntityManager em)
        {
            _width = width;
            _height = height;
            _maxRooms = maxRooms;
            _roomMaxSize = roomMaxSize;
            _roomMinSize = roomMinSize;
            _entManager = em;
            _map = new Core.DungeonMap();
        }

        public Core.DungeonMap CreateMap()
        {
            _map.Initialize(_width, _height);

            for (int r = _maxRooms; r > 0; r--)
            {
                int roomWidth = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomHeight = Game.Random.Next(_roomMinSize, _roomMaxSize);
                int roomXPos = Game.Random.Next(0, _width - roomWidth - 1);
                int roomYPos = Game.Random.Next(0, _height - roomHeight - 1);

                var newRoom = new RogueSharp.Rectangle(roomXPos, roomYPos, roomWidth, roomHeight);

                bool newRoomIntersects = _map.Rooms.Any(room => newRoom.Intersects(room));

                if (!newRoomIntersects)
                {
                    _map.Rooms.Add(newRoom);
                }
            }

            foreach (RogueSharp.Rectangle room in _map.Rooms)
            {
                CreateRoom(room);
            }

            for (int r = 1; r < _map.Rooms.Count; r++)
            {
                int previousRoomCentreX = _map.Rooms[r - 1].Center.X;
                int previousRoomCentreY = _map.Rooms[r - 1].Center.Y;
                int CurrentRoomCentreX = _map.Rooms[r].Center.X;
                int CurrentRoomCentreY = _map.Rooms[r].Center.Y;

                if (Game.Random.Next(1, 2) == 1)
                {
                    CreateHorizontalTunnel(previousRoomCentreX, CurrentRoomCentreX, previousRoomCentreY);
                    CreateVerticalTunnel(previousRoomCentreY, CurrentRoomCentreY, CurrentRoomCentreX);
                }
                else
                {
                    CreateVerticalTunnel(previousRoomCentreY, CurrentRoomCentreY, previousRoomCentreX);
                    CreateHorizontalTunnel(previousRoomCentreX, CurrentRoomCentreX, CurrentRoomCentreY);
                }
            }

            foreach(RogueSharp.Rectangle room in _map.Rooms)
            {
                CreateDoors(room);
            }

            CreateStairs();
            return _map;
        }

        private void CreateDoors( RogueSharp.Rectangle room )
        {
            int xMin = room.Left;
            int xMax = room.Right;
            int yMin = room.Top;
            int yMax = room.Bottom;

            List<RogueSharp.ICell> borderCells 
                = _map.GetCellsAlongLine(xMin, yMin, xMax, yMin).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMin, xMin, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMin, yMax, xMax, yMax));
            borderCells.AddRange(_map.GetCellsAlongLine(xMax, yMin, xMax, yMax));

            foreach( RogueSharp.ICell c in borderCells )
            {
                if (IsPotentialDoor(c))
                {
                    _map.SetCellProperties(c.X, c.Y, false, true, true);
                    _map.Doors.Add(new Core.DoorHelper(c.X, c.Y, false));

                    // create door entity - needs pos, render, door
                    _entManager.AddDoor(c.X, c.Y, false);
                }
            }
        }

        public bool IsPotentialDoor(RogueSharp.ICell cell)
        {
           
            if (!cell.IsWalkable)
            {
                return false;
            }

            // get refs to all surrouding cells
            RogueSharp.ICell right = _map.GetCell(cell.X + 1, cell.Y);
            RogueSharp.ICell left = _map.GetCell(cell.X - 1, cell.Y);
            RogueSharp.ICell top = _map.GetCell(cell.X, cell.Y - 1);
            RogueSharp.ICell bottom = _map.GetCell(cell.X, cell.Y + 1);
            
            //make sure no doors here
            if (_map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null )
            {
                return false;
            }
            
            if (right.IsWalkable && left.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
                return true;
            }
            if (!right.IsWalkable && !left.IsWalkable && top.IsWalkable && bottom.IsWalkable)
            {
                return true;
            }
            return false;
        }

        public void PlaceGold(EntityManager em)
        {
            foreach (var room in _map.Rooms)
            {
                if (RogueSharp.DiceNotation.Dice.Roll("1d10") < 7)
                {
                    int amountOfGold = RogueSharp.DiceNotation.Dice.Roll("1d6");
                    for (int i = 0; i < amountOfGold; i++)
                    {
                        RogueSharp.Point randRoomLoc = _map.GetRandomWalkableLocationInRoom(room);
                        if (randRoomLoc != null)
                        {
                            if (RogueSharp.DiceNotation.Dice.Roll("1d10") < 8)
                            {
                                em.AddTreasure(randRoomLoc.X, randRoomLoc.Y);
                            }
                            else
                            {
                                em.AddPotionAtLocation(randRoomLoc.X, randRoomLoc.Y);
                            }
                        }
                    }
                }
            }
        }

        public void PlaceMonsters(EntityManager entMan)
        {
            foreach (var room in _map.Rooms)
            {
                if (RogueSharp.DiceNotation.Dice.Roll("1d100") > 5)
                {
                    RogueSharp.Point randRoomLoc = _map.GetRandomWalkableLocationInRoom(room);
                    if (randRoomLoc != null)
                    {
                        entMan.AddMonster(randRoomLoc.X, randRoomLoc.Y, _map);
                    }
                }

            }
        }

        private void CreateRoom(RogueSharp.Rectangle room)
        {
            for (int x = room.Left + 1; x < room.Right; x++)
            {
                for (int y = room.Top + 1; y < room.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, true);
                }
            }
        }

        private void CreateStairs()
        {
            // down stairs
            int x = _map.Rooms.Last().Center.X;
            int y = _map.Rooms.Last().Center.Y;
            _entManager.AddStairs(x, y, false);
            x = _map.Rooms.First().Center.X;
            y = _map.Rooms.First().Center.Y;
            _entManager.AddStairs(x, y, true);
        }

        private void CreateHorizontalTunnel( int xStart, int xEnd, int yPos )
        {
            for (int x = Math.Min( xStart, xEnd); x <= Math.Max( xStart, xEnd ); x++ )
            {
                _map.SetCellProperties(x, yPos, true, true, true);
            }
        }

        private void CreateVerticalTunnel( int yStart, int yEnd, int xPos )
        {
            for (int y = Math.Min( yStart, yEnd ); y <= Math.Max( yStart, yEnd ); y++)
            {
                _map.SetCellProperties(xPos, y, true, true, true);
            }
        }
    }
}
