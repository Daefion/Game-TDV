using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public enum Direction
{
    Up, Down, Left, Right // 0, 1, 2, 3
}

namespace Project1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private int nrLinhas = 0;
        private int nrColunas = 0;
        public char[,] level;
        private Texture2D dot, box, wall; //Load images Texture
        private Texture2D[] player;
        int tileSize = 64;
        private Player Sokoban;
        public List<Point> boxes;
        public Direction direction = Direction.Down;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;


        }
        public bool Victory()
        {
            foreach (Point b in boxes) // pecorrer a lista das caixas
            {
                if (level[b.X, b.Y] != '.') return false; // verifica se há caixas sem pontos
            }
            return true;
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            LoadLevel("level1.txt"); //Carrega o ficheiro
            _graphics.PreferredBackBufferHeight = tileSize * level.GetLength(1); //definição da altura
            _graphics.PreferredBackBufferWidth = tileSize * level.GetLength(0); //definição da largura
            _graphics.ApplyChanges(); //aplica a atualização da janela
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("File");
            dot = Content.Load<Texture2D>("EndPoint_Yellow");
            box = Content.Load<Texture2D>("Crate_Gray");
            wall = Content.Load<Texture2D>("WallRound_Black");
            // TODO: use this.Content to load your game content here
            player = new Texture2D[4];
            player[(int)Direction.Down] = Content.Load<Texture2D>("Character4");
            player[(int)Direction.Up] = Content.Load<Texture2D>("Character7");
            player[(int)Direction.Left] = Content.Load<Texture2D>("Character1");
            player[(int)Direction.Right] = Content.Load<Texture2D>("Character2");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.R)) Initialize();

            if (Victory()) Exit();

            Sokoban.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FloralWhite);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "-No ceu tem pao?", new Vector2(50, 20), Color.PaleVioletRed);
            _spriteBatch.DrawString(font, $"Numero de Linhas = {nrLinhas} \n Numero de Colunas = {nrColunas}", new Vector2(0, 50), Color.Black);
            Rectangle position = new Rectangle(0, 0, tileSize, tileSize);
            


            for (int x = 0; x < level.GetLength(0); x++) //pega a primeira dimensão
            {
                for (int y = 0; y < level.GetLength(1); y++) //pega a segunda dimensão
                {
                    position.X = x * tileSize;
                    position.Y = y * tileSize;
                    switch (level[x, y])
                    {
                        //case 'Y':
                        //    _spriteBatch.Draw(player, position, Color.White);
                        //    break;
                        //case '#':
                        //    _spriteBatch.Draw(box, position, Color.White);
                        //    break;
                        case '.':
                            _spriteBatch.Draw(dot, position, Color.White);
                            break;
                        case 'X':
                            _spriteBatch.Draw(wall, position, Color.White);
                            break;

                    }
                }
            }
            position.X = Sokoban.Position.X * tileSize; //posição do Player
            position.Y = Sokoban.Position.Y * tileSize; //posição do Player
            _spriteBatch.Draw(player[(int)direction], position, Color.White); //desenha o Player

            foreach (Point b in boxes)
            {
                position.X = b.X * tileSize;
                position.Y = b.Y * tileSize;
                _spriteBatch.Draw(box, position, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void LoadLevel(string levelFile)
        {
            string[] linhas = File.ReadAllLines($"Content/{levelFile}"); // "Content/" + level

            nrLinhas = linhas.Length;
            nrColunas = linhas[0].Length;
            level = new char[nrColunas, nrLinhas];
            boxes = new List<Point>();

            for (int x = 0; x < nrColunas; x++)
            {
                for (int y = 0; y < nrLinhas; y++)
                {
                    if (linhas[y][x] == '#')
                    {
                        boxes.Add(new Point(x, y));
                        level[x, y] = ' '; // put a blank instead of the box '#'
                    }
                    else if (linhas[y][x] == 'Y')
                    {
                        Sokoban = new Player(this, x, y);
                        level[x, y] = ' '; // put a blank instead of the sokoban 'Y'
                    }
                    else
                    {
                        level[x, y] = linhas[y][x];
                    }
                }
            }



            for (int x = 0; x < nrColunas; x++)
            {
                for (int y = 0; y < nrLinhas; y++)
                {
                    if (linhas[y][x] == 'Y')
                    {
                        Sokoban = new Player(this, x, y);
                        level[x, y] = ' '; // put a blank instead of the sokoban 'Y'
                    }
                    else
                    {
                        level[x, y] = linhas[y][x];
                    }
                }
            }

        }
        public bool HasBox(int x, int y)
        {
            foreach (Point b in boxes)
            {
                if (b.X == x && b.Y == y) return true; // se a caixa tiver a mesma posição do Player
            }
            return false;
        }
        public bool FreeTile(int x, int y)
        {
            if (level[x, y] == 'X') return false; // se for uma parede está ocupada
            if (HasBox(x, y)) return false; // verifica se é uma caixa
            return true;
            /* The same as: return level[x,y] != 'X' && !HasBox(x,y); */
        }
    }

}