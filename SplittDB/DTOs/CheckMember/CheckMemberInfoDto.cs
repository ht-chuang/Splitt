namespace SplittDB.DTOs.CheckMember
{
    public class CheckMemberInfoDto
    {
        public int? Id { get; set; } = null!;

        public string? Name { get; set; } = null!;

        public int? CheckId { get; set; } = null!;

        public int? UserId { get; set; } = null!;

        public decimal? AmountOwed { get; set; } = null!;
    }
}