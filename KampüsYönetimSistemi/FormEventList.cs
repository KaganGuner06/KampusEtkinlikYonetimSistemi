using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public partial class FormEventList : Form
    {
        
        private DataGridView dgvEvents;
        private Panel pnlTop; 
        
        
        private GroupBox grpAdminTools;
        private TextBox txtTitle, txtLocation, txtQuota;
        private DateTimePicker dtpDate;
        private ComboBox cmbCategory, cmbClub;
        private Button btnAdd, btnUpdate, btnDelete;
        
        
        private Button btnApply;
        private ComboBox cmbFilterCat;
        private Button btnFilter;
        
        private DateTimePicker dtpFilterDate; 
        private Label lblFilterDate;

        
        private EventDal _eventDal;
        private AppDal _appDal;
        private CommonDal _commonDal;
        
        private int _selectedEventId = 0;

        public FormEventList()
        {
            this.Text = "Etkinlik Yönetim ve Başvuru Ekranı";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            _eventDal = new EventDal();
            _appDal = new AppDal();
            _commonDal = new CommonDal();

            SetupUI();
            
            
            if (Session.CurrentUser != null)
            {
                ApplyRoleLogic();
            }
            
            RefreshData();
        }

        private void SetupUI()
        {
            
            pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke, Padding = new Padding(10) };
            this.Controls.Add(pnlTop);

            
            grpAdminTools = new GroupBox { Text = "Yönetici İşlemleri", Dock = DockStyle.Top, Height = 170, Visible = false, BackColor = Color.AliceBlue };
            
            
            Label l1 = new Label { Text = "Başlık:", Location = new Point(20, 30), AutoSize = true };
            txtTitle = new TextBox { Location = new Point(90, 27), Width = 180 };
            
            Label l2 = new Label { Text = "Tarih:", Location = new Point(300, 30), AutoSize = true };
            dtpDate = new DateTimePicker { Location = new Point(360, 27), Width = 110, Format = DateTimePickerFormat.Short };

            Label l3 = new Label { Text = "Kota:", Location = new Point(500, 30), AutoSize = true };
            txtQuota = new TextBox { Location = new Point(550, 27), Width = 60, Text = "50" };

            Button btnCatManage = new Button { 
                Text = "📂 KATEGORİLER", Location = new Point(640, 25), Width = 120, Height = 30, 
                BackColor = Color.Gold, Font = new Font("Segoe UI", 8, FontStyle.Bold) 
            };
            btnCatManage.Click += (s, e) => { new FormCategories().ShowDialog(); LoadComboBoxes(); };

            
            Label l4 = new Label { Text = "Konum:", Location = new Point(20, 75), AutoSize = true };
            txtLocation = new TextBox { Location = new Point(90, 72), Width = 180 };

            Label l5 = new Label { Text = "Kategori:", Location = new Point(300, 75), AutoSize = true };
            cmbCategory = new ComboBox { Location = new Point(360, 72), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            
            Label l6 = new Label { Text = "Kulüp:", Location = new Point(520, 75), AutoSize = true };
            cmbClub = new ComboBox { Location = new Point(570, 72), Width = 190, DropDownStyle = ComboBoxStyle.DropDownList };

            
            btnAdd = new Button { Text = "EKLE", Location = new Point(20, 120), Width = 100, Height = 35, BackColor = Color.LightGreen };
            btnAdd.Click += BtnAdd_Click;

            btnUpdate = new Button { Text = "GÜNCELLE", Location = new Point(130, 120), Width = 100, Height = 35, BackColor = Color.LightSkyBlue };
            btnUpdate.Click += BtnUpdate_Click;

            btnDelete = new Button { Text = "SİL", Location = new Point(240, 120), Width = 100, Height = 35, BackColor = Color.LightCoral };
            btnDelete.Click += BtnDelete_Click;

            grpAdminTools.Controls.AddRange(new Control[] { l1, txtTitle, l2, dtpDate, l3, txtQuota, btnCatManage, l4, txtLocation, l5, cmbCategory, l6, cmbClub, btnAdd, btnUpdate, btnDelete });
            this.Controls.Add(grpAdminTools);

            
            dgvEvents = new DataGridView {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };
            dgvEvents.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvEvents.CellClick += DgvEvents_CellClick;
            
            this.Controls.Add(dgvEvents);
            dgvEvents.BringToFront(); 
            pnlTop.SendToBack(); 
            grpAdminTools.SendToBack();
        }

        private void ApplyRoleLogic()
        {
            string role = Session.CurrentUser?.Role?.Trim()?.ToUpper() ?? "STUDENT";

            if (role == "CLUB_MANAGER" || role == "ADMIN")
            {
                
                grpAdminTools.Visible = true; 
                LoadComboBoxes(); 
            }
            else
            {
                
                grpAdminTools.Visible = false;
                
                
                Label lblFilter = new Label { Text = "Kategori:", Location = new Point(20, 23), AutoSize = true, Font = new Font("Segoe UI", 9) };
                cmbFilterCat = new ComboBox { Location = new Point(80, 20), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
                
                
                lblFilterDate = new Label { Text = "Tarih:", Location = new Point(240, 23), AutoSize = true, Font = new Font("Segoe UI", 9) };
                dtpFilterDate = new DateTimePicker { Location = new Point(280, 20), Width = 110, Format = DateTimePickerFormat.Short };

                
                btnFilter = new Button { Text = "🔍 Ara", Location = new Point(400, 19), Width = 80, Height = 28, BackColor = Color.WhiteSmoke };
                
                
                btnFilter.Click += (s, e) => {
                    try {
                        int? catId = null;
                        if (cmbFilterCat.SelectedIndex > 0 && cmbFilterCat.SelectedValue is int val) catId = val;
                        
                        dgvEvents.DataSource = _eventDal.GetEventsByFilter(catId, dtpFilterDate.Value.Date);
                    }
                    catch (Exception ex) { MessageBox.Show("Filtreleme hatası: " + ex.Message); }
                };

                
                btnApply = new Button { Text = "✅ SEÇİLİ ETKİNLİĞE BAŞVUR", Location = new Point(750, 12), Width = 250, Height = 40, BackColor = Color.Gold, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                btnApply.Click += BtnStudentApply_Click;

                try { 
                    var cats = _commonDal.GetAllCategories();
                    cats.Insert(0, new Category { CategoryId = 0, CategoryName = "Tümü" });
                    cmbFilterCat.DataSource = cats; 
                    cmbFilterCat.DisplayMember = "CategoryName"; cmbFilterCat.ValueMember = "CategoryId";
                } catch {}

                pnlTop.Controls.AddRange(new Control[] { lblFilter, cmbFilterCat, lblFilterDate, dtpFilterDate, btnFilter, btnApply });
            }
        }

        private void LoadComboBoxes()
        {
            try {
                cmbCategory.DataSource = _commonDal.GetAllCategories();
                cmbCategory.DisplayMember = "CategoryName"; cmbCategory.ValueMember = "CategoryId";

                cmbClub.DataSource = _commonDal.GetAllClubs();
                cmbClub.DisplayMember = "ClubName"; cmbClub.ValueMember = "ClubId";
            } catch { }
        }

        private void RefreshData()
        {
            try
            {
                var list = _eventDal.GetAllEvents();
                dgvEvents.DataSource = list;

                if (dgvEvents.Columns.Contains("EventId")) dgvEvents.Columns["EventId"].Visible = false;
                if (dgvEvents.Columns.Contains("ClubId")) dgvEvents.Columns["ClubId"].Visible = false;
                if (dgvEvents.Columns.Contains("CategoryId")) dgvEvents.Columns["CategoryId"].Visible = false;
                if (dgvEvents.Columns.Contains("IsPublished")) dgvEvents.Columns["IsPublished"].Visible = false;
                if (dgvEvents.Columns.Contains("PosterUrl")) dgvEvents.Columns["PosterUrl"].Visible = false;

                if (dgvEvents.Columns.Contains("Title")) dgvEvents.Columns["Title"].HeaderText = "Başlık";
                if (dgvEvents.Columns.Contains("Description")) dgvEvents.Columns["Description"].HeaderText = "Açıklama";
                if (dgvEvents.Columns.Contains("EventDate")) dgvEvents.Columns["EventDate"].HeaderText = "Tarih";
                if (dgvEvents.Columns.Contains("Location")) dgvEvents.Columns["Location"].HeaderText = "Konum";
                if (dgvEvents.Columns.Contains("Quota")) dgvEvents.Columns["Quota"].HeaderText = "Kota";
                if (dgvEvents.Columns.Contains("ClubName")) dgvEvents.Columns["ClubName"].HeaderText = "Kulüp";
                
                
                if (dgvEvents.Columns.Contains("AverageRating"))
                {
                    dgvEvents.Columns["AverageRating"].HeaderText = "Puan ⭐";
                    dgvEvents.Columns["AverageRating"].DefaultCellStyle.Format = "N1";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken hata: " + ex.Message);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try {
                if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbCategory.SelectedIndex == -1 || cmbClub.SelectedIndex == -1)
                {
                    MessageBox.Show("Zorunlu alanları doldurun.");
                    return;
                }

                Event newEvent = new Event
                {
                    Title = txtTitle.Text,
                    Description = "Açıklama girilmedi",
                    Location = txtLocation.Text,
                    EventDate = dtpDate.Value,
                    Quota = int.TryParse(txtQuota.Text, out int q) ? q : 50,
                    CategoryId = (int)cmbCategory.SelectedValue,
                    ClubId = (int)cmbClub.SelectedValue,
                    PosterUrl = "default.jpg",
                    IsPublished = true
                };
                _eventDal.AddEvent(newEvent);
                MessageBox.Show("Eklendi! ✅");
                RefreshData();
            } catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (_selectedEventId == 0) return;
            try {
                Event evt = new Event
                {
                    EventId = _selectedEventId,
                    Title = txtTitle.Text,
                    Description = "Güncellendi",
                    Location = txtLocation.Text,
                    EventDate = dtpDate.Value,
                    Quota = int.TryParse(txtQuota.Text, out int q) ? q : 50,
                    CategoryId = (int)cmbCategory.SelectedValue,
                    ClubId = (int)cmbClub.SelectedValue,
                    PosterUrl = "default.jpg",
                    IsPublished = true
                };
                _eventDal.UpdateEvent(evt);
                MessageBox.Show("Güncellendi! ✏️");
                RefreshData();
            } catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_selectedEventId == 0) return;
            if (MessageBox.Show("Silinsin mi?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                _eventDal.DeleteEvent(_selectedEventId);
                RefreshData();
            }
        }

        private void DgvEvents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvEvents.Rows[e.RowIndex];
            _selectedEventId = Convert.ToInt32(row.Cells["EventId"].Value);

            if (grpAdminTools.Visible)
            {
                txtTitle.Text = row.Cells["Title"].Value.ToString();
                txtLocation.Text = row.Cells["Location"].Value.ToString();
                txtQuota.Text = row.Cells["Quota"].Value.ToString();
                dtpDate.Value = Convert.ToDateTime(row.Cells["EventDate"].Value);
                try { cmbCategory.SelectedValue = Convert.ToInt32(row.Cells["CategoryId"].Value); } catch {}
                try { cmbClub.SelectedValue = Convert.ToInt32(row.Cells["ClubId"].Value); } catch {}
            }
        }

        private void BtnStudentApply_Click(object? sender, EventArgs e)
        {
            if (dgvEvents.SelectedRows.Count == 0) 
            {
                MessageBox.Show("Lütfen önce listeden etkinlik seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _selectedEventId = Convert.ToInt32(dgvEvents.SelectedRows[0].Cells["EventId"].Value);
            
            try 
            {
                _appDal.ApplyToEvent(_selectedEventId, Session.CurrentUser!.UserId);
                MessageBox.Show("Başvurunuz Başarıyla Alındı! ✅", "Tebrikler", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } 
            catch (Exception ex) 
            { 
                if (ex.Message.Contains("23505") || ex.Message.Contains("uq_app_user_event"))
                    MessageBox.Show("⚠️ ZATEN başvurdunuz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}