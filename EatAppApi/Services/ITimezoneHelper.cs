using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatAppApi.Services
{
    public interface ITimezoneHelper
    {
        string ServerTimezoneId { get; }
        string GetServerTimezoneOffset();
        DateTime ConvertToClientTime(DateTime dateTime);
        
    }
}
