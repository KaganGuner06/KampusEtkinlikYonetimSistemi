using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    public partial class FormStudentClubs : Form
    {
        private DataGridView dgvClubs;
        private Button btnJoin;
        private CommonDal _commonDal;
        private ClubDal _clubDal;

        public FormStudentClubs()
        {
            this.Text = "Kulüp Listesi ve Üyelik";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            
            _commonDal = new CommonDal();
            _clubDal = new ClubDal();

            SetupUI();
            LoadClubs();
        }

        private void SetupUI()
        {
            Label lblTitle = new Label { Text = "Aktif Kulüpler", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            
            dgvClubs = new DataGridView();
            dgvClubs.Location = new Point(20, 60);
            dgvClubs.Size = new Size(540, 280);
            dgvClubs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClubs.ReadOnly = true;
            dgvClubs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClubs.MultiSelect = false;

            btnJoin = new Button { Text = "SEÇİLİ KULÜBE KATIL", Location = new Point(20, 350), Width = 540, Height = 40, BackColor = Color.MediumSeaGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnJoin.Click += BtnJoin_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvClubs);
            this.Controls.Add(btnJoin);
        }

        private void LoadClubs()
        {
            
            dgvClubs.DataSource = _commonDal.GetAllClubs(); 
        }

        private void BtnJoin_Click(object? sender, EventArgs e)
        {
            
            if (dgvClubs.SelectedRows.Count == 0) 
            { 
                MessageBox.Show("Lütfen listeden katılmak istediğiniz kulübü seçin."); 
                return; 
            }
            
            int clubId = Convert.ToInt32(dgvClubs.SelectedRows[0].Cells["ClubId"].Value);
            int userId = Session.CurrentUser!.UserId;

            try
            {
                
                _clubDal.JoinClub(userId, clubId);
                
                
                MessageBox.Show("Tebrikler! Kulübe başarıyla üye oldunuz. 🎉", "Hoşgeldin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                
                
                
                if (ex.Message.Contains("23505") || ex.Message.Contains("uq_user_club"))
                {
                    MessageBox.Show("⚠️ Bu kulübe ZATEN üyesiniz.\nTekrar üye olmanıza gerek yok.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    
                    MessageBox.Show("İşlem Başarısız: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}