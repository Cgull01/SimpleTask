using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace SimpleTask
{

    public partial class Form3 : Form
    {
        Form1 frm1;
        FlowLayoutPanel flp1;
        int buttonColorIndex;
        Button[] ColorButtons = new Button[9];
        Color newColor;


        public Form3(Form1 form1, FlowLayoutPanel flp)
        {
            InitializeComponent();

            if(VariablesBetweenForms.ViewOnly)
            {
                panel1.Visible = false;
                label2.Visible = false;
                colorPanel.Visible = false;
            }
            else
            {
                panel1.Visible = true;
                label2.Visible = true;
                colorPanel.Visible = true;
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

            richTextBox1.Text = VariablesBetweenForms.Title;
            richTextBox2.Text = VariablesBetweenForms.Text;

            newColor = VariablesBetweenForms.buttonColor;

            frm1 = form1;
            flp1 = flp;
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            if ((richTextBox1.Text != VariablesBetweenForms.Title || richTextBox2.Text != VariablesBetweenForms.Text) && !VariablesBetweenForms.ViewOnly)
            {
                label2.Visible = true;
                panel1.Visible = true;
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            richTextBox2.Font = new Font("Microsoft Sans Serif", trackBar1.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string buttonTitle = richTextBox1.Text,
                   buttonText = richTextBox2.Text;

            if (richTextBox1.Text.Length == 0)
                buttonTitle = " ";
            if (richTextBox2.Text.Length == 0)
                buttonText = " ";

            VariablesBetweenForms.buttonColor = newColor;
            VariablesBetweenForms.Title = buttonTitle;
            VariablesBetweenForms.Text = buttonText;

            Form1.ChangeButtonData(frm1, flp1);

            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
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

            richTextBox1.Text = VariablesBetweenForms.Title;
            richTextBox2.Text = VariablesBetweenForms.Text;

            label2.Visible = false;
            panel1.Visible = false;
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
    }
}
