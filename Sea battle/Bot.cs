namespace Sea_battle;

public class Bot
{
    public int[,] firstPlayerMap = new int[Form1.mapSize, Form1.mapSize];//bot's map
    public int[,] secondPlayerMap = new int[Form1.mapSize, Form1.mapSize];//player's map

    public Button[,] firstPlayerButtons = new Button[Form1.mapSize, Form1.mapSize];
    public Button[,] secondPlayerButtons = new Button[Form1.mapSize, Form1.mapSize];

    public Bot(int[,] firstPlayerMap, int[,] secondPlayerMap, Button[,] firstPlayerButtons, Button[,] secondPlayerButtons)
    {
        this.firstPlayerButtons = firstPlayerButtons;
        this.secondPlayerButtons = secondPlayerButtons;
        this.firstPlayerMap = firstPlayerMap;
        this.secondPlayerMap = secondPlayerMap;
    }

    public bool IsInsideMap(int i, int j)
    {
        if (i >= Form1.mapSize || i < 0 || j >= Form1.mapSize || j < 0) return false;
        return true;
    }

    public bool IsEmpty(int i, int j, int lengthShip, int direction)
    {
        bool isEmpty = true;
        if (direction == 1)
        {
            int length = i + lengthShip < Form1.mapSize ? i + lengthShip : i + lengthShip - 1; 
            int width = j + 1 < Form1.mapSize ? j + 1 : j; 
            for(int l = i - 1 >= 0 ? i - 1 : 0; l <= length; l++)
            for (int k = j - 1 >= 0 ? j - 1 : 0; k <= width; k++)
                if (firstPlayerMap[l, k] != 0)
                {
                    isEmpty = false;
                    break;
                } 
        }
        else
        {
            int length = j + lengthShip < Form1.mapSize ? j + lengthShip : j + lengthShip - 1; 
            int width = i + 1 < Form1.mapSize ? i + 1 : i; 
            for(int l = i - 1 >= 0 ? i - 1 : 0; l <= width; l++)
            for (int k = j - 1 >= 0 ? j - 1 : 0; k <= length; k++)
                if (firstPlayerMap[l, k] != 0)
                {
                    isEmpty = false;
                    break;
                } 
        }
        
        return isEmpty;
    }
    
    public int[,] PlaceShips()
    {
        int lengthShip = 4;
        int numberShips = 4;
        int shipsCount = 10;
        int direction;
        Random r = new Random();

        int positionX = 0;
        int positionY = 0;

        while (shipsCount > 0)
        {
            for (int i = 0; i < 5 - lengthShip; i++)
            {
                direction = r.Next(0, 2);
                positionX = r.Next(0, Form1.mapSize);
                positionY = r.Next(0, Form1.mapSize);

                while ((direction == 0 && !IsInsideMap(positionX, positionY + lengthShip - 1)) ||
                       (direction == 1 && !IsInsideMap(positionX + lengthShip - 1, positionY)) ||
                       !IsEmpty(positionX, positionY, lengthShip, direction))
                {
                    direction = r.Next(0, 2);
                    positionX = r.Next(0, Form1.mapSize);
                    positionY = r.Next(0, Form1.mapSize);
                }

                if (direction == 0)
                {
                    for (int k = positionY; k < positionY + lengthShip; k++)
                    {
                         firstPlayerMap[positionX, k] = 1;
                    }
                }
                else
                {
                    for (int k = positionX; k < positionX + lengthShip; k++)
                    {
                        firstPlayerMap[k, positionY] = 1;
                    }
                }
                
                
                shipsCount--;
                if(shipsCount <= 0) break;
            }
            lengthShip--;
            
        }

        return firstPlayerMap;
    }
    
    public bool Shoot()
    {
        bool hit = false;
        int delta = 0;
        Random r = new Random();
        
        int positionX = r.Next(1, Form1.mapSize); 
        int positionY = r.Next(1, Form1.mapSize);

        while (secondPlayerButtons[positionX, positionY].BackColor == Color.Black || 
               secondPlayerButtons[positionX, positionY].BackColor == Color.Indigo)
        {
            positionX = r.Next(1, Form1.mapSize); 
            positionY = r.Next(1, Form1.mapSize);
        }
            
        
        if (secondPlayerMap[positionX, positionY] != 0)
        {
            hit = true;
            secondPlayerMap[positionX, positionY] = 0;
            secondPlayerButtons[positionX, positionY].BackColor = Color.Indigo;
            secondPlayerButtons[positionX, positionY].Text = "X";
        }
        else
        {
            hit = false;
            secondPlayerButtons[positionX, positionY].BackColor = Color.Black;
        }

        if (hit) Shoot();
        return hit;
    }
}