using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;
using System.Linq;

namespace CampusEventManager
{
    public class PageHome : UserControl
    {
        private CommonDal _commonDal;
        private EventDal _eventDal;
        private FlowLayoutPanel pnlShowcase;

        public PageHome()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.AutoScroll = true; 

            _commonDal = new CommonDal();
            _eventDal = new EventDal();

            InitializeUI();
        }

        private void InitializeUI()
        {
            
            
            string userName = Session.CurrentUser?.FullName ?? "Öğrenci";
            
            Label lblWelcome = new Label 
            { 
                Text = $"👋 Merhaba, {userName}!", 
                Font = new Font("Segoe UI", 20, FontStyle.Bold), 
                ForeColor = Color.FromArgb(64, 64, 64),
                Location = new Point(30, 20), 
                AutoSize = true 
            };
            this.Controls.Add(lblWelcome);

            Label lblSub = new Label 
            { 
                Text = "Kampüste neler oluyor, hadi göz atalım.", 
                Font = new Font("Segoe UI", 11), 
                ForeColor = Color.Gray,
                Location = new Point(35, 60), 
                AutoSize = true 
            };
            this.Controls.Add(lblSub);

            
            
            var stats = _commonDal.GetDashboardStats();

            int startY = 100;
            
            
            CreateStatCard("Toplam Kulüp", stats.TotalClubs.ToString(), Color.Orange, new Point(30, startY));
            
            
            CreateStatCard("Yaklaşan Etkinlik", stats.TotalEvents.ToString(), Color.SteelBlue, new Point(260, startY));
            
            
            string role = Session.CurrentUser?.Role?.Trim()?.ToUpper();
            if (role == "ADMIN" || role == "CLUB_MANAGER")
            {
                
                CreateStatCard("Bekleyen Başvuru", stats.TotalApplications.ToString(), Color.IndianRed, new Point(490, startY));
            }
            else
            {
                
                CreateStatCard("Bugünün Tarihi", DateTime.Now.Day.ToString(), Color.MediumSeaGreen, new Point(490, startY));
            }

            
            Label lblShowcase = new Label 
            { 
                Text = "🔥 Yaklaşan Etkinlikler", 
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                Location = new Point(30, 240), 
                AutoSize = true 
            };
            this.Controls.Add(lblShowcase);

            
            pnlShowcase = new FlowLayoutPanel 
            { 
                Location = new Point(30, 280), 
                Size = new Size(950, 350), 
                AutoScroll = true,
                WrapContents = false, 
                FlowDirection = FlowDirection.LeftToRight
            };
            this.Controls.Add(pnlShowcase);

            LoadShowcaseEvents();
        }

        
        private void CreateStatCard(string title, string value, Color color, Point loc)
        {
            Panel pnl = new Panel { Location = loc, Size = new Size(200, 100), BackColor = color };
            
            Label lblValue = new Label { Text = value, Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.White, Location = new Point(10, 15), AutoSize = true };
            Label lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 10), ForeColor = Color.WhiteSmoke, Location = new Point(15, 65), AutoSize = true };
            
            pnl.Controls.Add(lblValue);
            pnl.Controls.Add(lblTitle);
            this.Controls.Add(pnl);
        }

        
        private void LoadShowcaseEvents()
        {
            try
            {
                
                var allEvents = _eventDal.GetAllEvents();
                
                
                var upcomingEvents = allEvents
                                     .Where(e => e.EventDate >= DateTime.Now)
                                     .OrderBy(e => e.EventDate)
                                     .Take(5)
                                     .ToList();

                if (upcomingEvents.Count == 0)
                {
                    Label lblEmpty = new Label { Text = "Şu an yaklaşan etkinlik yok.", AutoSize = true, Font = new Font("Segoe UI", 12), Margin = new Padding(10) };
                    pnlShowcase.Controls.Add(lblEmpty);
                }
                else
                {
                    foreach (var evt in upcomingEvents)
                    {
                        pnlShowcase.Controls.Add(CreateMiniCard(evt));
                    }
                }
            }
            catch { }
        }

        
        private Panel CreateMiniCard(Event evt)
        {
            Panel card = new Panel { Size = new Size(220, 300), BackColor = Color.WhiteSmoke, Margin = new Padding(0, 0, 20, 0), BorderStyle = BorderStyle.FixedSingle };
            
            
            PictureBox pb = new PictureBox { Dock = DockStyle.Top, Height = 120, SizeMode = PictureBoxSizeMode.StretchImage, BackColor = Color.LightGray };
            try { if (!string.IsNullOrEmpty(evt.PosterUrl)) pb.Image = Image.FromFile(evt.PosterUrl); } catch { }

            
            Label lblTitle = new Label { Text = evt.Title, Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(10, 130), Size = new Size(200, 40) };
            
            
            Label lblDate = new Label { Text = evt.EventDate.ToString("dd MMM HH:mm"), Location = new Point(10, 175), ForeColor = Color.Gray, AutoSize = true };

            
            Button btn = new Button { Text = "İncele", Location = new Point(10, 250), Size = new Size(200, 35), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            
            
            btn.Click += (s, e) => {
                FormMain main = (FormMain)this.FindForm();
                if (main != null) { main.pnlContent.Controls.Clear(); main.pnlContent.Controls.Add(new PageEventDetail(evt)); }
            };

            card.Controls.AddRange(new Control[] { pb, lblTitle, lblDate, btn });
            return card;
        }
    }
}