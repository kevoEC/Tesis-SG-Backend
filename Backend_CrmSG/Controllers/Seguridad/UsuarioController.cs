﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Backend_CrmSG.Models.Seguridad;
using Backend_CrmSG.DTOs;
using Backend_CrmSG.Services.Seguridad;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;



namespace Backend_CrmSG.Controllers.Seguridad
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly StoredProcedureService _storedProcedureService;
        private readonly IJwtService _jwtService;
        public UsuarioController(IUsuarioService usuarioService, IJwtService jwtService, StoredProcedureService storedProcedureService)
        {
            _usuarioService = usuarioService;
            _storedProcedureService = storedProcedureService;
            _jwtService = jwtService;
        }

        // POST: api/Usuario/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _storedProcedureService.EjecutarLoginSP(loginRequest.Email, loginRequest.Contraseña);

            if (result.Usuario == null)
                return Unauthorized("Credenciales inválidas o usuario inactivo.");

            var claims = new List<Claim>
    {
        new Claim("idUsuario", result.Usuario.Id.ToString()),
        new Claim(ClaimTypes.Email, result.Usuario.Email),
        new Claim(JwtRegisteredClaimNames.Sub, result.Usuario.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var rol in result.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var token = _jwtService.GenerateTokenFromClaims(claims);
            var permisosAgrupados = result.Permisos
            .GroupBy(p => new { p.Menu, p.Nombre, p.Ruta, p.Icono })
            .Select(g => new
            {
                Menu = g.Key.Menu,
                Nombre = g.Key.Nombre,
                Ruta = g.Key.Ruta,
                Icono = g.Key.Icono,
                Permisos = g.Select(p => p.Permiso).ToList()
            }).ToList();



            return Ok(new
            {
                Token = token,
                Usuario = new
                {
                    Id = result.Usuario.Id,
                    Nombre = result.Usuario.NombreCompleto,
                    Correo = result.Usuario.Email,
                    Identificacion = result.Usuario.Identificacion
                },
                Roles = result.Roles,
                Permisos = permisosAgrupados
            });
        }




        // GET: api/Usuario/roles/5
        [HttpGet("roles/{idUsuario}")]
        public async Task<IActionResult> GetRoles(int idUsuario)
        {
            var roles = await _usuarioService.GetRolesByUserIdAsync(idUsuario);
            return Ok(roles);
        }

        // GET: api/Usuario/menus/5
        [HttpGet("menus/{idUsuario}")]
        public async Task<IActionResult> GetMenus(int idUsuario)
        {
            var menus = await _usuarioService.GetMenusByUserIdAsync(idUsuario);
            return Ok(menus);
        }

        // CRUD Básico (Opcional)
        // GET: api/Usuario
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            await _usuarioService.AddAsync(usuario);
            return CreatedAtAction(nameof(GetById), new { id = usuario.IdUsuario }, usuario);
        }

        // PUT: api/Usuario/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest("El ID no coincide.");

            await _usuarioService.UpdateAsync(usuario);
            return NoContent();
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _usuarioService.DeleteAsync(id);
            return NoContent();
        }
    }
}
