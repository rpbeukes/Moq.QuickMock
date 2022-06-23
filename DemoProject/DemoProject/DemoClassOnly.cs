using Microsoft.Extensions.Logging;
using System;

namespace DemoProject
{
    public class DemoClassOnly
    {
        // big ctor
        public DemoClassOnly(ILogger<DemoClassOnly> logger,
                             string stringValue,
                             int intValue,
                             int? nullIntValue,
                             ICurrentUser currentUser,
                             Func<SomeCommand> cmdFactory,
                             Func<IValidator<InvoiceDetailsInput>> validatorFactory)
        {

        }

        // diff ctor
        public DemoClassOnly(ILogger<DemoClassOnly> logger,
                             string stringValue,
                             int intValue,
                             int? nullIntValue
                             )
        {
        }
    }
}
