namespace TextAnalyticsBot.DataModel.Luis
{
    public class LuisDialogPrompt
    {
        public string Prompt { get; set; }
        public string ParameterName { get; set; }
        public string ContextId { get; set; }
        public string Status { get; set; }
    }
}
