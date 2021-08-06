using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CandyBubble.GameObjects
{
    class Candy : GameObject
    {
        
        private int candyID;
        private int rowID, columnID;
        private float Speed;
        public Boolean matched;

        public enum CandyColor
        {
            GREEN,
            RED,
            YELLOW,
            BLUE,
            COUNT

        }
        public CandyColor candyColor;

        public enum CandyStates 
        {
            RANDOM,
            IN_GRID, 
            FIRED, 
            CHECK, 
            MATCHED
        }
        public CandyStates candyStates;
        
        public Candy(Texture2D texture, int color) : base(texture) {
            switch(color)
            {
                case 0 :
                {
                    candyColor = CandyColor.GREEN;
                    break;
                }
                case 1 :
                {
                    candyColor = CandyColor.RED;
                    break;
                }
                case 2 :
                {
                    candyColor = CandyColor.YELLOW;
                    break;
                }
                case 3 :
                {
                    candyColor = CandyColor.BLUE;
                    break;
                }
            }
        }
            
        public Candy(Texture2D texture, int candyID, String type, int color) : base(texture)
        {
            this.candyID = candyID;
            this.columnID = candyID / 100;
            this.rowID = candyID % 100;
            switch(type)
            {
                case "INIT" :
                {
                    candyStates = CandyStates.IN_GRID;
                    break;
                }
                
            }
            switch(color)
            {
                case 0 :
                {
                    candyColor = CandyColor.GREEN;
                    break;
                }
                case 1 :
                {
                    candyColor = CandyColor.RED;
                    break;
                }
                case 2 :
                {
                    candyColor = CandyColor.YELLOW;
                    break;
                }
                case 3 :
                {
                    candyColor = CandyColor.BLUE;
                    break;
                }
                
                
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture,
                            Position,
                            Viewport,
                            Color.White);
            base.Draw(spriteBatch);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
          
            switch(candyStates)
            {
                case CandyStates.RANDOM : 
                {   
                    break;
                }
                case CandyStates.IN_GRID : 
                {   
                    this.setVelocityINIT();                
                    Position += Velocity * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                    checkWin();
                    break;
                }
                case CandyStates.FIRED : 
                {   
                    if(Position.Y <= 0 || Position.Y >= Singleton.HEIGHT)
                    {
                        IsActive = false;
                        Singleton.Instance.CandyLeft--;
                        Singleton.Instance.Score -= 20 * Singleton.Instance.Level;
                    }
                 

                    //left wall
                    if(Position.X <= Singleton.HITBOX || Position.X >= Singleton.INVADERHORDEWIDTH)
                    {
                       
                        Direction.X *= -1;
                    }
                    
                    Position -= Direction * Velocity * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
           
                    checkCollision();
  
                    break;
                }
                case CandyStates.CHECK : 
                {
                    checkNeighbors();
                    break;
                }
                case CandyStates.MATCHED : 
                {
                    Singleton.Instance.CandyLeft--;
                    Singleton.Instance.Score += 100 * Singleton.Instance.Level;
                    this.IsActive = false;
                    Singleton.Instance.candyMap.Remove(candyID);
                    checkWin();
                    break;
                }
                
            }
            base.Update(gameTime, gameObjects);
        }

        public void checkWin()
        {   
            if(Position.Y >= (Singleton.HEIGHT - Singleton.CANNONAREAHEIGHT) && Name.Equals("Candy")) {
                Singleton.Instance.CurrentGameResult = Singleton.GameResult.Lose;
                Singleton.Instance.CurrentGameState = Singleton.GameState.GameEnded;
            } else if (Singleton.Instance.CandyLeft <= 0) {
                Singleton.Instance.CurrentGameResult = Singleton.GameResult.Win;
                Singleton.Instance.CurrentGameState = Singleton.GameState.GameEnded;
            }
        }

        public void checkNeighbors() 
        {   
            int check = 0;

            int neighbor1 = (this.columnID + 1) * 100 + this.rowID;
            int neighbor2 = (this.columnID - 1) * 100 + this.rowID;
            int neighbor3 = (this.columnID) * 100 + this.rowID - 1;
            int neighbor4;
            int neighbor5;
            int neighbor6;
            if ((this.rowID) % 2 == 0) 
            {
                neighbor4 = (this.columnID + 1) * 100 + this.rowID - 1;
                neighbor5 = (this.columnID) * 100 + this.rowID + 1;
                neighbor6 = (this.columnID + 1) * 100 + this.rowID + 1;
            } else 
            {
                neighbor4 = (this.columnID - 1) * 100 + this.rowID - 1;
                neighbor5 = (this.columnID) * 100 + this.rowID + 1;
                neighbor6 = (this.columnID - 1) * 100 + this.rowID + 1;
            }

            if(Singleton.Instance.candyMap.ContainsKey(neighbor1) && (Singleton.Instance.candyMap[neighbor1].candyColor.Equals(candyColor)) 
                && (Singleton.Instance.candyMap[neighbor1].matched.Equals(false)))            
            {
                this.matched = true;
                Singleton.Instance.candyMap[neighbor1].matched = true;
                Singleton.Instance.candyMap[neighbor1].candyStates = CandyStates.CHECK;
                
            }

            if(Singleton.Instance.candyMap.ContainsKey(neighbor2) && (Singleton.Instance.candyMap[neighbor2].candyColor.Equals(candyColor)) 
                && (Singleton.Instance.candyMap[neighbor2].matched.Equals(false)))            
            {
                this.matched = true;
                Singleton.Instance.candyMap[neighbor2].matched = true;
                Singleton.Instance.candyMap[neighbor2].candyStates = CandyStates.CHECK;
                
            }

            if(Singleton.Instance.candyMap.ContainsKey(neighbor3) && (Singleton.Instance.candyMap[neighbor3].candyColor.Equals(candyColor)) 
                && (Singleton.Instance.candyMap[neighbor3].matched.Equals(false)))            
            {
                this.matched = true;
                Singleton.Instance.candyMap[neighbor3].matched = true;
                Singleton.Instance.candyMap[neighbor3].candyStates = CandyStates.CHECK;
                
            }

            if(Singleton.Instance.candyMap.ContainsKey(neighbor4) && (Singleton.Instance.candyMap[neighbor4].candyColor.Equals(candyColor)) 
                && (Singleton.Instance.candyMap[neighbor4].matched.Equals(false)))            
            {
                matched = true;
                Singleton.Instance.candyMap[neighbor4].matched = true;
                Singleton.Instance.candyMap[neighbor4].candyStates = CandyStates.CHECK;
                
            }

            if(Singleton.Instance.candyMap.ContainsKey(neighbor5) && (Singleton.Instance.candyMap[neighbor5].candyColor.Equals(candyColor)) 
                && (Singleton.Instance.candyMap[neighbor5].matched.Equals(false)))            
            {
                this.matched = true;
                Singleton.Instance.candyMap[neighbor5].matched = true;
                Singleton.Instance.candyMap[neighbor5].candyStates = CandyStates.CHECK;
                
            }

            if(Singleton.Instance.candyMap.ContainsKey(neighbor6) && (Singleton.Instance.candyMap[neighbor6].candyColor.Equals(candyColor)) 
                && (Singleton.Instance.candyMap[neighbor6].matched.Equals(false)))            
            {
                this.matched = true;
                Singleton.Instance.candyMap[neighbor6].matched = true;
                Singleton.Instance.candyMap[neighbor6].candyStates = CandyStates.CHECK;
                
            }
            
            if(matched) 
            {
                candyStates = CandyStates.MATCHED;
            } 
            else 
            { 
                candyStates = CandyStates.IN_GRID;
            }
            
        }

        public void checkCollision() 
        {
            foreach(KeyValuePair<int, Candy> entry in Singleton.Instance.candyMap)
            {
                int otherCandyID = entry.Key;
                Candy othercandy = entry.Value;
                if(IsTouching(othercandy) && othercandy.Name.Equals("Candy")) {
                    int otherCandyRowID = otherCandyID % 100;
                    int otherCandyColumnID = otherCandyID / 100;
                    
                    if((Position.Y >= othercandy.Position.Y + (Singleton.HITBOX / 2))){
                        Position.Y = othercandy.Position.Y + Singleton.HITBOX;
                        rowID = otherCandyRowID + 1;
                        if((rowID) % 2 == 0) {
                            if(otherCandyColumnID == 8){
                                Position.X = (otherCandyColumnID - 1) * Singleton.HITBOX + (Singleton.HITBOX / 2);
                                
                            } else {
                                if(Position.X >= othercandy.Position.X) {
                                    Position.X = otherCandyColumnID * Singleton.HITBOX + (Singleton.HITBOX / 2);
                                } else {
                                    Position.X = (otherCandyColumnID - 1) * Singleton.HITBOX + (Singleton.HITBOX / 2);
                                }
                            }
                            columnID = (int) (Position.X - (Singleton.HITBOX / 2)) / Singleton.HITBOX;
                        } else {
                            if(Position.X <= othercandy.Position.X) {
                                Position.X = otherCandyColumnID * Singleton.HITBOX;
                            } else {
                                Position.X = (otherCandyColumnID + 1) * Singleton.HITBOX;
                            }
                            columnID = (int) (Position.X) / Singleton.HITBOX;
                        }

                    } else {
                        Position.Y = othercandy.Position.Y;
                        rowID = otherCandyRowID;
                        if((rowID) % 2 == 0) {
                            if(Position.X >= othercandy.Position.X){
                                Position.X = (otherCandyColumnID + 1) * Singleton.HITBOX + (Singleton.HITBOX / 2);
                            } else {
                                Position.X = (otherCandyColumnID - 1) * Singleton.HITBOX + (Singleton.HITBOX / 2);
                            }
                            columnID = (int) (Position.X - (Singleton.HITBOX / 2)) / Singleton.HITBOX;
                          
                        } else {
                            if(Position.X >= othercandy.Position.X){
                                Position.X = (otherCandyColumnID + 1) * Singleton.HITBOX;
                            } else {
                                Position.X = (otherCandyColumnID - 1) * Singleton.HITBOX;
                            }
                            columnID = (int) (Position.X) / Singleton.HITBOX;
                        }
                        

                    }
     
                    candyID = columnID * 100 + rowID;
                    Singleton.Instance.candyMap.Add(candyID, this);
                    candyStates = CandyStates.CHECK;
                    break;
                }
                
            }
         
        }

        public void setVelocityINIT() 
        {   
            this.Velocity.X = 0;
            this.Velocity.Y = (float) (((Singleton.Instance.Level  * 1.4) / 10 + 1) * 5);
            this.Direction.X = 0;
            this.Direction.Y = 0;
        }

       
       

       
    }
}
