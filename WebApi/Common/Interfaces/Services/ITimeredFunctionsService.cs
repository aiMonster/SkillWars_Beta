using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Services
{
    public interface ITimeredFunctionsService
    {
        Task<bool> Setup();
        
        Task CheckEndsTokenExpirates();
    }
}
