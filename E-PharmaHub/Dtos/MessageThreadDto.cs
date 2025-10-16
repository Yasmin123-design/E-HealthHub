namespace E_PharmaHub.Dtos
{
    public class MessageThreadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> ParticipantIds { get; set; }
    }
}
