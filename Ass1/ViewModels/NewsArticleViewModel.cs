using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ass1.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ass1.ViewModels
{
    public class NewsArticleViewModel
    {
        public string? NewsArticleId { get; set; }

        [Required(ErrorMessage = "News Title is required.")]
        public string NewsTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Headline is required.")]
        public string Headline { get; set; } = string.Empty;

        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "News Content is required.")]
        public string NewsContent { get; set; } = string.Empty;

        [Required(ErrorMessage = "News Source is required.")]
        public string NewsSource { get; set; } = string.Empty;

        public List<Category>? Categories { get; set; } = new();

        [Required(ErrorMessage = "Please select a category.")]

        public short? SelectedCategoryId { get; set; }

        public string? SelectedCategoryName { get; set; }

        public bool NewsStatus { get; set; } = true;

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public List<Tag>? Tags { get; set; } = new();

        public List<int>? SelectedTagIds { get; set; }

        public List<string>? SelectedTagNames { get; set; }
    }
}
