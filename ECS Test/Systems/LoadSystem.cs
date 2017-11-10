using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;

namespace ECS_Test.Systems
{
    class LoadSystem
    {
        public Dictionary<int, List<Components.Component>> LoadEntities()
        {
            Dictionary<int, List<Components.Component>> returnDict
                = new Dictionary<int, List<Components.Component>>();
              
            string inputJson = System.IO.File.ReadAllText("ents.json");

            Dictionary<int, Dictionary<Core.ComponentTypes, List<Dictionary<string, string>>>> output
                = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, Dictionary<Core.ComponentTypes, List<Dictionary<string, string>>>>>(inputJson);

            foreach (KeyValuePair<int, Dictionary<Core.ComponentTypes, List<Dictionary<string, string>>>> entry in output)
            {
                int eid = entry.Key;
                Core.Entity ent = new Core.Entity(eid);

                List<Components.Component> componentList = new List<Components.Component>();

                Dictionary<Core.ComponentTypes, List<Dictionary<string, string>>> compDict 
                    = entry.Value;

                foreach(KeyValuePair<Core.ComponentTypes, List<Dictionary<string, string>>> inner in compDict)
                {
                    if (inner.Key == Core.ComponentTypes.Position)
                    {
                        List<Dictionary<string, string>> comps = inner.Value;
                        foreach(Dictionary<string, string> cDict in comps)
                        {
                            int x = Int32.Parse(cDict["X"]);
                            int y = Int32.Parse(cDict["Y"]);

                            Components.PositionComp pc = new Components.PositionComp(x, y);
                            componentList.Add(pc);
                        }
                    }
                    else if (inner.Key == Core.ComponentTypes.Render)
                    {
                        List<Dictionary<string, string>> comps = inner.Value;
                        foreach (Dictionary<string, string> cDict in comps)
                        {
                            char glyph = cDict["Glyph"][0];
                            float r = float.Parse(cDict["ColorR"]);
                            float g = float.Parse(cDict["ColorG"]);
                            float b = float.Parse(cDict["ColorB"]);
                            RLNET.RLColor c = new RLNET.RLColor(r, g, b);

                            Components.RenderComp rc = new Components.RenderComp(glyph, c);
                            componentList.Add(rc);
                        }
                    }
                    //else if (inner.Key == Core.ComponentTypes.Health)
                    //{
                    //    List<Dictionary<string, string>> comps = inner.Value;
                    //    foreach (Dictionary<string, string> cDict in comps)
                    //    {
                    //        int h = Int32.Parse(cDict["Health"]);
                    //        int mh = Int32.Parse(cDict["MaxHealth"]);

                    //        Components.HealthComp msc = new Components.HealthComp();
                    //        msc.Health = h;
                    //        msc.MaxHealth = mh;

                    //        componentList.Add(msc);
                    //    }
                    //}
                    //else if (inner.Key == Core.ComponentTypes.Attributes)
                    //{
                    //    List<Dictionary<string, string>> comps = inner.Value;
                    //    foreach (Dictionary<string, string> cDict in comps)
                    //    {
                    //        int aw = Int32.Parse(cDict["Awareness"]);

                    //        Components.AttributesComp msc = new Components.AttributesComp(aw);

                    //        componentList.Add(msc);
                    //    }
                    //}
                    //else if (inner.Key == Core.ComponentTypes.AI)
                    //{
                    //    List<Dictionary<string, string>> comps = inner.Value;
                    //    foreach (Dictionary<string, string> cDict in comps)
                    //    {
                    //        bool aw = bool.Parse(cDict["HasAI"]);
                       
                    //        Components.AIComp msc = new Components.AIComp();
                    //        msc.HasAI = aw;
                    //        componentList.Add(msc);
                    //    }
                    //}
                }
                returnDict.Add(eid, componentList);
            }
            return returnDict;
        }

        public RogueSharp.MapState LoadMap()
        {
            string inputJson = System.IO.File.ReadAllText("map.json");

            RogueSharp.MapState map =
                Newtonsoft.Json.JsonConvert.DeserializeObject<RogueSharp.MapState>(inputJson);

            return map;
        }

        public Core.GameSaveInfo LoadStats()
        {
            string inputJson = System.IO.File.ReadAllText("game.json");

            Core.GameSaveInfo gsi = 
                Newtonsoft.Json.JsonConvert.DeserializeObject<Core.GameSaveInfo>(inputJson);

            return gsi;
        }

    }
}
