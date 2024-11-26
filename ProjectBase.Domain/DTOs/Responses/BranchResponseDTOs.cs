namespace ProjectBase.Domain.DTOs.Responses
{
    public class BranchGetListDTO
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public List<UserInBranchResponsesDTO> UserList { get; set; } = [];
    }
    public class BranchCommonResponseDTO
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
