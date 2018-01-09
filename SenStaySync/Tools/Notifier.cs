namespace SenStaySync
{
    using System;

    public class Notifier
    {
        public INotifier NotifierDriver;


        public Notifier(INotifier notifier = null)
        {
            if (notifier != null)
            {
                NotifierDriver = notifier;
            }
        }

        public void Note(object message)
        {
            if (message != null && NotifierDriver != null)
            {
                NotifierDriver.Note(message);
            }
        }
    }

    public static class N
    {
        public static Notifier Instance;

        public static void Note(object Message)
        {
            if (Instance == null)
            {
                INotifier notifier;
                try
                {
                    notifier = (INotifier) Activator.CreateInstance(Type.GetType(Config.I.NotiferClass));
                }
                catch
                {
                    notifier = new SilentNotifer();
                }
                Instance = new Notifier(notifier);
            }

            if (Instance != null)
            {
                Instance.Note(Message);
            }
        }
    }


    public interface INotifier
    {
        void Note(object Message);
    }

    public class ConsoleNotifier : INotifier
    {
        public void Note(object Message)
        {
            try
            {
                if (Message != null)
                {
                    Console.WriteLine(Message + "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Notifier exception: " + ex);
            }
        }
    }

    public class SilentNotifer : INotifier
    {
        public void Note(object Message)
        {
            // Exists to do nothing
        }
    }
}