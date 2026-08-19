using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore;

public class CategoryRepository
    : RepositoryBase<Category>, ICategoryRepository
{
    public CategoryRepository(RepositoryContext context)
        : base(context)
    {
    }

    public async Task<List<Category>> GetAllCategoriesAsync(bool trackChanges)
    {
        return await FindAll(trackChanges)
            .OrderBy(category => category.Id)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id,bool trackChanges)
    {
        return await FindByCondition(category => category.Id == id,trackChanges)
            .SingleOrDefaultAsync();
    }

    public void CreateCategory(Category category)
    {
        Create(category);
    }

    public void UpdateCategory(Category category)
    {
        Update(category);
    }

    public void DeleteCategory(Category category)
    {
        Delete(category);
    }
}