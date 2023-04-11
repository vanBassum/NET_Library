
namespace FormsLib.Controls
{
    partial class CollectionEditDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.collectionEditControl1 = new FormsLib.Controls.CollectionEditControl();
            this.SuspendLayout();
            // 
            // collectionEditControl1
            // 
            this.collectionEditControl1.CreateObject = null;
            this.collectionEditControl1.DataSource = null;
            this.collectionEditControl1.DisplayMember = "";
            this.collectionEditControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.collectionEditControl1.Location = new System.Drawing.Point(0, 0);
            this.collectionEditControl1.Name = "collectionEditControl1";
            this.collectionEditControl1.Size = new System.Drawing.Size(583, 395);
            this.collectionEditControl1.TabIndex = 0;
            // 
            // CollectionEditDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 395);
            this.Controls.Add(this.collectionEditControl1);
            this.Name = "CollectionEditDialog";
            this.Text = "CollectionEditDialog";
            this.Load += new System.EventHandler(this.CollectionEditDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.CollectionEditControl collectionEditControl1;
    }
}