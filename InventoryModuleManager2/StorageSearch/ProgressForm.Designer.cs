namespace StorageSearch
{
    partial class ProgressForm
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
            this.button_stop = new System.Windows.Forms.Button();
            this.progressBar_progress = new System.Windows.Forms.ProgressBar();
            this.label_title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_stop
            // 
            this.button_stop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_stop.Location = new System.Drawing.Point(153, 57);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(75, 23);
            this.button_stop.TabIndex = 0;
            this.button_stop.Text = "Стоп";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // progressBar_progress
            // 
            this.progressBar_progress.Location = new System.Drawing.Point(12, 28);
            this.progressBar_progress.MarqueeAnimationSpeed = 50;
            this.progressBar_progress.Name = "progressBar_progress";
            this.progressBar_progress.Size = new System.Drawing.Size(367, 23);
            this.progressBar_progress.TabIndex = 1;
            // 
            // label_title
            // 
            this.label_title.AutoSize = true;
            this.label_title.Location = new System.Drawing.Point(13, 9);
            this.label_title.Name = "label_title";
            this.label_title.Size = new System.Drawing.Size(35, 13);
            this.label_title.TabIndex = 2;
            this.label_title.Text = "label1";
            // 
            // ProgressForm
            // 
            this.AcceptButton = this.button_stop;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_stop;
            this.ClientSize = new System.Drawing.Size(392, 92);
            this.ControlBox = false;
            this.Controls.Add(this.label_title);
            this.Controls.Add(this.progressBar_progress);
            this.Controls.Add(this.button_stop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProgressForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.ProgressBar progressBar_progress;
        private System.Windows.Forms.Label label_title;
    }
}