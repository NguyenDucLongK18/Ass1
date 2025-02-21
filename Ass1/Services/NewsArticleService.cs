using Ass1.Models;
using Ass1.Repositories;
using Ass1.ViewModels;
using static Ass1.Services.NewsArticleService;

namespace Ass1.Services
{
    public class NewsArticleService
    {
        private readonly NewsArticleRepository _repository;

        public NewsArticleService(NewsArticleRepository repository)
        {
            _repository = repository;
        }

        public List<NewsArticle> GetPaginatedNews(bool isStaff, int page, int pageSize, out int totalArticles)
        {
            return _repository.GetPaginated(isStaff, page, pageSize, out totalArticles);
        }

        public NewsArticle GetNewsArticle(string id)
        {
            var article = _repository.GetById(id);
            return article == null ? new NewsArticle() : article;
        }

        public void CreateNewsArticle(NewsArticle model)
        {
            _repository.Insert(model);
        }

        public void UpdateNewsArticle(NewsArticle model)
        {
            _repository.Update(model);
        }

        public void DeleteNewsArticle(NewsArticle model)
        {
            _repository.Delete(model);
        }
    }
}
