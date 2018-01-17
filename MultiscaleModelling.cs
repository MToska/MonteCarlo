using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace MultiscaleModelling1
{
    public partial class MultiscaleModelling : Form
    {
        Graphics g;
        Random rand;

        Neighbourhood neighbourhood = new Neighbourhood();
        MonteCarlo monte_carlo = new MonteCarlo();
        SRXMC srx = new SRXMC();

        public void Properties()
        {
            Core.Main.method = cb_Neighbourhood.SelectedIndex;
            Core.Main.nucleons = Convert.ToInt32(tb_Nucleation.Text);
            Core.Main.SRXNucleation = Convert.ToInt32(tb_SRXnucleation.Text);
            Core.Main.MCStepsNumber = Convert.ToInt32(tb_MC_steps.Text);
            Core.Main.SRX_step = Convert.ToInt32(tb_SRX_steps.Text);
            Core.Main.SRXNucleons = Convert.ToInt32(tb_Inc_nucleation.Text);
            Core.Main.SRXMethod = cb_SRX_method.SelectedIndex;
            Core.Main.SRXRandom = cb_Ganywhere.Checked ? true : false;
            Core.Main.Homogenous = cb_HGenous.Checked ? true : false;
        }


        public MultiscaleModelling()
        {
            InitializeComponent();
            Properties();
            neighbourhood.newTab();
            rand = new Random();

            g = VisualizationPictureBox.CreateGraphics();
            g.Clear(Color.Gray);
            VisualizationPictureBox.MouseClick += panel_MouseClick;
            Core.Main.colours.Add(Color.White);
            
            cb_Neighbourhood.SelectedIndex = 0;
            cb_SRX_method.SelectedIndex = 0;
            srx.InitTab();
        }

        private void draw()
        {
            int size = Core.Main.X > Core.Main.Y ? Core.Main.X : Core.Main.Y;
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    SolidBrush brush = new SolidBrush(Core.Main.initTable[i][j] == 0 ? Color.White : Core.Main.initTable[i][j] == -1 ? Color.Black : (Color)Core.Main.colours[Core.Main.initTable[i][j]]);
                    Rectangle rect = new Rectangle(i * VisualizationPictureBox.Width / size, j * VisualizationPictureBox.Height / size, VisualizationPictureBox.Width / size, VisualizationPictureBox.Height / size);
                    g.FillRectangle(brush, rect);
                }
            }
        }

        public void run()
        {
            bool mapDifferences = true;

            while (neighbourhood.empty() && mapDifferences)
            {

                neighbourhood.Neumann(); 
                mapDifferences = neighbourhood.copying();
            }
            
            draw();
            Core.Main.limit = Core.Main.grain;
        }


        void panel_MouseClick(object sender, MouseEventArgs e)
        {
            int size =Core.Main.X >Core.Main.Y ?Core.Main.X :Core.Main.Y;
            int x = e.X / (VisualizationPictureBox.Width / size);
            int y = e.Y / (VisualizationPictureBox.Height / size);

            Core.Main.buforX = x;
            Core.Main.buforY = y;

            if (x >=Core.Main.X || y >=Core.Main.Y)
                return;
            if (Core.Main.initTable[x][y] == 0)
            {
                Core.Main.initTable[x][y] = Core.Main.grain;
                Color newColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                Core.Main.colours.Add(newColor);
                draw();
                Core.Main.grain++;
            }
            else if(substructureCheck || dualphaseCheck)
            {
                Core.Main.ID.Add(Core.Main.initTable[x][y]);
            }
        }

        private void DrawEnergy()
        {
            int size = Core.Main.X > Core.Main.Y ? Core.Main.X : Core.Main.Y;
            for (int i = 0; i < Core.Main.X; i++)
            {
                for (int j = 0; j < Core.Main.Y; j++)
                {
                    SolidBrush brush = new SolidBrush(Core.Main.energyColos[Core.Main.energyTable[i][j]]);
                    Rectangle rect = new Rectangle(i * VisualizationPictureBox.Width / size, j * VisualizationPictureBox.Height / size, VisualizationPictureBox.Width / size, VisualizationPictureBox.Height / size);
                    g.FillRectangle(brush, rect);
                }
            }
        }

        private void SRXMCStep(int nucleons)
        {
            srx.grains(nucleons);
                int x = rand.Next(Core.Main.X);
                int y = rand.Next(Core.Main.Y);

                List<int> neighbours = neighbourhood.GetNeighbours(x, y);
                List<int> neighboursToCheck = new List<int>(neighbours);
                neighboursToCheck.Add(Core.Main.initTable[x][y]);
                if (neighboursToCheck.Any(a => a != neighboursToCheck[0]))
                {
                    int newID = Core.Main.initTable[x][y];
                    while (newID == Core.Main.initTable[x][y])
                        newID = neighbours[rand.Next(neighbours.Count)];
                    if (newID >= Core.Main.limit && Core.Main.initTable[x][y] < Core.Main.limit)
                    {
                        int energyBefore = monte_carlo.MCEnergy(neighbours, Core.Main.initTable[x][y]) + Core.Main.energyTable[x][y];
                        int energyAfter = monte_carlo.MCEnergy(neighbours, newID);
                        if (energyAfter <= energyBefore)
                        {
                            Core.Main.initTable[x][y] = newID;
                            Core.Main.energyTable[x][y] = 0;
                        }
                    }
                }
            
            draw();
            Thread.Sleep(500);
        }

        private void Click_Start(object sender, EventArgs e) 
        {
            Properties();
            run();
        }

        private void Click_Clear(object sender, EventArgs e) 
        {
            Properties();
            neighbourhood.newTab();
            g.Clear(Color.White);
        }

        private void Click_Random(object sender, EventArgs e) 
        {
            Properties();
            neighbourhood.RandonNucl();
            draw();
        }


        bool substructureCheck = false;
        private void Click_Substructure(object sender, EventArgs e) 
        {
            if (substructureCheck)
            {
                substructureCheck = false;
                neighbourhood.ClearTable();
                draw();
            }
            else
            {
                substructureCheck = true;
                Core.Main.ID.Clear();
            }
        }

        bool dualphaseCheck = false;
        private void Click_Dualphase(object sender, EventArgs e)  
        {
            if (dualphaseCheck)
            {
                dualphaseCheck = false;
                neighbourhood.ChangeTableColor();
                draw();
            }
            else
            {
                dualphaseCheck = true;
                Core.Main.ID.Clear();
            }
        }

        private void Click_MonteCarlo(object sender, EventArgs e) 
        {
            Properties();
            monte_carlo.Monte_Carlo_Start();
            draw();
            if (Core.Main.MC && Core.Main.MCStepsNumber == 1)
                return;
            monte_carlo.Monte_Carlo_StartBIS();
            draw();
        }

        private void Click_SRXMC(object sender, EventArgs e)
        {
            Properties();
            neighbourhood.newLimTab();
            for (int i = 0; i < Core.Main.SRX_step; i++)
            {
                SRXMCStep(Core.Main.SRXNucleation);
                switch (Core.Main.SRXMethod)
                {
                    case 0:
                        break;
                    case 1:
                        Core.Main.SRXNucleation += Core.Main.SRXNucleons;
                        break;
                    case 2:
                        Core.Main.SRXNucleation = Math.Max(0, Core.Main.SRXNucleation - Core.Main.SRXNucleons);
                        break;
                    case 3:
                        Core.Main.SRXNucleation = 0;
                        break;
                }
            }
            draw();

        }

        private void Click_Energy(object sender, EventArgs e) 
        {
            srx.SpreadEnergy();
        }

        private void energyChanged(object sender, EventArgs e) 
        {
            if (cb_toggle_energy.Checked)
            {
                DrawEnergy();
            }
            else
            {
                draw();
            }
        }

        private void grainsNum_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void MultiscaleModelling_Load(object sender, EventArgs e)
        {

        }

        private void SRXMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void VisualizationPictureBox_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SRXSteps_TextChanged(object sender, EventArgs e)
        {

        }

        private void Nucleation_TextChanged(object sender, EventArgs e)
        {

        }

        private void after_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Neighbourhood_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SRXEnergy_TextChanged(object sender, EventArgs e)
        {

        }

        private void tb_Inc_nucleation_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
