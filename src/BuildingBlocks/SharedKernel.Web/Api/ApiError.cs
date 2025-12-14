using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Web.Api
{
    public sealed record ApiError(
    string Code,
    string Message,
    IDictionary<string, string[]>? Details = null
);
}
