namespace SimsProject.Serializer
{
    public interface ISerializable
    {
        string[] ToCsv();
        void FromCsv(string[] values);

    }
}
