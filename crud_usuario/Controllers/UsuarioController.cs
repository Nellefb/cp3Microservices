using crud_usuario.Entidades;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;



namespace crud_usuario.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {

        private readonly string? _connectionString;

        public UsuarioController(IConfiguration configuration)
        {
            //na conexão colocamos o que colocamos na appsettings
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //conexão 
        private IDbConnection OpenConnection()
        {

            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }



        //método para recuperar usuarios cadastrados, get
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //abre conexão
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, senha from Usuario; ";

            //armazenar resultado da conexão
            var result = await dbConnection.QueryAsync<Usuario>(sql);

            return Ok(result);
        }

        //usando id como parametro para pesquisar
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, senha from Usuario where id = @id; ";

            var result = await dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });

            dbConnection.Close();

            if (result == null)
                return NotFound();

            return Ok(result);

        }


        //inserir usuario
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            using IDbConnection dbConnection = OpenConnection();
            string query = @"INSERT into Usuario(nome,senha)
                VALUES(@Nome, @Senha); ";

            await dbConnection.ExecuteAsync(query, usuario);

            return Ok();
        }


        //editar usuario
        [HttpPut]
        public IActionResult Put([FromBody] Usuario usuario)
        {

            using IDbConnection dbConnection = OpenConnection();

            // Atualiza o usuario
            var query = @"UPDATE Usuario SET 
                          Nome = @Nome,
                          Senha = @Senha
                          WHERE Id = @Id";

            dbConnection.ExecuteAsync(query, usuario);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection dbConnection = OpenConnection();

            var produto = await dbConnection.QueryAsync<Usuario>("delete from usuario where id = @id;", new { id });
            return Ok();
        }
    

}
}