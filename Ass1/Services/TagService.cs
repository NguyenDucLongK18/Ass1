using Ass1.Models;
using Ass1.Repositories;
using Ass1.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Ass1.Services
{
    public class TagService
    {
        private readonly TagRepository _tagRepository;

        public TagService(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public List<Tag> GetAllTags() =>
            _tagRepository.GetAll();

        public List<Tag> GetByIds(List<int> ids)=>
            _tagRepository.GetTagsByIds(ids);

        public Tag GetTagById(int id)
        {
            var tag = _tagRepository.GetById(id);
            return tag;
        }

        public void AddTag(Tag tag)
        {
            _tagRepository.Insert(tag);
        }

        public void UpdateTag(Tag tag)
        {
            _tagRepository.Update(tag);
        }

        public void DeleteTag(int id)
        {
            var tag = _tagRepository.GetById(id);
            if (tag != null)
            {
                _tagRepository.Delete(tag);
            }
        }
    }
}
