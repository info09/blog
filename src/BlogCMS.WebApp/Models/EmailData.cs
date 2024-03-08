﻿using System.ComponentModel.DataAnnotations;

namespace BlogCMS.WebApp.Models
{
    public class EmailData
    {
        [EmailAddress]
        public string From { get; set; }

        [EmailAddress]
        public string ToAddress { get; set; }

        public IEnumerable<string> ToAddresses { get; set; } = new List<string>();

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        public IFormFileCollection Attachments { get; set; } = null;
    }
}
