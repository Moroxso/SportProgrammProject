using SportProgramm.BaseDate;
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
        SportProgrammProjectEntities Context { get; }
        void Initialize();
        void TestConnection();
        void CreateBackup();
        void RestoreFromBackup(string backupPath);
    }
}
