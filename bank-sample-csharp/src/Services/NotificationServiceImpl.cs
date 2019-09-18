using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using Tokenio.Proto.Common.NotificationProtos;
using Tokenio.Proto.Common.SubscriberProtos;
using Tokenio.Sdk.Api.Service;

namespace Tokenio.BankSample.Services
{
    public class NotificationServiceImpl: INotificationService
    {
        private static readonly ILog logger = LogManager
            .GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MemoryCache notificationStore;

        public NotificationServiceImpl()
        {
            this.notificationStore = new MemoryCache(new MemoryCacheOptions());

        }

        /// <summary>
        /// Retrieves a notification, for testing purposes.
        /// </summary>
        /// <param name="subscriberId">subscriberId to pull notifications for.</param>
        /// <returns>Notification that was found</returns>
        public Notification WaitForNotification(string subscriberId)
        {
            return (Notification)notificationStore.Get(subscriberId);
        }

        public void Notify(
            Notification notification,
            Subscriber subscriber)
        {
            var infoLog = "Sending notification " + notification + " " + subscriber;
            logger.Info(infoLog);

            IDictionary<string, string> instructions = subscriber.HandlerInstructions;

            // Store the notification, for testing purposes
            notificationStore.Set(subscriber.Id, notification);


            // Instructions can be customized. It's entirely up the the creator of the subscriber (bank)
            // which keys to use in the instructions.
            switch (instructions["platform"])
            {
                case "ANDROID":
                    string notificationToken = instructions.ContainsKey("android-token")
                        ? instructions["android-token"] : null;
                    // Send notification to android app
                    // AndroidNotifier.sendNotification(body, notificationToken);
                    break;
                case "IOS":
                    string pushToken = instructions.ContainsKey("push-token")
                        ? instructions["push-token"] : null;
                    // Send notification to IOS app
                    // IOSNotifier.sendNotification(body, pushToken);
                    break;
                case "IRON-BANK-BACKEND-SERVER":
                    string url = instructions.ContainsKey("url")
                        ? instructions["url"] : null;
                    // Send a request to some other system
                    // HttpNotifier.sendRequest(body, url);
                    break;
                default:
                    throw new SystemException("Invalid platform");
            }

        }
    }
}