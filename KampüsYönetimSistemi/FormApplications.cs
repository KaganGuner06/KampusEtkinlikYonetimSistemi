using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public partial class FormApplications : Form
    {
        
        private ComboBox cmbEvents;
        private Button btnApprove;
        private Button btnReject; 
        private DataGridView dgvApps;
        private Label lblStatus;

        
        private EventDal _eventDal;
        private AppDal _appDal;

        public FormApplications()
        {
            this.Text = "Başvuru Onay Merkezi";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen; 

            _eventDal = new EventDal();
            _appDal = new AppDal();

            SetupUI();
            
            
            LoadData(); 
        }

        private void SetupUI()
        {
            
            Label lblEvent = new Label { 
                Text = "Başvuruları Görüntülenecek Etkinliği Seçin:", 
                Location = new Point(20, 20), 
                AutoSize = true, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };
            
            cmbEvents = new ComboBox { 
                Location = new Point(20, 45), 
                Width = 740, 
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            
            
            cmbEvents.SelectedIndexChanged += CmbEvents_SelectedIndexChanged;

            
            dgvApps = new DataGridView();
            dgvApps.Location = new Point(20, 90);
            dgvApps.Size = new Size(740, 350);
            dgvApps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvApps.ReadOnly = true;
            dgvApps.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvApps.MultiSelect = false;
            dgvApps.BackgroundColor = Color.WhiteSmoke;
            dgvApps.RowHeadersVisible = false;
            dgvApps.AllowUserToAddRows = false;

            
            
            
            btnApprove = new Button { 
                Text = "✅ SEÇİLİ BAŞVURUYU ONAYLA", 
                Location = new Point(20, 450), 
                Size = new Size(360, 50), 
                BackColor = Color.MediumSeaGreen, 
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnApprove.Click += BtnApprove_Click;

            
            btnReject = new Button { 
                Text = "❌ SİL / REDDET", 
                Location = new Point(400, 450), 
                Size = new Size(360, 50), 
                BackColor = ColorTranslator.FromHtml("#E74C3C"), 
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnReject.Click += BtnReject_Click;

            
            lblStatus = new Label { 
                Text = "İşlem yapmak için yukarıdan etkinlik seçiniz.", 
                Location = new Point(20, 510), 
                AutoSize = true, 
                ForeColor = Color.DimGray 
            };

            this.Controls.Add(lblEvent); 
            this.Controls.Add(cmbEvents);
            this.Controls.Add(dgvApps);
            this.Controls.Add(btnApprove); 
            this.Controls.Add(btnReject); 
            this.Controls.Add(lblStatus);
        }

        private void LoadData()
        {
            try
            {
                
                cmbEvents.SelectedIndexChanged -= CmbEvents_SelectedIndexChanged;

                cmbEvents.DataSource = null;

                
                cmbEvents.DisplayMember = "Title";
                cmbEvents.ValueMember = "EventId";

                
                cmbEvents.DataSource = _eventDal.GetAllEvents();
                
                
                cmbEvents.SelectedIndex = -1;

                
                cmbEvents.SelectedIndexChanged += CmbEvents_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken hata: " + ex.Message);
            }
        }

        private void CmbEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            
            
            
            if (cmbEvents.SelectedIndex == -1) return;

            
            if (cmbEvents.SelectedValue == null) return;

            
            
            if (!(cmbEvents.SelectedValue is int)) return;

            try 
            {
                
                int eventId = (int)cmbEvents.SelectedValue;
                
                DataTable dt = _appDal.GetApplicationsByEvent(eventId);
                dgvApps.DataSource = dt;
                
                
                if (dgvApps.Columns.Contains("application_id")) dgvApps.Columns["application_id"].Visible = false;
                if (dgvApps.Columns.Contains("event_id")) dgvApps.Columns["event_id"].Visible = false;
                if (dgvApps.Columns.Contains("user_id")) dgvApps.Columns["user_id"].Visible = false;
                if (dgvApps.Columns.Contains("email")) dgvApps.Columns["email"].Visible = false;

                if (dgvApps.Columns.Contains("participant_name")) dgvApps.Columns["participant_name"].HeaderText = "Katılımcı Adı";
                if (dgvApps.Columns.Contains("application_status")) dgvApps.Columns["application_status"].HeaderText = "Durum";
                if (dgvApps.Columns.Contains("applied_at")) dgvApps.Columns["applied_at"].HeaderText = "Başvuru Tarihi";

                lblStatus.Text = $"{dt.Rows.Count} başvuru listelendi.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Liste yenilenirken hata: " + ex.Message);
            }
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (dgvApps.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen listeden onaylanacak kişiyi seçin.");
                return;
            }

            try
            {
                int appId = Convert.ToInt32(dgvApps.SelectedRows[0].Cells["application_id"].Value);
                string currentStatus = dgvApps.SelectedRows[0].Cells["application_status"].Value.ToString();

                if (currentStatus == "APPROVED")
                {
                    MessageBox.Show("Bu başvuru zaten onaylanmış.");
                    return;
                }

                _appDal.ApproveApplication(appId);
                MessageBox.Show("Başvuru Onaylandı! 🚀");
                RefreshGrid(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (dgvApps.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek başvuruyu seçin.");
                return;
            }

            if (MessageBox.Show("Bu başvuruyu silmek/reddetmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    int appId = Convert.ToInt32(dgvApps.SelectedRows[0].Cells["application_id"].Value);
                    _appDal.RemoveApplication(appId);
                    MessageBox.Show("Başvuru silindi.");
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }
    }
}