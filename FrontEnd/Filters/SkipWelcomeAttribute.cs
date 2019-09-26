using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEnd.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipWelcomeAttribute : Attribute, IFilterMetadata
    {

    }
}
