using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public partial class FormAddAnnouncement : Form
    {
        private TextBox txtTitle;
        private RichTextBox rtxtContent;
        private ComboBox cmbClubs;
        private AnnouncementDal _announcementDal = new AnnouncementDal();
        private ClubDal _clubDal = new ClubDal();

        public FormAddAnnouncement()
        {
            this.Text = "Yeni Duyuru Ekle";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            SetupUI();
            LoadClubs(); // Kulüpleri burada yüklüyoruz
        }

        private void SetupUI()
        {
            int y = 30; int x = 40; int w = 500;

            Label lblMain = new Label { Text = "Duyuru Bilgileri", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.DarkSlateBlue, Location = new Point(x, y), AutoSize = true };
            this.Controls.Add(lblMain); y += 50;

            // Konu
            Label l1 = new Label { Text = "Konu Başlığı:", Location = new Point(x, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            txtTitle = new TextBox { Location = new Point(x, y + 25), Width = w, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(l1); this.Controls.Add(txtTitle); y += 70;

            // Kulüp Seçimi (Kritik Kısım)
            Label l2 = new Label { Text = "Hangi Kulüp Adına:", Location = new Point(x, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            cmbClubs = new ComboBox { Location = new Point(x, y + 25), Width = w, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(l2); this.Controls.Add(cmbClubs); y += 70;

            // Mesaj
            Label l3 = new Label { Text = "Duyuru İçeriği:", Location = new Point(x, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            rtxtContent = new RichTextBox { Location = new Point(x, y + 25), Width = w, Height = 120, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(l3); this.Controls.Add(rtxtContent); y += 160;

            // Kaydet
            Button btnSave = new Button { 
                Text = "DUYURUYU YAYINLA", Location = new Point(x, y), Size = new Size(w, 50), 
                BackColor = ColorTranslator.FromHtml("#27AE60"), ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void LoadClubs()
        {
            try
            {
                var allClubs = _clubDal.GetAllClubs();
                
                // Güvenli rol kontrolü
                string userRole = Session.CurrentUser?.Role?.Trim()?.ToUpper() ?? "STUDENT";
                int currentUserId = Session.CurrentUser != null ? Session.CurrentUser.UserId : 0;

                if (userRole == "ADMIN")
                {
                    // Admin ise HEPSİNİ yükle
                    cmbClubs.DataSource = allClubs;
                    cmbClubs.DisplayMember = "ClubName";
                    cmbClubs.ValueMember = "ClubId";
                }
                else if (userRole == "CLUB_MANAGER")
                {
                    // Yönetici ise SADECE KENDİ kulübünü yükle
                    var myClubs = allClubs.Where(c => c.ManagerUserId == currentUserId).ToList();

                    if (myClubs.Count > 0)
                    {
                        cmbClubs.DataSource = myClubs;
                        cmbClubs.DisplayMember = "ClubName";
                        cmbClubs.ValueMember = "ClubId";
                        
                        // Tek kulüp varsa seçili gelsin ve kilitlensin
                        cmbClubs.SelectedIndex = 0; 
                        // cmbClubs.Enabled = false; // İstersen kilitleyebilirsin, birden fazla kulübü varsa açık kalsın
                    }
                    else
                    {
                        MessageBox.Show("Size atanmış bir kulüp bulunamadı!");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Yetkisiz işlem.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kulüpler yüklenirken hata: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(rtxtContent.Text) || cmbClubs.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz.");
                return;
            }

            try
            {
                int clubId = (int)cmbClubs.SelectedValue;
                _announcementDal.AddAnnouncement(txtTitle.Text, rtxtContent.Text, clubId);
                MessageBox.Show("Duyuru başarıyla yayınlandı!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }
    }
}