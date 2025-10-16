namespace E_PharmaHub.Dtos
{
    public class ThreadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<ParticipantDto> Participants { get; set; }
        public LastMessageDto LastMessage { get; set; }
    }
}
