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

                    Core.Entity ent = msg.entRequestingMove;

                    List<Components.Component> curEntCompList = _em.GetCompsByID(msg.entRequestingMove.UID);
                    Components.AIComp aiComp
                        = (Components.AIComp)curEntCompList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.AI);
                    Components.PositionComp posComp
                        = (Components.PositionComp)curEntCompList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);

                    //Core.Directions directionToMove;
                    //directionToMove = Core.Directions.None;
                    //int turnsSinceMove = aiComp.TurnsSinceMove;

                    // are we under attack
                    bool underAttack = aiComp.UnderAttack;

                    // are we standing on anything?
                    bool standing = AreWeStandingOnSomething(posComp);
                    bool objPickedUp = false;

                    // do we have a path
                    bool gotPath = aiComp.GotPath;

                    // are we asleep
                    //bool sleeping = (aiComp.AiState == Core.AIStates.Sleeping);

                    if (aiComp.Fleeing)
                    {
                        // run away!  Evaluate then either flee or stop fleeing
                        Game.MessageLog.Add("WE are fleeing!");
                    }
                    else
                    {
                        if (underAttack)
                        {
                            // under attack, fight or flee
                            Game.MessageLog.Add($"We are under attack captain from {aiComp.LastBasher.ToString()}");
                            bool enemyFound = false;
                            int xp = 0;
                            int yp = 0;

                            IEnumerable<RogueSharp.ICell> surroundingCells = _dungeonMap.GetBorderCellsInSquare(posComp.X, posComp.Y, 1);
                            foreach(RogueSharp.ICell cell in surroundingCells)
                            {
                                string lookUp = cell.X.ToString() + "-" + cell.Y.ToString();
                                if (_em.EntityPostionLookUp.ContainsKey(lookUp))
                                {
                                    List<Core.Entity> luList = _em.EntityPostionLookUp[lookUp];
                                    foreach(Core.Entity entInLu in luList)
                                    {
                                        if (entInLu.UID == aiComp.LastBasher)
                                        {
                                            enemyFound = true;
                                            xp = cell.X;
                                            yp = cell.Y;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (enemyFound)
                            {
                                Game.MessageLog.Add("Going for the kill!");
                                RogueSharp.Point nm = new RogueSharp.Point(xp, yp);
                                Core.DirectMoveEventArgs moveReq
                                    = new Core.DirectMoveEventArgs(Core.EventTypes.DirectMove, ent, nm);
                                Core.EventBus.Publish(Core.EventTypes.ActionReqMove, moveReq);
                            }

                        }
                        else
                        {
                            if (standing)
                            {
                                // standing on something, pick it up?
                                Game.MessageLog.Add("we are standing on something or other");
                                // have we seen it before

                                
                                List<int> objsOfInterest = new List<int>();
                                string posKey = posComp.X.ToString() + "-" + posComp.Y.ToString();

                                if (_em.EntityPostionLookUp.ContainsKey(posKey))
                                {
                                    foreach (Core.Entity entOnPos in _em.EntityPostionLookUp[posKey])
                                    {
                                        if (_em.CheckEntForBits(entOnPos.UID, (int)Core.ComponentTypes.Collectable))
                                        {
                                            if (aiComp.ItemsAlreadySeen.Contains(entOnPos.UID))
                                            {
                                                Game.MessageLog.Add("SEEN THIS ITEM BEFORE");
                                               // objsOfInterest.Add(entOnPos.UID);
                                            }
                                            else
                                            {
                                                // we've not seen this before.  Pick up?
                                                if (entOnPos.UID != ent.UID)
                                                {

                                                    aiComp.ItemsAlreadySeen.Add(entOnPos.UID);
                                                    objsOfInterest.Add(entOnPos.UID);
                                                }

                                            }
                                        }
                                    }
                                }

                                if (objsOfInterest.Count > 0)
                                {
                                    // we have interesting objects, do something
                                    
                                    objPickedUp = true;

                                    //int objToPick = rdm.Next(objsOfInterest.Count);
                                    int ei = objsOfInterest[0];
                                    // Game.MessageLog.Add($"trying to pick up count = {objsOfInterest.Count.ToString()}");
                                    Core.Entity pickedUpEnt = _em.JustEntities[ei];
                                    Game.MessageLog.Add($"i {ent.UID.ToString()} want to pick something up: {pickedUpEnt.UID.ToString()}");
                                    bool treaure = _em.CheckEntForBits(pickedUpEnt.UID, (int)Core.ComponentTypes.ItemValue);
                                    Game.MessageLog.Add($"treasure has valuecomp={treaure.ToString()}");
                                    Core.InventoryAddEventArgs addEvent
                                        = new Core.InventoryAddEventArgs(Core.EventTypes.InventoryAdd, ent.UID, pickedUpEnt.UID);
                                    Core.EventBus.Publish(Core.EventTypes.InventoryAdd, addEvent);
                                }

                            }

                            if (gotPath & !objPickedUp)
                            {
                                // are we at the target?
                                if (AtTarget(aiComp, posComp))
                                {
                                    //Game.MessageLog.Add("On Target!!!!!");
                                    aiComp.Target = new RogueSharp.Point(0,0);
                                    aiComp.PathToTarget = null;
                                    aiComp.GotPath = false;

                                    _dungeonMap.SetCellProperties(posComp.X, posComp.Y, false, true);
                                   
                                    Core.DeleteEntEventArgs deleteEv = new Core.DeleteEntEventArgs(Core.EventTypes.DeleteEntity, msg.entRequestingMove.UID);
                                    Core.EventBus.Publish(Core.EventTypes.DeleteEntity, deleteEv);
                                }
                                else
                                {
                                    // follow path
                                    if (!NextToTarget(aiComp, posComp))
                                    //if (!AtTarget(aiComp, posComp))
                                    {
                                        FollowPath(aiComp, posComp, msg.entRequestingMove);
                                    }
                                    else
                                    {
                                        // we're next to the target - move onto it
                                        //Game.MessageLog.Add("NExt To Target!!!!!");

                                        RogueSharp.Point nm = new RogueSharp.Point(aiComp.Target.X, aiComp.Target.Y);
                                        Core.DirectMoveEventArgs moveReq
                                            = new Core.DirectMoveEventArgs(Core.EventTypes.DirectMove, msg.entRequestingMove, nm);
                                        Core.EventBus.Publish(Core.EventTypes.ActionReqMove, moveReq);
                                    }
                                }

                                    
                            }
                            else if (!objPickedUp)
                            {
                                // no nothing, choose what to do...
                                //Game.MessageLog.Add("doing not much");
                                //Game.MessageLog.Add($"I am at target: {AtTarget(aiComp, posComp).ToString()}");

                                bool exitFound = SetTargetToExit(msg.entRequestingMove);
                                if (exitFound)
                                {
                                    aiComp.GotPath = exitFound;
                                    Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, msg.entRequestingMove);
                                    Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
                                }

                            }
                        }
                    }

                    aiComp.UnderAttack = false;
                    break;
            }
        }

        private bool AreWeStandingOnSomething(Components.PositionComp posComp)
        {
            bool objectFound = false;
            string posKey = posComp.X.ToString() + "-" + posComp.Y.ToString();
            if (_em.EntityPostionLookUp.ContainsKey(posKey))
            {
                foreach (Core.Entity entOnPos in _em.EntityPostionLookUp[posKey])
                {
                    if (_em.CheckEntForBits(entOnPos.UID, (int)Core.ComponentTypes.Collectable))
                    {
                        //Game.MessageLog.Add("standing on something collectable");
                        return true;
                    }

                }
            }
            return objectFound;
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

        private void ResetAI(Components.AIComp aiCompToReset)
        {
            aiCompToReset.GotPath = false;
            aiCompToReset.AtTarget = false;
            aiCompToReset.PathToTarget = null;
            aiCompToReset.Target = new RogueSharp.Point();
        }

        private bool SetTargetToExit(Core.Entity ent)
        {
            bool targetFound = false;
           
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
                _dungeonMap.SetIsWalkable(curPos.X, curPos.Y, true);
                //_dungeonMap.SetIsWalkable(posComp.X, posComp.Y, true);
                RogueSharp.PathFinder pF = new RogueSharp.PathFinder(_dungeonMap, 1.41);

                RogueSharp.Cell endCell = (RogueSharp.Cell)_dungeonMap.GetCell(posComp.X, posComp.Y);
                RogueSharp.Cell startCell = (RogueSharp.Cell)_dungeonMap.GetCell(curPos.X, curPos.Y);
                try
                {
                    RogueSharp.Path pathToTarget = pF.ShortestPath(startCell, endCell);
                    aiComp.PathToTarget = pathToTarget;
                    _dungeonMap.SetIsWalkable(curPos.X, curPos.Y, false);
                    //_dungeonMap.SetIsWalkable(posComp.X, posComp.Y, false);
                    targetFound = true;
                    return targetFound;
                }
                catch (RogueSharp.PathNotFoundException e)
                {
                    //TODO sort out this mess....
                    ResetAI(aiComp);
                    _dungeonMap.SetIsWalkable(curPos.X, curPos.Y, false);
                    _dungeonMap.SetIsWalkable(posComp.X, posComp.Y, false);
                    Game.MessageLog.Add("NO PATH Exception!");
                    return targetFound;
                }
            }
            else
            {
                return targetFound;
            }
        }

        private List<RogueSharp.Point> CheckSurroundingsForTreasure(Core.Entity ent)
        {
            List<RogueSharp.Point> retList = new List<RogueSharp.Point>();

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

                        foreach (Core.Entity entAtPos in entsList)
                        {
                            // look for gold
                            int innerID = entAtPos.UID;
                            if (_em.EntityBitLookUp.ContainsKey(innerID))
                            {
                                int entBit = _em.EntityBitLookUp[innerID];

                                if ((entBit & (int)Core.ComponentTypes.Collectable) > 0)
                                {
                                    // entity is collectable
                                    List<Components.Component> entCompList = _em.GetCompsByID(innerID);
                                    Components.CollectableComp collComp
                                        = (Components.CollectableComp)entCompList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Collectable);
                                    if (collComp.Treasure)
                                    {
                                        // sommething has value - ad to our target list
                                        targetList.Add(innerID, new RogueSharp.Point(xp, yp));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        
            return retList;
        }
    }
}
