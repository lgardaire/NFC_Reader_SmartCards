namespace CSharpNETWriter
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
            this.Button1 = new System.Windows.Forms.Button();
            this.txtCard = new System.Windows.Forms.TextBox();
            this.txtCom = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(327, 53);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(89, 44);
            this.Button1.TabIndex = 1;
            this.Button1.Text = "Write on Card";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // txtCard
            // 
            this.txtCard.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCard.Location = new System.Drawing.Point(10, 118);
            this.txtCard.Name = "txtCard";
            this.txtCard.ReadOnly = true;
            this.txtCard.Size = new System.Drawing.Size(301, 13);
            this.txtCard.TabIndex = 6;
            // 
            // txtCom
            // 
            this.txtCom.Location = new System.Drawing.Point(93, 66);
            this.txtCom.MaxLength = 100;
            this.txtCom.Name = "txtCom";
            this.txtCom.Size = new System.Drawing.Size(218, 20);
            this.txtCom.TabIndex = 5;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(7, 69);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(80, 13);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "Your message :";
            // 
            // comboBoxType
            // 
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "Text",
            "URI",
            "SP"});
            this.comboBoxType.Location = new System.Drawing.Point(107, 17);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(204, 21);
            this.comboBoxType.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Type of message :";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 189);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.txtCard);
            this.Controls.Add(this.txtCom);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Button1);
            this.Name = "Form1";
            this.Text = "C#NET using ASKreaderLib.DLL using ASKSCS.DLL";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.TextBox txtCard;
        internal System.Windows.Forms.TextBox txtCom;
        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.ComboBox comboBoxType;
        internal System.Windows.Forms.Label label2;
    }
}

