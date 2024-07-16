using System;
using System.Threading.Tasks;

namespace MVP.MVP_Root_Model.Scripts.Services.LongInitializationServices
{
    public class ThirdLongInitializationService : LongInitializationService
    {
        public ThirdLongInitializationService() => DelayTime = 2;
    }
}