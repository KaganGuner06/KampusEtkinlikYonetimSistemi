using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;
using System.IO;

namespace CampusEventManager
{
    public partial class FormAddClub : Form
    {
        private TextBox txtName, txtShortDesc, txtInstagram, txtLinkedin;
        private RichTextBox rtxtFullDesc;
        private ComboBox cmbCategory;
        private PictureBox pbLogo, pbCover;
        private CheckBox chkApprovalRequired;
        private Button btnSave;
        
        
        private Club _existingClub = null;
        private string logoPath = null;
        private string coverPath = null;

        private ClubDal _clubDal;
        private CommonDal _commonDal;

        
        public FormAddClub(Club clubToEdit = null)
        {
            _existingClub = clubToEdit;
            
            
            this.Text = _existingClub == null ? "YENİ KULÜP OLUŞTUR" : "KULÜBÜ DÜZENLE";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            _clubDal = new ClubDal();
            _commonDal = new CommonDal();

            SetupUI();
            LoadCategories();

            
            if (_existingClub != null)
            {
                FillData();
            }
        }

        private void SetupUI()
        {
            
            Panel pnlLeft = new Panel { Location = new Point(20, 20), Size = new Size(420, 600), BackColor = Color.White };
            this.Controls.Add(pnlLeft);

            
            Panel pnlRight = new Panel { Location = new Point(460, 20), Size = new Size(400, 600), BackColor = Color.White };
            this.Controls.Add(pnlRight);

            
            int y = 20;
            Label lblTitle = new Label { Text = "Kulüp Bilgileri", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, y), AutoSize = true, ForeColor = Color.DarkSlateBlue };
            pnlLeft.Controls.Add(lblTitle); y += 40;

            AddLabelAndInput(pnlLeft, "Kulüp Adı:", ref y, out txtName);

            Label lCat = new Label { Text = "Kategori:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cmbCategory = new ComboBox { Location = new Point(20, y + 25), Width = 380, DropDownStyle = ComboBoxStyle.DropDownList };
            pnlLeft.Controls.Add(lCat); pnlLeft.Controls.Add(cmbCategory); y += 70;

            chkApprovalRequired = new CheckBox { Text = "Üyelik için Onay Gerekir", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10) };
            pnlLeft.Controls.Add(chkApprovalRequired); y += 40;

            AddLabelAndInput(pnlLeft, "Kısa Açıklama (Max 150):", ref y, out txtShortDesc);

            Label lDesc = new Label { Text = "Detaylı Açıklama:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            rtxtFullDesc = new RichTextBox { Location = new Point(20, y + 25), Width = 380, Height = 120, BorderStyle = BorderStyle.FixedSingle };
            pnlLeft.Controls.Add(lDesc); pnlLeft.Controls.Add(rtxtFullDesc);

            
            int ry = 20;
            Label lblImg = new Label { Text = "Görseller & Medya", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, ry), AutoSize = true, ForeColor = Color.DarkSlateBlue };
            pnlRight.Controls.Add(lblImg); ry += 40;

            
            Label lLogo = new Label { Text = "Logo:", Location = new Point(20, ry), AutoSize = true };
            pbLogo = new PictureBox { Location = new Point(20, ry + 25), Size = new Size(100, 100), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.WhiteSmoke };
            Button btnLogo = new Button { Text = "Seç", Location = new Point(130, ry + 25), Width = 60 };
            btnLogo.Click += (s, e) => SelectImage(pbLogo, ref logoPath);
            pnlRight.Controls.Add(lLogo); pnlRight.Controls.Add(pbLogo); pnlRight.Controls.Add(btnLogo); ry += 140;

            
            Label lCover = new Label { Text = "Kapak Fotoğrafı:", Location = new Point(20, ry), AutoSize = true };
            pbCover = new PictureBox { Location = new Point(20, ry + 25), Size = new Size(350, 120), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.StretchImage, BackColor = Color.WhiteSmoke };
            Button btnCover = new Button { Text = "Seç", Location = new Point(20, ry + 150), Width = 350 };
            btnCover.Click += (s, e) => SelectImage(pbCover, ref coverPath);
            pnlRight.Controls.Add(lCover); pnlRight.Controls.Add(pbCover); pnlRight.Controls.Add(btnCover); ry += 200;

            
            AddLabelAndInput(pnlRight, "Instagram Link:", ref ry, out txtInstagram);
            AddLabelAndInput(pnlRight, "LinkedIn Link:", ref ry, out txtLinkedin);

            
            btnSave = new Button { 
                Text = _existingClub == null ? "KULÜBÜ OLUŞTUR" : "DEĞİŞİKLİKLERİ KAYDET", 
                Location = new Point(20, 530), Size = new Size(360, 50), 
                BackColor = _existingClub == null ? Color.SeaGreen : Color.Orange, 
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold) 
            };
            btnSave.Click += BtnSave_Click;
            pnlRight.Controls.Add(btnSave);
        }

        private void AddLabelAndInput(Panel pnl, string text, ref int currentY, out TextBox txtBox)
        {
            Label l = new Label { Text = text, Location = new Point(20, currentY), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtBox = new TextBox { Location = new Point(20, currentY + 25), Width = pnl.Width - 40, BorderStyle = BorderStyle.FixedSingle };
            pnl.Controls.Add(l); pnl.Controls.Add(txtBox);
            currentY += 60;
        }

        private void SelectImage(PictureBox pb, ref string pathRef)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Resim|*.jpg;*.png" };
            if (ofd.ShowDialog() == DialogResult.OK) { pb.Image = Image.FromFile(ofd.FileName); pathRef = ofd.FileName; }
        }

        private void LoadCategories()
        {
            try {
                cmbCategory.DataSource = _commonDal.GetAllCategories();
                cmbCategory.DisplayMember = "CategoryName";
                cmbCategory.ValueMember = "CategoryId";
            } catch { 
                cmbCategory.Items.Add("Genel"); cmbCategory.SelectedIndex = 0;
            }
        }

        
        private void FillData()
        {
            txtName.Text = _existingClub.ClubName;
            txtShortDesc.Text = _existingClub.Description;
            rtxtFullDesc.Text = _existingClub.FullDescription;
            cmbCategory.SelectedValue = _existingClub.CategoryId;
            chkApprovalRequired.Checked = _existingClub.RequiresApproval;
            txtInstagram.Text = _existingClub.InstagramLink;
            txtLinkedin.Text = _existingClub.LinkedinLink;
            
            
            logoPath = _existingClub.LogoUrl;
            coverPath = _existingClub.CoverUrl;

            
            try { if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath)) pbLogo.Image = Image.FromFile(logoPath); } catch {}
            try { if (!string.IsNullOrEmpty(coverPath) && File.Exists(coverPath)) pbCover.Image = Image.FromFile(coverPath); } catch {}
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Kulüp adı zorunludur."); return; }
            
            try
            {
                Club c = new Club
                {
                    ClubName = txtName.Text,
                    Description = txtShortDesc.Text,
                    FullDescription = rtxtFullDesc.Text,
                    CategoryId = cmbCategory.SelectedValue != null ? (int)cmbCategory.SelectedValue : 1,
                    
                    ManagerUserId = _existingClub == null ? (Session.CurrentUser != null ? Session.CurrentUser.UserId : 1) : _existingClub.ManagerUserId,
                    IsActive = true,
                    RequiresApproval = chkApprovalRequired.Checked,
                    LogoUrl = logoPath,
                    CoverUrl = coverPath,
                    InstagramLink = txtInstagram.Text,
                    LinkedinLink = txtLinkedin.Text
                };

                if (_existingClub == null)
                {
                    
                    _clubDal.AddClub(c);
                    MessageBox.Show("Kulüp başarıyla oluşturuldu!");
                }
                else
                {
                    
                    c.ClubId = _existingClub.ClubId;
                    _clubDal.UpdateClub(c);
                    MessageBox.Show("Kulüp bilgileri güncellendi!");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }
    }
}