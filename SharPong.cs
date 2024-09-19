using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SharPong
{
    public class SharPong : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int SCREEN_WIDTH = 1080;
        const int SCREEN_HEIGHT = 720;

        // Paddle Vars //
        private int paddle_width;
        private int paddle_height;
        private float paddleSpeed;
        private Texture2D paddle_one;
        private Vector2 paddleOnePosition;
        private Texture2D paddle_two;
        private Vector2 paddleTwoPosition;
        private Rectangle paddleOneRect;
        private Rectangle paddleTwoRect;
        private Texture2D paddleOneRectTexture;
        private Texture2D paddleTwoRectTexture;


        // Horizontal Lines vars (Top/Bottom lines) //
        private int horizontalLineWidth;
        private int horizontalLineHeight;
        private Texture2D horizontalLine;
        private Vector2 horizontalLineTopPos;
        private Vector2 horizontalLineBottomPos;
        private Texture2D horizontalLineRectTopTexture;
        private Texture2D horizontalLineRectBottomTexture;
        private Rectangle horizontalLineTopRect;
        private Rectangle horizontalLineBottomRect;


        // Dash lines
        private int dashLineWidth;
        private int dashLineHeight;
        private Texture2D dash;


        // Ball
        private int ballWidth;
        private int ballHeight;
        private float ballSpeed;
        private Texture2D ballTexture;
        private Vector2 ballPosition;
        private Rectangle ballRect;
        private Texture2D ballRectTexture;
        private double maxBounceAngle;
        private double bounceAngle;
        private double ballVx;
        private double ballVy;
        private int ballDirection;

        // Score Text 
        private SpriteFont scoreFont;
		private SpriteFont startScreenFont;
		private SpriteFont startScreenFontSmall;
		private bool playerOneWon;
		private bool playerTwoWon;
		private int winningScore;


        // Title Screen

		private String pongTitle;
		private String pressStart;
        
        private int playerOneScore;
        private int playerTwoScore;

		private bool gameStarted = false;


        // Flags
        public bool ShowCollisionRect = true;


        static void Main(string[] args)
        {
            // Your game logic here
        }


        public SharPong()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Screen Width / Height
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            // Paddle init
            paddle_width = (int)(graphics.PreferredBackBufferWidth * 0.02); // Set paddle with to be 2% of the screen
            paddle_height = (int)(graphics.PreferredBackBufferHeight * 0.15); // Set the paddle height to be 15% of the screen

            paddleOnePosition = new Vector2(0, 400);  // Start at (0,50) , left side of the screen
            paddleTwoPosition = new Vector2(graphics.PreferredBackBufferWidth - paddle_width, 400); // Set position to screen width so paddle is on the right side

            paddleOneRect = new Rectangle((int)paddleOnePosition.X, (int)paddleTwoPosition.Y, paddle_width, paddle_height);
            paddleTwoRect = new Rectangle((int)paddleTwoPosition.X, (int)paddleTwoPosition.Y, paddle_width, paddle_height);

            paddleSpeed = 1000f; // Paddle speed for bath paddles

            // Horizontal Lines init
            horizontalLineWidth = SCREEN_WIDTH;
            horizontalLineHeight = (int)(graphics.PreferredBackBufferHeight * 0.01); // Set the height of the horizontal line to be 1% of the screen
            horizontalLineTopPos = new Vector2(0, 0);
            horizontalLineBottomPos = new Vector2(0, SCREEN_HEIGHT - horizontalLineHeight);
            horizontalLineTopRect = new Rectangle((int)horizontalLineTopPos.X, (int)horizontalLineTopPos.Y, horizontalLineWidth, horizontalLineHeight);
            horizontalLineBottomRect = new Rectangle((int)horizontalLineBottomPos.X, (int)horizontalLineBottomPos.Y, horizontalLineWidth, horizontalLineHeight);


            // Dash lines init
            dashLineWidth = (int)(graphics.PreferredBackBufferWidth * 0.01);
            dashLineHeight = (int)(graphics.PreferredBackBufferWidth * 0.01);

            // Ball init
            ballPosition = new Vector2(500, 500);
			ballWidth = (int)(SCREEN_WIDTH * 0.025);
			ballHeight = (int)(SCREEN_WIDTH * 0.025);
            ballSpeed = 700f;
            ballVx = ballSpeed * Math.Cos(maxBounceAngle);
			ballVy = ballSpeed * -Math.Sin(maxBounceAngle);
            ballRect = new Rectangle((int)ballPosition.X, (int)ballPosition.Y, ballWidth, ballHeight);
            maxBounceAngle = 3 * Math.PI / 12;
            bounceAngle = 0;
            ballDirection = 1;


            // Score 
            playerOneScore = 0;
            playerTwoScore = 0;
			winningScore = 15;
			playerOneWon = false;
			playerTwoWon = false;

			// Start screen
			pongTitle = "PONG";
			pressStart = "Press Space";


            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load paddles texture
            paddle_one = new Texture2D(graphics.GraphicsDevice, paddle_width, paddle_height);
            paddle_two = new Texture2D(graphics.GraphicsDevice, paddle_width, paddle_height);
            paddleOneRectTexture = new Texture2D(graphics.GraphicsDevice, paddleOneRect.Width, paddleOneRect.Height);
            paddleTwoRectTexture = new Texture2D(graphics.GraphicsDevice, paddleTwoRect.Width, paddleOneRect.Height);

            // Horizontal Line texture
            horizontalLine = new Texture2D(graphics.GraphicsDevice, horizontalLineWidth, horizontalLineHeight);
            horizontalLineRectTopTexture = new Texture2D(graphics.GraphicsDevice, horizontalLineWidth, horizontalLineHeight);
            horizontalLineRectBottomTexture = new Texture2D(graphics.GraphicsDevice, horizontalLineWidth, horizontalLineHeight);

            // Dash Line texture
            dash = new Texture2D(graphics.GraphicsDevice, dashLineWidth, dashLineHeight);

            // Ball textures
            ballTexture = new Texture2D(graphics.GraphicsDevice, ballWidth, ballHeight);
            ballRectTexture = new Texture2D(graphics.GraphicsDevice, 40, 40);
            // hbrenes@gmail.com
            // Sprite Font
            scoreFont = Content.Load<SpriteFont>("Score");
			startScreenFont = Content.Load<SpriteFont>("StartScreen");
			startScreenFontSmall = Content.Load<SpriteFont>("StartScreenSmall");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

			var kstate = Keyboard.GetState();

			if (kstate.IsKeyDown(Keys.Space) && !gameStarted)
                gameStarted = true;

			if (gameStarted && !playerOneWon && !playerTwoWon)
			{
				paddleTwoRect.Y = (int)paddleTwoPosition.Y;


				// Move Paddle 1
				if (kstate.IsKeyDown(Keys.W) && paddleOnePosition.Y > horizontalLineHeight)
					paddleOnePosition.Y -= (int)(paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);


				if (kstate.IsKeyDown(Keys.S) && paddleOnePosition.Y < (SCREEN_HEIGHT - paddle_height) - horizontalLineHeight)
					paddleOnePosition.Y += (int)(paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

				paddleOneRect.Y = (int)paddleOnePosition.Y;

				// Move Paddle 2
				if (kstate.IsKeyDown(Keys.Up) && paddleTwoPosition.Y > horizontalLineHeight)
					paddleTwoPosition.Y -= (int)(paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

				if (kstate.IsKeyDown(Keys.Down) && paddleTwoPosition.Y < (SCREEN_HEIGHT - paddle_height) - horizontalLineHeight)
					paddleTwoPosition.Y += (int)(paddleSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);



				ballVx = ballSpeed * Math.Cos(bounceAngle) * ballDirection;
				ballVy = ballSpeed * -Math.Sin(bounceAngle) * ballDirection;


				// Move Ball/ballrect : test
				ballPosition.X += (int)(ballVx * (float)gameTime.ElapsedGameTime.TotalSeconds);
				ballPosition.Y += (int)(ballVy * (float)gameTime.ElapsedGameTime.TotalSeconds);

				ballRect.X = (int)ballPosition.X;
				ballRect.Y = (int)ballPosition.Y;


				bool collidesPaddleTwo = CheckCollision(ballRect, paddleTwoRect);
				bool collidesPaddleOne = CheckCollision(ballRect, paddleOneRect);
				bool collidesHorizontalTop = CheckCollision(ballRect, horizontalLineTopRect);
				bool collidesHorizontalBot = CheckCollision(ballRect, horizontalLineBottomRect);

				if (collidesPaddleOne)
				{
					ballDirection = 1;
					int relativeCollision = ballRect.Y - paddleOneRect.Y;

					if (relativeCollision <= (paddle_height / 2) && relativeCollision > 0)
					{
						bounceAngle = maxBounceAngle * 1;
					}
					else if (relativeCollision >= paddle_height - (paddle_height / 2) && relativeCollision < paddle_height)
					{
						bounceAngle = maxBounceAngle * -1;
					}
					else if (relativeCollision < 0)
					{
						bounceAngle = maxBounceAngle * 1;
						ballSpeed += 200f;
					}
					else if (relativeCollision > paddle_height)
					{
						bounceAngle = maxBounceAngle * -1;
						ballSpeed += 200f;
					}
					else
					{
						bounceAngle = 0;
					}
				}
				else if (collidesPaddleTwo)
				{

					ballDirection = -1;
					int relativeCollision = ballRect.Y - paddleTwoRect.Y;

					if (relativeCollision <= (paddle_height / 2) && relativeCollision > 0)
					{
						bounceAngle = maxBounceAngle * -1;
					}
					else if (relativeCollision >= paddle_height - (paddle_height / 2) && relativeCollision < paddle_height)
					{
						bounceAngle = maxBounceAngle * 1;
					}
					else if (relativeCollision < 0)
					{
						bounceAngle = maxBounceAngle * -1;
						ballSpeed += 200f;
					}
					else if (relativeCollision > paddle_height)
					{
						bounceAngle = maxBounceAngle * 1;
						ballSpeed += 200f;
					}
					else
					{
						bounceAngle = 0;
					}
				}
				else if (collidesHorizontalTop || collidesHorizontalBot)
				{
					bounceAngle = bounceAngle * -1;
				}

				// Reset ball position if it's out of the window and depending on the side
				// that the ball went out update player one or player two score.
				if (ballPosition.X > SCREEN_WIDTH)
				{
					ResetBall(1);
					playerOneScore += 1;
				}
				else if (ballPosition.X < 0)
				{
					// Player Two Score +1
					ResetBall(-1);
					playerTwoScore += 1;
				}


				// Check for winning condition
				if (playerOneScore >= winningScore)
				{
					playerOneWon = true;
				}
				else if (playerTwoScore >= winningScore)
				{
					playerTwoWon = true;
				}
			}
            
                    
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

			if (gameStarted){
				DrawMainGame();
			}else {
				DrawStartScreen();
			}
            

            spriteBatch.End();
            base.Draw(gameTime);
        }


        static private bool CheckCollision(Rectangle a, Rectangle b)
        {
            if (a.Intersects(b))
            {
                return true;
            }
            return false;
        }


        private void ResetBall(int direction)
        {
            // Player One Score +1 
            ballPosition.X = SCREEN_WIDTH / 2 - ballWidth;
            ballPosition.Y = SCREEN_HEIGHT / 2 - ballHeight;
            bounceAngle = 0;
            ballDirection = direction;
            ballSpeed = 700f;
        }

		private void DrawStartScreen() {
			spriteBatch.DrawString(startScreenFont, pongTitle, new Vector2((SCREEN_WIDTH/2) - startScreenFont.MeasureString(pongTitle).X / 2, 100), Color.White);
			spriteBatch.DrawString(startScreenFontSmall, pressStart, new Vector2((SCREEN_WIDTH / 2) - startScreenFontSmall.MeasureString(pressStart).X / 2, 250), Color.White);
		}

		private void DrawMainGame(){

			// Draw left/right paddles 
            Color[] data = new Color[paddle_width * paddle_height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            paddle_one.SetData(data);
            paddle_two.SetData(data);

            spriteBatch.Draw(paddle_one, paddleOnePosition, Color.White);
            spriteBatch.Draw(paddle_two, paddleTwoPosition, Color.White);

            // Draw top/bottom horizonal lines
            Color[] hzline_data = new Color[horizontalLineWidth * horizontalLineHeight];
            for (int i = 0; i < hzline_data.Length; ++i) hzline_data[i] = Color.White;
            horizontalLine.SetData(hzline_data);

            spriteBatch.Draw(horizontalLine, horizontalLineTopPos, Color.White);
            spriteBatch.Draw(horizontalLine, horizontalLineBottomPos, Color.White);

            // Draw dash lines in the center of screen
            Color[] dash_data = new Color[dashLineWidth * dashLineHeight];
            for (int i = 0; i < dash_data.Length; ++i) dash_data[i] = Color.White;
            dash.SetData(dash_data);


            int dashAmount = SCREEN_HEIGHT / dashLineHeight;    // Amount of lines needed to fill the screen height

            for (int j = 0; j <= dashAmount; j++)
            {
                if (j % 2 == 0)     // Skip one dash to make it a dashed line
                    spriteBatch.Draw(dash, new Vector2((SCREEN_WIDTH / 2) - dashLineWidth / 2, (dashLineHeight * j)), Color.White);
            }


            // Draw ball texture/sprite
            Color[] ball_data = new Color[ballWidth * ballHeight];
            for (int i = 0; i < ball_data.Length; ++i) ball_data[i] = Color.White;
            ballTexture.SetData(ball_data);
            spriteBatch.Draw(ballTexture, ballPosition, Color.White);


            // Draw both player scores
			spriteBatch.DrawString(scoreFont, playerOneScore.ToString(), new Vector2((int)(((SCREEN_WIDTH * 0.45) - scoreFont.MeasureString(playerOneScore.ToString()).X)), 100), Color.White);
            spriteBatch.DrawString(scoreFont, playerTwoScore.ToString(), new Vector2((int)(SCREEN_WIDTH * 0.55), 100), Color.White);			
		}

    }
}