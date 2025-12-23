using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq; 
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    public class PageClubs : UserControl
    {
        private ClubDal _clubDal = new ClubDal();
        private CommonDal _commonDal = new CommonDal();
        
        private FlowLayoutPanel pnlContainer;
        private List<Category> _categories; 

        
        private int CardWidth = 300;
        private int CardHeight = 350;
        private int CardMargin = 20;

        public PageClubs()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorTranslator.FromHtml("#ECF0F1");
            
            LoadCategories();
            InitializeUI();
            LoadClubs();

            this.Resize += (s, e) => CenterCards();
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            CenterCards();
        }

        private void CenterCards()
        {
            if (pnlContainer == null || pnlContainer.Controls.Count == 0 || pnlContainer.Width == 0) return;
            int containerW = pnlContainer.ClientSize.Width;
            int itemW = CardWidth + (CardMargin * 2);
            int itemsPerRow = Math.Max(1, containerW / itemW);
            int totalContentW = itemsPerRow * itemW;
            int remainingSpace = containerW - totalContentW;
            int leftPad = Math.Max(20, remainingSpace / 2);
            if (pnlContainer.Padding.Left != leftPad) pnlContainer.Padding = new Padding(leftPad, 20, 0, 20);
        }

        private void LoadCategories()
        {
            try { _categories = _commonDal.GetAllCategories(); }
            catch { _categories = new List<Category>(); }
        }

        private void InitializeUI()
        {
            
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 0)
            };

            Label lblTitle = new Label
            {
                Text = "Öğrenci Kulüpleri",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                AutoSize = true,
                Location = new Point(30, 20)
            };

            
            
            string role = Session.CurrentUser?.Role?.Trim()?.ToUpper() ?? "";
            
            
            if (role == "ADMIN" || role == "CLUB_MANAGER")
            {
                Button btnAdd = new Button
                {
                    Text = "+ KULÜP EKLE",
                    BackColor = ColorTranslator.FromHtml("#27AE60"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Size = new Size(150, 40),
                    Cursor = Cursors.Hand,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new Point(pnlHeader.Width - 180, 20)
                };
                btnAdd.FlatAppearance.BorderSize = 0;

                
                btnAdd.Click += (s, e) => 
                {
                    FormAddClub frm = new FormAddClub();
                    
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        LoadClubs();
                    }
                };
                
                pnlHeader.Controls.Add(btnAdd);
            }

            pnlHeader.Controls.Add(lblTitle);

            
            pnlContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ColorTranslator.FromHtml("#ECF0F1"),
                Padding = new Padding(20)
            };

            this.Controls.Add(pnlContainer);
            this.Controls.Add(pnlHeader);
        }

        private void LoadClubs()
        {
            pnlContainer.Controls.Clear();
            try
            {
                var clubs = _clubDal.GetAllClubs();

                if (clubs.Count == 0)
                {
                    Label lblEmpty = new Label { Text = "Henüz kulüp bulunmuyor.", AutoSize = true, Font = new Font("Segoe UI", 14), ForeColor = Color.Gray, Margin = new Padding(50) };
                    pnlContainer.Controls.Add(lblEmpty);
                }
                else
                {
                    foreach (var club in clubs)
                    {
                        pnlContainer.Controls.Add(CreateClubCard(club));
                    }
                    
                    this.PerformLayout();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kulüpler yüklenemedi: " + ex.Message);
            }
        }

        private Panel CreateClubCard(Club club)
        {
            
            Panel card = new Panel
            {
                Width = CardWidth,
                Height = CardHeight,
                BackColor = Color.White,
                Margin = new Padding(CardMargin)
            };

            
            PictureBox pbLogo = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 180,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.WhiteSmoke
            };

            if (!string.IsNullOrEmpty(club.LogoUrl) && File.Exists(club.LogoUrl))
                try { pbLogo.Image = Image.FromFile(club.LogoUrl); } catch { }
            else
            {
                
                string initial = string.IsNullOrEmpty(club.ClubName) ? "?" : club.ClubName.Substring(0, 1);
                pbLogo.Controls.Add(new Label { Text = initial, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 50, FontStyle.Bold), ForeColor = Color.Gray });
            }

            
            Panel pnlInfo = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };

            string catName = "Genel";
            var cat = _categories.FirstOrDefault(c => c.CategoryId == club.CategoryId);
            if (cat != null) catName = cat.CategoryName;

            Label lblCategory = new Label
            {
                Text = catName.ToUpper(),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#1ABC9C"),
                Dock = DockStyle.Top,
                Height = 20
            };

            Label lblName = new Label
            {
                Text = club.ClubName,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                Dock = DockStyle.Top,
                Height = 50
            };

            
            Button btnDetail = new Button
            {
                Text = "Kulübü İncele",
                BackColor = ColorTranslator.FromHtml("#34495E"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Bottom,
                Height = 40,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnDetail.FlatAppearance.BorderSize = 0;

            
            btnDetail.Click += (s, e) => 
            {
                FormClubDetail frm = new FormClubDetail(club.ClubId);
                frm.ShowDialog();
            };

            pnlInfo.Controls.Add(lblName);
            pnlInfo.Controls.Add(lblCategory);
            pnlInfo.Controls.Add(btnDetail);

            card.Controls.Add(pnlInfo);
            card.Controls.Add(pbLogo);

            return card;
        }
    }
}