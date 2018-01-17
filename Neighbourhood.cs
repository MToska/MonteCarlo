using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace MultiscaleModelling1
{
    public partial class Neighbourhood
    {
        Random rand;
        
        public Neighbourhood()
        {
            rand = new Random();
        }

        public List<int> GetNeighbours(int x, int y)
        {
            return MCvonNeuman(x, y);
        }

        private int getX(int x)
        {

            int ret = x % Core.Main.X;
            if (ret < 0)
                return 0;
            else if (x >= Core.Main.X)
                return Core.Main.X - 1;
            else
                return ret;

        }

        private int getY(int y)
        {

            int ret = y % Core.Main.Y;
            if (ret < 0)
                return 0;
            else if (y >= Core.Main.Y)
                return Core.Main.Y - 1;
            else
                return ret;
        }


        public void newTab()
        {
            Core.Main.initTable = new int[Core.Main.X][];
            Core.Main.supportTable = new int[Core.Main.X][];
            for (int i = 0; i < Core.Main.X; i++)
            {
                Core.Main.initTable[i] = new int[Core.Main.Y];
                Core.Main.supportTable[i] = new int[Core.Main.Y];
            }
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    Core.Main.initTable[i][j] = 0;
                    Core.Main.supportTable[i][j] = 0;
                }
            }
        }

        public void newLimTab()
        {
            Core.Main.tab = new bool[Core.Main.X][];
            for (int i = 0; i < Core.Main.X; i++)
            {
                Core.Main.tab[i] = new bool[Core.Main.Y];
            }

            for (int x = 0; x < Core.Main.X; x++)
            {
                for (int y = 0; y < Core.Main.Y; y++)
                {
                    if (Core.Main.ID.Contains(Core.Main.initTable[x][y]))
                        Core.Main.tab[x][y] = false;
                }
            }
        }

        private bool checking()
        {
            int changes = 0;
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    if (Core.Main.initTable[i][j] != Core.Main.supportTable[i][j])
                        changes++;
                }
            }
            return changes > 0;
        }

        public bool copying()
        {
            if (checking())
            {
                for (int i = 0; i < Core.Main.X; i++)
                {
                    for (int j = 0; j < Core.Main.Y; j++)
                    {
                        Core.Main.initTable[i][j] = Core.Main.supportTable[i][j];
                    }
                }
                return true;
            }
            else
                return false;
        }

        public void RandonNucl(int ile = -1)
        {
            int Ile = ile > -1 ? ile : Core.Main.nucleons;
            for (int i = 0; i < Ile; i++)
            {
                int x = rand.Next(0, Core.Main.X);
                int y = rand.Next(0, Core.Main.Y);
                if (Core.Main.initTable[x][y] == 0 || Core.Main.initTable[x][y] < Core.Main.limit)
                {
                    Core.Main.initTable[x][y] = Core.Main.grain;
                    Color newColor = Color.FromArgb(20, rand.Next(256), rand.Next(256));
                    Core.Main.colours.Add(newColor);
                    Core.Main.grain++;
                    if (ile > -1) Core.Main.energyTable[x][y] = 0;
                }
            }
        }

        public void RandonNSR(int howmuch = -1)
        {
            int howMuch = howmuch > -1 ? howmuch : Core.Main.SRXNucleation;
            for (int i = 0; i < howMuch; i++)
            {
                int x = rand.Next(0, Core.Main.X);
                int y = rand.Next(0, Core.Main.Y);
                if (Core.Main.initTable[x][y] == 0 || Core.Main.initTable[x][y] < Core.Main.limit)
                {
                    Core.Main.initTable[x][y] = Core.Main.grain;
                    Color newColor = Color.FromArgb(rand.Next(256), 0, 0);
                    Core.Main.colours.Add(newColor);
                    Core.Main.grain++;
                    if (howmuch > -1) Core.Main.energyTable[x][y] = 0;
                }
            }
        }

        public bool empty()
        {
            int ile = 0;
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    ile += Core.Main.initTable[i][j] == 0 ? 1 : 0;
                }
            }
            return ile > 0 ? true : false;
        }


        private int WinnerExt(List<int> neighbourhoods, int howMuch)
        {
            Dictionary<int, int> winner = new Dictionary<int, int>();
            foreach (int neighbourhood in neighbourhoods)
            {
                if (winner.Keys.Contains(neighbourhood))
                    winner[neighbourhood]++;
                else
                    winner.Add(neighbourhood, 1);
            }

            int common = 0;
            int maxV = 0;
            foreach (KeyValuePair<int, int> numer in winner)
            {
                if (!Core.Main.ID.Contains(numer.Key) && numer.Key > 0 && numer.Value > maxV)
                {
                    common = numer.Key;
                    maxV = numer.Value;
                }
            }
            if (maxV > howMuch)
                return common;
            else
                return 0;
        }

       

        private int Winner(List<int> neighbourhoods)
        {
            Dictionary<int, int> numerous = new Dictionary<int, int>();
            foreach (int neighbourhood in neighbourhoods)
            {
                if (numerous.Keys.Contains(neighbourhood))
                    numerous[neighbourhood]++;
                else
                    numerous.Add(neighbourhood, 1);
            }

            int common = 0;
            int maxV = 0;
            foreach (KeyValuePair<int, int> numer in numerous)
            {
                if (!Core.Main.ID.Contains(numer.Key) && numer.Key > 0 && numer.Value > maxV)
                {
                    common = numer.Key;
                    maxV = numer.Value;
                }
            }

            return common;
        }

        public void Neumann()
        {
            List<int> neighbourhoods;
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    if (Core.Main.initTable[i][j] == 0)
                    {
                        neighbourhoods = new List<int>();

                        neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j)]);
                        neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j)]);
                        neighbourhoods.Add(Core.Main.initTable[getX(i)][getY(j - 1)]);
                        neighbourhoods.Add(Core.Main.initTable[getX(i)][getY(j + 1)]);
                        Core.Main.supportTable[i][j] = Winner(neighbourhoods);
                    }
                    else
                        Core.Main.supportTable[i][j] = Core.Main.initTable[i][j];
                }
            }
        }


        public void ClearTable()
        {
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    if (!Core.Main.ID.Contains(Core.Main.initTable[i][j]))
                        Core.Main.initTable[i][j] = 0;
                }
            }
        }

        public void ChangeTableColor()
        {
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    if (!Core.Main.ID.Contains(Core.Main.initTable[i][j]))
                        Core.Main.initTable[i][j] = 0;
                    else
                        Core.Main.initTable[i][j] = Core.Main.ID[0];
                }
            }
        }


        private int Rule1(int i, int j)
        {
            List<int> neighbourhoods = new List<int>();

            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i)][getY(j + 1)]);

            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j + 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j + 1)]);

            return WinnerExt(neighbourhoods, 5);
        }

        private int Rule2(int i, int j)
        {
            List<int> neighbourhoods = new List<int>();

            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i)][getY(j + 1)]);

            return WinnerExt(neighbourhoods, 3);
        }

        private int Rule3(int i, int j)
        {
            List<int> neighbourhoods = new List<int>();

            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j + 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j + 1)]);

            return WinnerExt(neighbourhoods, 5);
        }

        private int Rule4(int i, int j)
        {
            List<int> neighbourhoods = new List<int>();

            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i)][getY(j + 1)]);

            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j + 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i + 1)][getY(j - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(i - 1)][getY(j + 1)]);

            int res = Winner(neighbourhoods);
            int n = rand.Next(0, 100);
            return res;
        }

        private List<int> MCvonNeuman(int x, int y)
        {
            List<int> neighbourhoods = new List<int>();

            neighbourhoods.Add(Core.Main.initTable[getX(x - 1)][getY(y)]);
            neighbourhoods.Add(Core.Main.initTable[getX(x + 1)][getY(y)]);
            neighbourhoods.Add(Core.Main.initTable[getX(x)][getY(y - 1)]);
            neighbourhoods.Add(Core.Main.initTable[getX(x)][getY(y + 1)]);

            return neighbourhoods;
        }




    }
}
 