namespace Unity.GraphToolkit.Editor.Implementation
{
    class PublicLibraryHelper : ItemLibraryHelper
    {
        public PublicLibraryHelper(GraphModel graphModel) : base(graphModel) { }
        public override IItemDatabaseProvider GetItemDatabaseProvider()
        {
            return m_DatabaseProvider ??= new PublicDatabaseProviderImp(GraphModel);
        }
    }
}
