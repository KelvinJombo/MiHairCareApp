namespace MiHairCareApp.Application.DTO
{
    public class AvailabilityDto
    {
        public record AvailabilityRequestDto(DateTime Date);

        public record AvailabilityResponseDto(
            string StylistId,
            DateTime Date,
            List<string> WorkingHours,
            List<string> BookedSlots,
            List<string> AvailableSlots
        );


    }
}
