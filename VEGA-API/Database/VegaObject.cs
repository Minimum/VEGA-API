using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VEGA_API.Database
{
    public abstract class VegaObject
    {
        public virtual VegaObject Mask()
        {
            return this;
        }

        protected byte[] GetHash(String data)
        {
            byte[] byteData;

            using (SHA512 hash = SHA512.Create())
            {
                byteData = hash.ComputeHash(Encoding.UTF8.GetBytes(data));
            }

            return byteData;
        }
    }
}
