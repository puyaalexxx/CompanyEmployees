using CompanyEmployees.Models;

namespace CompanyEmployees.Service;

public class PlayerGenerator : IPlayerGenerator
{
    private readonly string[] _maleNames = { "Jon Irenicus", "Kagain", "Minsc", "Xzar", "Drizzt Do'Urden" };
    private readonly string[] _femaleNames = { "Imoen", "Jaheira", "Dynaheir", "Branwen", "Bodhi" };
    
    public Player CreateNewPlayer()
    {
        string playerName;
        
        var random = new Random();
        
        var playerNameIndex = random.Next(7);
        var playerGenderIndex = random.Next(2);
        var playerHairColorIndex = random.Next(7);
        var playerAge = random.Next(18, 100);
        var strength = random.Next(8, 18);
        
        if (playerGenderIndex is 0)
            playerName = _maleNames[playerNameIndex];
        else
            playerName = _femaleNames[playerNameIndex];
        
        return new Player
        {
            Name = playerName,
            Gender = (Gender)playerGenderIndex,
            HairColor = (HairColor)playerHairColorIndex,
            Age = playerAge,
            Strength = strength,
            Race = "Human"
        };
    }
}