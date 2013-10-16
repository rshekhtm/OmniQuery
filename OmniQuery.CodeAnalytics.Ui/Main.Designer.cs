namespace OmniQuery.CodeAnalytics.Ui
{
    partial class Main
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
            this.AnalysisTree = new System.Windows.Forms.TreeView();
            this.QueryTab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.QueryText = new System.Windows.Forms.TextBox();
            this.ResultGrid = new System.Windows.Forms.DataGridView();
            this.QueryTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // AnalysisTree
            // 
            this.AnalysisTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.AnalysisTree.Location = new System.Drawing.Point(2, 3);
            this.AnalysisTree.Name = "AnalysisTree";
            this.AnalysisTree.Size = new System.Drawing.Size(211, 482);
            this.AnalysisTree.TabIndex = 0;
            // 
            // QueryTab
            // 
            this.QueryTab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.QueryTab.Controls.Add(this.tabPage1);
            this.QueryTab.Location = new System.Drawing.Point(219, 3);
            this.QueryTab.Name = "QueryTab";
            this.QueryTab.SelectedIndex = 0;
            this.QueryTab.Size = new System.Drawing.Size(466, 206);
            this.QueryTab.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.QueryText);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(458, 180);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "New Query";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // QueryText
            // 
            this.QueryText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.QueryText.Location = new System.Drawing.Point(3, 3);
            this.QueryText.Multiline = true;
            this.QueryText.Name = "QueryText";
            this.QueryText.Size = new System.Drawing.Size(452, 174);
            this.QueryText.TabIndex = 0;
            // 
            // ResultGrid
            // 
            this.ResultGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.ResultGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultGrid.Location = new System.Drawing.Point(219, 215);
            this.ResultGrid.Name = "ResultGrid";
            this.ResultGrid.Size = new System.Drawing.Size(466, 270);
            this.ResultGrid.TabIndex = 2;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 488);
            this.Controls.Add(this.ResultGrid);
            this.Controls.Add(this.QueryTab);
            this.Controls.Add(this.AnalysisTree);
            this.Name = "Main";
            this.Text = "OmniQuery Code Analytics";
            this.QueryTab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView AnalysisTree;
        private System.Windows.Forms.TabControl QueryTab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView ResultGrid;
        private System.Windows.Forms.TextBox QueryText;

    }
}

