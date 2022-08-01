using System.Collections.Generic;
using System.Windows.Forms;

namespace SimpleTask
{
    class group
    {
        public List<DataButton> DataButtons { get; set; }
        public Button SideButton { get; set; }
        
        public FlowLayoutPanel FlowPanel { get; set; }
        public string GroupName { get; set; }
        public string Notes { get; set; }

        public group()
        {
            DataButtons = new List<DataButton>();
            Notes = "";
        }
    }

}
