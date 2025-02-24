using Ass1.Models;
using Ass1.Repositories;
using Ass1.Services;
using Ass1.Utils;
using Ass1.ViewModels;
using NuGet.Packaging;
using System.Reflection;

public class NewsArticleService : IService<NewsArticle, NewsArticleViewModel>
{
    private readonly NewsArticleRepository _repository;

    private readonly TagService _tagService;

    private readonly UserManagerService _userManagerService;

    private readonly MailUtils _mailUtils;

    public NewsArticleService(NewsArticleRepository repository, TagService tagService, UserManagerService userManagerService, MailUtils mailUtils)
    {
        _repository = repository;
        _tagService = tagService;
        _userManagerService = userManagerService;
        _mailUtils = mailUtils;
    }

    public List<NewsArticleViewModel> GetPaginatedNews(bool isStaff, string newsArticleSearch, int page, int pageSize, out int totalArticles)
    {
        var articles = _repository.GetPaginated(isStaff, newsArticleSearch, page, pageSize, out totalArticles);
        return articles.Select(ConvertToViewModel).ToList();
    }

    public NewsArticleViewModel GetNewsArticle(string id)
    {
        var article = _repository.GetById(id);
        return article == null ? null : ConvertToViewModel(article);
    }

    public async Task CreateNewsArticle(NewsArticleViewModel articleViewModel)
    {
        var selectedTag = _tagService.GetByIds(articleViewModel.SelectedTagIds);

        articleViewModel.NewsArticleId = Guid.NewGuid().ToString("N").Substring(0, 20);

        articleViewModel.SelectedTags = selectedTag;

        var timeZoneInfo = TimeUtils.asiaTime();

        var article = ConvertToEntity(articleViewModel);
        _repository.Insert(article);

        await SendMailNotification(articleViewModel);
    }

    public void UpdateNewsArticle(NewsArticleViewModel oldViewModel, NewsArticleViewModel newViewModel)
    {
        if (oldViewModel == null || newViewModel == null)
        {
            throw new ArgumentNullException("ViewModels cannot be null.");
        }

        newViewModel.ModifiedDate = TimeUtils.asiaTime();

        newViewModel.SelectedTags = _tagService.GetByIds(newViewModel.SelectedTagIds) ?? new List<Tag>();

        var entity = _repository.GetById(newViewModel.NewsArticleId);

        //update properties
        entity.NewsTitle = newViewModel.NewsTitle;
        entity.Headline = newViewModel.Headline;
        entity.NewsContent = newViewModel.NewsContent;
        entity.NewsSource = newViewModel.NewsSource;
        entity.CategoryId = newViewModel.SelectedCategoryId;
        entity.NewsStatus = newViewModel.NewsStatus;

        entity.ModifiedDate = newViewModel.ModifiedDate;
        entity.UpdatedById = newViewModel.UpdatedById;

        // Update tags (if applicable)
        entity.Tags.Clear();
        if (newViewModel.SelectedTags != null)
        {
            entity.Tags.AddRange(newViewModel.SelectedTags);
        }

        _repository.Update(entity);
    }

    public void DeleteNewsArticle(NewsArticleViewModel articleViewModel)
    {
        var entity = _repository.GetById(articleViewModel.NewsArticleId);
        entity.Tags.Clear();
        _repository.Delete(entity);
    }

    public List<NewsStatisticViewModel> GetNewsStatistics(DateTime startDate, DateTime endDate)
    {
        var query = _repository.GetAll()
            .Where(a => a.CreatedDate.HasValue &&
                        a.CreatedDate.Value.Date >= startDate.Date &&
                        a.CreatedDate.Value.Date <= endDate.Date);
        return query
            .GroupBy(a => a.CreatedDate.Value.Date)
            .Select(g => new NewsStatisticViewModel
            {
                Date = g.Key,
                TotalCount = g.Count(),
                PublishedCount = g.Count(a => a.NewsStatus == true),
                DraftCount = g.Count(a => a.NewsStatus == false)
            }).ToList();
    }

    public List<NewsArticleViewModel> DisplayHistory(short userId)
    {
        var articles = _repository.ShowHistory(userId);
        return articles.Select(ConvertToViewModel).ToList();
    }

    public async Task SendMailNotification(NewsArticleViewModel viewModel)
    {
        List<SystemAccountViewModel> AdminList = _userManagerService.GetAdmin();

        foreach (var admin in AdminList)
        {
            try
            {
                await _mailUtils.NotificationMailAsync(viewModel, admin.AccountEmail);
                Console.WriteLine($"Notification sent to: {admin.AccountEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {admin.AccountEmail}: {ex.Message}");
            }
        }
    }


    public NewsArticleViewModel ConvertToViewModel(NewsArticle article)
    {
        string updatedName = null;

        if (article.UpdatedBy != null)
        {
            updatedName = article.UpdatedBy.AccountName;
        }

        return new NewsArticleViewModel
        {
            NewsArticleId = article.NewsArticleId,
            NewsTitle = article.NewsTitle,
            NewsContent = article.NewsContent,
            SelectedCategoryId = article.CategoryId,
            Category = article.Category,
            NewsStatus = article.NewsStatus ?? false,
            CreatedDate = article.CreatedDate,
            ModifiedDate = article.ModifiedDate ?? null,
            NewsSource = article.NewsSource,
            Headline = article.Headline,
            CreatedById = article.CreatedById,
            UpdatedById = article.UpdatedById,
            CreatedBy = article.CreatedBy?.AccountName,
            UpdatedBy = updatedName ?? null,
            SelectedTags = (List<Tag>)article.Tags
        };
    }

    public NewsArticle ConvertToEntity(NewsArticleViewModel viewModel)
    {
        return new NewsArticle
        {
            NewsArticleId = viewModel.NewsArticleId,
            NewsTitle = viewModel.NewsTitle,
            NewsContent = viewModel.NewsContent,
            NewsStatus = viewModel.NewsStatus,
            CategoryId = viewModel.SelectedCategoryId,
            CreatedDate = viewModel.CreatedDate,
            ModifiedDate = viewModel.ModifiedDate,
            NewsSource = viewModel.NewsSource,
            Headline = viewModel.Headline,
            CreatedById = viewModel.CreatedById,
            UpdatedById = viewModel.UpdatedById,
            Tags = viewModel.SelectedTags ?? new List<Tag>(),
        };
    }
}
