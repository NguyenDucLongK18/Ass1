using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ass1.Models;

public partial class SystemAccount
{
    [Key]
    public short AccountId { get; set; }

    public string? AccountName { get; set; }

    public string? AccountEmail { get; set; }

    public int? AccountRole { get; set; }

    public string? AccountPassword { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<NewsArticle> NewsArticleCreatedBies { get; set; } = new List<NewsArticle>();

    public virtual ICollection<NewsArticle> NewsArticleUpdatedBies { get; set; } = new List<NewsArticle>();
}
