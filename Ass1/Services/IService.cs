namespace Ass1.Services
{
    public interface IService<TEntity, TViewModel>
    {
        TViewModel ConvertToViewModel(TEntity entity);
        TEntity ConvertToEntity(TViewModel viewModel);
    }
}
