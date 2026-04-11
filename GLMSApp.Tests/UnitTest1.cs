using Xunit;

namespace GLMSApp.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CurrencyConversionTest()
        {
            decimal amount = 10;
            decimal rate = 18;

            decimal result = amount * rate;

            Assert.Equal(180, result);
        }

        [Fact]
        public void FileValidationTest()
        {
            string fileName = "virus.exe";

            bool isValid = !fileName.EndsWith(".exe");

            Assert.False(isValid);
        }
    }
}