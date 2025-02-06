using Plugin.Firebase.Functions;

namespace Spots.CloudFunctions
{
    public static class CloudFunctionsManager
    {
        private const string TRIGGER_NOTIFICATION = "TRIGGER_NOTIFICATION";
        public static Task CallNotificationFunction(string dataJson)
        {
            var x = CrossFirebaseFunctions.Current
                .GetHttpsCallable(TRIGGER_NOTIFICATION);
            return CrossFirebaseFunctions.Current
                .GetHttpsCallable(TRIGGER_NOTIFICATION)
                .CallAsync(dataJson);
        }
    }
}
