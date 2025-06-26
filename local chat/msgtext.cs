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
            countChar = userText.Text .Length;
        }

        private int countChar;
        public Image image
        {
            get
            {
                return pictureBox1.Image;
            }
            set { pictureBox1.Image = value == null ? pictureBox1.Image : value; }

        }
        public string username
        {
            get
            {
                return NickNameUser.Text;
            }
            set
            {
                NickNameUser.Text = value;
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
                int count = value.Length / countChar;
                if (count >0)
                {
                    int oldSizeH = userText.Height;
                    userText.Size = new Size(userText.Width, userText.Height * count);
                    this.Height = this.Height +userText.Height - oldSizeH ;
                }




               userText.Text= value;
            }
        }
    }
}
