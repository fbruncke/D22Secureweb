namespace D22Secureweb.Model
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;   //tell the compiler that a null is expected
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;

    }
}
