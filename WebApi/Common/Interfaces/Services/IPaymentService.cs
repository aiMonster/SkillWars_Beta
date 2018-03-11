using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Common.Interfaces.Services
{
    public interface IPaymentService
    {
        Task NewTransaction(List<KeyValuePair<string, StringValues>> query);
    }
}
