using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RHOnline.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O Campo 'Login' é Obrigatório")]
        [MaxLength(50, ErrorMessage = "O login deve conter menos que 50 caracteres!")]
        public string CPF { get; set;}

        [Required(ErrorMessage ="O Campo 'Senha' é Obrigatório")]
        [MinLength(8, ErrorMessage = "A senha deve ser maior que 8 caracteres!")]
        [MaxLength(21, ErrorMessage = "A senha deve conter menos que 20 caracteres!")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O Campo 'Nome' é Obrigatório")]
        [MaxLength(50,ErrorMessage ="O Campo 'Nome' deve ter menos que 50 caracteres")]
        public string Nome { get; set; }
        
        [Phone(ErrorMessage = "Insira um número de telefone válido")]
        public string Telefone { get; set; }

        [Phone(ErrorMessage = "Insira um numero de telefone válido")]
        public string Celular { get; set; }

        [EmailAddress(ErrorMessage ="Insira um email válido!")]
        public string Email { get; set; }


        //Nivel 1: Administrador;
        //Nível 2: Usuário;
        //Nivel 3: Suporte (TI);
        public byte Nivel { get; set; }
       
        public DateTime DataCadastro { get; set; }

        public DateTime UltimoAcesso { get; set; }

        public DateTime DataAdmissao { get; set; }

        public Loja Loja { get; set; }
        

        // 0  -> Não
        // 1  -> Sim
        public byte Ativo { get; set; }

        //Código Gerado
        public int CodigoAtivacao { get; set; }

        // 0  -> Não
        // 1  -> Sim
        public byte Cadastrado { get; set; }

        //Verificação de Email 1: Verificado, 0: Não Verificado
        public byte Verificado { get; set; }
        
        //0: Não envia alertas de holerites
        //1: Envia Alertas de holerites
        public byte Alertas { get; set; }


    }
}
