namespace Core.Scripts.Services.LongInitializationServices
{
    public class SecondLongInitializationService : LongInitializationService
    {
        public SecondLongInitializationService() => DelayTime = 3;
    }
}