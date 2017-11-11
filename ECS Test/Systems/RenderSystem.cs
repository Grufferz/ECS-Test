using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;

namespace ECS_Test.Systems
{
    class RenderSystem
    {
        public static EntityManager EntityManager { get; private set; }

        public RenderSystem(EntityManager em)
        {
            EntityManager = em;
        }

        public void Update(RLConsole console, RLConsole statsConsole)
        {
            var posBit = (int)Core.ComponentTypes.Position;
            var rendBit = (int)Core.ComponentTypes.Render;
            int statBit = (int)Core.ComponentTypes.Health;
            int detailsBit = (int)Core.ComponentTypes.CreatureDetails;
            int yPos = 16;
            int xPos = 2;

            Dictionary<int, int> ents = EntityManager.EntityBitLookUp;

            foreach (KeyValuePair<int, int> pair in ents)
            {
                int eid = pair.Key;
                int furnitureRes = (int)Core.ComponentTypes.Furniture & pair.Value;
                if(furnitureRes > 0)
                {
                    Components.RenderComp rc = (Components.RenderComp)EntityManager.GetSingleComponentByID(eid, Core.ComponentTypes.Render);
                    Components.PositionComp pc = (Components.PositionComp)EntityManager.GetSingleComponentByID(eid, Core.ComponentTypes.Position);
                    console.Set(pc.X, pc.Y, rc.Colour, Core.Colours.FloorBackground, rc.Glyph);
                }
            }

            foreach (KeyValuePair<int, int> pair in ents)
            {
                int res = posBit & pair.Value;
                int res2 = rendBit & pair.Value;
                int actRes = (int)Core.ComponentTypes.Actor & pair.Value;

                char renderChar = 'X';
                RLColor c = RLColor.White;

                List<Components.Component> comps = EntityManager.GetCompsByID(pair.Key);

                if ((res > 0 && res2 > 0) && (actRes == 0))
                {

                    Components.RenderComp rendComp 
                        = (Components.RenderComp)comps.Find(s => s.CompType == Core.ComponentTypes.Render);

                    Components.PositionComp posComp =
                        (Components.PositionComp)comps.Find(s => s.CompType == Core.ComponentTypes.Position);

                    if (rendComp != null && posComp != null)
                    {
                        console.Set(posComp.X, posComp.Y, rendComp.Colour, 
                            Core.Colours.FloorBackground, rendComp.Glyph);
                    }
                    if (rendComp != null)
                    {
                        renderChar = rendComp.Glyph;
                        c = rendComp.Colour;
                    }
                }

                //draw stats

               
            }
            foreach (KeyValuePair<int, int> pair in ents)
            {
                // draw only actors on top
                if ((pair.Value & (int)Core.ComponentTypes.Actor) > 0)
                {

                    List<Components.Component> comps = EntityManager.GetCompsByID(pair.Key);

                    Components.RenderComp rendComp = (Components.RenderComp)comps.Find(s => s.CompType == Core.ComponentTypes.Render);

                    Components.PositionComp posComp =
                        (Components.PositionComp)comps.Find(s => s.CompType == Core.ComponentTypes.Position);
                    Components.AIComp aiComp = (Components.AIComp)comps.Find(x => x.CompType == Core.ComponentTypes.AI);

                    RLColor rColor;

                    if (aiComp.Fleeing)
                    {
                        rColor = RLColor.Red;
                    }
                    else
                    {
                        rColor = rendComp.Colour;
                    }

                    if (rendComp != null && posComp != null)
                    {
                        console.Set(posComp.X, posComp.Y, rColor,
                            Core.Colours.FloorBackground, rendComp.Glyph);
                    }

                    int res3 = statBit & pair.Value;
                    if (res3 > 0)
                    {
                        Components.HealthComp healthStat =
                            (Components.HealthComp)comps.Find(s => s.CompType == Core.ComponentTypes.Health);

                        Components.CreatureDetailsComp detailsComp =
                            (Components.CreatureDetailsComp)comps.Find(x => x.CompType == Core.ComponentTypes.CreatureDetails);

                        Components.InventoryComp invComp = (Components.InventoryComp)comps.Find(x => x.CompType == Core.ComponentTypes.Inventory);

                        if (healthStat != null && detailsComp != null)
                        {
                            statsConsole.Print(xPos, yPos, detailsComp.PersonalName + " the " + detailsComp.Name, rendComp.Colour);
                            yPos++;
                            statsConsole.Print(xPos, yPos, rendComp.Glyph.ToString(), rendComp.Colour);
                            int width
                                = Convert.ToInt32(((double)healthStat.Health / (double)healthStat.MaxHealth) * 16);
                            int remainingWidth = 16 - width;
                            statsConsole.SetBackColor(xPos + 2, yPos, width, 1, Core.Swatch.Primary);
                            statsConsole.SetBackColor(xPos + 2 + width, yPos, remainingWidth, 1, Core.Swatch.PrimaryDarkest);
                            statsConsole.Print(xPos + 2, yPos, $": {healthStat.Health.ToString()}", Core.Swatch.DbLight);
                            yPos++;
                            statsConsole.Print(xPos, yPos, $"Carrying {invComp.Treasure.Count.ToString()} Gold", rendComp.Colour);
                            yPos = yPos + 2;
                        }
                    }
                }
            }
            // stats console
            int count = 0;
            for (int yp = 0; yp < EntityManager.GetHeight(); yp++)
            {
                for (int xp=0; xp < EntityManager.GetWidth(); xp++ )
                {
                    if (EntityManager.Positions[yp,xp].Count > 0)
                    {
                        HashSet<int> ent = EntityManager.Positions[yp, xp];
         
                        foreach(int indvID in ent)
                        {
                            List<Components.Component> inner = EntityManager.Entities[indvID];
                            foreach( Components.Component ec in inner)
                            {
                                if (ec.CompType == Core.ComponentTypes.Collectable)
                                {
                                    count++;
                                }
                            }
                        }
                    }
                }
            }

            statsConsole.Print(1, 1, $"collectables=: {count.ToString()}", Core.Colours.TextHeading);
        }
    }
}
