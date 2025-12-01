namespace Business.Constants
{
    /// Proje genelinde kullanılacak tüm sabit metinleri ve mesajları tutar.
    public static class Messages
    {
        // --- Genel Mesajlar ---
        public static string OperationSuccessful = "İşlem başarılı.";

        // --- User Mesajları ---
        public static string UserAdded = "Kullanıcı başarıyla eklendi.";
        public static string UserUpdated = "Kullanıcı bilgileri güncellendi.";
        public static string UserDeleted = "Kullanıcı silindi.";
        public static string UserNotFound = "Kullanıcı bulunamadı.";
        public static string UsersListed = "Kullanıcılar listelendi.";
        public static string UserFetched = "Kullanıcı bilgisi getirildi.";
        public static string UserEmailAlreadyExists = "Bu e-posta adresi zaten kullanımda.";

        // --- Medication Mesajları ---
        public static string MedicationAdded = "İlaç başarıyla eklendi.";
        public static string MedicationUpdated = "İlaç bilgileri güncellendi.";
        public static string MedicationDeleted = "İlaç silindi.";
        public static string MedicationNotFound = "İlaç bulunamadı.";
        public static string MedicationsListed = "İlaçlar listelendi.";
        public static string MedicationsListedForUser = "Kullanıcının ilaçları listelendi.";

        // --- Caregiver/Patient İlişki Mesajları ---
        public static string PatientIsAlreadyFollowed = "Bu hasta zaten takip ediliyor.";
        public static string FollowRelationshipNotFound = "Takip ilişkisi bulunamadı.";
        public static string UserIsNowFollowing = "Hasta takibe alındı.";
        public static string UserUnfollowed = "Hasta takipten çıkarıldı.";
        public static string PatientsListed = "Hastalar listelendi.";
        public static string CaregiversListed = "Hasta yakınları listelendi.";

        // --- Intake Mesajları ---
        public static string IntakeNotFound = "İlaç alım kaydı bulunamadı.";
        public static string IntakeMarkedAsTaken = "İlaç 'alındı' olarak işaretlendi.";
        public static string IntakeMarkedAsSkipped = "İlaç 'atlandı' olarak işaretlendi.";
        public static string IntakeStatusUpdated = "İlaç alım durumu güncellendi.";
        public static string IntakesListed = "İlaç alım kayıtları listelendi.";
    }
}