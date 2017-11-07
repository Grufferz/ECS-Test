using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class AISystem : IBaseSystem
    {
        private EntityManager _em;
        private GarbageSystem _garbageSystem;
        private Random rdm;
        private Core.DungeonMap _dungeonMap;
        private RogueSharp.PathFinder pFinder;

        public AISystem(EntityManager em, Core.DungeonMap dm, GarbageSystem gs)
        {
            _em = em;
            _dungeonMap = dm;
            _garbageSystem = gs;
            rdm = new Random();

            Core.EventBus.Subscribe(Core.EventTypes.AIRequest, (sender, e) => OnMessage(e));
        }

        public void Update()
        {

        }

        public void OnMessage(Core.MessageEventArgs e)
        {
            switch (e.MessageType)
            {
                case Core.EventTypes.AIRequest:
                    Core.AIReqMessageEventArgs msg = (Core.AIReqMessageEventArgs)e;

                    List<Components.Component> curEntCompList = _em.GetCompsByID(msg.entRequestingMove.UID);
                    Components.AIComp aiComp
                        = (Components.AIComp)curEntCompList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.AI);
                    Components.PositionComp posComp
                        = (Components.PositionComp)curEntCompList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);

                    //Core.Directions directionToMove;
                    //directionToMove = Core.Directions.None;
                    int turnsSinceMove = aiComp.TurnsSinceMove;


                    // are we standing on anything?
                    bool standing = false;
                    string posKey = posComp.X.ToString() + "-" + posComp.Y.ToString();
                    if (_em.EntityPostionLookUp.ContainsKey(posKey))
                    {
                        foreach (Core.Entity entOnPos in _em.EntityPostionLookUp[posKey])
                        {
                            if (_em.CheckEntForBits(entOnPos.UID, (int)Core.ComponentTypes.Collectable))
                            {

                            }

                        }
                    }


                    // are we sleeping?
                    if (aiComp.AiState == Core.AIStates.Sleeping)
                    {
                        // chance of walking up?

                        if (RogueSharp.DiceNotation.Dice.Roll("1d10") > 3)
                        {
                            aiComp.AiState = Core.AIStates.Exiting;
                        }

                        Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, msg.entRequestingMove);
                        Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
                    }
                    else
                    {
                        // have we got a target?
                        if (aiComp.GotPath)
                        {
                            //start following path
                            FollowPath(aiComp, posComp, msg.entRequestingMove);
                        }
                        else
                        {
                            // are we at target?
                            if (AtTarget(aiComp, posComp))
                            {
                                //TODO - Do something when you get to target
                                Game.MessageLog.Add("AT TARGET!!!!!!");
                                // remove ent from map
                                _dungeonMap.SetCellProperties(posComp.X, posComp.Y, false, true);
                                // delete entity
                                Core.DeleteEntEventArgs deleteEv = new Core.DeleteEntEventArgs(Core.EventTypes.DeleteEntity, msg.entRequestingMove.UID);
                                Core.EventBus.Publish(Core.EventTypes.DeleteEntity, deleteEv);

                            }
                            else
                            {
                                // entity needs a target
                                if (!NextToTarget(aiComp, posComp))
                                {
                                    aiComp.PathToTarget = null;
                                    bool targetFound = SetTargetToExit(msg.entRequestingMove);
                                    aiComp.GotPath = targetFound;

                                    //aiComp.PathToTarget = null;
                                    //Game.MessageLog.Add($"Looking For Exit: Route Found = {targetFound.ToString()}");

                                    Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, msg.entRequestingMove);
                                    Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
                                }
                                else
                                {
                                    // entity IS next to target, move there
                                    Game.MessageLog.Add("I'm Next To Target!");
                                    aiComp.GotPath = false;
                                    aiComp.PathToTarget = null;
                                    RogueSharp.Point nm = new RogueSharp.Point(aiComp.Target.X, aiComp.Target.Y);
                                    Core.DirectMoveEventArgs moveReq
                                        = new Core.DirectMoveEventArgs(Core.EventTypes.DirectMove, msg.entRequestingMove, nm);
                                    Core.EventBus.Publish(Core.EventTypes.ActionReqMove, moveReq);
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private bool AtTarget(Components.AIComp aiC, Components.PositionComp posC)
        {
            if (posC.X == aiC.Target.X && posC.Y == aiC.Target.Y)
            {
                return true;
            }
            return false;
        }

        private bool NextToTarget(Components.AIComp aiC, Components.PositionComp posC)
        {
            IEnumerable<RogueSharp.ICell> adjacents = _dungeonMap.GetBorderCellsInSquare(posC.X, posC.Y, 1);
            foreach (RogueSharp.Cell eachCell in adjacents)
            {
                if (eachCell.X == aiC.Target.X && eachCell.Y == aiC.Target.Y)
                {
                    return true;
                }
            }
            return false;
        }
        
        private void FollowPath(Components.AIComp aiC, Components.PositionComp posC, Core.Entity entMoving)
        {
            try
            {
                RogueSharp.Cell nextMove = (RogueSharp.Cell)aiC.PathToTarget.StepForward();

                // is mext cell last one?  If not then just walk path
                if (nextMove != (RogueSharp.Cell)aiC.PathToTarget.End)
                {
                    // follow path
                    RogueSharp.Point nm = new RogueSharp.Point(nextMove.X, nextMove.Y);
                    Core.DirectMoveEventArgs moveReq
                        = new Core.DirectMoveEventArgs(Core.EventTypes.DirectMove, entMoving, nm);
                    Core.EventBus.Publish(Core.EventTypes.ActionReqMove, moveReq);
                }
                else
                {
                    // check for end of path, are we in cell next to target?
                    bool nextToTarget = NextToTarget(aiC, posC);

                    if (nextToTarget)
                    { 
                        Game.MessageLog.Add("I'm There Sonny");
                        //TODO Finalise movement
                        aiC.GotPath = false;
                        aiC.PathToTarget = null;
                        RogueSharp.Point nm = new RogueSharp.Point(aiC.Target.X, aiC.Target.Y);
                        Core.DirectMoveEventArgs moveReq
                            = new Core.DirectMoveEventArgs(Core.EventTypes.DirectMove, entMoving, nm);
                        Core.EventBus.Publish(Core.EventTypes.ActionReqMove, moveReq);
                    }
                    else
                    {
                        // stuck, create new target
                        Game.MessageLog.Add("STUCKY MUFFINS");
                        bool targetFound = SetTargetToExit(entMoving);
                        aiC.GotPath = targetFound;
                        Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, entMoving);
                        Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
                    }
                }
            }
            catch (RogueSharp.NoMoreStepsException eexc)
            {
                Game.MessageLog.Add("You're breaking my heart, no more steps...");
                //aiC.PathToTarget = null;
                //bool targetFound = SetTargetToExit(entMoving);
                //aiC.GotPath = targetFound;

                aiC.AiState = Core.AIStates.Sleeping;
                aiC.Target = new RogueSharp.Point(0,0);
                aiC.PathToTarget = null;

                Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, entMoving);
                Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
            }
        }

        private bool SetTargetToExit(Core.Entity ent)
        {
            bool targetFound = false;
            RogueSharp.PathFinder pF = new RogueSharp.PathFinder(_dungeonMap);

            // get the AI component
            List<Components.Component> entComps = _em.GetCompsByID(ent.UID);
            Components.AIComp aiComp
                = (Components.AIComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.AI);

            Components.PositionComp curPos
                = (Components.PositionComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);

            int exitBit = (int)Core.ComponentTypes.Stairs;
            List<int> stairsFound = new List<int>();
            foreach(KeyValuePair<int, int> entry in _em.EntityBitLookUp)
            {
                int v = entry.Value;
                if ((exitBit & v) > 0)
                {
                    stairsFound.Add(entry.Key);
                }
            }

            if (stairsFound.Count > 0)
            {
                int sid = stairsFound.First();
                List<Components.Component> compList = _em.GetCompsByID(sid);
                Components.PositionComp posComp 
                    = (Components.PositionComp)compList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);
                aiComp.Target = new RogueSharp.Point(posComp.X, posComp.Y);

                // get path to target
                RogueSharp.Cell endCell = (RogueSharp.Cell)_dungeonMap.GetCell(posComp.X, posComp.Y);
                RogueSharp.Cell startCell = (RogueSharp.Cell)_dungeonMap.GetCell(curPos.X, curPos.Y);
                try
                {
                    RogueSharp.Path pathToTarget = pF.ShortestPath(startCell, endCell);
                    aiComp.PathToTarget = pathToTarget;
                    targetFound = true;
                    return targetFound;
                }
                catch (RogueSharp.PathNotFoundException e)
                {
                    //TODO sort out this mess....

                    Game.MessageLog.Add("NO PATH Exception!");
                    return targetFound;
                }
            }
            else
            {
                return targetFound;
            }
        }

        private void CheckSurroundingsForGold(Core.Entity ent)
        {
            List<Components.Component> compList = _em.GetCompsByID(ent.UID);
            Components.PositionComp pc
                = (Components.PositionComp)compList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);

            Components.AttributesComp attC
                = (Components.AttributesComp)compList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Attributes);

            Components.AIComp aiC = (Components.AIComp)compList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.AI);

            _dungeonMap.ComputeFov(pc.X, pc.Y, attC.Awareness, false);
            IEnumerable<RogueSharp.ICell> mapCells = _dungeonMap.GetAllCells();

            Dictionary<int, RogueSharp.Point> targetList = new Dictionary<int, RogueSharp.Point>();

            foreach(RogueSharp.ICell eachCell in mapCells)
            {
                // are they in fov
                if (eachCell.IsInFov)
                {
                    // look for something interesting
                    int xp = eachCell.X;
                    int yp = eachCell.Y;
                    string key = xp.ToString() + "-" + yp.ToString();

                    if (_em.EntityPostionLookUp.ContainsKey(key))
                    {

                        List<Core.Entity> entsList = _em.EntityPostionLookUp[key];
                        //if (entsList.Count > 1)
                        //{
                        //    Game.MessageLog.Add($"Can see {entsList.Count.ToString()} at {key}");
                        //}
                        foreach (Core.Entity entAtPos in entsList)
                        {
                            
                            // look for gold
                            int innerID = entAtPos.UID;
                            if (_em.EntityBitLookUp.ContainsKey(innerID))
                            {
                                int entBit = _em.EntityBitLookUp[innerID];
                                
                                int cb = entBit & (int)Core.ComponentTypes.Value;

                                //Game.MessageLog.Add($"Contains {cb.ToString()}");

                                if ((entBit & (int)Core.ComponentTypes.Value) > 0)
                                {
                                    // sommething has value - ad to our target list
                                    targetList.Add(innerID, new RogueSharp.Point(xp, yp));
                                }
                            }
                        }
                    }
                }
            }

            //Game.MessageLog.Add($"TargetList = {targetList.Count.ToString()}");

            // do something with targetList
            if (targetList.Count > 0)
            {
                int distanceToTarget = 1000;
                RogueSharp.Point targetPoint = new RogueSharp.Point(0, 0);
                foreach (KeyValuePair<int, RogueSharp.Point> entry in targetList)
                {

                    List<RogueSharp.Point> path
                        = Core.Bresenhams.SuperCoverLine(new RogueSharp.Point(pc.X, pc.Y), entry.Value);
                    int pathSize = path.Count();

                    if (pathSize < distanceToTarget)
                    {

                        targetPoint = entry.Value;
                        distanceToTarget = pathSize;
                    }

                }

                RogueSharp.Cell endCell = (RogueSharp.Cell)_dungeonMap.GetCell(targetPoint.X, targetPoint.Y);
                RogueSharp.Cell startCell = (RogueSharp.Cell)_dungeonMap.GetCell(pc.X,pc.Y);
                RogueSharp.PathFinder pF = new RogueSharp.PathFinder(_dungeonMap);

                try
                {
                    RogueSharp.Path pathToTarget = pF.ShortestPath(startCell, endCell);
                    aiC.PathToTarget = pathToTarget;
                    aiC.Target = targetPoint;
                    aiC.GotPath = true;
                    Game.MessageLog.Add($"I'm Greedy! Gold is {distanceToTarget.ToString()} away");
                }
                catch (Exception e)
                {
                    Game.MessageLog.Add("GREEDY BUT NO PATH TO GOLD!");
                }

            }
        }
    }
}
