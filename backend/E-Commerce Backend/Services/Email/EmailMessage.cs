namespace E_Commerce_Backend.Services.EmailService
{
    public class EmailMessage
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; } = string.Empty;
        public string TemplateName { get; set; } // e.g., "WelcomeEmail"
        public object TemplateModel { get; set; } // The dynamic data for the template
                                                  // public List<Attachment> Attachments { get; set; } // Easy to add later!
    }
}
