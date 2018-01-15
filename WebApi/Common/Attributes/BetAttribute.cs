using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Attributes
{
    public class BetAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                double bet = Convert.ToDouble(value);
                if (bet > 0)
                    return true;
                else
                    this.ErrorMessage = "Bet cannot be <= 0!! IDIOT";
            }
            return false;
        }
    }
}
