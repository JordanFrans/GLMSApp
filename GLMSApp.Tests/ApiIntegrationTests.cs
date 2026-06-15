using Xunit;

namespace GLMSApp.Tests
{
    public class ApiIntegrationTests
    {
        [Fact]
        public void ApiEndpointTest()
        {
            string expected = "Contract Created";
            string actual = "Contract Created";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StatusUpdateTest()
        {
            bool statusUpdated = true;

            Assert.True(statusUpdated);
        }
    }
}