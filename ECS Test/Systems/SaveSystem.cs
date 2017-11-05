using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft;

namespace ECS_Test.Systems
{
    class SaveSystem
    {
        public void SaveEntites(Dictionary<int, List<Components.Component>> entDict)
        {
            Dictionary<int, Dictionary<Core.ComponentTypes, List<Dictionary<string, string>>>> outputDict 
                = new Dictionary<int, Dictionary<Core.ComponentTypes, List<Dictionary<string, string>>>>();

            foreach( KeyValuePair<int, List<Components.Component>> entry in entDict)
            {
                int entID = entry.Key;
                List<Components.Component> compList = entry.Value;

                Dictionary<Core.ComponentTypes, List<Dictionary<string, string>>> rendDict = 
                    new Dictionary<Core.ComponentTypes, List<Dictionary<string, string>>>();

                // save render comp
                List<Components.Component> rendCompList 
                    = compList.FindAll(s => s.CompType == Core.ComponentTypes.Render);
                if (rendCompList != null)
                {
                    List<Dictionary<string, string>> rendInfo = SaveRenderComp(rendCompList);
                    rendDict.Add(Core.ComponentTypes.Render, rendInfo);
                }

                // save positionComp
                List<Components.Component> posComp =
                    compList.FindAll(s => s.CompType == Core.ComponentTypes.Position);
                if (posComp != null)
                {
                    List<Dictionary<string, string>> returnInfo = SavePositionComp(posComp);
                    rendDict.Add(Core.ComponentTypes.Position, returnInfo);
                }

                //save health stats
                List<Components.Component> healthC 
                    = compList.FindAll(s => s.CompType == Core.ComponentTypes.Health);
                if (healthC != null)
                {
                    List<Dictionary<string, string>> returnInfo = SaveHealthComp(healthC);
                    rendDict.Add(Core.ComponentTypes.Health, returnInfo);
                }

                //save attributes
                List<Components.Component> attC 
                    = compList.FindAll(s => s.CompType == Core.ComponentTypes.Attributes);
                if (attC != null)
                {
                    List<Dictionary<string, string>> returnInfo = SaveAttributesComp(attC);
                    rendDict.Add(Core.ComponentTypes.Attributes, returnInfo);
                }

                //save AI
                List<Components.Component> aiC
                    = compList.FindAll(s => s.CompType == Core.ComponentTypes.AI);
                if (aiC != null)
                {
                    List<Dictionary<string, string>> returnInfo = SaveAIComp(aiC);
                    rendDict.Add(Core.ComponentTypes.AI, returnInfo);
                }

                //save schedulable
                List<Components.Component> shedC
                    = compList.FindAll(s => s.CompType == Core.ComponentTypes.Schedulable);
                if (shedC != null)
                {
                    List<Dictionary<string, string>> returnInfo = SaveShedComp(shedC);
                    rendDict.Add(Core.ComponentTypes.Schedulable, returnInfo);
                }

                outputDict.Add(entID, rendDict);

            }
            string entsJson = Newtonsoft.Json.JsonConvert.SerializeObject(outputDict,
                Newtonsoft.Json.Formatting.Indented);

            using (StreamWriter sw = new StreamWriter("ents.json"))
            {
                sw.Write(entsJson);
            }

        }

        public void SaveStats(int mapLevel, int randomSeed)
        {
            Core.GameSaveInfo gameSaver = new Core.GameSaveInfo(randomSeed, mapLevel);

            string gameJson = Newtonsoft.Json.JsonConvert.SerializeObject(gameSaver, 
                Newtonsoft.Json.Formatting.Indented);

            using (StreamWriter sw = new StreamWriter("game.json"))
            {
                sw.Write(gameJson);
            }
        }

        private List<Dictionary<string, string>> SaveHealthComp(List<Components.Component> msList)
        {
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
            foreach (Components.HealthComp listItem in msList)
            {
                Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
                tempDictionary.Add("Health", listItem.Health.ToString());
                tempDictionary.Add("MaxHealth", listItem.MaxHealth.ToString());

                returnList.Add(tempDictionary);
            }
            return returnList;
        }

        private List<Dictionary<string, string>> SaveRenderComp(List<Components.Component> rendList)
        {
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
            foreach(Components.RenderComp listItem in rendList)
            {
                Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
                tempDictionary.Add("Glyph", listItem.Glyph.ToString());
                float g = listItem.Colour.g;
                float b = listItem.Colour.b;
                float r = listItem.Colour.r;

                tempDictionary.Add("ColorR", r.ToString());
                tempDictionary.Add("ColorB", b.ToString());
                tempDictionary.Add("ColorG", g.ToString());

                returnList.Add(tempDictionary);
            }
            return returnList;
        }

        private List<Dictionary<string, string>> SaveShedComp(List<Components.Component> shedList)
        {
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
            foreach (Components.SchedulableComp listItem in shedList)
            {
                Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
                tempDictionary.Add("Time", listItem.Time.ToString());

                returnList.Add(tempDictionary);
            }
            return returnList;
        }

        private List<Dictionary<string, string>> SaveAIComp(List<Components.Component> aiList)
        {
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
            foreach (Components.AIComp listItem in aiList)
            {
                Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
                tempDictionary.Add("HasAI", listItem.HasAI.ToString());

                returnList.Add(tempDictionary);
            }
            return returnList;
        }

        private List<Dictionary<string, string>> SaveAttributesComp(List<Components.Component> attList)
        {
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
            foreach (Components.AttributesComp listItem in attList)
            {
                Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
                tempDictionary.Add("Awareness", listItem.Awareness.ToString());

                returnList.Add(tempDictionary);
            }
            return returnList;
        }

        private List<Dictionary<string, string>> SavePositionComp(List<Components.Component> aList)
        {
            List<Dictionary<string, string>> returnList = new List<Dictionary<string, string>>();
            foreach (Components.PositionComp listItem in aList)
            {
                Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
                tempDictionary.Add("X", listItem.X.ToString());
                tempDictionary.Add("Y", listItem.Y.ToString());

                returnList.Add(tempDictionary);
            }
            return returnList;
        }

        public void SaveMap(RogueSharp.Map dungeonMap)
        {
            RogueSharp.MapState saveMap = dungeonMap.Save();

            string mapJson = Newtonsoft.Json.JsonConvert.SerializeObject(saveMap, 
                Newtonsoft.Json.Formatting.Indented);

            using (StreamWriter sw = new StreamWriter("map.json"))
            {
                sw.Write(mapJson);
            }
        }


    }
}
