namespace final_task_api.models
{
    public class QuestionModel
    {
        public int? Id { get; set; }
        public string Question { get; set; }
        public int? Required { get; set; }
        public List<AnswerModel> Answers { get; set; }
    }
}
