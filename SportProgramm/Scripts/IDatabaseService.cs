using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportProgramm.Scripts
{
    public interface IDatabaseService
    {
        bool IsConnected { get; }
        void Initialize();
        void TestConnection();
        void CreateBackup();
        void RestoreFromBackup(string backupPath);
    }
}
