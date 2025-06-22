using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace local_chat
{
    public partial class msgtext : UserControl
    {
        public msgtext()
        {
            InitializeComponent();
        }


        public Image image
        {
            get
            {
                return pictureBox1.Image;
            }
            set { pictureBox1.Image = value; }

        }
        public string username
        {
            get
            {
                return userNamelb.Text;
            }
            set
            {
                userNamelb.Text = value;
            }
        }
        public string usertext
        {
            get
            {
                return userText.Text;
            }
            set 
            {
               userText.Text= value;
            }
        }
    }
}
