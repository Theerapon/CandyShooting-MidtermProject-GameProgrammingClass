using CandyBubble.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace CandyBubble
{
    
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Cannon cannon;

        List<GameObject> _gameObjects;
        int _numObject;
        
        SpriteFont _font;
        SpriteFont _fontMain;
        SpriteFont _fontScore;
        SpriteFont _fontLevel;
        Texture2D background;    
        Texture2D frame;

        Texture2D pixel;
        Texture2D spaceCandyTexture;

        Candy _currentCandy, _nextCandy;
        int timeBetweenShots = 300; // Thats 300 milliseconds
        int shotTimer = 0;
        Random rnd;
        Vector2 fontSize;

        
        

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //int g  = Singleton.Instance.randomColor[0];
        }

        
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = Singleton.WIDTH;
            graphics.PreferredBackBufferHeight = Singleton.HEIGHT;
            graphics.ApplyChanges();

            _gameObjects = new List<GameObject>();
            Singleton.Instance.candyMap = new Dictionary<int, Candy>();
            Singleton.Instance.CurrentKey = Keyboard.GetState();
            Singleton.Instance.CurrentMouse = Mouse.GetState();
            Singleton.Instance.CurrentGameResult = Singleton.GameResult.Lose;
            

            
            base.Initialize();
        }
        
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font/GameFont");
            _fontMain = Content.Load<SpriteFont>("Font/GameFontMain");
            _fontScore = Content.Load<SpriteFont>("Font/GameFontScore");
            _fontLevel = Content.Load<SpriteFont>("Font/GameFontLevel");
            frame = Content.Load<Texture2D>("Images/marshmallow");
            background = Content.Load<Texture2D>("Images/background");
            spaceCandyTexture = this.Content.Load<Texture2D>("Images/cody");
            rnd = new Random();


            Singleton.Instance.CurrentGameState = Singleton.GameState.GameMain;
            
        }

       
        protected override void UnloadContent()
        {
        }

        
        protected override void Update(GameTime gameTime)
        {
            Singleton.Instance.CurrentKey = Keyboard.GetState();
            _numObject = _gameObjects.Count;

            switch (Singleton.Instance.CurrentGameState)
            {
                case Singleton.GameState.GameMain:
                    if (!Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey) && Singleton.Instance.CurrentKey.IsKeyDown(Keys.Space))
                    {
                        
                        //Space keys pressed to start
                        Singleton.Instance.CurrentGameState = Singleton.GameState.GameStart;
                    }
                    if (!Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey) && Singleton.Instance.CurrentKey.IsKeyDown(Keys.Escape))
                    {
                        //Space keys pressed to start
                        this.Exit();
                    }
                    break;

                case Singleton.GameState.GameStart:
                    if(Singleton.Instance.CurrentGameResult == Singleton.GameResult.Lose){
                        Singleton.Instance.Level = 1;
                        Singleton.Instance.Score = 0;
                        Reset();
                    } else {
                        Singleton.Instance.Level++;
                        Singleton.Instance.Score = 0;
                        Reset();
                    }
                    Singleton.Instance.CurrentGameState = Singleton.GameState.GamePlaying;
                    break;

                case Singleton.GameState.GamePlaying:
                    Vector2 Difference = Vector2.Subtract(cannon.Position, new Vector2(Singleton.Instance.CurrentMouse.X,Singleton.Instance.CurrentMouse.Y));
                    Vector2 Direction = Vector2.Normalize(Difference);
                    
                    
                    if (Singleton.Instance.CurrentMouse.LeftButton == ButtonState.Pressed)
                    {
                        shotTimer += gameTime.ElapsedGameTime.Milliseconds;
                        if(shotTimer > timeBetweenShots)
                        {
                            shotTimer = 0;
                            Candy candy = _currentCandy;
                            candy.Position = new Vector2((Singleton.INVADERHORDEWIDTH / 2) + (Singleton.HITBOX / 2 ), Singleton.INVADERHORDEHEIGHT + (Singleton.CANNONAREAHEIGHT / 2) - (Singleton.HITBOX / 2));
                            candy.Name = "Candy";
                            candy.candyStates = Candy.CandyStates.FIRED;
                            candy.Velocity = new Vector2(1000,1000);
                            candy.Direction = Direction;
                            
                            Singleton.Instance.CandyLeft++;
                            _currentCandy = _nextCandy;
                            _currentCandy.Position = new Vector2(Singleton.WIDTH - (Singleton.SCOREAREAWIDTH / 2) - (Singleton.HITBOX), (Singleton.HEIGHT - Singleton.HITBOX * 2));

                            int indexColor = rnd.Next(4);
                            _nextCandy = new Candy(spaceCandyTexture, indexColor)
                            {
                                Viewport = new Rectangle(0, indexColor * Singleton.HITBOX, Singleton.HITBOX, Singleton.HITBOX),
                               Position = new Vector2(Singleton.WIDTH - (Singleton.SCOREAREAWIDTH / 2) - (Singleton.HITBOX), (Singleton.HEIGHT - Singleton.HITBOX * 4))
                            };
                            _gameObjects.Add(_nextCandy);
                            
                        }
                    }

                    for (int i = 0; i < _numObject; i++)
                    {
                        if (_gameObjects[i].IsActive)
                        {
                            _gameObjects[i].Update(gameTime, _gameObjects);
                        }
                    }
                    for (int i = 0; i < _numObject; i ++)
                    {
                        if (!_gameObjects[i].IsActive)
                        {
                            _gameObjects.RemoveAt(i);
                            i--;
                            _numObject--;
                        }
                    }
                    break;

                case Singleton.GameState.GameEnded:
                    _gameObjects.Clear();
                    Singleton.Instance.candyMap.Clear();

                    if (!Singleton.Instance.CurrentKey.Equals(Singleton.Instance.PreviousKey) && Singleton.Instance.CurrentKey.GetPressedKeys().Length > 0)
                    {
                        if(Singleton.Instance.CurrentGameResult == Singleton.GameResult.Win) {
                            Singleton.Instance.CurrentGameState = Singleton.GameState.GameStart;
                        } else if(Singleton.Instance.CurrentGameResult == Singleton.GameResult.Lose) {
                            Singleton.Instance.CurrentGameState = Singleton.GameState.GameMain;
                        }
                    }
                        
                    break;
            }

            Singleton.Instance.PreviousKey = Singleton.Instance.CurrentKey;

            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightBlue);
            spriteBatch.Begin();

            //Vector2 fontSize = _font.MeasureString(Singleton.Instance.Score.ToString());
            //spriteBatch.DrawString(_font, Singleton.Instance.Score.ToString(), new Vector2((Singleton.WIDTH - fontSize.X) / 4, 20), Color.White);
            

            switch (Singleton.Instance.CurrentGameState)
            {
                case Singleton.GameState.GameMain:
                    fontSize = _fontMain.MeasureString("Press Spacebar key to Start");
                    spriteBatch.DrawString(_fontMain, "Press Spacebar key to Start", new Vector2((Singleton.WIDTH - fontSize.X) / 2, (Singleton.HEIGHT - fontSize.Y) / 2 - 50), Color.Black);
                    fontSize = _fontMain.MeasureString("Press Escape key to Exits");
                    spriteBatch.DrawString(_fontMain, "Press Escape key to Exits", new Vector2((Singleton.WIDTH - fontSize.X) / 2, (Singleton.HEIGHT - fontSize.Y) / 2 + 50), Color.Black);
                    break;
               case Singleton.GameState.GamePlaying:
                    spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    spriteBatch.Draw(frame, new Rectangle(0, 0, Singleton.WIDTH-350, Singleton.HEIGHT), Color.White);
                    //level
                    fontSize = _fontLevel.MeasureString("Level");
                    spriteBatch.DrawString(_fontLevel, "Level",  new Vector2((Singleton.WIDTH - fontSize.X/ 2) - Singleton.SCOREAREAWIDTH /2 - 40, Singleton.HEIGHT / 3), Color.Black);
                    fontSize = _fontLevel.MeasureString(Singleton.Instance.Level.ToString());
                    spriteBatch.DrawString(_fontLevel, Singleton.Instance.Level.ToString(), 
                        new Vector2((Singleton.WIDTH - fontSize.X/ 2) - Singleton.SCOREAREAWIDTH /2 + 40, Singleton.HEIGHT / 3),Color.White);
                    //Score
                    fontSize = _fontLevel.MeasureString("Score");
                    spriteBatch.DrawString(_fontLevel, "Score",  new Vector2((Singleton.WIDTH - fontSize.X / 2) - Singleton.SCOREAREAWIDTH /2, 120 ), Color.Black);
                    fontSize = _fontScore.MeasureString(Singleton.Instance.Score.ToString());
                    spriteBatch.DrawString(_fontScore, Singleton.Instance.Score.ToString(), new Vector2((Singleton.WIDTH - fontSize.X / 2) - Singleton.SCOREAREAWIDTH /2, 60), Color.White);

                    fontSize = _font.MeasureString("Next");
                    spriteBatch.DrawString(_font, "Next",  new Vector2((Singleton.WIDTH - fontSize.X/ 2) - Singleton.SCOREAREAWIDTH /2 + 40, Singleton.HEIGHT - Singleton.HITBOX * 4 + Singleton.HITBOX / 2), Color.Black);

                    fontSize = _font.MeasureString("Current");
                    spriteBatch.DrawString(_font, "Current",  new Vector2((Singleton.WIDTH - fontSize.X/ 2) - Singleton.SCOREAREAWIDTH /2 + 40, Singleton.HEIGHT - Singleton.HITBOX * 2 + Singleton.HITBOX / 2), Color.Black);
                    break;
                case Singleton.GameState.GameEnded:
                    //score
                    fontSize = _font.MeasureString("Score");
                    spriteBatch.DrawString(_font, "Score", new Vector2((Singleton.WIDTH - fontSize.X) / 2, (Singleton.HEIGHT - fontSize.Y) / 2 + 20), Color.Black);
                    fontSize = _fontScore.MeasureString(Singleton.Instance.Score.ToString());
                    spriteBatch.DrawString(_fontScore, Singleton.Instance.Score.ToString(), new Vector2((Singleton.WIDTH - fontSize.X) / 2, (Singleton.HEIGHT - fontSize.Y) / 2 + 60), Color.White);
                    //result
                    fontSize = _font.MeasureString("You are ");
                    spriteBatch.DrawString(_font, "You are ", new Vector2((Singleton.WIDTH - fontSize.X) / 2 - 10, (Singleton.HEIGHT - fontSize.Y) / 2 - 20), Color.Black);
                    fontSize = _font.MeasureString(Singleton.Instance.CurrentGameResult.ToString());
                    spriteBatch.DrawString(_font, Singleton.Instance.CurrentGameResult.ToString(), new Vector2((Singleton.WIDTH - fontSize.X) / 2 + 50, (Singleton.HEIGHT - fontSize.Y) / 2 - 20), Color.White);
                    //key
                    fontSize = _font.MeasureString("Press Specbar key to next state");
                    spriteBatch.DrawString(_font, "Press Specbar key to next state", new Vector2((Singleton.WIDTH - fontSize.X) / 2, (Singleton.HEIGHT - fontSize.Y) / 2 - 50), Color.Black);
                    break;
                
            }


            for (int i = 0; i < _gameObjects.Count; i++)
            {
                
                _gameObjects[i].Draw(spriteBatch);
            }

            spriteBatch.End();
            graphics.BeginDraw();
            base.Draw(gameTime);
        }

        protected void Reset()
        {
            Singleton.Instance.candyMap.Clear();
            Texture2D spaceCandyTexture = this.Content.Load<Texture2D>("Images/cody");
            Singleton.Instance.CandyLeft = 0;
            
            

            for(int i = 0; i < (int)Candy.CandyColor.COUNT; i++) 
            {
                Singleton.Instance.randomColor.Enqueue(rnd.Next(4));
            }

            _gameObjects.Clear();

            //candy random 1
            int indexColor = rnd.Next(4);
            _currentCandy = new Candy(spaceCandyTexture, indexColor)
            {
                Name = "CandyRandom",
                Viewport = new Rectangle(0, indexColor * Singleton.HITBOX, Singleton.HITBOX, Singleton.HITBOX),
                Position = new Vector2(Singleton.WIDTH - (Singleton.SCOREAREAWIDTH / 2) - (Singleton.HITBOX), (Singleton.HEIGHT - Singleton.HITBOX * 2))
                
            };
            _gameObjects.Add(_currentCandy);

            //candy random 2
            indexColor = rnd.Next(4);
            _nextCandy = new Candy(spaceCandyTexture, indexColor)
            {
                Name = "CandyRandom",
                Viewport = new Rectangle(0, indexColor * Singleton.HITBOX, Singleton.HITBOX, Singleton.HITBOX),
                Position = new Vector2(Singleton.WIDTH - (Singleton.SCOREAREAWIDTH / 2) - (Singleton.HITBOX), (Singleton.HEIGHT - Singleton.HITBOX * 4))
            };
            _gameObjects.Add(_nextCandy);

            //add cannon
            cannon =  new Cannon(spaceCandyTexture)
            {
                Name = "Cannon",
                Viewport = new Rectangle(0, 288, Singleton.HITBOX * 2, Singleton.HITBOX * 2),
                Position = new Vector2((Singleton.WIDTH - Singleton.SCOREAREAWIDTH) / 2, Singleton.INVADERHORDEHEIGHT + Singleton.HITBOX),
                
            };
            _gameObjects.Add(cannon);
            
            int bubbleColums = (Singleton.INVADERHORDEWIDTH / Singleton.HITBOX); //8 colums
            for (int i = 1; i <= Singleton.INITROW; i++) //rows 16
            {
                for (int j = 1; j <= bubbleColums; j++) //colums 8
                {
                    int yPicture = 0;
                    float x;
                    float y;
                   
                    if (i % 2  == 0) //7 colums
                    {
                        x = (Singleton.INVADERHORDEWIDTH / 8 * j) + (Singleton.INVADERHORDEWIDTH / 8) + (Singleton.HITBOX / 2) - Singleton.HITBOX;
                        if(j == (bubbleColums)) continue; 
                    } 
                    else //8 colums
                    {
                        x = (Singleton.INVADERHORDEWIDTH / 8 * j) + (Singleton.INVADERHORDEWIDTH / 8) - Singleton.HITBOX;
                    }
                    y = Singleton.INITPOSITION_Y + (i * Singleton.HITBOX);
                    int color = rnd.Next(4);
                    yPicture = color * Singleton.HITBOX;
                    int candyID = j * 100 + i;
                    resetCandy(x,y,yPicture,candyID,color);
                }
            } 

           

            
          

        }

        public void resetCandy(float x, float y, int yPicture, int candyID, int color) 
        {
            Texture2D spaceCandyTexture = this.Content.Load<Texture2D>("Images/cody");
            Candy candy = new Candy(spaceCandyTexture,candyID,"INIT",color)
            {
                Name = "Candy",
                Viewport = new Rectangle(0,yPicture,Singleton.HITBOX,Singleton.HITBOX),
                Position = new Vector2(x,y)
            };
            Singleton.Instance.candyMap.Add(candyID,candy);
            _gameObjects.Add(candy);
            Singleton.Instance.CandyLeft++;
        }

        
             
    }    
    
           
            
        
}
