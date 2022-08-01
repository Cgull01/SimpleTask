using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SimpleTask
{

    public partial class Form1 : Form
    {

        FlowLayoutPanel MainPanel = new FlowLayoutPanel();

        int LastClosedPanelIndex = new int(); // Last closed panel index

        static List<group> Groups = new List<group>(); // list full of button data

        group currentGroup = new group(); // currently focused group

        group lastGroup = new group(); // last focused group

        int buttonType = 0; // new button type (text, link, image)

        int lastpressedButtonIndex = 0;

        int buttonColorIndex; // new button color

        int[] defaultColorIndex = new int[3]; // text-link-image buttons default color index

        bool IsAddingButton = false; // true - sidebutton is being created, 
                                     // false - sidebutton is being edited.

        List<int> ButtonsToDelete = new List<int>(); // List of buttons ready to delete

        string ImageLocation = AppDomain.CurrentDomain.BaseDirectory + "Images/";

        List<Color> ButtonColor = new List<Color> // Selection of button colors
        {
            Color.LightGray,
            Color.Silver,
            Color.Gray,
            Color.CornflowerBlue,
            Color.FromArgb(255, 128, 128),
            Color.PaleGreen,
            Color.LightSkyBlue,
            Color.Plum,
            Color.Khaki
        };

        Button[] ColorButtons = new Button[9];

        Size buttonSize = new Size();

        public Form1()
        {

            InitializeComponent();

            //---------- Setup MainPanel ----------------------------------
            MainPanel.AutoScroll = true;
            MainPanel.BackColor = System.Drawing.Color.FromArgb(44,47,53);
            MainPanel.Location = new System.Drawing.Point(202, 33);
            MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            MainPanel.BringToFront();
            MainPanel.ForeColor = System.Drawing.Color.White;
            MainPanel.Name = "MainPanel";
            MainPanel.Size = new System.Drawing.Size(666, 409);
            MainPanel.TabIndex = 10;
            this.Controls.Add(MainPanel);
            //---------- Pass buttons to public array of buttons-----------

            Button[] A = { BTNcolorLightGray, BTNcolorGray, BTNcolorDarkGray, BTNcolorBlue, BTNcolorRed, BTNcolorLme, BTNcolorLightBlue, BTNcolorPink, BTNcolorLightYellow };

            for (int i = 0; i < 9; i++)
                ColorButtons[i] = A[i];

            AddColorButtonActions();


            //---------------------------------------------------
            InOut.ReadFromJSON(Groups, ButtonColor);

            bool[] searchSettings = new bool[8];

            InOut.ReadSettingsFromTextFile(ref defaultColorIndex, ref searchSettings, ref LastClosedPanelIndex, ref buttonSize);

            NumUpDownWidth.Value = buttonSize.Width;
            NumUpDownHeight.Value = buttonSize.Height;

            BTNexampleSize.Size = buttonSize;

            if (LastClosedPanelIndex >= Groups.Count)
                LastClosedPanelIndex = 0;

            toolStripMenuItem8.Checked = searchSettings[0];
            toolStripMenuItem7.Checked = searchSettings[1];
            toolStripMenuItem1.Checked = searchSettings[2];
            toolStripMenuItem2.Checked = searchSettings[3];
            toolStripMenuItem4.Checked = searchSettings[4];
            toolStripMenuItem5.Checked = searchSettings[5];
            toolStripMenuItem6.Checked = searchSettings[6];
            toolStripMenuItem9.Checked = searchSettings[7];

            ShowMostClickedButtons();

            if (Groups.Count == 0)
            {
                group newgroup = new group();

                Groups.Add(newgroup);

                Groups[0].GroupName = "Group1";

                CreateSideButton(Groups[0].GroupName, 0);

                CreatePlusButton();

                currentGroup = Groups[0];
                MainPanel.Visible = true;
                currentGroup.SideButton.BackColor = Color.FromArgb(18, 99, 163); // color current group button

            }
            else
            {

                for (int j = 0; j < Groups.Count; j++)
                {
                    CreateSideButton(Groups[j].GroupName, j);
                }

                currentGroup = Groups[LastClosedPanelIndex]; // focus on current group of buttons

                for (int i = 0; i < currentGroup.DataButtons.Count; i++)
                {
                    CreateButton(currentGroup.DataButtons[i], currentGroup, MainPanel);
                }

                CreatePlusButton();

                MainPanel.Visible = true; // Show current panel
                MainPanel.BringToFront();

                currentGroup.SideButton.BackColor = Color.FromArgb(18, 99, 163); // color current group button
            }

        }

        private void ShowMostClickedButtons()
        {
            List<DataButton> SortedButtons = new List<DataButton>();

            foreach (group g in Groups)
            {
                for (int i = 0; i < g.DataButtons.Count; i++)
                {
                    SortedButtons.Add(g.DataButtons[i]);
                }
            }

            SortedButtons.Sort((a, b) => b.ClickCount.CompareTo(a.ClickCount));

            group SortedButtonsGroup = new group();

            FLPsuggestedElements.Controls.Clear();

            for (int i = 0; i < SortedButtons.Count; i++)
            {
                CreateButton(SortedButtons[i], SortedButtonsGroup, FLPsuggestedElements);
            }


            foreach (Button btn in FLPsuggestedElements.Controls)
            {
                btn.MouseDown += (object sender, MouseEventArgs e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        VariablesBetweenForms.ViewOnly = true;
                    }

                };

            }
        }

        private void BTNsideButtonsPlus_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < Groups.Count; i++)
            {
                Groups[i].SideButton.Enabled = false;
            }

            IsAddingButton = true;

            ShowControls(PanelcreateSidebuttonYESNO, TXTBoxnewSidebutton);
            HideControls(BTNsideButtonsPlus);

            PanelcreateSidebuttonYESNO.BackColor = Color.FromArgb(54, 57, 63);

            FLPsideButtons.Controls.Add(TXTBoxnewSidebutton);

            TXTBoxnewSidebutton.Text = "Group" + (Groups.Count + 1).ToString();

            var index = FLPsideButtons.Controls.Count - 2;
            FLPsideButtons.Controls.SetChildIndex(TXTBoxnewSidebutton, index);
            FLPsideButtons.Controls.SetChildIndex(PanelcreateSidebuttonYESNO, index + 1);

            FLPsideButtons.HorizontalScroll.Value = FLPsideButtons.HorizontalScroll.Maximum;



        }

        private void BTNcreateSidebuttonYES_Click(object sender, EventArgs e)
        {
            PanelcreateSidebuttonYESNO.Visible = false;
            BTNsideButtonsPlus.Visible = true;
            BTNsideButtonsPlus.Enabled = true;
            TXTBoxnewSidebutton.Visible = false;

            for (int i = 0; i < Groups.Count; i++)
            {
                Groups[i].SideButton.Enabled = true;
            }

            if (IsAddingButton)
            {

                string ButtonName = TXTBoxnewSidebutton.Text.ToString();

                group newgroup = new group();

                newgroup.GroupName = ButtonName;

                newgroup.Notes = "";

                Groups.Add(newgroup);

                CreateSideButton(ButtonName, Groups.Count - 1);

                currentGroup.SideButton.BackColor = Color.FromArgb(33, 150, 243);

                currentGroup = Groups[Groups.Count-1];

                RefreshButtons();

            }
            else
            {
                string newName = TXTBoxnewSidebutton.Text;

                lastGroup.SideButton.Visible = true;
                lastGroup.SideButton.Text = newName;

                lastGroup.GroupName = newName;

                currentGroup.SideButton.Visible = true;
            }


        }

        private void BTNcreateSidebuttonNO_Click(object sender, EventArgs e)
        {
            PanelcreateSidebuttonYESNO.Visible = false;
            BTNsideButtonsPlus.Enabled = true;
            TXTBoxnewSidebutton.Visible = false;

            for (int i = 0; i < Groups.Count; i++)
            {
                Groups[i].SideButton.Enabled = true;
            }

            if (IsAddingButton)
            {

                BTNsideButtonsPlus.Visible = true;
            }
            else
            {
                BTNsideButtonsPlus.Visible = true;

                foreach (group g in Groups)
                    g.SideButton.Visible = true;

            }
        }

        private void BTNdeleteElements_Click(object sender, EventArgs e)
        {
            
            ShowInformationPanel("Select elements you want to delete", false);

            TXTboxSearchBar.Text = "";
            TXTboxSearchBar.Enabled = false;

            FLPelementMoving.Visible = true;
            FLPelementMoving.BackColor = Color.FromArgb(41, 44, 48);
            FLPelementMoving.Controls.Clear();

            ButtonsToDelete.Clear();

            HideControls(PanelAddElement, BTNdeleteElements, BTNmoveOutside, BTNmoveInside, LabelmoveElements, MainPanel, FLPsuggestedElements, LabelsuggestedElements);

            FLPelementMoving.BringToFront();

            for (int i = 0; i < currentGroup.DataButtons.Count; i++)
            {
                Button newButton = new Button();

                newButton.Text = currentGroup.DataButtons[i].Title;
                newButton.ForeColor = Color.Black;
                newButton.Size = buttonSize;

                newButton.BackColor = currentGroup.DataButtons[i].Color;
                newButton.FlatStyle = FlatStyle.Flat;
                newButton.FlatAppearance.BorderSize = 2;
                newButton.FlatAppearance.BorderColor = currentGroup.DataButtons[i].Color;

                if (currentGroup.DataButtons[i].Type == "image" && !File.Exists(ImageLocation + currentGroup.DataButtons[i].Text))
                {
                    newButton.BackColor = Color.FromArgb(64, 64, 64);
                    newButton.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
                }

                int x = i;

                newButton.Click += delegate
                {
                    if (!ButtonsToDelete.Contains(x))
                    {
                        ButtonsToDelete.Add(x);
                        newButton.BackColor = Color.FromArgb(100, 100, 100);
                        newButton.ForeColor = Color.DarkGray;
                        newButton.FlatAppearance.BorderSize = 3;
                    }
                    else
                    {
                        ButtonsToDelete.Remove(x);
                        newButton.BackColor = currentGroup.DataButtons[x].Color;
                        newButton.ForeColor = Color.Black;
                        newButton.FlatAppearance.BorderSize = 0;

                    }
                };

                FLPelementMoving.Controls.Add(newButton);
            }
        }
        
        private void BTNtextTypeElement_Click(object sender, EventArgs e)
        {
            LabelnewElementText.Text = "Text";
            ChooseButtonType(0);
        }

        private void BTNlinkTypeElement_Click(object sender, EventArgs e)
        {
            LabelnewElementText.Text = "Link";
            ChooseButtonType(1);
        }

        private void BTNimageTypeElement_Click(object sender, EventArgs e)
        {
            LabelnewElementText.Text = "Image";
            ChooseButtonType(2);

        }

        private void BTNcreateElementCancel_Click(object sender, EventArgs e)
        {
            PanelAddElement.Visible = false;
        }

        private void BTNcreateElementAdd_Click(object sender, EventArgs e)
        {
            string buttonText = RTBnewElementText.Text + "\n";

            bool error = false;

            DataButton dataButton = new DataButton();
            dataButton.Color = ButtonColor[buttonColorIndex];

            switch (buttonType)
            {
                case 0: // text

                    if (RTBnewElementText.Text.Length > 0 && RTBnewElementName.Text.Length > 0)
                    {
                        dataButton.Type = "text";
                        dataButton.Title = RTBnewElementName.Text;
                        dataButton.Text = buttonText;

                        currentGroup.DataButtons.Add(dataButton);
                    }
                    else
                    {
                        ShowInformationPanel("Error: text box is empty", true);
                        error = true;

                    }

                    break;

                case 1: // link

                    if (RTBnewElementText.Text.Length > 0 && RTBnewElementName.Text.Length > 0)
                    {

                        dataButton.Type = "link";
                        dataButton.Title = RTBnewElementName.Text;


                        /*if (!buttonText.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) &&
                            !buttonText.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) &&
                            !buttonText.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
                        {
                            buttonText = "http://" + buttonText;
                        }
*/
                        dataButton.Text = buttonText;

                        currentGroup.DataButtons.Add(dataButton);

                    }
                    else
                    {
                        ShowInformationPanel("Error: text box is empty", true);
                        error = true;

                    }

                    break;

                case 2: // image

                    if (pictureBoxnewElement.Image != null && pictureBoxnewElement.Image != pictureBoxnewElement.InitialImage)
                    {
                        if (RTBnewElementName.Text.Length > 0)
                        {
                            Random rnd = new Random();
                            string imageName = "";
                            for (int i = 0; i < 10; i++)
                            {
                                imageName += rnd.Next(0, 15);
                            }
                            imageName += ".png";

                            dataButton.Type = "image";
                            dataButton.Title = RTBnewElementName.Text;
                            dataButton.Text = imageName;

                            currentGroup.DataButtons.Add(dataButton);

                            pictureBoxnewElement.Image.Save(ImageLocation + imageName, ImageFormat.Png);
                        }
                        else
                        {
                            ShowInformationPanel("Error: text box is empty", true);
                            error = true;
                        }


                    }
                    else
                    {
                        ShowInformationPanel("Error: no picture loaded", true);
                        error = true;
                    }

                    break;

                default:
                    ShowInformationPanel("Error: type not selected", true);

                    error = true;
                    break;
            }

            if (!error)
            {
                PanelAddElement.Visible = false;

                var PlusButton = MainPanel.Controls[MainPanel.Controls.Count - 1];

                CreateButton(dataButton, currentGroup, MainPanel);

                MainPanel.Controls.Add(PlusButton);
            }

        }

        private void BTNdeleteElementsNO_Click(object sender, EventArgs e)
        {
            CloseInformationPanel(sender, e);

            ShowControls(BTNmoveOutside, BTNmoveInside, LabelmoveElements, BTNdeleteElements, MainPanel, FLPsuggestedElements, LabelsuggestedElements);
            HideControls(FLPelementMoving);

            TXTboxSearchBar.Enabled = true;
        }

        private void BTNdeleteElementsYES_Click(object sender, EventArgs e)
        {
            CloseInformationPanel(sender, e);

            ShowControls(BTNmoveOutside, BTNmoveInside, LabelmoveElements, BTNdeleteElements, MainPanel, FLPsuggestedElements, LabelsuggestedElements);
            HideControls(FLPelementMoving);

            TXTboxSearchBar.Enabled = true;

            SortList(ButtonsToDelete);

            foreach (int x in ButtonsToDelete)
            {
                if (currentGroup.DataButtons[x].Type == "image")
                {
                    System.IO.File.Delete(ImageLocation + currentGroup.DataButtons[x].Text);
                }

                MainPanel.Controls.RemoveAt(x);

                currentGroup.DataButtons.RemoveAt(x);
            }

            ShowMostClickedButtons();

        }

        private void BTNgroupNotes_Click(object sender, EventArgs e)
        {
            RTBnotes.Text = currentGroup.Notes;
            //panel4.BringToFront();

            if (!PanelNotes.Visible)
            {
                Labelnotes.Text = "'" + currentGroup.GroupName + "' Notes";
                PanelNotes.Width = 300;
                BTNgroupNotes.Text = ">";
                PanelNotes.Visible = true;
                PanelNotes.BringToFront();


            }
            else
            {
                PanelNotes.Width = 0;
                BTNgroupNotes.Text = "<";
                PanelNotes.Visible = false;

            }
        }

        private void BTNsearchFilter_Click(object sender, EventArgs e)
        {
            switch (contextMenuStrip1.Visible)
            {
                case true:
                    contextMenuStrip1.Hide();
                    break;

                case false:
                    contextMenuStrip1.Show(BTNsearchFilter.PointToScreen(new Point(0, BTNsearchFilter.Height)));
                    break;
            }

        }

        private void BTNclearSearch_Click(object sender, EventArgs e)
        {
            TXTboxSearchBar.Text = "";
        }

        private void BTNsettings_Click(object sender, EventArgs e)
        {

            CloseInformationPanel(sender, e);

            switch (PanelSettings.Visible)
            {
                case true:
                    PanelSettings.Visible = false;
                    RefreshButtons();
                    break;

                case false:

                    PanelSettings.Visible = true;
                    PanelSettings.BringToFront();
                    break;
            }
        }

        private void BTNmoveOutside_Click(object sender, EventArgs e)
        {
            if (BTNmoveOutside.BackColor == Color.FromArgb(18, 99, 163))
            {
                CloseInformationPanel(sender, e);

                BTNmoveOutside.BackColor = Color.FromArgb(33, 150, 243);

                RefreshButtons();

                ShowControls(MainPanel, BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, FLPsideButtons, FLPsuggestedElements, LabelsuggestedElements);
                HideControls(FLPelementMoving, FLPelementMovingSidebuttons);

                MainPanel.BringToFront();

                TXTboxSearchBar.Enabled = true;

            }
            else
            {

                ShowInformationPanel("1) Select elements       2) Select a group to move elements", false);

                HideControls(PanelAddElement, BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, FLPsideButtons, MainPanel, FLPsuggestedElements, LabelsuggestedElements);

                BTNmoveOutside.BackColor = Color.FromArgb(18, 99, 163);
                BTNmoveInside.BackColor = Color.FromArgb(33, 150, 243);
                FLPelementMoving.BackColor = Color.FromArgb(46, 46, 65);

                ShowControls(FLPelementMoving, FLPelementMovingSidebuttons);

                FLPelementMoving.Controls.Clear();
                FLPelementMovingSidebuttons.Controls.Clear();

                List<int> ButtonsToMove = new List<int>();

                for (int i = 0; i < Groups.Count; i++)
                {
                    Button groupButton = new Button();

                    groupButton.Margin = new Padding(30, 10, 16, 0);
                    groupButton.Text = Groups[i].GroupName;
                    groupButton.ForeColor = Color.White;
                    groupButton.BackColor = Color.FromArgb(33, 150, 243);
                    groupButton.Font = new Font("Century Gothic", 12, FontStyle.Bold);
                    groupButton.Size = new Size(137, 38);
                    groupButton.FlatStyle = FlatStyle.Popup;

                    if (currentGroup == Groups[i])
                        groupButton.Enabled = false;

                    int x = i;
                    groupButton.Click += delegate
                    {
                        CloseInformationPanel(sender, e);
                        ShowInformationPanel(String.Format("Elements succesfully transfered to '{0}'",currentGroup.GroupName), false);
                        if (ButtonsToMove.Count > 0)
                        {
                            for (int j = 0; j < ButtonsToMove.Count; j++)
                            {
                                Groups[x].DataButtons.Add(currentGroup.DataButtons[ButtonsToMove[j]]);
                            }

                            SortList(ButtonsToMove);

                            for (int j = 0; j < ButtonsToMove.Count; j++)
                            {
                                currentGroup.DataButtons.RemoveAt(ButtonsToMove[j]);
                            }
                        }

                        ShowControls(MainPanel, FLPsideButtons, BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, FLPsuggestedElements, LabelsuggestedElements);
                        HideControls(FLPelementMovingSidebuttons, FLPelementMoving);

                        RefreshButtons();

                        BTNmoveOutside.BackColor = Color.FromArgb(33, 150, 243);

                        ButtonsToMove.Clear();

                    };
                    FLPelementMovingSidebuttons.Controls.Add(groupButton);

                    TXTboxSearchBar.Enabled = false;

                }
                FLPelementMovingSidebuttons.BringToFront();


                for (int i = 0; i < currentGroup.DataButtons.Count; i++)
                {
                    Button newButton = new Button();

                    newButton.Text = currentGroup.DataButtons[i].Title;
                    newButton.ForeColor = Color.Black;
                    newButton.Size = buttonSize;

                    newButton.BackColor = currentGroup.DataButtons[i].Color;
                    newButton.FlatStyle = FlatStyle.Flat;
                    newButton.FlatAppearance.BorderSize = 2;
                    newButton.FlatAppearance.BorderColor = currentGroup.DataButtons[i].Color;

                    if (currentGroup.DataButtons[i].Type == "image" && !File.Exists(ImageLocation + currentGroup.DataButtons[i].Text))
                    {
                        newButton.BackColor = Color.FromArgb(64, 64, 64);
                        newButton.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
                    }

                    int x = i;
                    newButton.Click += delegate
                    {
                        if (!ButtonsToMove.Contains(x))
                        {
                            ButtonsToMove.Add(x);
                            newButton.BackColor = Color.FromArgb(100, 100, 100);
                            newButton.ForeColor = Color.DarkGray;
                            newButton.FlatAppearance.BorderSize = 3;
                        }
                        else
                        {
                            ButtonsToMove.Remove(x);
                            newButton.BackColor = currentGroup.DataButtons[x].Color;
                            newButton.ForeColor = Color.Black;
                            newButton.FlatAppearance.BorderSize = 0;

                        }
                    };

                    FLPelementMoving.Controls.Add(newButton);
                }



            }
        }

        private void BTNmoveInside_Click(object sender, EventArgs e)
        {
            if (BTNmoveInside.BackColor == Color.FromArgb(18, 99, 163))
            {
                CloseInformationPanel(sender, e);

                BTNmoveInside.BackColor = Color.FromArgb(33, 150, 243);

                RefreshButtons();

                ShowControls(MainPanel, BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, FLPsuggestedElements, LabelsuggestedElements);

                TXTboxSearchBar.Enabled = true;

                HideControls(FLPelementMoving);

            }
            else
            {
                ShowInformationPanel("Click on element to move it", false);

                HideControls(FLPelementMovingSidebuttons, PanelAddElement, BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, MainPanel, FLPsuggestedElements, LabelsuggestedElements);

                FLPelementMovingSidebuttons.Visible = false;
                FLPsideButtons.Visible = true;

                BTNmoveOutside.BackColor = Color.FromArgb(33, 150, 243);
                BTNmoveInside.BackColor = Color.FromArgb(18, 99, 163);

                TXTboxSearchBar.Text = "";
                TXTboxSearchBar.Enabled = false;

                FLPelementMoving.Visible = true;
                FLPelementMoving.BackColor = Color.FromArgb(42, 47, 61);
                FLPelementMoving.Controls.Clear();

                ButtonsToDelete.Clear();

                FLPelementMoving.BringToFront();

                for (int i = 0; i < currentGroup.DataButtons.Count; i++)
                {
                    Button newButton = new Button();

                    newButton.Text = currentGroup.DataButtons[i].Title;
                    newButton.ForeColor = Color.Black;
                    newButton.Size = buttonSize;

                    newButton.BackColor = currentGroup.DataButtons[i].Color;
                    newButton.FlatStyle = FlatStyle.Flat;
                    newButton.FlatAppearance.BorderSize = 2;
                    newButton.FlatAppearance.BorderColor = currentGroup.DataButtons[i].Color;

                    if (currentGroup.DataButtons[i].Type == "image" && !File.Exists(ImageLocation + currentGroup.DataButtons[i].Text))
                    {
                        newButton.BackColor = Color.FromArgb(64, 64, 64);
                        newButton.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
                    }

                    Button leftButton = new Button();

                    leftButton.Size = new Size(37, newButton.Size.Height);
                    leftButton.Location = new Point(newButton.Location.X, newButton.Location.Y);
                    leftButton.BackColor = Color.LightBlue;
                    leftButton.FlatStyle = FlatStyle.Flat;
                    leftButton.Visible = false;
                    leftButton.Text = "<";
                    leftButton.TextAlign = ContentAlignment.MiddleCenter;

                    leftButton.Click += delegate
                    {
                        int buttonIndex = FLPelementMoving.Controls.GetChildIndex(newButton);
                        MoveListObject(buttonIndex, buttonIndex - 1, newButton);
                    };

                    Button rightButton = new Button();

                    rightButton.Size = new Size(37, newButton.Size.Height);
                    rightButton.Location = new Point(newButton.Location.X + newButton.Size.Width - 37, newButton.Location.Y);
                    rightButton.BackColor = Color.LightBlue;
                    rightButton.FlatStyle = FlatStyle.Flat;
                    rightButton.Visible = false;
                    rightButton.Text = ">";
                    rightButton.TextAlign = ContentAlignment.MiddleCenter;

                    rightButton.Click += delegate
                    {
                        int buttonIndex = FLPelementMoving.Controls.GetChildIndex(newButton);
                        MoveListObject(buttonIndex, buttonIndex + 1, newButton);
                    };


                    newButton.Controls.Add(leftButton);
                    newButton.Controls.Add(rightButton);

                    newButton.Click += delegate
                    {
                        switch (leftButton.Visible)
                        {
                            case false:
                                leftButton.Visible = true;
                                rightButton.Visible = true;
                                break;

                            case true:
                                leftButton.Visible = false;
                                rightButton.Visible = false;

                                break;
                        }

                    };


                    FLPelementMoving.Controls.Add(newButton);

                }
            }
        }

        private void BTNsetDefaultSize_Click(object sender, EventArgs e)
        {
            buttonSize = new Size(120, 50);
            BTNexampleSize.Size = buttonSize;
            NumUpDownWidth.Value = 120;
            NumUpDownHeight.Value = 50;
        }

        private void pictureBoxnewElement_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                pictureBoxnewElement.Image = Clipboard.GetImage();
            }
            else
            {
                ShowInformationPanel("Clipboard does not contain an image.", true);

            }

        }

        private void pictureBoxnewElement_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop, false);

            if (data != null)
            {
                var fileNames = data as string[];

                if (fileNames.Length > 0)
                {
                    pictureBoxnewElement.Image = Image.FromFile(fileNames[0]);

                }
            }
        }

        private void pictureBoxnewElement_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var file in files)
            {
                var ext = System.IO.Path.GetExtension(file);
                if (ext.Equals(".png", StringComparison.CurrentCultureIgnoreCase) || ext.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }

        }
        //----------------------------------------------

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShowInformationPanel("Program is closing...", false);
            bool[] SearchSettings = new bool[8];

            SearchSettings[0] = toolStripMenuItem8.Checked;
            SearchSettings[1] = toolStripMenuItem7.Checked;
            SearchSettings[2] = toolStripMenuItem1.Checked;
            SearchSettings[3] = toolStripMenuItem2.Checked;
            SearchSettings[4] = toolStripMenuItem4.Checked;
            SearchSettings[5] = toolStripMenuItem5.Checked;
            SearchSettings[6] = toolStripMenuItem6.Checked;
            SearchSettings[7] = toolStripMenuItem9.Checked;

            InOut.OutPutSettingsToTextFile(defaultColorIndex, SearchSettings, LastClosedPanelIndex, buttonSize);

            //InOut.OutputToExcel(Groups, ButtonColor);

            InOut.OutputToJSON(Groups, ButtonColor);
        }

        private void TXTBoxnewSidebutton_Enter(object sender, EventArgs e)
        {
            TXTBoxnewSidebutton.Text = "";
        }

        private void RTBnotes_TextChanged(object sender, EventArgs e)
        {
            currentGroup.Notes = RTBnotes.Text;
        }

        private void TXTboxSearchBar_TextChanged(object sender, EventArgs e)
        {
            bool SearchInAllGroups = toolStripMenuItem9.Checked;

            FLPsearchResults.Controls.Clear();

            if (TXTboxSearchBar.Text.Length > 0)
            {
                ShowControls(FLPsearchResults);
                HideControls(BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, BTNmoveOutside, BTNmoveInside, LabelmoveElements);

                FLPsearchResults.BringToFront();

                if (!SearchInAllGroups)
                {
                    for (int i = 0; i < currentGroup.DataButtons.Count; i++)
                    {
                        if (SearchForButton(currentGroup.DataButtons[i]))
                        {
                            CreateButton(currentGroup.DataButtons[i], currentGroup, FLPsearchResults);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Groups.Count; i++)
                    {
                        for (int j = 0; j < Groups[i].DataButtons.Count; j++)
                        {
                            if (SearchForButton(Groups[i].DataButtons[j]))
                            {
                                CreateButton(Groups[i].DataButtons[j], Groups[i], FLPsearchResults);

                            }
                        }
                    }
                }

            }
            else
            {
                ShowControls(BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, BTNmoveOutside, BTNmoveInside, LabelmoveElements);
                HideControls(FLPsearchResults);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (PanelinformationPanel.Height < 33)
            {
                PanelinformationPanel.Height += 3;
            }
            else
            {
                timer1.Stop();
            }
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem temp = (ToolStripMenuItem)sender;

            temp.Checked = !temp.Checked;

            TXTboxSearchBar_TextChanged(sender, e);

            contextMenuStrip1.Show();
        }

        private void NumUpDownWidth_ValueChanged(object sender, EventArgs e)
        {
            buttonSize.Width = (int)NumUpDownWidth.Value;
            BTNexampleSize.Width = (int)NumUpDownWidth.Value;
        }

        private void NumUpDownHeight_ValueChanged(object sender, EventArgs e)
        {
            buttonSize.Height = (int)NumUpDownHeight.Value;
            BTNexampleSize.Height = (int)NumUpDownHeight.Value;
        }

        private void AddColorButtonActions()
        {
            for (int i = 0; i < 9; i++)
            {
                int x = i;

                ColorButtons[x].MouseDown += (object sender, MouseEventArgs e) =>
                {

                    switch (e.Button)
                    {
                        case MouseButtons.Left: // left click


                            buttonColorIndex = x;

                            ColorButtons[lastpressedButtonIndex].FlatAppearance.BorderSize = 0;
                            lastpressedButtonIndex = x;

                            ColorButtons[x].FlatAppearance.BorderSize = 1; // outline pressed color button

                            break;

                        case MouseButtons.Right: // rightclick

                            ContextMenuStrip Menu = new ContextMenuStrip();

                            ToolStripMenuItem TS = new ToolStripMenuItem();

                            string[] TypeName = { "text", "link", "image" };

                            TS.Text = "Set as default color for " + TypeName[buttonType] + " buttons";

                            TS.Click += delegate
                            {

                                buttonColorIndex = x;

                                defaultColorIndex[buttonType] = x; // set color as default color for selected button type

                                ColorButtons[lastpressedButtonIndex].FlatAppearance.BorderSize = 0; // dim last pressed button
                                lastpressedButtonIndex = x; // set last pressed button to current button

                                ColorButtons[x].FlatAppearance.BorderSize = 1; // outline pressed button

                            };

                            Menu.Items.Add(TS);

                            ColorButtons[x].ContextMenuStrip = Menu;

                            break;
                    }

                };
            }
        }

        private void CreateSideButton(string ButtonName, int index)
        {
            Button newButton = new Button();

            newButton.Margin = new Padding(30, 10, 16, 0);
            newButton.Text = ButtonName;
            newButton.ForeColor = Color.White;
            newButton.BackColor = Color.FromArgb(33, 150, 243);
            newButton.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            newButton.Size = new Size(137, 38);
            newButton.FlatStyle = FlatStyle.Popup;

            newButton.MouseDown += (object sender, MouseEventArgs e) =>
            {
                CloseInformationPanel(sender, e);


                switch (e.Button)
                {
                    case MouseButtons.Left:

                        ShowControls(BTNsideButtonsPlus, BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, BTNmoveOutside, BTNmoveInside, LabelmoveElements);
                        HideControls(FLPsearchResults, PanelAddElement, TXTBoxnewSidebutton, PanelcreateSidebuttonYESNO);

                        BTNmoveOutside.BackColor = Color.FromArgb(33, 150, 243);
                        BTNmoveInside.BackColor = Color.FromArgb(33, 150, 243);

                        TXTboxSearchBar.Text = "";
                        TXTboxSearchBar.Enabled = true;

                        PanelNotes.Width = 0;
                        BTNgroupNotes.Text = "<";

                        MainPanel.Visible = true;

                        currentGroup.SideButton.BackColor = Color.FromArgb(33, 150, 243);
                        currentGroup = Groups[index];

                        RefreshButtons();

                        LastClosedPanelIndex = index;

                        PanelAddElement.BringToFront();

                        break;

                    case MouseButtons.Right:

                        ContextMenuStrip Menu = new ContextMenuStrip();

                        ToolStripMenuItem TS1 = new ToolStripMenuItem();

                        TS1.Text = "Delete";

                        TS1.Click += delegate
                        {
                            for (int i = 0; i < Groups[index].DataButtons.Count; i++)
                            {
                                if (Groups[index].DataButtons[i].Type == "image")
                                {
                                    System.IO.File.Delete(ImageLocation + Groups[index].DataButtons[i].Text);
                                }
                            }

                            // Refresh SideButtons
                            //---------------------
                            for (int i = 0; i < Groups.Count; i++)
                            {
                                FLPsideButtons.Controls.Remove(Groups[i].SideButton);
                            }

                            this.Controls.Remove(Groups[index].SideButton);

                            Groups.RemoveAt(index);

                            for (int i = 0; i < Groups.Count; i++)
                            {
                                CreateSideButton(Groups[i].GroupName, i);
                            }
                            //---------------------

                            LastClosedPanelIndex = 0;

                            if (Groups.Count == 0)
                            {
                                HideControls(BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, BTNgroupNotes, BTNmoveOutside, BTNmoveInside, LabelmoveElements);
                            }
                            else
                            {
                                currentGroup = Groups[0];
                                ShowControls(BTNsideButtonsPlus, BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, BTNgroupNotes, BTNmoveOutside, BTNmoveInside, LabelmoveElements);
                            }

                            RefreshButtons();
                            ShowMostClickedButtons();
                            ShowInformationPanel("Group succesfully deleted",false);


                        };

                        Menu.Items.Add(TS1);

                        ToolStripMenuItem TS2 = new ToolStripMenuItem();

                        TS2.Text = "Rename";

                        TS2.Click += delegate
                        {
                            lastGroup = Groups[index];

                            IsAddingButton = false;

                            lastGroup.SideButton.Visible = false;

                            TXTBoxnewSidebutton.Visible = true;

                            FLPsideButtons.Controls.Add(TXTBoxnewSidebutton);

                            TXTBoxnewSidebutton.Text = Groups[index].SideButton.Text;

                            FLPsideButtons.Controls.SetChildIndex(TXTBoxnewSidebutton, index + 1);

                            PanelcreateSidebuttonYESNO.Visible = true;

                            FLPsideButtons.Controls.SetChildIndex(PanelcreateSidebuttonYESNO, index + 2);

                            for (int i = 0; i < Groups.Count; i++)
                            {
                                Groups[i].SideButton.Enabled = false;
                            }
                            BTNsideButtonsPlus.Enabled = false;

                        };

                        Menu.Items.Add(TS2);

                        newButton.ContextMenuStrip = Menu;
                        break;

                }

            };

            Groups[index].SideButton = newButton;
            this.Controls.Add(newButton);

            var indx = FLPsideButtons.Controls.Count - 3;
            FLPsideButtons.Controls.Add(newButton);
            FLPsideButtons.Controls.SetChildIndex(newButton, indx);

            //panel4.BringToFront();
            PanelNotes.BringToFront();

            if (Groups.Count == 1)
            {
                newButton.BackColor = Color.FromArgb(18, 99, 163); // color current group button
                MainPanel.BringToFront();
                MainPanel.Visible = true;
                currentGroup = Groups[0];

                ShowControls(BTNdeleteElements, BTNdeleteElementsNO, BTNdeleteElementsYES, BTNgroupNotes, BTNmoveOutside, BTNmoveInside, LabelmoveElements);

            }
        }

        private void CreatePlusButton()
        {
            Button newButton = new Button();

            newButton.BackColor = Color.FromArgb(86, 86, 98);
            newButton.ForeColor = Color.Black;
            newButton.Text = "+";
            newButton.Size = buttonSize;
            newButton.Font = new Font("Romantic", 15, FontStyle.Bold);

            newButton.FlatStyle = FlatStyle.Flat;
            newButton.FlatAppearance.BorderSize = 2;
            newButton.FlatAppearance.BorderColor = Color.Gray;

            newButton.Click += delegate
            {
                ShowControls(PanelAddElement);
                HideControls(colorPanel, pictureBoxnewElement);

                PanelAddElement.BringToFront();

                RTBnewElementName.Text = "";
                RTBnewElementText.Text = "";

                buttonType = -1;

                BTNtextTypeElement.BackColor = Color.FromArgb(224, 224, 224);
                BTNlinkTypeElement.BackColor = Color.FromArgb(224, 224, 224);
                BTNimageTypeElement.BackColor = Color.FromArgb(224, 224, 224);

                pictureBoxnewElement.Image = null;

            };

            MainPanel.Controls.Add(newButton);
        }

        public static void ChangeButtonData(Form1 frm, FlowLayoutPanel flp)
        {
            int groupIndex = VariablesBetweenForms.groupIndex;
            int ButtonIndex = Groups[groupIndex].DataButtons.IndexOf(VariablesBetweenForms.dataButton);

            Groups[groupIndex].DataButtons[ButtonIndex].Title = VariablesBetweenForms.Title;

            int textLength = VariablesBetweenForms.Text.Length;

            Groups[groupIndex].DataButtons[ButtonIndex].Text = VariablesBetweenForms.Text;

            Groups[groupIndex].DataButtons[ButtonIndex].Color = VariablesBetweenForms.buttonColor;

            flp.Controls[ButtonIndex].Text = VariablesBetweenForms.Title;

            string temp = frm.TXTboxSearchBar.Text;
            frm.TXTboxSearchBar.Text = "";
            frm.TXTboxSearchBar.Text = temp;

            frm.RefreshButtons();

        }

        private void CreateButton(DataButton dataButton, group buttonGroup, FlowLayoutPanel flowPanel)
        {
            Button newButton = new Button();

            newButton.ForeColor = Color.Black;
            newButton.BackColor = dataButton.Color;

            newButton.Text = dataButton.Title;
            newButton.Size = buttonSize;
            newButton.FlatStyle = FlatStyle.Flat;

            newButton.FlatAppearance.BorderSize = 2;
            newButton.FlatAppearance.BorderColor = dataButton.Color;

            newButton.MouseHover += delegate
            {
                RTBelementPreview.Text = dataButton.Text;
            };

            newButton.MouseLeave += delegate
            {
                RTBelementPreview.Text = "";
            };

            switch (dataButton.Type)
            {
                case "image":

                    if (File.Exists(ImageLocation + dataButton.Text))
                    {
                        newButton.MouseHover += delegate
                        {
                            Image img;
                            using (var bmpTemp = new Bitmap(ImageLocation + dataButton.Text))
                            {
                                img = new Bitmap(bmpTemp);
                            }

                            pictureBoxelementPreview.Visible = true;
                            RTBelementPreview.Visible = false;
                            pictureBoxelementPreview.Image = img;

                        };

                        newButton.MouseLeave += delegate
                        {
                            pictureBoxelementPreview.Visible = false;
                            RTBelementPreview.Visible = true;

                        };

                        newButton.MouseDown += (object sender, MouseEventArgs e) =>
                        {
                            switch (e.Button)
                            {
                                case MouseButtons.Left: // left click

                                    ShowInformationPanel("Image copied to clipboard", false);

                                    Image img = Image.FromFile(ImageLocation + dataButton.Text);
                                    Clipboard.SetImage(img);
                                    img.Dispose();

                                    dataButton.ClickCount++;

                                    break;

                                case MouseButtons.Right: // rightclick

                                    //FLPsearchResults.Visible = false;

                                    ContextMenuStrip Menu = new ContextMenuStrip();

                                    ToolStripMenuItem TS = new ToolStripMenuItem();

                                    VariablesBetweenForms.ViewOnly = false;

                                    TS.Text = "Expand";

                                    TS.Click += delegate
                                    {
                                        Image bitmapImage;
                                        using (var bmpTemp = new Bitmap(ImageLocation + dataButton.Text))
                                        {
                                            bitmapImage = new Bitmap(bmpTemp);
                                        }

                                        VariablesBetweenForms.Text = dataButton.Text;
                                        VariablesBetweenForms.Title = dataButton.Title;
                                        VariablesBetweenForms.g = buttonGroup;
                                        VariablesBetweenForms.dataButton = dataButton;
                                        VariablesBetweenForms.groupIndex = Groups.IndexOf(buttonGroup);
                                        VariablesBetweenForms.buttonColor = dataButton.Color;

                                        Form2 Form2 = new Form2(bitmapImage, dataButton.Title + ".png", this, MainPanel);
                                        Form2.SetDesktopLocation(Cursor.Position.X, Cursor.Position.Y);
                                        Form2.Show(); // Shows Form2

                                    };

                                    Menu.Items.Add(TS);

                                    newButton.ContextMenuStrip = Menu;

                                    break;
                            }
                        };
                    }
                    else
                    {

                        newButton.Click += delegate
                        {
                            Clipboard.SetText(dataButton.Text);
                            string informationText = "Image element named '" + dataButton.Title + "' is missing an image";
                            ShowInformationPanel(informationText, true);
                            dataButton.ClickCount++;

                        };
                        newButton.BackColor = Color.FromArgb(64, 64, 64);
                        newButton.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);


                    }
                    break;

                case "text":

                    newButton.MouseDown += (object sender, MouseEventArgs e) =>
                    {
                        switch (e.Button)
                        {
                            case MouseButtons.Left: // left click

                                ShowInformationPanel("Text copied to clipboard", false);

                                try
                                {
                                    Clipboard.SetDataObject(dataButton.Text, true, 10, 100);
                                }
                                catch (Exception)
                                {

                                }

                                RTBelementPreview.Text = dataButton.Text;
                                dataButton.ClickCount++;


                                break;

                            case MouseButtons.Right: // right click

                                ContextMenuStrip Menu = new ContextMenuStrip();

                                ToolStripMenuItem TS = new ToolStripMenuItem();

                                VariablesBetweenForms.ViewOnly = false;

                                TS.Text = "Expand";

                                TS.Click += delegate
                                {
                                    VariablesBetweenForms.Text = dataButton.Text;
                                    VariablesBetweenForms.Title = dataButton.Title;
                                    VariablesBetweenForms.g = buttonGroup;
                                    VariablesBetweenForms.dataButton = dataButton;
                                    VariablesBetweenForms.groupIndex = Groups.IndexOf(buttonGroup);
                                    VariablesBetweenForms.buttonColor = dataButton.Color;


                                    Form3 Form3 = new Form3(this, MainPanel);
                                    Form3.SetDesktopLocation(Cursor.Position.X, Cursor.Position.Y);
                                    Form3.Show(); // Shows Form3

                                };

                                Menu.Items.Add(TS);

                                newButton.ContextMenuStrip = Menu;

                                break;

                        }
                    };

                    break;

                case "link":

                    newButton.MouseDown += (object sender, MouseEventArgs e) =>
                    {
                        switch (e.Button)
                        {
                            case MouseButtons.Left: // left cllick

                                ShowInformationPanel("Link opened in browser", false);

                                Process.Start(dataButton.Text);

                                break;


                            case MouseButtons.Right: // right click

                                ContextMenuStrip Menu = new ContextMenuStrip();

                                ToolStripMenuItem TS = new ToolStripMenuItem();

                                VariablesBetweenForms.ViewOnly = false;

                                TS.Text = "Expand";

                                TS.Click += delegate
                                {
                                    VariablesBetweenForms.Text = dataButton.Text;
                                    VariablesBetweenForms.Title = dataButton.Title;
                                    VariablesBetweenForms.g = buttonGroup;
                                    VariablesBetweenForms.dataButton = dataButton;
                                    VariablesBetweenForms.groupIndex = Groups.IndexOf(buttonGroup);
                                    VariablesBetweenForms.buttonColor = dataButton.Color;


                                    Form3 Form3 = new Form3(this, MainPanel);

                                    Form3.SetDesktopLocation(Cursor.Position.X, Cursor.Position.Y);
                                    Form3.Show(); // Shows Form3

                                };

                                Menu.Items.Add(TS);

                                newButton.ContextMenuStrip = Menu;

                                break;
                        }


                    };

                    break;
            }

            this.Controls.Add(newButton);

            flowPanel.Controls.Add(newButton);

        }

        private void ChooseButtonType(int index)
        {
            Button[] TypeButtons = { BTNtextTypeElement, BTNlinkTypeElement, BTNimageTypeElement };

            buttonType = index;

            foreach (Button btn in TypeButtons)
                btn.BackColor = Color.FromArgb(224, 224, 224);

            TypeButtons[index].BackColor = Color.FromArgb(192, 255, 192);

            colorPanel.Visible = true;

            if (buttonType == 2)
            {
                pictureBoxnewElement.Visible = true;

                pictureBoxnewElement.Image = pictureBoxnewElement.InitialImage;
            }
            else
            {
                pictureBoxnewElement.Visible = false;
            }

            buttonColorIndex = defaultColorIndex[index];

            ColorButtons[lastpressedButtonIndex].FlatAppearance.BorderSize = 0;
            lastpressedButtonIndex = buttonColorIndex;

            ColorButtons[buttonColorIndex].FlatAppearance.BorderColor = Color.Black;
            ColorButtons[buttonColorIndex].FlatAppearance.BorderSize = 1;
        }

        private bool SearchForButton(DataButton dataButton)
        {
            bool searchForTitle = toolStripMenuItem1.Checked;
            bool searchForText = toolStripMenuItem2.Checked;

            bool searchTypeText = toolStripMenuItem4.Checked;
            bool searchTypeLink = toolStripMenuItem5.Checked;
            bool searchTypeImage = toolStripMenuItem6.Checked;

            bool MatchWholeText = toolStripMenuItem7.Checked;

            if (MatchWholeText)
            {
                if (searchForTitle)
                {
                    if (searchTypeText)
                    {
                        if (dataButton.Type == "text")
                            if (AreStringsEqual(dataButton.Title))
                            {
                                return true;
                            }
                    }

                    if (searchTypeLink)
                    {
                        if (dataButton.Type == "link")
                            if (AreStringsEqual(dataButton.Title))
                            {
                                return true;
                            }
                    }

                    if (searchTypeImage)
                    {
                        if (dataButton.Type == "image")
                            if (AreStringsEqual(dataButton.Title))
                            {
                                return true;
                            }
                    }
                }

                if (searchForText)
                {
                    string ButtonText = dataButton.Text;
                    ButtonText = ButtonText.Replace("\n", string.Empty);

                    if (searchTypeText)
                    {
                        if (dataButton.Type == "text")
                            if (AreStringsEqual(ButtonText))
                            {
                                return true;
                            }
                    }

                    if (searchTypeLink)
                    {
                        if (dataButton.Type == "link")
                            if (AreStringsEqual(ButtonText))
                            {
                                return true;
                            }
                    }

                    if (searchTypeImage)
                    {
                        if (dataButton.Type == "image")
                            if (AreStringsEqual(ButtonText))
                            {
                                return true;
                            }
                    }
                }
            }
            else // match part of text
            {
                if (searchForTitle)
                {
                    if (searchTypeText)
                    {
                        if (dataButton.Type == "text")
                            if (StringContainsInput(dataButton.Title))
                            {
                                return true;
                            }
                    }

                    if (searchTypeLink)
                    {
                        if (dataButton.Type == "link")
                            if (StringContainsInput(dataButton.Title))
                            {
                                return true;
                            }
                    }

                    if (searchTypeImage)
                    {
                        if (dataButton.Type == "image")
                            if (StringContainsInput(dataButton.Title))
                            {
                                return true;
                            }
                    }
                }

                if (searchForText)
                {
                    string ButtonText = dataButton.Text;
                    ButtonText = ButtonText.Replace("\n", string.Empty);

                    if (searchTypeText)
                    {
                        if (dataButton.Type == "text")
                            if (StringContainsInput(ButtonText))
                            {
                                return true;
                            }
                    }

                    if (searchTypeLink)
                    {
                        if (dataButton.Type == "link")
                            if (StringContainsInput(ButtonText))
                            {
                                return true;
                            }
                    }

                    if (searchTypeImage)
                    {
                        if (dataButton.Type == "image")
                            if (StringContainsInput(ButtonText))
                            {
                                return true;
                            }
                    }
                }
            }

            return false;
        }

        private bool AreStringsEqual(string buttonString)
        {
            bool IsCaseSensitive = toolStripMenuItem8.Checked;
            bool result = false;

            switch (IsCaseSensitive)
            {
                case true:
                    if (buttonString == TXTboxSearchBar.Text)
                    {
                        result = true;
                    }
                    break;

                case false:
                    if (buttonString.Equals(TXTboxSearchBar.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool StringContainsInput(string buttonString)
        {
            bool IsCaseSensitive = toolStripMenuItem8.Checked;
            bool result = false;

            switch (IsCaseSensitive)
            {
                case true:
                    if (buttonString.Contains(TXTboxSearchBar.Text))
                    {
                        result = true;
                    }
                    break;

                case false:
                    if (buttonString.ToLower().Contains(TXTboxSearchBar.Text.ToLower()))
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private void MoveListObject(int lastindex, int newindex, Button newButton)
        {
            DataButton temp = currentGroup.DataButtons[lastindex];
            if (newindex == -1)
            {
                newindex = currentGroup.DataButtons.Count - 1;
            }
            else if (newindex == currentGroup.DataButtons.Count)
            {
                newindex = 0;
            }

            FLPelementMoving.Controls.SetChildIndex(newButton, newindex);

            currentGroup.DataButtons.RemoveAt(lastindex);

            currentGroup.DataButtons.Insert(newindex, temp);
        }

        private void RefreshButtons()
        {
            MainPanel.Controls.Clear();

            for (int j = 0; j < currentGroup.DataButtons.Count; j++)
            {
                CreateButton(currentGroup.DataButtons[j], currentGroup, MainPanel);
            }

            currentGroup.SideButton.BackColor = Color.FromArgb(18, 99, 163);


            CreatePlusButton();
        }

        private static void SortList(List<int> A) // bubble sort algorithm
        {
            for (int i = 0; i < A.Count - 1; i++)
            {
                for (int j = A.Count - 1; j > i; j--)
                {
                    if (A[j] > A[j - 1])
                    {
                        int temp = A[j];
                        A[j] = A[j - 1];
                        A[j - 1] = temp;
                    }
                }
            }
        }

        private void ShowInformationPanel(string infoText, bool IsError)
        {
            if (IsError)
            {
                PanelinformationPanel.BackColor = Color.Maroon;
                BTNinformationPanelClose.BackColor = Color.FromArgb(255, 128, 128);

            }
            else
            {
                PanelinformationPanel.BackColor = Color.FromArgb(10, 91, 158);
                BTNinformationPanelClose.BackColor = Color.FromArgb(13, 131, 227);

            }

            timer1.Start();

            PanelinformationPanel.BringToFront();

            LabelinfoPanelText.Text = infoText;

        }

        private void CloseInformationPanel(object sender, EventArgs e)
        {
            PanelinformationPanel.Height = 0;
        }

        public void HideControls(params Control[] HiddenControls)
        {
            foreach (Control control in HiddenControls)
                control.Visible = false;
        }

        public void ShowControls(params Control[] VisibleControls)
        {
            foreach (Control control in VisibleControls)
                control.Visible = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.Control && e.KeyCode == Keys.F)
            {
                TXTboxSearchBar.Focus();
            }
            else
              if (e.Control && e.KeyCode == Keys.N)
            {
                BTNgroupNotes.PerformClick();
            }


        }

    }
}


