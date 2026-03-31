namespace InsuranceApp.Domain.Common.Interfaces;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}