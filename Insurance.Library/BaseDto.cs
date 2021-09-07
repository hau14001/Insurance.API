using System;

namespace Be.Library
{
    public class BaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
