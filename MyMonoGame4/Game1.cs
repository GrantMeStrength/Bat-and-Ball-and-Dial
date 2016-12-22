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

namespace MyMonoGame4
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteClass ball;
        SpriteClass bat;
        SpriteClass[] wall;

        Windows.UI.Input.RadialController _dial;
        Windows.UI.Input.RadialControllerMenuItem item0;

        float screenWidth;
        float screenHeight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
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

            item0 = Windows.UI.Input.RadialControllerMenuItem.CreateFromKnownIcon("Bat and Ball", Windows.UI.Input.RadialControllerMenuKnownIcon.InkColor);

            AddItem();

           
        }

        private async Task AddItem()
        {
            _dial.Menu.Items.Add(item0);
            await Task.Delay(400);
            _dial.Menu.SelectMenuItem(this.item0);
          

        }
        private void _dial_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            // Start the ball moving when the Dial is clicked

            ball.X = screenWidth / 2;
            ball.Y = screenHeight / 2;
            ball.DX = 512;
            ball.DY = 512;

            bat.X = screenWidth / 2;
            bat.Y = screenHeight - 256;
        }

        private void _dial_RotationChanged(Windows.UI.Input.RadialController sender, Windows.UI.Input.RadialControllerRotationChangedEventArgs args)
        {
            // The user has rotated the dial, so move the bat accordingly.
            bat.X += (float)(args.RotationDeltaInDegrees * 16.0);
            bat.Y = screenHeight - 256;
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

        
         // Create the bricks that make up the wall

            wall = new SpriteClass[12*27];

            int i = 0;

            for (float y = 200; y < 800; y += 50)
            {
                for (float x = 200; x < 2900; x += 100)
                {
                    wall[i] = new SpriteClass(GraphicsDevice, "content/block.png");
                    wall[i].isCircular = false;
                    wall[i].Y = y;
                    wall[i].X = x;
                    i++;
                }
            }
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

            for (int i = 0; i < wall.Length; i++)
            {
                if (wall[i].Collision(ball))
                {
                    ball.DY = -ball.DY;
                    wall[i].Y = -100; // Simply push it out of the way, above the screen
                }
            }

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
            int c = 0;
            for (int i = 0; i < wall.Length; i++)
            {
                // Pick a new color every line
                if (i % 54 == 0) c++;
                if (wall[i].Y != -100)
                wall[i].DrawColor(spriteBatch,c);

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
