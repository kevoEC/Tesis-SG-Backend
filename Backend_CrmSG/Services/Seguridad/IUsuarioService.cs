﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Backend_CrmSG.Models.Seguridad;

namespace Backend_CrmSG.Services.Seguridad
{
    public interface IUsuarioService
    {
        // Autenticación
        Task<Usuario?> AuthenticateAsync(string email, string password);

        // Obtiene roles de un usuario
        Task<IEnumerable<string>> GetRolesByUserIdAsync(int idUsuario);

        // Obtiene menús según roles (podrías devolver objetos de tipo Menu)
        Task<IEnumerable<object>> GetMenusByUserIdAsync(int idUsuario);

        // CRUD básico (opcional)
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario> GetByIdAsync(int id);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
        Task<Usuario?> BuscarPorCorreoAsync(string email);

    }
}
