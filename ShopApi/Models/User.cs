using System.ComponentModel.DataAnnotations;

namespace ShopApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MinLength(3, ErrorMessage = "Deve conter entre 3 e 20 caracteres")]
        [MaxLength(20, ErrorMessage = "Deve conter entre 3 e 20 caracteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MinLength(3, ErrorMessage = "Deve conter entre 3 e 20 caracteres")]
        [MaxLength(20, ErrorMessage = "Deve conter entre 3 e 20 caracteres")]
        public string Password { get; set; }

        public string Role { get; set; }
    }
}
