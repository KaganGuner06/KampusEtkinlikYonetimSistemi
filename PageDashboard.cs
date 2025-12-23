using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;

namespace CampusEventManager
{
    
    public class PageDashboard : UserControl
    {
        
        private CommonDal _commonDal = new CommonDal();
        private AnnouncementDal _announcementDal = new AnnouncementDal();

        public PageDashboard()
        {
            this.Dock = DockStyle.Fill; 
            this.BackColor = ColorTranslator.FromHtml("#ECF0F1"); 
            this.Padding = new Padding(20);

            InitializeUI();
        }

        private void InitializeUI()
        {
            
            FlowLayoutPanel pnlStats = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 150,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false
            };

            
            try 
            {
                var stats = _commonDal.GetDashboardStats();
                
                
                pnlStats.Controls.Add(CreateStatCard("Toplam Etkinlik", stats.TotalEvents.ToString(), "#3498DB")); 
                pnlStats.Controls.Add(CreateStatCard("Aktif Kulüpler", stats.TotalClubs.ToString(), "#E67E22"));  
                pnlStats.Controls.Add(CreateStatCard("Toplam Başvuru", stats.TotalApplications.ToString(), "#9B59B6")); 
            }
            catch
            {
                
                pnlStats.Controls.Add(CreateStatCard("Toplam Etkinlik", "-", "#3498DB"));
                pnlStats.Controls.Add(CreateStatCard("Aktif Kulüpler", "-", "#E67E22"));
                pnlStats.Controls.Add(CreateStatCard("Toplam Başvuru", "-", "#9B59B6"));
            }

            
            Label lblAnnounce = new Label
            {
                Text = "Son Duyurular",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#2C3E50"),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.BottomLeft
            };
            
            Panel pnlSpacer = new Panel { Dock = DockStyle.Top, Height = 20 }; 

            
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false, 
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                GridColor = Color.WhiteSmoke
            };

            
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#2C3E50");
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 40;
            
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.ForeColor = Color.DimGray;
            grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#1ABC9C"); 
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.RowTemplate.Height = 35;

            
            try
            {
                grid.DataSource = _announcementDal.GetAllAnnouncements();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Duyurular yüklenemedi: " + ex.Message);
            }

            
            
            
            
            this.Controls.Add(grid);          
            this.Controls.Add(lblAnnounce);   
            this.Controls.Add(pnlSpacer);     
            this.Controls.Add(pnlStats);      
        }

        
        private Panel CreateStatCard(string title, string value, string colorHex)
        {
            Panel card = new Panel
            {
                Width = 250,
                Height = 120,
                BackColor = ColorTranslator.FromHtml(colorHex),
                Margin = new Padding(0, 0, 20, 0) 
            };

            Label lblVal = new Label
            {
                Text = value,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTitle = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            card.Controls.Add(lblVal);
            card.Controls.Add(lblTitle);
            return card;
        }
    }
}