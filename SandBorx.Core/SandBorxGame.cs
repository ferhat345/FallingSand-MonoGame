using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;

namespace SandBorx.Core
{
    public class SandBorxGame : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        private Texture2D simulationTexture;
        private Color[] colorData;

        private RenderTarget2D virtualScreen;
        private Rectangle virtualScreenDestination;
        private readonly Point virtualScreenSize = new Point(1280, 840);

        int displayWidth;
        int displayHeight;

        private Texture2D pixelTexture;
        private Texture2D sandIcon, wallIcon, waterIcon, woodIcon, fireIcon, steamIcon, lavaIcon, stoneIcon, smokeIcon, acidIcon, oilIcon, plantIcon, gunpowderIcon, virusIcon, spoutIcon, tntIcon, uraniumIcon, glassIcon, fuseIcon, iceIcon, nitrogenIcon;

        private Random random = new Random();
        private byte selectedElement = SAND;
        private int brushSize = 18;
        private int simulationStepsPerFrame = 2;


        const byte EMPTY = 0;
        const byte SAND = 1;
        const byte WALL = 2;
        const byte WATER = 3;
        const byte FIRE = 4;
        const byte WOOD = 5;
        const byte STEAM = 6;
        const byte LAVA = 7;
        const byte STONE = 8;
        const byte SMOKE = 9;
        const byte ACID = 10;
        const byte OIL = 11;
        const byte PLANT = 12;
        const byte GUNPOWDER = 13;
        const byte VIRUS = 14;
        const byte SPOUT = 15;
        const byte TNT = 16;
        const byte URANIUM = 17;
        const byte GLASS = 18;
        const byte FUSE = 19;
        const byte ICE = 20;
        const byte NITROGEN = 21;


        const int SIM_WIDTH = 1280;
        const int SIM_HEIGHT = 720;
        byte[,] grid = new byte[SIM_WIDTH, SIM_HEIGHT];
        byte[,] next_grid = new byte[SIM_WIDTH, SIM_HEIGHT];

        private HashSet<Point> activeSand, nextActiveSand;
        private HashSet<Point> activeWater, nextActiveWater;
        private HashSet<Point> activeFire, nextActiveFire;
        private HashSet<Point> activeSteam, nextActiveSteam;
        private HashSet<Point> activeLava, nextActiveLava;
        private HashSet<Point> activeSmoke, nextActiveSmoke;
        private HashSet<Point> activeAcid, nextActiveAcid;
        private HashSet<Point> activeOil, nextActiveOil;
        private HashSet<Point> activePlant, nextActivePlant;
        private HashSet<Point> activeGunpowder, nextActiveGunpowder;
        private HashSet<Point> activeVirus, nextActiveVirus;
        private HashSet<Point> activeSpout, nextActiveSpout;
        private HashSet<Point> activeTnt, nextActiveTnt;
        private HashSet<Point> activeUranium, nextActiveUranium;
        private HashSet<Point> activeFuse, nextActiveFuse;
        private HashSet<Point> activeIce, nextActiveIce;
        private HashSet<Point> activeNitrogen, nextActiveNitrogen;


        private Rectangle sandButton, wallButton, waterButton, fireButton, woodButton, steamButton, lavaButton, stoneButton, smokeButton, acidButton, oilButton, plantButton, gunpowderButton, virusButton, spoutButton, tntButton, uraniumButton, glassButton, fuseButton, iceButton, nitrogenButton;

        public SandBorxGame()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphicsDeviceManager.IsFullScreen = true;
            graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;


            activeSand = new HashSet<Point>(); nextActiveSand = new HashSet<Point>();
            activeWater = new HashSet<Point>(); nextActiveWater = new HashSet<Point>();
            activeFire = new HashSet<Point>(); nextActiveFire = new HashSet<Point>();
            activeSteam = new HashSet<Point>(); nextActiveSteam = new HashSet<Point>();
            activeLava = new HashSet<Point>(); nextActiveLava = new HashSet<Point>();
            activeSmoke = new HashSet<Point>(); nextActiveSmoke = new HashSet<Point>();
            activeAcid = new HashSet<Point>(); nextActiveAcid = new HashSet<Point>();
            activeOil = new HashSet<Point>(); nextActiveOil = new HashSet<Point>();
            activePlant = new HashSet<Point>(); nextActivePlant = new HashSet<Point>();
            activeGunpowder = new HashSet<Point>(); nextActiveGunpowder = new HashSet<Point>();
            activeVirus = new HashSet<Point>(); nextActiveVirus = new HashSet<Point>();
            activeSpout = new HashSet<Point>(); nextActiveSpout = new HashSet<Point>();
            activeTnt = new HashSet<Point>(); nextActiveTnt = new HashSet<Point>();
            activeUranium = new HashSet<Point>(); nextActiveUranium = new HashSet<Point>();
            activeFuse = new HashSet<Point>(); nextActiveFuse = new HashSet<Point>();
            activeIce = new HashSet<Point>(); nextActiveIce = new HashSet<Point>();
            activeNitrogen = new HashSet<Point>(); nextActiveNitrogen = new HashSet<Point>();
        }

        protected override void Initialize() 
        { base.Initialize();

            virtualScreen = new RenderTarget2D(GraphicsDevice, virtualScreenSize.X, virtualScreenSize.Y);

            float scale = Math.Min((float)GraphicsDevice.Viewport.Width / virtualScreenSize.X, (float)GraphicsDevice.Viewport.Height / virtualScreenSize.Y);
            int destWidth = (int)(virtualScreenSize.X * scale);
            int destHeight = (int)(virtualScreenSize.Y * scale);
            int destX = (GraphicsDevice.Viewport.Width - destWidth) / 2;
            int destY = (GraphicsDevice.Viewport.Height - destHeight) / 2;
            virtualScreenDestination = new Rectangle(destX, destY, destWidth, destHeight);

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            // OPTIMIZAS
            simulationTexture = new Texture2D(GraphicsDevice, SIM_WIDTH, SIM_HEIGHT);
            colorData = new Color[SIM_WIDTH * SIM_HEIGHT];
            
            sandIcon = Content.Load<Texture2D>("sand");
            wallIcon = Content.Load<Texture2D>("wall");
            waterIcon = Content.Load<Texture2D>("water");
            woodIcon = Content.Load<Texture2D>("wood");
            fireIcon = Content.Load<Texture2D>("fire");
            steamIcon = Content.Load<Texture2D>("steam");
            lavaIcon = Content.Load<Texture2D>("lava");
            stoneIcon = Content.Load<Texture2D>("stone");
            smokeIcon = Content.Load<Texture2D>("smoke");
            acidIcon = Content.Load<Texture2D>("acid");
            oilIcon = Content.Load<Texture2D>("oil");
            plantIcon = Content.Load<Texture2D>("plant");
            gunpowderIcon = Content.Load<Texture2D>("gunpowder");
            virusIcon = Content.Load<Texture2D>("virus");
            spoutIcon = Content.Load<Texture2D>("spout");
            tntIcon = Content.Load<Texture2D>("tnt");
            uraniumIcon = Content.Load<Texture2D>("uranium");
            glassIcon = Content.Load<Texture2D>("glass");
            fuseIcon = Content.Load<Texture2D>("fuse");
            iceIcon = Content.Load<Texture2D>("ice");
            nitrogenIcon = Content.Load<Texture2D>("nitrogen");
            
            // UI Butonlarını
            int uiY1 = SIM_HEIGHT + 10;
            int uiY2 = SIM_HEIGHT + 60;
            int xPos = 10;
            int buttonSize = 40;
            int buttonSpacing = 50;

            sandButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            wallButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            waterButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            woodButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            fireButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            steamButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            lavaButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            stoneButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            smokeButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            acidButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;
            oilButton = new Rectangle(xPos, uiY1, buttonSize, buttonSize); xPos += buttonSpacing;

            xPos = 10;
            plantButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            gunpowderButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            virusButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            spoutButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            tntButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            uraniumButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            glassButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            fuseButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            iceButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize); xPos += buttonSpacing;
            nitrogenButton = new Rectangle(xPos, uiY2, buttonSize, buttonSize);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                ProcessInputAt(ScreenToVirtual(mouseState.Position));
            }

            var touchState = TouchPanel.GetState();
            foreach (var touch in touchState)
            {
                if (touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved)
                {
                    ProcessInputAt(ScreenToVirtual(new Point((int)touch.Position.X, (int)touch.Position.Y)));
                }
            }

            for (int i = 0; i < simulationStepsPerFrame; i++)
            {
                StepSimulation();
            }

            base.Update(gameTime);
        }

        private void ProcessInputAt(Point position)
        {
            if (sandButton.Contains(position)) { selectedElement = SAND; return; }
            if (wallButton.Contains(position)) { selectedElement = WALL; return; }
            if (waterButton.Contains(position)) { selectedElement = WATER; return; }
            if (woodButton.Contains(position)) { selectedElement = WOOD; return; }
            if (fireButton.Contains(position)) { selectedElement = FIRE; return; }
            if (steamButton.Contains(position)) { selectedElement = STEAM; return; }
            if (lavaButton.Contains(position)) { selectedElement = LAVA; return; }
            if (stoneButton.Contains(position)) { selectedElement = STONE; return; }
            if (smokeButton.Contains(position)) { selectedElement = SMOKE; return; }
            if (acidButton.Contains(position)) { selectedElement = ACID; return; }
            if (oilButton.Contains(position)) { selectedElement = OIL; return; }
            if (plantButton.Contains(position)) { selectedElement = PLANT; return; }
            if (gunpowderButton.Contains(position)) { selectedElement = GUNPOWDER; return; }
            if (virusButton.Contains(position)) { selectedElement = VIRUS; return; }
            if (spoutButton.Contains(position)) { selectedElement = SPOUT; return; }
            if (tntButton.Contains(position)) { selectedElement = TNT; return; }
            if (uraniumButton.Contains(position)) { selectedElement = URANIUM; return; }
            if (glassButton.Contains(position)) { selectedElement = GLASS; return; }
            if (fuseButton.Contains(position)) { selectedElement = FUSE; return; }
            if (iceButton.Contains(position)) { selectedElement = ICE; return; }
            if (nitrogenButton.Contains(position)) { selectedElement = NITROGEN; return; }

            int startX = position.X - brushSize / 2;
            int startY = position.Y - brushSize / 2;
            for (int bY = 0; bY < brushSize; bY++)
            {
                for (int bX = 0; bX < brushSize; bX++)
                {
                    int drawX = startX + bX;
                    int drawY = startY + bY;
                    if (drawX >= 0 && drawX < SIM_WIDTH && drawY >= 0 && drawY < SIM_HEIGHT && grid[drawX, drawY] == EMPTY)
                    {
                        grid[drawX, drawY] = selectedElement;
                        var p = new Point(drawX, drawY);
                        switch (selectedElement)
                        {
                            case SAND: activeSand.Add(p); break;
                            case WATER: activeWater.Add(p); break;
                            case FIRE: activeFire.Add(p); break;
                            case STEAM: activeSteam.Add(p); break;
                            case LAVA: activeLava.Add(p); break;
                            case SMOKE: activeSmoke.Add(p); break;
                            case ACID: activeAcid.Add(p); break;
                            case OIL: activeOil.Add(p); break;
                            case PLANT: activePlant.Add(p); break;
                            case GUNPOWDER: activeGunpowder.Add(p); break;
                            case VIRUS: activeVirus.Add(p); break;
                            case SPOUT: activeSpout.Add(p); break;
                            case TNT: activeTnt.Add(p); break;
                            case URANIUM: activeUranium.Add(p); break;
                            case FUSE: activeFuse.Add(p); break;
                            case ICE: activeIce.Add(p); break;
                            case NITROGEN: activeNitrogen.Add(p); break;
                        }
                    }
                }
            }
        }

        private void StepSimulation()
        {
            Array.Copy(grid, next_grid, grid.Length);
            
            ClearNextActiveLists();

            ProcessReactiveParticles();
            ProcessHeavyParticles();
            ProcessLightParticles();

            SwapActiveLists();
            
            Array.Copy(next_grid, grid, grid.Length);
        }

        private void ProcessReactiveParticles()
        {
            foreach (Point p in activeSpout) ProcessParticle(p);
            foreach (Point p in activeVirus) ProcessParticle(p);
            foreach (Point p in activeUranium) ProcessParticle(p);
            foreach (Point p in activeIce) ProcessParticle(p);
        }

        private void ProcessHeavyParticles()
        {
            foreach (Point p in activeSand) ProcessParticle(p);
            foreach (Point p in activeWater) ProcessParticle(p);
            foreach (Point p in activeFire) ProcessParticle(p);
            foreach (Point p in activeLava) ProcessParticle(p);
            foreach (Point p in activeAcid) ProcessParticle(p);
            foreach (Point p in activeOil) ProcessParticle(p);
            foreach (Point p in activeGunpowder) ProcessParticle(p);
            foreach (Point p in activeTnt) ProcessParticle(p);
            foreach (Point p in activeFuse) ProcessParticle(p);
            foreach (Point p in activeNitrogen) ProcessParticle(p);
            foreach (Point p in activePlant) ProcessParticle(p);
        }

        private void ProcessLightParticles()
        {
            foreach (Point p in activeSteam) ProcessParticle(p);
            foreach (Point p in activeSmoke) ProcessParticle(p);
        }

        private void ProcessParticle(Point p)
        {
            byte type = grid[p.X, p.Y];
            if (next_grid[p.X, p.Y] != type) return;
            bool moved = false;

            switch (type)
            {
                 case SPOUT:
                    if (p.Y < SIM_HEIGHT - 1 && grid[p.X, p.Y + 1] == EMPTY && random.Next(5) == 0)
                    {
                        next_grid[p.X, p.Y + 1] = WATER;
                        nextActiveWater.Add(new Point(p.X, p.Y + 1));
                    }
                    nextActiveSpout.Add(p);
                    break;
                case VIRUS:
                    int dirX = random.Next(-1, 2); int dirY = random.Next(-1, 2);
                    if (dirX != 0 || dirY != 0)
                    {
                        int infectX = p.X + dirX; int infectY = p.Y + dirY;
                        if (infectX >= 0 && infectX < SIM_WIDTH && infectY >= 0 && infectY < SIM_HEIGHT)
                        {
                            byte target = grid[infectX, infectY];
                            if ((target == PLANT || target == WOOD || target == WATER) && random.Next(15) == 0)
                            {
                                next_grid[infectX, infectY] = VIRUS; nextActiveVirus.Add(new Point(infectX, infectY));
                            }
                        }
                    }
                    if (random.Next(500) == 0) { next_grid[p.X, p.Y] = EMPTY; } else { nextActiveVirus.Add(p); }
                    break;
                case URANIUM:
                    for (int dx = -1; dx <= 1; dx++) for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = p.X + dx; int ny = p.Y + dy;
                        if (nx >= 0 && nx < SIM_WIDTH && ny >= 0 && ny < SIM_HEIGHT)
                        {
                            if (grid[nx, ny] == WATER && random.Next(100) == 0) { next_grid[nx, ny] = ACID; nextActiveAcid.Add(new Point(nx, ny)); }
                            if (grid[nx, ny] == PLANT && random.Next(150) == 0) { next_grid[nx, ny] = SAND; nextActiveSand.Add(new Point(nx, ny)); }
                        }
                    }
                    nextActiveUranium.Add(p);
                    break;
                case ICE:
                    bool melted = false;
                    for (int dx = -1; dx <= 1; dx++) for (int dy = -1; dy <= 1; dy++)
                    {
                        if (melted) break;
                        int nx = p.X + dx; int ny = p.Y + dy;
                        if (nx >= 0 && nx < SIM_WIDTH && ny >= 0 && ny < SIM_HEIGHT)
                        {
                            byte neighbor = grid[nx, ny];
                            if ((neighbor == FIRE || neighbor == LAVA) && random.Next(5) == 0)
                            {
                                next_grid[p.X, p.Y] = WATER; nextActiveWater.Add(p); melted = true;
                            }
                        }
                    }
                    if (!melted)
                    {
                        for (int dx = -1; dx <= 1; dx++) for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = p.X + dx; int ny = p.Y + dy;
                            if (nx >= 0 && nx < SIM_WIDTH && ny >= 0 && ny < SIM_HEIGHT && grid[nx, ny] == WATER && random.Next(50) == 0)
                            {
                                next_grid[nx, ny] = ICE; nextActiveIce.Add(new Point(nx, ny));
                            }
                        }
                        nextActiveIce.Add(p);
                    }
                    break;
                case SAND:
                case GUNPOWDER:
                case FUSE:
                    if (p.Y < SIM_HEIGHT - 1)
                    {
                        byte below = grid[p.X, p.Y + 1];
                        if (below == EMPTY || below == WATER || below == OIL || below == NITROGEN)
                        {
                            next_grid[p.X, p.Y] = below;
                            if (below == WATER) nextActiveWater.Add(new Point(p.X, p.Y)); else if (below == OIL) nextActiveOil.Add(new Point(p.X, p.Y)); else if (below == NITROGEN) nextActiveNitrogen.Add(new Point(p.X, p.Y));
                            next_grid[p.X, p.Y + 1] = type;
                            if (type == SAND) nextActiveSand.Add(new Point(p.X, p.Y + 1)); else if (type == GUNPOWDER) nextActiveGunpowder.Add(new Point(p.X, p.Y + 1)); else if (type == FUSE) nextActiveFuse.Add(new Point(p.X, p.Y + 1));
                            moved = true;
                        }
                        else
                        {
                            int dir = (random.Next(2) == 0) ? -1 : 1;
                            Action<int> tryMoveDiag = (d) => {
                                if (!moved && p.X + d >= 0 && p.X + d < SIM_WIDTH)
                                {
                                    byte diag = grid[p.X + d, p.Y + 1];
                                    if (diag == EMPTY || diag == WATER || diag == OIL || diag == NITROGEN)
                                    {
                                        next_grid[p.X, p.Y] = diag;
                                        if (diag == WATER) nextActiveWater.Add(new Point(p.X, p.Y)); else if (diag == OIL) nextActiveOil.Add(new Point(p.X, p.Y)); else if (diag == NITROGEN) nextActiveNitrogen.Add(new Point(p.X, p.Y));
                                        next_grid[p.X + d, p.Y + 1] = type;
                                        if (type == SAND) nextActiveSand.Add(new Point(p.X + d, p.Y + 1)); else if (type == GUNPOWDER) nextActiveGunpowder.Add(new Point(p.X + d, p.Y + 1)); else if (type == FUSE) nextActiveFuse.Add(new Point(p.X + d, p.Y + 1));
                                        moved = true;
                                    }
                                }
                            };
                            tryMoveDiag(dir);
                            tryMoveDiag(-dir);
                        }
                    }
                    if (!moved) { if (type == SAND) nextActiveSand.Add(p); else if (type == GUNPOWDER) nextActiveGunpowder.Add(p); else if (type == FUSE) nextActiveFuse.Add(p); }
                    break;
                case WATER:
                    int waterDir = (random.Next(2) == 0) ? -1 : 1;
                    if (p.Y < SIM_HEIGHT - 1 && grid[p.X, p.Y + 1] == EMPTY && next_grid[p.X, p.Y + 1] == EMPTY)
                    {
                        next_grid[p.X, p.Y] = EMPTY; next_grid[p.X, p.Y + 1] = WATER; nextActiveWater.Add(new Point(p.X, p.Y + 1)); moved = true;
                    }
                    else if (p.Y < SIM_HEIGHT - 1)
                    {
                        bool movedDiag = false;
                        int checkX1 = p.X + waterDir;
                        if (checkX1 >= 0 && checkX1 < SIM_WIDTH && grid[checkX1, p.Y + 1] == EMPTY && next_grid[checkX1, p.Y + 1] == EMPTY)
                        {
                            next_grid[p.X, p.Y] = EMPTY; next_grid[checkX1, p.Y + 1] = WATER; nextActiveWater.Add(new Point(checkX1, p.Y + 1)); movedDiag = true;
                        }
                        else
                        {
                            int checkX2 = p.X - waterDir;
                            if (checkX2 >= 0 && checkX2 < SIM_WIDTH && grid[checkX2, p.Y + 1] == EMPTY && next_grid[checkX2, p.Y + 1] == EMPTY)
                            {
                                next_grid[p.X, p.Y] = EMPTY; next_grid[checkX2, p.Y + 1] = WATER; nextActiveWater.Add(new Point(checkX2, p.Y + 1)); movedDiag = true;
                            }
                        }
                        if (movedDiag) moved = true;
                    }
                    if (!moved && random.Next(4) == 0)
                    {
                        bool movedSide = false;
                        int checkX1 = p.X + waterDir;
                        if (checkX1 >= 0 && checkX1 < SIM_WIDTH && grid[checkX1, p.Y] == EMPTY && next_grid[checkX1, p.Y] == EMPTY)
                        {
                            next_grid[p.X, p.Y] = EMPTY; next_grid[checkX1, p.Y] = WATER; nextActiveWater.Add(new Point(checkX1, p.Y)); movedSide = true;
                        }
                        else
                        {
                            int checkX2 = p.X - waterDir;
                            if (checkX2 >= 0 && checkX2 < SIM_WIDTH && grid[checkX2, p.Y] == EMPTY && next_grid[checkX2, p.Y] == EMPTY)
                            {
                                next_grid[p.X, p.Y] = EMPTY; next_grid[checkX2, p.Y] = WATER; nextActiveWater.Add(new Point(checkX2, p.Y)); movedSide = true;
                            }
                        }
                        if (movedSide) moved = true;
                    }
                    if (!moved) { nextActiveWater.Add(p); }
                    break;
                case ACID:
                case OIL:
                case NITROGEN:
                    if (type == ACID)
                    {
                        for (int dx = -1; dx <= 1; dx++) for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = p.X + dx; int ny = p.Y + dy;
                            if (nx >= 0 && nx < SIM_WIDTH && ny >= 0 && ny < SIM_HEIGHT)
                            {
                                byte target = grid[nx, ny];
                                if (target != EMPTY && target != WALL && target != GLASS && target != ACID && target != SPOUT && random.Next(10) == 0) { next_grid[nx, ny] = EMPTY; }
                            }
                        }
                        if (random.Next(200) == 0) { next_grid[p.X, p.Y] = EMPTY; break; }
                    }
                    else if (type == NITROGEN)
                    {
                        for (int dx = -1; dx <= 1; dx++) for (int dy = -1; dy <= 1; dy++)
                        {
                            int nx = p.X + dx; int ny = p.Y + dy;
                            if (nx >= 0 && nx < SIM_WIDTH && ny >= 0 && ny < SIM_HEIGHT)
                            {
                                byte target = grid[nx, ny];
                                if (target == WATER && random.Next(5) == 0) { next_grid[nx, ny] = ICE; nextActiveIce.Add(new Point(nx, ny)); }
                                else if (target == LAVA && random.Next(3) == 0) { next_grid[nx, ny] = STONE; }
                            }
                        }
                        if (random.Next(30) == 0) { next_grid[p.X, p.Y] = EMPTY; break; }
                    }
                    int fluidDir = (random.Next(2) == 0) ? -1 : 1;
                    Action<Point> setActive = (pt) => {
                        if (type == ACID) nextActiveAcid.Add(pt); else if (type == OIL) nextActiveOil.Add(pt); else if (type == NITROGEN) nextActiveNitrogen.Add(pt);
                    };
                    if (p.Y < SIM_HEIGHT - 1)
                    {
                        byte below = grid[p.X, p.Y + 1];
                        if (below == EMPTY && next_grid[p.X, p.Y + 1] == EMPTY) { next_grid[p.X, p.Y] = EMPTY; next_grid[p.X, p.Y + 1] = type; setActive(new Point(p.X, p.Y + 1)); moved = true; }
                        else if (type == OIL && below == WATER) { next_grid[p.X, p.Y] = WATER; nextActiveWater.Add(p); next_grid[p.X, p.Y + 1] = OIL; nextActiveOil.Add(new Point(p.X, p.Y + 1)); moved = true; }
                    }
                    if (!moved && p.Y < SIM_HEIGHT - 1)
                    {
                        Action<int> tryMoveDiag = (d) => {
                            if (!moved && p.X + d >= 0 && p.X + d < SIM_WIDTH && grid[p.X + d, p.Y + 1] == EMPTY && next_grid[p.X + d, p.Y + 1] == EMPTY)
                            {
                                next_grid[p.X, p.Y] = EMPTY; next_grid[p.X + d, p.Y + 1] = type; setActive(new Point(p.X + d, p.Y + 1)); moved = true;
                            }
                        };
                        tryMoveDiag(fluidDir);
                        tryMoveDiag(-fluidDir);
                    }
                    if (!moved && random.Next(type == OIL ? 10 : 4) == 0)
                    {
                        Action<int> tryMoveSide = (d) => {
                            if (!moved && p.X + d >= 0 && p.X + d < SIM_WIDTH && grid[p.X + d, p.Y] == EMPTY && next_grid[p.X + d, p.Y] == EMPTY)
                            {
                                next_grid[p.X, p.Y] = EMPTY; next_grid[p.X + d, p.Y] = type; setActive(new Point(p.X + d, p.Y)); moved = true;
                            }
                        };
                        tryMoveSide(fluidDir);
                        tryMoveSide(-fluidDir);
                    }
                    if (!moved) setActive(p);
                    break;
                case PLANT:
                    int plantDir = (random.Next(2) == 0) ? -1 : 1;
                    if (p.Y < SIM_HEIGHT - 1 && grid[p.X, p.Y + 1] == EMPTY && next_grid[p.X, p.Y + 1] == EMPTY)
                    {
                        next_grid[p.X, p.Y] = EMPTY; next_grid[p.X, p.Y + 1] = PLANT; nextActivePlant.Add(new Point(p.X, p.Y + 1)); moved = true;
                    }
                    else if (p.Y < SIM_HEIGHT - 1)
                    {
                        bool movedDiag = false;
                        int checkX1 = p.X + plantDir;
                        if (checkX1 >= 0 && checkX1 < SIM_WIDTH && grid[checkX1, p.Y + 1] == EMPTY && next_grid[checkX1, p.Y + 1] == EMPTY)
                        {
                            next_grid[p.X, p.Y] = EMPTY; next_grid[checkX1, p.Y + 1] = PLANT; nextActivePlant.Add(new Point(checkX1, p.Y + 1)); movedDiag = true;
                        }
                        else
                        {
                            int checkX2 = p.X - plantDir;
                            if (checkX2 >= 0 && checkX2 < SIM_WIDTH && grid[checkX2, p.Y + 1] == EMPTY && next_grid[checkX2, p.Y + 1] == EMPTY)
                            {
                                next_grid[p.X, p.Y] = EMPTY; next_grid[checkX2, p.Y + 1] = PLANT; nextActivePlant.Add(new Point(checkX2, p.Y + 1)); movedDiag = true;
                            }
                        }
                        if (movedDiag) moved = true;
                    }
                    if (!moved && random.Next(2) == 0)
                    {
                        bool movedSide = false;
                        int checkX1 = p.X + plantDir;
                        if (checkX1 >= 0 && checkX1 < SIM_WIDTH && grid[checkX1, p.Y] == EMPTY && next_grid[checkX1, p.Y] == EMPTY)
                        {
                            next_grid[p.X, p.Y] = EMPTY; next_grid[checkX1, p.Y] = PLANT; nextActivePlant.Add(new Point(checkX1, p.Y)); movedSide = true;
                        }
                        else
                        {
                            int checkX2 = p.X - plantDir;
                            if (checkX2 >= 0 && checkX2 < SIM_WIDTH && grid[checkX2, p.Y] == EMPTY && next_grid[checkX2, p.Y] == EMPTY)
                            {
                                next_grid[p.X, p.Y] = EMPTY; next_grid[checkX2, p.Y] = PLANT; nextActivePlant.Add(new Point(checkX2, p.Y)); movedSide = true;
                            }
                        }
                        if (movedSide) moved = true;
                    }
                    if (!moved)
                    {
                        bool grew = false;
                        for (int dx = -2; dx <= 2; dx++) for (int dy = -2; dy <= 2; dy++)
                        {
                            if (grew) break;
                            if (dx == 0 && dy == 0) continue;
                            int nx = p.X + dx; int ny = p.Y + dy;
                            if (nx >= 0 && nx < SIM_WIDTH && ny >= 0 && ny < SIM_HEIGHT)
                            {
                                if (grid[nx, ny] == WATER && random.Next(3) == 0)
                                {
                                    for (int gx = -1; gx <= 1; gx++) for (int gy = -1; gy <= 1; gy++)
                                    {
                                        if (grew) break;
                                        int growX = p.X + gx; int growY = p.Y + gy;
                                        if (growX >= 0 && growX < SIM_WIDTH && growY >= 0 && growY < SIM_HEIGHT && grid[growX, growY] == EMPTY)
                                        {
                                            next_grid[growX, growY] = PLANT; nextActivePlant.Add(new Point(growX, growY));
                                            next_grid[nx, ny] = EMPTY;
                                            grew = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!moved) { nextActivePlant.Add(p); }
                    break;
                case FIRE:
                    Action<int, int> tryBurn = (burnX, burnY) => {
                        if (burnX >= 0 && burnX < SIM_WIDTH && burnY >= 0 && burnY < SIM_HEIGHT)
                        {
                            byte target = grid[burnX, burnY];
                            if ((target == WOOD || target == PLANT || target == OIL || target == GUNPOWDER || target == FUSE || target == TNT) && random.Next(4) == 0)
                            {
                                next_grid[burnX, burnY] = FIRE; nextActiveFire.Add(new Point(burnX, burnY));
                            }
                            else if (target == WATER && random.Next(5) == 0)
                            {
                                next_grid[burnX, burnY] = STEAM; nextActiveSteam.Add(new Point(burnX, burnY));
                            }
                        }
                    };
                    tryBurn(p.X - 1, p.Y); tryBurn(p.X + 1, p.Y); tryBurn(p.X, p.Y - 1); tryBurn(p.X, p.Y + 1);
                    if (p.Y > 0 && grid[p.X, p.Y - 1] == EMPTY && random.Next(10) == 0) { next_grid[p.X, p.Y - 1] = SMOKE; nextActiveSmoke.Add(new Point(p.X, p.Y - 1)); }
                    if (random.Next(50) == 0) { next_grid[p.X, p.Y] = EMPTY; } else { nextActiveFire.Add(p); }
                    break;
                case LAVA:
                    bool transformed = false;
                    for (int dx = -1; dx <= 1; dx++) for (int dy = -1; dy <= 1; dy++)
                    {
                        if (transformed) break;
                        int nx = p.X + dx; int ny = p.Y + dy;
                        if (nx >= 0 && nx < SIM_WIDTH && ny >= 0 && ny < SIM_HEIGHT)
                        {
                            byte neighbor = grid[nx, ny];
                            if (neighbor == WATER || neighbor == NITROGEN) { next_grid[p.X, p.Y] = STONE; next_grid[nx, ny] = STEAM; nextActiveSteam.Add(new Point(nx, ny)); transformed = true; }
                            else if (neighbor == GLASS) { next_grid[nx, ny] = LAVA; nextActiveLava.Add(new Point(nx, ny)); }
                        }
                    }
                    if (transformed) break;
                    int lavaDir = (random.Next(2) == 0) ? -1 : 1;
                    if (p.Y < SIM_HEIGHT - 1 && grid[p.X, p.Y + 1] == EMPTY) { next_grid[p.X, p.Y] = EMPTY; next_grid[p.X, p.Y + 1] = LAVA; nextActiveLava.Add(new Point(p.X, p.Y + 1)); moved = true; }
                    else if (p.Y < SIM_HEIGHT - 1) { if (p.X + lavaDir >= 0 && p.X + lavaDir < SIM_WIDTH && grid[p.X + lavaDir, p.Y + 1] == EMPTY) { next_grid[p.X, p.Y] = EMPTY; next_grid[p.X + lavaDir, p.Y + 1] = LAVA; nextActiveLava.Add(new Point(p.X + lavaDir, p.Y + 1)); moved = true; } else if (p.X - lavaDir >= 0 && p.X - lavaDir < SIM_WIDTH && grid[p.X - lavaDir, p.Y + 1] == EMPTY) { next_grid[p.X, p.Y] = EMPTY; next_grid[p.X - lavaDir, p.Y + 1] = LAVA; nextActiveLava.Add(new Point(p.X - lavaDir, p.Y + 1)); moved = true; } }
                    if (!moved && random.Next(15) == 0) { if (p.X + lavaDir >= 0 && p.X + lavaDir < SIM_WIDTH && grid[p.X + lavaDir, p.Y] == EMPTY) { next_grid[p.X, p.Y] = EMPTY; next_grid[p.X + lavaDir, p.Y] = LAVA; nextActiveLava.Add(new Point(p.X + lavaDir, p.Y)); moved = true; } }
                    if (!moved && random.Next(500) == 0) { next_grid[p.X, p.Y] = STONE; } else if (!moved) { nextActiveLava.Add(p); }
                    break;
                case TNT:
                    bool exploded = false;
                    for (int dx = -1; dx <= 1; dx++) for (int dy = -1; dy <= 1; dy++)
                    {
                        if (exploded) break;
                        int nx = p.X + dx; int ny = p.Y + dy;
                        if (nx >= 0 && nx < SIM_WIDTH && ny >= 0 && ny < SIM_HEIGHT && (grid[nx, ny] == FIRE || grid[nx, ny] == LAVA)) { exploded = true; }
                    }
                    if (exploded)
                    {
                        next_grid[p.X, p.Y] = EMPTY;
                        int radius = 5;
                        for (int ex = -radius; ex <= radius; ex++) for (int ey = -radius; ey <= radius; ey++)
                        {
                            if (ex * ex + ey * ey <= radius * radius)
                            {
                                int blastX = p.X + ex; int blastY = p.Y + ey;
                                if (blastX >= 0 && blastX < SIM_WIDTH && blastY >= 0 && blastY < SIM_HEIGHT && grid[blastX, blastY] != WALL)
                                {
                                    if (random.Next(3) > 0) { next_grid[blastX, blastY] = FIRE; nextActiveFire.Add(new Point(blastX, blastY)); }
                                    else { next_grid[blastX, blastY] = SMOKE; nextActiveSmoke.Add(new Point(blastX, blastY)); }
                                }
                            }
                        }
                        break;
                    }
                    if (p.Y < SIM_HEIGHT - 1 && grid[p.X, p.Y + 1] == EMPTY) { next_grid[p.X, p.Y] = EMPTY; next_grid[p.X, p.Y + 1] = TNT; nextActiveTnt.Add(new Point(p.X, p.Y + 1)); }
                    else { nextActiveTnt.Add(p); }
                    break;
                case STEAM:
                case SMOKE:
                    if (p.Y > 0)
                    {
                        if (type == STEAM) { if (random.Next(100) == 0) { next_grid[p.X, p.Y] = WATER; nextActiveWater.Add(p); moved = true; } }
                        else if (type == SMOKE) { if (random.Next(200) == 0) { next_grid[p.X, p.Y] = EMPTY; moved = true; } }
                        if (!moved)
                        {
                            int dir = random.Next(-1, 2);
                            int newX = p.X + dir;
                            int newY = p.Y - 1;
                            if (newX >= 0 && newX < SIM_WIDTH && grid[newX, newY] == EMPTY && next_grid[newX, newY] == EMPTY)
                            {
                                next_grid[p.X, p.Y] = EMPTY;
                                next_grid[newX, newY] = type;
                                if (type == STEAM) nextActiveSteam.Add(new Point(newX, newY)); else nextActiveSmoke.Add(new Point(newX, newY));
                                moved = true;
                            }
                        }
                    }
                    if (!moved) { if (type == STEAM) nextActiveSteam.Add(p); else nextActiveSmoke.Add(p); }
                    break;
            }
        }
        
        private void ClearNextActiveLists()
        {
            nextActiveSand.Clear(); nextActiveWater.Clear(); nextActiveFire.Clear();
            nextActiveSteam.Clear(); nextActiveLava.Clear(); nextActiveSmoke.Clear();
            nextActiveAcid.Clear(); nextActiveOil.Clear(); nextActivePlant.Clear();
            nextActiveGunpowder.Clear(); nextActiveVirus.Clear(); nextActiveSpout.Clear();
            nextActiveTnt.Clear(); nextActiveUranium.Clear(); nextActiveFuse.Clear();
            nextActiveIce.Clear(); nextActiveNitrogen.Clear();
        }

        private void Swap<T>(ref T a, ref T b)
        {
            T temp = a; a = b; b = temp;
        }

        private void SwapActiveLists()
        {
            Swap(ref activeSand, ref nextActiveSand); Swap(ref activeWater, ref nextActiveWater);
            Swap(ref activeFire, ref nextActiveFire); Swap(ref activeSteam, ref nextActiveSteam);
            Swap(ref activeLava, ref nextActiveLava); Swap(ref activeSmoke, ref nextActiveSmoke);
            Swap(ref activeAcid, ref nextActiveAcid); Swap(ref activeOil, ref nextActiveOil);
            Swap(ref activePlant, ref nextActivePlant); Swap(ref activeGunpowder, ref nextActiveGunpowder);
            Swap(ref activeVirus, ref nextActiveVirus); Swap(ref activeSpout, ref nextActiveSpout);
            Swap(ref activeTnt, ref nextActiveTnt); Swap(ref activeUranium, ref nextActiveUranium);
            Swap(ref activeFuse, ref nextActiveFuse); Swap(ref activeIce, ref nextActiveIce);
            Swap(ref activeNitrogen, ref nextActiveNitrogen);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_spriteBatch == null) return;

            GraphicsDevice.SetRenderTarget(virtualScreen);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            UpdateTextureFromGrid();
            simulationTexture.SetData(colorData);
            _spriteBatch.Draw(simulationTexture, new Rectangle(0, 0, SIM_WIDTH, SIM_HEIGHT), Color.White);

            DrawUI();

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(virtualScreen, virtualScreenDestination, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private Point ScreenToVirtual(Point screenPosition)
        {
            float scaleX = (float)virtualScreenSize.X / virtualScreenDestination.Width;
            float scaleY = (float)virtualScreenSize.Y / virtualScreenDestination.Height;
            int virtualX = (int)((screenPosition.X - virtualScreenDestination.X) * scaleX);
            int virtualY = (int)((screenPosition.Y - virtualScreenDestination.Y) * scaleY);
            return new Point(virtualX, virtualY);
        }

        private void UpdateTextureFromGrid()
        {
            for (int y = 0; y < SIM_HEIGHT; y++)
            {
                for (int x = 0; x < SIM_WIDTH; x++)
                {
                    byte type = grid[x, y];
                    int index = y * SIM_WIDTH + x; 
                    Color color = Color.Black;
                    
                    switch (type)
                    {
                        case EMPTY: color = Color.Black; break;
                        case SAND: color = Color.Yellow; break;
                        case WALL: color = Color.Gray; break;
                        case WATER: color = Color.Blue; break;
                        case FIRE: color = (random.Next(2) == 0) ? Color.OrangeRed : Color.Yellow; break;
                        case WOOD: color = Color.SaddleBrown; break;
                        case STEAM: color = Color.LightGray; break;
                        case LAVA: color = (random.Next(2) == 0) ? Color.Orange : Color.DarkRed; break;
                        case STONE: color = Color.DimGray; break;
                        case SMOKE: color = Color.DarkSlateGray; break;
                        case ACID: color = Color.LawnGreen; break;
                        case OIL: color = new Color(20, 20, 20); break;
                        case PLANT: color = Color.Green; break;
                        case GUNPOWDER: color = Color.DarkGray; break;
                        case VIRUS: color = (random.Next(2) == 0) ? Color.MediumPurple : Color.Magenta; break;
                        case SPOUT: color = Color.CadetBlue; break;
                        case TNT: color = Color.Red; break;
                        case URANIUM: color = Color.GreenYellow; break;
                        case GLASS: color = Color.LightBlue; break;
                        case FUSE: color = Color.DarkGoldenrod; break;
                        case ICE: color = Color.AliceBlue; break;
                        case NITROGEN: color = Color.LightCyan; break;
                    }
                    colorData[index] = color;
                }
            }
        }
        
        private void DrawUI()
        {
            Rectangle uiBackground = new Rectangle(0, SIM_HEIGHT, SIM_WIDTH, 120);
            _spriteBatch.Draw(pixelTexture, uiBackground, Color.DarkSlateGray);

            _spriteBatch.Draw(pixelTexture, sandButton, selectedElement == SAND ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, wallButton, selectedElement == WALL ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, waterButton, selectedElement == WATER ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, woodButton, selectedElement == WOOD ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, fireButton, selectedElement == FIRE ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, steamButton, selectedElement == STEAM ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, lavaButton, selectedElement == LAVA ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, stoneButton, selectedElement == STONE ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, smokeButton, selectedElement == SMOKE ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, acidButton, selectedElement == ACID ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, oilButton, selectedElement == OIL ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, plantButton, selectedElement == PLANT ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, gunpowderButton, selectedElement == GUNPOWDER ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, virusButton, selectedElement == VIRUS ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, spoutButton, selectedElement == SPOUT ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, tntButton, selectedElement == TNT ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, uraniumButton, selectedElement == URANIUM ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, glassButton, selectedElement == GLASS ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, fuseButton, selectedElement == FUSE ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, iceButton, selectedElement == ICE ? Color.DarkOrange : Color.SlateGray);
            _spriteBatch.Draw(pixelTexture, nitrogenButton, selectedElement == NITROGEN ? Color.DarkOrange : Color.SlateGray);

            _spriteBatch.Draw(sandIcon, sandButton, Color.White);
            _spriteBatch.Draw(wallIcon, wallButton, Color.White);
            _spriteBatch.Draw(waterIcon, waterButton, Color.White);
            _spriteBatch.Draw(woodIcon, woodButton, Color.White);
            _spriteBatch.Draw(fireIcon, fireButton, Color.White);
            _spriteBatch.Draw(steamIcon, steamButton, Color.White);
            _spriteBatch.Draw(lavaIcon, lavaButton, Color.White);
            _spriteBatch.Draw(stoneIcon, stoneButton, Color.White);
            _spriteBatch.Draw(smokeIcon, smokeButton, Color.White);
            _spriteBatch.Draw(acidIcon, acidButton, Color.White);
            _spriteBatch.Draw(oilIcon, oilButton, Color.White);
            _spriteBatch.Draw(plantIcon, plantButton, Color.White);
            _spriteBatch.Draw(gunpowderIcon, gunpowderButton, Color.White);
            _spriteBatch.Draw(virusIcon, virusButton, Color.White);
            _spriteBatch.Draw(spoutIcon, spoutButton, Color.White);
            _spriteBatch.Draw(tntIcon, tntButton, Color.White);
            _spriteBatch.Draw(uraniumIcon, uraniumButton, Color.White);
            _spriteBatch.Draw(glassIcon, glassButton, Color.White);
            _spriteBatch.Draw(fuseIcon, fuseButton, Color.White);
            _spriteBatch.Draw(iceIcon, iceButton, Color.White);
            _spriteBatch.Draw(nitrogenIcon, nitrogenButton, Color.White);
        }
    }
}