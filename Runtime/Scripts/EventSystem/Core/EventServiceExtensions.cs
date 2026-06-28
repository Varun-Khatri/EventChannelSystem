namespace VK.Events
{
    public static class EventServiceExtensions
    {
        public static void PublishOptional(
            this IEventService eventService,
            int eventId,
            bool sendParam,
            int param)
        {
            if (eventService == null)
                return;

            if (sendParam)
                eventService.Publish(eventId, param);
            else
                eventService.Publish(eventId);
        }
    }
}