using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public partial class FormMain : Form
    {
        private System.ComponentModel.IContainer components = null;

        
        private readonly Color clrSidebar = ColorTranslator.FromHtml("#2C3E50");
        private readonly Color clrHeader = Color.White;
        private readonly Color clrBackground = ColorTranslator.FromHtml("#ECF0F1");
        private readonly Color clrAccent = ColorTranslator.FromHtml("#1ABC9C");
        private readonly Color clrText = ColorTranslator.FromHtml("#34495E");

        
        private Panel pnlSidebar;
        private Panel pnlHeader;
        public Panel pnlContent; 
        private Label lblHeaderTitle;
        private Label lblUserInfo;

        public FormMain()
        {
            InitializeComponent();
            SetupLayout();
            
            if (Session.CurrentUser != null)
                lblUserInfo.Text = $"{Session.CurrentUser.FullName} ({Session.CurrentUser.Role})";
            else
                lblUserInfo.Text = "Misafir Kullanıcı";

            
            LoadPage("Dashboard");
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Kampüs Etkinlik Sistemi";
        }

        private void SetupLayout()
        {
            
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = clrSidebar };
            
            Label lblLogo = new Label {
                Text = "Yönetim", Dock = DockStyle.Top, Height = 80,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter
            };
            pnlSidebar.Controls.Add(lblLogo);

            
            
            
            AddMenuButton("Ana Sayfa", 90, (s, e) => LoadPage("Dashboard"));
            AddMenuButton("Etkinlikler", 145, (s, e) => LoadPage("Events"));
            AddMenuButton("Kulüpler", 200, (s, e) => LoadPage("Clubs"));
            AddMenuButton("Duyurular", 255, (s, e) => LoadPage("Announcements"));
            AddMenuButton("Profilim", 365, (s, e) => LoadPage("Profile"));
            
            string userRole = Session.CurrentUser?.Role?.Trim()?.ToUpper() ?? "";

            if (userRole == "STUDENT")
            {
                AddMenuButton("Başvurularım", 310, (s, e) => LoadPage("MyApps"));
            }

            if (userRole == "ADMIN" || userRole == "CLUB_MANAGER")
            {
                AddMenuButton("Başvuru Ynt.", 310, (s, e) => 
                {
                    FormApplications appForm = new FormApplications();
                    appForm.ShowDialog();
                });
            }

            
            Button btnExit = new Button {
                Text = "  Çıkış Yap", Dock = DockStyle.Bottom, Height = 50,
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White, Font = new Font("Segoe UI", 10), Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => {
                Session.CurrentUser = null;
                new FormLogin().Show();
                this.Close();
            };
            pnlSidebar.Controls.Add(btnExit);

            
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = clrHeader };
            
            lblHeaderTitle = new Label {
                Text = "Ana Sayfa", Font = new Font("Segoe UI", 14), ForeColor = clrText,
                Location = new Point(20, 15), AutoSize = true
            };
            
            lblUserInfo = new Label {
                Text = "...", Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = Color.Gray,
                Location = new Point(this.Width - 300, 20), AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            Label lblClose = new Label { Text = "X", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(this.Width - 40, 15), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblClose.Click += (s, e) => Application.Exit();

            Label lblMin = new Label { Text = "_", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(this.Width - 70, 10), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            lblMin.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            pnlHeader.Controls.Add(lblHeaderTitle);
            pnlHeader.Controls.Add(lblUserInfo);
            pnlHeader.Controls.Add(lblClose);
            pnlHeader.Controls.Add(lblMin);
            pnlHeader.MouseDown += Form_MouseDown;

            
            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = clrBackground, Padding = new Padding(20) };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlSidebar);
        }

        private void AddMenuButton(string text, int top, EventHandler onClick)
        {
            Button btn = new Button {
                Text = "  " + text, Top = top, Left = 0, Width = 240, Height = 50,
                FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = clrSidebar,
                Font = new Font("Segoe UI", 11), Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(15, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            btn.MouseEnter += (s, e) => btn.BackColor = clrAccent;
            btn.MouseLeave += (s, e) => btn.BackColor = clrSidebar;
            pnlSidebar.Controls.Add(btn);
        }

        
        private void LoadPage(string page)
        {
            pnlContent.Controls.Clear();
            lblHeaderTitle.Text = page switch
            {
                "Dashboard" => "Ana Sayfa",
                "Events" => "Etkinlikler",
                "Clubs" => "Kulüpler",
                "Profile" => "Profil Ayarlarım",
                "Announcements" => "Duyurular",
                "MyApps" => "Başvurularım",
                _ => page
            };

            switch (page)
            {
                case "Dashboard":
                    
                    
                    pnlContent.Controls.Add(new PageHome());
                    break;
                case "Events":
                    pnlContent.Controls.Add(new PageEvents());
                    break;
                case "Profile":
                    pnlContent.Controls.Add(new PageProfile());
                    break;
                case "Clubs":
                    pnlContent.Controls.Add(new PageClubs());
                    break;
                case "Announcements":
                    pnlContent.Controls.Add(new PageAnnouncements());
                    break;
                case "MyApps":
                    pnlContent.Controls.Add(new PageStudentApps());
                    break;
            }
        }

        [DllImport("user32.dll")] public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")] public static extern bool ReleaseCapture();
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, 0x2, 0); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }
    }
}