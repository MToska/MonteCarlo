using System;
using System.Collections.Generic;
using System.Drawing;


namespace MultiscaleModelling1
{
    public partial class SRXMC
    {
        Neighbourhood neighbourhood = new Neighbourhood();
        Random rand;

        public SRXMC()
        {
            rand = new Random();
        }


        public void grains(int nucleons)
        {
            if (Core.Main.SRXRandom)
            {
                int nMax = 100;
                while (nucleons > 0)
                {
                    int x = rand.Next(Core.Main.X);
                    int y = rand.Next(Core.Main.Y);
                    if (Core.Main.tab[x][y] && Core.Main.initTable[x][y] < Core.Main.limit)
                    {
                        Core.Main.initTable[x][y] = Core.Main.grain;                        
                        Color newColor = Color.FromArgb(rand.Next(256), 0, 0);
                        Core.Main.colours.Add(newColor);
                        Core.Main.grain++;
                        Core.Main.energyTable[x][y] = 0;
                        nucleons--;
                        nMax = 50;
                    }
                    if (nMax == 0)
                        break;
                    nMax--;
                }
            }
            else
            {
                neighbourhood.RandonNSR(nucleons);
            }
        }

        public void SpreadEnergy()
        {
            InitTab();
            int energy = Core.Main.SRX_energy;
            if (Core.Main.Homogenous)
            {
                for (int i = 0; i < Core.Main.X; i++)
                {
                    for (int j = 0; j < Core.Main.Y; j++)
                    {
                        Core.Main.energyTable[i][j] = energy;
                    }
                }
            }
            else
            {
                neighbourhood.newLimTab();
                for (int i = 0; i < Core.Main.X; i++)
                {
                    for (int j = 0; j < Core.Main.Y; j++)
                    {
                        if (Core.Main.tab[i][j])
                            Core.Main.energyTable[i][j] = energy;
                        else
                            Core.Main.energyTable[i][j] = 1;
                    }
                }
            }
        }

        public void InitTab()
        {
            Core.Main.energyColos = new List<Color>();
            int mEnergy = Core.Main.SRX_energy;
            int newColor = 255 / (mEnergy + 1);
            int green = 0;
            int blue = 255;
            for (int i = 0; i <= mEnergy; i++)
            {
                Core.Main.energyColos.Add(Color.FromArgb(0, green, blue));
                green += newColor;
                blue -= newColor;
            }
            newEnergy();
        }

        private void newEnergy()
        {
            Core.Main.energyTable = new int[Core.Main.X][];

            for (int i = 0; i < Core.Main.X; i++)
            {
                Core.Main.energyTable[i] = new int[Core.Main.Y];
            }
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    Core.Main.energyTable[i][j] = 0;
                }
            }
        }

    }
}
