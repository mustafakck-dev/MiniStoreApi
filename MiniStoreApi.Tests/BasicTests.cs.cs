namespace MiniStoreApi.Tests;

public class BasicTests
{
    [Fact] //xUnit’e bunun bir test olduğunu söyler.
    public void Addition_TwoNumbers_ShouldReturnSum()
    {
        // Arrange
        const int firstNumber = 2;
        const int secondNumber = 3;

        // Act
        var result = firstNumber + secondNumber;

        // Assert
        Assert.Equal(5, result);
    }
}