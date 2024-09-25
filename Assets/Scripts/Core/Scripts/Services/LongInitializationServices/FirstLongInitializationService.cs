namespace Core.Scripts.Services.LongInitializationServices
{
    public class FirstLongInitializationService : LongInitializationService
    {
        public FirstLongInitializationService() => DelayTime = 1;
    }
}