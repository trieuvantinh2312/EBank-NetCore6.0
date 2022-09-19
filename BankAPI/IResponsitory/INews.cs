using BankModel;

namespace BankAPI.IResponsitory
{
    public interface INews
    {
        public Task<List<News>> ListNews();
        public Task<News> GetNew(int id); 
        public Task<News> PostNews(News news);
        public Task<News> UpdateNews(News news);
    }
}
