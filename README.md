## KampusEtkinlikYonetimSistemi

Bu proje, üniversite kampüsündeki etkinliklerin yönetilmesi, öğrenci başvuruları ve kulüp yönetimini sağlayan bir C# WinForms otomasyonudur.

## Özellikler
- **Öğrenci Paneli:** Etkinlikleri listeleme, filtreleme ve başvuru yapma.
- **Yönetici Paneli:** Etkinlik oluşturma, başvuruları onaylama/reddetme.
- **Trigger Kullanımı:** Kontenjan dolduğunda otomatik başvuru kapatma, kayıt olunurken üniversite e-mail kontrolü yapma.
- **Stored Procedure:** Gelişmiş raporlama ve onay işlemleri.

## Kurulum
1. Projeyi bilgisayarınıza indirin.
2. `Database` klasöründeki **setup.sql** dosyasını pgAdmin'de "Query Tool" ile açıp çalıştırarak veritabanını oluşturun.
3. `DataAccess/DbHelper.cs` dosyasındaki bağlantı şifresini kendi PostgreSQL şifrenizle güncelleyin.
4. `CampusEventManager.sln` dosyasına çift tıklayarak projeyi başlatın.

## Kullanılan Teknolojiler
- C# .NET (Windows Forms)
- PostgreSQL
- N-Tier Architecture (Katmanlı Mimari)
