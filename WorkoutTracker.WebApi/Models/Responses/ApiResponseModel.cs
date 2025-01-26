namespace WorkoutTracker.WebApi.Models.Responses
{
    public class ApiResponseModel<TData>
    {
        public required bool Success { get; set; }

        public required string Message { get; set; }

        public TData? Data { get; set; }
    }
}
