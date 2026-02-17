namespace ORG.BasicInfo.Domain.FormUserAggregate
{
    public class FilesFormsSys 
    {
        public Guid Id { get; private set; }
        public Guid IdFormUser { get; private set; }
        public string Title { get; private set; }
 
        public ulong FileSize { get; private set; }
        public long UploadDate { get; private set; }
        public Guid LUserCreate { get; private set; }
        public string KeyFile { get; private set; }


        public FilesFormsSys(
             Guid idFormUser,
   
            string title,
            ulong fileSize,
            string keyFile,
            Guid lUserCreate

            ) 
        {
            Id = Guid.NewGuid();
            UploadDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            LUserCreate = lUserCreate;
            IdFormUser = idFormUser;
            Title = title;
            FileSize = fileSize;
            KeyFile = keyFile;
       
        }

    }

}
