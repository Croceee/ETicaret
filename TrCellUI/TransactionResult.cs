using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrCellUI
{
    public class TransactionResult<X>
    {
        public bool Result { get; set; }

        public string ErrorType { get; set; }

        public string ErrorMessage { get; set; }

        public string Description { get; set; }

        public X Entity { get; set; }

        public IEnumerable<X> Entities { get; set; }
    }
}
