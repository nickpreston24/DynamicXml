namespace RegexBuilder
{
    public class Email
    {
        /*
         * Reminder:
         * These will be patterns not for the entire email, nor to split it,
         * but to recognize these individual PARTS of the email!
         */
        [RegexPattern("")] public string User { get; set; } // everything before @
        [RegexPattern("")] public string Provider { get; set; } //everything after @
    }
}