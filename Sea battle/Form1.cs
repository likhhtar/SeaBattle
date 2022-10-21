using System.Media;

namespace Sea_battle;

public partial class Form1 : Form
{
    public const int mapSize = 10;
    public const int cellSize = 50;
    public string alphabet = "ABCDEFGHIJ";

    public int[,] firstPlayerMap = new int[mapSize, mapSize];
    public int[,] secondPlayerMap = new int[mapSize, mapSize];
    public bool[,] used = new bool[mapSize, mapSize];

    public Button[,] firstPlayerButtons = new Button[mapSize, mapSize];
    public Button[,] secondPlayerButtons = new Button[mapSize, mapSize];

    public bool isPlaying = false;
    public bool isMusicPlaying = false;

    public Bot bot;

    public Label error = new Label()
    {
        Size = new Size(1000, 200),
        Font = new Font("Arial", 19),
        Location = new Point(mapSize * cellSize - 400, mapSize * cellSize + 30),
        Text = "ERROR: the ships are not arranged according to the rules",
        ForeColor = Color.Red
    };
    
    public Label winFirst = new Label()
    {
        Size = new Size(1000, 200),
        Font = new Font("Arial", 40),
        Location = new Point(mapSize * cellSize - 190, mapSize * cellSize + 50),
        Text = "YOU WON!!!!",
        ForeColor = Color.Green
    };
    
    public Label winSecond = new Label()
    {
        Size = new Size(1000, 200),
        Font = new Font("Arial", 40),
        Location = new Point(mapSize * cellSize - 170, mapSize * cellSize + 50),
        Text = "YOU LOSE",
        ForeColor = Color.Red
    };

    public Button restartButton;
    public Button musicButton;
    System.Media.SoundPlayer music = new System.Media.SoundPlayer(@"C:\Users\User\source\repos\Sea battle\audio\main.wav");

    public Form1()
    {
        InitializeComponent();
        this.Text = "Sea Battle";
        Init();
    }

    public void Init()
    {
        isPlaying = false;
        CreateMaps();
        bot = new Bot(secondPlayerMap, firstPlayerMap, secondPlayerButtons, firstPlayerButtons);
        secondPlayerMap = bot.PlaceShips();
    }

    public void CreateMaps()
    {
        this.Width = mapSize * 2 * cellSize + 70;
        this.Height = (mapSize + 1) * cellSize + 150;
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                firstPlayerMap[i, j] = 0;

                Button button = new Button();
                button.Location = new Point(j * cellSize, i * cellSize);
                button.Size = new Size(cellSize, cellSize);
                button.BackColor = Color.White;
                if (j == 0 || i == 0)
                {
                    button.BackColor = Color.DarkGray;
                    if (j == 0 && i != 0) button.Text = alphabet[i].ToString();
                    if (i == 0 && j != 0) button.Text = j.ToString();
                }
                else button.Click += new EventHandler(PlaceShips);
                firstPlayerButtons[i, j] = button;
                this.Controls.Add(button);
            }
        }
        
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                secondPlayerMap[i, j] = 0;
                
                Button button = new Button();
                button.Location = new Point(550 + j * cellSize, i * cellSize);
                button.Size = new Size(cellSize, cellSize);
                button.BackColor = Color.White;
                if (j == 0 || i == 0)
                {
                    button.BackColor = Color.DarkGray;
                    if (j == 0 && i != 0) button.Text = alphabet[i].ToString();
                    if (i == 0 && j != 0) button.Text = j.ToString();
                }
                else
                {
                    button.Click += new EventHandler(ShootClick);
                }
                secondPlayerButtons[i, j] = button;
                this.Controls.Add(button);
            }
        }

        Label map1 = new Label();
        map1.Text = "Player's map";
        map1.Location = new Point(mapSize * cellSize / 2 - 60, mapSize * cellSize + 5);
        this.Controls.Add(map1);
        
        Label map2 = new Label();
        map2.Text = "Bot's map";
        map2.Location = new Point(mapSize * cellSize + 270, mapSize * cellSize + 5);
        this.Controls.Add(map2);

        Button startButton = new Button();
        startButton.Text = "Start";
        startButton.Location = new Point(0, mapSize * cellSize + 30);
        startButton.Size = new Size(cellSize + 10, cellSize / 2 + 5);
        startButton.BackColor = Color.Plum;
        startButton.Font = new Font("Arial", 10);
        startButton.Click += new EventHandler(Start);
        this.Controls.Add(startButton);

        musicButton = new Button();
        musicButton.BackColor = Color.Beige;
        musicButton.BackgroundImage = Image.FromFile("C:/Users/User/source/repos/Sea battle/images/soundoff2.png");  
        musicButton.Size = new Size(40, 35);
        musicButton.Location = new Point(mapSize * cellSize * 2, mapSize * cellSize + 30);
        musicButton.Click += new EventHandler(Music);
        this.Controls.Add(musicButton);
        
        restartButton = new Button()
        {
            Text = "Restart",
            Location = new Point(mapSize * cellSize - 70, mapSize * cellSize),
            Size = new Size(200, cellSize),
            Font = new Font("Arial", 20),
            BackColor = Color.Red,
            
        };
        restartButton.Click += new EventHandler(Restart);
    }
    
    public void Music(object sender, EventArgs e)
    {
        if (isMusicPlaying)
        {
            isMusicPlaying = false;
            musicButton.BackgroundImage = Image.FromFile("C:/Users/User/source/repos/Sea battle/images/soundoff2.png");
            music.Stop();
        }
        else
        {
            isMusicPlaying = true;
            musicButton.BackgroundImage = Image.FromFile("C:/Users/User/source/repos/Sea battle/images/sound2.png");
            music.PlayLooping();
        }
    }
    public void Start(object sender, EventArgs e)
    {
        if (CheckShips())
        {
            isPlaying = true;
            if(error != null) this.Controls.Remove(error);
        }
        else
        {
            this.Controls.Add(error);
        }
    }
    
    public void Restart(object sender, EventArgs e)
    {
        this.Controls.Clear();
        Init();
    }

    public bool CheckShips()
    {
        ClearUsed();
        int[] ships = new[] { 4, 3, 2, 1 };
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if(used[i, j]) continue;
                if (firstPlayerMap[i, j] == 1)
                {
                    int lengthShip = FindShips(i, j);
                    if (lengthShip == -1) return false;
                    ships[lengthShip - 1]--;
                    if (ships[lengthShip - 1] < 0)
                    {
                        Console.WriteLine(lengthShip);
                        return false;
                    }
                }
            }
        }

        for (int i = 0; i < 4; i++)
            if (ships[i] != 0)
            {
                Console.WriteLine(i);
                 return false;
            }
               
        return true;
    }

    public void ClearUsed()
    {
        for(int i = 0; i < mapSize; i++)
        for (int j = 0; j < mapSize; j++)
            used[i, j] = false;
    }

    public int FindShips(int x, int y)
    {
        int lengthShip = 1, directionX = 0, directionY = 0, i = x, j = y, result;
        if (IsInsideMap(x + 1, y) && firstPlayerMap[x + 1, y] == 1)
        {
            directionX++; i++;
            while (IsInsideMap(i, j) && firstPlayerMap[i, j] == 1)
            {
                lengthShip++;
                i++;
            }
        }
        else if (IsInsideMap(x - 1, y) && firstPlayerMap[x - 1, y] == 1)
        {
            directionX--; i--;
            while (IsInsideMap(i, j) && firstPlayerMap[i, j] == 1)
            {
                lengthShip++;
                i--;
            }
        }
        else if (IsInsideMap(x, y + 1) && firstPlayerMap[x, y + 1] == 1)
        {
            directionY++; j++;
            while (IsInsideMap(i, j) && firstPlayerMap[i, j] == 1)
            {
                lengthShip++;
                j++;
            }
        }
        else if (IsInsideMap(x, y - 1) && firstPlayerMap[x, y - 1] == 1)
        {
            directionY--; j--;
            while (IsInsideMap(i, j) && firstPlayerMap[i, j] == 1)
            {
                lengthShip++;
                j--;
            }
        }

        result = lengthShip;
        
        bool goodShip = true;
        if (directionY == 0)
        {
            int length = x + lengthShip < mapSize ? x + lengthShip : x + lengthShip - 1; 
            int width = y + 1 < mapSize ? y + 1 : y; 
            
            if(y >= 1) for (int k = x - 1 >= 0 ? x - 1 : 0; k <= length; k++)
                if (firstPlayerMap[k, y - 1] == 1)
                    goodShip = false;
            if(y < mapSize - 1) for (int k = x - 1 >= 0 ? x - 1 : 0; k <= length; k++)
                if (firstPlayerMap[k, y + 1] == 1)
                    goodShip = false;

            for(int l = x - 1 >= 0 ? x - 1 : 0; l <= length; l++)
            for (int k = y - 1 >= 0 ? y - 1 : 0; k <= width; k++)
                used[l, k] = true;
        }
        else
        {
            int length = y + lengthShip < mapSize ? y + lengthShip : y + lengthShip - 1; 
            int width = x + 1 < mapSize ? x + 1 : x; 

            if(x >= 1) for (int k = y - 1 >= 0 ? y - 1 : 0; k <= length; k++) 
                if (firstPlayerMap[x - 1, k] == 1)
                    goodShip = false;
            if(x < mapSize - 1) for (int k = y - 1 >= 0 ? y - 1 : 0; k <= length; k++)
                if (firstPlayerMap[x + 1, k] == 1)
                    goodShip = false;
            
            for(int l = x - 1 >= 0 ? x - 1 : 0; l <= width; l++)
            for (int k = y - 1 >= 0 ? y - 1 : 0; k <= length; k++)
                used[l, k] = true;
        }

        if (!goodShip) result = -1;
        return result;
    }
    
    public bool IsInsideMap(int i, int j)
    {
        if (i >= mapSize || i < 0 || j >= mapSize || j < 0) return false;
        return true;
    }
    
    public int CheckMapIsEmpty()
    {
        bool isEmpty1 = true;
        bool isEmpty2 = true;
        for (int i = 1; i < mapSize; i++)
        {
            for (int j = 1; j < mapSize; j++)
            {
                if (firstPlayerMap[i, j] != 0) isEmpty1 = false;
                if (secondPlayerMap[i, j] != 0) isEmpty2 = false;
            }
        }

        if (isEmpty1) return 1;
        if (isEmpty2) return 2;
        return 0;
    }
    public void PlaceShips(object sender, EventArgs e)
    {
        Button pressedButton = sender as Button;
        if (!isPlaying)
        {
            if (firstPlayerMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] == 0)
            {
                pressedButton.BackColor = Color.Firebrick;
                firstPlayerMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = 1;
            }
            else
            {
                pressedButton.BackColor = Color.White;
                firstPlayerMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = 0;
            }
        }
    }

    public void ShootClick(object sender, EventArgs e)
    {
        Button pressedButton = sender as Button;
        bool playerTurn = Shoot(secondPlayerMap, pressedButton);
        if (!playerTurn)
            bot.Shoot();

        int win = CheckMapIsEmpty();
        if (win == 1)
        {
            if(isMusicPlaying) music.Stop();
            WinSecond();
            if(isMusicPlaying) music.PlayLooping();
        }
        else if (win == 2)
        {
            if(isMusicPlaying) music.Stop();
            WinFirst();
            if(isMusicPlaying) music.PlayLooping();
        }
    }

    public void WinFirst()
    {
        this.Controls.Add(winFirst);
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\User\source\repos\Sea battle\audio\win.wav");
        if(isMusicPlaying) player.Play();
        this.Controls.Add(restartButton);
    }

    public void WinSecond()
    {
        this.Controls.Add(winSecond);
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"C:\Users\User\source\repos\Sea battle\audio\lost.wav");
        if(isMusicPlaying) player.Play();
        this.Controls.Add(restartButton);
    }
    
    public bool Shoot(int[,] map, Button pressedButton)
    {
        bool hit = false;
        if (isPlaying)
        {
            int delta = 0;
            if (pressedButton.Location.X > 550) delta = 550;
            if (map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta)/ cellSize] != 0)
            {
                hit = true;
                map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] = 0;
                pressedButton.BackColor = Color.Indigo;
                pressedButton.Text = "X";
            }
            else
            {
                hit = false;
                pressedButton.BackColor = Color.Black;
            }
        }
        return hit;
    }
}