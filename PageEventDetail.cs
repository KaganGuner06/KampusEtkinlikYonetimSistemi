using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    public class PageEventDetail : UserControl
    {
        private Event _currentEvent;
        private EventDal _eventDal = new EventDal();
        private AppDal _appDal = new AppDal();
        private Button btnApply;

        // Renk Paleti
        private Color clrHeader = ColorTranslator.FromHtml("#2C3E50");
        private Color clrAccent = ColorTranslator.FromHtml("#1ABC9C");
        private Color clrDelete = ColorTranslator.FromHtml("#E74C3C");

        public PageEventDetail(Event evt)
        {
            _currentEvent = evt;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.AutoScroll = true; 
            InitializeUI();
            CheckApplicationStatus();
        }

        private void InitializeUI()
        {
            this.Controls.Clear();

            // 1. ÜST PANEL (Geri Dön)
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke, Padding = new Padding(10) };
            Button btnBack = new Button
            {
                Text = "← Geri Dön",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = clrHeader,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(20, 10),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => GoBackToList();
            pnlTop.Controls.Add(btnBack);

            // 2. ANA İÇERİK PANELİ (FlowLayout kullanarak daha düzenli dizebiliriz)
            Panel pnlContent = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = Color.White,
                Padding = new Padding(40)
            };

            // Görsel (Poster)
            PictureBox pbImage = new PictureBox
            {
                Width = 400,
                Height = 300,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.WhiteSmoke,
                Location = new Point(40, 20),
                BorderStyle = BorderStyle.FixedSingle
            };
            try 
            { 
                if (!string.IsNullOrEmpty(_currentEvent.PosterUrl) && File.Exists(_currentEvent.PosterUrl))
                    pbImage.Image = Image.FromFile(_currentEvent.PosterUrl); 
            } catch { }

            // --- BİLGİLER BÖLÜMÜ ---
            int infoX = 470;
            
            // Başlık
            Label lblTitle = new Label { Text = _currentEvent.Title, Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = clrHeader, Location = new Point(infoX, 20), AutoSize = true, MaximumSize = new Size(500, 0) };

            // Puan
            Label lblRating = new Label { 
                Text = _currentEvent.AverageRating > 0 ? $"⭐ {_currentEvent.AverageRating:N1} / 5" : "⭐ Yeni Etkinlik", 
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.Orange, 
                Location = new Point(infoX, 70), 
                AutoSize = true 
            };

            // Kulüp Bilgisi
            Label lblClubInfo = new Label { 
                Text = "Düzenleyen: " + (_currentEvent.ClubName ?? "Bilinmeyen Kulüp"), 
                Font = new Font("Segoe UI", 12, FontStyle.Bold), 
                ForeColor = Color.DarkSlateBlue, 
                Location = new Point(infoX, 105), 
                AutoSize = true 
            };

            // Detaylar
            Label lblDate = CreateInfoLabel("📅 Tarih: " + _currentEvent.EventDate.ToString("dd.MM.yyyy HH:mm"), infoX, 140);
            Label lblLoc = CreateInfoLabel("📍 Konum: " + _currentEvent.Location, infoX, 175);
            Label lblQuota = CreateInfoLabel("👥 Kontenjan: " + _currentEvent.Quota + " Kişi", infoX, 210);
            
            // Açıklama Kısmı
            Label lblDescTitle = new Label { Text = "Etkinlik Hakkında", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = clrHeader, Location = new Point(40, 360), AutoSize = true };
            Label lblDesc = new Label { Text = _currentEvent.Description, Font = new Font("Segoe UI", 11), ForeColor = Color.DimGray, Location = new Point(40, 400), Width = 900, AutoSize = true, MaximumSize = new Size(900, 0) };

            // 3. AKSİYON PANELİ (Alt Kısım)
            Panel pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.WhiteSmoke, Padding = new Padding(40, 20, 0, 0) };

            string userRole = Session.CurrentUser?.Role?.Trim()?.ToUpper() ?? "STUDENT";

            if (userRole == "ADMIN" || userRole == "CLUB_MANAGER")
            {
                // Admin Panelinde sadece SİL butonu bıraktık (Düzenle kaldırıldı)
                Button btnDelete = CreateActionButton("ETKİNLİĞİ SİL", clrDelete, new Point(40, 25));
                btnDelete.Click += BtnDelete_Click;
                pnlActions.Controls.Add(btnDelete);

                Label lblAdminNote = new Label { 
                    Text = "ℹ️ Yönetici modundasınız. Düzenleme işlemini ana listeden yapabilirsiniz.", 
                    Font = new Font("Segoe UI", 9, FontStyle.Italic), 
                    ForeColor = Color.DimGray, 
                    Location = new Point(240, 40), 
                    AutoSize = true 
                };
                pnlActions.Controls.Add(lblAdminNote);
            }
            else
            {
                // Öğrenci için BAŞVUR butonu
                btnApply = CreateActionButton("ETKİNLİĞE BAŞVUR", clrAccent, new Point(40, 25));
                btnApply.Width = 300; 
                btnApply.Click += BtnApply_Click;
                pnlActions.Controls.Add(btnApply);
            }

            // Kontrolleri Ekleme
            pnlContent.Controls.AddRange(new Control[] { lblDesc, lblDescTitle, lblQuota, lblLoc, lblDate, lblClubInfo, lblRating, lblTitle, pbImage });
            this.Controls.AddRange(new Control[] { pnlContent, pnlActions, pnlTop });
            
            pnlTop.BringToFront();
        }

        private void CheckApplicationStatus()
        {
            if (Session.CurrentUser == null || btnApply == null) return;

            // Daha önce başvurdu mu kontrolü
            bool isApplied = _appDal.IsUserApplied(Session.CurrentUser.UserId, _currentEvent.EventId);
            if (isApplied)
            {
                btnApply.Text = "BAŞVURU YAPILDI ✅";
                btnApply.BackColor = Color.Green;
                btnApply.Enabled = false;
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (Session.CurrentUser == null) return;

            try
            {
                _appDal.ApplyToEvent(Session.CurrentUser.UserId, _currentEvent.EventId);
                MessageBox.Show("Başvurunuz başarıyla alındı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CheckApplicationStatus();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bu etkinliği silmek istediğinize emin misiniz? Bu işlem geri alınamaz.", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _eventDal.DeleteEvent(_currentEvent.EventId);
                    GoBackToList();
                }
                catch (Exception ex) { MessageBox.Show("Silme işlemi başarısız: " + ex.Message); }
            }
        }

        private void GoBackToList()
        {
            FormMain main = (FormMain)this.FindForm();
            if (main != null) 
            { 
                main.pnlContent.Controls.Clear(); 
                main.pnlContent.Controls.Add(new PageEvents()); 
            }
        }

        private Label CreateInfoLabel(string text, int x, int y) => new Label { Text = text, Font = new Font("Segoe UI", 12), ForeColor = Color.DimGray, Location = new Point(x, y), AutoSize = true };
        
        private Button CreateActionButton(string text, Color bg, Point loc)
        {
            Button btn = new Button { Text = text, BackColor = bg, ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Size = new Size(180, 50), Location = loc, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}