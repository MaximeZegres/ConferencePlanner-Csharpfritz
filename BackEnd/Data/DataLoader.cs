using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Data
{
    public abstract class DataLoader
    {
        public abstract Task LoadDataAsync(string conferenceName, Stream fileStream, ApplicationDbContext db);
    }
}
