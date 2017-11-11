using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS_Test.Systems
{
    class CollisionSystem : IBaseSystem
    {
        private EntityManager _entityManager;

        public CollisionSystem(EntityManager em)
        {
            _entityManager = em;
            Core.EventBus.Subscribe(Core.EventTypes.CollisionCheck, (sender, e) => OnMessage(e));
        }

        public void Update()
        {

        }

        public void OnMessage(Core.MessageEventArgs e)
        {
            switch(e.MessageType)
            {
                case Core.EventTypes.CollisionCheck:

                    Core.CollisionEventArgs details = (Core.CollisionEventArgs)e;
                    //check entity manager lookup dict
                    //string posKey = details.x.ToString() + "-" + details.y.ToString();
                    bool collision = false;
                    List<int> deathList = new List<int>();

                    //if (_entityManager.EntityPostionLookUp.ContainsKey(posKey))

                    int entIDRequestingMove = details.EntRequestingMove.UID;

                    HashSet<int> innerIDS = _entityManager.Positions[details.y, details.x];
                    foreach( int ids in innerIDS)
                    {
                        // we have something there...

                        //List<Core.Entity> entL = _entityManager.EntityPostionLookUp[posKey];
                        Components.PositionComp pc
                            = (Components.PositionComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.Position);
                        
                        if (pc.X == details.x && pc.Y == details.y)
                        {
                            int checker = _entityManager.EntityBitLookUp[ids];
                            if ((checker & (int)Core.ComponentTypes.Health) > 0)
                            {
                                //Game.MessageLog.Add("FIGHT NOW");
                                collision = true;

                                //get details of person being hit
                                Components.HealthComp hComp =
                                    (Components.HealthComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.Health);
                                Components.AttributesComp attComp 
                                    = (Components.AttributesComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.Attributes);
                                Components.AIComp aiComp = (Components.AIComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.AI);

                                // get hitter details
                                Components.AttributesComp attOfHitterComp 
                                    = (Components.AttributesComp)_entityManager.GetSingleComponentByID(entIDRequestingMove, Core.ComponentTypes.Attributes);

                                bool hit = false;
                                int dmg = 1;

                                if (RogueSharp.DiceNotation.Dice.Roll("1d20") < attOfHitterComp.Strength)
                                {
                                    hit = true;
                                    
                                    // get weapon we are using
                                    Components.InventoryComp invComp 
                                        = (Components.InventoryComp)_entityManager.GetSingleComponentByID(entIDRequestingMove, Core.ComponentTypes.Inventory);

                                    // get damage bonus
                                    int dmgBonus = RogueSharp.DiceNotation.Dice.Roll(attOfHitterComp.DmgBonus);
                                    dmgBonus *= attOfHitterComp.DmgMod;

                                    if (invComp.CurrentWeapon > 0)
                                    {
                                        Components.WeaponComp weapon 
                                            = (Components.WeaponComp)_entityManager.GetSingleComponentByID(invComp.CurrentWeapon, Core.ComponentTypes.Weapon);
                                        int dmgB = weapon.DamageBase;
                                        string d = "1d" + dmgB.ToString();
                                        dmg = RogueSharp.DiceNotation.Dice.Roll(d);

                                    }
                                    int totalDamage = dmgBonus + dmg;
                                    if (totalDamage < 0){ totalDamage = 0; }
                                    hComp.Health -= totalDamage;
                                    if (hComp.Health < 0)
                                    {
                                        hComp.Health = 0;
                                    }

                                    attComp.Morale -= totalDamage;
                                    if (attComp.Morale < 1)
                                    {
                                        attComp.Morale = 0;
                                    }

                                    if (hComp.Health < (hComp.MaxHealth * 0.2))
                                    {
                                        aiComp.Fleeing = true;
                                    }

                                }


                                aiComp.UnderAttack = true;

                                aiComp.LastBasher = entIDRequestingMove;
                                if (!aiComp.CurrentEnemies.Contains(entIDRequestingMove))
                                {
                                    aiComp.CurrentEnemies.Add(entIDRequestingMove);
                                }
                                Components.CreatureDetailsComp cdComp =
                                    (Components.CreatureDetailsComp)_entityManager.GetSingleComponentByID(ids, Core.ComponentTypes.CreatureDetails);
                                List<Components.Component> ourList = _entityManager.GetCompsByID(details.EntRequestingMove.UID);
                                Components.CreatureDetailsComp ourComp =
                                    (Components.CreatureDetailsComp)ourList.FirstOrDefault(x => x.CompType == Core.ComponentTypes.CreatureDetails);
                                if (hit)
                                {
                                    Game.MessageLog.Add($"{ourComp.PersonalName} Bashes {cdComp.PersonalName} for {dmg} Damage");
                                }
                                else
                                {
                                    Game.MessageLog.Add($"{ourComp.PersonalName} Misses {cdComp.PersonalName}");
                                }
                                

                                if (hComp.Health < 1)
                                {
                                    // entity is dead, collect up and kill at end
                                    deathList.Add(ids);

                                }
                            }
                        }
                    }

                    foreach (int i in deathList)
                    {
                        Core.DeleteEntEventArgs msg = new Core.DeleteEntEventArgs(Core.EventTypes.DeadEntity, i);
                        Core.EventBus.Publish(Core.EventTypes.DeadEntity, msg);
                    }

                    if (!collision)
                    { 
                        // move okay
                        Core.MoveOkayEventArgs msg = new Core.MoveOkayEventArgs(Core.EventTypes.MoveOK, details.EntRequestingMove, details.x, details.y);
                        Core.EventBus.Publish(Core.EventTypes.MoveOK, msg);
                    }
                    //else
                    //{
                    //    // fight on
                    //    Core.NoMoveEventArgs msg 
                    //        = new Core.NoMoveEventArgs(Core.EventTypes.NoMove, details.EntRequestingMove);
                    //    Core.EventBus.Publish(Core.EventTypes.NoMove, msg);

                    //}
                    break;
            }
        }
    }
}
