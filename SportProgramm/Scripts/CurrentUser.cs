using SportProgramm.BaseDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportProgramm.Scripts
{
    public static class CurrentUser
    {
        public static Users User { get; set; }
        public static bool IsAuthenticated => User != null;
        public static bool IsAdmin => User?.IdRole == 1;
        public static string DisplayName => User?.Name ?? "Гость";
    }
}
