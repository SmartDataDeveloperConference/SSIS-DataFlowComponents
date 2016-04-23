namespace Sddc.SampleDataFlowComponent
{
    partial class SetWebsiteAddressDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.websiteAddressTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(323, 39);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // websiteAddressTextBox
            // 
            this.websiteAddressTextBox.Location = new System.Drawing.Point(13, 13);
            this.websiteAddressTextBox.Name = "websiteAddressTextBox";
            this.websiteAddressTextBox.Size = new System.Drawing.Size(385, 20);
            this.websiteAddressTextBox.TabIndex = 1;
            // 
            // SetWebsiteAddressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 73);
            this.Controls.Add(this.websiteAddressTextBox);
            this.Controls.Add(this.okButton);
            this.Name = "SetWebsiteAddressDialog";
            this.Text = "SetWebsiteAddressDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox websiteAddressTextBox;
    }
}