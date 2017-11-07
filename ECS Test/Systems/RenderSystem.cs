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
                int res = posBit & pair.Value;
                int res2 = rendBit & pair.Value;

                char renderChar = 'X';
                RLColor c = RLColor.White;

                List<Components.Component> comps = EntityManager.GetCompsByID(pair.Key);

                if (res > 0 && res2 > 0)
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
                int res3 = statBit & pair.Value;
                if (res3 > 0)
                {
                    Components.HealthComp healthStat = 
                        (Components.HealthComp)comps.Find(s => s.CompType == Core.ComponentTypes.Health);

                    Components.CreatureDetailsComp detailsComp =
                        (Components.CreatureDetailsComp)comps.Find(x => x.CompType == Core.ComponentTypes.CreatureDetails);
                    
                    if (healthStat != null && detailsComp != null)
                    {
                        statsConsole.Print(xPos, yPos, detailsComp.PersonalName + " the " + detailsComp.Name, c);
                        yPos++;
                        statsConsole.Print(xPos, yPos, renderChar.ToString(), c);
                        int width
                            = Convert.ToInt32(((double)healthStat.Health / (double)healthStat.MaxHealth) * 16);
                        int remainingWidth = 16 - width;
                        statsConsole.SetBackColor(xPos + 2, yPos, width, 1, Core.Swatch.Primary);
                        statsConsole.SetBackColor(xPos + 2 + width, yPos, remainingWidth, 1, Core.Swatch.PrimaryDarkest);
                        statsConsole.Print(xPos + 2, yPos, $": {healthStat.Health.ToString()}", Core.Swatch.DbLight);
                        yPos = yPos + 2;
                    }
                }
               
            }

            // stats console
            statsConsole.Print(1, 1, $"Y=: {yPos.ToString()}", Core.Colours.TextHeading);
        }
    }
}
