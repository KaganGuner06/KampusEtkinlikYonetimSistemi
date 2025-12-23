using System;
using System.Drawing;
using System.Windows.Forms;

namespace CampusEventManager
{
    
    public class FormAnnouncementDetail : Form
    {
        public FormAnnouncementDetail(string title, string content, string clubName, string date)
        {
            this.Text = "Duyuru Detayı";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            
            Label lblTitle = new Label { Text = title, Location = new Point(20, 20), Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.DarkSlateBlue, AutoSize = true, MaximumSize = new Size(440, 0) };
            this.Controls.Add(lblTitle);

            
            Label lblInfo = new Label { Text = $"{clubName} • {date}", Location = new Point(20, lblTitle.Bottom + 10), Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.Gray, AutoSize = true };
            this.Controls.Add(lblInfo);

            
            RichTextBox rtxt = new RichTextBox 
            { 
                Text = content, 
                Location = new Point(20, lblInfo.Bottom + 20), 
                Size = new Size(440, 250), 
                Font = new Font("Segoe UI", 11), 
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ReadOnly = true 
            };
            this.Controls.Add(rtxt);

            
            Button btnClose = new Button { Text = "Kapat", Location = new Point(180, 320), Size = new Size(120, 35), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClose.Click += (s, e) => this.Close();
            
        }
    }
}