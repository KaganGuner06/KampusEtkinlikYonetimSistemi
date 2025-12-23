using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;
using System.Collections.Generic;

namespace CampusEventManager
{
    public class PageEventAdd : UserControl
    {
        private EventDal _eventDal = new EventDal();
        private CommonDal _commonDal = new CommonDal();

        
        private TextBox txtTitle, txtLocation, txtPosterUrl;
        private TextBox txtDescription;
        private NumericUpDown nudQuota;
        private DateTimePicker dtpDate;
        private ComboBox cmbCategory, cmbClub;
        
        
        private CheckBox chkIsPublished; 

        public PageEventAdd()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.AutoScroll = true; 
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };
            Button btnBack = new Button { Text = "← İptal", FlatStyle = FlatStyle.Flat, Location = new Point(20, 15), Size = new Size(100, 30), Cursor = Cursors.Hand };
            btnBack.Click += (s, e) => GoBack();
            Label lblTitle = new Label { Text = "Yeni Etkinlik Ekle", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(140, 15), AutoSize = true };
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnBack);

            
            Panel pnlForm = new Panel { Location = new Point(50, 80), Size = new Size(600, 800), AutoSize = true };
            int y = 0;
            
            txtTitle = AddField(pnlForm, "Etkinlik Başlığı:", ref y);
            
            Label lblDesc = new Label { Text = "Açıklama:", Location = new Point(0, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(0, y + 25), Width = 500, Height = 100, Multiline = true, ScrollBars = ScrollBars.Vertical, BorderStyle = BorderStyle.FixedSingle };
            pnlForm.Controls.Add(lblDesc);
            pnlForm.Controls.Add(txtDescription);
            y += 140;

            Label lblDate = new Label { Text = "Tarih:", Location = new Point(0, y), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            dtpDate = new DateTimePicker { Location = new Point(0, y + 25), Width = 200, Format = DateTimePickerFormat.Short };
            pnlForm.Controls.Add(lblDate);
            pnlForm.Controls.Add(dtpDate);

            Label lblLoc = new Label { Text = "Konum:", Location = new Point(250, y), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtLocation = new TextBox { Location = new Point(250, y + 25), Width = 250, BorderStyle = BorderStyle.FixedSingle, Height = 25 };
            pnlForm.Controls.Add(lblLoc);
            pnlForm.Controls.Add(txtLocation);
            y += 70;

            Label lblCat = new Label { Text = "Kategori:", Location = new Point(0, y), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cmbCategory = new ComboBox { Location = new Point(0, y + 25), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(lblCat);
            pnlForm.Controls.Add(cmbCategory);

            Label lblQuota = new Label { Text = "Kontenjan:", Location = new Point(250, y), Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            nudQuota = new NumericUpDown { Location = new Point(250, y + 25), Width = 100, Maximum = 5000, Value = 50 };
            pnlForm.Controls.Add(lblQuota);
            pnlForm.Controls.Add(nudQuota);
            y += 70;

            Label lblClub = new Label { Text = "Kulüp:", Location = new Point(0, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            cmbClub = new ComboBox { Location = new Point(0, y + 25), Width = 500, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(lblClub);
            pnlForm.Controls.Add(cmbClub);
            y += 70;

            Label lblPoster = new Label { Text = "Etkinlik Posteri:", Location = new Point(0, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            txtPosterUrl = new TextBox { Location = new Point(0, y + 25), Width = 380, Height = 30, BorderStyle = BorderStyle.FixedSingle, ReadOnly = true }; 
            Button btnChooseImg = new Button { Text = "Resim Seç...", Location = new Point(390, y + 23), Width = 110, Height = 30, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnChooseImg.Click += (s, e) => {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK) txtPosterUrl.Text = ofd.FileName;
            };

            pnlForm.Controls.Add(lblPoster);
            pnlForm.Controls.Add(txtPosterUrl);
            pnlForm.Controls.Add(btnChooseImg);
            y += 70;

            
            chkIsPublished = new CheckBox 
            { 
                Text = "Etkinlik yayında olsun.", 
                Location = new Point(0, y), 
                Checked = true, 
                Font = new Font("Segoe UI", 10),
                AutoSize = true
            };
            pnlForm.Controls.Add(chkIsPublished);
            y += 40; 

            
            Button btnSave = new Button 
            { 
                Text = "KAYDET", 
                Location = new Point(0, y), 
                Size = new Size(200, 50), 
                BackColor = ColorTranslator.FromHtml("#27AE60"), 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;
            pnlForm.Controls.Add(btnSave);

            this.Controls.Add(pnlForm);
            this.Controls.Add(pnlHeader);
        }

        private TextBox AddField(Panel p, string label, ref int y)
        {
            Label lbl = new Label { Text = label, Location = new Point(0, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            TextBox txt = new TextBox { Location = new Point(0, y + 25), Width = 500, Height = 30, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10) };
            p.Controls.Add(lbl);
            p.Controls.Add(txt);
            y += 70;
            return txt;
        }

        private void LoadData()
        {
            try 
            {
                cmbCategory.DataSource = null; 
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryId";
                cmbCategory.DataSource = _commonDal.GetAllCategories();

                var clubs = _commonDal.GetAllClubs(); 
                if (clubs.Count == 0) MessageBox.Show("Sistemde kayıtlı kulüp bulunamadı!");

                cmbClub.DataSource = null; 
                cmbClub.DisplayMember = "ClubName"; 
                cmbClub.ValueMember = "ClubId";
                cmbClub.DataSource = clubs;
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbClub.SelectedIndex == -1 || cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen başlık, kategori ve kulüp alanlarını doldurun.");
                return;
            }

            try
            {
                Event newEvent = new Event
                {
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    EventDate = dtpDate.Value,
                    Location = txtLocation.Text,
                    Quota = (int)nudQuota.Value,
                    CategoryId = (int)cmbCategory.SelectedValue,
                    ClubId = (int)cmbClub.SelectedValue,
                    PosterUrl = string.IsNullOrWhiteSpace(txtPosterUrl.Text) ? "default_event.jpg" : txtPosterUrl.Text,
                    
                    
                    IsPublished = chkIsPublished.Checked 
                };

                _eventDal.AddEvent(newEvent);
                MessageBox.Show("Etkinlik başarıyla kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GoBack();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void GoBack()
        {
            FormMain main = (FormMain)this.FindForm();
            if (main != null) {
                main.pnlContent.Controls.Clear();
                main.pnlContent.Controls.Add(new PageEvents());
            }
        }
    }
}