using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Core
{
    class EntityFactory
    {

        public static EntityReturner CreateDoor(int x, int y, bool isOpen)
        {
            List<Components.Component> compList = new List<Components.Component>();
            
            // set bitwise to 0
            int checker = 0;

            Components.PositionComp positionComp = new Components.PositionComp(x, y);
            compList.Add(positionComp);
            checker = checker | (int)Core.ComponentTypes.Position;

            char c = isOpen ? '-' : '+';
            Components.RenderComp rendComp = new Components.RenderComp(c, Colours.Door);
            compList.Add(rendComp);
            checker = checker | (int)Core.ComponentTypes.Render;

            Components.DoorComp doorComp = new Components.DoorComp(isOpen, false);
            compList.Add(doorComp);
            checker = checker | (int)Core.ComponentTypes.Door;

            EntityReturner er = new EntityReturner(checker, compList);
            return er;
        }

        public static EntityReturner CreateOrc(int xPos, int yPos, string name)
        {
            List<Components.Component> compList = new List<Components.Component>();
            //stats
            int lev = 1;
            int size = 7;
            int intBase = 1;

            // set bitwise to 0
            int checker = 0;

            Components.PositionComp positionComp = new Components.PositionComp(xPos, yPos);
            compList.Add(positionComp);
            checker = checker | (int)Core.ComponentTypes.Position;

            Components.RenderComp rendComp = new Components.RenderComp('o', Core.Colours.OrcColour);
            compList.Add(rendComp);
            checker = checker | (int)Core.ComponentTypes.Render;

            Components.AttributesComp attComp = new Components.AttributesComp(15, size, lev, intBase);
            compList.Add(attComp);
            checker = checker | (int)Core.ComponentTypes.Attributes;

            //get base factor for health
            int hp = (int)(size + attComp.Hardiness) / 2;

            Components.HealthComp healthComp = new Components.HealthComp(hp);
            compList.Add(healthComp);
            checker = checker | (int)Core.ComponentTypes.Health;

            Components.ActorComp actComp = new Components.ActorComp();
            compList.Add(actComp);
            checker = checker | (int)Core.ComponentTypes.Actor;

            Components.AIComp aiComp = new Components.AIComp();
            compList.Add(aiComp);
            checker = checker | (int)Core.ComponentTypes.AI;

            int speed = Game.Random.Next(3) + 2;
            Components.SchedulableComp shedComp = new Components.SchedulableComp(speed);
            compList.Add(shedComp);
            checker = checker | (int)Core.ComponentTypes.Schedulable;

            Components.InventoryComp invComp = new Components.InventoryComp();
            compList.Add(invComp);
            checker = checker | (int)Core.ComponentTypes.Inventory;

            Components.CreatureDetailsComp detailsComp
                = new Components.CreatureDetailsComp("Orc", name, Types.CreatureTypes.Orc);
            compList.Add(detailsComp);
            checker = checker | (int)ComponentTypes.CreatureDetails;

            EntityReturner er = new EntityReturner(checker, compList);
            return er;
        }

        public static EntityReturner CreateGold(int x, int y)
        {
            // set bitwise to 0
            int checker = 0;

            List<Components.Component> compList = new List<Components.Component>();

            Components.PositionComp positionComp = new Components.PositionComp(x, y);
            compList.Add(positionComp);
            checker = checker | (int)Core.ComponentTypes.Position;

            Components.RenderComp rendComp = new Components.RenderComp('$', RLNET.RLColor.Yellow);
            compList.Add(rendComp);
            checker = checker | (int)Core.ComponentTypes.Render;

            Components.ItemValueComp valComp = new Components.ItemValueComp(RogueSharp.DiceNotation.Dice.Roll("1d6"));
            compList.Add(valComp);
            checker = checker | (int)Core.ComponentTypes.ItemValue;

            Game.MessageLog.Add($"V={valComp.ItemValue.ToString()}");

            Components.CollectableComp collComp 
                = new Components.CollectableComp(1, true, true, Types.ItemTypes.Treasure);
            compList.Add(collComp);
            checker = checker | (int)Core.ComponentTypes.Collectable;

            EntityReturner er = new EntityReturner(checker, compList);
            return er;
        }

        public static EntityReturner CreateKobold(int xPos, int yPos, string name)
        {
            List<Components.Component> compList = new List<Components.Component>();

            //stats
            int lev = 1;
            int size = 4;
            int intBase = 1;

            // set bitwise to 0
            int checker = 0;

            Components.PositionComp positionComp = new Components.PositionComp(xPos, yPos);
            compList.Add(positionComp);
            checker = checker | (int)Core.ComponentTypes.Position;

            Components.RenderComp rendComp = new Components.RenderComp('k', RLNET.RLColor.Green);
            compList.Add(rendComp);
            checker = checker | (int)Core.ComponentTypes.Render;

            Components.AttributesComp attComp = new Components.AttributesComp(15, size, lev, intBase);
            compList.Add(attComp);
            checker = checker | (int)Core.ComponentTypes.Attributes;

            //get base factor for health
            int hp = (int)(size + attComp.Hardiness) / 2;

            Components.HealthComp healthComp = new Components.HealthComp(hp);
            compList.Add(healthComp);
            checker = checker | (int)Core.ComponentTypes.Health;

            Components.ActorComp actComp = new Components.ActorComp();
            compList.Add(actComp);
            checker = checker | (int)Core.ComponentTypes.Actor;

            Components.AIComp aiComp = new Components.AIComp();
            compList.Add(aiComp);
            checker = checker | (int)Core.ComponentTypes.AI;

            int speed = Game.Random.Next(2) + 1;
            Components.SchedulableComp shedComp = new Components.SchedulableComp(speed);
            compList.Add(shedComp);
            checker = checker | (int)Core.ComponentTypes.Schedulable;

            Components.InventoryComp invComp = new Components.InventoryComp();
            compList.Add(invComp);
            checker = checker | (int)Core.ComponentTypes.Inventory;

            Components.CreatureDetailsComp detailsComp
                = new Components.CreatureDetailsComp("Koblod", name, Types.CreatureTypes.Kobold);
            compList.Add(detailsComp);
            checker = checker | (int)ComponentTypes.CreatureDetails;

            EntityReturner er = new EntityReturner(checker, compList);
            return er;
        }

        public static EntityReturner CreateStairs(int xp, int yp, bool isUp)
        {
            List<Components.Component> compList = new List<Components.Component>();

            // set bitwise to 0
            int checker = 0;

            Components.PositionComp positionComp = new Components.PositionComp(xp, yp);
            compList.Add(positionComp);
            checker = checker | (int)Core.ComponentTypes.Position;

            char c = isUp ? '<' : '>';
            Components.RenderComp rendComp = new Components.RenderComp(c, RLNET.RLColor.White);
            compList.Add(rendComp);
            checker = checker | (int)Core.ComponentTypes.Render;

            Components.StairComp stairComp = new Components.StairComp(isUp);
            compList.Add(stairComp);
            checker = checker | (int)Core.ComponentTypes.Stairs;

            Components.UseableComp useComp = new Components.UseableComp();
            compList.Add(useComp);
            checker = checker | (int)Core.ComponentTypes.Useable;

            EntityReturner er = new EntityReturner(checker, compList);
            return er;
        }
    }
}
