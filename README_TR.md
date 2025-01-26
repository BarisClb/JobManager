# JobManager - Merkezi Zamanlanmış Görev Yöneticisi

JobManager, **merkezi bir zamanlanmış görev yöneticisi** olup, **geliştirme sürecini hızlandırmak** amacıyla tasarlanmıştır. **Görev gruplama**, **dinamik görev yönetimi ve çalıştırması** ve **yapılandırılmış loglama** sağlar.

### 🚀 Temel Odak Alanları:
- **Görev Gruplama & Ayrıştırma:**  
  - Her servis, kendi görev grubuyla çalışır; diğer gruplardaki görevleri başlatamaz.
  - `appsettings.json` ve ortam değişkenleri ile yönetilir.
  - Tek bir kod tabanı, tüm görev gruplarını destekler.

- **Dinamik Görev Çalıştırma:**  
  - **Swagger üzerinden görev çalıştırma**, Hangfire paneline bağlı olmadan mümkündür.
  - **Dinamik parametreler** desteklenir ve isimlendirme kurallarına göre alınır.

- **Gelişmiş Zamanlama & Kontrol:**  
  - Tek tek veya tüm görevlerin zamanlamasını güncelleme.
  - **Varsayılan zamanlar, devre dışı bırakma, ve kaldırma** seçenekleri desteklenir.

- **Özel Loglama & İzleme:**  
  - **Görev detayları, çalışma modu, ortam bilgisi** loglanır.
  - **Elasticsearch** ve **Serilog** ile yapılandırılmış loglama kullanılır.

- **Otomatik Bağımlılık Enjeksiyonu:**  
  - Servisler dışındaki tüm bileşenler otomatik olarak enjekte edilir.
  - **Servis enjeksiyonu manuel** yapılır, çünkü çapraz bağımlılıklar Microsoft tarafından desteklenmez.

## 🛠️ Kullanılan Teknolojiler

- **Hangfire** (Görev Planlayıcı)
- **Elasticsearch** (Loglama ve İzleme)
- **Serilog** (Yapılandırılmış Loglama)
- **Swagger Entegrasyonu** (Dinamik görev çalıştırma & konfigürasyon)

## 🌐 API Uç Noktaları

### 🔄 Görev Yönetimi & Çalıştırma

```http
POST /StartJobByJobName
```
**Açıklama:** Belirtilen görevi dinamik olarak başlatır.

```http
GET /GetJobParametersJobByName
```
**Açıklama:** Görev parametrelerini getirir.

```http
PATCH /UpdateJobTimeByName
```
**Açıklama:** Bir görevin cron zamanlamasını günceller.

```http
PATCH /UpdateAllJobsWithDefaultTimes
```
**Açıklama:** Tüm görevleri `appsettings.json`'daki varsayılan zamanlamaya günceller.

```http
DELETE /DeactivateAllJobs
```
**Açıklama:** Tüm planlanmış görevleri devre dışı bırakır.

```http
DELETE /RemoveAllJobs
```
**Açıklama:** Hangfire'daki tüm tekrarlayan görevleri kaldırır.

### 📜 Yapılandırma & Sabitler

```http
GET /JobSettings
```
**Açıklama:** Aktif görev ayarlarını, varsayılan zamanlamaları ve hata ayıklama bilgilerini getirir.

```http
GET /AppSettings
```
**Açıklama:** Uygulama ortamı ve proje ayarlarını getirir.

```http
GET /AppConstants
```
**Açıklama:** `appsettings.json` içindeki **Constants** klasörüne eklenen tüm sabitleri getirir.

## 🏗️ Sistem Akışı

1. **Görev Başlatma & Gruplama**
   - Görevler `appsettings.json` ve **PROJECT** ortam değişkenine göre yüklenir.
   - Her servis, yalnızca kendi görev grubuna erişebilir.

2. **Görev Çalıştırma & Zamanlama**
   - **Swagger** üzerinden görev çalıştırılabilir.
   - **Varsayılan zamanlar, devre dışı bırakma (31 Şubat) ve kaldırma seçenekleri** desteklenir.

3. **Bağımlılık Enjeksiyonu & Loglama**
   - **Otomatik enjeksiyon**: Görevler, işleyiciler ve tekrarlayan işler.
   - **Manuel enjeksiyon**: Servisler (çapraz bağımlılıklar nedeniyle).
   - **Loglama**: Görev süreci, mod bilgisi ve parametreler loglanır.
