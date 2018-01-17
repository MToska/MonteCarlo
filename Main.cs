using System.Collections.Generic;
using System.Drawing;
using System.Collections;

namespace MultiscaleModelling1
{
    public class Main
    {
        public int X = 100;
        public int Y = 100;
        public int grain = 1;

        public int[][] initTable;
        public int[][] supportTable;
        public bool[][] tab;
        public int nucleons;
        public int method = 0;

        public ArrayList colours = new ArrayList();
        public List<int> ID = new List<int>();

        public bool MC;
        public int MCStepsNumber;

        public int SRXMethod;
        public int SRX_step;
        public int SRXNucleons;
        public bool SRXRandom;
        public int SRX_energy;
        public bool Homogenous;
        public int SRXNucleation;

        public int[][] energyTable;
        public int limit;

        public int buforX;
        public int buforY;
        public List<Color> energyColos;
    }
    
    
}
