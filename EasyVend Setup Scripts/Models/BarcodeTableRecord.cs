using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class BarcodeTableRecord
    {
        public int ID { get; set; }

        public string Barcode { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }
    }
}
