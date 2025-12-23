using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.Entities;
using CampusEventManager.DataAccess;
using System.Collections.Generic;

namespace CampusEventManager
{
    public class PageEventEdit : UserControl
    {
        private Event _eventToEdit; // Eğer null ise YENİ KAYIT, dolu ise DÜZENLEME
        private EventDal _eventDal = new EventDal();
        private CommonDal _commonDal = new CommonDal();
        private ClubDal _clubDal = new ClubDal(); 
        private bool isEditMode = false;

        // Form Elemanları
        private TextBox txtTitle, txtLocation, txtPosterUrl;
        private TextBox txtDescription;
        private NumericUpDown nudQuota;
        private DateTimePicker dtpDate;
        private ComboBox cmbCategory, cmbClub;
        private CheckBox chkPublish;
        private Button btnAction; // Kaydet veya Güncelle butonu

        // Constructor: null gelirse yeni kayıt, nesne gelirse düzenleme
        public PageEventEdit(Event evt)
        {
            _eventToEdit = evt;
            isEditMode = (evt != null);

            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;
            this.AutoScroll = true; 
            
            InitializeUI();
            
            // Verileri doldurmadan önce listeleri (Kategori/Kulüp) yükle
            LoadLists();

            if (isEditMode) 
            {
                FillFields(); // Düzenleme ise kutuları doldur
                btnAction.Text = "DEĞİŞİKLİKLERİ KAYDET";
                btnAction.BackColor = ColorTranslator.FromHtml("#E67E22"); // Turuncu
            }
            else
            {
                // Yeni kayıt ise varsayılan değerler
                dtpDate.Value = DateTime.Now.AddDays(1); 
                btnAction.Text = "YENİ ETKİNLİK OLUŞTUR";
                btnAction.BackColor = ColorTranslator.FromHtml("#27AE60"); // Yeşil
            }
        }

        private void InitializeUI()
        {
            // 1. HEADER
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };
            
            Button btnBack = new Button { Text = "← İptal", FlatStyle = FlatStyle.Flat, Location = new Point(20, 15), Size = new Size(100, 30), Cursor = Cursors.Hand };
            btnBack.Click += (s, e) => GoBack();
            
            string titleText = isEditMode ? "Etkinliği Düzenle" : "Yeni Etkinlik Oluştur";
            Label lblTitle = new Label { Text = titleText, Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(140, 15), AutoSize = true };
            
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnBack);

            // 2. FORM ALANI
            Panel pnlForm = new Panel { Location = new Point(50, 80), Size = new Size(600, 850), AutoSize = true };
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
            nudQuota = new NumericUpDown { Location = new Point(250, y + 25), Width = 100, Maximum = 5000 };
            pnlForm.Controls.Add(lblQuota);
            pnlForm.Controls.Add(nudQuota);
            y += 70;

            Label lblClub = new Label { Text = "Kulüp:", Location = new Point(0, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
            cmbClub = new ComboBox { Location = new Point(0, y + 25), Width = 500, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlForm.Controls.Add(lblClub);
            pnlForm.Controls.Add(cmbClub);
            y += 70;

            // Resim Alanı
            Label lblPoster = new Label { Text = "Etkinlik Posteri (Dosya Yolu):", Location = new Point(0, y), Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };
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

            // Yayın Durumu
            chkPublish = new CheckBox { Text = "Etkinlik Yayında Olsun", Location = new Point(0, y), Font = new Font("Segoe UI", 10), AutoSize = true, Checked = true };
            pnlForm.Controls.Add(chkPublish);
            y += 50;

            // KAYDET / GÜNCELLE BUTONU
            btnAction = new Button 
            { 
                Text = "İŞLEMİ TAMAMLA", 
                Location = new Point(0, y), 
                Size = new Size(250, 50), 
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAction.Click += BtnAction_Click;
            pnlForm.Controls.Add(btnAction);

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

        private void LoadLists()
        {
            try {
                cmbCategory.DataSource = _commonDal.GetAllCategories();
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryId";

                cmbClub.DataSource = _clubDal.GetAllClubs();
                cmbClub.DisplayMember = "ClubName";
                cmbClub.ValueMember = "ClubId";
            }
            catch { }
        }

        private void FillFields()
        {
            try {
                txtTitle.Text = _eventToEdit.Title;
                txtDescription.Text = _eventToEdit.Description;
                txtLocation.Text = _eventToEdit.Location;
                txtPosterUrl.Text = _eventToEdit.PosterUrl;
                dtpDate.Value = _eventToEdit.EventDate;
                nudQuota.Value = _eventToEdit.Quota;
                chkPublish.Checked = _eventToEdit.IsPublished;

                cmbCategory.SelectedValue = _eventToEdit.CategoryId;
                cmbClub.SelectedValue = _eventToEdit.ClubId;
            }
            catch (Exception ex) { MessageBox.Show("Veriler yüklenirken hata: " + ex.Message); }
        }

        private void BtnAction_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) { MessageBox.Show("Başlık boş olamaz."); return; }

            try
            {
                if (isEditMode)
                {
                    // GÜNCELLEME MODU
                    _eventToEdit.Title = txtTitle.Text;
                    _eventToEdit.Description = txtDescription.Text;
                    _eventToEdit.EventDate = dtpDate.Value;
                    _eventToEdit.Location = txtLocation.Text;
                    _eventToEdit.Quota = (int)nudQuota.Value;
                    _eventToEdit.CategoryId = (int)cmbCategory.SelectedValue;
                    _eventToEdit.ClubId = (int)cmbClub.SelectedValue;
                    _eventToEdit.PosterUrl = txtPosterUrl.Text;
                    _eventToEdit.IsPublished = chkPublish.Checked;

                    _eventDal.UpdateEvent(_eventToEdit);
                    MessageBox.Show("Etkinlik güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // YENİ KAYIT MODU
                    Event newEvent = new Event
                    {
                        Title = txtTitle.Text,
                        Description = txtDescription.Text,
                        EventDate = dtpDate.Value,
                        Location = txtLocation.Text,
                        Quota = (int)nudQuota.Value,
                        CategoryId = (int)cmbCategory.SelectedValue,
                        ClubId = (int)cmbClub.SelectedValue,
                        PosterUrl = txtPosterUrl.Text,
                        IsPublished = chkPublish.Checked,
                        AverageRating = 0
                    };

                    _eventDal.AddEvent(newEvent);
                    MessageBox.Show("Yeni etkinlik oluşturuldu!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                GoBack(); // Listeye dön
            }
            catch (Exception ex) { MessageBox.Show("İşlem Hatası: " + ex.Message); }
        }

        private void GoBack()
        {
            FormMain main = (FormMain)this.FindForm();
            if (main != null) {
                main.pnlContent.Controls.Clear();
                // İşlem bitince tekrar Etkinlik Listesine dön
                main.pnlContent.Controls.Add(new PageEvents());
            }
        }
    }
}