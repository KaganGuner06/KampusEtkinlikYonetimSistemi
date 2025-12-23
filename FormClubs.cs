using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public partial class FormClubs : Form
    {
        private DataGridView dgvClubs;
        private Button btnAddClub;
        private ClubDal _clubDal;

        public FormClubs()
        {
            this.Text = "Kulüp Listesi";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;

            _clubDal = new ClubDal();

            SetupUI();
            LoadClubs();
        }

        private void SetupUI()
        {
            // Üst Panel (Başlık ve Ekle Butonu)
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White };
            this.Controls.Add(pnlTop);

            Label lblTitle = new Label { Text = "KULÜPLER", Location = new Point(20, 25), Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, ForeColor = Color.DarkSlateBlue };
            pnlTop.Controls.Add(lblTitle);

            btnAddClub = new Button { Text = "+ YENİ KULÜP", Location = new Point(600, 25), Size = new Size(150, 40), BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnAddClub.Click += BtnAddClub_Click;
            pnlTop.Controls.Add(btnAddClub);

            // Tablo (Liste)
            dgvClubs = new DataGridView();
            dgvClubs.Dock = DockStyle.Fill;
            dgvClubs.BackgroundColor = Color.WhiteSmoke;
            dgvClubs.BorderStyle = BorderStyle.None;
            dgvClubs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClubs.RowHeadersVisible = false;
            dgvClubs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClubs.AllowUserToAddRows = false;
            dgvClubs.ReadOnly = true;
            
            // Tablo Stili
            dgvClubs.EnableHeadersVisualStyles = false;
            dgvClubs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvClubs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvClubs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvClubs.ColumnHeadersHeight = 40;
            dgvClubs.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvClubs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 204, 113);

            // Olay: Çift Tıklama
            dgvClubs.CellDoubleClick += DgvClubs_CellDoubleClick;

            this.Controls.Add(dgvClubs);
            dgvClubs.BringToFront(); // Tabloyu öne getir
        }

        private void LoadClubs()
        {
            try
            {
                dgvClubs.DataSource = _clubDal.GetAllClubs();
                
                // İstenmeyen kolonları gizleyebilirsin (Opsiyonel)
                if(dgvClubs.Columns["FullDescription"] != null) dgvClubs.Columns["FullDescription"].Visible = false;
                if(dgvClubs.Columns["LogoUrl"] != null) dgvClubs.Columns["LogoUrl"].Visible = false;
                if(dgvClubs.Columns["CoverUrl"] != null) dgvClubs.Columns["CoverUrl"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Liste yüklenirken hata: " + ex.Message);
            }
        }

        private void BtnAddClub_Click(object sender, EventArgs e)
        {
            FormAddClub frm = new FormAddClub();
            // Form kapandığında (DialogResult.OK dönerse) listeyi yenile
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadClubs();
            }
        }

        private void DgvClubs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // ID'yi al (Senin Entity'deki isim 'ClubId')
            int id = Convert.ToInt32(dgvClubs.Rows[e.RowIndex].Cells["ClubId"].Value);
            
            FormClubDetail frm = new FormClubDetail(id);
            frm.ShowDialog();
        }
    }
}