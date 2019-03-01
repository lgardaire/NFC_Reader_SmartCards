using System;

namespace CSharpNETTestASKCSCDLL
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Button1 = new System.Windows.Forms.Button();
            this.txtCard = new System.Windows.Forms.TextBox();
            this.txtCom = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBoxContent = new CSharpNETTestASKCSCDLL.PlaceHolderTextBox();
            this.textBoxPrefix = new CSharpNETTestASKCSCDLL.PlaceHolderTextBox();
            this.SuspendLayout();
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(322, 12);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(89, 44);
            this.Button1.TabIndex = 1;
            this.Button1.Text = "Read on Card";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // txtCard
            // 
            this.txtCard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCard.Location = new System.Drawing.Point(149, 30);
            this.txtCard.Name = "txtCard";
            this.txtCard.ReadOnly = true;
            this.txtCard.Size = new System.Drawing.Size(155, 13);
            this.txtCard.TabIndex = 6;
            // 
            // txtCom
            // 
            this.txtCom.Location = new System.Drawing.Point(44, 27);
            this.txtCom.Name = "txtCom";
            this.txtCom.ReadOnly = true;
            this.txtCom.Size = new System.Drawing.Size(88, 20);
            this.txtCom.TabIndex = 5;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(7, 30);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(34, 13);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "COM:";
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(44, 78);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(260, 195);
            this.textBox1.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(758, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(89, 44);
            this.button2.TabIndex = 8;
            this.button2.Text = "Write on Card";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "Text",
            "URI",
            "Raw"});
            this.comboBoxType.Location = new System.Drawing.Point(455, 35);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(143, 21);
            this.comboBoxType.TabIndex = 10;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(455, 91);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 11;
            this.button3.Text = "Add";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(455, 12);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(124, 20);
            this.textBox2.TabIndex = 12;
            this.textBox2.Text = "Choose your record type";
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Location = new System.Drawing.Point(455, 120);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(260, 171);
            this.textBox3.TabIndex = 15;
            // 
            // textBoxContent
            // 
            this.textBoxContent.Location = new System.Drawing.Point(528, 61);
            this.textBoxContent.Name = "textBoxContent";
            this.textBoxContent.PlaceHolderText = "Content here";
            this.textBoxContent.Size = new System.Drawing.Size(187, 20);
            this.textBoxContent.TabIndex = 17;
            // 
            // textBoxPrefix
            // 
            this.textBoxPrefix.Location = new System.Drawing.Point(455, 61);
            this.textBoxPrefix.Name = "textBoxPrefix";
            this.textBoxPrefix.PlaceHolderText = "Prefix here";
            this.textBoxPrefix.Size = new System.Drawing.Size(67, 20);
            this.textBoxPrefix.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 295);
            this.Controls.Add(this.textBoxPrefix);
            this.Controls.Add(this.textBoxContent);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txtCard);
            this.Controls.Add(this.txtCom);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Button1);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            String type = comboBoxType.Text;
            
            if (type == "Text")
            {
                this.textBoxPrefix.Hide();
            }
            else if (type == "URI")
            {
                this.textBoxPrefix.Show();
                this.textBoxPrefix.PlaceHolderText = "Prefix here";
            }
            else if (type == "Raw")
            {
                this.textBoxPrefix.Hide();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        #endregion

        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.TextBox txtCard;
        internal System.Windows.Forms.TextBox txtCom;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox textBox1;
        internal System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        internal System.Windows.Forms.TextBox textBox3;
        private PlaceHolderTextBox textBoxContent;
        private PlaceHolderTextBox textBoxPrefix;
    }
}

