namespace WarframeRivenScanner
{
  partial class MainForm
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
      this.statsPicBox = new System.Windows.Forms.PictureBox();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.weaponNameBox = new System.Windows.Forms.TextBox();
      this.rivenNameBox = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.statsPicBox)).BeginInit();
      this.SuspendLayout();
      // 
      // statsPicBox
      // 
      this.statsPicBox.Location = new System.Drawing.Point(12, 12);
      this.statsPicBox.Name = "statsPicBox";
      this.statsPicBox.Size = new System.Drawing.Size(240, 350);
      this.statsPicBox.TabIndex = 1;
      this.statsPicBox.TabStop = false;
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(460, 12);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(230, 84);
      this.textBox1.TabIndex = 2;
      // 
      // weaponNameBox
      // 
      this.weaponNameBox.Location = new System.Drawing.Point(460, 103);
      this.weaponNameBox.Name = "weaponNameBox";
      this.weaponNameBox.Size = new System.Drawing.Size(230, 20);
      this.weaponNameBox.TabIndex = 3;
      // 
      // rivenNameBox
      // 
      this.rivenNameBox.Location = new System.Drawing.Point(460, 130);
      this.rivenNameBox.Name = "rivenNameBox";
      this.rivenNameBox.Size = new System.Drawing.Size(230, 20);
      this.rivenNameBox.TabIndex = 4;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.rivenNameBox);
      this.Controls.Add(this.weaponNameBox);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.statsPicBox);
      this.Name = "MainForm";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.MainForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.statsPicBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.PictureBox statsPicBox;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TextBox weaponNameBox;
    private System.Windows.Forms.TextBox rivenNameBox;
  }
}

