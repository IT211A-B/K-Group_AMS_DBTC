namespace Backend.Backend.DTOs
{
    public class ResponseDTO<T>
    {
        public int Status_code { get; set; }
        public T? Data { get; set; }
        public string? Detail {get; set;}
    }
}