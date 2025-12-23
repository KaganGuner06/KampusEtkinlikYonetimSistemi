using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    public partial class FormLogin : Form
    {
        private System.ComponentModel.IContainer components = null;

        
        private readonly Color clrSidebar = ColorTranslator.FromHtml("#2C3E50");
        private readonly Color clrBackground = ColorTranslator.FromHtml("#ECF0F1");
        private readonly Color clrAccent = ColorTranslator.FromHtml("#1ABC9C");
        private readonly Color clrText = ColorTranslator.FromHtml("#34495E");

        
        private UserDal _userDal = new UserDal(); 
        
        
        private Panel pnlLeft;
        private TextBox txtUsername; 
        private TextBox txtPassword;
        private Label lblFeedback;

        public FormLogin()
        {
            InitializeComponent();
            SetupCustomUI();
            AddDevButtons(); 
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 450);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Giriş Yap";
            this.DoubleBuffered = true;
        }

        private void SetupCustomUI()
        {
            
            pnlLeft = new Panel { Dock = DockStyle.Left, Width = 250, BackColor = clrSidebar };
            Label lblBrand = new Label {
                Text = "GİRİŞ PANELİ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            pnlLeft.Controls.Add(lblBrand);

            
            this.BackColor = clrBackground;
            
            
            Label lblClose = new Label {
                Text = "X",
                ForeColor = clrText,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(710, 10),
                Cursor = Cursors.Hand,
                AutoSize = true
            };
            lblClose.Click += (s, e) => Application.Exit();

            
            Label lblTitle = new Label {
                Text = "SİSTEME GİRİŞ",
                ForeColor = clrText,
                Font = new Font("Segoe UI", 20, FontStyle.Regular),
                Location = new Point(300, 50),
                AutoSize = true
            };

            
            txtUsername = CreateModernTextBox("Kullanıcı Adı", 300, 120);
            
            
            txtPassword = CreateModernTextBox("Şifre", 300, 180);
            txtPassword.Tag = "isPassword"; 

            
            lblFeedback = new Label {
                Text = "",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(300, 230),
                AutoSize = true
            };

            
            Button btnLogin = new Button {
                Text = "GİRİŞ YAP",
                BackColor = clrAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(300, 260),
                Size = new Size(400, 45),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            
            Label lblReg = new Label { 
                Text = "Hesabın yok mu? Kayıt Ol", 
                ForeColor = Color.Blue, 
                Font = new Font("Segoe UI", 10, FontStyle.Underline), 
                Location = new Point(430, 380), 
                AutoSize = true, 
                Cursor = Cursors.Hand 
            };
            lblReg.Click += (s, e) => {
                FormRegister frm = new FormRegister();
                frm.Show();
                this.Hide();
            };

            
            this.Controls.Add(lblReg);
            this.Controls.Add(lblFeedback);
            this.Controls.Add(btnLogin);
            this.Controls.Add(txtUsername);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblClose);
            this.Controls.Add(pnlLeft);

            
            pnlLeft.MouseDown += Form_MouseDown;
            this.MouseDown += Form_MouseDown;
        }

        private TextBox CreateModernTextBox(string placeholder, int x, int y)
        {
            TextBox txt = new TextBox {
                Text = placeholder,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 11),
                Location = new Point(x, y),
                Size = new Size(400, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            txt.Enter += (s, e) => {
                if (txt.Text == placeholder) {
                    txt.Text = "";
                    txt.ForeColor = clrText;
                    if (txt.Tag?.ToString() == "isPassword") txt.PasswordChar = '●';
                }
            };

            txt.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txt.Text)) {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                    if (txt.Tag?.ToString() == "isPassword") txt.PasswordChar = '\0';
                }
            };
            return txt;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text; 
            string pass = txtPassword.Text;

            if (username == "Kullanıcı Adı" || string.IsNullOrWhiteSpace(username)) {
                lblFeedback.Text = "Lütfen kullanıcı adınızı giriniz.";
                return;
            }

            try {
                
                User user = _userDal.Login(username, pass);

                if (user != null) {
                    Session.CurrentUser = user; 
                    FormMain main = new FormMain();
                    this.Hide();
                    main.Show();
                }
                else {
                    lblFeedback.Text = "Hatalı kullanıcı adı veya şifre.";
                }
            }
            catch (Exception ex) {
                lblFeedback.Text = "Sistem hatası: " + ex.Message;
            }
        }

        [DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")] public static extern bool ReleaseCapture();
        private void Form_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, 0x2, 0); }
        }
        
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void AddDevButtons()
        {
            
            Button btnDevAdmin = new Button { 
                Text = "Hızlı Admin", 
                Location = new Point(300, 320), 
                Size = new Size(190, 35),
                BackColor = Color.OrangeRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            
            btnDevAdmin.Click += (s, e) => {
                txtUsername.ForeColor = clrText;
                txtPassword.ForeColor = clrText;
                txtPassword.PasswordChar = '●';
                txtUsername.Text = "admin"; 
                txtPassword.Text = "12345"; 
                BtnLogin_Click(null, null); 
            };

            
            Button btnDevStudent = new Button { 
                Text = "Hızlı Öğrenci", 
                Location = new Point(510, 320), 
                Size = new Size(190, 35),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            
            btnDevStudent.Click += (s, e) => {
                txtUsername.ForeColor = clrText;
                txtPassword.ForeColor = clrText;
                txtPassword.PasswordChar = '●';
                txtUsername.Text = "kavakli"; 
                txtPassword.Text = "12345"; 
                BtnLogin_Click(null, null);
            };

            this.Controls.Add(btnDevAdmin);
            this.Controls.Add(btnDevStudent);
        }
    }
}