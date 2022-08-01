using System;
using System.Drawing;
using System.Windows.Forms;


namespace SimpleTask
{
    public partial class Form2 : Form
    {
        Form1 frm1;
        FlowLayoutPanel flp1;

        int buttonColorIndex;
        Button[] ColorButtons = new Button[9];
        Color newColor;

        public Form2(Image img, string windowName, Form1 form1, FlowLayoutPanel flp)
        {

            InitializeComponent();

            if(VariablesBetweenForms.ViewOnly)
            {
                button1.Visible = false;
            }
            else
            {
                button1.Visible = true;
            }
            //---------- Pass buttons to public array of buttons-----------

            Button[] A = { button13, button14, button15, button16, button17, button18, button19, button20, button21 };

            for (int i = 0; i < 9; i++)
            {
                ColorButtons[i] = A[i];
                ColorButtons[i].FlatAppearance.BorderColor = Color.Black;

                if (VariablesBetweenForms.buttonColor == ColorButtons[i].BackColor)
                {
                    buttonColorIndex = i;
                    A[i].FlatAppearance.BorderSize = 1;
                }
            }

            AddColorButtonActions();

            //---------------------------------------------------

            newColor = VariablesBetweenForms.buttonColor;

            this.Text = windowName;

            pictureBox1.Image = img;

            richTextBox1.Text = VariablesBetweenForms.Title;

            frm1 = form1;
            flp1 = flp;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

            if (richTextBox1.Text != VariablesBetweenForms.Title)
            {
                label2.Visible = true;
                panel1.Visible = true;
            }

        }

        private void AddColorButtonActions()
        {
            int lastpressedButtonIndex = buttonColorIndex;

            for (int i = 0; i < 9; i++)
            {
                int x = i;

                ColorButtons[x].Click += delegate
                {

                    buttonColorIndex = x;

                    ColorButtons[lastpressedButtonIndex].FlatAppearance.BorderSize = 0;

                    lastpressedButtonIndex = x;

                    ColorButtons[x].FlatAppearance.BorderSize = 1; // outline pressed color button

                    newColor = ColorButtons[x].BackColor;

                    label2.Visible = true;
                    panel1.Visible = true;

                };
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = VariablesBetweenForms.Title;

            int index = 0;
            for (int i = 0; i < 9; i++)
            {
                ColorButtons[i].FlatAppearance.BorderSize = 0;

                if (VariablesBetweenForms.buttonColor == ColorButtons[i].BackColor)
                {
                    index = i;
                }
            }

            buttonColorIndex = index;
            ColorButtons[index].FlatAppearance.BorderSize = 1;


            label2.Visible = false;
            panel1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string buttonTitle = richTextBox1.Text;

            if (richTextBox1.Text.Length == 0)
                buttonTitle = " ";

            VariablesBetweenForms.Title = buttonTitle;
            VariablesBetweenForms.buttonColor = newColor;

            Form1.ChangeButtonData(frm1, flp1);

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch(panel2.Size.Height)
            {
                case 0:
                    if(this.Width <= 934)
                    {
                        this.Width = 934;
                    }
                    panel2.Height = 68;
                    button1.Location = new Point(button1.Location.X, 68);
                    break;

                case 68:
                    panel2.Height = 0;
                    button1.Location = new Point(button1.Location.X, 0);
                    break;
            }
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            if(this.Width <= 934)
            {
                panel2.Height = 0;
                button1.Location = new Point(button1.Location.X, 0);
            }
            else
            {
                panel2.Height = 68;
                button1.Location = new Point(button1.Location.X, 68);
            }
        }
    }
}
