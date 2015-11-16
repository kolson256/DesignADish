namespace DesignADishAdmin
{
    partial class AdminForm
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
            this.btnExtractTerms = new System.Windows.Forms.Button();
            this.btnIngredient = new System.Windows.Forms.Button();
            this.btnStopTerm = new System.Windows.Forms.Button();
            this.lblTerm = new System.Windows.Forms.Label();
            this.cmbWordCount = new System.Windows.Forms.ComboBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnBuildIndex = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExtractTerms
            // 
            this.btnExtractTerms.Location = new System.Drawing.Point(189, 31);
            this.btnExtractTerms.Name = "btnExtractTerms";
            this.btnExtractTerms.Size = new System.Drawing.Size(65, 35);
            this.btnExtractTerms.TabIndex = 1;
            this.btnExtractTerms.Text = "Extract Terms";
            this.btnExtractTerms.UseVisualStyleBackColor = true;
            this.btnExtractTerms.Click += new System.EventHandler(this.btnExtractTerms_Click);
            // 
            // btnIngredient
            // 
            this.btnIngredient.Location = new System.Drawing.Point(189, 85);
            this.btnIngredient.Name = "btnIngredient";
            this.btnIngredient.Size = new System.Drawing.Size(65, 35);
            this.btnIngredient.TabIndex = 2;
            this.btnIngredient.Text = "Ingredient";
            this.btnIngredient.UseVisualStyleBackColor = true;
            this.btnIngredient.Click += new System.EventHandler(this.btnTermCategory_Click);
            // 
            // btnStopTerm
            // 
            this.btnStopTerm.Location = new System.Drawing.Point(260, 85);
            this.btnStopTerm.Name = "btnStopTerm";
            this.btnStopTerm.Size = new System.Drawing.Size(65, 35);
            this.btnStopTerm.TabIndex = 3;
            this.btnStopTerm.Text = "Stop Term";
            this.btnStopTerm.UseVisualStyleBackColor = true;
            this.btnStopTerm.Click += new System.EventHandler(this.btnTermCategory_Click);
            // 
            // lblTerm
            // 
            this.lblTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTerm.Location = new System.Drawing.Point(12, 139);
            this.lblTerm.Name = "lblTerm";
            this.lblTerm.Size = new System.Drawing.Size(491, 42);
            this.lblTerm.TabIndex = 8;
            this.lblTerm.Text = "Terms";
            this.lblTerm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbWordCount
            // 
            this.cmbWordCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbWordCount.FormattingEnabled = true;
            this.cmbWordCount.Items.AddRange(new object[] {
            "       1",
            "       2",
            "       3",
            "       4",
            "       5",
            "       6"});
            this.cmbWordCount.Location = new System.Drawing.Point(260, 37);
            this.cmbWordCount.Name = "cmbWordCount";
            this.cmbWordCount.Size = new System.Drawing.Size(65, 24);
            this.cmbWordCount.TabIndex = 10;
            this.cmbWordCount.Text = "       1";
            // 
            // lblCount
            // 
            this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.Location = new System.Drawing.Point(189, 181);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(136, 42);
            this.lblCount.TabIndex = 11;
            this.lblCount.Text = "Term Count";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnBuildIndex
            // 
            this.btnBuildIndex.Location = new System.Drawing.Point(425, 23);
            this.btnBuildIndex.Name = "btnBuildIndex";
            this.btnBuildIndex.Size = new System.Drawing.Size(78, 50);
            this.btnBuildIndex.TabIndex = 12;
            this.btnBuildIndex.Text = "Build Recipe-Term Index";
            this.btnBuildIndex.UseVisualStyleBackColor = true;
            this.btnBuildIndex.Click += new System.EventHandler(this.btnBuildIndex_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 250);
            this.Controls.Add(this.btnBuildIndex);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.cmbWordCount);
            this.Controls.Add(this.lblTerm);
            this.Controls.Add(this.btnStopTerm);
            this.Controls.Add(this.btnIngredient);
            this.Controls.Add(this.btnExtractTerms);
            this.Name = "AdminForm";
            this.Text = "Design A Dish Admin";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExtractTerms;
        private System.Windows.Forms.Button btnIngredient;
        private System.Windows.Forms.Button btnStopTerm;
        private System.Windows.Forms.Label lblTerm;
        private System.Windows.Forms.ComboBox cmbWordCount;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Button btnBuildIndex;
    }
}

