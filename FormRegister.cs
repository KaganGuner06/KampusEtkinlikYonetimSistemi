using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    public partial class FormRegister : Form
    {
        // Renkler
        private readonly Color clrSidebar = ColorTranslator.FromHtml("#2C3E50");
        private readonly Color clrBackground = ColorTranslator.FromHtml("#ECF0F1");
        private readonly Color clrAccent = ColorTranslator.FromHtml("#27AE60"); // Yeşil
        private readonly Color clrText = ColorTranslator.FromHtml("#34495E");

        private UserDal _userDal = new UserDal();
        private TextBox txtUsername, txtName, txtEmail, txtPassword; // txtUsername eklendi

        public FormRegister()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 560); // Boyut biraz artırıldı
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Kayıt Ol";
        }

        private void SetupUI()
        {
            // 1. SOL TARAF (Sidebar)
            Panel pnlLeft = new Panel { Dock = DockStyle.Left, Width = 260, BackColor = clrSidebar };
            Label lblBrand = new Label { Text = "ARAMIZA\nKATIL", ForeColor = Color.White, Font = new Font("Segoe UI", 20, FontStyle.Bold), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            pnlLeft.Controls.Add(lblBrand);
            this.Controls.Add(pnlLeft);

            // 2. SAĞ TARAF (Form)
            this.BackColor = clrBackground;

            Label lblClose = new Label { Text = "X", ForeColor = clrText, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(760, 10), Cursor = Cursors.Hand, AutoSize = true };
            lblClose.Click += (s, e) => Application.Exit();
            this.Controls.Add(lblClose);

            Label lblTitle = new Label { Text = "ÖĞRENCİ KAYDI", ForeColor = clrText, Font = new Font("Segoe UI", 18, FontStyle.Regular), Location = new Point(300, 30), AutoSize = true };
            this.Controls.Add(lblTitle);

            // Inputlar
            int startY = 90;

            // YENİ: Kullanıcı Adı Kutusu
            txtUsername = CreateModernTextBox("Kullanıcı Adı (Giriş için)", 300, startY);
            txtUsername.Width = 430;

            txtName = CreateModernTextBox("Adınız Soyadınız", 300, startY + 65);
            txtName.Width = 430;

            txtEmail = CreateModernTextBox("Üniversite Email (@edu.tr)", 300, startY + 130);
            txtEmail.Width = 430; 

            txtPassword = CreateModernTextBox("Şifre Belirle", 300, startY + 195);
            txtPassword.Width = 430;
            txtPassword.Tag = "isPassword";

            this.Controls.AddRange(new Control[] { txtUsername, txtName, txtEmail, txtPassword });

            // Kayıt Butonu
            Button btnRegister = new Button
            {
                Text = "KAYDI TAMAMLA",
                BackColor = clrAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(300, startY + 270),
                Size = new Size(430, 45),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;
            this.Controls.Add(btnRegister);

            // Giriş Yap Linki
            Label lblLogin = new Label { Text = "Zaten hesabın var mı? Giriş Yap", ForeColor = Color.Gray, Font = new Font("Segoe UI", 10, FontStyle.Underline), Location = new Point(420, startY + 330), AutoSize = true, Cursor = Cursors.Hand };
            lblLogin.Click += (s, e) => { new FormLogin().Show(); this.Hide(); };
            this.Controls.Add(lblLogin);

            pnlLeft.MouseDown += Form_MouseDown;
            this.MouseDown += Form_MouseDown;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // Boşluk kontrolüne txtUsername eklendi
            if (CheckEmpty(txtUsername) || CheckEmpty(txtName) || CheckEmpty(txtEmail) || CheckEmpty(txtPassword))
            {
                MessageBox.Show("Lütfen tüm alanları geçerli şekilde doldurunuz.");
                return;
            }

            try
            {
                User newUser = new User
                {
                    Username = txtUsername.Text, // Yeni Username atandı
                    FullName = txtName.Text,
                    Email = txtEmail.Text,
                    Role = "STUDENT" 
                };

                // Backend metodu çağrılıyor
                _userDal.RegisterUser(newUser, txtPassword.Text);
                
                MessageBox.Show("Kayıt başarılı! Artık kullanıcı adınızla giriş yapabilirsiniz.", "Tebrikler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                new FormLogin().Show();
                this.Hide(); // Kayıttan sonra gizle
            }
            catch (Exception ex)
            {
                // SQL Trigger'dan gelen "Sadece @edu.tr..." hatası tam burada yakalanır
                MessageBox.Show(ex.Message, "Kayıt Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TextBox CreateModernTextBox(string placeholder, int x, int y)
        {
            TextBox txt = new TextBox { Text = placeholder, ForeColor = Color.Gray, Font = new Font("Segoe UI", 11), Location = new Point(x, y), Size = new Size(430, 30), BorderStyle = BorderStyle.FixedSingle };
            txt.Enter += (s, e) => { if (txt.Text == placeholder) { txt.Text = ""; txt.ForeColor = clrText; if (txt.Tag?.ToString() == "isPassword") txt.PasswordChar = '●'; } };
            txt.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txt.Text)) { txt.Text = placeholder; txt.ForeColor = Color.Gray; if (txt.Tag?.ToString() == "isPassword") txt.PasswordChar = '\0'; } };
            return txt;
        }

        private bool CheckEmpty(TextBox txt) => 
            string.IsNullOrWhiteSpace(txt.Text) || 
            txt.Text.Contains("Kullanıcı") || 
            txt.Text.Contains("Adınız") || 
            txt.Text.Contains("Email") || 
            txt.Text.Contains("Şifre");

        [DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")] public static extern bool ReleaseCapture();
        private void Form_MouseDown(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, 0x2, 0); } }
    }
}