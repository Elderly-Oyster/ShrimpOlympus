using System;
using System.Threading.Tasks;

namespace MVP.MVP_Root_Model.Scripts.Services.LongInitializationServices
{
    public class SecondLongInitializationService : LongInitializationService
    {
        public SecondLongInitializationService() => DelayTime = 3;
    }
}