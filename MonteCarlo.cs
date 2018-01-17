using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MultiscaleModelling1
{
    public partial class MonteCarlo
    {
        Neighbourhood neighbourhood = new Neighbourhood();
        Random rand;

        public MonteCarlo()
        {
            rand = new Random();
        }


        private List<int> GetNeighbours(int x, int y)
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
            int @return = y % Core.Main.Y;
            if (@return < 0)
                return 0;
            else if (y >= Core.Main.Y)
                return Core.Main.Y - 1;
            else
                return @return;
        }


        private void MonteCarloStep()
        {
            while (anyLim())
            {
                int i = rand.Next(Core.Main.X);
                int j = rand.Next(Core.Main.Y);
                if (Core.Main.tab[i][j])
                {
                    List<int> neighbours = GetNeighbours(i, j);
                    List<int> limNeighbour = new List<int>(neighbours);
                    limNeighbour.Add(Core.Main.initTable[i][j]);
                    if (checkingMC(limNeighbour))
                    {
                        int energy = MCEnergy(neighbours, Core.Main.initTable[i][j]);
                        int newId = Core.Main.initTable[i][j];
                        while (newId == Core.Main.initTable[i][j] || Core.Main.ID.Contains(newId))
                            newId = neighbours[rand.Next(neighbours.Count)];
                        int newEnergy = MCEnergy(neighbours, newId);
                        if (newEnergy <= energy)
                            Core.Main.initTable[i][j] = newId;
                    }
                    Core.Main.tab[i][j] = false;
                }
            }
        }


        public void Monte_Carlo_Start()
        {
            Core.Main.MC = false;
            while (neighbourhood.empty())
            {
                Core.Main.MC = true;
                for (int x = 0; x < Core.Main.X; x++)
                {
                    for (int y = 0; y < Core.Main.Y; y++)
                    {
                        if (Core.Main.initTable[x][y] == 0)
                        {
                            int v = rand.Next(Core.Main.nucleons) + 1;
                            Core.Main.initTable[x][y] = v;
                            Color newColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                            if (Core.Main.colours.Count <= Core.Main.nucleons)
                            {
                                Core.Main.colours.Add(newColor);
                                Core.Main.grain++;
                            }
                        }
                    }
                }
            }
        }

        public void Monte_Carlo_StartBIS()
        {
            for (int i = 0; i < Core.Main.MCStepsNumber; i++)
            {
                newLimTab();
                MonteCarloStep();
            }

        }
      
        private void newLimTab()
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
                    else
                        Core.Main.tab[x][y] = true;
                }
            }
        }

        private bool anyLim()
        {
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    if (Core.Main.tab[i][j])
                        return true;
                }
            }
            return false;
        }

        public int MCEnergy(List<int> neighbours, int id)
        {
            int energy = 0;
            foreach (int nid in neighbours)
            {
                if (nid != id && nid > 0 && !(Core.Main.ID.Contains(nid) || Core.Main.ID.Contains(id)))
                    energy++;
            }
            return energy;
        }

        private bool checkingMC(List<int> neighbourhoods)
        {
            Dictionary<int, int> numerous = new Dictionary<int, int>();
            foreach (int neighbourhood in neighbourhoods)
            {
                if (neighbourhood < 0 || Core.Main.ID.Contains(neighbourhood))
                    continue;
                if (numerous.Keys.Contains(neighbourhood))
                    numerous[neighbourhood]++;
                else
                    numerous.Add(neighbourhood, 1);
            }
            return numerous.Count > 1;
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
