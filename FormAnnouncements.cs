using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public partial class FormAnnouncements : Form
    {
        private DataGridView dgvAnnounce;
        private AnnouncementDal _annDal;
        
        
        private TextBox txtTitle, txtContent;
        private Button btnPost;

        public FormAnnouncements()
        {
            this.Text = "Duyuru Panosu";
            this.Size = new Size(750, 650); 
            this.StartPosition = FormStartPosition.CenterParent;
            _annDal = new AnnouncementDal();

            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            
            dgvAnnounce = new DataGridView { 
                Location = new Point(20, 20), 
                Size = new Size(690, 380), 
                ReadOnly = true, 
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, 
                
                
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, 
                BackgroundColor = Color.White
            };
            
            
            dgvAnnounce.DefaultCellStyle.WrapMode = DataGridViewTriState.True; 
            
            this.Controls.Add(dgvAnnounce);

            
            
            if (Session.CurrentUser?.Role == "CLUB_MANAGER" || Session.CurrentUser?.Role == "ADMIN")
            {
                GroupBox grpAdd = new GroupBox { Text = "Yeni Duyuru Yayınla", Location = new Point(20, 420), Size = new Size(690, 160) };
                
                
                Label l1 = new Label { Text = "Konu Başlığı:", Location = new Point(20, 30), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
                txtTitle = new TextBox { Location = new Point(120, 27), Width = 550 };
                
                
                Label l2 = new Label { Text = "Mesaj İçeriği:", Location = new Point(20, 70), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
                txtContent = new TextBox { Location = new Point(120, 67), Width = 430, Height = 70, Multiline = true, ScrollBars = ScrollBars.Vertical };

                
                btnPost = new Button { Text = "YAYINLA", Location = new Point(560, 67), Width = 110, Height = 70, BackColor = Color.Teal, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                btnPost.Click += BtnPost_Click;

                grpAdd.Controls.Add(l1); grpAdd.Controls.Add(txtTitle);
                grpAdd.Controls.Add(l2); grpAdd.Controls.Add(txtContent);
                grpAdd.Controls.Add(btnPost);
                this.Controls.Add(grpAdd);
            }
        }

        private void BtnPost_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtContent.Text))
            {
                MessageBox.Show("Lütfen konu ve mesaj alanlarını doldurun.");
                return;
            }

            try {
                
                _annDal.AddAnnouncement(txtTitle.Text, txtContent.Text, 1);
                MessageBox.Show("Duyuru Başarıyla Yayınlandı! 📢");
                LoadData();
                txtTitle.Clear(); txtContent.Clear();
            } catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void LoadData()
        {
            dgvAnnounce.DataSource = _annDal.GetAllAnnouncements();
        }
    }
}