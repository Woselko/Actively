using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayMakerDomain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public virtual Player Player { get; set; }
        public int PlayerId { get; set; }
        public bool IsPaymentCompleted { get; set; } = false;
        public MethodOfPayemnt? MethodOfPayemnt { get; set; } = null;
        public virtual Game Game { get; set; }
        public int GameId { get; set; }
        public float Amount { get; set; } // double/decimal?

    }
}
