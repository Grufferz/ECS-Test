using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class MovementSystem : IBaseSystem
    {
        private Core.DungeonMap _dungeonMap;
        private EntityManager _entityManager;

        public MovementSystem(Core.DungeonMap dungeon, EntityManager eman)
        {
            _dungeonMap = dungeon;
            _entityManager = eman;
            Core.EventBus.Subscribe(Core.EventTypes.ActionReqMove, (sender, e) => OnMessage(e));
            Core.EventBus.Subscribe(Core.EventTypes.MoveOK, (sender, e) => OnMessage(e));
            Core.EventBus.Subscribe(Core.EventTypes.DirectMove, (sender, e) => OnMessage(e));
            Core.EventBus.Subscribe(Core.EventTypes.NoMove, (sender, e) => OnMessage(e));
        }

        public void Update()
        {

        }

        public void OnMessage(Core.MessageEventArgs e)
        {
            switch (e.MessageType)
            {
                case Core.EventTypes.ActionReqMove:
                    // request movement
                    Core.MovementReqEventArgs moveReq = (Core.MovementReqEventArgs)e;
                    Core.Entity entoToMove = moveReq.EntRequestingMove;
                    int eid = entoToMove.UID;
                    Core.Directions dirToMove = moveReq.Direction;

                    Components.PositionComp posComp
                        = (Components.PositionComp)_entityManager.GetSingleComponentByID(eid, Core.ComponentTypes.Position);
                    int newX = posComp.X;
                    int newY = posComp.Y;
                    switch (dirToMove)
                    {
                        case Core.Directions.None:

                            break;
                        case Core.Directions.DownLeft:
                            newX = posComp.X - 1;
                            newY = posComp.Y + 1;
                            break;
                        case Core.Directions.Down:
                            newX = posComp.X;
                            newY = posComp.Y + 1;
                            break;
                        case Core.Directions.DownRight:
                            newX = posComp.X + 1;
                            newY = posComp.Y + 1;
                            break;
                        case Core.Directions.Left:
                            newX = posComp.X - 1;
                            newY = posComp.Y;
                            break;
                        case Core.Directions.Centre:

                            break;
                        case Core.Directions.Right:
                            newX = posComp.X + 1;
                            newY = posComp.Y;
                            break;
                        case Core.Directions.UpLeft:
                            newX = posComp.X - 1;
                            newY = posComp.Y - 1;
                            break;
                        case Core.Directions.Up:
                            newX = posComp.X;
                            newY = posComp.Y - 1;
                            break;
                        case Core.Directions.UpRight:
                            newX = posComp.X + 1;
                            newY = posComp.Y - 1;
                            break;
                    }

                    //check for collision

                    Core.CollisionEventArgs msg = new Core.CollisionEventArgs(Core.EventTypes.CollisionCheck, entoToMove, newX, newY);
                    Core.EventBus.Publish(Core.EventTypes.CollisionCheck, msg);

                    break;

                case Core.EventTypes.MoveOK:
                    
                    Core.MoveOkayEventArgs m = (Core.MoveOkayEventArgs)e;
                    if (_dungeonMap.GetCell(m.newX, m.newY).IsWalkable)
                    {
                        // get current pos

                        Core.Entity ent = m.EntRequestingMove;
                        Components.PositionComp pos 
                            = (Components.PositionComp) _entityManager.GetSingleComponentByID(ent.UID, Core.ComponentTypes.Position);
                        Components.AIComp aiComp
                            = (Components.AIComp)_entityManager.GetSingleComponentByID(ent.UID, Core.ComponentTypes.AI);

                        aiComp.TurnsSinceMove = 0;
                        _dungeonMap.SetIsWalkable(pos.X, pos.Y, true);
                        _entityManager.RemoveEntFromPosition(pos.X, pos.Y, ent);
                        pos.X = m.newX;
                        pos.Y = m.newY;
                        _dungeonMap.SetIsWalkable(m.newX, m.newY, false);
                        _entityManager.AddEntToPosition(m.newX, m.newY, ent);
                        
                    }
                    break;
                case Core.EventTypes.DirectMove:

                    Core.DirectMoveEventArgs dm = (Core.DirectMoveEventArgs)e;

                    //check for collision

                    Core.CollisionEventArgs checkMsg 
                        = new Core.CollisionEventArgs(Core.EventTypes.CollisionCheck, 
                        dm.EntRequestingMove, dm.PointToMoveTo.X, dm.PointToMoveTo.Y);
                    Core.EventBus.Publish(Core.EventTypes.CollisionCheck, checkMsg);

                    break;
                case Core.EventTypes.NoMove:

                    Core.NoMoveEventArgs nmEvent = (Core.NoMoveEventArgs)e;

                    Components.AIComp aiC =
                        (Components.AIComp)_entityManager.GetSingleComponentByID(nmEvent.EntNotMoving.UID, Core.ComponentTypes.AI);
                    aiC.TurnsSinceMove++;
                    //Game.MessageLog.Add($"Turns Since = {aiC.TurnsSinceMove.ToString()}");
                    break;
            }
        }
    }
}
