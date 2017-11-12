using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft;

namespace ECS_Test.Systems
{
    class EntityManager
    {
        private int _entityID;
        public Dictionary<int, List<Components.Component>> Entities { get; private set; }
        public Dictionary<int, int> EntityBitLookUp { get; private set; }
        public Dictionary<string, List<Core.Entity>> EntityPostionLookUp { get; private set; }
        public Dictionary<int, Core.Entity> JustEntities { get; private set; }
        public List<string> CreatNames;
        public HashSet<int>[,] Positions;
        public Random r;
        private int _width;
        private int _height;

        public EntityManager(int w, int h)
        {
            _entityID = 1;
            Entities = new Dictionary<int, List<Components.Component>>();
            EntityBitLookUp = new Dictionary<int, int>();
            //EntityPostionLookUp = new Dictionary<string, List<Core.Entity>>();
            JustEntities = new Dictionary<int, Core.Entity>();

            _height = h;
            _width = w;
            Positions = new HashSet<int>[_height, _width];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Positions[y, x] = new HashSet<int>();
                }
            }

            r = new Random();
            CreatNames = ReadCreatureNames(100);
        }

        public void AddMonster(int x, int y, Core.DungeonMap m)
        {
            Core.Entity e = new Core.Entity(_entityID);
            Core.EntityReturner er;

            //int ind = r.Next(CreatNames.Count);
            int ind = Game.Random.Next(CreatNames.Count - 1);
            string creatureName = CreatNames[ind];

            int entType = RogueSharp.DiceNotation.Dice.Roll("1d20");
            if ( entType <= 2 )
            {
                er = Core.EntityFactory.CreateTroll(x, y, creatureName, m);
            }
            else if (entType > 2 && entType <6 )
            {
                er = Core.EntityFactory.CreateOrc(x, y, creatureName, m);
            }
            else if (entType >= 6 && entType < 14)
            {
                er = Core.EntityFactory.CreateKobold(x, y, creatureName, m);
            }
            else
            {
                er = Core.EntityFactory.CreateRat(x, y, creatureName, m);
            }

            //add entity to entity dict
            Entities.Add(_entityID, er.ComponentList);
            EntityBitLookUp.Add(_entityID, er.LookUpBit);
            JustEntities.Add(_entityID, e);

            //add to PositionLookUp
            AddEntToPosition(x, y, e.UID);

            //try adding to schedule
            Components.Component ts = GetSingleComponentByID(_entityID, Core.ComponentTypes.Schedulable);
            if (ts != null)
            {
                Components.SchedulableComp sc = (Components.SchedulableComp)ts;
                int entTime = sc.Time;
                Game.ShedSystem.Add(e, entTime);
            }

            // inc entityID
            _entityID++;

            //add weapon to entity
            AddWeaponToEntity(e.UID);
            if (RogueSharp.DiceNotation.Dice.Roll("1d10") > 8)
            {
                AddPotionToEntity(e.UID);
            }

        }

        public void AddPotionToEntity(int eid)
        {
            Core.Entity e = new Core.Entity(_entityID);
            Core.EntityReturner er;

            int potionID = e.UID;

            er = Core.EntityFactory.CreateHealthPotion();

            Entities.Add(_entityID, er.ComponentList);
            EntityBitLookUp.Add(_entityID, er.LookUpBit);
            JustEntities.Add(_entityID, e);

            _entityID++;

            // add potion to entity
            Core.InventoryAddEventArgs addEvent = new Core.InventoryAddEventArgs(Core.EventTypes.InventoryAdd, eid, potionID);
            Core.EventBus.Publish(Core.EventTypes.InventoryAdd, addEvent);
        }

        public void AddWeaponToEntity(int eid)
        {
            Core.Entity e = new Core.Entity(_entityID);
            Core.EntityReturner er;

            int weaponID = e.UID;

            Components.CreatureDetailsComp cdc = (Components.CreatureDetailsComp)GetSingleComponentByID(eid, Core.ComponentTypes.CreatureDetails);

            if (cdc.CreatureType != Types.CreatureTypes.Troll)
            {
                er = Core.EntityFactory.CreateSword();
            }
            else
            {
                er = Core.EntityFactory.CreateTrollClub();
            }
            

            Entities.Add(_entityID, er.ComponentList);
            EntityBitLookUp.Add(_entityID, er.LookUpBit);
            JustEntities.Add(_entityID, e);

            _entityID++;

            // add weapon to entity
            Core.InventoryAddEventArgs addEvent = new Core.InventoryAddEventArgs(Core.EventTypes.InventoryAdd, eid, weaponID);
            Core.EventBus.Publish(Core.EventTypes.InventoryAdd, addEvent);
        }

        public void AddDoor(int x, int y, bool isOpen)
        {
            var e = new Core.Entity(_entityID);

            Core.EntityReturner er = Core.EntityFactory.CreateDoor(x, y, isOpen);

            //add entity to entity dict
            Entities.Add(_entityID, er.ComponentList);
            EntityBitLookUp.Add(_entityID, er.LookUpBit);
            JustEntities.Add(_entityID, e);

            //add to PositionLookUp
            AddEntToPosition(x, y, e.UID);

            // inc entityID
            _entityID++;
        }

        public void AddStairs(int x, int y, bool isUp)
        {
            var e = new Core.Entity(_entityID);
            // List<Components.Component> compList = new List<Components.Component>();

            Core.EntityReturner er = Core.EntityFactory.CreateStairs(x, y, isUp);

            //add entity to entity dict
            Entities.Add(_entityID, er.ComponentList);
            EntityBitLookUp.Add(_entityID, er.LookUpBit);
            JustEntities.Add(_entityID, e);

            //add to PositionLookUp
            AddEntToPosition(x, y, e.UID);

            // inc entityID
            _entityID++;
        }

        public void AddTreasure(int x, int y)
        {
            var e = new Core.Entity(_entityID);
            // List<Components.Component> compList = new List<Components.Component>();

            Core.EntityReturner er = Core.EntityFactory.CreateGold(x, y, r.Next());

            //add entity to entity dic
            Entities.Add(_entityID, er.ComponentList);
            EntityBitLookUp.Add(_entityID, er.LookUpBit);
            JustEntities.Add(_entityID, e);

            AddEntToPosition(x, y, e.UID);

            // inc entityID
            _entityID++;
        }

        public bool CheckEntForBits(int eID, int checkBit)
        {
            if (EntityBitLookUp.TryGetValue(eID, out int cb))
            {
                if ((checkBit & cb) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void ClearEntities()
        {
            Entities.Clear();
            EntityBitLookUp.Clear();
            JustEntities.Clear();
        }

        public Dictionary<int, List<Components.Component>> GetEntities()
        {
            return Entities;
        }

        public List<int> GetEntsByBitwise(Core.ComponentTypes compToLookFor)
        {
            //Dictionary<int, List<Components.Component>>  RetDict = new Dictionary<int, List<Components.Component>>();
            List<int> RetList = new List<int>();
            int toLookFor = (int)compToLookFor;
            foreach (KeyValuePair<int, int> entry in EntityBitLookUp)
            {
                int v = entry.Value;
                if ((toLookFor & v) > 0)
                {
                    RetList.Add(entry.Key);
                }
            }
            return RetList;
        }

        public List<int> GetEntIDByBit(int toLookFor)
        {
            List<int> retList = new List<int>();
            foreach (KeyValuePair<int, int> entry in EntityBitLookUp)
            {
                int v = entry.Value;
                if ((toLookFor & v) > 0)
                {
                    retList.Add(entry.Key);
                }
            }
            return retList;
        }

        public List<Components.Component> GetCompsByID(int eid)
        {
            List<Components.Component> retList;
            if (Entities.TryGetValue(eid, out retList))
            {
                return retList;
            }
            else
            {
                return null;
            }

        }

        public Dictionary<int, List<Components.Component>> GetDetailsByIDList(List<int> entIDs)
        {
            Dictionary<int, List<Components.Component>> returnDict = new Dictionary<int, List<Components.Component>>();

            foreach (int eid in entIDs)
            {
                if (Entities.ContainsKey(eid))
                {
                    List<Components.Component> compList = Entities[eid];
                    returnDict.Add(eid, compList);
                }
            }
            return returnDict;
        }

        public Components.Component GetSingleComponentByID(int eid, Core.ComponentTypes compType)
        {
            Components.Component returnComp = null;

            List<Components.Component> compList;
            if (Entities.TryGetValue(eid, out compList))
            {
                if (compList != null)
                {
                    returnComp = compList.FirstOrDefault(s => s.CompType == compType);
                }
            }
            return returnComp;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public void PrintLastID()
        {
            string lid = _entityID.ToString();
            //Game.MessageLog.Add($"Last ID = {lid}");
        }

        private List<string> ReadCreatureNames(int amount)
        {
            List<string> retList = new List<string>();
            List<string> names = new List<string>();

            string[] lines = System.IO.File.ReadAllLines(@"names.txt");

            foreach(string line in lines)
            {
                names.Add(line);
            }

            for (int i=0; i<amount; i++)
            {
                int index = r.Next(names.Count);
                retList.Add(names[index]);
                names.RemoveAt(index);
            }

            return retList;
        }

        public void RemoveEntity(int eid)
        {
            if (Entities.ContainsKey(eid))
            {
                List<Components.Component> compList = Entities[eid];
                if (compList.Exists(x => x.CompType == Core.ComponentTypes.Position))
                {
                    Components.PositionComp posComp 
                        = (Components.PositionComp)GetSingleComponentByID(eid, Core.ComponentTypes.Position);
                    RemoveEntFromPosition(posComp.X, posComp.Y, eid);
                    //string lu = posComp.X.ToString() + "-" + posComp.Y.ToString();
                    //if (EntityPostionLookUp.ContainsKey(lu))
                    //{
                    //    List<Core.Entity> entList = EntityPostionLookUp[lu];
                    //    entList.RemoveAll(x => x.UID == eid);
                    //    if (entList.Count == 0)
                    //    {
                    //        EntityPostionLookUp.Remove(lu);
                    //    }
                    //}
                    
                }
                JustEntities.Remove(eid);
                Entities.Remove(eid);
                EntityBitLookUp.Remove(eid);
            }
        }

        public void AddEntToPosition(int xp, int yp, int eid)
        {
            Positions[yp, xp].Add(eid);

            //string dictKey = xp.ToString() + "-" + yp.ToString();
            //if (!EntityPostionLookUp.ContainsKey(dictKey))
            //{
            //    EntityPostionLookUp.Add(dictKey, new List<Core.Entity>());
            //}
            //EntityPostionLookUp[dictKey].Add(ent);
        }

        public void AddPositionToEnt(int eid, int x, int y)
        {
            Components.PositionComp pC = new Components.PositionComp(x, y);

            List<Components.Component> entComps = Entities[eid];
            entComps.Add(pC);

            int checker = EntityBitLookUp[eid];
            checker = checker | (int)Core.ComponentTypes.Position;
            EntityBitLookUp[eid] = checker;
            

            AddEntToPosition(x, y, eid);
        }

        public void AddFurnitureToEnt(int eid)
        {
            Components.FurnitureComp fc = new Components.FurnitureComp();

            List<Components.Component> entComps = Entities[eid];
            entComps.Add(fc);

            int checker = EntityBitLookUp[eid];
            checker = checker | (int)Core.ComponentTypes.Furniture;
            EntityBitLookUp[eid] = checker;

        }

        public void RemoveCompFromEnt(int eid, Core.ComponentTypes cType)
        {

            List<Components.Component> entComps = Entities[eid];
            entComps.RemoveAll(x => x.CompType == cType);

            int checker = 0;
            EntityBitLookUp[eid] = checker;
            foreach (Components.Component c in entComps)
            {
                if (c.CompType == Core.ComponentTypes.Actor)
                {
                    checker = checker | (int)Core.ComponentTypes.Actor;
                }
                if (c.CompType == Core.ComponentTypes.AI)
                {
                    checker = checker | (int)Core.ComponentTypes.AI;
                }
                if (c.CompType == Core.ComponentTypes.Collectable)
                {
                    checker = checker | (int)Core.ComponentTypes.Collectable;
                }
                if (c.CompType == Core.ComponentTypes.CreatureDetails)
                {
                    checker = checker | (int)Core.ComponentTypes.CreatureDetails;
                }
                if (c.CompType == Core.ComponentTypes.Door)
                {
                    checker = checker | (int)Core.ComponentTypes.Door;
                }
                if (c.CompType == Core.ComponentTypes.Furniture)
                {
                    checker = checker | (int)Core.ComponentTypes.Furniture;
                }
                if (c.CompType == Core.ComponentTypes.Health)
                {
                    checker = checker | (int)Core.ComponentTypes.Health;
                }
                if (c.CompType == Core.ComponentTypes.Inventory)
                {
                    checker = checker | (int)Core.ComponentTypes.Inventory;
                }
                if (c.CompType == Core.ComponentTypes.ItemValue)
                {
                    checker = checker | (int)Core.ComponentTypes.ItemValue;
                }
                if (c.CompType == Core.ComponentTypes.Magic)
                {
                    checker = checker | (int)Core.ComponentTypes.Magic;
                }
                if (c.CompType == Core.ComponentTypes.MonsterStats)
                {
                    checker = checker | (int)Core.ComponentTypes.MonsterStats;
                }
                if (c.CompType == Core.ComponentTypes.Position)
                {
                    checker = checker | (int)Core.ComponentTypes.Position;
                }
                if (c.CompType == Core.ComponentTypes.Render)
                {
                    checker = checker | (int)Core.ComponentTypes.Render;
                }
                if (c.CompType == Core.ComponentTypes.Schedulable)
                {
                    checker = checker | (int)Core.ComponentTypes.Schedulable;
                }
                if (c.CompType == Core.ComponentTypes.Stairs)
                {
                    checker = checker | (int)Core.ComponentTypes.Stairs;
                }
                if (c.CompType == Core.ComponentTypes.Useable)
                {
                    checker = checker | (int)Core.ComponentTypes.Useable;
                }

                EntityBitLookUp[eid] = checker;
            }
        }

        public void RemoveEntFromPosition(int xp, int yp, int entID)
        {
            HashSet<int> al = Positions[yp, xp];
            //Game.MessageLog.Add($"beforehand = {al.Count.ToString()}");
            //Game.MessageLog.Add($"Before = {al.Count().ToString()}");
            //al.RemoveWhere(x => x == entID);
            al.Remove(entID);
            //Game.MessageLog.Add($"After = {al.Count().ToString()}");
        }

        public bool CheckPosForCollectableEnt(int xp, int yp)
        {
            HashSet<int> al = Positions[yp, xp];
            foreach (int eid in al)
            {
                int cb = EntityBitLookUp[eid];
                if ((cb & (int)Core.ComponentTypes.Collectable) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void SaveGame(RogueSharp.Map dm, int mapLevel, int randomSeed)
        {
            // save map
            Systems.SaveSystem saveSystem = new Systems.SaveSystem();
            saveSystem.SaveMap(dm);
            //save stats
            saveSystem.SaveStats(mapLevel, randomSeed);
            // save Entities
            saveSystem.SaveEntites(Entities);

        }

        public bool SomethingAtPosition(int xp, int yp)
        {
            bool found = false;
            string k = xp.ToString() + "-" + yp.ToString();
            found = EntityPostionLookUp.ContainsKey(k);

            return found;
        }

        public int GetMaxIDNumber()
        {
            return _entityID;
        }

        public void LoadEntities()
        {
            Systems.LoadSystem loadSys = new Systems.LoadSystem();
            Entities.Clear();
            Entities = loadSys.LoadEntities();
            if (Entities.Count > 0)
            {
                Game.MessageLog.Add("YAY!");
                EntityBitLookUp.Clear();
                foreach (KeyValuePair<int, List<Components.Component>> entry in Entities)
                {
                    List<Components.Component> compList = entry.Value;
                    int eid = entry.Key;
                    int checker = 0;

                    foreach (Components.Component c in compList)
                    {
                        Core.ComponentTypes ct = c.CompType;
                        checker = checker | (int)ct;
                    }

                    EntityBitLookUp.Add(eid, checker);
                }

                // reset max entityID counter
                List<int> idList = new List<int>(Entities.Keys);
                int maxID = idList.Max();
                _entityID = maxID + 1;
            }
        }
    }
}
