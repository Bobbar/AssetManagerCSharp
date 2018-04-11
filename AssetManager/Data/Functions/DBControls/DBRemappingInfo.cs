namespace AssetManager.Data.Functions
{
    public struct DBRemappingInfo
    {
        public string FromColumnName { get; set; }
        public string ToColumnName { get; set; }

        public DBRemappingInfo(string fromColumn, string toColumn)
        {
            FromColumnName = fromColumn;
            ToColumnName = toColumn;
        }
    }
}