using System;
using System.Collections.Generic;
using System.Text;

namespace DVG.WIS.Caching.Configs
{
    public class FileCacheDependency
    {
        public FileCacheDependency(string filename, Microsoft.Extensions.Primitives.IChangeToken filewatch = null)
        {
            FileName = filename;
            FileWatch = filewatch;
        }
        public string FileName { get; }
        public Microsoft.Extensions.Primitives.IChangeToken FileWatch { get; }
    }
}
