using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public class PageEvents : UserControl
    {
        // UI Kontrolleri
        private FlowLayoutPanel pnlEvents;
        private ComboBox cmbCategory;
        private DateTimePicker dtpDate;
        private TextBox txtSearch; // Arama Kutusu
        private Button btnFilter, btnClear;
        
        // Veri Erişimi
        private EventDal _eventDal;
        private CommonDal _commonDal;

        public PageEvents()
        {
            // Sayfa Ayarları
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;
            
            // Veri katmanlarını başlat
            _eventDal = new EventDal();
            _commonDal = new CommonDal();

            // Arayüzü ve Verileri Yükle
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            // --- 1. ANA KAPLAYICI (ÜST PANEL) ---
            Panel pnlTop = new Panel 
            { 
                Dock = DockStyle.Top, 
                Height = 80, 
                BackColor = Color.White, 
                Padding = new Padding(5) 
            };

            // --- 2. SAĞ TARAFTAKİ ADMIN PANELİ ---
            // Yetki kontrolü yapıyoruz
            Panel pnlAdminRight = new Panel { Visible = false, Dock = DockStyle.Right, Width = 1 }; 

            string role = Session.CurrentUser?.Role?.Trim()?.ToUpper();
            if (role == "ADMIN" || role == "CLUB_MANAGER")
            {
                // Butonları sağa yaslamak için FlowLayout
                FlowLayoutPanel flpAdmin = new FlowLayoutPanel 
                { 
                    Dock = DockStyle.Fill, 
                    BackColor = Color.Transparent,
                    FlowDirection = FlowDirection.RightToLeft, 
                    Padding = new Padding(0, 20, 5, 0) 
                };

                // + YENİ ETKİNLİK Butonu
                Button btnNewEvent = new Button 
                { 
                    Text = "+ YENİ", 
                    Size = new Size(90, 35), 
                    BackColor = ColorTranslator.FromHtml("#27AE60"), 
                    ForeColor = Color.White, 
                    FlatStyle = FlatStyle.Flat, 
                    Cursor = Cursors.Hand, 
                    Font = new Font("Segoe UI", 9, FontStyle.Bold), 
                    Margin = new Padding(5, 0, 0, 0) 
                };
                // Yeni etkinlik için null gönderiyoruz
                btnNewEvent.Click += (s, e) => OpenPage(new PageEventEdit(null));

                // KATEGORİLER Butonu
                Button btnCats = new Button 
                { 
                    Text = "KATEGORİLER", 
                    Size = new Size(110, 35), 
                    BackColor = Color.Orange, 
                    ForeColor = Color.White, 
                    FlatStyle = FlatStyle.Flat, 
                    Cursor = Cursors.Hand, 
                    Font = new Font("Segoe UI", 8, FontStyle.Bold), 
                    Margin = new Padding(5, 0, 0, 0) 
                };
                btnCats.Click += (s, e) => new FormCategories().ShowDialog();

                flpAdmin.Controls.Add(btnNewEvent);
                flpAdmin.Controls.Add(btnCats);

                // Admin Paneli Konteyneri
                pnlAdminRight = new Panel 
                { 
                    Dock = DockStyle.Right, 
                    Width = 230, 
                    BackColor = Color.White, 
                    Visible = true
                };
                pnlAdminRight.Controls.Add(flpAdmin);
            }

            // --- 3. SOL TARAFTAKİ FİLTRELER ---
            FlowLayoutPanel flpFilters = new FlowLayoutPanel 
            { 
                Dock = DockStyle.Fill, 
                BackColor = Color.Transparent, 
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 25, 0, 0),
                AutoSize = false,
                WrapContents = false 
            };

            // Arama Kutusu
            Label lblSearch = new Label { Text = "Ara:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(5, 5, 0, 0) };
            txtSearch = new TextBox { Width = 100, Font = new Font("Segoe UI", 10) };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) ApplyFilter(); };

            // Kategori Seçimi
            Label lblCat = new Label { Text = "Kat:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(10, 5, 0, 0) };
            cmbCategory = new ComboBox { Width = 110, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };

            // Tarih Seçimi
            Label lblDate = new Label { Text = "Trh:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(10, 5, 0, 0) };
            dtpDate = new DateTimePicker { Width = 100, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10), ShowCheckBox = true, Checked = false };

            // Ara Butonu (Büyüteç)
            btnFilter = new Button 
            { 
                Text = "🔍", 
                Size = new Size(40, 28), 
                BackColor = Color.SteelBlue, 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat, 
                Cursor = Cursors.Hand, 
                Font = new Font("Segoe UI", 11), 
                Margin = new Padding(10, -2, 0, 0) 
            };
            btnFilter.Click += (s, e) => ApplyFilter();

            // Temizle Butonu (X)
            btnClear = new Button 
            { 
                Text = "❌", 
                Size = new Size(40, 28), 
                BackColor = Color.LightGray, 
                ForeColor = Color.Black, 
                FlatStyle = FlatStyle.Flat, 
                Cursor = Cursors.Hand, 
                Font = new Font("Segoe UI", 10), 
                Margin = new Padding(5, -2, 0, 0) 
            };
            btnClear.Click += (s, e) => { 
                cmbCategory.SelectedIndex = 0; 
                dtpDate.Value = DateTime.Now; 
                dtpDate.Checked = false; 
                txtSearch.Text = ""; 
                LoadData(); 
            };

            flpFilters.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblCat, cmbCategory, lblDate, dtpDate, btnFilter, btnClear });

            // --- 4. YERLEŞİM SIRALAMASI (ÖNEMLİ) ---
            pnlTop.Controls.Add(flpFilters);    
            pnlTop.Controls.Add(pnlAdminRight); 
            
            // Admin paneli sağa yapışsın (Arka plana göndererek önceliği ona veriyoruz)
            pnlAdminRight.SendToBack(); 
            // Filtre paneli kalan boşluğu doldursun
            flpFilters.BringToFront();  

            this.Controls.Add(pnlTop);

            // --- 5. LİSTE PANELİ ---
            pnlEvents = new FlowLayoutPanel 
            { 
                Dock = DockStyle.Fill, 
                AutoScroll = true, 
                Padding = new Padding(50), 
                BackColor = Color.WhiteSmoke 
            };
            this.Controls.Add(pnlEvents);
            pnlEvents.BringToFront();

            LoadCategories();
        }

        private void LoadCategories()
        {
            try {
                var cats = _commonDal.GetAllCategories();
                cats.Insert(0, new Category { CategoryId = 0, CategoryName = "Tümü" });
                cmbCategory.DataSource = cats;
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryId";
            } catch { }
        }

        private void LoadData()
        {
            pnlEvents.Controls.Clear();
            try {
                var events = _eventDal.GetAllEvents();
                foreach (var evt in events) pnlEvents.Controls.Add(CreateEventCard(evt));
            } catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void ApplyFilter()
        {
            pnlEvents.Controls.Clear();
            try {
                // Kategori ID
                int? catId = null;
                if (cmbCategory.SelectedValue != null && int.TryParse(cmbCategory.SelectedValue.ToString(), out int val) && val > 0) 
                    catId = val;

                // Tarih (Sadece checkbox seçiliyse)
                DateTime? filterDate = dtpDate.Checked ? dtpDate.Value.Date : (DateTime?)null;
                
                // Arama Metni
                string searchText = txtSearch.Text.Trim();

                // Filtreli Getir (EventDal güncellenmiş olmalı)
                var events = _eventDal.GetEventsByFilter(catId, filterDate, searchText);
                
                if (events.Count == 0) {
                    Label lblEmpty = new Label { Text = "Etkinlik bulunamadı.", AutoSize = true, Font = new Font("Segoe UI", 12), ForeColor = Color.DimGray, Margin = new Padding(50) };
                    pnlEvents.Controls.Add(lblEmpty);
                } else {
                    foreach (var evt in events) pnlEvents.Controls.Add(CreateEventCard(evt));
                }
            } catch (Exception ex) { MessageBox.Show("Filtreleme Hatası: " + ex.Message); }
        }

        private Panel CreateEventCard(Event evt)
        {
            // Kart Çerçevesi
            Panel card = new Panel { Size = new Size(340, 450), BackColor = Color.White, Margin = new Padding(20), BorderStyle = BorderStyle.FixedSingle };
            
            // Resim
            PictureBox pb = new PictureBox { Dock = DockStyle.Top, Height = 180, SizeMode = PictureBoxSizeMode.StretchImage, BackColor = Color.LightGray };
            try { if (!string.IsNullOrEmpty(evt.PosterUrl)) pb.Image = Image.FromFile(evt.PosterUrl); } catch { }
            card.Controls.Add(pb);

            // Dinamik İçerik Konumu
            int currentY = 190; 
            int xPadding = 15;

            // Başlık
            Label lblTitle = new Label { 
                Text = evt.Title, 
                Font = new Font("Segoe UI", 13, FontStyle.Bold), 
                Location = new Point(xPadding, currentY), 
                AutoSize = true, 
                MaximumSize = new Size(310, 0) 
            };
            card.Controls.Add(lblTitle);
            currentY += lblTitle.Height + 10; 

            // Puan
            Label lblRating = new Label { 
                Text = evt.AverageRating > 0 ? $"⭐ {evt.AverageRating:N1}" : "⭐ Yeni", 
                Location = new Point(xPadding, currentY), 
                AutoSize = true, 
                ForeColor = Color.Orange,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            card.Controls.Add(lblRating);
            currentY += 25;

            // Tarih
            Label lblDate = new Label { Text = "📅 " + evt.EventDate.ToString("dd.MM.yyyy HH:mm"), Location = new Point(xPadding, currentY), AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 10) };
            card.Controls.Add(lblDate);
            currentY += 25;

            // Konum
            Label lblLoc = new Label { Text = "📍 " + evt.Location, Location = new Point(xPadding, currentY), AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Segoe UI", 10) };
            card.Controls.Add(lblLoc);
            
            // Alt Buton Paneli
            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(10) };
            
            // İncele Butonu
            Button btnDetail = new Button { 
                Text = "İNCELE", 
                Dock = DockStyle.Fill, 
                BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, 
                Cursor = Cursors.Hand, Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            // Detay sayfasına yönlendirme
            btnDetail.Click += (s, e) => OpenPage(new PageEventDetail(evt)); 
            pnlBottom.Controls.Add(btnDetail);

            // Admin veya Kulüp Yöneticisi ise DÜZENLE butonu ekle
            string role = Session.CurrentUser?.Role?.Trim()?.ToUpper();
            if (role == "ADMIN" || role == "CLUB_MANAGER")
            {
                btnDetail.Width = 200; 
                btnDetail.Dock = DockStyle.Left;

                Button btnEdit = new Button {
                    Text = "✏️",
                    Dock = DockStyle.Right,
                    Width = 60,
                    BackColor = Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand, Font = new Font("Segoe UI", 12)
                };
                
                // --- KRİTİK NOKTA: DÜZENLEME SAYFASINA YÖNLENDİRME ---
                // Eğer burası çalışıyor ama sayfa "Hazırlanıyor" diyorsa, sorun PageEventEdit.cs dosyasındadır.
                // Ancak bu kod doğru sayfayı açar.
                btnEdit.Click += (s, e) => OpenPage(new PageEventEdit(evt)); 
                
                pnlBottom.Controls.Add(btnEdit);
            }

            card.Controls.Add(pnlBottom);
            return card;
        }

        // Sayfa Değiştirme Yardımcısı
        private void OpenPage(UserControl page)
        {
            FormMain main = (FormMain)this.FindForm();
            if (main != null)
            {
                main.pnlContent.Controls.Clear();
                main.pnlContent.Controls.Add(page);
            }
        }
    }
}