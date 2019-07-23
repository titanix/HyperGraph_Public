namespace Leger.Extra.SqlBinding
{
    public interface IDatabaseCreator
    {
        void CreateDatabase();
        void PopulateDatabase(IGraph data);
    }
}