using CampusEventManager.Entities;

namespace CampusEventManager
{
    // Giriş yapan kullanıcıyı program kapanana kadar burada tutacağız
    public static class Session
    {
        public static User? CurrentUser = null;
    }
}