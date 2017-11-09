using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;

namespace ECS_Test
{

    class Game
    {
        private static readonly int _screenWidth = 160;
        private static readonly int _screenHeight = 90;
        private static RLRootConsole _rootConsole;

        //map console
        private static readonly int _mapWidth = 130;
        private static readonly int _mapHeight = 60;
        private static RLConsole _mapConsole;

        //message console
        private static readonly int _messageWidth = 130;
        private static readonly int _messageHeight = 15;
        private static RLConsole _messageConsole;

        //stat console
        private static readonly int _statWidth = 36;
        private static readonly int _statHeight = 75;
        private static RLConsole _statConsole;

        //inventory console
        private static readonly int _inventoryWidth = 130;
        private static readonly int _inventoryHeight = 15;
        private static RLConsole _inventoryConsole;

        public static Systems.MessageLog MessageLog;

        public static RogueSharp.Random.IRandom Random { get; set; }

        private static bool _renderRequired = true;
        public static Systems.CommandSystem CommandSystem { get; private set; } 
        public static Systems.SystemManager SystemsManager { get; private set; }
        public static Systems.EntityManager EntityManager { get; private set; }
        public static Systems.RenderSystem RenderSystem { get; private set; }
        public static Systems.SchedulingSystem ShedSystem { get; private set; }
        public static Systems.AISystem AISystem { get; private set; }
        public static Systems.MovementSystem MovementSystem { get; private set; }
        public static Systems.CollisionSystem CollisionSystem { get; private set; }
        public static Systems.UseSystem UseSystem { get; private set; }
        public static Systems.GarbageSystem GarbageSystem { get; private set; }
        public static Systems.InventorySystem InventorySystem { get; private set; }

        public static Core.DungeonMap DungeonMap { get; private set; }

        private static int _mapLevel = 1;
        private static int seed = (int)DateTime.UtcNow.Ticks;
        private static int _turn = 0;


        static void Main(string[] args)
        {
           
            Random = new RogueSharp.Random.DotNetRandom(seed);

            // create output system
            string fontFileName = "terminal8x8.png";
            string consoleTitle = $"ECS Test Level = {_mapLevel}";

            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight,
    8, 8, 1f, consoleTitle);
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);
            
            // create systems, entityManager first
            EntityManager = new Systems.EntityManager();

            CommandSystem = new Systems.CommandSystem();
            MessageLog = new Systems.MessageLog();

            SystemsManager = new Systems.SystemManager(EntityManager);
            RenderSystem = new Systems.RenderSystem(EntityManager);
            ShedSystem = new Systems.SchedulingSystem();
            CollisionSystem = new Systems.CollisionSystem(EntityManager);
            UseSystem = new Systems.UseSystem(EntityManager);
            InventorySystem = new Systems.InventorySystem(EntityManager);

            //listen for critical events
            Core.EventBus.Subscribe(Core.EventTypes.GameCritical, (sender, e) => OnMessage(e));


            //MessageLog.Add("Rogue At level 1");

            //create map stuff
            Systems.MapGenerator mapGenerator = new Systems.MapGenerator(_mapWidth, 
                _mapHeight, 25, 20, 7, _mapLevel, EntityManager);
            DungeonMap = mapGenerator.CreateMap();
            MovementSystem = new Systems.MovementSystem(DungeonMap, EntityManager);

            // place monsters
            mapGenerator.PlaceMonsters(EntityManager);
            mapGenerator.PlaceGold(EntityManager);

            //run initial FOV for monsters
            DungeonMap.UpdateFOVForMonsters(EntityManager);

            //creat garbage system
            GarbageSystem = new Systems.GarbageSystem(ShedSystem, EntityManager, DungeonMap);

            //create AI system
            AISystem = new Systems.AISystem(EntityManager, DungeonMap, GarbageSystem);

            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Core.Swatch.DbWood);
            _inventoryConsole.Print(1, 1, "Inventory", Core.Colours.TextHeading);

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;
            _rootConsole.Run();
        }

        private static void OnMessage(Core.MessageEventArgs e)
        {
            if (e.MessageType == Core.EventTypes.GameCritical)
            {
                Core.GameCriticalEventArgs gce = (Core.GameCriticalEventArgs)e;

                switch(gce.gameEvent)
                {
                    case Core.GameEvents.QUIT:
                        _rootConsole.Close();
                        break;
                }
            }
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            //capture keys
            //bool didPlayerAct = false;
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            if (keyPress != null)
            {
              //  didPlayerAct = true;

                // send message saying key pressed....
                Core.EventTypes t = Core.EventTypes.KeyPressed;
                Core.KeyPressEventArgs args = new Core.KeyPressEventArgs(t, keyPress);
                Core.EventBus.Publish(Core.EventTypes.KeyPressed, args);
            }

            //    else if (keyPress.Key == RLKey.Space)
            //    {
            //        //SAVE GAME
            //        Systems.SaveSystem saveSystem = new Systems.SaveSystem();
            //        saveSystem.SaveMap(DungeonMap);
            //        //save stats
            //        saveSystem.SaveStats(_mapLevel, seed);
            //        // save Entities
            //        saveSystem.SaveEntites(EntityManager.GetEntities());
            //        //EntityManager.SaveGame(DungeonMap, _mapLevel, seed);
            //    }
            //    else if (keyPress.Key == RLKey.Enter)
            //    {
            //        // LOAD SAVED GAME
            //        Systems.LoadSystem ls = new Systems.LoadSystem();
            //        // load stats and stuff
            //        Core.GameSaveInfo gsi = ls.LoadStats();

            //        // load map and restore
            //        _mapLevel = gsi.MapLevel;
            //        RogueSharp.MapState ms = ls.LoadMap();
            //        DungeonMap.Restore(ms);

            //        MessageLog = new Systems.MessageLog();
            //        CommandSystem = new Systems.CommandSystem();

            //        //TODO - restore scheuling system... 

            //        _rootConsole.Title = $"ECS Test Level - {_mapLevel}";

            //        // load entities
            //        EntityManager.ClearEntities();
            //        EntityManager.LoadEntities();

            //        didPlayerAct = true;
            //    }
            //}

            //if (didPlayerAct)
            //{

            _renderRequired = true;

            if (!ShedSystem.IsEmpty())
            {
                Core.Entity nextEnt = ShedSystem.Get();

                // ai requests doing something
                Core.AIReqMessageEventArgs aiReq
                    = new Core.AIReqMessageEventArgs(Core.EventTypes.AIRequest, nextEnt);
                Core.EventBus.Publish(Core.EventTypes.AIRequest, aiReq);

                // update fov
                DungeonMap.UpdateFOVForMonsters(EntityManager);

                //add monster back in queue
                Components.Component ts = EntityManager.GetSingleComponentByID(nextEnt.UID, Core.ComponentTypes.Schedulable);
                if (ts != null)
                {
                    Components.SchedulableComp sc = (Components.SchedulableComp)ts;
                    int entTime = sc.Time;
                    ShedSystem.Add(nextEnt, entTime);
                }
            }
            _turn++;
            //MessageLog.Add($"Step: {++_turn}");
            //}

            // run systems update
            GarbageSystem.Update();

        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            // update render systems
            if ( _renderRequired )
            {
                _mapConsole.Clear();
                _statConsole.Clear();
                _messageConsole.Clear();

                DungeonMap.Draw(_mapConsole);
                RenderSystem.Update(_mapConsole, _statConsole);
                MessageLog.Draw(_messageConsole);

                //blit subconsoles
                RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight,
                    _rootConsole, 0, _inventoryHeight);
                RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight,
                    _rootConsole, _mapWidth, 0);
                RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight,
                    _rootConsole, 0, _screenHeight - _messageHeight);
                RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight,
                    _rootConsole, 0, 0);

                // tell RLNET to draw console
                _rootConsole.Draw();

                _renderRequired = false;
            }




        }
    }
}
