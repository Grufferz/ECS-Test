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
        //private RogueSharp.FieldOfView fover;
        private List<RogueSharp.Point> objsOfInterest;

        public AISystem(EntityManager em, Core.DungeonMap dm, GarbageSystem gs)
        {
            _em = em;
            _dungeonMap = dm;
            _garbageSystem = gs;
            rdm = new Random();
            RogueSharp.FieldOfView fover = new RogueSharp.FieldOfView(_dungeonMap);
            objsOfInterest = new List<RogueSharp.Point>();

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
                    Components.AttributesComp attComp = (Components.AttributesComp)curEntCompList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Attributes);

                    //Core.Directions directionToMove;
                    //directionToMove = Core.Directions.None;
                    //int turnsSinceMove = aiComp.TurnsSinceMove;

                    //aiComp.DMap.ComputeFov(posComp.X, posComp.Y, attComp.Awareness, false);


                    //IEnumerable<RogueSharp.ICell> fo = GetFOV(posComp.X, posComp.Y, attComp.Awareness, _dungeonMap);

                    IEnumerable<RogueSharp.ICell> fo;

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
                                //string lookUp = cell.X.ToString() + "-" + cell.Y.ToString();
                                //if (_em.EntityPostionLookUp.ContainsKey(lookUp))
                                HashSet<int> luPos = _em.Positions[cell.Y, cell.X];
                                foreach( int i in luPos)
                                {
                                    if (i == aiComp.LastBasher)
                                    {
                                        enemyFound = true;
                                        xp = cell.X;
                                        yp = cell.Y;
                                        break;
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

                                int objCount = 0;
                                List<int> pickUps = new List<int>();

                                HashSet<int> hsOfids = _em.Positions[posComp.Y, posComp.X];
                                foreach( int id in hsOfids)
                                {
                                    Components.CollectableComp cc 
                                        = (Components.CollectableComp) _em.GetSingleComponentByID(id, Core.ComponentTypes.Collectable);

                                    if (cc != null)
                                    {
                                        //if (!aiComp.ItemsAlreadySeen.Contains(id))
                                        //{
                                        //Game.MessageLog.Add("ADDING ITEMS ))))))))))))))))))))))))))))))))))))))))))))))))))))))");
                                        objCount++;
                                        pickUps.Add(id);
                                            //aiComp.ItemsAlreadySeen.Add(id);
                                        //objsOfInterest.Add(id);
                                        //}
                                    }

                                }


                                //string posKey = posComp.X.ToString() + "-" + posComp.Y.ToString();

                                //if (_em.EntityPostionLookUp.ContainsKey(posKey))
                                //{
                                //    foreach (Core.Entity entOnPos in _em.EntityPostionLookUp[posKey])
                                //    {
                                //        if (_em.CheckEntForBits(entOnPos.UID, (int)Core.ComponentTypes.Collectable))
                                //        {
                                //            if (aiComp.ItemsAlreadySeen.Contains(entOnPos.UID))
                                //            {
                                //                Game.MessageLog.Add("SEEN THIS ITEM BEFORE");
                                //               // objsOfInterest.Add(entOnPos.UID);
                                //            }
                                //            else
                                //            {
                                //                // we've not seen this before.  Pick up?
                                //                if (entOnPos.UID != ent.UID)
                                //                {
                                //                    Game.MessageLog.Add("ADDING ITEMS");
                                //                    aiComp.ItemsAlreadySeen.Add(entOnPos.UID);
                                //                    objsOfInterest.Add(entOnPos.UID);
                                //                }

                                //            }
                                //        }
                                //    }
                                //}

                                //if (objsOfInterest.Count > 0)
                                if (objCount > 0)
                                {
                                    // we have interesting objects, do something
                                    
                                    objPickedUp = true;

                                    //int objToPick = rdm.Next(objsOfInterest.Count);
                                    int ei = pickUps[rdm.Next(pickUps.Count)];
                                    // Game.MessageLog.Add($"trying to pick up count = {objsOfInterest.Count.ToString()}");
                                    //Core.Entity pickedUpEnt = _em.JustEntities[ei];
                                    Game.MessageLog.Add($"i {ent.UID.ToString()} want to pick something up: {ei.ToString()}");
                                    //bool treaure = _em.CheckEntForBits(ei, (int)Core.ComponentTypes.ItemValue);
                                   
                                    Core.InventoryAddEventArgs addEvent
                                        = new Core.InventoryAddEventArgs(Core.EventTypes.InventoryAdd, ent.UID, ei);
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
                                   
                                   // Core.DeleteEntEventArgs deleteEv = new Core.DeleteEntEventArgs(Core.EventTypes.DeleteEntity, msg.entRequestingMove.UID);
                                   // Core.EventBus.Publish(Core.EventTypes.DeleteEntity, deleteEv);
                                }
                                else
                                {
                                    // follow path
                                    if (!NextToTarget(aiComp, posComp))
                                    //if (!AtTarget(aiComp, posComp))
                                    {

                                        FollowPath(aiComp, posComp, msg.entRequestingMove);
                                        //bool target = SetPathToTarget(ent, aiComp.Target);
                                        //aiComp.GotPath = target;
                                        //if (target)
                                        //{
                                        //    FollowPath(aiComp, posComp, msg.entRequestingMove);
                                        //}
                                        
                                    }
                                    else
                                    {
                                        // we're next to the target - move onto it
                                        Game.MessageLog.Add("Next To Target!!!!!");

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
                                Game.MessageLog.Add("doing not much");

                                fo = GetFOV(posComp.X, posComp.Y, attComp.Awareness, _dungeonMap);
                                List<RogueSharp.Point> treasureList = CanISeeTreasure(ent, fo);
                                //Game.MessageLog.Add($"I can see {treasureList.Count.ToString()} items of treasure");

                                if (objsOfInterest.Count > 0)
                                //if (treasureList.Count > 0)
                                {
                                    Game.MessageLog.Add("collecting treasure");

                                    //RogueSharp.Point tp = treasureList[rdm.Next(treasureList.Count)];
                                    RogueSharp.Point tp = objsOfInterest[rdm.Next(objsOfInterest.Count)];

                                    // check object still exists....

                                    bool doesCollectableExist = _em.CheckPosForCollectableEnt(tp.X, tp.Y);
                                    //Game.MessageLog.Add($"DOES COLLL EXIST?  {doesCollectableExist.ToString()}");

                                    bool target = SetPathToTarget(ent, tp);
                                    aiComp.GotPath = target;
                                    //objsOfInterest.Remove(tp);
                                    

                                    //Game.MessageLog.Add($"Found way to TREASURE={target.ToString()}");
                                    Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, msg.entRequestingMove);
                                    Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
                                }
                                //else
                                //{
                                //    Game.MessageLog.Add("I'm stuck, going to random room");
                                //    //int r = rdm.Next(_dungeonMap.Rooms.Count);
                                //    //int roomX = _dungeonMap.Rooms[r].Center.X;
                                //    //int roomY = _dungeonMap.Rooms[r].Center.Y;
                                //    //bool target = SetPathToTarget(ent, new RogueSharp.Point(roomX, roomY));
                                //    ////aiComp.GotPath = target;
                                //    //Game.MessageLog.Add($"TARGE={target.ToString()}");

                                //}
                                else
                                {
                                    bool exitFound = false;
                                    //bool exitFound = SetTargetToExit(msg.entRequestingMove);
                                    if (exitFound)
                                    {
                                        Game.MessageLog.Add("leaving");
                                        aiComp.GotPath = exitFound;
                                        //Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, msg.entRequestingMove);
                                        //Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
                                    }
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
            HashSet<int> innerIDS = _em.Positions[posComp.Y, posComp.X];
            foreach (int iid in innerIDS)
            {
                Components.CollectableComp cc = 
                    (Components.CollectableComp)_em.GetSingleComponentByID(iid, Core.ComponentTypes.Collectable);
                if (cc != null)
                {
                    //Game.MessageLog.Add("standing on something collectable");
                    return true;
                }

            }
            return false;
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
                RogueSharp.Cell nextMove = (RogueSharp.Cell)aiC.PathToTarget.TryStepForward();

                if (nextMove != null)
                {

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
                            ResetAI(aiC);
                            //bool targetFound = SetTargetToExit(entMoving);
                            //aiC.GotPath = targetFound;
                            //Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, entMoving);
                            //Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
                        }
                    }
                }
                else
                {
                    Game.MessageLog.Add("HAYLP");
                    bool nextToTarget = NextToTarget(aiC, posC);
                    //Game.MessageLog.Add($"{posC.X.ToString()} and {posC.Y.ToString()} looking for {aiC.Target.X.ToString()} and {aiC.Target.Y.ToString()}");

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
                        Game.MessageLog.Add("STUCKY MUFFINS 22");
                        //bool targetFound = SetTargetToExit(entMoving);
                        //aiC.GotPath = targetFound;
                        //Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, entMoving);
                        //Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
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

                //Core.NoMoveEventArgs nmEv = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, entMoving);
                //Core.EventBus.Publish(Core.EventTypes.NoMove, nmEv);
            }
        }

        private void ResetAI(Components.AIComp aiCompToReset)
        {
            aiCompToReset.GotPath = false;
            aiCompToReset.AtTarget = false;
            aiCompToReset.PathToTarget = null;
            aiCompToReset.Target = new RogueSharp.Point();
        }

        private bool SetPathToTarget(Core.Entity ent, RogueSharp.Point target)
        {
            List<Components.Component> entComps = _em.GetCompsByID(ent.UID);
            Components.PositionComp curPos 
                = (Components.PositionComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);

            Components.AIComp aiComp = (Components.AIComp)entComps.FirstOrDefault(x => x.CompType == Core.ComponentTypes.AI);

            // get path to target
            _dungeonMap.SetIsWalkable(curPos.X, curPos.Y, true);
            _dungeonMap.SetIsWalkable(target.X, target.Y, true);
            RogueSharp.PathFinder pF = new RogueSharp.PathFinder(_dungeonMap, 1.41);

            RogueSharp.Cell endCell = (RogueSharp.Cell)_dungeonMap.GetCell(target.X, target.Y);
            RogueSharp.Cell startCell = (RogueSharp.Cell)_dungeonMap.GetCell(curPos.X, curPos.Y);

            try
            {
                RogueSharp.Path pathToTarget = pF.ShortestPath(startCell, endCell);
                aiComp.Target = new RogueSharp.Point(endCell.X, endCell.Y);
                aiComp.PathToTarget = pathToTarget;
                _dungeonMap.SetIsWalkable(curPos.X, curPos.Y, false);
                //_dungeonMap.SetIsWalkable(target.X, target.Y, false);
                return true;
            }
            catch (RogueSharp.PathNotFoundException e)
            {
                //TODO sort out this mess....
                ResetAI(aiComp);
                _dungeonMap.SetIsWalkable(curPos.X, curPos.Y, false);
                Game.MessageLog.Add("NO PATH Exception!");
                return false;
            }
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

        private List<RogueSharp.Point> CanISeeTreasure(Core.Entity ent, IEnumerable<RogueSharp.ICell> fov)
        {
            List<RogueSharp.Point> treasureList = new List<RogueSharp.Point>();

            List<Components.Component> compList = _em.GetCompsByID(ent.UID);

            Components.PositionComp pc
                = (Components.PositionComp)compList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Position);

            //Components.AIComp aiC = (Components.AIComp)compList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.AI);

            //Components.AttributesComp attC
            //    = (Components.AttributesComp)compList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.Attributes);

            //GetFOV(pc.X, pc.Y, attC.Awareness, false);
            //IEnumerable<RogueSharp.ICell> fo = GetFOV(pc.X, pc.Y, attC.Awareness, _dungeonMap);
            //Game.MessageLog.Add($"I CAN SEE {fo.Count().ToString()} CELLS");

            //IEnumerable<RogueSharp.ICell> mapCells = aiC.DMap.GetAllCells();

            int count = 0;

            foreach (RogueSharp.ICell eachCell in fov)
            {
                count++;
                // look for something interesting
                int xp = eachCell.X;
                int yp = eachCell.Y;

                if (xp != pc.X && yp != pc.Y)
                {

                    HashSet<int> innerEnt = _em.Positions[yp, xp];
                    foreach (int inE in innerEnt)
                    {
                        bool found = false;
                        List<Components.Component> innerList = _em.Entities[inE];
                        foreach (Components.Component c in innerList)
                        {
                            //Game.MessageLog.Add(c.ToString());
                            if (c.CompType == Core.ComponentTypes.Collectable)
                            {
                                found = true;
                                break;
                            }
                        }

                        int checker = _em.EntityBitLookUp[inE];
                        Components.CollectableComp cc = (Components.CollectableComp)_em.GetSingleComponentByID(inE, Core.ComponentTypes.Collectable);
                        bool collll = cc != null;
                        //Game.MessageLog.Add($"{inE.ToString()} ---  I AM COLLECTABLE {found.ToString()} YEDS!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        if (found)
                        //if ((checker & (int)Core.ComponentTypes.Collectable) > 0)
                        {
                            RogueSharp.Point p = new RogueSharp.Point(xp, yp);
                            if (!objsOfInterest.Contains(p))
                            {
                                objsOfInterest.Add(p);
                            }

                            
                            treasureList.Add(new RogueSharp.Point(xp, yp));
                        }
                    }
                }
            }
            Game.MessageLog.Add($"I can SEE {fov.Count().ToString()} cells from ere of which {treasureList.Count.ToString()} contain items");
            Game.MessageLog.Add($"ObjsOfIOnterest contains {objsOfInterest.Count.ToString()}");
            return treasureList;
        }

        

        public IEnumerable<RogueSharp.ICell> GetFOV(int x, int y, int d, Core.DungeonMap dmap)
        {
            //List<RogueSharp.ICell> circleFOV = new List<RogueSharp.ICell>();
            var fieldofview = new RogueSharp.FieldOfView(dmap);
            var cellsInFov = fieldofview.ComputeFov(x, y, d, false);
            //List<RogueSharp.ICell> circle = dmap.GetCellsInCircle(x, y, d).ToList();
            return cellsInFov;
            //Game.MessageLog.Add($"WHEREAS I CONTAIN {cellsInFov.Count().ToString()} CELLS");
            //foreach (RogueSharp.ICell cell in cellsInFov)
            //{
            //    //if (circle.Contains(cell))
            //    //{
            //    circleFOV.Add(cell);
            //       // _dungeonMap.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
            //    //}
            //}
            ////Game.MessageLog.Add($"I CONTAIN {circleFOV.Count().ToString()} CELLS");
            //return circleFOV;
        }
    }
}
