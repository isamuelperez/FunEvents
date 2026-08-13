using System;
using System.Collections.Generic;
using System.Text;

namespace FunEvents.Application.Common.Results
{
    public sealed record ResultError(
        string Code,
        string Message
    );
}
