using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    public partial class FormRateEvent : Form
    {
        
        
        private int _eventId; 
        
        private ComboBox cmbRating;
        private TextBox txtComment;
        private Button btnSend;
        private FeedbackDal _feedDal;

        public FormRateEvent(int eventId, string eventTitle)
        {
            
            this._eventId = eventId;
            
            this.Text = "Değerlendir: " + eventTitle;
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            _feedDal = new FeedbackDal();

            SetupUI();
        }

        private void SetupUI()
        {
            Label l1 = new Label { Text = "Puanınız (1-5):", Location = new Point(20, 20), AutoSize = true };
            cmbRating = new ComboBox { Location = new Point(20, 45), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRating.Items.AddRange(new object[] { "1 - Kötü", "2 - Orta", "3 - İyi", "4 - Çok İyi", "5 - Mükemmel" });
            cmbRating.SelectedIndex = 4; 

            Label l2 = new Label { Text = "Yorumunuz:", Location = new Point(20, 80), AutoSize = true };
            txtComment = new TextBox { Location = new Point(20, 105), Width = 340, Height = 80, Multiline = true };

            btnSend = new Button { Text = "GÖNDER", Location = new Point(20, 200), Width = 340, Height = 40, BackColor = Color.Gold, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnSend.Click += BtnSend_Click;

            this.Controls.Add(l1); this.Controls.Add(cmbRating);
            this.Controls.Add(l2); this.Controls.Add(txtComment);
            this.Controls.Add(btnSend);
        }

        private void BtnSend_Click(object? sender, EventArgs e)
        {
            try
            {
                int rating = cmbRating.SelectedIndex + 1; 
                
                
                _feedDal.GiveFeedback(this._eventId, Session.CurrentUser!.UserId, rating, txtComment.Text);
                
                MessageBox.Show("Geri bildiriminiz için teşekkürler! ⭐");
                this.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("unique")) MessageBox.Show("Bu etkinliği zaten oyladınız.");
                else MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}