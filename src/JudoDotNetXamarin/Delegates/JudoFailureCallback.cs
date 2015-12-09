using JudoDotNetXamarin;
using JudoPayDotNet.Models;

namespace JudoDotNetXamarin
{
    public delegate void JudoFailureCallback (JudoError error, PaymentReceiptModel receipt = null);
}

