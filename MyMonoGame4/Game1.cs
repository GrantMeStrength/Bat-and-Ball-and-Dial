using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.UI.ViewManagement;
using Windows.Devices.HumanInterfaceDevice;
using Windows.UI.Input;
using Windows.UI;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Microsoft.Xna.Framework.Input;

namespace MyMonoGame4
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // Game constants, which you should experiment with.
        const int SCREENEDGEOFFSET = 200;
        const int SCREENTOPOFFSET = 200;
        const int PADDLEOFFSET = 64;
        const int BRICKHORIZONTALSPACING = 100;
        const int BRICKVERTICALSPACING = 50;
        const int NUMBEROFBRICKROWS = 10;

        // MonoGame specific objects
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Our game objects
        SpriteClass ball;
        SpriteClass bat;
        List<SpriteClass> wall;

        // Special Microsoft Dial control objects
        Windows.UI.Input.RadialController _dial;
        Windows.UI.Input.RadialControllerMenuItem item0;

        // Used to track screen size, as it will vary from device to device.
        float screenWidth;
        float screenHeight;

        public Game1()
        {
            // Set up the MonoGame display

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {


            base.Initialize();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            screenHeight = (float)ApplicationView.GetForCurrentView().VisibleBounds.Height;
            screenWidth = (float)ApplicationView.GetForCurrentView().VisibleBounds.Width;


            // Take high density screens into account, such as Surface Pro
            DisplayInformation d = DisplayInformation.GetForCurrentView();
            screenHeight *= (float)d.RawPixelsPerViewPixel;
            screenWidth *= (float)d.RawPixelsPerViewPixel;


            // Set up the Dial Contoller to report rotation events

            RadialControllerSystemMenuItemKind[] default_items = { RadialControllerSystemMenuItemKind.Volume };
            RadialControllerConfiguration.GetForCurrentView().SetDefaultMenuItems(new List<RadialControllerSystemMenuItemKind>(default_items));
            _dial = Windows.UI.Input.RadialController.CreateForCurrentView();
            _dial.RotationResolutionInDegrees = 2;
            _dial.UseAutomaticHapticFeedback = false;

            _dial.RotationChanged += _dial_RotationChanged;
            _dial.ButtonClicked += _dial_ButtonClicked;

            // Add support for this app to the Dial control
            item0 = Windows.UI.Input.RadialControllerMenuItem.CreateFromKnownIcon("Bat and Ball", Windows.UI.Input.RadialControllerMenuKnownIcon.InkColor);
            AddItem();

            // Hide the mouse
            this.IsMouseVisible = false;

            // Create the rows of bricks that make up the wall
            CreateBrickWall();

        }

        private void CreateBrickWall()
        {
            // Create the bricks that make up the wall
            // The exact number of bricks depends on the width of the screen, so we use a List<> structure,
            // which doesn't need to know in advance how big it needs to be.

            wall = new List<SpriteClass>();
   
            for (float y = SCREENTOPOFFSET; y < SCREENTOPOFFSET + (NUMBEROFBRICKROWS * BRICKVERTICALSPACING) ; y += BRICKVERTICALSPACING)
            {
                for (float x = SCREENEDGEOFFSET; x < (screenWidth - SCREENEDGEOFFSET); x += BRICKHORIZONTALSPACING)
                {
                    SpriteClass brick = new SpriteClass(GraphicsDevice, "content/block.png");
                    brick.isCircular = false;
                    brick.Y = y;
                    brick.X = x;
                    wall.Add(brick);
                }
            }
        }

        private async Task AddItem()
        {
            // Used when adding features to the Dial control
            _dial.Menu.Items.Add(item0);
            await Task.Delay(400);
            _dial.Menu.SelectMenuItem(this.item0);
        }

        private void _dial_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            // Start the ball moving when the Dial is clicked

            StartThingsMoving();
        }

        private void StartThingsMoving()
        {
            // Called at very start of game to give the ball and bat initial speed and location
            ball.X = screenWidth / 2;
            ball.Y = screenHeight / 2;
            ball.DX = 512;
            ball.DY = 512;

            bat.X = screenWidth / 2;
            bat.Y = screenHeight - PADDLEOFFSET;
        }

        private void _dial_RotationChanged(Windows.UI.Input.RadialController sender, Windows.UI.Input.RadialControllerRotationChangedEventArgs args)
        {
            // The user has rotated the dial, so move the bat accordingly.
            bat.X += (float)(args.RotationDeltaInDegrees * 16.0);
            bat.Y = screenHeight - PADDLEOFFSET;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ball = new SpriteClass(GraphicsDevice, "content/bounceyball.png");
            ball.isCircular = true;

            bat = new SpriteClass(GraphicsDevice, "content/bat.png");
            bat.isCircular = false;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Check for keys being pressed.

            KeyboardHandler();

            // Bounce ball

            ball.X += ball.DX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ball.Y += ball.DY * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ball.X < 0 || ball.X > screenWidth) ball.DX = -ball.DX;
            if (ball.Y < 0 || ball.Y > screenHeight) ball.DY = -ball.DY;

            // Check for the ball hitting the bat

          if (bat.Collision(ball))
            {
                ball.DY = -ball.DY;

                // If the ball hits a side of the bat, it can also reverse the DX value
                if (ball.DY < 0)
                {
                    if (ball.X > bat.X)
                    {
                        ball.DX = -ball.DX;
                    }
                }
                else
                {
                    if (ball.X < bat.X)
                    {
                        ball.DX = -ball.DX;
                    }
                }
            }

            // Check for hitting wall

            foreach (SpriteClass brick in wall)
            {       
                    if (brick.Collision(ball))
                    {
                        ball.DY = -ball.DY;
                        brick.Y = -100; // This value is only so we can remove it later.
                    }
            }

            // Now here comes bit of magic which goes through the entire collection of bricks
            // removing those with the Y co-ordinate is -100. As a result, they 
            // will no longer be drawn or tested for collisions.

            // You might wonder, why not just use Remove(brick) in the test above,
            // instead of assigning that -100 value. The reason is that you aren't
            // allowed to change a structure such as a list while you are in a loop that
            // is accessing each element of it. It would be like taking a ladder apart
            // while you are still climbing it: it wouldn't end well.

            wall.RemoveAll(delegate (SpriteClass brick)
            {
                return brick.Y == -100;
            });



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            // Our drawing code - all the sprites we want to draw are contained within the SpriteBatch begin/end

            spriteBatch.Begin();

            ball.Draw(spriteBatch);
            bat.Draw(spriteBatch);


            // Draw all the bricks, switching to a new color for each new line
            int color = 0;
            float oldY = 0;
            foreach (SpriteClass brick in wall)
            {
                if (brick.Y != oldY)
                {
                    color++;
                    if (color > 6) color = 1;
                    oldY = brick.Y;
                }
                brick.DrawColor(spriteBatch, color);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }


        void KeyboardHandler()
        {

            KeyboardState state = Keyboard.GetState();

            // Quit the game if ESC is pressed.
            if (state.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Move the paddle left and right
            if (state.IsKeyDown(Keys.Left))
            {
                bat.X -=16;
                bat.Y = screenHeight - PADDLEOFFSET;
            }

            if (state.IsKeyDown(Keys.Right))
            {
                bat.X += 16;
                bat.Y = screenHeight - PADDLEOFFSET;
            }

            // Start the game when Space bar is pressed
            if (state.IsKeyDown(Keys.Space))
            {
                StartThingsMoving();
            }
        }

    }

}
