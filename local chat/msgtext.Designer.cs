namespace local_chat
{
    partial class msgtext
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.NickNameUser = new System.Windows.Forms.Label();
            this.userText = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // NickNameUser
            // 
            this.NickNameUser.AutoSize = true;
            this.NickNameUser.Location = new System.Drawing.Point(74, 13);
            this.NickNameUser.Name = "NickNameUser";
            this.NickNameUser.Size = new System.Drawing.Size(27, 13);
            this.NickNameUser.TabIndex = 1;
            this.NickNameUser.Text = "имя";
            // 
            // userText
            // 
            this.userText.Location = new System.Drawing.Point(74, 40);
            this.userText.Name = "userText";
            this.userText.Size = new System.Drawing.Size(271, 13);
            this.userText.TabIndex = 2;
            this.userText.Text = "шшшшшшшшшшшшшшшшшшшшшшшшшшшшшшшшш";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pictureBox1.Image = global::local_chat.Properties.Resources.illustration_of_human_icon_user_symbol_icon_modern_design_on_blank_background_free_vector;
            this.pictureBox1.InitialImage = global::local_chat.Properties.Resources.illustration_of_human_icon_user_symbol_icon_modern_design_on_blank_background_free_vector;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(65, 59);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // msgtext
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.userText);
            this.Controls.Add(this.NickNameUser);
            this.Controls.Add(this.pictureBox1);
            this.Name = "msgtext";
            this.Size = new System.Drawing.Size(360, 65);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label NickNameUser;
        private System.Windows.Forms.Label userText;
    }
}
