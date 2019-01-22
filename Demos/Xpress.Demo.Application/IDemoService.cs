using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.Application;

namespace Xpress.Demo.Application
{
    public interface IDemoService : IApplicationService
    {
        void WriteMessage(string message);
    }
}
