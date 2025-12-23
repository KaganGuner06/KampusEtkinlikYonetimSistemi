using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;
using System.Diagnostics; 

namespace CampusEventManager
{
    public partial class FormClubDetail : Form
    {
        private int _clubId;
        private ClubDal _clubDal;
        private EventDal _eventDal;
        private Club _currentClub;

        // UI Elemanları
        private PictureBox pbCover, pbLogo;
        private Label lblName, lblCategory;
        private TabControl tabControl;
        private Button btnJoin, btnDelete, btnEdit;
        private DataGridView dgvEvents, dgvMembers;

        public FormClubDetail(int clubId)
        {
            _clubId = clubId;
            
            // Veritabanı nesnelerini oluştur
            _clubDal = new ClubDal();
            _eventDal = new EventDal(); 

            this.Text = "Kulüp Detayı";
            this.Size = new Size(950, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            try 
            {
                _currentClub = _clubDal.GetClubById(_clubId);
            }
            catch { }

            if (_currentClub == null)
            {
                // Sessizce kapat, popup gösterme
                this.Close();
                return;
            }

            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            // Kapak ve Bilgi Paneli
            pbCover = new PictureBox { Dock = DockStyle.Top, Height = 220, BackColor = Color.LightGray, SizeMode = PictureBoxSizeMode.StretchImage };
            this.Controls.Add(pbCover);

            Panel pnlInfo = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.WhiteSmoke };
            this.Controls.Add(pnlInfo);

            pbLogo = new PictureBox { Location = new Point(30, 10), Size = new Size(80, 80), SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            pnlInfo.Controls.Add(pbLogo);

            lblName = new Label { Text = _currentClub.ClubName, Location = new Point(130, 20), Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true };
            pnlInfo.Controls.Add(lblName);

            lblCategory = new Label { Text = "Kategori ID: " + _currentClub.CategoryId, Location = new Point(130, 55), Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.Gray, AutoSize = true };
            pnlInfo.Controls.Add(lblCategory);

            // Yetki Kontrolü
            string role = Session.CurrentUser?.Role?.Trim()?.ToUpper() ?? "STUDENT";
            int currentUserId = Session.CurrentUser?.UserId ?? 0;
            bool isManager = _currentClub.ManagerUserId == currentUserId;
            
            if (role == "ADMIN" || isManager)
            {
                btnDelete = new Button { Text = "SİL", Location = new Point(800, 30), Size = new Size(100, 45), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
                btnDelete.Click += BtnDelete_Click;
                pnlInfo.Controls.Add(btnDelete);

                btnEdit = new Button { Text = "DÜZENLE", Location = new Point(680, 30), Size = new Size(110, 45), BackColor = Color.Orange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
                btnEdit.Click += BtnEdit_Click;
                pnlInfo.Controls.Add(btnEdit);
            }
            else
            {
                btnJoin = new Button { Text = "KULÜBE KATIL", Location = new Point(750, 30), Size = new Size(150, 45), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
                btnJoin.Click += BtnJoin_Click;
                pnlInfo.Controls.Add(btnJoin);
            }

            // Sekmeler
            tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11) };
            tabControl.TabPages.Add(new TabPage("Hakkında"));
            
            TabPage tabEvents = new TabPage("Etkinlikler");
            dgvEvents = CreateGrid(); 
            tabEvents.Controls.Add(dgvEvents);
            tabControl.TabPages.Add(tabEvents);

            if (role == "ADMIN" || isManager)
            {
                TabPage tabMembers = new TabPage("Üyeler");
                dgvMembers = CreateGrid();
                tabMembers.Controls.Add(dgvMembers);
                tabControl.TabPages.Add(tabMembers);
            }

            this.Controls.Add(tabControl);
            tabControl.BringToFront();
        }

        private DataGridView CreateGrid()
        {
            return new DataGridView { 
                Dock = DockStyle.Fill, 
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, 
                BackgroundColor = Color.White, 
                BorderStyle = BorderStyle.None, 
                ReadOnly = true, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                AllowUserToAddRows = false, 
                RowHeadersVisible = false 
            };
        }

        private void LoadData()
        {
            try {
                _currentClub = _clubDal.GetClubById(_clubId);
                lblName.Text = _currentClub.ClubName;
            } catch { }

            try { if(!string.IsNullOrEmpty(_currentClub.CoverUrl)) pbCover.Image = Image.FromFile(_currentClub.CoverUrl); } catch { }
            try { if(!string.IsNullOrEmpty(_currentClub.LogoUrl)) pbLogo.Image = Image.FromFile(_currentClub.LogoUrl); } catch { }

            FillAboutTab();

            // --- ETKİNLİKLERİ YÜKLE (SESSİZ MOD) ---
            try 
            {
                if (_eventDal == null) _eventDal = new EventDal(); 

                var dt = _eventDal.GetEventsByClub(_clubId);
                dgvEvents.DataSource = dt;

                // Sütun adı kontrolü (Hata vermez, varsa yapar yoksa geçer)
                string colPuan = null;
                if (dgvEvents.Columns.Contains("Puan")) colPuan = "Puan";
                else if (dgvEvents.Columns.Contains("puan")) colPuan = "puan";
                else if (dgvEvents.Columns.Contains("avg_rating")) colPuan = "avg_rating";

                if (colPuan != null)
                {
                    dgvEvents.Columns[colPuan].HeaderText = "Ort. Puan ⭐";
                    dgvEvents.Columns[colPuan].DefaultCellStyle.Format = "N1"; 
                    dgvEvents.Columns[colPuan].Width = 120;
                }
                
                if (dgvEvents.Columns.Contains("event_id")) dgvEvents.Columns["event_id"].Visible = false;
            } 
            catch
            { 
                // Hata olursa kullanıcıya popup gösterme, sadece logla veya sessiz kal.
                // Bu sayede o sinir bozucu kutu çıkmaz.
            }

            // --- ÜYELER ---
            if (dgvMembers != null)
            {
                try { dgvMembers.DataSource = _clubDal.GetClubMembers(_clubId); } catch { }
            }
        }

        private void FillAboutTab()
        {
            TabPage page = tabControl.TabPages[0];
            page.Controls.Clear();
            page.AutoScroll = true;
            page.BackColor = Color.White;

            Label lFull = new Label { 
                Text = string.IsNullOrEmpty(_currentClub.FullDescription) ? _currentClub.Description : _currentClub.FullDescription, 
                Location = new Point(20, 20), 
                Width = 880, 
                AutoSize = true, 
                Font = new Font("Segoe UI", 11) 
            };
            lFull.MaximumSize = new Size(880, 0); 
            page.Controls.Add(lFull);

            if (!string.IsNullOrEmpty(_currentClub.InstagramLink))
            {
                int y = lFull.Bottom + 20;
                LinkLabel lnkInsta = new LinkLabel { Text = "Instagram: " + _currentClub.InstagramLink, Location = new Point(20, y), AutoSize = true, LinkColor = Color.Purple };
                lnkInsta.LinkClicked += (s, e) => { try { Process.Start(new ProcessStartInfo { FileName = _currentClub.InstagramLink, UseShellExecute = true }); } catch {} };
                page.Controls.Add(lnkInsta);
            }
        }

        // --- BUTON İŞLEVLERİ ---

        private void BtnJoin_Click(object sender, EventArgs e)
        {
            try {
                int userId = Session.CurrentUser?.UserId ?? 0;
                if (userId == 0) { MessageBox.Show("Giriş yapmalısınız."); return; }
                
                _clubDal.JoinClub(_clubId, userId); 
                MessageBox.Show("Başarıyla katıldınız!");
                btnJoin.Enabled = false; 
                btnJoin.Text = "ÜYESİNİZ";
                btnJoin.BackColor = Color.Gray;
            } 
            catch (Exception ex) { MessageBox.Show("Bilgi: " + ex.Message); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bu kulübü silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                try { _clubDal.DeleteClub(_clubId); this.Close(); } catch { }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            FormAddClub frm = new FormAddClub(_currentClub);
            if (frm.ShowDialog() == DialogResult.OK) LoadData();
        }
    }
}