using System.Reflection;
using SharpEngine.Core.Entities;
using Xunit;

namespace SharpEngine.Core.ArchitectureTests;

public class GameObjectTests
{

    [Fact]
    public void GameObjects_Should_Not_Have_Public_Constructors()
    {
        var gameObjectTypes = typeof(GameObject).Assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                typeof(GameObject).IsAssignableFrom(t));

        foreach (var type in gameObjectTypes)
        {
            var publicConstructors = type.GetConstructors(
                BindingFlags.Public | BindingFlags.Instance);

            Assert.Empty(publicConstructors);
        }
    }

}
