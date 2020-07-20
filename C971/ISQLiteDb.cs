using SQLite;

namespace C971
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}