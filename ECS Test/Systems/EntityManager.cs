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
        public Random r;

        public EntityManager()
        {
            _entityID = 1;
            Entities = new Dictionary<int, List<Components.Component>>();
            EntityBitLookUp = new Dictionary<int, int>();
            EntityPostionLookUp = new Dictionary<string, List<Core.Entity>>();
            JustEntities = new Dictionary<int, Core.Entity>();
      
            r = new Random();
            CreatNames = ReadCreatureNames(100);
        }

        public void AddMonster(int x, int y)
        {
            Core.Entity e = new Core.Entity(_entityID);
            Core.EntityReturner er;
            
            int ind = r.Next(CreatNames.Count);
            string creatureName = CreatNames[ind];


            if (RogueSharp.DiceNotation.Dice.Roll("1d10") < 6)
            {
                er = Core.EntityFactory.CreateOrc(x, y, creatureName);
            }
            else
            {
                er = Core.EntityFactory.CreateKobold(x, y, creatureName);
            }

            //add entity to entity dict
            Entities.Add(_entityID, er.ComponentList);
            EntityBitLookUp.Add(_entityID, er.LookUpBit);
            JustEntities.Add(_entityID, e);

            //add to PositionLookUp
            AddEntToPosition(x, y, e);

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
            AddEntToPosition(x, y, e);

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
            AddEntToPosition(x, y, e);

            // inc entityID
            _entityID++;
        }

        public void AddTreasure(int x, int y)
        {
            var e = new Core.Entity(_entityID);
           // List<Components.Component> compList = new List<Components.Component>();

            Core.EntityReturner er = Core.EntityFactory.CreateGold(x, y);

            //add entity to entity dict
            Entities.Add(_entityID, er.ComponentList);
            EntityBitLookUp.Add(_entityID, er.LookUpBit);
            JustEntities.Add(_entityID, e);

            AddEntToPosition(x, y, e);

            // inc entityID
            _entityID++;
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
            foreach(KeyValuePair<int, int> entry in EntityBitLookUp)
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

        public List<Components.Component> GetCompsByID (int eid)
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

            foreach( int eid in entIDs )
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
                    string lu = posComp.X.ToString() + "-" + posComp.Y.ToString();
                    if (EntityPostionLookUp.ContainsKey(lu))
                    {
                        List<Core.Entity> entList = EntityPostionLookUp[lu];
                        entList.RemoveAll(x => x.UID == eid);
                        if (entList.Count == 0)
                        {
                            EntityPostionLookUp.Remove(lu);
                        }
                    }
                }
                JustEntities.Remove(eid);
                Entities.Remove(eid);
                EntityBitLookUp.Remove(eid);
            }
        }

        public void AddEntToPosition(int xp, int yp, Core.Entity ent)
        {
            string dictKey = xp.ToString() + "-" + yp.ToString();
            if (!EntityPostionLookUp.ContainsKey(dictKey))
            {
                EntityPostionLookUp.Add(dictKey, new List<Core.Entity>());
            }
            EntityPostionLookUp[dictKey].Add(ent);
        }

        public void RemoveEntFromPosition(int xp, int yp, Core.Entity ent)
        {
            string dictKey = xp.ToString() + "-" + yp.ToString();
            bool deleteMe = false;
            if (EntityPostionLookUp.ContainsKey(dictKey))
            {
                List<Core.Entity> l = EntityPostionLookUp[dictKey];
                l.Remove(ent);
                if (l.Count == 0)
                {
                    deleteMe = true;
                }
            }
            if (deleteMe)
            {
                EntityPostionLookUp.Remove(dictKey);
            }
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
