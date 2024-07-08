using System.ComponentModel.DataAnnotations;

namespace CSProsLibrary.Models;

public class Country
{
    public int Id { get; set; }
    
    [MaxLength(50)]
    
    public required string Name { get; set; }
    
    [MaxLength(5)]
    public required string AbbreviatedName { get; set; }
    public required string CountryImage { get; set; }
}