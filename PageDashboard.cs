using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    // Bu sınıf, FormMain içindeki panele yüklenecek olan "Dashboard" sayfasıdır.
    public class PageDashboard : UserControl
    {
        // --- DAL ---
        private CommonDal _commonDal = new CommonDal();
        private AnnouncementDal _announcementDal = new AnnouncementDal();

        public PageDashboard()
        {
            this.Dock = DockStyle.Fill; // Paneli tamamen kapla
            this.BackColor = ColorTranslator.FromHtml("#ECF0F1"); // Arkaplan Rengi
            this.Padding = new Padding(20);

            InitializeUI();
        }

        private void InitializeUI()
        {
            // 1. ÜST KISIM: İstatistik Kartları Paneli
            FlowLayoutPanel pnlStats = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 150,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false
            };

            // Verileri Çek
            try 
            {
                var stats = _commonDal.GetDashboardStats();
                
                // Kartları Oluştur
                pnlStats.Controls.Add(CreateStatCard("Toplam Etkinlik", stats.TotalEvents.ToString(), "#3498DB")); // Mavi
                pnlStats.Controls.Add(CreateStatCard("Aktif Kulüpler", stats.TotalClubs.ToString(), "#E67E22"));  // Turuncu
                pnlStats.Controls.Add(CreateStatCard("Toplam Başvuru", stats.TotalApplications.ToString(), "#9B59B6")); // Mor
            }
            catch
            {
                // Veritabanı boşsa veya hata varsa dummy veri göster (Çökmemesi için)
                pnlStats.Controls.Add(CreateStatCard("Toplam Etkinlik", "-", "#3498DB"));
                pnlStats.Controls.Add(CreateStatCard("Aktif Kulüpler", "-", "#E67E22"));
                pnlStats.Controls.Add(CreateStatCard("Toplam Başvuru", "-", "#9B59B6"));
            }

            // 2. ALT KISIM: Duyurular Başlığı ve Tablosu
            Label lblAnnounce = new Label
            {
                Text = "Son Duyurular",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.BottomLeft
            };
            // Biraz boşluk bırakmak için dummy panel
            Panel pnlSpacer = new Panel { Dock = DockStyle.Top, Height = 20 }; 

            // Modern GridView (Tablo)
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false, // Sol baştaki boş kutuyu gizle
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Color.WhiteSmoke
            };

            // Tablo Tasarımı
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#2C3E50");
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;
            
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.ForeColor = Color.DimGray;
            grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#1ABC9C"); // Seçilince turkuaz ol
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.RowTemplate.Height = 35;

            // Veriyi Bağla
            try
            {
                grid.DataSource = _announcementDal.GetAllAnnouncements();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Duyurular yüklenemedi: " + ex.Message);
            }

            // Sayfaya Ekleme Sırası (Dock mantığı ters işler: En son eklenen en üstte kalırsa dikkat)
            // Dolayısıyla Fill olanı en başa, Top olanları sonra ekleyeceğiz ama Controls.Add sırası önemli.
            
            // Panel içine ekleyelim
            this.Controls.Add(grid);          // Fill (En altta yerleşir, kalanı kaplar)
            this.Controls.Add(lblAnnounce);   // Top
            this.Controls.Add(pnlSpacer);     // Top
            this.Controls.Add(pnlStats);      // Top (En üstte)
        }

        // Yardımcı Metot: Renkli Kutu Oluşturur
        private Panel CreateStatCard(string title, string value, string colorHex)
        {
            Panel card = new Panel
            {
                Width = 250,
                Height = 120,
                BackColor = ColorTranslator.FromHtml(colorHex),
                Margin = new Padding(0, 0, 20, 0) // Sağdan boşluk
            };

            Label lblVal = new Label
            {
                Text = value,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTitle = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(lblVal);
            card.Controls.Add(lblTitle);
            return card;
        }
    }
}