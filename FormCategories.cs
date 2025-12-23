using System;
using System.Drawing;
using System.Windows.Forms;
using CampusEventManager.DataAccess;
using CampusEventManager.Entities;

namespace CampusEventManager
{
    public class FormCategories : Form
    {
        // UI Elemanları
        private ListBox lstCategories;
        private TextBox txtName;
        private Button btnAdd, btnDelete;
        
        // Veri Katmanı
        private CommonDal _commonDal;

        public FormCategories()
        {
            this.Text = "Kategori Yönetimi";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow; // Sadece kapatma tuşu olsun

            _commonDal = new CommonDal();
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            Label lblTitle = new Label { Text = "Kategoriler", Location = new Point(20, 20), Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true };
            this.Controls.Add(lblTitle);

            // 1. LİSTE (Kategorileri gösteren kutu)
            lstCategories = new ListBox { Location = new Point(20, 50), Size = new Size(340, 300), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(lstCategories);

            // 2. EKLEME ALANI
            txtName = new TextBox { Location = new Point(20, 370), Size = new Size(240, 30), Font = new Font("Segoe UI", 10), PlaceholderText = "Kategori Adı..." };
            this.Controls.Add(txtName);

            btnAdd = new Button { Text = "EKLE", Location = new Point(270, 368), Size = new Size(90, 32), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            // 3. SİLME BUTONU
            btnDelete = new Button { Text = "Seçili Kategoriyi Sil", Location = new Point(20, 415), Size = new Size(340, 35), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);
        }

        private void LoadData()
        {
            try
            {
                lstCategories.Items.Clear();
                var cats = _commonDal.GetAllCategories();
                
                // Kategorileri ListBox'a ekle
                foreach (var cat in cats)
                {
                    // Hem adını gösterip hem ID'sini saklamak için özel nesne ekliyoruz
                    lstCategories.Items.Add(new CategoryItem { Id = cat.CategoryId, Name = cat.CategoryName });
                }
            }
            catch (Exception ex) { MessageBox.Show("Veri yüklenemedi: " + ex.Message); }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) return;
            try
            {
                _commonDal.AddCategory(txtName.Text.Trim());
                txtName.Clear();
                LoadData(); // Listeyi yenile
                MessageBox.Show("Kategori eklendi.");
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstCategories.SelectedItem == null) 
            {
                MessageBox.Show("Lütfen silmek için listeden bir kategori seçin.");
                return;
            }
            
            // Seçili öğeyi al
            var selectedItem = (CategoryItem)lstCategories.SelectedItem;

            if (MessageBox.Show($"'{selectedItem.Name}' kategorisini silmek istediğine emin misin?\n(Dikkat: Bu kategoriye bağlı etkinlikler etkilenebilir!)", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _commonDal.DeleteCategory(selectedItem.Id);
                    LoadData(); // Listeyi yenile
                    MessageBox.Show("Kategori silindi.");
                }
                catch (Exception ex) 
                { 
                    // Yabancı anahtar hatası (Foreign Key) olursa burası çalışır
                    MessageBox.Show("Bu kategori SİLİNEMEZ çünkü şu an bazı etkinliklerde kullanılıyor!\nÖnce o etkinlikleri silmelisiniz.", "Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                }
            }
        }

        // ListBox içinde ID ve İsim tutmak için yardımcı sınıf
        private class CategoryItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public override string ToString() => Name; // ListBox'ta sadece isim görünür
        }
    }
}