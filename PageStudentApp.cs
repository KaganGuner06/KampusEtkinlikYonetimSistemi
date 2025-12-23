using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    // Form yerine UserControl kullanıyoruz ki ana ekrana gömülebilsin
    public class PageStudentApps : UserControl
    {
        private DataGridView dgvMyApps;
        private Button btnRate; // Puanla Butonu
        private AppDal _appDal;

        public PageStudentApps()
        {
            // Form ayarları yerine Panel ayarları
            this.Dock = DockStyle.Fill; 
            this.BackColor = ColorTranslator.FromHtml("#ECF0F1");
            
            _appDal = new AppDal();

            SetupUI();
            LoadMyApps();
        }

        private void SetupUI()
        {
            // 1. HEADER (Başlık Paneli)
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(20) };
            Label lblTitle = new Label { Text = "Başvuru Geçmişim", AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#2C3E50"), Location = new Point(20, 15) };
            pnlHeader.Controls.Add(lblTitle);

            // 2. TABLO
            dgvMyApps = new DataGridView();
            dgvMyApps.Dock = DockStyle.Fill; // Alanı kapla
            dgvMyApps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMyApps.ReadOnly = true; 
            dgvMyApps.SelectionMode = DataGridViewSelectionMode.FullRowSelect; 
            dgvMyApps.MultiSelect = false;
            dgvMyApps.BackgroundColor = Color.WhiteSmoke;
            dgvMyApps.BorderStyle = BorderStyle.None;
            
            // Tablo Görsel Ayarları
            dgvMyApps.EnableHeadersVisualStyles = false;
            dgvMyApps.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#3498DB");
            dgvMyApps.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMyApps.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMyApps.ColumnHeadersHeight = 40;

            // 3. PUANLA BUTONU (Alt Panelde)
            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(20) };
            
            btnRate = new Button { 
                Text = "⭐ SEÇİLİ ETKİNLİĞİ PUANLA", 
                Dock = DockStyle.Right, // Sağa yasla
                Width = 250, 
                BackColor = Color.Orange, 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Visible = false
            };

            btnRate.Click += (s, e) => {
                if (dgvMyApps.SelectedRows.Count == 0) 
                {
                    MessageBox.Show("Lütfen puanlamak için bir etkinlik seçin.");
                    return;
                }

                // Tablodaki verileri al
                // Not: AppDal.GetApplicationsByUser metodundaki sütun adlarına göre (Id, Etkinlik)
                try {
                    int eventId = Convert.ToInt32(dgvMyApps.SelectedRows[0].Cells["Id"].Value); // AppDal'da 'Id' demiştik
                    string title = dgvMyApps.SelectedRows[0].Cells["Etkinlik"].Value.ToString()!;

                    // Puanlama Formunu Aç
                    new FormRateEvent(eventId, title).ShowDialog();
                }
                catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            };

            pnlBottom.Controls.Add(btnRate);

            // Kontrolleri Ekleme Sırası (Dock mantığına göre)
            this.Controls.Add(dgvMyApps); // Fill (Orta)
            this.Controls.Add(pnlBottom); // Bottom
            this.Controls.Add(pnlHeader); // Top
        }

        private void LoadMyApps()
        {
            if (Session.CurrentUser != null)
            {
                // AppDal içindeki GetApplicationsByUser metodunu çağırıyoruz
                dgvMyApps.DataSource = _appDal.GetApplicationsByUser(Session.CurrentUser.UserId);
                
                // ID sütununu gizleyelim
                if (dgvMyApps.Columns.Contains("Id"))
                {
                    dgvMyApps.Columns["Id"].Visible = false; 
                }
            }
        }
    }
}