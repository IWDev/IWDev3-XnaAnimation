using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Animation
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D texGuy;

        int frameWidth, frameHeight;

        int currentFrame, numFrames;
        double targetFrameTime, currentFrameTime;

        int walkingDirection;
        bool walking;

        Rectangle destRect;

        int parallax1Pos, parallax2Pos, parallax3Pos;
        Texture2D texParallax1, texParallax2, texParallax3;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Using an 800x600 resolution due to the projector!
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Time (in milliseconds) for one frame of animation
            targetFrameTime = 150;

            // Total number of animation frames
            numFrames = 6;

            // Walking direction: -1 = left, 0 = stopped, 1 = right
            walkingDirection = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load our animation texture
            texGuy = Content.Load<Texture2D>("gb_walk");

            // Calculate frame sizes using the texture dimensions
            frameWidth = texGuy.Width / numFrames;
            frameHeight = texGuy.Height / 2; // (Two rows in our sprite sheet)

            // Set up the draw destination rectangle for our dude
            // somewhere in the middle/bottom of the screen
            destRect = new Rectangle(350, 350, frameWidth, frameHeight);

            // Load parallax textures
            texParallax1 = Content.Load<Texture2D>("bg1");
            texParallax2 = Content.Load<Texture2D>("bg2");
            texParallax3 = Content.Load<Texture2D>("fg");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // We're not walking yet!
            walking = false;

            // Walk left
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < -0.2f ||
               Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                walkingDirection = -1;
                walking = true;
            }

            // Walk right
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0.2f ||
               Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                walkingDirection = 1;
                walking = true;
            }

            // Do animation if we're walking
            if (walking)
            {
                // Add the time elapsed since the last frame to our current frame time
                currentFrameTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                // If the current frame time has reached our target frame time...
                if (currentFrameTime >= targetFrameTime)
                {
                    // Reset current frame time to 0
                    currentFrameTime = 0;

                    // Increment current frame
                    currentFrame++;

                    // Loop the animation if we've reached the total frame count
                    // Remember the frame counter is base 0, so frames 0-5 
                    // make up our six frames of animation, 6 is one frame too many!
                    if (currentFrame == numFrames)
                        currentFrame = 0;
                }

                // Move the parallax!
                parallax1Pos += walkingDirection;
                if (parallax1Pos >= 800) parallax1Pos -= 800;
                if (parallax1Pos < 0) parallax1Pos = 800;
                parallax2Pos += walkingDirection*2;
                if (parallax2Pos >= 800) parallax2Pos -= 800;
                if (parallax2Pos < 0) parallax2Pos = 800;
                parallax3Pos += walkingDirection*4;
                if (parallax3Pos >= 800) parallax3Pos -= 800;
                if (parallax3Pos < 0) parallax3Pos = 800;
            }
            else // Not currently walking
            {
                // Reset current frame and counter, so we always start the
                // animation from the first frame
                currentFrame = 0;
                currentFrameTime = 0;
            }

            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set up a source rectangle, according to our animation
            Rectangle sourceRect;
            if (!walking)
                // The idle frame is on the second row of the sheet, so we use frameHeight
                // as out rectangle's top position
                sourceRect = new Rectangle(0, frameHeight, frameWidth, frameHeight);
            else
                // Animation is on the first row of the sheet, so we use 0 as the rectangle
                // top position. We then multiply the frameWidth by our current frame number
                // to find the left position
                sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            // Begin drawing
            spriteBatch.Begin();

            // Background layers
            spriteBatch.Draw(texParallax1,
                             new Rectangle(0, 0, 800 - parallax1Pos, 600),
                             new Rectangle(parallax1Pos, 0, 800 - parallax1Pos, 600),
                             Color.White);
            spriteBatch.Draw(texParallax1,
                             new Rectangle(800 - parallax1Pos, 0, parallax1Pos, 600),
                             new Rectangle(0, 0, parallax1Pos, 600),
                             Color.White);
            spriteBatch.Draw(texParallax2,
                             new Rectangle(0, 0, 800 - parallax2Pos, 600),
                             new Rectangle(parallax2Pos, 0, 800 - parallax2Pos, 600),
                             Color.White);
            spriteBatch.Draw(texParallax2,
                             new Rectangle(800 - parallax2Pos, 0, parallax2Pos, 600),
                             new Rectangle(0, 0, parallax2Pos, 600),
                             Color.White);

            // Draw our guy!
            spriteBatch.Draw(texGuy, destRect, sourceRect, Color.White,
                             0f, Vector2.Zero,
                             walkingDirection==-1?SpriteEffects.FlipHorizontally:SpriteEffects.None,
                             0);

            // Foreground layer
            spriteBatch.Draw(texParallax3,
                             new Rectangle(0, 0, 800 - parallax3Pos, 600),
                             new Rectangle(parallax3Pos, 0, 800 - parallax3Pos, 600),
                             Color.White);
            spriteBatch.Draw(texParallax3,
                             new Rectangle(800 - parallax3Pos, 0, parallax3Pos, 600),
                             new Rectangle(0, 0, parallax3Pos, 600),
                             Color.White);

            // End drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
