using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync
{
    public static class LoadUtils
    {

        public static T Load<T>(string Path, bool CreateIfNotExists = true) where T : ISavable
        {
            var path = Path;
            var obj = FileUtils.LoadFromJsonFile<T>(path);
            if (obj == null)
            {
                if (FileUtils.FileExists(path))
                {
                    FileUtils.BackupFile(path);
                }

                if (CreateIfNotExists)
                {
                    obj = Activator.CreateInstance<T>();
                    obj.Save();
                }
            }
            return obj;
        }
    }
}
