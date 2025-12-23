using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public class PageAnnouncements : UserControl
    {
        private AnnouncementDal _announcementDal = new AnnouncementDal();
        private DataGridView dgvAnnouncements;
        private Button btnAdd;

        public PageAnnouncements()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorTranslator.FromHtml("#ECF0F1");
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            // HEADER
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White, Padding = new Padding(30, 20, 30, 0) };
            Label lblTitle = new Label { Text = "Duyurular", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#2C3E50"), AutoSize = true, Location = new Point(30, 20) };
            
            // ADMIN/YÖNETİCİ Butonu
            string role = Session.CurrentUser?.Role?.Trim()?.ToUpper() ?? "";
            if (role == "ADMIN" || role == "CLUB_MANAGER")
            {
                btnAdd = new Button { Text = "+ DUYURU EKLE", BackColor = ColorTranslator.FromHtml("#E67E22"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Size = new Size(150, 40), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right, Location = new Point(pnlHeader.Width - 180, 20) };
                btnAdd.Click += (s, e) => {
                    if (new FormAddAnnouncement().ShowDialog() == DialogResult.OK) LoadData();
                };
                pnlHeader.Controls.Add(btnAdd);
            }
            pnlHeader.Controls.Add(lblTitle);

            // TABLO
            dgvAnnouncements = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = ColorTranslator.FromHtml("#ECF0F1"),
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowTemplate = { Height = 40 }
            };

            // Görsel Ayarlar
            dgvAnnouncements.EnableHeadersVisualStyles = false;
            dgvAnnouncements.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#34495E");
            dgvAnnouncements.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAnnouncements.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvAnnouncements.ColumnHeadersHeight = 45;
            dgvAnnouncements.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvAnnouncements.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#1ABC9C");

            // --- YENİ EKLENEN KISIM: ÇİFT TIKLAMA OLAYI ---
            dgvAnnouncements.CellDoubleClick += (s, e) => 
            {
                if (e.RowIndex >= 0) // Başlığa tıklanmadıysa
                {
                    // Seçili satırdaki verileri al (SQL sorgundaki AS isimlerine dikkat!)
                    string title = dgvAnnouncements.Rows[e.RowIndex].Cells["Konu"].Value.ToString();
                    string content = dgvAnnouncements.Rows[e.RowIndex].Cells["Mesaj"].Value.ToString();
                    string club = dgvAnnouncements.Rows[e.RowIndex].Cells["Kulüp"].Value.ToString();
                    string date = dgvAnnouncements.Rows[e.RowIndex].Cells["Tarih"].Value.ToString();

                    // Detay penceresini aç
                    new FormAnnouncementDetail(title, content, club, date).ShowDialog();
                }
            };
            // ----------------------------------------------

            Panel pnlGridContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30) };
            pnlGridContainer.Controls.Add(dgvAnnouncements);

            this.Controls.Add(pnlGridContainer);
            this.Controls.Add(pnlHeader);
        }

        private void LoadData()
        {
            try
            {
                dgvAnnouncements.DataSource = _announcementDal.GetAllAnnouncements();
                // SQL sorgusundaki 'AS' isimlerine göre sütun ayarları
                if (dgvAnnouncements.Columns["Konu"] != null) dgvAnnouncements.Columns["Konu"].FillWeight = 30;
                if (dgvAnnouncements.Columns["Mesaj"] != null) dgvAnnouncements.Columns["Mesaj"].FillWeight = 50; 
                if (dgvAnnouncements.Columns["Kulüp"] != null) dgvAnnouncements.Columns["Kulüp"].FillWeight = 15;
                if (dgvAnnouncements.Columns["Tarih"] != null) dgvAnnouncements.Columns["Tarih"].FillWeight = 15;
            }
            catch { }
        }
    }
}