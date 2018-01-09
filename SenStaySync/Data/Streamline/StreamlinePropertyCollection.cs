namespace SenStaySync.Data.Streamline
{
    using System;
    using System.Collections.ObjectModel;

    public class StreamlinePropertyCollection : Collection<StreamlinePropertyInfo>
    {
        public DateTime DateCreated = DateTime.Now;

        public void Save()
        {
            var Path = Config.I.PropertyCollectionJsonPath;
            if (FileUtils.FileExists(Path))
            {
                FileUtils.BackupFile(Path);
            }
            this.SaveToFileAsJson(Path);
        }

        public static StreamlinePropertyCollection Load()
        {
            var FilePath = Config.I.PropertyCollectionJsonPath;
            return FileUtils.LoadFromJsonFile<StreamlinePropertyCollection>(FilePath);
        }


        public SenStayPropertyIndex CreateIndex()
        {
            var Index = new SenStayPropertyIndex();

            foreach (var Item in this)
            {
                Index.Add(Item);
            }

            return Index;
        }
    }
}