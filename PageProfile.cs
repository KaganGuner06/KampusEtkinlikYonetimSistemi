using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    public class PageProfile : UserControl
    {
        private UserDal _userDal = new UserDal();
        private TextBox txtName, txtEmail, txtUsername, txtNewPass;

        public PageProfile()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            InitializeUI();
        }

        private void InitializeUI()
        {
            var user = Session.CurrentUser;

            Label lblTitle = new Label { Text = "👤 Profil Bilgilerim", Font = new Font("Segoe UI", 18, FontStyle.Bold), Location = new Point(30, 20), AutoSize = true };
            
            // Bilgi Giriş Alanları
            int startY = 80;
            txtName = CreateField("Ad Soyad:", user?.FullName, startY);
            txtUsername = CreateField("Kullanıcı Adı:", user?.Username, startY + 70);
            txtEmail = CreateField("E-posta:", user?.Email, startY + 140);
            txtNewPass = CreateField("Yeni Şifre (Değiştirmek istemiyorsanız boş bırakın):", "", startY + 210);
            txtNewPass.PasswordChar = '●';

            Button btnSave = new Button {
                Text = "DEĞİŞİKLİKLERİ KAYDET",
                Size = new Size(250, 45),
                Location = new Point(30, startY + 300),
                BackColor = ColorTranslator.FromHtml("#1ABC9C"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;

            this.Controls.AddRange(new Control[] { lblTitle, btnSave });
        }

        private TextBox CreateField(string labelText, string value, int y)
        {
            Label lbl = new Label { Text = labelText, Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI", 10) };
            TextBox txt = new TextBox { Text = value, Location = new Point(30, y + 25), Size = new Size(400, 30), Font = new Font("Segoe UI", 11) };
            this.Controls.Add(lbl);
            this.Controls.Add(txt);
            return txt;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try {
                var user = Session.CurrentUser;
                user.FullName = txtName.Text;
                user.Username = txtUsername.Text;
                user.Email = txtEmail.Text;

                _userDal.UpdateUserProfile(user);

                if (!string.IsNullOrWhiteSpace(txtNewPass.Text)) {
                    _userDal.UpdatePassword(user.UserId, txtNewPass.Text);
                }

                MessageBox.Show("Profiliniz başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}