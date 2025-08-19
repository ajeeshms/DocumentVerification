namespace DesktopClient {
    partial class Form1 {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.TextBox txtResponse;
        private System.Windows.Forms.Timer timer1;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.btnUpload = new System.Windows.Forms.Button();
            this.lblTimer = new System.Windows.Forms.Label();
            this.txtResponse = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(20, 20);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(120, 30);
            this.btnUpload.TabIndex = 0;
            this.btnUpload.Text = "Upload File";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Location = new System.Drawing.Point(160, 28);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(89, 15);
            this.lblTimer.TabIndex = 1;
            this.lblTimer.Text = "Elapsed: 0 sec";
            // 
            // txtResponse
            // 
            this.txtResponse.Location = new System.Drawing.Point(20, 70);
            this.txtResponse.Multiline = true;
            this.txtResponse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.Size = new System.Drawing.Size(500, 300);
            this.txtResponse.TabIndex = 2;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(550, 400);
            this.Controls.Add(this.txtResponse);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.btnUpload);
            this.Name = "Form1";
            this.Text = "SecurePrint - Client";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
