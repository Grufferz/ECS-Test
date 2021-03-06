﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class DungeonMap : RogueSharp.Map
    {
        public List<RogueSharp.Rectangle> Rooms;
        public List<DoorHelper> Doors;

        public DungeonMap()
        {
            Rooms = new List<RogueSharp.Rectangle>();
            Doors = new List<DoorHelper>();
        }

        public bool DoesRoomHaveWalkableSpace(RogueSharp.Rectangle room)
        {
            for( int x = 1; x <= room.Width; x++ )
            {
                for( int y = 1; y <= room.Height; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))   
                    {
                        return true;
                    }   
                }
            }
            return false;
        }

        public void Draw(RLNET.RLConsole mapConsole)
        {
            mapConsole.Clear();
            foreach( RogueSharp.Cell cell in GetAllCells() )
            {
                SetConsoleSymbolForCell(mapConsole, cell);
            }
        }

        public RogueSharp.Point GetTargetInRandomRoom(int curX, int curY)
        {
            RogueSharp.Point rp = new RogueSharp.Point();

            RogueSharp.Point curPoint = new RogueSharp.Point(curX, curY);

            Random rdm = new Random();
            RogueSharp.Rectangle selectedRoom;
            bool found = false;
            while (!found)
            {
                selectedRoom = Rooms[rdm.Next(Rooms.Count)];
                if (!selectedRoom.Contains(curPoint))
                {
                    rp = GetRandomWalkableLocationInRoom(selectedRoom);
                    found = true;
                }
            }

            return rp;
        }

        public DoorHelper GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        public RogueSharp.Point GetRandomWalkableLocationInRoom( RogueSharp.Rectangle room )
        {
            if (DoesRoomHaveWalkableSpace( room ))
            {
                for( int i = 0; i < 100; i++ )
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if (IsWalkable(x, y))
                    {
                        return new RogueSharp.Point(x, y);
                    }
                }
            }
            return default(RogueSharp.Point);
        }

        private void SetConsoleSymbolForCell( RLNET.RLConsole console, RogueSharp.Cell cell)
        {
            // if not explored don't draw
            if (!cell.IsExplored)
            {
                return;
            }
            // if cell in FOV draw with lighter colour
            if (IsInFov(cell.X, cell.Y))
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colours.FloorFov, Colours.FloorBackgroundFov, '.');
                }
                else
                {
                    console.Set(cell.X, cell.Y, Colours.WallFov, Colours.WallBackgroundFov, '#');
                }
            }
            else
            {
                if (cell.IsWalkable)
                {
                    console.Set(cell.X, cell.Y, Colours.Floor, Colours.FloorBackground, '.');
                }
                else
                {   
                    console.Set(cell.X, cell.Y, Colours.Wall, Colours.WallBackground, '#');
                }
            }
        }

        public void SetIsWalkable(int x, int y, bool isWalkable )
        {
            RogueSharp.Cell cell = (RogueSharp.Cell)GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        public void UpdateFOVForMonsters(Systems.EntityManager em)
        {
            int bitmask = ((int)ComponentTypes.Attributes | (int)ComponentTypes.Position);
            List<int> entList = em.GetEntIDByBit(bitmask);
            Dictionary<int, List<Components.Component>> entities = em.GetEntities();
            List<Components.Component> cList;
            foreach( int ent in entList )
            {
                if (entities.TryGetValue(ent, out cList))
                {
                    int aware;
                    int x, y;
                    Components.PositionComp pos = 
                        (Components.PositionComp)cList.Find(s => s.CompType == ComponentTypes.Position);
                    if (pos != null)
                    {
                        Components.AttributesComp at =
                            (Components.AttributesComp)cList.Find(s => s.CompType == ComponentTypes.Attributes);
                        if (at != null)
                        {
                            aware = at.Awareness;
                            x = pos.X;
                            y = pos.Y;

                            ComputeFov(x, y, aware, true);
                            foreach(RogueSharp.Cell cell in GetAllCells())
                            {
                                if ( IsInFov(cell.X, cell.Y))
                                {
                                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
