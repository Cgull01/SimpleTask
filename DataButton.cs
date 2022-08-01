using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleTask
{
    class DataButton
    {
        public DataButton()
        {
            ClickCount = 0;
        }

        public string Title { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public int ClickCount { get; set; }



    }

}
