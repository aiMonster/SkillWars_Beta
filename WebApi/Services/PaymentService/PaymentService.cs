using Common.Interfaces.Services;
using DataAccessLayer.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly MSContext _context;
        private readonly string _securityKey;
        private readonly string _ik_co_id;

        public PaymentService(MSContext context, IConfigurationRoot root)
        {
            _context = context;
            _securityKey = root.GetSection("Interkassa")["securityKey"];
            _ik_co_id = root.GetSection("Interkassa")["ik_co_id"];
        }

        public async Task NewTransaction(List<KeyValuePair<string, StringValues>> query)
        {
            try
            {                
                if(query.Where(u => u.Key == "ik_co_id").FirstOrDefault().Value != _ik_co_id)
                {                    
                    return;
                }
                else if(query.Where(u => u.Key == "ik_inv_st").FirstOrDefault().Value != "success")
                {                   
                    return;
                }

                //checking ik_sign
                var ik_sign = query.Where(k => k.Key == "ik_sign").FirstOrDefault();
                query.Remove(ik_sign);
                query.Sort(CompareByKey);
                string result = "";
                foreach(var kv in query)
                {
                    result += kv.Value + ":";
                }
                result += _securityKey;

                MD5 md5Hasher = MD5.Create();
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(result));
                if(ik_sign.Value != Convert.ToBase64String(data))
                {
                    return;
                }                

                var userId = Convert.ToInt32(query.Where(k => k.Key == "ik_pm_no").FirstOrDefault().Value);
                var amount = Convert.ToInt32(query.Where(k => k.Key == "ik_co_rfn").FirstOrDefault().Value);

                var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
                if(user != null)
                {
                    user.Balance += amount;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //send message that we have got money from unknown 
                }

                //send notification for that user

            }
            catch(Exception ex)
            {
                HardLogger.Logs += "We are in catch\n";
                Console.WriteLine();
               
            }
            return;
        }

        static int CompareByKey(KeyValuePair<string, StringValues> a, KeyValuePair<string, StringValues> b)
        {
            return a.Key.CompareTo(b.Key);
        }

    }
}
