using System;
using CookBook.App.Services;
using CookBook.BL.Interfaces;
using CookBook.BL.Repositories;
using CookBook.BL.Repositories.Obsolete;
using CookBook.BL.Services;
using CookBook.DAL.Factories;
using Microsoft.Extensions.Configuration;

namespace CookBook.App.ViewModels
{
    public class ViewModelLocator
    {
        private readonly IIngredientRepository ingredientRepository;
        private readonly IMediator mediator;
        private readonly IMessageBoxService messageBoxService;
        private readonly IRecipeRepository recipesRepository;
        private IConfigurationRoot Configuration;

        public ViewModelLocator()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
            mediator = new Mediator();
            IDbContextFactory dbContextFactory = new DbContextFactory(Configuration.GetConnectionString("DefaultConnection"));
#if DEBUG
            using (var dbx = dbContextFactory.CreateDbContext())
            {
                dbx.Database.EnsureCreated();
            }
#endif

            ingredientRepository = new IngredientRepository(dbContextFactory);
            recipesRepository = new RecipeRepository(dbContextFactory);
            messageBoxService = new MessageBoxService();
        }


        public IngredientListViewModel IngredientListViewModel => new IngredientListViewModel(ingredientRepository, mediator);

        public RecipesListViewModel RecipeListViewModel => new RecipesListViewModel(recipesRepository, mediator);

        public IngredientDetailViewModel IngredientDetailViewModel =>
            new IngredientDetailViewModel(ingredientRepository, messageBoxService, mediator);

        public RecipeDetailViewModel RecipeDetailViewModel => new RecipeDetailViewModel(recipesRepository, messageBoxService, mediator);

        public IngredientAmountDetailViewModel IngredientAmountDetailViewModel =>
            new IngredientAmountDetailViewModel(ingredientRepository, mediator);
    }
}