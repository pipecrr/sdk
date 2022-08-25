namespace Siesa.SDK.Shared.Criptography
{
    public interface ISDKJWT
    {
        public string Generate<T>(T obj, long min);
        public T Validate<T>(string token);
    }
}