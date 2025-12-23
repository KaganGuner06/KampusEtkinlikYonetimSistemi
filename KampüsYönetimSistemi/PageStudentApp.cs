using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    
    public class PageStudentApps : UserControl
    {
        private DataGridView dgvMyApps;
        private Button btnRate; 
        private AppDal _appDal;

        public PageStudentApps()
        {
            
            this.Dock = DockStyle.Fill; 
            this.BackColor = ColorTranslator.FromHtml("#ECF0F1");
            
            _appDal = new AppDal();

            SetupUI();
            LoadMyApps();
        }

        private void SetupUI()
        {
            
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(20) };
            Label lblTitle = new Label { Text = "Başvuru Geçmişim", AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#2C3E50"), Location = new Point(20, 15) };
            pnlHeader.Controls.Add(lblTitle);

            
            dgvMyApps = new DataGridView();
            dgvMyApps.Dock = DockStyle.Fill; 
            dgvMyApps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMyApps.ReadOnly = true; 
            dgvMyApps.SelectionMode = DataGridViewSelectionMode.FullRowSelect; 
            dgvMyApps.MultiSelect = false;
            dgvMyApps.BackgroundColor = Color.WhiteSmoke;
            dgvMyApps.BorderStyle = BorderStyle.None;
            
            
            dgvMyApps.EnableHeadersVisualStyles = false;
            dgvMyApps.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#3498DB");
            dgvMyApps.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMyApps.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMyApps.ColumnHeadersHeight = 40;

            
            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(20) };
            
            btnRate = new Button { 
                Text = "⭐ SEÇİLİ ETKİNLİĞİ PUANLA", 
                Dock = DockStyle.Right, 
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

                
                
                try {
                    int eventId = Convert.ToInt32(dgvMyApps.SelectedRows[0].Cells["Id"].Value); 
                    string title = dgvMyApps.SelectedRows[0].Cells["Etkinlik"].Value.ToString()!;

                    
                    new FormRateEvent(eventId, title).ShowDialog();
                }
                catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
            };

            pnlBottom.Controls.Add(btnRate);

            
            this.Controls.Add(dgvMyApps); 
            this.Controls.Add(pnlBottom); 
            this.Controls.Add(pnlHeader); 
        }

        private void LoadMyApps()
        {
            if (Session.CurrentUser != null)
            {
                
                dgvMyApps.DataSource = _appDal.GetApplicationsByUser(Session.CurrentUser.UserId);
                
                
                if (dgvMyApps.Columns.Contains("Id"))
                {
                    dgvMyApps.Columns["Id"].Visible = false; 
                }
            }
        }
    }
}