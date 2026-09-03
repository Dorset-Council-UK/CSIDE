using Microsoft.AspNetCore.Components;
using NodaTime;

namespace CSIDE.Web.Helpers
{
    public static class DateInputHelper
    {
        public static void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate?> updateAction)
        {
            var value = eventArgs.Value?.ToString();

            if (string.IsNullOrWhiteSpace(value))
            {
                updateAction(null);
                return;
            }

            try
            {
                var pattern = NodaTime.Text.LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
                var parseResult = pattern.Parse(value);

                if (parseResult.Success)
                {
                    updateAction(parseResult.Value);
                }
            }
            catch (Exception)
            {
                // Problem parsing date, don't update
            }
        }

        public static void UpdateDateProperty(ChangeEventArgs eventArgs, Action<LocalDate> updateAction)
        {
            UpdateDateProperty(eventArgs, (LocalDate? date) =>
            {
                if (date.HasValue)
                {
                    updateAction(date.Value);
                }
            });
        }
    }
}
