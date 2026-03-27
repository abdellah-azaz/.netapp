using System;

namespace MonAppMultiplateforme.Models
{
    public class VaultFile
    {
        public string file_id { get; set; } = string.Empty;
        public string filename { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        
        // UI helper properties
        public string DisplayName => filename;
        public string CreatedAtFormatted => created_at.ToString("dd/MM/yyyy HH:mm");
    }
}
