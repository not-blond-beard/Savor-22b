using GraphQlClient.EventCallbacks;
using Newtonsoft.Json.Linq;

public static class SocketDataReceiver{
    public delegate void SubscriptionDataHandler(OnSubscriptionDataReceived subscriptionDataReceived);

    static public Event<OnSubscriptionDataReceived>.EventListener Receiver(string socketId, SubscriptionDataHandler handler){
        return (OnSubscriptionDataReceived subscriptionDataReceived) => {
            JObject json = JObject.Parse(subscriptionDataReceived.data);
            string id = json["id"]?.ToString();

            if (id is null || id == socketId) {
                handler(subscriptionDataReceived);
            }
        };
    }
}