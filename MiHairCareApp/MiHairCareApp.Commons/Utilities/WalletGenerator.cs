using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Commons.Utilities
{
    public static class WalletGenerator
    {
        public static string SetWalletId(string phoneNumber)
        {
            phoneNumber = phoneNumber.Trim(); // Trim leading and trailing spaces

            if (phoneNumber.StartsWith("+234"))
            {
                phoneNumber = phoneNumber.Substring(4);
            }
            else if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = phoneNumber.Substring(1);
            }
            else
            {
                throw new Exception("Phone number must start with +234 or 0");
            }

            // Check if remaining numbers are exactly 10 digits
            if (phoneNumber.Length != 10 || !phoneNumber.All(char.IsDigit))
            {
                throw new Exception("Phone number must be 10 digits long and contain only digits");
            }

            return phoneNumber;
        }

    }
}
