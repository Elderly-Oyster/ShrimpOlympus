using System;
using System.Threading.Tasks;

namespace MVP.MVP_Root_Model.Scripts.Services.LongInitializationServices
{
    public class FirstLongInitializationService : LongInitializationService
    {
        public FirstLongInitializationService() => DelayTime = 1;
    }
}