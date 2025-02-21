using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ass1.Models
{
    public partial class NewsArticle
    {
        [StringLength(20)]
        public string NewsArticleId { get; set; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
        .Replace("/", "_").Replace("+", "-")
        .Substring(0, 18);

        public string? NewsTitle { get; set; }

        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedById { get; set; }

        public short? UpdatedById { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual Category? Category { get; set; }

        public virtual SystemAccount? CreatedBy { get; set; }

        public virtual SystemAccount? UpdatedBy { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}