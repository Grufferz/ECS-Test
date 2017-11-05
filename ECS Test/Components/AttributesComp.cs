using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Components
{
    class AttributesComp : Component
    {
        public int Awareness { get; set; }
        public int Size { get; set; }
        public int Level { get; set; }
        public int Strength { get; set; }
        public int Hardiness { get; set; }
        public int Dexterity { get; set; }
        public int Power { get; set; }
        public int Intelligence { get; set; }
        public int Luck { get; set; }
        public int XP { get; set; }
        public int Sanity { get; set; }
        public string DmgBonus { get; set; }
        public int DmgMod { get; set; }

        public AttributesComp(int a, int si, int lev, int intBase )
        {
            Size = si;
            Awareness = a;
            Level = lev;
            CompType = Core.ComponentTypes.Attributes;

            // roll attributes
            Strength = RogueSharp.DiceNotation.Dice.Roll("3d6");
            Hardiness = RogueSharp.DiceNotation.Dice.Roll("3d6");
            Dexterity = RogueSharp.DiceNotation.Dice.Roll("3d6");
            Power = RogueSharp.DiceNotation.Dice.Roll("3d6");
            Luck = RogueSharp.DiceNotation.Dice.Roll("3d6");
            string intSt = intBase.ToString() + "d6";
            Intelligence = RogueSharp.DiceNotation.Dice.Roll(intSt);

            Sanity = Power * 5;

            int dmgSw = Strength + Size;

            if (dmgSw < 13)
            {
                DmgBonus = "1d6";
                DmgMod = -1;
            }
            else if (dmgSw > 12 && dmgSw < 17)
            {
                DmgBonus = "1d4";
                DmgMod = -1;
            }
            else if (dmgSw > 16 && dmgSw < 25)
            {
                DmgBonus = "0";
                DmgMod = 0;
            }
            else if (dmgSw > 24 && dmgSw < 33)
            {
                DmgBonus = "1d4";
                DmgMod = 1;
            }
            else if (dmgSw > 32 && dmgSw < 41)
            {
                DmgBonus = "1d6";
                DmgMod = 1;
            }
            else if (dmgSw > 40 && dmgSw < 57)
            {
                DmgBonus = "2d6";
                DmgMod = 1;
            }
            else if (dmgSw > 56 && dmgSw < 73)
            {
                DmgBonus = "3d6";
                DmgMod = 1;
            }
            else if (dmgSw > 72 && dmgSw < 89)
            {
                DmgBonus = "4d6";
                DmgMod = 1;
            }
            else
            {
                DmgBonus = "5d6";
                DmgMod = 1;
            }
        }
    }
}
