namespace Insurance.Domain.Common
{
    public class BaseFileEntity : AuditedEntity
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public string FileExtension { get; set; }
        public string OriginalFileName { get; set; }
    }
}