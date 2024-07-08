namespace CSProsLibrary.Models;

public class Map
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string DemoName { get; set; }
    public required string MapLayoutImage { get; set; }
    public required string ProfileImage { get; set; }
    public int GamesPlayedOnMap { get; set; } = 0;
}