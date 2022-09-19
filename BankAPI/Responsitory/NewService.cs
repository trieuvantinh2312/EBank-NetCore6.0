using BankAPI.DataContext;
using BankAPI.IResponsitory;
using BankModel;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Responsitory
{
    public class NewService : INews
    {
        public ApplicationDbContext db;
        public NewService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<News> GetNew(int id)
        {
            return await db.News.FindAsync(id);
        }

        public async Task<List<News>> ListNews()
        {
            return await db.News.ToListAsync() ;
        }

        public async Task<News> PostNews(News news)
        {
            await db.News.AddAsync(news);
            await db.SaveChangesAsync();
            return news;
        }

        public async Task<News> UpdateNews(News news)
        {
            var model = await db.News.FindAsync(news.NewId);
            model.Description = news.Description;
            model.Title = news.Title;
            model.ImageSlide = news.ImageSlide;
            model.ImageMain = news.ImageMain;
            model.Status = news.Status;
            await db.SaveChangesAsync();
            return model;
        }
    }
}
