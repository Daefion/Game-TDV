using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection.Emit;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Project1
{
    /*
    * - WinScreen
    * - MultipleLevels
    * - Timer
    * - max refresh count
    */
    //public enum Direction
    //{
    //    Up, Down, Left, Right // 0, 1, 2, 3
    //}


    public class Game1 : Game
    {
        //C:\Daniel\ipca\licenciatura\Tecnicas de Desenvolvimento de Videojogos\git\sokoban_Aula_03\Sokoban_Projeto_01\Sokoban_Projeto_01

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int nrLinhas = 0;
        private int nrColunas = 0;
        private SpriteFont font;
        private SpriteFont font1;
        private Texture2D dot, box, wall; //Load images Texture 
        //private Texture2D[] player;
        private Player sokoban;
        //private char[,] level;
        private double levelTime = 0f;
        private int liveCount = 3;
        public const int tileSize = 64; //potencias de 2 (operações binárias)

        //public Direction direction = Direction.Down;
        public char[,] level;
        public List<Point> boxes;
        private string[] levelNames = { "level2.txt" }; // Level list
        private int currentLevel = 0; // Current level
        private bool rDown = false; // if R is still pressed down
        private bool isWin = false;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //LoadLevel("level1.txt");
            LoadLevel(levelNames[currentLevel]);
            _graphics.PreferredBackBufferHeight = tileSize * (1 + level.GetLength(1)); //definição da altura
            _graphics.PreferredBackBufferWidth = tileSize * level.GetLength(0); //definição da largura
            _graphics.ApplyChanges(); //aplica a atualização da janela
            sokoban.LoadContents();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Use the name of your sprite font file here instead of 'File'.
            font = Content.Load<SpriteFont>("File");
            font1 = Content.Load<SpriteFont>("File1");
            //player = Content.Load<Texture2D>("Character4");
            dot = Content.Load<Texture2D>("EndPoint_Yellow");
            box = Content.Load<Texture2D>("Crate_Gray");
            wall = Content.Load<Texture2D>("WallRound_Black");

            //player = new Texture2D[4];
            //player[(int) Direction.Down] = Content.Load<Texture2D>("Character4");
            //player[(int)Direction.Up] = Content.Load<Texture2D>("Character7");
            //player[(int)Direction.Left] = Content.Load<Texture2D>("Character1");
            //player[(int)Direction.Right] = Content.Load<Texture2D>("Character2");


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {


            // increment the timer according to the elapsed time between invocations to Update.
            //levelTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (!isWin) levelTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //if (Keyboard.GetState().IsKeyDown(Keys.R)) Initialize(); // Game restart

            //if (Keyboard.GetState().IsKeyDown(Keys.R)) Initialize(); // Game restart
            if (!rDown && Keyboard.GetState().IsKeyDown(Keys.R))
            {
                rDown = true;
                liveCount--;
                if (isWin ||liveCount < 0)
                {
                    // Reset level
                    currentLevel = 0;
                    levelTime = 0f;
                    liveCount = 3;
                    isWin = false;
                }
                Initialize(); // Game restart
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.R))
            {
                rDown = false;
            }

            if (Victory())
            {
                if (currentLevel < levelNames.Length - 1)
                {
                    currentLevel++;
                    Initialize();
                }
                else
                {
                    //Exit(); // FIXME: Win screen
                    isWin = true;
                }
            }
            //sokoban.Update(gameTime);
            if (!isWin) sokoban.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MediumPurple);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            _spriteBatch.Begin();
            //_spriteBatch.DrawString(font1, // Tipo de letra
            // "Tempo Decorrido = ", // Texto
            //new Vector2(5, level.GetLength(1) * tileSize + 5), // Posição do texto
            //Color.White, // Cor da letra
            //0f, //Rotação
            //Vector2.Zero, // Origem
            //2f, // Escala
            //SpriteEffects.FlipVertically, //Sprite effect (FlipHorizontally)
            //0); // Ordenar sprites
            _spriteBatch.DrawString(font, "O texto que quiser", new Vector2(0, 40), Color.Black);
            _spriteBatch.DrawString(font, $"Numero de Linhas = {nrLinhas}", new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(font, $"Numero de Colunas = {nrColunas}", new Vector2(0, 20), Color.Black);


            string lives = $"Lives: {liveCount}";
            Point measure = font.MeasureString(lives).ToPoint();
            int posX = level.GetLength(0) * tileSize - measure.X * 2 - 5;
            _spriteBatch.DrawString(font, // Tipo de Letra
            lives, // Texto
            new Vector2(posX, level.GetLength(1) * tileSize + 10), // Posição do texto
            Color.Coral, //Cor da Letra
            0f, //Rotação
            Vector2.Zero, // Origem
            2f, // Escala
            SpriteEffects.None, //FlipHorizontally, //Sprite effect
            0); // Ordenar sp
            Rectangle position = new Rectangle(0, 0, tileSize, tileSize); //calculo do retangulo a depender do tileSize


            if (isWin)
            {
                Vector2 windowSize = new Vector2(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);

                // Transparent Layer
                Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1); // Texture of 1 x 1 pixel
                pixel.SetData(new[] { Color.White }); // unique pixel is white
                _spriteBatch.Draw(pixel,
                new Rectangle(Point.Zero, windowSize.ToPoint()),
                new Color(Color.Green, 0.5f));

                // Draw Win Message
                string win = $"You took {levelTime:F1} seconds to Win!";
                Vector2 winMeasures = font1.MeasureString(win) / 2f;
                Vector2 windowCenter = windowSize / 2f;
                Vector2 pos = windowCenter - winMeasures;
                _spriteBatch.DrawString(font1, win, pos, Color.Coral, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            }
            for (int x = 0; x < level.GetLength(0); x++)  //pega a primeira dimensão
            {
                for (int y = 0; y < level.GetLength(1); y++) //pega a segunda dimensão
                {
                    position.X = x * tileSize; // define o position
                    position.Y = y * tileSize; // define o position
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

            sokoban.Draw(_spriteBatch);
            _spriteBatch.DrawString(font1, // Tipo de letra
$"Time: {levelTime:F2}", //string.Format("Time: {0:F2}", levelTime) // Texto
new Vector2(5, level.GetLength(1) * tileSize + 10), // Posição do texto
Color.White, // Cor da letra
0f, //Rotação
Vector2.Zero, // Origem
2f, // Escala
SpriteEffects.None, //FlipHorizontally, //Sprite effect
0); // Ordenar sprites
            //position.X = sokoban.Position.X * tileSize; //posição do Player
            //position.Y = sokoban.Position.Y * tileSize; //posição do Player
            //_spriteBatch.Draw(player[(int) direction], position, Color.White); //desenha o Player

            foreach (Point b in boxes)
            {
                position.X = b.X * tileSize;
                position.Y = b.Y * tileSize;
                _spriteBatch.Draw(box, position, Color.White);
            }

            _spriteBatch.End();

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
            if (level[x, y] == 'X') return false;  // se for uma parede está ocupada
            if (HasBox(x, y)) return false; // verifica se é uma caixa
            return true;

            /* The same as:    return level[x,y] != 'X' && !HasBox(x,y);   */
        }

        public bool Victory()
        {
            foreach (Point b in boxes) // pecorrer a lista das caixas
            {
                if (level[b.X, b.Y] != '.') return false; // verifica se há caixas sem pontos
            }
            return true;
        }
        void LoadLevel(string levelFile)
        {
            string[] linhas = File.ReadAllLines($"Content/{levelFile}");  // "Content/" + level
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
                        sokoban = new Player(this, x, y);
                        level[x, y] = ' '; // put a blank instead of the sokoban 'Y'
                    }
                    else
                    {
                        level[x, y] = linhas[y][x];
                    }
                }
            }
        }

    }
}