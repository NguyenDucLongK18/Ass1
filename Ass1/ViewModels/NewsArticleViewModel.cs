using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public Category? Category { get; set; } = new();

        [Required(ErrorMessage = "Please select a category.")]

        public short? SelectedCategoryId { get; set; }

        public string? SelectedCategoryName { get; set; }

        public bool NewsStatus { get; set; } = true;

        [NotMapped]
        public string? CreatedBy { get; set; }

        [NotMapped]
        public string? UpdatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public List<Tag>? SelectedTags { get; set; } = new();

        public List<int>? SelectedTagIds { get; set; }

        public List<string>? SelectedTagNames { get; set; }

        public short? CreatedById { get; set; }

        public short? UpdatedById { get; set; }

        public string? newsTitleSearch { get; set; }
    }
}